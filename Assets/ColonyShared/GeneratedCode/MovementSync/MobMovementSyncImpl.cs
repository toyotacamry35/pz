using System;
using System.Threading.Tasks;
using ColonyShared.GeneratedCode.MovementSync;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratedCode.MapSystem;
using NLog;
using GeneratedDefsForSpells;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Utils;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Wizardry;
using static SharedCode.MovementSync.MobMovementStatePacked;
using static SharedCode.Utils.ReplicationMaskUtils;

namespace GeneratedCode.DeltaObjects
{
    public partial class MobMovementSync : IHookOnInit, IHookOnStart, IHookOnReplicationLevelChanged, IHookOnDestroy, IHookOnUnload
    {
        private static readonly Task<bool> TrueTask = Task.FromResult(true);
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(MobMovementSync));

        private IHasWorldSpaced _worldSpaced => (IHasWorldSpaced)parentEntity;
        private VisibilityGrid _grid;
        private MobMovementState _state;
        private WorldBoundsWatchdog _watchdog;
        private const bool WorldBoundsWatchDogEnabled = false;
        private long _counter;
        private readonly MobMovementStateEvent _movementStateChanged = new MobMovementStateEvent();
        private readonly object _lock = new object();
        private readonly object _pathFindingOwnerLock = new object();
        private int _orderErrors;
        private readonly PositionHistory _history = new PositionHistory(Constants.WorldConstants.ServerMovementMobHistoryCapacity, Vector3.one);
        private Guid _pathFindingOwnerRepositoryIdRuntime = Guid.Empty;

        public Transform Transform
        {
            get { lock (_lock) {
                    if (_counter == 0) return new Transform(MobMovementStatePacked.Unpack(__SyncMovementReliable).Position);
                    return new Transform(_state.Position, _state.Rotation); } }
        }

        public Transform SetTransform
        {
            set
            {
                // Logger.IfDebug()?.Message($"MobMovementSync.SetTransform: {ParentEntityId}  {0}", value).Write();
                var state = new MobMovementState(value.Position, Vector3.zero, value.Rotation, 0, LocomotionFlags.Teleport);
                var counter = _counter + 1;
                if (UpdateState(state, counter))
                {
                    UpdateGrid(state);
                    var pack = Pack(state, counter);
                    __SyncMovementReliable = pack;
                }
            }
        }

        public IPositionHistory History => _history;

        public IMobMovementStateEvent OnMovementStateChanged => _movementStateChanged;

        public Guid PathFindingOwnerRepositoryIdRuntime
        {
            get
            {
                lock (_pathFindingOwnerLock)
                {
                    return _pathFindingOwnerRepositoryIdRuntime;
                }
            }
            private set
            {
                lock (_pathFindingOwnerLock)
                {
                    _pathFindingOwnerRepositoryIdRuntime = value;
                }
            }
        }

        public Task<bool> SetPathFindingOwnerRepositoryIdImpl(Guid repositoryId)
        {
            PathFindingOwnerRepositoryIdRuntime = repositoryId;
            PathFindingOwnerRepositoryId = repositoryId;
            return TrueTask;
        }

        public MobMovementState MovementState
        {
            get { lock (_lock) return _state; }
        }

        private CurveLogger _curveLogger;
        public async Task UpdateMovementImpl(MobMovementStatePacked packedState, long counter, bool important)
        {
            var state = Unpack(packedState);
            //if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_ent.Id, "MobMovementSync.SetTransform: #{0} {1}", counter, state.Position).Write();
            if (UpdateState(state, counter))
            {
                UpdateGrid(state);
                if (important)
                {
                    if ((parentEntity as IHasLogableEntity)?.LogableEntity?.IsCurveLoggerEnable ?? false)
                        LogableHelper.GetLoggerIfEnableAndActive(ref _curveLogger, parentEntity.Id.ToString())?.AddData("UpdateMovement (set reliable prop).Pos", SyncTime.Now, packedState.Position);

                    __SyncMovementReliable = packedState;
                }
                else
                {
                    if ((parentEntity as IHasLogableEntity)?.LogableEntity?.IsCurveLoggerEnable ?? false)
                        LogableHelper.GetLoggerIfEnableAndActive(ref _curveLogger, parentEntity.Id.ToString())?.AddData("UpdateMovement (invoke unreliable event).Pos", SyncTime.Now, packedState.Position);

                    await On__SyncMovementUnreliable(packedState);
                }

                _movementStateChanged.Invoke(state, counter);

                var watchdog = _watchdog;
                if (watchdog != null)
                    await watchdog.Update(state.Position);
            }
        }
        
