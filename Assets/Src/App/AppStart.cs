using System;
using ResourcesSystem.Loader;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using ColonyShared.SharedCode.Utils;
using SharedCode.Entities.GameObjectEntities;
using Assets.Src.Shared.Impl;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using UnityEngine.AI;
using System.Threading;
using Assets.Src.Arithmetic;
using Assets.Src.Scenes;
using ColonyShared.GeneratedCode;
using ReactivePropsNs;
using SharedCode.Utils.Threads;
using SharedCode.MovementSync;
using UnityEngine.Profiling;
using SharedCode.Aspects.Item.Templates;
using ResourcesSystem;
using ColonyShared.SharedCode;
using GeneratedCode.DeltaObjects;
using Infrastructure.Cloud;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.App.QualitySettings;
using Assets.Src.ResourcesSystem.Base;
using Uins.Settings;
using Core.Environment.Logging;
using Core.Environment.Logging.Extension;
using Core.Environment.Resources;
using LoggerExtensions;
using ResourceSystem.Reactions;
using SharedCode.EntitySystem;
using SharedCode.Logging;
using SharedCode.Serializers.Protobuf;
using Src.Debugging;
using Uins;
using System.Linq;
using Assets.Src.App;

public class AppStart : MonoBehaviour
{
    private const int RestartRequiredExitCode = 3;
    private const int RestartAndNoAutoplayRequiredExitCode = 4;
    private const int PzApiConnectionTimeoutMilliseconds = 10000;

    [NotNull]
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    [SerializeField, UsedImplicitly]
    private AppDependencyRoot AppDependencyRoot;

    private DisposableComposite D = new DisposableComposite();

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly Task _mainProcess = Task.CompletedTask;

    //=== Props ===============================================================

    //=== Unity ===============================================================

    [UsedImplicitly]
    private void Awake()
    {
        ///#DANGER! Uncomment only locally - DON'T COMMIT uncommented
        // if (DbgLog.Enabled) 
        //     DevelopLobbyGuiNode.Init_Log_System();

        System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
        w.Start();
        Physics.autoSyncTransforms = false;
        NavMesh.pathfindingIterationsPerFrame = 100000000;
        NavMesh.avoidancePredictionTime = 0.05f;
        SyncTime.StartUpdating(DateTime.UtcNow.Ticks);

        if (DebugState.Instance.InitLogSystemOnStart || PlayerPrefs.GetString("Logger") == "Enabled")
        {
            Debug.LogWarning("LOG SYSTEM ENABLED BY PLAYER PREFS");
            DevelopLobbyGuiNode.Init_Log_System();
        }
        else if (Environment.GetCommandLineArgs().Contains("-enablelogsystem"))
        {
            Debug.LogWarning("LOG SYSTEM ENABLED BY CMDLINE ARG");
            DevelopLobbyGuiNode.Init_Log_System();
        }

        Log.StartupStopwatch.Start();

        D.Add(new VideoSettings());
        //TypesCollector.CollectTypes(TypesToCollect.Agents);
        ValueConverterInitialized.InitValueConverters();
        var unitySpawnService = new UnitySpawnService();
        GameObjectCreator.ClusterSpawnInstance = unitySpawnService;
        EntitytObjectsUnitySpawnService.SpawnService = GameObjectCreator.ClusterSpawnInstance;

        AppDependencyRoot.AssertIfNull(nameof(AppDependencyRoot));

        var gameResourcesPath = ResourceSystemInit.ResourceRootDefaultLocation;
        var gameResources = new GameResources(new FolderLoader(gameResourcesPath.FullName));
        gameResources.ConfigureForUnityRuntime();
        gameResources.CreateNetIDs();
        Log.StartupStopwatch.Milestone("CreateNetIDs done");
        GameResourcesHolder.Instance = gameResources;

        Constants.WorldConstantsRef = CmdArgumentHelper.GetConstantsParams(Constants.WorldConstantsRef);

        CmdArgumentHelper.FillStartupParams(StartupParams.Instance);
        ProfilerProxy.Profiler = new UnityProfiler();
        Plugins.DebugDrawingExtension.DebugDraw.Manager.Initialize();

        // Inits (don't forget about `ShutDowns` also):
        // Inits, that could be placed at Awake of their scene, better place there
        GraphicSettingsViewModel.Init(this);
        QualitySimpleSetterViewModel.Init();

        w.Stop();
        Logger.IfInfo()?.Message($"App start finished in {w.Elapsed.TotalSeconds}").Write();
        Log.StartupStopwatch.Milestone("AppStart Awake DONE!");
        //DevelopLobbyGuiNode.Init_Log_System(); //DEBUG если нужно посмотреть ошибки на старте
    }

