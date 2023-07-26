using System;
using System.Collections;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Player;
using Assets.Src.Aspects.Doings;
using Assets.Src.Character;
using Assets.Src.Shared;
using UnityEngine;
using Assets.Src.Camera;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using NLog;
using Assets.Src.Target;
using OutlineEffect;
using SharedCode.EntitySystem;
using Src.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine.Serialization;
using ColonyShared.SharedCode.Aspects.Locomotion;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Src.Aspects.Doings;
using Src.Aspects.Impl;
using UnityEngine.Assertions;
using System.Linq;
using Assets.Src.Aspects.Impl.Factions.Template;
using System.Threading.Tasks;
using Assets.Src.Aspects.Templates;
using Assets.Src.Cartographer;
using Assets.Src.Lib.Cheats;
using Assets.Src.Server.Impl;
using ColonyShared.SharedCode.Utils;
using Src.Aspects.Locomotion;
using Assets.Src.Aspects.VisualMarkers;
using Assets.Src.BuildingSystem;
using Assets.Src.Character.Events;
using Assets.Src.GameObjectAssembler;
using Assets.Src.Locomotion.Debug;
using Assets.Src.NetworkedMovement;
using ColonyShared.GeneratedCode.Shared.Aspects;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using ReactivePropsNs;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Entities;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using static SharedCode.Wizardry.UnityEnvironmentMark;

using Transform = UnityEngine.Transform;
using static SharedCode.Aspects.Item.Templates.Constants;
using PositionRotationUnity = Assets.Src.Aspects.Impl.PositionRotation;
using SharedCode.Wizardry;
using SharedCode.Entities.Engine;
using Uins;
using SharedCode.Serializers;

namespace Assets.Src.Aspects.Impl
{
    public class CharacterPawn : EntityGameObjectComponent, ICameraRigHolder, IDumpingLoggerProvider, IMutationStageProvider, ICharacterPawn
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static readonly CharacterNetworkTransport NetworkTransport = new CharacterNetworkTransport((t) => AsyncUtils.RunAsyncTask(t));
        private static readonly CharacterPawnScheduler Scheduler = new CharacterPawnScheduler();
        private static readonly Lazy<LayerMask> GroundLayers = new Lazy<LayerMask>(() => PhysicsLayers.BuildMask);
        private static readonly Lazy<LayerMask> IgnoreCollisionLayers = new Lazy<LayerMask>(() => PhysicsLayers.ActiveMask | PhysicsLayers.CharacterControllerMask);
        private static readonly Lazy<GameObjectMatcher> SoftCollisionMatcher = new Lazy<GameObjectMatcher>(() => new GameObjectMatcher(
            layers: PhysicsLayers.ActiveMask,
            tags: new[] {GameObjectTags.SoftColliderTag, GameObjectTags.MobDangerousTag, GameObjectTags.MobHarmlessTag}));

        [SerializeField] private PlayerCameraRigSpawner _cameraSpawner;
        [SerializeField] private MockCharacterView _mockViewPrefab;
        [SerializeField] private SetTargetCone _setTargetConePrefab;
        [SerializeField] private PlayerBrain _playerBrainPrefab;
        [SerializeField] private BotBrain _botBrainPrefab;
        [SerializeField] private GameObject _hitColliderPlace; 
        [SerializeField] private GameObject _softColliderPlace; 
        [FormerlySerializedAs("_locomotionTrail")] [SerializeField] private LocomotionDebugTrail _locomotionDebugTrail;
        [SerializeField] private float _onDieDelay = 3f;

        private ISpellDoer _spellDoer;
        private IDisposableCharacterBrain _characterBrain;
        private CalcersCache _locomotionCalcersCache;
        private ILocomotionEngine _locomotion;
        private LocomotionPipeline _locomotionClientOnlyPipeline;
        private bool _locomotionClientOnlyInitialized;
        private ILocomotionDebugInfoProvider _locomotionDebugProvider;
        private LocomotionDebug.Context _locomotionDebugContext; //v
        private IDisposable _locomotionReactions;
        private LocomotionDebugTrail _locomotionDebugTrailInstance;
        private PlayerCameraRig _cameraRig;
        private TargetHolder _targetHolder;
        private SetTargetCone _setTarget;
        private AttackDoer _attackDoer;
        private readonly List<Component> _locomotionComponents = new List<Component>();
        private StreamingAgent _characterStreamerAgent = null;
        private VisualMarkerSelector _visualMarkerSelector;
        private (MutationStageDef Actual, MutationStageDef View) _mutation;
        private (GenderDef Actual, GenderDef View) _gender;
        private readonly ReactiveProperty<ICharacterView> _view = new ReactiveProperty<ICharacterView>();
        private DumpingLogger _dLogger;
        private CharacterPositionLoggingHandler _positionLogger = new CharacterPositionLoggingHandler();
        private int _logBackCounter;
        private WorldCharacterDef CharacterDef => (WorldCharacterDef) Ego.EntityDef;

        //=== Props ===========================================================

        public ICameraRig CameraRig => _cameraRig;
        public IGuideProvider CameraGuideProvider => _characterBrain;
        public ICharacterBrain Brain => _characterBrain;
        public ICharacterBuildInterface BuildInterface => _characterBrain.BuildInterface;
        public Dictionary<object, Texture> DebugTextures { get; set; } = new Dictionary<object, Texture>();
        public ILocomotionDebugInfoProvider LocomotionDebugInfo => _locomotionDebugProvider;
        public LocomotionDebugTrail LocomotionDebugTrail => _locomotionDebugTrailInstance;
        public bool Alive { get; private set; }
        public MutationStageDef MutationStage => _mutation.Actual;
        public OuterRef EntityRef => Ego.OuterRef;
        public IEntitiesRepository Repository => Ego.ClientRepo;
        IReactiveProperty<ISubjectView> ISubjectPawn.View => _view;
        IReactiveProperty<IEntityView> IEntityPawn.View => _view;
        public ISpellDoer SpellDoer => _spellDoer;
        public DumpingLogger DLogger => _dLogger;
        public new bool HasAuthority => base.HasAuthority;

