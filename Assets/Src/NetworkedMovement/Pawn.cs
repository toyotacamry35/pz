using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using System.Collections;
using Assets.Src.Tools;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.CustomData;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using Assets.Src.Lib.Cheats;
using Assets.Src.NetworkedMovement.MoveActions;
using Assets.Src.Server.Impl;
using Assets.Src.Shared;
using Assets.Src.SpawnSystem;
using SharedCode.MovementSync;
using SharedCode.EntitySystem;
using Assets.Src.Src.NetworkedMovement;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using SharedCode.AI;
using Src.Locomotion;
using Src.Locomotion.Unity;
using Assets.Src.Aspects.Impl;
using Assets.Src.Lib.Extensions;
using Assets.Src.Locomotion.Utils;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using Src.NetworkedMovement;
using SharedCode.Serializers;
using Src.Debugging;

using SharedVec3 = SharedCode.Utils.Vector3;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


namespace Assets.Src.NetworkedMovement
{
    public interface IBoolVisibilityProvider
    {
        bool IsClose { get; set; }
    }

    [RequireComponent(typeof(Rigidbody), typeof(DirectMotionProducer))]
    public partial class Pawn : EntityGameObjectComponent, /*ILocomotionDebugable*/IDebugInfoProvider, IBoolVisibilityProvider
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Pawn");
        [NotNull] private static readonly NLog.Logger ___Logger = LogManager.GetCurrentClassLogger();

        private bool IsDebug => Consts.IsDebug;

        public static HashSet<Guid> EnteredDebugRegions = new HashSet<Guid>();

        private const int MoveActionsUpdateTickOnFull = 0; //but we'll lerp on far end
        private const int MoveActionsUpdateTickOnFaraway = 3; //1;
        private const int MoveActionsUpdateTickOnSuspended = 10; //3;
        private readonly long DelayForSuspendWhenNoAction = SyncTime.FromSeconds(1.0f); // seconds
        private const int LocoCoroutineUpdateStepLerpToMax = 3;
        private const int LocoCoroutineUpdateStepFar = 5; ///10; //after Task "Mobs n/w synchronization" done lower this value to, say, 5
 
        [SerializeField] public float CurrLocoCoroutineCyclesPerSec;
        [SerializeField] public float CurrMoveActionsCoroutineCyclesPerSec;
        [SerializeField] private DirectMotionProducer DirectMotionProducer; // assigns by Def

        // --- For WatchDog: -------------------------
        private readonly List<float> _locoCoroutineCycleTimes = new List<float>();
        private readonly List<float> _moveActionsCoroutineCycleTimes = new List<float>();
        ///#PZ-7910: #TODO: причесать это всё - единый источник всех этих дистанций  ///#PZ-9704!
        private IRelevanceLevelProviderSettings _relevanceSettingsUpdate;
        private int _locoCoroutineTimeStep;
        private float _locoCoroutineLastExecutedTime;
        private bool _locomotionIsBroken;
        private bool _moveActionIsBroken;
        private MoveActionsDoer _moveActionsDoer;
        private Rigidbody _rigidbody;
        private Collider _colliderS; //S - Server
        private NavMeshAgent _agentS; //S - Server
        private ISubjectView _clientView;
        private LocomotionPawnBody _virtualBodyS; //S - Server
        private Coroutine _moveActionsCoroutine;
        private Coroutine _locomotionCoroutine;
        private ILocomotionRelevancyLevelProvider _relevanceProviderNetwork;
        private IMobRelevanceProvider _relevanceProviderUpdateCl;
        private IMobRelevanceProvider _relevanceProviderUpdateS;
        private LocomotionEngine _locomotionCl;
        private LocomotionEngine _locomotionS; 

        private LocomotionEngine _locomotionTheOnlyClS; ///#PZ-13761: 

        // suffix "Sh" means - single instance is used at Server & Cl loco-pipeline:
        private readonly LocomotionSyncedClock _locomotionClockSh = new LocomotionSyncedClock(); //v
        private ILocomotionDebugInfoProvider _locomotionDebugProvider;
        private LocomotionDebug.Context _locomotionDebugContext; //v
        // suffix "Sh" means - single instance is used at Server & Cl loco-pipeline:
        private LocomotionPipeline _locomotionClientOnlyPipelineSh;
        private SimulationLevelCoordinator _simulationLevelServer;// Is needed only on Server:
        private SimulationLevelCoordinator _simulationLevelClient; // Is needed only on Client:
        private LocomotionDebugTrail _locomotionDebugTrailInstance;
        private Vector3 _prevBodyPosition;
        private PawnPositionLoggingHandler _posLoggingHandler;
        private MobGuideProvider _guideProvider;
        private LocomotionTransformBody _transformBodyS;
        private ILocomotionStateMachine _stateMachineS;
        private bool IsLocomotionEnabled() => true;
        private MobLocomotionDef LocomotionDef => ((LegionaryEntityDef) Ego.EntityDef).MobLocomotion.Target;
        private float RelevancyBordersClose => _relevanceSettingsUpdate.DistanceForMinRelevanceLevel;
        private float RelevancyBordersMid => GlobalConstsHolder.GlobalConstsDef.Tmp_Dbg_RelevancyBordersMid; // 0f == 14.99f 
        private float RelevancyBordersFar => GlobalConstsHolder.GlobalConstsDef.VisibilityDistance;
        // suffix "Sh" means - single instance is used at Server & Cl loco-pipeline:
        private LocomotionPipeline LocomotionClientOnlyPipelineSh => _locomotionClientOnlyPipelineSh = _locomotionClientOnlyPipelineSh ?? new LocomotionPipeline();
        private bool _locomotionClientOnlyPipelineInited = false;

        
        private LocomotionEngineAgent _locomotionAgentS;
        private NavMeshGroundSensor _groundSensorS;
        private LocomotionEnvironment _environmentS;
        private LocomotionHistory _historyS;
        private MobStateMachineContext _stateMachineContextS;
        private LocomotionNetworkSender _sender;
        // Is server loco inited & not cleaned_hard:
        private bool IsServerLocoValid => _locomotionS != null;
        private bool _isServerLocoActive;
        // suffix "Sh" means - single instance is used at Server & Cl loco-pipeline:
        private LocomotionInterpolationBufferNode _interpolationBufferSh;

        private LocomotionKinematicBody _kinematicBodyCl;
        private LocomotionNetworkReceiver _receiver;

        public enum State
        {
            NotInitialized,
            Dead,
            Alive
        }

        public enum SimulationLevel
        {
            Off,
            Suspended,
            Faraway,
            Full
        }
        
        public enum UpdateResultMean
        {
            Null,
            Yield,
            Break,
        }
        
        public event Action PawnBroke;

        public NavMeshAgent NavMeshAgent => _agentS;

        public ILocomotionDebugInfoProvider LocomotionDebugInfo => _locomotionDebugProvider;

        public LocomotionDebugTrail LocomotionDebugTrail => _locomotionDebugTrailInstance;

        public IMoveActionsDoer MoveActionsDoer => _moveActionsDoer;

        public bool IsClose { get; set; }

        // --- Override: -------------------------------------------------------------------

        void Awake()
        {
            //DirectMotionProducer = GetComponent<DirectMotionProducer>();
            DirectMotionProducer.AssertIfNull(nameof(DirectMotionProducer));

            IsClose = true;

            // для совместимости со старыми префабами
            if (GetComponent<ClientViewed>() is ClientViewed cv)
            {
                _viewPrefab = cv._viewPrefab;
                _offset = cv._offset;
            }

            _view.Zip(D, _isVisibleByCamera)
                .Action(D, (view, vd) =>
                {
                    if (view != null)
                        vd.Attach(view.GameObject);
                    else
                        vd.Detach();
                });
            
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;

            {
                // It is needed only on S. BUT done here (at Awake) - Why?:
                {
                    // Why?: 'cos 1st `ActionAdded` events could fire before `GotServer` call, 'cos SpellSys already works:
                    _colliderS = GetComponent<Collider>();
                    _agentS = GetComponent<NavMeshAgent>();
                    //if (DbgLog.Enabled) DbgLog.Log($"_agentS.enabled := false(1) [{_agentS.isOnNavMesh} :: {_agentS.navMeshOwner}]({EntityId})");
                    _agentS.enabled = false;
                    //if (DbgLog.Enabled) DbgLog.Log($"_agentS.enabled := false(2) [{_agentS.isOnNavMesh} :: {_agentS.navMeshOwner}]({EntityId})");
                    //_visionColliderS = GetComponentsInChildren<Collider>().Single(x => x.gameObject.layer == LayerMask.NameToLayer("Vision"));
                    _virtualBodyS = new LocomotionPawnBody(this);

                    ///#PZ-13761: #Dbg: #Tmp:
                    if (Dbg_PZ_13761_Enabled)
                    {
                        Dbg_PZ_13761_Dbg_Data_AwakePos = transform.position;
                        Dbg_PZ_13761_Dbg_Data_AwakeTime = Time.time;
                    }
                }
                // Why?: ??
                _simulationLevelServer = new SimulationLevelCoordinator(ApplySimulationLevelServer, SimulationLevel.Suspended, this);
            }
            {
                // It is needed only on Cl. BUT done here (at Awake) - Why?: ??
                _simulationLevelClient = new SimulationLevelCoordinator(ApplySimulationLevelClient, SimulationLevel.Full, this);
            }

            if (Debug.isDebugBuild)
            if (Debug.isDebugBuild)
                _posLoggingHandler = new PawnPositionLoggingHandler(this);
        }

