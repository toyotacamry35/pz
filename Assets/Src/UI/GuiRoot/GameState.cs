using System;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared.Arithmetic;
using Assets.ColonyShared.SharedCode.Arithmetic;
using Assets.Src.App.Common;
using Assets.Src.Client;
using GeneratedCode.Custom.Config;
using Infrastructure.Config;
using JetBrains.Annotations;
using NLog;
using SharedCode.Config;
using SharedCode.EntitySystem;
using UnityEngine;
using Assets.Src.BuildingSystem;
using Assets.Src.Shared.Impl;
using Assets.Src.ResourceSystem;
using Assets.Src.Cartographer;
using System.Threading;
using Assets.Src.App;
using Assets.Src.ResourcesSystem.Base;
using Infrastructure.Cloud;
using ReactivePropsNs;
using Assets.Src.Telemetry;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Sessions;
using SharedCode.Repositories;

public class GameState : DependencyEndNode
{
    [NotNull]
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    [SerializeField]
    public StaticDefsHolder StaticDefsHolder;

    [SerializeField, UsedImplicitly]
    private GameOptions _gameOptions;

    [SerializeField, UsedImplicitly]
    public EntitiesRepositoryConfigRef DefaultConfigDef;

    [SerializeField, UsedImplicitly]
    public LoginServerConfigRef LoginServer;

    private CancellationTokenSource _connectCts;
    private Task _connectProcess = Task.CompletedTask;

    private readonly CancellationTokenSource _ownCts = new CancellationTokenSource();

    private TelemetrySystem _telemetrySystem;
    private BuildSystem _buildSystem;
    private SceneStreamerSystem _sceneStreamerSystem;

    //=== Props ===============================================================

    public static GameState Instance { get; private set; }
    public string NetIssueText { get; set; }
    public GameOptions GameOptions => _gameOptions;

    public CharacterRuntimeData CharacterRuntimeData { get; set; }

    public MapDef CurrentMap { get; set; }

    public ReactiveProperty<bool> IsInGameRp = new ReactiveProperty<bool>();

    public TelemetrySystem TelemetrySystem => _telemetrySystem;

    public BuildSystem BuildSystem => _buildSystem;

    public SceneStreamerSystem SceneStreamerSystem => _sceneStreamerSystem;

    private IEntitiesRepository _clientClusterNode;

    public IEntitiesRepository ClientClusterNode
    {
        get => _clientClusterNode;
        set
        {
            _clientClusterNode = value;
            _clientClusterRepositoryRp.Value = value;
        }
    }

    public ReactiveProperty<string> PlatformApiTokenRp { get; } = new ReactiveProperty<string>();

    private readonly ReactiveProperty<Guid> _accountIdRp = new ReactiveProperty<Guid>();

    public Guid AccountId
    {
        get => _accountIdRp.Value;
        set => _accountIdRp.Value = value;
    }

    public IStream<Guid> AccountIdStream => _accountIdRp;

    private readonly ReactiveProperty<Guid> _loginEntityIdRp = new ReactiveProperty<Guid>();

    public Guid LoginEntityId
    {
        set => _loginEntityIdRp.Value = value;
    }

    public IStream<Guid> LoginEntityIdStream => _loginEntityIdRp;

    private readonly ReactiveProperty<IEntitiesRepository> _clientClusterRepositoryRp = new ReactiveProperty<IEntitiesRepository>();
    public IStream<IEntitiesRepository> ClientClusterRepository => _clientClusterRepositoryRp;

    private readonly ListStream<ResourceRef<RealmRulesQueryDef>> _queriesStream = new ListStream<ResourceRef<RealmRulesQueryDef>>();
    private IListStream<ResourceRef<RealmRulesQueryDef>> QueriesStream => _queriesStream;
    private ResourceRef<RealmRulesQueryDef>[] _queries;

    public ResourceRef<RealmRulesQueryDef>[] Queries
    {
        get => _queries;
        set
        {
            _queries = value;
            foreach (var query in _queries)
                _queriesStream.Add(query);
        }
    }

    public ClientRepositoryProcess ClientRepositoryHost { get; set; }

    //=== Unity ===========================================================
    private void Awake()
    {
        _telemetrySystem = new TelemetrySystem();
        _buildSystem = new BuildSystem();
        _sceneStreamerSystem = new SceneStreamerSystem();
        Instance = SingletonOps.TrySetInstance(this, Instance);
        //OnSceneObjectsWeaver = new OnSceneObjectsWeaver(NetObjsCatalogue);
        StaticDefsHolder.AssertIfNull(nameof(StaticDefsHolder), gameObject);
        _gameOptions.AssertIfNull(nameof(_gameOptions));
        IsInGameRp.Value = false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance == this)
            Instance = null;