        public int LogBackCounter
        {
            get => _logBackCounter;
            set
            {
                if (_dLogger == null)
                    return;

                if (!_dLogger.Activate())
                    Logger.IfError()?.Message($"Can't activate {nameof(DumpingLogger)}.").Write();
                _logBackCounter = value;
            }
        }

        // --- Unity: ------------------------------------------------

        /*protected override void AwakeInternal()
        {
            _view.Value = null;

            if (false)
            {
                _dLogger = new DumpingLogger("Default", this);
                if (!_dLogger.Activate())
                    Logger.IfError()?.Message("Can't activate " + nameof(DumpingLogger)).Write();
            }
            
        }*/
        public void Start()
        {
            Scheduler.RegisterCharacter(this);
        }
        
        public void DoUpdate(float deltaTime)
        {
            _attackDoer?.Update();
            _locomotion?.Execute(deltaTime);
        }
        
        // This comp. is destroyed only on the end of game session
        protected override void DestroyInternal()
        {
            SurvivalGuiNode.Instance?.OnPawnDestroy(this);
            Scheduler.UnregisterCharacter(this);
        }


        // --- EGO: ---------------------------------------------------------

        private bool _gotClientInvoked = false;
        protected override void GotClient()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi]: Got Client{(IsHostMode ? " (Host Mode)" : "")} <{Ego.EntityId}>").Write();

            if (!_isClInitialized)
            {
                var repo = ClientRepo;
                var @ref = GetOuterRef<IEntity>();
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await repo.Get(@ref))
                    {
                        var worldCharacter = wrapper?.Get<IWorldCharacterClientBroadcast>(@ref, ReplicationLevel.ClientBroadcast);
                        if (worldCharacter != null)
                        {
                            _mutation.Actual = worldCharacter.MutationMechanics.Stage;
                            worldCharacter.MutationMechanics.SubscribePropertyChanged(nameof(worldCharacter.MutationMechanics.Stage), OnMutationChanged);

                            _gender.Actual = worldCharacter.Gender;
                            worldCharacter.SubscribePropertyChanged(nameof(worldCharacter.Gender), OnGenderChanged);

                            if (await worldCharacter.Mortal.GetIsAlive())
                            {
                                var xform = worldCharacter.MovementSync.Transform;
                                var pose = new PositionRotationUnity((Vector3)xform.Position, (Quaternion)xform.Rotation);
                                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                                {
                                    OnSpawnViewU(ServerOrClient.Client, pose);
                                    GotClientAuthorityAction?.Invoke();
                                    _gotClientInvoked = true;
                                });
                            }
                        }
                    }
                });
                
                if (!IsHostMode)
                    _positionLogger.SubscribeUnsubscribe(OuterRef, repo, SubscribeUnsubscribe.Subscribe);
            }
            SurvivalGuiNode.Instance?.OnPawnSpawn(this);
        }

        private async Task OnMutationChanged(EntityEventArgs args)
        {
            _mutation.Actual = (MutationStageDef)args.NewValue;
            var repo = ClientRepo;
            var @ref = GetOuterRef<IEntity>();
            using (var wrapper = await repo.Get(@ref))
            {
                var worldCharacter = wrapper?.Get<IWorldCharacterClientBroadcast>(@ref, ReplicationLevel.ClientBroadcast);
                if (worldCharacter != null && await worldCharacter.Mortal.GetIsAlive())
                    UnityQueueHelper.RunInUnityThreadNoWait(() => CleanAndInit(CharacterDef));
            }
        }

        private async Task OnGenderChanged(EntityEventArgs args)
        {
            _gender.Actual = (GenderDef)args.NewValue;
            var repo = ClientRepo;
            var @ref = GetOuterRef<IEntity>();
            using (var wrapper = await repo.Get(@ref))
            {
                var worldCharacter = wrapper?.Get<IWorldCharacterClientBroadcast>(@ref, ReplicationLevel.ClientBroadcast);
                if (worldCharacter != null && await worldCharacter.Mortal.GetIsAlive())
                    UnityQueueHelper.RunInUnityThreadNoWait(() => CleanAndInit(CharacterDef));
            }
        }

        private Action GotClientAuthorityAction = null;
        protected override void GotClientAuthority()
        {
            Action action = () =>
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi]: Got Client Authority{(IsHostMode ? " (Host Mode)" : "")} <{Ego.EntityId}>").Write();
                if (_isSpawnPositionGot) // нельзя вызывать InitAuthClientU до прихода Resurrect, так как без него у нас нет координат спавна 
                    InitAuthClientU(CharacterDef);
                else
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi] Postpone initialization for Resurrect <{Ego.EntityId}>").Write();
            };

            if (_gotClientInvoked)
            {
                action();
            }
            else
            {
                GotClientAuthorityAction = action;
            }
        }

        protected override void GotServer()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi]: Got Server{(IsHostMode ? " (Host Mode)" : "")} <{Ego.EntityId}>").Write();
            var repo = ServerRepo;
            var @ref = GetOuterRef<IEntity>();
            var characterDef = CharacterDef;
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(@ref))
                {
                    var worldCharacter = wrapper?.Get<IWorldCharacterAlways>(@ref, ReplicationLevel.Always);
                    if (worldCharacter != null && await worldCharacter.Mortal.GetIsAlive())
                    {
                        UnityQueueHelper.RunInUnityThreadNoWait(() =>
                        {
                            InitServerU(characterDef);
                        });
                    }
                }
            });

            _positionLogger.SubscribeUnsubscribe(OuterRef, repo, SubscribeUnsubscribe.Subscribe);
            
            if (_dLogger != null)
                _dLogger.IsServer = true;
        }
        protected override void LostServer()
        {
            CleanServer();
        }

        protected override void LostClient()
        {
            if (!HasClientAuthority)
                CleanClient();

            var repo = ClientRepo;
            if (repo != null)
            {
                var @ref = GetOuterRef<IEntity>();
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await repo.Get(@ref))
                    {
                        var worldCharacter = wrapper?.Get<IWorldCharacterClientBroadcast>(@ref, ReplicationLevel.ClientBroadcast);
                        if (worldCharacter != null)
                        {
                            worldCharacter.MutationMechanics.UnsubscribePropertyChanged(nameof(worldCharacter.MutationMechanics.Stage), OnMutationChanged);
                        }
                    }
                });
            }
        }

        protected override void LostClientAuthority()
        {
            CleanAuthClient();
        }

        public GameObject DetachView()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].DetachView <{Ego.EntityId}> {_view.Value}").Write();
            var view = _view.Value;
            _view.Value = null;
            GameObject viewGo = null;
            if (view != null)
            {
                viewGo = view.GameObject;
                view.Detach(OuterRef, Repository);
                GetComponent<VisualEventProxy>()?.SetRoot(viewGo);
            }
            return viewGo;
        }

        //#Note: Is called via `OnDie` by `DeathResurrectHandler`
        public void OnDieU(ServerOrClient context)
        {
            switch (context)
            {
                case ServerOrClient.Server:
                    {
                        Assert.IsTrue(IsServer);
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].OnDie({context}). ({Ego.EntityId}). [Server] `IsHostMode`:{IsHostMode}.").Write();
                        CleanServer();
                        break;
                    }
                case ServerOrClient.Client:
                    {
                        Assert.IsTrue(IsClient);
                        if (HasClientAuthority)
                        {
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].OnDie({context}). ({Ego.EntityId}). [AuthoritativeClient].").Write();
                            GameState.Instance.StartInstrumentedCoroutine(WaitThenOnDieDoAuthClient());
                        }
                        else
                        {
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].OnDie({context}). ({Ego.EntityId}). [Client].").Write();
                            CleanClient();
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(context), context, null);
                    }
            }
        }

        public void OnSpawnViewU(ServerOrClient context, PositionRotationUnity pose, bool isResurrectionAfterDeath = false)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].OnSpawnViewU({context}, {pose.Position}) <{Ego.EntityId}>").Write();
            transform.position = pose.Position;
            transform.rotation = pose.Rotation;
            _isSpawnPositionGot = true;
            
            var characterDef = CharacterDef;

            switch (context)
            {
                case ServerOrClient.Server:
                    {
                        Assert.IsTrue(IsServer);
                        //Logger.IfInfo()?.Message($"UI.[ChClVi].OnResurrect({context}). ({Ego.EntityId}). [Server] `IsHostMode`:{IsHostMode}.").Write();
                        InitServerU(characterDef);
                        break;
                    }
                case ServerOrClient.Client:
                    {
                        Assert.IsTrue(IsClient);
                        if (HasClientAuthority)
                        {
                            //Logger.IfInfo()?.Message($"UI.[ChClVi].OnResurrect({context}). ({Ego.EntityId}). [AuthoritativeClient].").Write();
                            InitAuthClientU(characterDef, isResurrectionAfterDeath);
                        }
                        else
                        {
                            //Logger.IfInfo()?.Message($"UI.[ChClVi].OnResurrect({context}). ({Ego.EntityId}). [Client].").Write();
                            InitClientU(characterDef, isResurrectionAfterDeath);
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(context), context, null);
                    }
            }
        }

        //#Note: Is called via `OnResurrect` by `DeathResurrectHandler`
        public void OnResurrectU(ServerOrClient context, PositionRotationUnity pose)
        //#Note: On entity/object cre-tion could be called multiple times by `DeathResurrectHandler`.  Если не будет работать Ок, то можно ...
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].OnResurrectU({context}, {pose.Position}) <{Ego.EntityId}>").Write();
            OnSpawnViewU(context, pose, true);
        }


        // --- Privates: ------------------------------------------------------------------------------------

        private bool _isAuthClInitialized;
        private bool _isClInitialized;
        private bool _isServerInitialized;
        private bool _isSpawnPositionGot;

        private void InitServerU(WorldCharacterDef def)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].Init[S]({Ego.EntityId}) --- -- -  .").Write();
            if (_isServerInitialized)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Already initialized S, so do nothing.").Write();;
                return;
            }

            var hitCollider = CreateHitCollider(def);
            InitServerLocomotion(def.Locomotion, hitCollider);

            _isServerInitialized = true;
        }

        private void InitClientU(WorldCharacterDef def, bool isResurrectionAfterDeath = false)
        {
            if (HasClientAuthority) // we'll init by `InitAuthClientU`
                return;
            
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].Init[Cl]({Ego.OuterRef}) --- -- -  .").Write();
            if (_isClInitialized)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Already initialized Cl, so do nothing.").Write();;
                return;
            }

            InstantiateAndInitViewIfShould(isResurrectionAfterDeath).InitClient(Ego.OuterRef);
            _view.Value.SetGuideProvider(new DummyGuideProvider(transform));
            var hitCollider = CreateHitCollider(def);
            InitClientLocomotion(def.Locomotion, hitCollider);
            _isClInitialized = true;

