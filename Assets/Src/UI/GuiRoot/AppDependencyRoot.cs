using Assets.Src;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using Assets.Src.Scenes;
using Assets.Src.Server;
using ColonyDI;
using Infrastructure.Cloud;
using Infrastructure.Config;
using ResourceSystem.Reactions;
using SharedCode.EntitySystem;
using SharedCode.Serializers.Protobuf;
using SharedCode.Utils.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;
using Uins;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppDependencyRoot : MonoBehaviour, IDependencyNode
{
    public SurvivalGuiNode SurvivalGui;

    public GameState GameState;

    public WindowStackId[] UsedWindowStackIds;

    private readonly List<IDependencyNode> _children = new List<IDependencyNode>();

    [SerializeField]
    private MapRootRef MapRoot;

    //=== Props ===============================================================

    private WindowsManager _orgWindowsManager;

    private WindowsManager OrgWindowsManager => _orgWindowsManager ?? (_orgWindowsManager = new WindowsManager(UsedWindowStackIds));

    public IEnumerable<IDependencyNode> Children => _children;

    public IDependencyNode Parent { get; set; }


    //=== Unity ===============================================================

    private void Awake()
    {
        if (GameState.AssertIfNull(nameof(GameState)))
        {
            UI.Logger.IfError()?.Message($"<{nameof(AppDependencyRoot)}>: empty or wrong", gameObject).Write();
        }
    }

    private void OnDestroy()
    {
        _orgWindowsManager?.Dispose();
    }


    //=== Public ==============================================================

    public async Task MainGameProcessInf(Func<CancellationToken,Task> backgroundInit, IEnumerable<string> scenes, InnerProcess inner, CancellationToken ct)
    {
        var waitFor = backgroundInit(ct);

        Log.StartupStopwatch.Milestone("MainGameProcessInf");
        CheckMissingScenes(scenes);
        Log.StartupStopwatch.Milestone("CheckMissingScenes DONE!");
        
        await InitDependencyInjection();
        Log.StartupStopwatch.Milestone("InitDependencyInjection DONE!");

        if (await TryStartAsBot(inner, ct))
            return;
        Log.StartupStopwatch.Milestone("TryStartAsBot DONE!");

        var startupLogic = await LoadScenes(ct);
        await UnityQueueHelper.RunInUnityThread(ScenesLoader.ClearAfterSceneLoading);
        await startupLogic.DoStart(waitFor, inner, ct);
    }

    private async Task<IStartupLogic> LoadScenes(CancellationToken ct)
    {
        Log.StartupStopwatch.Milestone("Start Load Scenes");
        
        var objectsToInject = new List<IDependencyNode>();
        var dependencyScenes = MapRoot.Target.KeyDependencyScenes.Where(v => ScenesLoader.IsSceneInBuildSettings(v.Path));
        foreach (var scene in dependencyScenes)
        {
            await ScenesLoader.LoadScenesAsync(Enumerable.Repeat(scene.Path, 1));
            Log.StartupStopwatch.Milestone(scene.Path);
            var nodes = await UnityQueueHelper.RunInUnityThread(() => LinkScene(scene.Path));
            var dependencyNodes = nodes as IDependencyNode[] ?? nodes.ToArray();
            Log.StartupStopwatch.Milestone("LinkScene done!");

            var intermediateLogics = dependencyNodes.OfType<IStartupIntermediateLogic>();
            var intermediateLogic = intermediateLogics.SingleOrDefault();
            if (intermediateLogic != null)
            {
                try
                {
                    if (await intermediateLogic.CanStart())
                        await intermediateLogic.DoLogic(ct);
                } catch (Exception e) {                
                    UI.Logger.Error(e, "Exception in intermediate logic of scene {0}", scene.Path);
                }
                if (intermediateLogic.MustBeUnloadedAfterLogic)
                    await ScenesLoader.UnloadScenesAsync(new[]{scene.Path});
            }
        
            var logic = dependencyNodes.OfType<IStartupLogic>();
            var startupLogic = logic as IStartupLogic[] ?? logic.ToArray();
            if (startupLogic.Skip(1).Any())
                UI.Logger.Error(
                    "Multiple {0} components in scene {1}, order of execution is undefined",
                    typeof(IStartupLogic).NiceName(),
                    scene.Path
                );
            if (!startupLogic.Any())
            {
                objectsToInject.AddRange(dependencyNodes);
                continue;
            }

            //встречена сцена логики, линкуем подгруженные сцены и останавливаем дальнейшую загрузку, сцена логики берет управление
            await UnityQueueHelper.RunInUnityThread(
                () => DependencyInjection.LinkExtraChildren(dependencyNodes.Concat(objectsToInject), this));
            objectsToInject.Clear();

            IStartupLogic nodeToStart = null;
            foreach (var node in startupLogic)
            {
                if (await node.CanStart())
                {
                    nodeToStart = node;
                    break;
                }
            }

            if (nodeToStart == null)
                continue;

            return nodeToStart;
        }

        throw new InvalidOperationException("No startup logic found");
    }

    private static async Task<bool> TryStartAsBot(InnerProcess inner, CancellationToken ct)
    {
        if (await ServerDependencyNode.CanStart())
        {
            await ServerDependencyNode.Start(inner, ct);
            return true;
        }

        return false;
    }

    private async Task InitDependencyInjection()
    {
        await UnityQueueHelper.RunInUnityThread(
            () =>
            {
                RegisterBuiltinNodes();
                DependencyInjection.InjectRoot(this);
            });
    }

    private void CheckMissingScenes(IEnumerable<string> scenes)
    {
        var missingScenes = MapRoot.Target.KeyDependencyScenes.Where(v => !scenes.Contains(v.Path) && !v.Optional);
        foreach (var scene in missingScenes)
            UI. Logger.IfError()?.Message("Missing scene {0}",  scene.Path).Write();
    }

    public void AfterDependenciesInjected()
    {
    }

    public void AfterDependenciesInjectedOnAllProviders()
    {
    }


    //=== Private =============================================================

    private void RegisterBuiltinNodes()
    {
        _children.Add(OrgWindowsManager);
        _children.Add(GameState);
        _children.Add(SurvivalGui);
    }

    private IEnumerable<IDependencyNode> LinkScene(string scenePath)
    {
        var nodes = GetNodesFromScene(scenePath).ToArray();
        _children.AddRange(nodes);
        return nodes;
    }

    private IEnumerable<IDependencyNode> GetNodesFromScene(string scenePath)
    {
        var scene = SceneManager.GetSceneByPath(scenePath);

        GameObject[] sceneRootGameObjects = scene.GetRootGameObjects();
        if (!sceneRootGameObjects.Any())
        {
            UI.Logger.Error(
                $"Unable to get components cause root gameObjects are empty " +
                $"(last loaded scene '{scene.name}')",
                gameObject);
            return Enumerable.Empty<IDependencyNode>();
        }

        return sceneRootGameObjects.SelectMany(v => v.GetComponents<IDependencyNode>());
    }

    [Serializable]
    public class MapRootRef : JdbRef<MapRootDef>
    {
    }
}