        _ownCts.Cancel();
        _ownCts.Dispose();
    }


    //=== Public ==============================================================
    public override void AfterDependenciesInjected()
    {
    }

    public async Task Host(
        LobbyGuiNode lobby,
        string verifyServerAddress,
        ContainerConfig config,
        CloudSharedDataConfig sharedConfig,
        ConnectionParams connParams)
    {
        if (_connectCts != null)
            await AsyncProcessExtensions.ShutdownProcess(_connectCts, _connectProcess);
        _connectCts = new CancellationTokenSource();

        var clientRepoCfg = DefaultConfigDef.Target;

        AsyncProcess proc = (inner, ct) => ConnectProcess.HostInf(
            lobby,
            verifyServerAddress,
            connParams,
            config,
            sharedConfig,
            clientRepoCfg,
            inner,
            ct);
        _connectProcess = proc.Exec(_connectCts.Token);

        await LogAsyncProcessResult(_connectProcess);
    }

    public async Task MainLobbyProcess(InnerProcess innerProc, CancellationToken ct)
    {
        using (var cts = new CancellationTokenSource())
        {
            var outer = AsyncProcessExtensions.EmptyProcess(ct);
            var our = AsyncProcessExtensions.EmptyProcess(_ownCts.Token);
            var inner = innerProc(cts.Token);

            await Task.WhenAny(our, inner, outer);

            if (_connectCts != null)
            {
                await AsyncProcessExtensions.ShutdownProcess(_connectCts, _connectProcess);
                _connectCts = null;
            }

            cts.Cancel();
            try
            {
                await inner;
            }
            catch (OperationCanceledException e)
            {
                Logger.IfInfo()?.Message(e, "Main Lobby process was cancelled").Write();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Main Lobby process faulted").Write();
            }

            Debug.Log("Shut down the game");
            AppStart.Quit();
        }
    }

    public async Task Connect(
        LobbyGuiNode lobby,
        string verifyServerAddress,
        ConnectionParams connParams,
        PlayParams playParams = default,
        bool hideLoader = false)
    {
        if (_connectCts != null)
            await AsyncProcessExtensions.ShutdownProcess(_connectCts, _connectProcess);
        _connectCts = new CancellationTokenSource();

        var clientRepoCfg = DefaultConfigDef.Target;

        AsyncProcess proc = (inner, ct) => ConnectProcess.ConnectInf(
            lobby,
            verifyServerAddress,
            connParams,
            clientRepoCfg,
            playParams,
            hideLoader,
            inner,
            ct
        );
        _connectProcess = proc.Exec(_connectCts.Token);

        await LogAsyncProcessResult(_connectProcess);
    }

    public void ExitGame() => _ownCts.Cancel();

    //=== Internal ============================================================

    internal GameObject GetGoFromGameObjectsForEntities(OuterRef<IEntity> @ref)
    {
        if (!@ref.IsValid)
            return null;

        return GetGoFromGameObjectsForEntities(ReplicaTypeRegistry.GetTypeById(@ref.TypeId), @ref.Guid);
    }

    internal GameObject GetGoFromGameObjectsForEntities(Type type, Guid guid)
    {
        var typeId = ReplicaTypeRegistry.GetIdByType(type);
        var oRef = new OuterRef<SharedCode.Entities.GameObjectEntities.IEntityObject>(guid, typeId);
        var go = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(oRef);
        if (go == null)
            Logger.IfError()?.Message($"{guid} misses object").Write();
        return go;
    }


    //=== Private =============================================================

    private static async Task LogAsyncProcessResult(Task process)
    {
        try
        {
            await process;
            Logger.IfInfo()?.Message("Connect process exited").Write();
        }
        catch (OperationCanceledException e)
        {
            Logger.IfInfo()?.Message(e, "Connect process was cancelled").Write();
        }
        catch (Exception e)
        {
            Logger.IfError()?.Message(e, "Connect process faulted").Write();
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void TriggerStaticCtorsToBeCalledHack()
    {
        Logger.IfDebug()?.Message($"#Dbg: {nameof(TriggerStaticCtorsToBeCalledHack)} call.").Write();

        LootTableCalcer.TriggerStaticCtorToBeCalledHack();
        LootTablePredicateCalcer.TriggerStaticCtorToBeCalledHack();
        LootItemChanceWeightCalcerCalcer.TriggerStaticCtorToBeCalledHack();
        ComputableStateMachinePredicateCalcer.TriggerStaticCtorToBeCalledHack();
    }


    //=== Class =======================================================================================================

    [Serializable]
    public class EntitiesRepositoryConfigRef : JdbRef<EntitiesRepositoryConfig>
    {
    }

    /// <summary>
    /// Static Instance is present on server & on client (instances are independent i.e. unsynced).
    /// In host mode only single instance exist - is shared by server & client logic
    /// </summary>
    [Serializable]
    public class LoginServerConfigRef : JdbRef<LoginServerConfigDef>
    {
    }
}