        private Task OnSyncMovementUnreliable(MobMovementStatePacked pack)
        {
            var state = Unpack(pack);
            if (UpdateState(state, pack.Counter))
            {
                UpdateGrid(state);
                _movementStateChanged.Invoke(state, pack.Counter);
            }
            return Task.CompletedTask;
        }

        private Task OnSyncMovementStateReliable(EntityEventArgs args)
        {
            var pack = (MobMovementStatePacked) args.NewValue;
            var state = Unpack(pack);
            if (UpdateState(state, pack.Counter))
            {
                UpdateGrid(state);
                _movementStateChanged.Invoke(state, pack.Counter);
            }
            return Task.CompletedTask;
        }

        private bool UpdateState(MobMovementState state, long counter)
        {
            _history.Push(state.Position, 0/*frame.State.Rot.eulerAngles.y*/, counter);
            lock (_lock)
            {
                if (counter < _counter)
                {
                    if(_counter - counter > 1000)
                        Logger.IfWarn()?.Message(parentEntity.Id, $"Too big order error: {_counter} - {counter} = {_counter - counter}").Write();
                    if(++_orderErrors > 10)
                        Logger.IfWarn()?.Message(parentEntity.Id, $"Too many order errors in a row: {_orderErrors}").Write();
                    return false;
                }
                _orderErrors = 0;
                _counter = counter;
                _state = state;
                return true;
            }
        }

        private void UpdateGrid(MobMovementState state)
        {
            _grid = VisibilityGrid.Get(((IHasWorldSpaced)parentEntity).WorldSpaced.OwnWorldSpace.Guid, EntitiesRepository);
            //_grid.SetGridData(state.Position, new OuterRef<IEntity>(_ent), state);
            _grid?.SetGridData(state.Position, new OuterRef<IEntity>(parentEntity), new MobMovementState() {Def = ((IEntityObject)parentEntity).Def});
                    /// Position = ((IMobMovementSync)_ent).SyncTransform.Position,
                    /// Rotation = ((IMobMovementSync)_ent).SyncTransform.Rotation }); ///Task #PZ-9627: #TODO: Разделить на 2 типа - отдельный для ~spatialHashGrid'а
        }

        public async Task OnStart()
        {
            (parentEntity as IHasWorldSpaced).WorldSpaced.SubscribePropertyChanged(nameof(IHasWorldSpaced.WorldSpaced.OwnWorldSpace), UpdateGridHandler);

            if (_worldSpaced.WorldSpaced.OwnWorldSpace.Guid != Guid.Empty)
                await UpdateGridHandler(null);
        }

        public Task OnDestroy()
        {
            _grid?.ForgetGridData<CharacterMovementState>(new OuterRef<IEntity>(parentEntity));
            return Task.CompletedTask;
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            AsyncUtils.RunAsyncTask(() => OnReplicationLevelChangedAsync(oldReplicationMask, newReplicationMask), EntitiesRepository);
        }

        public Task OnUnload()
        {
            if (parentEntity.IsMaster())
                (parentEntity as IHasWorldSpaced).WorldSpaced.UnsubscribePropertyChanged(nameof(IWorldSpaced.OwnWorldSpace), UpdateGridHandler);
            return Task.CompletedTask;
        }

        private async Task UpdateGridHandler(EntityEventArgs args)
        {
            using (await parentEntity.GetThis())
                UpdateGrid(_state);
        }