#if PLAYERLESS_HOST
            if(IsHostMode)
            {
                // в режиме "хост-без-игрока" прицепляем камеру к клиенту заспавненному на хосте для наблюдения зан ним
                _cameraRig = _cameraSpawner.Spawn();
                _cameraRig.Follow = transform;
                _cameraRig.LookAt = transform;
            }
#endif
        }

        private void InitAuthClientU(WorldCharacterDef def, bool isResurrectionAfterDeath = false)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].InitAuthClient({Ego.OuterRef}) - -- --- ---- ----- ---- --- -- - -- --- ---- ----- ---- --- -- -  .").Write();
            if (_isAuthClInitialized)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Already initialized AuthCl, so do nothing.").Write();;
                return;
            }

            InstantiateAndInitViewIfShould(isResurrectionAfterDeath).InitClientAuthority(Ego.OuterRef); //can be called from GotClAuth before Resurrect invoke (where `InstantiateAndInitViewIfShould` is called)
            CleanAuthClient(false);
            var hitCollider = CreateHitCollider(def);
            _targetHolder = gameObject.AddComponent<TargetHolder>();
            _targetHolder.HasAutority = true;
            _attackDoer = new AttackDoer(GetOuterRef<IHasAttackEngineClientFull>(), ClientRepo, _view.Value.AttackDoerSupport, _view.Value.AttackSubscriptionHandlers, _view.Value.GameObject.transform);

            _isAuthClInitialized = true;

            AsyncUtils.RunAsyncTask(async () =>
            {   
                BotActionDef botBrainDef = default;
                //using (var character = await ClientRepo.Get<IWorldCharacter>(Ego.EntityId))
                //botBrainDef = character.Get<IWorldCharacterClientFull>(Ego.EntityId)?.BotBrainDef;

                var isBot = false;
                using (var wrapper = await ClientRepo.GetFirstService<IBotCoordinatorServer>())
                {
                    if (wrapper == null)
                         Logger.IfError()?.Message("IBotCoordinator not found {0}",  ClientRepo.Id).Write();

                    IBotsHolderServer botsIds = default;
                    var service = wrapper?.GetFirstService<IBotCoordinatorServer>();
                    if (service?.BotsByAccount?.TryGetValue(Ego.ClientRepo.Id, out botsIds) ?? false)
                    {
                        isBot = botsIds.Bots.ContainsKey(Ego.OuterRefEntity.Guid);
                    }
                }
                if (isBot)
                    botBrainDef = new BotActionDef();

                ISpellDoer spellDoer;
                using (var charCnt = await ClientRepo.Get(TypeId, EntityId))
                    spellDoer = charCnt.Get<IHasWizardEntityClientFull>(TypeId, EntityId, ReplicationLevel.ClientFull).SlaveWizardHolder.SpellDoer;

                UnityQueueHelper.RunInUnityThreadNoWait(async () =>
                {
                    var brain = await CreateBrain(spellDoer, botBrainDef);
                    Assert.IsNotNull(brain);
                
                    _view.Value.SetGuideProvider(brain);

                    InitAuthClientLocomotion(def.Locomotion, brain, spellDoer, hitCollider);

                    InitTargetCone(brain);

                    _characterBrain = brain;

                    if (!brain.IsBot)
                    {
                        _cameraRig.SetGuideProvider(brain);
                        _cameraRig.Follow = transform;
                        _cameraRig.LookAt = transform;

                        InitSelectionController(spellDoer);

                        SurvivalGuiNode.Instance?.OnOurPawnChanged(null, this);
                    }

                    if (WorldConstants.EnableOutline)
                    {
                        var render = _view.Value.GameObject.GetComponentsInChildren<Renderer>().FirstOrDefault();
                        if (render)
                        {
                            var outline = render.gameObject.GetOrAddComponent<Outline>();
                            outline.eraseRenderer = true;
                            outline.color = OutlineColor.Erase;
                        }
                    }

                    _spellDoer = spellDoer;

                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "InitAuthClient.Done. Brain:{0}",  brain.GetType()).Write();                        
                });
            });
        }
        
        private async Task<IDisposableCharacterBrain> CreateBrain(ISpellDoer spellDoer, BotActionDef botBrainDef)
        {
            if (botBrainDef != default(BotActionDef))
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Create Bot Brain with Def: {0}",  botBrainDef.____GetDebugShortName()).Write();
                if (!gameObject.name.EndsWith("_bot"))
                    gameObject.name += "_bot";
                // very dirty hack
                // we need this because when bot is spawning his input is ignored for 5 seconds CharacterMovementSync _lastSentTeleportTimestamp
                // and then bot is stuck because he is not in MovementGrid 
                await Task.Delay(TimeSpan.FromSeconds(10));
                return await Instantiate(_botBrainPrefab, transform).Init(spellDoer, OuterRef, Repository, _targetHolder, new CameraGuideControllerSettings(), botBrainDef);
            }
            else
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Create Player Brain").Write();;
                _cameraRig = _cameraSpawner.SpawnCameraRig();
                Assert.IsNotNull(_cameraRig);
                if(!gameObject.name.EndsWith("_me"))
                    gameObject.name += "_me";
                return await Instantiate(_playerBrainPrefab, transform).Init(spellDoer, OuterRef, Repository, _targetHolder, new CameraGuideControllerSettings(_cameraRig));
            }
        }

        private void InitTargetCone(ICharacterBrain brain)
        {
            Transform parentSetTargetCone = _view.Value.GameObject.transform;
            _setTarget = Instantiate(_setTargetConePrefab, parentSetTargetCone.position, parentSetTargetCone.rotation, parentSetTargetCone);
            _setTarget.Init(_targetHolder, gameObject, brain);
        }

        private void InitSelectionController(ISpellDoer spellDoer)
        {
            if (_visualMarkerSelector == null)
            {
                Logger.IfDebug()?.Message($"No VisualMarkerSelector set in {nameof(CharacterPawn)} on {gameObject.name}, initializing").Write();
                _visualMarkerSelector = gameObject.AddComponent<VisualMarkerSelector>();
                _visualMarkerSelector.Init(spellDoer, _targetHolder);
            }
        }

        private void CleanServer()
        {
            Assert.IsTrue(_isServerInitialized);
            DisposeLocomotion();
            _positionLogger.SubscribeUnsubscribe(OuterRef, ServerRepo, SubscribeUnsubscribe.Unsubscribe);

            _isServerInitialized = false;
            _isSpawnPositionGot = false;
        }

        private void CleanClient()
        {         
            if (HasClientAuthority)
                return;
   
            if (!_isClInitialized)
                return;

            DisposeLocomotion();
            DisposeLocomotionClientOnlyPipeline();

            if (!IsHostMode)
            {
                _positionLogger.SubscribeUnsubscribe(OuterRef, ClientRepo, SubscribeUnsubscribe.Unsubscribe);
            }

            DestroyView();

            _spellDoer = null;
            
            _isClInitialized = false;
            _isSpawnPositionGot = false;
        }
        
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void CleanAuthClient(bool doCheck = true)
        {
            if (doCheck)
                Assert.IsTrue(_isAuthClInitialized);

            DirectMotionProducer.RemoveFrom(transform);
            DisposeLocomotion();
            DisposeLocomotionClientOnlyPipeline();
            
            if (_setTarget != null)
                if (_setTarget.gameObject != null)
                    Destroy(_setTarget.gameObject);

            if (_visualMarkerSelector != null)
                Destroy(_visualMarkerSelector);

            if (_characterBrain != null && !_characterBrain.IsBot && SurvivalGuiNode.Instance != null && SurvivalGuiNode.Instance.OurUserPawn == gameObject)
                SurvivalGuiNode.Instance.OnOurPawnChanged(this, null);

            _characterBrain?.Dispose();
            _characterBrain = null;

            _cameraSpawner.ReleaseCameraRig(_cameraRig);
            _cameraRig = null;
            if (_targetHolder)
                Destroy(_targetHolder);

            _attackDoer?.Dispose();
            _attackDoer = null;

            _isAuthClInitialized = false;
            _isSpawnPositionGot = false;
        }

        public void CleanAndInit(WorldCharacterDef def)
        {
            var a = _isAuthClInitialized;
            var c = _isClInitialized;
            var s = _isServerInitialized;

            if (s)
                CleanServer();
            if (c)
                CleanClient();
            if (a)
            {
                if (SurvivalGuiNode.Instance != null && SurvivalGuiNode.Instance.OurUserPawn == gameObject)
                {
                    Alive = true;
                    SurvivalGuiNode.Instance.OnOurPawnChanged(this, null);
                }
                CleanAuthClient();
            }

            if (a)
                InitAuthClientU(def); 
            if (c)
                InitClientU(def);
            if (s)
                InitServerU(def);
        }


        private void InitAuthClientLocomotion(CharacterLocomotionDef locomotionDef, ICharacterBrain brain, ISpellDoer spellDoer, Collider hitCollider)
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Init auth client locomotion").Write();;
            DisposeLocomotion();
            var settings = new CharacterLocomotionSettings(locomotionDef);
            var charController = CreateCharacterController(locomotionDef.CharacterController);
            var colliderProxy = CreateColliderProxy(charController);
            var softCollider = CreateSoftCollider(locomotionDef);
            _characterStreamerAgent = _characterStreamerAgent ?? new StreamingAgent(Ego.EntityId, brain.IsBot);
            _characterStreamerAgent?.Teleport(transform.position.ToLocomotion());
            var calcersCache = new CalcersCache(GetOuterRef<IEntity>(), ClientRepo, (t) => AsyncUtils.RunAsyncTask(t), brain.IsBot ? 1 : 0.1f);
            var locomotionClock = new LocomotionSyncedClock();
            var constants = new LocomotionConstants(locomotionDef.Constants);
            var stats = new CharacterStatsProvider(locomotionDef, constants, calcersCache);
            var groundSensor = new RaycastGroundSensor(settings, colliderProxy);
            var environment = new LocomotionEnvironment(settings, groundSensor, colliderProxy, IsLocomotionEnabled);
            var body = new LocomotionCharacterBody(settings, charController);
            var relevance = new CharacterClientRelevanceLevel(this, settings.Client);
            var reactions = new CharacterLocomotionReactions(locomotionDef.Bindings, spellDoer, EntityId, brain.InputActionStatesGenerator);
            var history = new LocomotionHistory(body, environment); 
            var directMotionProducer = DirectMotionProducer.Create(transform, _view.Value.Animator, body, brain, OuterRef.Guid);
            var locomotionAgent = new LocomotionEngineAgent();
            LocomotionAgentExtensions.SetLocomotionToEntity(true, locomotionAgent, directMotionProducer, brain, OuterRef, ClientRepo);
            var stateMachineContext = CharacterStateMachineContext.Create(
                input: brain,
                body: body,
                stats: stats,
                environment: environment,
                history: history,
                clock: locomotionClock,
                constants: constants,
                calcersCache: calcersCache);
            var stateMachine = CharacterStateMachine.Create(stateMachineContext, locomotionDef.Bindings, reactions, calcersCache, this);
            var networkTransport = NetworkTransport.CreateMasterSend(Ego.OuterRef, ClientRepo);
            LocomotionStickinessPredicate stickinessPredicate = (nfo,pos,vel) => CheckStickiness(nfo, pos, vel, stats, stateMachineContext.Input, colliderProxy);
            _locomotion = new LocomotionEngine(LocomotionDebugAgent)
                .Updateables(
                    locomotionClock,
                    calcersCache,
                    _positionLogger,
                    groundSensor,
                    environment,
                    history,
					directMotionProducer,
                    stateMachineContext
                )
                .Debugables(
                    stateMachineContext,
                    stateMachine,
                    colliderProxy,
                    LocomotionDebugUnity.GatherRealVelocity(transform)
                )
                .ComposePipeline(body, _0 => _0
                    .AddPass(new LocomotionTimestampNode(locomotionClock, 0), _7 => _7 // Should be _before_ LocoReclaimer node.
                        .AddPass(new LocomotionNetworkReclaimer(NetworkTransport.CreateMasterReceive(Ego.OuterRef, ClientRepo), Ego.OuterRef, ClientRepo, this), _1 => _1
                            .AddPass(stateMachine, _2 => _2
                                .AddCommitIf(WorldConstants.ActorWithActorCollisions, new LocomotionIgnoreCollidersNode(transform, new[]{charController, softCollider}, LocomotionFlags.NoCollideWithActors, IgnoreCollisionLayers.Value)) 
                                .AddPassIf(WorldConstants.ActorWithActorCollisions, new LocomotionCharacterDepenetrationNode(settings, softCollider, SoftCollisionMatcher.Value, colliderProxy, this), _3 => _3    
                                    .AddPass(new LocomotionSimpleMovementNode(), _4 => _4    
                                        .AddCommitIf(WorldConstants.ActorWithActorCollisions, new LocomotionObjectsPusher(softCollider, PhysicsLayers.DetachedDestructChunksMask))
                                        .AddPassIf(WorldConstants.ActorWithActorCollisions, new LocomotionCharacterCollisionNode(settings, softCollider, SoftCollisionMatcher.Value, colliderProxy, stickinessPredicate, this), _5 => _5
                                            .AddPass(new LocomotionSpeedLimiter(settings, GetOuterRef<IEntity>().Guid, this), _6 => _6
                                                .AddCommit(LocomotionDebug.GatherVariables)
                                                .AddPass(body, _8 => _8
                                                    .AddCommit(LocomotionDebug.GatherBody(body))
                                                    .AddCommit(new LocomotionNetworkSender(networkTransport, locomotionClock, relevance, settings.Client, this, EntityId))
                                                    .AddCommit(_characterStreamerAgent)
                                                    .AddCommit(locomotionAgent)
                                                    .AddCommit(new LocomotionColliderResizer(settings, charController))
                                                    .AddCommit(new LocomotionColliderResizer(settings, softCollider, true))
                                                    .AddCommit(new LocomotionColliderResizer(settings, hitCollider, true))
                                                    .AddCommit(LocomotionClientOnlyPipeline)
                                            )))))))));



            _locomotionReactions = reactions;
            FillLocomotionClientOnlyPipeline(settings);
        }

        private void InitServerLocomotion(CharacterLocomotionDef locomotionDef, Collider hitCollider)
        {
            if (!HasClientAuthority)
            {
                if (IsHostMode)
                {
                    InitClientLocomotion(locomotionDef, hitCollider);
                }
                else
                {
                    if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("Init server locomotion").Write();;
                    DisposeLocomotion();
                    var settings = new CharacterLocomotionSettings(locomotionDef);
                    var locomotionClock = new LocomotionSyncedClock();
                    var virtualBody = new LocomotionVirtualBodyWithFlags();
                    var realBody = new LocomotionTransformBody(transform);
                    var networkTransport = NetworkTransport.CreateSlaveReceive(Ego.OuterRef, ServerRepo);
                    var networkReceiver = new LocomotionNetworkReceiverPass(networkTransport, settings.Server);
                    _locomotion = new LocomotionEngine(LocomotionDebugAgent)
                        .Updateables(
                            locomotionClock,
                            _positionLogger
                        )
                        .Debugables(
                            LocomotionDebug.GatherBody(realBody),
                            LocomotionDebugUnity.GatherRealVelocity(transform)
                        )
                        .ComposePipeline(virtualBody, _1 => _1
                            .AddPass(networkReceiver, _2 => _2
                                .AddPass(new LocomotionSimpleMovementNode(), _4 => _4    
                                    .AddCommit(LocomotionDebug.GatherPredicted)
                                    .AddCommit(virtualBody)
                                    .AddCommit(LocomotionDebug.GatherVariables)
                                    .AddCommit(realBody)
                                    .AddCommit(LocomotionDebug.GatherBody(realBody))
                                    .AddCommit(new LocomotionColliderResizer(settings, hitCollider, true))
                                    .AddCommit(LocomotionClientOnlyPipeline)
                        )));
                }
            }
        }
        
        private void InitClientLocomotion(CharacterLocomotionDef locomotionDef, Collider hitCollider)
        {
            var settings = new CharacterLocomotionSettings(locomotionDef);
            if (!HasClientAuthority)
            {
                string mode;
                IEntitiesRepository repo;
                LocomotionNetworkReceiverPass.ISettings receiverSettings; 
                LocomotionSmoothingNode.ISettings smootherSettings; 
                LocomotionDestuckerNode.ISettings destuckerSettings; 
                if (IsHostMode)
                {
                    mode = "host";
                    repo = ServerRepo;
                    smootherSettings = settings.Server;
                    destuckerSettings = settings.Server;
                    receiverSettings = settings.Server;
                }
                else
                {
                    mode = "client";
                    repo = ClientRepo;
                    smootherSettings = settings.Client;
                    destuckerSettings = settings.Client;
                    receiverSettings = settings.Client;
                }
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Init {mode} locomotion").Write();
                DisposeLocomotion();
                var locomotionClock = new LocomotionSyncedClock();
                var charController = CreateCharacterController(locomotionDef.CharacterController);
                var softCollider = CreateSoftCollider(locomotionDef);
                var virtualBody = new LocomotionVirtualBodyWithFlags();
                var realBody = new LocomotionCharacterBody(settings, charController);
                var networkTransport = NetworkTransport.CreateSlaveReceive(Ego.OuterRef, repo);
                _locomotion = new LocomotionEngine(LocomotionDebugAgent)
                    .Updateables(
                        locomotionClock,
                        _positionLogger
                    )
                    .Debugables(
                        LocomotionDebugUnity.GatherRealVelocity(transform)
                    )
                    .ComposePipeline(virtualBody, _1 => _1
                        .AddPass(new LocomotionNetworkReceiverPass(networkTransport, receiverSettings), _2 => _2
                            .AddCommit(new LocomotionIgnoreCollidersNode(transform, new[]{charController}, LocomotionFlags.NoCollideWithActors, IgnoreCollisionLayers.Value)) // чтобы, когда у удалённого игрока коллизии выключены, локальный игрок не коллизился с ним со своей стороны. иначе локального игрока будет выталкивать из удалённого.
                            .AddPass(new LocomotionSimpleMovementNode(), _4 => _4    
                                .AddCommit(LocomotionDebug.GatherPredicted)
                                .AddCommit(virtualBody)
                                .AddPass(new LocomotionDestuckerNode(destuckerSettings, realBody), _6 => _6
                                    .AddPass(new LocomotionSmoothingNode(smootherSettings, realBody), _5 => _5
                                        .AddCommit(LocomotionDebug.GatherVariables)
                                        .AddPass(realBody, _3 => _3
                                            .AddCommit(new LocomotionColliderResizer(settings, charController))
                                            .AddCommit(new LocomotionColliderResizer(settings, softCollider, true))
                                            .AddCommit(new LocomotionColliderResizer(settings, hitCollider, true))
                                            .AddCommit(LocomotionClientOnlyPipeline)
                                            .AddCommit(LocomotionDebug.GatherBody(realBody))
                    ))))));
            }
            if (_view.Value != null)
                FillLocomotionClientOnlyPipeline(settings);
        }
        
        private CharacterController CreateCharacterController(CharacterControllerDef def)
        {
            var c = JsonToGO.Instance.AddOrGetComponent<CharacterController>(gameObject, def)
                   ?? throw new Exception($"Can't create character controller on {name}");
            _locomotionComponents.Add(c);
            return c;
        }

        private LocomotionCharacterCollider CreateColliderProxy(CharacterController controller)
        {
            var cc = gameObject.AddComponent<LocomotionCharacterCollider>()
                     ?? throw new Exception($"Can't create character controller collider adapter on {name}");
            cc.Init(controller, GroundLayers.Value, SoftCollisionMatcher.Value.Layers);
            _locomotionComponents.Add(cc);
            return cc;
        }
        
        private (Rigidbody, Collider) CreateRigidbody(CharacterLocomotionDef locomotionDef)
        {
            var r = JsonToGO.Instance.AddOrGetComponent<Rigidbody>(gameObject, locomotionDef.Rigidbody.Target)
                    ?? throw new Exception($"Can't create rigid body on {name}");
            var c = JsonToGO.Instance.AddOrGetComponent<Collider>(gameObject, locomotionDef.Collider.Target)
                    ?? throw new Exception($"Can't create collider on {name}");
            _locomotionComponents.Add(r);
            _locomotionComponents.Add(c);
            return (r,c);
        }

        private Collider CreateCollider(CharacterLocomotionDef locomotionDef)
        {
            var c = JsonToGO.Instance.AddOrGetComponent<Collider>(gameObject, locomotionDef.Collider.Target) 
                    ?? throw new Exception($"Can't create collider on {name}");
            _locomotionComponents.Add(c);
            return c;
        }

        private Collider CreateSoftCollider(CharacterLocomotionDef locomotionDef)
        {
            var c = JsonToGO.Instance.AddOrGetComponent<Collider>(_softColliderPlace, locomotionDef.SoftCollider.Target) 
                    ?? throw new Exception($"Can't create soft collider on {name}");
            _locomotionComponents.Add(c);
            return c;
        }
        
        private Collider CreateHitCollider(IHasBoundsDef def)
        {
            var c = _hitColliderPlace.GetOrAddComponent<CapsuleCollider>();
            c.direction = 1;
            c.radius = def.Bounds.Radius;
            c.height = def.Bounds.Height;
            c.center = new Vector3(0, def.Bounds.OffsetY, 0);
            c.isTrigger = true;
            return c;
        }
        
        private void DisposeLocomotion()
        {
            _locomotion?.Dispose();
            _locomotion = null;
            _locomotionReactions?.Dispose();
            _locomotionReactions = null;
            _locomotionDebugTrailInstance?.Dispose();
            foreach (var component in _locomotionComponents)
                if (component) DestroyImmediate(component);
            _locomotionComponents.Clear();
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("UI.[ChClVi].DisposeLocomotion Loco is set to null. >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Loco").Write();;
        }

     
        private void FillLocomotionClientOnlyPipeline(CharacterLocomotionSettings settings)
        {
            if (!_locomotionClientOnlyInitialized)
            {
                _locomotionClientOnlyInitialized = true;
                _locomotionDebugTrailInstance?.Initialize(() => GameCamera.Camera);
                if (WorldConstants.UseMockCharacterView < MockView.NoAnimations)
                {
                    LocomotionClientOnlyPipeline
                        .AddCommit(new CharacterCommitToAnimator(_view.Value.Animator, settings))
                        .AddCommit(new CharacterBodyTurning(_view.Value.TwistMotor, _view.Value.TurningMotor));
                }
            }        
        }

        private void DisposeLocomotionClientOnlyPipeline()
        {
            _locomotionClientOnlyInitialized = false;
            _locomotionClientOnlyPipeline?.Clear();
            _locomotionDebugTrailInstance?.Dispose();
        }

        private bool IsLocomotionEnabled() => _view.Value != null;

        private ICharacterView InstantiateAndInitViewIfShould(bool isResurrectionAfterDeath = false)
        {
            if (_view.Value != null)
            {
                if (_mutation.Actual == _mutation.View && _gender.Actual == _gender.View)
                {
                    if (Logger.IsDebugEnabled)
                        Logger.IfDebug()?.Message($"UI.[ChClVi].InstantiateAndInitViewIfShould `{nameof(_view.Value)}` != null. So do nothing.").Write();
                    return _view.Value;
                }
                
                DestroyView();
            }

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].InstantiateView <{Ego.EntityId}>").Write();

            Assert.IsNull(_view.Value, nameof(_view));
            
            var dollDef = CharacterDollSelector.SelectDoll(CharacterDef, _mutation.Actual, _gender.Actual);
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"DollDef:{dollDef} Prefab:{dollDef?.Prefab}").Write();

            ICharacterView view;
            if (WorldConstants.UseMockCharacterView < MockView.NoView)
                view = Instantiate(dollDef.Prefab.Target, transform).GetComponent<ICharacterView>();
            else
                view = Instantiate(_mockViewPrefab, transform);
            Assert.IsNotNull(view);
            view.Attach(OuterRef, Repository, dollDef);
            view.SetMutationStage(_mutation.Actual);
            Assert.IsNull(_view.Value);
            _view.Value = view;
            
            _mutation.View = _mutation.Actual;
            _gender.View = _gender.Actual;
            
            GetComponent<VisualEventProxy>()?.SetRoot(gameObject);

            return view;
        }

        private void DestroyView()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"UI.[ChClVi].DestroyView <{EntityId}> {_view.Value}").Write();
            
            var view = _view.Value;
            _view.Value = null;
            
            if (view != null)
            {
                view.Detach(OuterRef, Repository);
                var viewGo = view.GameObject;
                if (viewGo != null)
                    DestroyImmediate(viewGo);
            }
        }

        private IEnumerator WaitThenOnDieDoAuthClient()
        {
            Alive = false;
             Logger.IfDebug()?.Message("UI.[ChClVi].WaitThenOnDieDo(-1.1).").Write();;
            yield return new WaitForSeconds(_onDieDelay);

             Logger.IfDebug()?.Message("UI.[ChClVi].OnDestroy(0)..").Write();;

            CleanAuthClient();
        }

        private static bool CheckStickiness(CollisionInfo nfo, LocomotionVector selfPosition, LocomotionVector selfVelocity, ICharacterStatsProvider stats, ILocomotionInputProvider input, ILocomotionCollider collider)
        {
            if (input[CharacterInputs.Sticking])
            {
                if (nfo.Point.Vertical - selfPosition.Vertical > collider.Radius &&
                    SharedCode.Utils.Vector2.Dot(nfo.Point.Horizontal - selfPosition.Horizontal.normalized, selfVelocity.Horizontal.Normalized) > Mathf.Cos(stats.StickHitAngle))
                    return true;
            }            
            return false;
        }
        
        private LocomotionPipeline LocomotionClientOnlyPipeline => 
            _locomotionClientOnlyPipeline ?? (_locomotionClientOnlyPipeline = new LocomotionPipeline());

        private LocomotionDebug.Context LocomotionDebugAgent
        {
            get
            {
                if (_locomotionDebugContext == null && (ServerProvider.IsClient/*IsClient*/ /*IsHostMode*/) && ClientCheatsState.DebugInfo)
                {
                    var debugProxy = new DebugProxy();
                    _locomotionDebugTrailInstance = Instantiate(_locomotionDebugTrail, transform);
                    _locomotionDebugContext =
                        new LocomotionDebug.Context(EntityId,
                            new LocomotionCompositeDebugAgent(
                                debugProxy,
                                _locomotionDebugTrailInstance,
                                _positionLogger,
                                new LocomotionLoggerAgent()
                            ));
                    _locomotionDebugProvider = debugProxy;
                }
                return _locomotionDebugContext;
            }
        }

        EntityGameObject IEntityPawn.Ego => Ego;

        private class StreamingAgent : ILocomotionPipelineCommitNode, IDisposable
        {
            private readonly Guid _characterId;
            private readonly bool _isBot;
            private readonly DisposableComposite _disposables = new DisposableComposite();
            private bool _inGame;

            public StreamingAgent(Guid characterId, bool isBot)
            {
                _characterId = characterId;
                _isBot = isBot;
                GameState.Instance.IsInGameRp.Action(_disposables, x => _inGame = x);
            }

            bool ILocomotionPipelineCommitNode.IsReady => _inGame && SceneStreamerSystem.Streamer.GetStatus(_characterId) >= SceneStreamerStatus.ImportantReady;

            //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
            // So DO NOT change `inVars`!
            void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
            {
                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: a)StreamingAgent");

                if ((inVars.Flags & LocomotionFlags.Teleport) != 0)
                    Teleport(inVars.Position);
                else
                    Move(inVars.Position);

                LocomotionProfiler.EndSample();
            }

            void Move(LocomotionVector pos)
            {
                var position = (Vector3)LocomotionHelpers.LocomotionToWorldVector(pos);
                SceneStreamerSystem.Streamer.SetPosition(_characterId, position, false);
            }

            public void Teleport(LocomotionVector pos)
            {
                var position = (Vector3)LocomotionHelpers.LocomotionToWorldVector(pos);
                SceneStreamerSystem.Streamer.SetPosition(_characterId, position, true);
            }

            public void Dispose()
            {
                if (_isBot)
                    SceneStreamerSystem.Streamer.Remove(_characterId);
                _disposables.Dispose();
            }
        }

        class DummyGuideProvider : IGuideProvider
        {
            private readonly Transform _transform;
            public DummyGuideProvider(Transform transform) => _transform = transform;
            public SharedCode.Utils.Vector3 Guide => _transform.forward.ToShared();
        }
    }
}