    class UnityProfiler : IUnityProfilerImpl
    {
        public void BeginSample(string label)
        {
            Profiler.BeginSample(label);
        }

        public void EndSample()
        {
            Profiler.EndSample();
        }
    }

    [UsedImplicitly]
    private void Start()
    {
        GameState.Instance.IsInGameRp.Skip(D, 1)
            .Action(
                D,
                inGame =>
                {
                    if (!inGame)
                    {
                        Pool.DumpWithOverflow();
                        Pool.DumpWithDefaultCapacity();
                    }
                }); // дампать надо именно при выходе из сессии, а не из игры (когда зарелизятся вообще все объекты из пулов).  

        Task BackgroundInit(CancellationToken ct)
        {
            var res = TaskEx.Run(() => GameResourcesHolder.Instance.LoadAllResources(), ct);
            var types = TaskEx.Run(
                () => TypesCollector.CollectTypes(),
                ct); // при сборке бингингов может происходить массовая загрузка jdb и в результате всё это тоже занимает время
            var protoBuf = TaskEx.Run(
                () => ProtoBufSerializer.TryInit(new[] {typeof(IEntity).Assembly, typeof(IResource).Assembly, typeof(ArgDef).Assembly}),
                ct);

            var pzApi = TaskEx.Run(
                async () => { await PzApiConnect(); },
                ct);

            return Task.WhenAll(res, types, protoBuf, pzApi);
        }

        var scenes = ScenesInBuild.Scenes;
        AsyncProcess main = (inner, ct) => AppDependencyRoot.MainGameProcessInf(BackgroundInit, scenes, inner, ct);
        AsyncProcess quit = (inner, ct) => QuitGame(D, inner, ct);
        TaskEx.Run(() => quit.Then(main).Exec(_cts.Token)).WrapErrors();
    }

    private static async Task PzApiConnect()
    {
        try
        {
            var connect = PzApiHolder.Communicator.Connect();
            var timeout = Task.Delay(TimeSpan.FromMilliseconds(PzApiConnectionTimeoutMilliseconds));
            var task = await Task.WhenAny(connect, timeout);
            if (task == connect)
                PzApiHolder.Connected = await connect;
        }
        catch (Exception e)
        {
            Logger.IfError()?.Message("Communicator Connection Error").Exception(e).Write();
        }
    }

    private static async Task QuitGame(DisposableComposite d, InnerProcess inner, CancellationToken ct)
    {
        try
        {
            await inner(ct);
        }
        catch (Exception e)
        {
            Logger.IfFatal()?.Exception(e).Write();
        }
        finally
        {
            Logger.IfFatal()?.Message("Exiting").Write();

            SharedCode.Utils.DebugCollector.Collect.Instance.Dispose();
            d.Clear();

            await UnityQueueHelper.RunInUnityThread(Application.Quit);
#if UNITY_EDITOR
            await UnityQueueHelper.RunInUnityThread(() => UnityEditor.EditorApplication.isPlaying = false);
#endif
        }
    }

    [UsedImplicitly]
    public void Update()
    {
        SharedCode.Utils.DebugCollector.Collect.IfActive?.Event("AppStart.Update");
        SyncTime.NewTime(DateTime.UtcNow.Ticks);
    }

    [UsedImplicitly]
    public void FixedUpdate()
    {
        SharedCode.Utils.DebugCollector.Collect.IfActive?.Event("AppStart.FixedUpdate");
        SyncTime.NewTime(DateTime.UtcNow.Ticks);
    }

    private void OnDestroy()
    {
        // ShutDowns (don't forget about `Inits` also):
        // ShutDowns, that could be placed at OnDestroy of their scene, better place there
        QualitySimpleSetterViewModel.ShutDown();
        GraphicSettingsViewModel.ShutDown();

        AsyncProcessExtensions.ShutdownProcess(_cts, _mainProcess).WrapErrors();
    }

    public static void Restart()
    {
        UnityQueueHelper.RunInUnityThread(() => Application.Quit(RestartRequiredExitCode));
    }

    public static void RestartAndNoAutoplay()
    {
        UnityQueueHelper.RunInUnityThread(() => Application.Quit(RestartAndNoAutoplayRequiredExitCode));
    }

    public static void Quit()
    {
        UnityQueueHelper.RunInUnityThread(Application.Quit);
    }
}