        public async Task OnReplicationLevelChangedAsync(long oldReplicationMask, long newReplicationMask)
        {
            if (IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Master) ||
                (IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast) &&
                 !IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master)) 
                  || (IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Always) &&
                 !IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master) &&
                 !IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast)))
            {
                // Replica on Client or Server
                using (await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                {
                    __SyncMovementUnreliable -= OnSyncMovementUnreliable;
                    __SyncMovementUnreliable += OnSyncMovementUnreliable;
                    SubscribePropertyChanged(nameof(__SyncMovementReliable), OnSyncMovementStateReliable);
                    var pack = __SyncMovementReliable;
                    var state = MobMovementStatePacked.Unpack(pack);
                    UpdateState(state, pack.Counter);
                    UpdateGrid(state);
                    _movementStateChanged.Invoke(state, pack.Counter);
                    _watchdog = null;
                }
            }
            else if (IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master))
            {
                // Master
                using (await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                {
                    __SyncMovementUnreliable -= OnSyncMovementUnreliable;
                    UnsubscribePropertyChanged(nameof(__SyncMovementReliable), OnSyncMovementStateReliable);
                    MobMovementState state;
                    long counter;
                    lock (_lock)
                    {
                        state = _state;
                        counter = _counter;
                    }
                    UpdateGrid(state);
                    _history.Push(state.Position, 0 /*frame.State.Rot.eulerAngles.y*/, counter);
                    _movementStateChanged.Invoke(state, counter);
                    if (WorldBoundsWatchDogEnabled)
                    {
                        var scenicEntity = (IScenicEntity) parentEntity;
                        _watchdog = await new WorldBoundsWatchdog().Initialize(
                            new OuterRef<IHasMortalServer>(parentEntity.Id, parentEntity.TypeId),
                            EntitiesRepository,
                            scenicEntity.MapOwner);
                    }
                }
            }
            if (newReplicationMask == 0)
            {
                _grid?.ForgetGridData<MobMovementState>(new OuterRef<IEntity>(parentEntity));
            }
        }

        public Task SetMovementDataImpl(MovementData data)
        {
            Logger.IfDebug()?.Message(ParentEntityId, $"SetMovementData | NewData:{data}\n CurrentData:{MovementData}").Write();
            if (data.Start || data.SpellId == MovementData.SpellId && data.MoveEffectDef == MovementData.MoveEffectDef)
            {
                MovementData = data;
                Logger.IfDebug()?.Message(ParentEntityId, $"After SetMovementData | MovementData:{data}").Write();
            }
            return Task.CompletedTask;
        }

        public async Task StopMovementImpl(SpellId spellId, MoveEffectDef moveEffectDef, bool success)
        {
            var wizId = ((IHasWizardEntity)parentEntity).Wizard.Id;
            using (var wizardC = await EntitiesRepository.Get<IWizardEntityServer>(wizId))
            {
                var wiz = wizardC.Get<IWizardEntityServer>(wizId);
                if(wiz.Spells.TryGetValue(spellId, out var spell))
                {
                    if(spell.CastData.Def.IsInfinite)
                        await wiz.StopCastSpell(spellId, success ? SpellFinishReason.SucessOnDemand : SpellFinishReason.FailOnDemand);
                }
                else
                    await wiz.StopCastSpell(spellId, success ? SpellFinishReason.SucessOnDemand : SpellFinishReason.FailOnDemand);
            }
        }

        public Task OnInit()
        {
            MovementData = new MovementData();
            return Task.CompletedTask;
        }

        private class MobMovementStateEvent : IMobMovementStateEvent
        {
            public event Action<MobMovementState, long> Action;
            public void Invoke(MobMovementState state, long counter) => Action?.Invoke(state, counter);
        }

        public Vector3 Position => Transform.Position;

        public Quaternion Rotation => Transform.Rotation;

        public Vector3 Scale => Transform.Scale;

        public Vector3 SetPosition
        {
            set { SetTransform = Transform.WithPosition(value); }
        }

        public Quaternion SetRotation
        {
            set { SetTransform = Transform.WithRotation(value); }
        }

        public Vector3 SetScale
        {
            set { SetTransform = Transform.WithScale(value); }
        }

        private static readonly NLog.Logger LegionaryLogger = LogManager.GetLogger("LegionaryEntity");

        public async Task InvokeSetDebugMobPositionLoggingEventImpl(bool enabledStatus, bool dump)
        {
            if (LegionaryLogger.IsDebugEnabled) LegionaryLogger.IfDebug()?.Message($"3. InvokeSetDebugMobPositionLoggingEventImpl call OnSetDebugMobPositionLoggingEvent({enabledStatus}, {dump})").Write();
            await OnSetDebugMobPositionLoggingEvent(enabledStatus, dump);
        }


    }
}