        private void MoveDataChanged(MovementData data)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, $"Move Data Changed | {data}").Write();

            if (data.IsValid)
            {
                var spellId = data.SpellId;
                var effectDef = data.MoveEffectDef;
                
                if (data.Start)
                {
                    var spellDef = data.CastData.Def;
                    var castData = data.CastData.Clone();
                    var entityRef = new OuterRef<IEntity>(EntityId, TypeId);

                    if (Logger.IsDebugEnabled && ((IResource) effectDef).Address.Root != ((IResource) spellDef).Address.Root)
                        Logger.IfError()?.Write(EntityId, $"Inconsistent movement data | Effect:{effectDef.____GetDebugAddress()} Spell:{spellDef.____GetDebugAddress()} Data:{data}");
                
                    UnityQueueHelper.RunInUnityThreadNoWait( () =>
                        {
                            var spellPred = new SpellPredCastData(
                                castData: castData,
                                caster: entityRef,
                                repo: ClientRepo,
                                currentTime: 0,
                                wizard: default,
                                slaveMark: null,
                                modifiers: null,
                                canceled: false
                            );
                            MoveActionsDoer?.OnMoveEffectStarted(spellId, effectDef, spellDef, spellPred);
                        });
                }
                else
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(
                        () => MoveActionsDoer?.OnMoveEffectFinished(spellId, effectDef));
                }
            }
        }

        protected override void DestroyInternal()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: DestroyInternal");

            DestroyView();

            if (_moveActionsDoer != null)
            {
                _moveActionsDoer.Dispose();
                _moveActionsDoer = null;
            }

            if (IsServer)
            {
                UninitServer();
                //if (_posLoggingHandler != null)
                //    _posLoggingHandler.SubscribeUnsubscribe(GetOuterRef<IEntity>(), ServerRepo, SubscribeUnsubscribe.Unsubscribe); The only subscription is on GotCl now
            }

            if (IsClient)
            {
                UninitClient();
                if (!IsHostMode)
                {
                    if (_posLoggingHandler != null)
                        _posLoggingHandler.SubscribeUnsubscribe(GetOuterRef<IEntity>(), ClientRepo, SubscribeUnsubscribe.Unsubscribe);
                }
            }
        }

        //#Note: 'cos of some reasons (1 of which is proper initialization order including setting proper position) mob is instantiated with isAlive == false
        //.. Then Resurrect event is invoked. So for now the entrance point of all initialization are `OnResurrect...` methods, which are called by `DeathResurrectHandler`.
        //.. That's why there are no `GotServer` / `-Client` methods.
        protected override void GotServer() //срабатывает в т.ч. на изменении `PathFindingOwnership`
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: GotServer");

            ///#PZ-13568: #Dbg:
            if (DbgLog.Enabled) DbgLog.Logger.IfWarn()?.Message($"PZ-13761: Pawn.GotServer ({EntityId})").UnityObj(gameObject).Write();

            if (Constants.WorldConstants.UseMockLocomotion != MockLocomotionUsage.None)
                return;

            //_posLoggingHandler?.SubscribeUnsubscribe(GetOuterRef<IEntity>(), ServerRepo, SubscribeUnsubscribe.Subscribe); already subscribed at GotClient

            ///#PZ-17474: TODO: #TRY: Нужно ли это именно тут? - хочется перенести в `InitLocoServer`, но оно срабатывает из OnResurrect и судя по логам стреляет чуть позже (т.к. из другого EGOComp-та - DeathResurrectHandler'а). Хотя наверное можно - по GotServer тоже срабатывает
            if (_moveActionsDoer != null)
            {
                //_moveActionsDoer.DBG_EnsureIsClear();
                _moveActionsDoer.Restart();
            }
            else
            {
                _moveActionsDoer = new MoveActionsDoer(_virtualBodyS, _agentS, OuterRef, ServerRepo, this);
                // Отписка в симметричном месте не нужна, т.к. там это решается через "SubscribeUnsubscribeMobMovement(SubscribeUnsubscribe.Unsubscribe.." - другое место отрезания общей нити выполнения
                _moveActionsDoer.ActionAdded += OnNewMoveAction;
            }

            if (Dbg_PZ_13761_Enabled)
            {
                Dbg_PZ_13761_Dbg_Data = new PZ_13761_Dbg_Data(_moveActionsDoer, _agentS, transform, Dbg_PZ_13761_Dbg_Data_AwakeTime, Dbg_PZ_13761_Dbg_Data_AwakePos, this);
                Dbg_PZ_13761_Dbg_Data.PosGotServer = transform.position;
                Dbg_PZ_13761_Dbg_Data.GotServerTime = Time.time;
                Dbg_PZ_13761_Dbg_Data.EntId = EntityId;
            }
            _pZ_13761_GotServ = true;

            SubscribeUnsubscribeMobMovement(SubscribeUnsubscribe.Subscribe, ServerRepo);
        }

        // Вообще обычно Unsubscribe звать не нужно, т.к. SubscribePropertyChanged это только про локальную реплику, которая когда LostServer пропадает из репликации
        // !НО!: мы ж тут сами себя перехитрили и ServerRepo когда GotPathOwnership - это просто подложенная ссылка на Cl repo. Фактически никакой репликации/разрепликации не происходит! Поэтому `Unsubscribe` звать нужно!
        internal void SubscribeUnsubscribeMobMovement(SubscribeUnsubscribe instruction, IEntitiesRepository serverRepo)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var entW = await serverRepo.Get(TypeId, EntityId))
                {
                    var mobMovement = entW.Get<IHasMobMovementSyncAlways>(TypeId, EntityId, ReplicationLevel.Always)?.MovementSync;
                    if (mobMovement == null)
                    {
                        Logger.IfError/*IfDebug*/()?.Message($"{EntityId} SubscribeUnsubscribeMobMovement({instruction}) : mobMovement == null.  [ It's ok if LostCL & LostS - both ]");
                        return;
                    }
        
                    switch (instruction)
                    {
                        case Aspects.SubscribeUnsubscribe.Subscribe:
                            MoveDataChanged(mobMovement.MovementData);
                            mobMovement.SubscribePropertyChanged(nameof(mobMovement.MovementData), MovementDataChanged);
                            if (DbgLog.Enabled) DbgLog.Log(17474, "Subscribed MovementDataChanged");
                            break;
                        case Aspects.SubscribeUnsubscribe.Unsubscribe:
                            mobMovement.UnsubscribePropertyChanged(nameof(mobMovement.MovementData), MovementDataChanged);
                            if (DbgLog.Enabled) DbgLog.Log(17474, "UnSubscribed MovementDataChanged");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                    }
                }
            });
        }


        private async Task MovementDataChanged(EntityEventArgs args)
        {
            using (var cnt = await args.Sender.EntitiesRepository.Get(args.Sender.ParentTypeId, args.Sender.ParentEntityId))
            {
                var mobMovement = cnt.Get<IHasMobMovementSyncAlways>(TypeId, EntityId, ReplicationLevel.Always)?.MovementSync;
                if (mobMovement != null)
                    MoveDataChanged(mobMovement.MovementData);
            }
        }
        

        protected override void GotClient()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: GotClient");

            //if (!IsHostMode) This is the only subscription place left now.
                _posLoggingHandler?.SubscribeUnsubscribe(GetOuterRef<IEntity>(), ClientRepo, SubscribeUnsubscribe.Subscribe);
        }

        protected override void LostServer()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: LostServer");
Debug.Assert(ServerRepo != null); // && ReferenceEquals(ServerRepo, ClientRepo) - Cl repo could be null, if lost Cl & lost S at same time - LostCl fires first & Cl repo here == null

            if (Constants.WorldConstants.UseMockLocomotion != MockLocomotionUsage.None)
                return;
            UninitServer();
        }

        protected override void GotPathfindingOwnership()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: GotPathfindingOwnership");

            ///#PZ-17474: (actually #PZ-17219) no_need:
            // if (!Alive)
            // {
            //     Logger.IfWarn()?.Message($"GotPathfindingOwnership is called, but !Alive. ({EntityId}).  [ It could be Ok, if mob dies & switch pathOwnership at same time, but perhaps not so Ok, if we just didn't got resurrect yet, 'cos of order of execution ]");
            //     // We got Alive - from EGO.GotClient. GotPathfindingOwnership subscription is done at UnitySpawnService, when object is instantiated.
            //     return;
            // }

            //_moveActionsDoer.DBG_EnsureIsClear();
            OnWakeup();
        }

        protected override void LostPathfindingOwnership()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: LostPathfindingOwnership //{IsClient} / ?{IsServer}?(should be False!)  //hope _agent 'll be reset");

            if (IsDebug)
                Debug.Assert(!IsServer);

            // it is true everytime
            if (IsClient)
            {
                _simulationLevelServer.ChangeState(State.Dead);  ///#PZ-17474: (A) ?O? Почему тут именно такой порядок был? - может важно. Сейчас нарушился - отпаиска переехала в ___StopLocoS___
                                                                 
                //old: _moveActionsDoer.ActionAdded -= OnNewMoveAction;
                //old: _moveActionsDoer = null;
                //moved: _moveActionsDoer.StopAndClean(); ///#PZ-17474:  Возможно перенести в общий стоп: __________StopServer__________

                ///#moved to ___StopLocoS___
                // ///#PZ-17474: TODO: Если Выстреливает LostServer, то перенести это туда (но может важен порядок тут?)
                // AsyncUtils.RunAsyncTask
                // (
                //     async () =>
                //     {
                //         if (IsClient)
                //         {
                //             using (var entW = await ClientRepo.Get(TypeId, EntityId))
                //             {
                //                 var mobMovement = entW.Get<IHasMobMovementSyncAlways>(TypeId, EntityId, ReplicationLevel.Always)?.MovementSync;
                //                 if (mobMovement != null)
                //                     mobMovement.UnsubscribePropertyChanged(nameof(mobMovement.MovementData), MovementDataChanged);
                //             }
                //         }
                //     }
                // );

                if (Alive) // in other case all the same(*) 'll happen on resurrect. *) - Namely: 1) InitLocomotionClient, 2) ChangeState(State.Alive) & 3) Raise(SimulationLevel.Full)
                { 
                    _simulationLevelClient.ChangeState(State.Dead);
                    InitLocomotionClient(LocomotionDef);
                    _simulationLevelClient.Raise(SimulationLevel.Full);
                    _simulationLevelClient.ChangeState(State.Alive);
                }
            }
        }

        protected override void LostClient()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: LostClient");

            DestroyView();
            if (!IsHostMode) //Hope, it's true here if namely Host-mode (not pathOwnership)
                UninitClient();
        }

        private void ApplySimulationLevelServer(SimulationLevel simulationLevel, bool raise)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: ApplySimulationLevelServer({simulationLevel}, {raise})");

            Dbg_PZ_13761_Dbg_Data?.SimulLvlChanged(simulationLevel);

            if (Dbg_LogSimLvlChange) Dbg_SimLvlChangeLog(simulationLevel, raise, " <<S.>> ");

            switch (simulationLevel)
            {
                case SimulationLevel.Suspended:
                    if (raise)
                    {
                        //#moved to Far: _moveActionsCoroutine = this.StartInstrumentedCoroutineLiteWeight(MoveActionsCoroutine());
                        //_Deprecated_moveActionsUpdateTick = MoveActionsUpdateTickOnSuspended;
                    }
                    else
                    {
                        //#moved to Far: this.StopCoroutineIfNotNull(ref _moveActionsCoroutine);
                    }

                    break;

                case SimulationLevel.Faraway:
                    if (raise)
                    {
                        NavMeshHit myNavHit = SamplePositionOnNavMesh();
                        if (myNavHit.hit)
                        {
                            //#Locomotion: Reset agent here
                            if (Dbg_PZ_13761_Enabled)
                            { 
                                var log1 = new NMAMoveData(_agentS, myNavHit.position);
                                _agentS.nextPosition = myNavHit.position;
                                var log2 = new NMAMoveData(_agentS, NMAMoveData.NanV3);
                                ///#PZ-13568: #Dbg:
                                Dbg_PZ_13761_Dbg_Data?.OnNMAMoveEvent(NMAMoveType.P_SetNxtPos, (log1, log2));
                            }
                            else
                                _agentS.nextPosition = myNavHit.position;
                        }
                        else
                            Logger.Error($"Can't SamplePosition({transform.position}, {100}, {NavMesh.AllAreas}) for {EntityId}. on Faraway.raise.");

                        _agentS.enabled = true;

///#PZ-17474: #Dbg:
if (DbgLog.Enabled) DbgLog.Log(17474, $"ApplySimulationLevelServer:: _agent is reset! MAY BE SHOULD ALSO RESET ITS SPEED & ETC HERE"); ///TODO: MAY BE SHOULD ALSO RESET _agents' SPEED & ETC HERE

                        //if (DbgLog.Enabled) DbgLog.Log($"_agentS.enabled == TRUE [{_agentS.isOnNavMesh} :: {_agentS.navMeshOwner}]({EntityId})");       /// <<-------------<<<
                        _moveActionsCoroutine = _moveActionsCoroutine ?? this.StartInstrumentedCoroutineLiteWeight(MoveActionsCoroutine());
                        _locomotionCoroutine = _locomotionCoroutine ?? (!_locomotionIsBroken ? this.StartInstrumentedCoroutineLiteWeight(LocomotionCoroutine()) : null);
                        //_Deprecated_moveActionsUpdateTick = MoveActionsUpdateTickOnFaraway;
                    }
                    else
                    {
                        _agentS.enabled = false;
                        //if (DbgLog.Enabled) DbgLog.Log($"_agentS.enabled == XxXxX [{_agentS.isOnNavMesh} :: {_agentS.navMeshOwner}]({EntityId})");
                        this.StopCoroutineIfNotNull(ref _locomotionCoroutine);
                        this.StopCoroutineIfNotNull(ref _moveActionsCoroutine);
                        //_Deprecated_moveActionsUpdateTick = MoveActionsUpdateTickOnSuspended;
                    }

                    break;

                case SimulationLevel.Full:
                    if (raise)
                    {
                        //_Deprecated_moveActionsUpdateTick = MoveActionsUpdateTickOnFull;
                        //_visionColliderS.enabled = true;
                        // _colliderS.enabled = true;
                    }
                    else
                    {
                        //_Deprecated_moveActionsUpdateTick = MoveActionsUpdateTickOnFaraway;
                        //_visionColliderS.enabled = false;
                        // _colliderS.enabled = false;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(simulationLevel), simulationLevel, null);
            }
        }

        const float InitialR = 1f;
        const float IncreaseRFactor = 10f;
        static Dictionary<float, int> _dbgCountingSuccessSamplesR = new Dictionary<float, int>(3);
        // For optimization sake we call `NavMesh.SamplePosition` with small R, then increase R: 1, 10, 100.
        // Due to recomendations here: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        // : "If you are trying to find a random point on the NavMesh, it is better to use recommended radius and do try multiple times instead of using one very large radius."
        private NavMeshHit SamplePositionOnNavMesh()
        {
            var r = InitialR;
            var filter = new NavMeshQueryFilter() {areaMask = NavMesh.AllAreas, agentTypeID = _agentS.agentTypeID};
            NavMeshHit hit;
            do
            {
                if (NavMesh.SamplePosition(transform.position, out hit, r, filter))
                    break;
                r *= IncreaseRFactor;
            } while (r <= 100f);

            //#Dbg:
            // Collect & log statistics: TODO: see statistics & may be alter `InitialR` & `IncreaseRFactor`
            if (Debug.isDebugBuild && DbgLog.Enabled && hit.hit)
            {
                if (_dbgCountingSuccessSamplesR.TryGetValue(r, out var count))
                    _dbgCountingSuccessSamplesR[r] = count+1;
                else
                    _dbgCountingSuccessSamplesR[r] = 1;
                
                //Log statistics, when enough:
                if (count > 100)
                {
                    DbgLog.Log($"#DBG: CountingSuccess SamplePositionOnNavMesh Radiuses: {_dbgCountingSuccessSamplesR.ItemsToStringSimple()}");
                    _dbgCountingSuccessSamplesR.Clear();
                }
            }

            return hit;
        }

        private void ApplySimulationLevelClient(SimulationLevel simulationLevel, bool raise)
        {
            //#Note: Now on Cl it works only once* for mob: it is set to ".Full" on AwakeInternal & never changes 
            //.. (*- raise is executed step-by-step raising every lvl on way to target lvl)

            if (Dbg_LogSimLvlChange) Dbg_SimLvlChangeLog(simulationLevel, raise, "--(Cl.)--");

            switch (simulationLevel)
            {
                case SimulationLevel.Suspended:
                    if (raise)
                    {
                        if (!IsHostMode)
                            _locomotionCoroutine = this.StartInstrumentedCoroutine(LocomotionCoroutine());
                    }
                    else
                    {
                        if (!IsHostMode)
                            this.StopCoroutineIfNotNull(ref _locomotionCoroutine);
                    }

                    break;

                case SimulationLevel.Faraway:
                    break;

                case SimulationLevel.Full:
                    if (raise)
                    {
                        _clientView.Enabled = true;
                        _colliderS.enabled = true;
                    }
                    else
                    {
                        if (_clientView != null)
                            _clientView.Enabled = false;
                        _colliderS.enabled = false; //  выключаем коллайдер, чтобы в период между смертью моба и удалением его энтити, в него не приходили удары
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(simulationLevel), simulationLevel, null);
            }
        }

        private IEnumerator LocomotionCoroutine()
        {
            while (true)
            {
                // ReSharper disable once MustUseReturnValue
                var (mean, delay) = LocoTickStep_DBG = LocomotionUpdate();
                switch (mean)
                {
                    case UpdateResultMean.Break:
                        _locomotionIsBroken = true;
                        yield break;
                    case UpdateResultMean.Null:
                        yield return null;
                        break;
                    case UpdateResultMean.Yield:
                        yield return CoroutineAwaiters.GetTick(delay);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // @returns: a) null if You should "yield return null" outside; || 
        //.. b) positive int - if you should "yield return GetTick(that int)". ||
        //.. c) "-1" as invalid result if something went wrong
        private (UpdateResultMean Mean, int Period) LocomotionUpdate()
        {
            try
            {
                _posLoggingHandler?.CurveLogger?.IfActive?.AddData("7) LocoUpdNow", SyncTime.Now, 0);

                ///#PZ-9704: #Dbg:
                if (false)
                    if (IsServer && GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef))
                    {
                        if (_dbg_1st100LocoUpdatesDataOnServer == null)
                            _dbg_1st100LocoUpdatesDataOnServer = new List<DbgLoco1st100Data>(100);

                        if (_dbg_1st100LocoUpdatesDataOnServer.Count < 100)
                            _dbg_1st100LocoUpdatesDataOnServer.Add(new DbgLoco1st100Data(DateTime.UtcNow,
                                (_virtualBodyS != null) ? (SharedVec3?) LocomotionHelpers.LocomotionToWorldVector(_virtualBodyS.Position) : null,
                                (_transformBodyS != null) ? (SharedVec3?) LocomotionHelpers.LocomotionToWorldVector(_transformBodyS.Position) : null,
                                transform.position,
                                _moveActionsDoer.CurrentAction?.GetType(),
                                _simulationLevelServer));
                    }

                _lastTickLoco = DateTime.UtcNow;
                ++_locoUpdatesCount;
                //#Dbg: prepare data for watchdog:
                if (_isEnabledCollectDebugData)
                    _locoCoroutineCycleTimes.Add(Time.realtimeSinceStartup);

                ///var newExecutedTime = Time.realtimeSinceStartup;
                var newExecutedTime = Time.time;
                var dt = newExecutedTime - _locoCoroutineLastExecutedTime;
                if (dt != 0)
                {
                    if (_isServerLocoActive)
                        _locomotionS?.Execute(dt);
                    else
                        _locomotionCl?.Execute(dt);
                    Dbg_PZ_13761_Dbg_Data?.OnLocoUpdEvent();
                }

                _locoCoroutineLastExecutedTime = newExecutedTime;

                ///#PZ-13761: #Dbg: 
                if (PZ_13761_Dbg_Data.Mode == 1)
                    if (Dbg_PZ_13761_Dbg_Data?.ShouldSave(newExecutedTime) ?? false)
                    {
                        var pathExist = !Dbg_PZ_13761_Dbg_Data.PrevNMAPos.HasValue // not 1st call
                                        || NavMesh.CalculatePath(_agentS.nextPosition, Dbg_PZ_13761_Dbg_Data.PrevNMAPos.Value, new NavMeshQueryFilter() {areaMask = _agentS.areaMask, agentTypeID = _agentS.agentTypeID}, Dbg_PZ_13761_Dbg_Data.LastPath);
                        Dbg_PZ_13761_Dbg_Data.Save(new PZ_13761_Dbg_Data.PZ_13761_Dbg_PositionsAndMNAData(_virtualBodyS.Position.ToWorld().ToUnity(), transform.position, _agentS.enabled, _agentS.isOnNavMesh, _agentS.hasPath, _agentS.nextPosition, _agentS.steeringTarget, pathExist, newExecutedTime), DBG_Forced_DAMP_PZ_13761);
                        DBG_Forced_DAMP_PZ_13761 = false;
                    }
            }
            catch (Exception e)
            {
                var msg = $"Stop LocomotionCoroutine due exception: \"{e}\". entityId: {EntityId}, g.o.: {gameObject?.name}, g.o.pos: {transform?.position}, locoUpdCount:{_locoUpdatesCount}, SM.CurrState:{_stateMachineS?.CurrentStateName}" +
                          $"\n_dbg_1st100LocoUpdatesDataOnServer: {(_dbg_1st100LocoUpdatesDataOnServer != null ? string.Join(";\n", _dbg_1st100LocoUpdatesDataOnServer) : "null")}.";
                Logger.IfError()?.Message(EntityId, msg).Write();
                if (IsDebug)
                { 
                    Dbg_PZ_13761_Dbg_Data?.OnLocomotionBroken(msg);
                    //old: PawnBroke?.Invoke();
                    PawnDataSharedProxy.Dbg_BrokenLocomotions.Push(new DbgBrokenLocomotionData(EntityId, gameObject.name, DateTime.UtcNow, _locomotionCl?.DbgUpdateCounter ?? -1, transform.position.ToShared(), e.Message));
                }
                return (UpdateResultMean.Break, -1); // as "yield break;"
            }

            return CalculateLocomotionUpdatePeriod();
        }

        (UpdateResultMean Mean, int Period) CalculateLocomotionUpdatePeriod()
        {
            // #Outdated: Update routine: 
            //  1) till `RelevancyBordersMid` every update;  
            //  2) till `RelevancyBordersFar` by `.GetTick(lerp(0-->3))`;
            //  3) behind `RelevancyBordersFar` by `.GetTick(3)`.
            var closestDist = (_isServerLocoActive) 
                ? Math.Min(
                    (_relevanceProviderUpdateCl?.ClosestObserverDistance ?? float.MaxValue),
                    (_relevanceProviderUpdateS? .ClosestObserverDistance ?? float.MaxValue))
                :   (_relevanceProviderUpdateCl?.ClosestObserverDistance ?? float.MaxValue);

            if (closestDist == float.MaxValue)
                return (UpdateResultMean.Yield, LocoCoroutineUpdateStepFar);
            
            if (closestDist < 0f)
            {
                if (!_closestDistBelow0ErrSpamPreventer)
                {
                    _closestDistBelow0ErrSpamPreventer = true;
                    Logger.IfError()?.Message(EntityId, $"Unexpected `{nameof(closestDist)}` < 0f (=={closestDist})! (entityId: {EntityId}, g.o.: {gameObject.name}, g.o.pos: {transform.position}).").Write();
                }
                return (UpdateResultMean.Break, -1); // as "yield break;"
            }
            
            if (closestDist <= RelevancyBordersMid)
                return (UpdateResultMean.Null, 0); // as "yield return null;"

            var k = Mathf.InverseLerp(RelevancyBordersMid, RelevancyBordersFar, closestDist);
            return (UpdateResultMean.Yield, (int) (k * LocoCoroutineUpdateStepLerpToMax)); // as "yield return CoroutineAwaiters.GetTick(result.Value);"
        }
        

        private IEnumerator MoveActionsCoroutine()
        {
            while (true)
            {
                _lastTickMoveActions = DateTime.UtcNow;
                //#Dbg: prepare data for watchdog:
                if (_isEnabledCollectDebugData)
                    _moveActionsCoroutineCycleTimes.Add(Time.realtimeSinceStartup);

                //#Dbg:
                if (Dbg)
                    if (GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef) && Time.realtimeSinceStartup > dbg_nextTimeLog)
                    {
                        dbg_nextTimeLog = Time.realtimeSinceStartup + dbg_deltaTimeLog;
                        SimLvl_Server = _simulationLevelServer.ToString();
                        SimLvl_Cl = _simulationLevelClient.ToString();
                    }

                try
                {
                    _moveActionsDoer.Update(_simulationLevelServer);
                    Dbg_PZ_13761_Dbg_Data?.OnMADUpdEvent();
                    if (_moveActionsDoer.CurrentAction == null && SyncTime.InThePast(_moveActionsDoer.LastActionFinishedAt + DelayForSuspendWhenNoAction))
                        // #Note: Проверяем тут в т.ч. и `ReqSimulationLevel`, т.к. иначе тут по запросу поднять с Off до Full, прощёлкивает
                        //   по-очереди вверх, на Far стартует MoveactionsCoroutine, которая тут понижает, после чего исполнение возвращается
                        //   к разматывающему 1ую инструкцию `while`, который снова повышает и до Full таки доходит, но лишние шаги назад-вперёд
                        //   получаются.
                        if (_simulationLevelServer <= SimulationLevel.Faraway
                            && _simulationLevelServer.ReqSimulationLevel <= SimulationLevel.Faraway)
                            _simulationLevelServer.Lower(SimulationLevel.Suspended);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(EntityId, $" {EntityId} Stop MoveActionsCoroutine due exception: {e}.").Write();
                    _moveActionIsBroken = true;
                    PawnBroke?.Invoke();
                    yield break;
                }


                //#old: yield return _moveActionsUpdateTick > 0 ? CoroutineAwaiters.GetTick(_moveActionsUpdateTick) : null;
                // Update routine: 
                //  1) till `RelevancyBordersClose` every update;  
                //  2) till `RelevancyBordersFar` by `.GetTick(lerp(0-->3))`;
                //  3) behind `RelevancyBordersFar` by `.GetTick(3)`.
                var closestDist = _relevanceProviderUpdateS?.ClosestObserverDistance ?? float.MaxValue;  ///#PZ-17474: (#OPT: #Danger) ?? к Володе И.: реализация д/ Ownership выдаёт всегда 0 - какой R мобов, которые без ownership'а вообще? (это ж по R репликации?) до этих изменений было так же?
                if (closestDist < RelevancyBordersClose)
                {
                    yield return MoveActionsUpdateTickOnFull > 0 ? CoroutineAwaiters.GetTick(MoveActionsUpdateTickOnFull) : null;
                }
                else
                {
                    var k = Mathf.InverseLerp(RelevancyBordersClose, RelevancyBordersFar, closestDist);
                    yield return CoroutineAwaiters.GetTick((int) (k * MoveActionsUpdateTickOnFaraway));
                }
            }
        }

        private void OnNewMoveAction()
        {
#if DEBUG
            Debug.Assert(_moveActionsDoer.DBG_IsActive);
#endif
            _simulationLevelServer.Raise(SimulationLevel.Suspended + 1);
        }

        public void OnResurrectServerU(PositionRotation at)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: OnResurrectServerU");

            ResurrectServer();
        }

        private void ResurrectServer()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"InitServer(IsHostMode:{0}) {EntityId}", IsHostMode).Write();
            InitServer();
            _simulationLevelServer.ChangeState(State.Alive);
        }

        public void OnResurrectClientU([NotNull] ISubjectView view, PositionRotation at)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: OnResurrectClientU");

            ResurrectClient(view);
        }

        private void ResurrectClient(ISubjectView view)
        {
            InitClient(view);
            _simulationLevelClient.ChangeState(State.Alive);
        }

        public void OnDieServerU()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: OnDieServerU");

            _simulationLevelServer.ChangeState(State.Dead);
            UninitServer();
        }

        public void OnDieClientU()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: OnDieClientU");

            _simulationLevelClient.ChangeState(State.Dead);
            UninitClient();
        }


        // --- Init/Uninit: -------------------------------------------------------------------

        private /*public*/ void InitServer()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: InitServer");

            Plh?.CurveLogger?.IfActive?.AddData("0.3) InitServer.Pos", SyncTime.Now, transform.position.ToLocomotion());
            // MoveActionsDoerInst = new MoveActionsDoer(this, _spellDoer); // нужен, по идее, только на сервере, но так как первые спеллы могут с кастоваться, до того как будет вызван InitServer, создаётся в Awake везде
            PlaceOnSurface(transform.position); // позиция transform'а уже выставлена ранее в UnitySpawnService.InstantiateObject вызовом SpawnGroup.GroupPerGuid[].Spawn()
            Plh?.CurveLogger?.IfActive?.AddData("0.3) InitServer.PlaceOnSurface.Pos", SyncTime.Now, transform.position.ToLocomotion());
            InitLocomotionServer(LocomotionDef);
            if (Debug.isDebugBuild)
                PawnWatchDog.RegisterServerPawn(this);
        }

        private /*public*/ void InitClient([NotNull] ISubjectView view)
        {
            _clientView = view ?? throw new ArgumentNullException(nameof(view));
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            InitLocomotionClient(LocomotionDef);
        }

        // Is called from (3): DestroyIntrnl, LostS, OnDieServerU
        private /*public*/ void UninitServer()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: UninitServer");

            __________StopLocomotionServer__________();
        }

        private /*public*/ void UninitClient()
        {
            // We should off server, before off client.
            CleanLocomotion_Hard(true);
            CleanLocomotion_Hard(false);
            _clientView = null;
        }

        private void InitLocomotionServer(MobLocomotionDef locomotionDef) 
        {          
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: InitLocomotionServer");
           
            //#Dbg
            if (locomotionDef == null)
            {
                 Logger.IfError()?.Message("#Critical: locomotionDef == null").Write();;
                return;
            }

            ____StopLocomotionClient____();
            if (IsServerLocoValid)
                ResetLocomotionServer_Soft(locomotionDef);
            else
                InitLocomotionServer_Hard(locomotionDef);
        }

        private void ResetLocomotionServer_Soft(MobLocomotionDef locomotionDef)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: ResetLocomotionServer_Soft");

            Debug.Assert(IsServerLocoValid); 
            Debug.Assert(!_isServerLocoActive);

   /*c*/    if (!_pZ_13761_GotServ)
   /*c*/        Logger.If(LogLevel.Error)?.Message($"PZ-13761: InitLocomotionServer_Soft called BEFORE GotServer ! ({EntityId})").UnityObj(gameObject).Write();
   /*c*/    else if (Dbg_PZ_13761_Dbg_Data != null) Dbg_PZ_13761_Dbg_Data.PZ_13761_InitLocoServ_calledAfter_GotServ = true;
   /*c*/
   /*c*/    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "--++== InitLocomotionServer [_]").Write();
   /*c*/    
   /*c*/    var startPosition = LocomotionHelpers.WorldToLocomotionVector(transform.position);
   /*c*/    Plh?.CurveLogger?.AddData("8) InitLocomotionServer_Soft", SyncTime.Now, startPosition);
   /*c*/    _virtualBodyS.Setup(startPosition, LocomotionVector.Zero, LocomotionHelpers.WorldToLocomotionOrientation(transform.rotation),
   /*c*/                                                    (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.ShouldSaveLocoVarsAfterAlarm   : (Func<bool>) null,
   /*c*/                                                    (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.SaveLocoVarsAfterAlarmIfShould : (Action<LocomotionVariables, Type>) null);
            _groundSensorS.Reset();
            _transformBodyS.Reset();
            _environmentS.Reset();
            _historyS.Reset();
            _stateMachineContextS.Reset();
            // ReSharper disable once PossibleNullReferenceException
            (_stateMachineS as IResettable).Reset();
            _sender.Restart();
            _guideProvider.Restart(OuterRef, ServerRepo);
            DirectMotionProducer.Clean();
            LocomotionAgentExtensions.SetLocomotionToEntity(true, _locomotionAgentS, DirectMotionProducer, _guideProvider, OuterRef, ServerRepo);
            _interpolationBufferSh.SwitchState(true);

            PawnWatchDog.RegisterServerPawn(this); 

            _isServerLocoActive = true;
        }

        // A mirror method is `ResetLocomotionServer_Soft`
        private void __________StopLocomotionServer__________(bool stopBeforeCleanHard = false) 
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: __________StopLocomotionServer__________");
            _isServerLocoActive = false;

            PawnWatchDog.UnregisterServerPawn(this);

            //subcr. is in GotServer
            if (ServerRepo != null)
                SubscribeUnsubscribeMobMovement(SubscribeUnsubscribe.Unsubscribe, ServerRepo);

            _moveActionsDoer?.Stop();

            // "!stopBeforeCleanHard" here, 'cos it would be just waste work, 'cos whole loco 'll now be cleaned (incl.all these nodes)
            // "IsServerLocoValid" - for call from `OnDestroy` - when all is cleaned already
            if (!stopBeforeCleanHard && IsServerLocoValid)
            {
                _interpolationBufferSh.SwitchState(false);
                _locomotionAgentS.Reset();
                if (ServerRepo != null)
                    LocomotionAgentExtensions.CleanLocomotionToEntity(_locomotionAgentS, OuterRef, ServerRepo);
                _guideProvider.Stop(OuterRef, ServerRepo);
                _sender.Stop();
            }
        }

        // Valid only on S. Are used for debug:
        private void InitLocomotionServer_Hard(MobLocomotionDef locomotionDef)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: InitLocomotionServer_Hard");

            if (IsDebug)
            {
                Debug.Assert(!IsServerLocoValid);
                Debug.Assert(!_isServerLocoActive);
            }

            try
            {

   /*c*/    Plh?.CurveLogger?.AddData("8) PathdingOwner", SyncTime.Now, 1);
   /*c*/    ///#PZ-13568: #Dbg:
   /*c*/    if (DbgLog.Enabled) DbgLog.Logger.If(LogLevel.Warn)?.Message($"#Dbg: PZ-13761: Pawn.InitLocomotionServer ({EntityId})").UnityObj(gameObject).Write();
   /*c*/    if (!_pZ_13761_GotServ)
   /*c*/        Logger.If(LogLevel.Error)?.Message($"PZ-13761: InitLocomotionServer called BEFORE GotServer ! ({EntityId})").UnityObj(gameObject).Write();
   /*c*/    else
   /*c*/        if (Dbg_PZ_13761_Dbg_Data != null) Dbg_PZ_13761_Dbg_Data.PZ_13761_InitLocoServ_calledAfter_GotServ = true;
   /*c*/
   /*c*/
   /*c*/    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "--++== InitLocomotionServer [_]").Write();
   /*c*/    //CleanLocomotionServer();
   /*c*/    var startPosition = LocomotionHelpers.WorldToLocomotionVector(transform.position);
   /*c*/    Plh?.CurveLogger?.AddData("8) InitLocomotionServer", SyncTime.Now, startPosition);
   /*c*/    _virtualBodyS.Setup(startPosition, LocomotionVector.Zero, LocomotionHelpers.WorldToLocomotionOrientation(transform.rotation),
   /*c*/                                                    (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.ShouldSaveLocoVarsAfterAlarm   : (Func<bool>)null,
   /*c*/                                                    (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.SaveLocoVarsAfterAlarmIfShould : (Action<LocomotionVariables, Type>)null);
            var settings = new MobLocomotionSettings(locomotionDef);
            var constants = new LocomotionConstants(locomotionDef.Constants);
            var stats = new MobStatsProvider(locomotionDef, constants);
            //var navMeshBody = new LocomotionNavMeshBody(_agent, groundSensor, debug);
            var navMeshAgent = new LocomotionNavMeshAgent(_agentS, EntityId, Plh,
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.OnNMAMoveEvent                 : (Action<NMAMoveType, (NMAMoveData, NMAMoveData)>)null,
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.ShouldSaveLocoVarsAfterAlarm   : (Func<bool>)null,
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.SaveLocoVarsAfterAlarmIfShould : (Action<LocomotionVariables, Type>)null, 
                                                            PZ_13761_Dbg_Data.Logger);
            _groundSensorS = new NavMeshGroundSensor(settings, _agentS, _virtualBodyS, Plh); 
            var physics = new LocomotionSimpleMovementWithGroundNode(_groundSensorS,
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.ShouldSaveLocoVarsAfterAlarm   : (Func<bool>)null,
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.SaveLocoVarsAfterAlarmIfShould : (Action<LocomotionVariables, Type>)null,
                                                            Plh);
            _transformBodyS = new LocomotionTransformBody(transform);
            _environmentS = new LocomotionEnvironment(settings, _groundSensorS, new LocomotionNullCollider(), IsLocomotionEnabled);
            _historyS = new LocomotionHistory(_transformBodyS, _environmentS);
            _stateMachineContextS = MobStateMachineContext.Create(                        
                input: _moveActionsDoer,
                body: _virtualBodyS,
                stats: stats,
                environment: _environmentS,
                history: _historyS,
                clock: _locomotionClockSh,
                constants: constants);
            _stateMachineS = MobStateMachine.Create(_stateMachineContextS, _moveActionsDoer.Reactions, Plh, 
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.ShouldSaveLocoVarsAfterAlarm   : (Func<bool>)null,
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.SaveLocoVarsAfterAlarmIfShould : (Action<LocomotionVariables, Type>)null);
            if (Dbg_PZ_13761_Dbg_Data != null)
            { 
                _stateMachineS.OnStartNewStateEvent   += Dbg_PZ_13761_Dbg_Data.OnSMStateStarted;
                _stateMachineS.OnFinishCurrStateEvent += Dbg_PZ_13761_Dbg_Data.OnSMStateFinished;
            }
            var boolVisibilityProvider = GetComponent<IBoolVisibilityProvider>();
            _relevanceProviderNetwork = new MobRelevanceLevelProviderServer(transform, settings.Server, boolVisibilityProvider);

            _relevanceProviderUpdateS = new PathfindingOwnerRelevanceLevelProvider();
            _relevanceSettingsUpdate = settings.Client; //?? ///#PZ-13761: Side question: is it Ok? may be should be "server"??
            // if (!IsHostMode)
            // {
            //     _relevanceSettingsUpdate = settings.Server;
            //     _relevanceProviderUpdate = relevanceProviderServer;
            // }
            // else
            // {
            //     _relevanceSettingsUpdate = settings.Client;
            // _relevanceProviderUpdate = new MobRelevanceLevelProviderClient(transform, _relevanceSettingsUpdate, _isVisibleByCamera.Value = new IsVisibleByCameraDetector());
            // }
            var networkTransport = new MobNetworkTransport(Ego.OuterRef, ServerRepo, (t) => AsyncUtils.RunAsyncTask(t), Plh);
            _sender = new LocomotionNetworkSender(networkTransport, _locomotionClockSh, _relevanceProviderNetwork, settings.Server, null, EntityId, Plh);
            var timestamp = new LocomotionTimestampNode(_locomotionClockSh, 0, 
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.ShouldSaveLocoVarsAfterAlarm   : (Func<bool>)null,
                                                            (Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.SaveLocoVarsAfterAlarmIfShould : (Action<LocomotionVariables, Type>)null);
            ///var spatialLegionary = GetComponent<SpatialLegionary>();
            
            ///#PZ-13761: #Dbg #Tmp:
            if (Dbg_PZ_13761_Enabled && Dbg_PZ_13761_Dbg_Data == null)
                Logger./*Debug*/Error("((((((((((((((((((((((((((((((((((((((((((( (1)  InitLocomotionServer. Dbg_PZ_13761_Dbg_Data == null )))))))))))))))))))))))))))))))))))))))))))))");
            var _dbgTeleportListener1 = (Dbg_PZ_13761_Dbg_Data != null) ? new DebugTeleportFlagListener(Dbg_PZ_13761_Dbg_Data.OnGotTeleportBeforeInterpolator) : null;
            var _dbgTeleportListener2 = (Dbg_PZ_13761_Dbg_Data != null) ? new DebugTeleportFlagListener(Dbg_PZ_13761_Dbg_Data.OnGotTeleportAfterInterpolator)  : null;

            _guideProvider = new MobGuideProvider(transform);
            _guideProvider.Restart(OuterRef, ServerRepo);

            //old: var directMotionProducer = DirectMotionProducer.Create(transform, () => _view.Value?.Animator, _virtualBodyS, _moveActionsDoer, OuterRef.Guid);
            DirectMotionProducer.Init(() => _view.Value?.Animator, _virtualBodyS, _moveActionsDoer, OuterRef.Guid);         ///TODO: ///@@@ STOPPED_HERE (по пайплайну)
            _locomotionAgentS = new LocomotionEngineAgent();
            LocomotionAgentExtensions.SetLocomotionToEntity(true, _locomotionAgentS, DirectMotionProducer, _guideProvider, OuterRef, ServerRepo);

            _interpolationBufferSh = ReinitInterpolationNode(settings, true);

            //#NOTE: [About shared nodes]:
            //  Both pipelines (Cl & S uses some shared nodes - has "_shared" suffix): all of them now has no Dispose implementation - so no problem.
            //  But! if any of shared nodes has .Dispose - it could be a problem - 'cos loco.Dispose - calls Dispose recursively & know nothing about shared nodes.
            //  In this case think about WrapperNode for each of these shared nodes, which .Dispose implementation decrements users-of-instance-counter & calls .Dispose of wrapee
            //  only when hits value 0.  //(More info in task #PZ-17746)

            _locomotionS = new LocomotionEngine(LocomotionDebugContext);
            _locomotionS
                .Updateables(
                    _locomotionClockSh,
                    _groundSensorS,
                    _environmentS,
                    _historyS,
                    DirectMotionProducer,
                    _stateMachineContextS
                )
                .Debugables(
                    _stateMachineContextS,
                    LocomotionDebug.GatherBody(_transformBodyS),
                    LocomotionDebugUnity.GatherRealVelocity(transform)
                    //this
                )
                .ComposePipeline(_virtualBodyS, a => a
                    .AddPass(_stateMachineS, b => b
                        .AddPass(physics, c => c
                            .AddPass(navMeshAgent, d => d
                                .AddCommit(_virtualBodyS)
#if DEBUG
                                .AddCommit(_dbgTeleportListener1)
#endif
                                .AddPass(timestamp, e => e
                                    .AddCommit(_sender)
                                    .AddPass(_interpolationBufferSh, f => f //#important: this node stores curr.frame, but returns frame stored .5sec ago(see ".AddPass(timestamp..").
                                        .AddCommit(_transformBodyS)
#if DEBUG
                                        .AddCommit(_dbgTeleportListener2)
#endif
                                        .AddCommit(LocomotionClientOnlyPipelineSh)
                                        .AddCommit(LocomotionDebug.GatherVariables)
                                        .AddCommit(_locomotionAgentS)
                                    ))))));

            //#PZ-17474: Draft - single Cl+S locomotion:
            // _locomotionTheOnlyClS = new LocomotionEngine(LocomotionDebugContext);
            // _locomotionTheOnlyClS
            //     .Updateables(
            //         _locomotionClock,
            //         ifS(groundSensor),
            //         ifS(_environmentS),
            //         ifS(history),
            //         ifS(DirectMotionProducer),
            //         ifS(stateMachineContext)
            //     )
            //     .Debugables(
            //         ifS(stateMachineContext),
            //         ifS(LocomotionDebug.GatherBody(_transformBodyS)),
            //         LocomotionDebug.GatherBody(kinematicBody),
            //         LocomotionDebugUnity.GatherRealVelocity(transform)
            //         //this
            //     )
            //     .ComposePipeline(_virtualBodyS, a => a
            //         .AddPass(_stateMachineS, b => b
            //             .AddPass(physics, c => c
            //                 .AddPass(navMeshAgent, d => d
            //                     .AddCommit(_virtualBodyS)
            //                     .AddCommit(_dbgTeleportListener1)
            //                     .AddPass(timestamp, e => e
            //                         .AddCommit(_sender)
            //        /*Shared*/         .AddPass(new LocomotionInterpolationBufferNode(_locomotionClock, settings.Server, EntityId, Plh, shouldSaveVars, saveVarsCallback), f => f //#important: this node stores curr.frame, but returns frame stored .5sec ago(see ".AddPass(timestamp..").
            //                             .AddCommit(_transformBodyS)
            //                             .AddCommit(_dbgTeleportListener2)
            //        /*Shared*/           .AddCommit(LocomotionClientOnlyPipeline)
            //        /*Shared*/           .AddCommit(LocomotionDebug.GatherVariables)
            //                             .AddCommit(_locomotionAgentS)
            //                         ))))));

                if (IsDebug)
                { 
                    ///#PZ-13761: #Dbg #Tmp:
                    if (Dbg_PZ_13761_Enabled && Dbg_PZ_13761_Dbg_Data == null)
                        Logger./*Debug*/Error("((((((((((((((((((((((((((((((((((((((((((( (2)  InitLocomotionServer. Dbg_PZ_13761_Dbg_Data == null )))))))))))))))))))))))))))))))))))))))))))))");
                    Dbg_PZ_13761_Dbg_Data?.SubscribeToLocoNMANode(navMeshAgent);
                }
                _isServerLocoActive = true;
            }
            catch (Exception e)
            {
                CleanLocomotion_Hard(true);
                Logger.Error($"Got esception, so Loco 'll be cleaned_hard. Exception: \"{e}\"");
                throw e;
            }

            // Do 1 update independent of SimulationLevel to applicate position from rigid body to entity:
            // ReSharper disable once MustUseReturnValue
            LocomotionUpdate();
        }

        LocomotionInterpolationBufferNode ReinitInterpolationNode(MobLocomotionSettings settings, bool initialStateIsServer)
        {
            if (_interpolationBufferSh == null)
            {
                var shouldSaveVars   = (IsDebug && initialStateIsServer && Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.ShouldSaveLocoVarsAfterAlarm   : (Func<bool>)null;
                var saveVarsCallback = (IsDebug && initialStateIsServer && Dbg_PZ_13761_Dbg_Data != null) ? Dbg_PZ_13761_Dbg_Data.SaveLocoVarsAfterAlarmIfShould : (Action<LocomotionVariables, Type>)null;

                _interpolationBufferSh = new LocomotionInterpolationBufferNode(_locomotionClockSh, settings.Client, settings.Server, initialStateIsServer, EntityId, Plh, shouldSaveVars, saveVarsCallback);
            }
            else
                _interpolationBufferSh.SwitchState(initialStateIsServer);

            return _interpolationBufferSh;
        }

        private void InitLocomotionClient(MobLocomotionDef locomotionDef)
        {
            if (_locomotionCl != null)
                ResetLocomotionClient_Soft(LocomotionDef);
            else
                InitLocomotionClient_Hard(LocomotionDef);
        }

        private void ResetLocomotionClient_Soft(MobLocomotionDef locomotionDef)
        {
            ///#PZ-17474: #Dbg:
            if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: ResetLocomotionClient_Soft");

            Debug.Assert(_locomotionCl != null); 

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, $"--++== ResetLocomotionClient_Soft (IsHostMode:{IsHostMode})").Write();

            _kinematicBodyCl.Reset();
            _receiver.Restart();
            _damperNode.Reset();
        }

        //Should be called only when we're switching to server loco mode. A mirror method is `ResetLocomotionClient_Soft`
        private void ____StopLocomotionClient____()
        {
            ///#PZ-17474: #Dbg:
            if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: ____StopLocomotionClient____");
            
            _receiver?.Stop();
        }


        //@param: `initEvenIfHost` is used when we are losing isServer, but it's still true right now:
        private void InitLocomotionClient_Hard(MobLocomotionDef locomotionDef)
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: InitLocomotionClient_Hard"); //(evenIfHost:{initEvenIfHost})");

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, $"--++== InitLocomotionClient (IsHostMode:{IsHostMode})").Write();

            var settings = new MobLocomotionSettings(locomotionDef);

            // client relevance level for update is prevail

            bool shouldInitClLoco = !IsServer;/*IsHostMode*/ //|| initEvenIfHost;       ///#PZ-17474: ?? to_Vova_I: В хост режиме всё точно так же? Т.е. IsServer у нас тут всегда false. В смысле, в хост-режиме у нас нет такого, что мобы IsServer и IsClient но !PathOwnership?
            if (shouldInitClLoco) //кулинг на случай host-режима: _locomotionCl не собираем
            {
                //no_need: CleanLocomotion_Hard(false);
                _kinematicBodyCl = new LocomotionKinematicBody(_rigidbody, Plh);
                var transport = new MobNetworkTransport(Ego.OuterRef, ClientRepo, (t) => AsyncUtils.RunAsyncTask(t), Plh);
                //                var extrapolator = new LocomotionExtrapolatorWithCorrection(settings.Client);
                _receiver = new LocomotionNetworkReceiver(transport, _locomotionClockSh, EntityId, null, Plh);
                _relevanceSettingsUpdate = settings.Client;
                _relevanceProviderUpdateCl = new MobRelevanceLevelProviderClient(transform, _relevanceSettingsUpdate, _isVisibleByCamera.Value = new IsVisibleByCameraDetector());
                _damperNode = new LocomotionDamperNode(settings, _kinematicBodyCl, Plh, EntityId);

                _interpolationBufferSh = ReinitInterpolationNode(settings, false);

                //#NOTE: [About shared nodes]:
                //  Both pipelines (Cl & S uses some shared nodes - has "_shared" suffix): all of them now has no Dispose implementation - so no problem.
                //  But! if any of shared nodes has .Dispose - it could be a problem - 'cos loco.Dispose - calls Dispose recursively & know nothing about shared nodes.
                //  In this case think about WrapperNode for each of these shared nodes, which .Dispose implementation decrements users-of-instance-counter & calls .Dispose of wrapee
                //  only when hits value 0.  //(More info in task #PZ-17746)

                _locomotionCl = new LocomotionEngine(LocomotionDebugContext);
                _locomotionCl
                    .Updateables(
                        _locomotionClockSh
                    )
                    .Debugables(
                        LocomotionDebug.GatherBody(_kinematicBodyCl),
                        LocomotionDebugUnity.GatherRealVelocity(transform)
                    )
                    .ComposePipeline(_receiver, a => a
                        .AddPass(_interpolationBufferSh, b => b
                            .AddPass(new LocomotionSimpleExtrapolationNode(settings.Client, _locomotionClockSh, EntityId, Plh), c => c // extrapolation is optional
                                //#Tmp replaced by debug:
                                .AddPass(_damperNode, d => d //.AddPass(new LocomotionDamperNode(settings, kinematicBody), d => d
                                    .AddCommit(_kinematicBodyCl)
                                    .AddCommit(LocomotionClientOnlyPipelineSh)
                                    .AddCommit(LocomotionDebug.GatherVariables)
                                ))));
            }

            if (!(IsHostMode && GlobalConstsHolder.GlobalConstsDef.DebugMobs_OffClLocoInHostMode) && !_locomotionClientOnlyPipelineInited)
            {
                LocomotionClientOnlyPipelineSh.AddCommit(new MobCommitToAnimator(_clientView.Animator, settings, _clientView.GameObject.transform, EntityId, Plh));
                var groundAligner = _clientView.GameObject.GetComponent<RaycastOnGroundAligner>();
                if (groundAligner != null)
                    LocomotionClientOnlyPipelineSh.AddCommit(groundAligner.Init(transform, PhysicsLayers.BuildMask));

                _locomotionDebugTrailInstance?.Initialize(() => Assets.Src.Camera.GameCamera.Camera);
                _locomotionClientOnlyPipelineInited = true;
            }
            if (shouldInitClLoco) //кулинг на случай host-режима: _locomotionCl не собираем
                // Do 1 update independent of SimulationLevel to applicate position from rigid body to entity:
                // ReSharper disable once MustUseReturnValue
                LocomotionUpdate();
        }

        void CleanLocomotion_Hard(bool isServer) 
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: CleanLocomotion_Hard({isServer})");

            if (isServer)
                //_isServerLocoActive = false;
                __________StopLocomotionServer__________(true);

            //DirectMotionProducer.RemoveFrom(transform);  //#no_need: 'cos this method (CleanLocomotion_Hard) is called when Pawn 'll be destroyed soon.
            DisposeLocomotion(isServer);
        }

        private void DisposeLocomotion(bool isServer)
        {
            var loco = isServer ? _locomotionS : _locomotionCl;
            if (loco != null)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "DisposeLocomotion").Write();;
                loco.Dispose();
                if (isServer)
                    _locomotionS = null;
                else
                    _locomotionCl = null;
            }
            if (!isServer) //is Cl
                _locomotionDebugTrailInstance?.Dispose();

            if (isServer && _guideProvider != null)
            {
                _guideProvider.Stop(OuterRef, ServerRepo);
                _guideProvider = null;
            }

            _relevanceProviderNetwork = null;
            if (isServer)
                _relevanceProviderUpdateS = null;
            else
                _relevanceProviderUpdateCl = null;
        }

        private LocomotionDebug.Context LocomotionDebugContext
        {
            get
            {
                if (_locomotionDebugContext == null)
                {
                    if (ServerProvider.IsClient /*IsClient*/ /*IsHostMode*/)
                    {
                        if (ClientCheatsState.DebugInfo) // if debug mode enabled
                        {
                            var debugProxy = new DebugProxy();
                            _locomotionDebugTrailInstance = _locomotionDebugTrail ? Instantiate(_locomotionDebugTrail, transform) : null;
                            _locomotionDebugContext =
                                new LocomotionDebug.Context(EntityId,
                                    new LocomotionCompositeDebugAgent(
                                        debugProxy,
                                        _locomotionDebugTrailInstance,
                                        new LocomotionLoggerAgent(),
                                        new LocomotionDebugDraw()
                                    ));
                            _locomotionDebugProvider = debugProxy;
                        }
                    }
                }

                return _locomotionDebugContext;
            }
        }

        protected override void OnWakeup()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: OnWakeup");

            IsClose = true;

            //            PawnBecameNear?.Invoke();
            //            OnGotFirstObserver();
            _simulationLevelServer.Raise(SimulationLevel.Full);

            ///#Dbg:
            // if (GlobalConstsHolder.Dbg_Mobs7910 && _relevanceProvider.ClosestObserverDistance > 100f)
            //     if (_relevanceProvider.Dbg_ClosestObserverDistance_Forced_DANGER > 100f)
            //         Logger.Warn(
            //             $"OnWakeup, but dist > 100: {_relevanceProvider.ClosestObserverDistance}. {EntityId}. Pos: {transform.position}"); // It's ok now - looks like it can't be correctly updated at this moment
        }

        protected override void OnFallAsleep()
        {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (LogAll || EntityInDebugFocus.Mob == this)) DbgLog.Log(17474, $"pz-17474: {EntityId}: OnFallAsleep"); /// Похоже никогда не срабатывает

            IsClose = false;
            //            PawnBecameFar?.Invoke();
            //            OnLostLastObserver();
            _simulationLevelServer.Lower(SimulationLevel.Faraway);
        }

        // --- API -------------------------------------------------------------------

        internal void ForgetView([NotNull] ISubjectView view)
        {
            if (_clientView == view)
            {
                _locomotionClientOnlyPipelineSh?.Clear();
                _locomotionDebugTrailInstance?.Dispose();
                _clientView = null;
            }
            else if (_clientView != null)
                Logger.IfWarn()?.Message(EntityId, $"ForgetViewGo called, but `_clientView`({_clientView}) != `view`({view}").Write();
        }
        
        private void PlaceOnSurface(Vector3 position)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Try to place mob at position {0}",  position).Write();
            //var hit = GetSurface(position);
            var hit = LocomotionNavMeshAgent.GetSurface(gameObject, position, _agentS.areaMask);;
            if (hit.hit)
            {
                if (!Vector3Extension.Approx3(hit.position, position, 0.01f))
                    Logger.IfDebug()?.Message(EntityId, "Mob's initial position changed from {0} to {1}",  position, hit.position).Write();
                transform.position = hit.position;

                //#Dbg:
                if (Vector3Extension.Approx3(transform.position, Vector3.zero, 10f))
                    Logger.IfWarn()?.Message("#Mobs: PlaceOnSurface(pos:{0}) got hit, but placed near 0,0,0: {1} (g.o.:{2})", position, transform.position, gameObject.name)
                        .UnityObj(gameObject)
                        .Write();
                //if (LocomotionNavMeshAgent.Logger.IsDebugEnabled) LocomotionNavMeshAgent.Logger.IfDebug()?.Message("#WARP: " + hit.position).Write();

                if (Dbg_PZ_13761_Enabled)
                { 
                    var log1 = new NMAMoveData(_agentS, hit.position);

                    _agentS.Warp(hit.position);
                    
                    var log2 = new NMAMoveData(_agentS, NMAMoveData.NanV3);
                    ///#PZ-13568: #Dbg:
                    Dbg_PZ_13761_Dbg_Data?.OnNMAMoveEvent(NMAMoveType.P_Warp, (log1, log2));
                }
                else
                    _agentS.Warp(hit.position);
            }
            else
            {
                transform.position = position;
                //#Dbg:
                if (Vector3Extension.Approx3(transform.position, Vector3.zero, 10f))
                    Logger.IfWarn()?.Message("#Mobs: PlaceOnSurface(pos:{0}) didn't got hit, & placed near 0,0,0: {1} (g.o.:{2})", position, transform.position,
                            gameObject.name)
                        .UnityObj(gameObject)
                        .Write();
            }
        }

        public static int Label(string text, int index, Rect texRect)
        {
            var rect = texRect;
            rect.position += index * new Vector2(0, rect.size.y);
            GUI.Label(rect, text);
            index++;
            return index;
        }

        struct SimulationLevelCoordinator
        {
            private readonly Action<SimulationLevel, bool> _applier;
            private readonly Pawn _pawn;
            private State _state;
            private SimulationLevel _simulationLevel;
            private SimulationLevel _reqSimulationLevel;
            internal SimulationLevel ReqSimulationLevel => _reqSimulationLevel;

            public SimulationLevelCoordinator(Action<SimulationLevel, bool> applier, SimulationLevel initLevel, Pawn pawn)
            {
                _state = State.NotInitialized;
                _simulationLevel = SimulationLevel.Off;
                _reqSimulationLevel = initLevel;
                _applier = applier;
                _pawn = pawn;
            }

            public static implicit operator SimulationLevel(SimulationLevelCoordinator self)
            {
                return self._simulationLevel;
            }

            public void ChangeState(State state)
            {
                if (_state == state)
                    return;

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_pawn.EntityId, "State: {0} => {1}",  _state, state).Write();

                switch (state)
                {
                    case State.Dead:
                        Lower(SimulationLevel.Off);
                        _state = state;
                        break;
                    case State.Alive:
                        _state = state;
                        Raise(_reqSimulationLevel);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public void Raise(SimulationLevel simulationLevel)
            {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (_pawn.LogAll || EntityInDebugFocus.Mob == _pawn)) DbgLog.Log(17474, $"pz-17474: {_pawn.EntityId}: SimLvl.Raise (curr:{_reqSimulationLevel}(req:{_reqSimulationLevel}) --> {simulationLevel}");

                if (_reqSimulationLevel < simulationLevel)
                    _reqSimulationLevel = simulationLevel;
                if (_state <= State.Dead)
                    return;
                if (_simulationLevel >= simulationLevel)
                    return;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_pawn.EntityId, "Raise Simulation Level: {0} => {1}",  _simulationLevel, simulationLevel).Write();
                while (_simulationLevel < simulationLevel)
                {
                    _applier(++_simulationLevel, true);
                }

                _pawn.LogPawnState();
            }

            public void Lower(SimulationLevel simulationLevel)
            {
///#PZ-17474: #Dbg:
if (DbgLog.Enabled && (_pawn.LogAll || EntityInDebugFocus.Mob == _pawn)) DbgLog.Log(17474, $"pz-17474: {_pawn.EntityId}: SimLvl.Lower (curr:{_reqSimulationLevel}(req:{_reqSimulationLevel}) --> {simulationLevel}");

                if (_reqSimulationLevel > simulationLevel)
                    _reqSimulationLevel = simulationLevel;
                if (_state <= State.Dead)
                    return;
                if (_simulationLevel <= simulationLevel)
                    return;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_pawn.EntityId, "Lower Simulation Level: {0} => {1}",  _simulationLevel, simulationLevel).Write();
                while (_simulationLevel > simulationLevel)
                    _applier(_simulationLevel--, false);
                _pawn.LogPawnState();
            }

            public override string ToString() => _reqSimulationLevel.ToString();
        }

        ///#PZ-17474: #Dbg:
        bool LogAll = true;

    }
}