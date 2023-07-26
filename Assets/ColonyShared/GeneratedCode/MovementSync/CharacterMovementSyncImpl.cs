using System;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.GeneratedCode.MovementSync;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratedCode.MapSystem;
using SharedCode.MovementSync;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Entities.Service;
using SharedCode.Entities.GameObjectEntities;
using TimeUnits = System.Int64;

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterMovementSync : IHookOnDestroy, IHookOnReplicationLevelChanged
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(CharacterMovementSync));

        private VisibilityGrid _grid;
        private CharacterMovementStateFrame _state;
        private CharacterMovementState _lastOnGroundState;
        private bool _hasLastOnGroundState;
        private WorldBoundsWatchdog _watchdog;
        private readonly CharacterMovementStateEvent _movementStateChanged = new CharacterMovementStateEvent();
        private readonly CharacterMovementStateEvent _movementStateReclaimed = new CharacterMovementStateEvent();
        private readonly object _lock = new object();
        private int _orderErrors;
        private readonly PositionHistory _history = new PositionHistory(Constants.WorldConstants.ServerMovementHistoryCapacity, Vector3.one);

    #region for Teleport
        // Как это работает: Сервер, применяя телепорт (тут в SetTransform), сетит  `_lastSentTeleportTimestamp`. Пока он не сброшен в -1 (**), все пакеты от Cl игнорятся.
        //.. Когда Cl получает телепорт (LocoReclaimer нода), он отвечает серверу by rpc `TeleportDone` (в кот.вкладывает:
        //..   - (1) серверный timestamp (==_lastSentTeleportTimestamp), чтобы сервер смог сопоставить со своим тек. знач-ем _lastSentTeleportTimestamp 
        //..         (проверка, на последний ли телепорт это прилетел ответ);
        //..   - (2) и свой timestamp - кот-ый на сервере запис-ся в `_ignoreClFramesEarlierThan`, чтобы отбросить все пакеты с клиента, рождённые раньше,
        //..         которые могут ещё долетать после этого rpc из-за: "<i>'cos rpc & frames are passed from Cl to S. by different channels & (I suppose) could be reordered.</i>"
        private TimeUnits _lastSentTeleportTimestamp = -1;
        // Cl passes its timestamp in response to teleport command from server. All Cl frames with greater timestamp are formed after applying that teleport.
        // I use this field instead of just switch flag in Cl-rpc, 'cos rpc & frames are passed from Cl to S. by different channels & (I suppose) could be reordered.
        private TimeUnits _ignoreClFramesEarlierThan = -1;
        private readonly TimeSpan _ignoreClInputWhileTeleportingTimeout = TimeSpan.FromSeconds(5f);
    #endregion for Teleport

        protected override void constructor()
        {
            base.constructor();
            _state.Counter = -1;
        }

        public Transform Transform
        {
            get { lock (_lock) return new Transform(_state.State.Position, _state.State.Rotation); }
        }

        public Transform SetTransform
        {
            set
            {
                long counter;
                lock (_lock)
                    counter = _state.Counter;
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("CharacterMovementSync.SetTransform: {0} {1}", value, parentEntity.Id).Write();
                //if (DbgLog.Enabled) DbgLog.Log($"ChMvmntSync.SetTransform pos:{value.Position}, rot:{value.Rotation.eulerAngles}.");
                var frame = new CharacterMovementStateFrame
                {
                    State = new CharacterMovementState { Position = value.Position, Rotation = value.Rotation, Flags = LocomotionFlags.Teleport },
                    Counter = counter + 1
                };

                Interlocked.Exchange(ref _lastSentTeleportTimestamp, frame.Counter);
                //DbgLog.Log($"SetTransform. _lastSentTeleportTimestamp (now=={_lastSentTeleportTimestamp}) is set to {frame.Counter} (==)");
                StartIgnoreClInputWhileTeleportingTimeout(frame.Counter);

                if (UpdateState(frame))
                {
                    UpdateGrid(frame.State);
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        using (await parentEntity.GetThisExclusive())
                        {
                            await On__SyncMovementStateReclaim(frame);
                            __SyncMovementStateReliable = frame;
                        }
                    }, EntitiesRepository);
                }
            }
        }

        // To prevent infinite ignoring Cl inputs if Cl response was lost(not sent) for some reason
        private void StartIgnoreClInputWhileTeleportingTimeout(TimeUnits teleportTimeStamp)
        {
            var repo = EntitiesRepository;
            AsyncUtils.RunAsyncTask(async () =>
            {
                await Task.Delay(_ignoreClInputWhileTeleportingTimeout);
                //#no_need: lock is no need here
                // using (var justLockSelf = await repo.Get(parentEntity.TypeId, parentEntity.Id))
                // {
                    //DbgLog.Log($"StartIgnoreClInputWhileTeleportingTimeout. Delay expired ({_lastSentTeleportTimestamp})");
                    Interlocked.CompareExchange(ref _lastSentTeleportTimestamp, -1, teleportTimeStamp);
                // }
            }, repo);
        }

        public IPositionHistory History => _history;

        public ICharacterMovementStateEvent OnMovementStateChanged => _movementStateChanged;

        public ICharacterMovementStateEvent OnMovementStateReclaimed => _movementStateReclaimed;

        public CharacterMovementStateFrame MovementState
        {
            get { lock (_lock) return _state; }
        }

        // Is called on Server
        public async Task UpdateMovementImpl(CharacterMovementState state, bool important, long counter)
        {
            if (Interlocked.Read(ref _lastSentTeleportTimestamp) > -1) //w/o interlocked should be atomic but for safe
            {
                //DbgLog.Log("% %% %%% %%%% %%% %% % %% %%% %%%% %%% %% %  UpdateMovementImpl(). REJECT, 'cos: _lastSentTeleportTimestamp > 0 (==" + _lastSentTeleportTimestamp);
                return;
            }
            else if (Interlocked.Read(ref _ignoreClFramesEarlierThan) > counter)
            {
                //DbgLog.Log($"# %% %#% %##% %#% %% #  UpdateMovementImpl(). REJECT, 'cos: _ignoreClFramesEarlierThan > counter : _iCFET:{_ignoreClFramesEarlierThan}, cntr:{counter}. d:{_ignoreClFramesEarlierThan - counter}");
                return;
            }
            

            long counterCur;
            lock (_lock)
                counterCur = _state.Counter; 
            if (counterCur == -1) // you shouldn't be able to move if there was no SetTransform at creation
                return;
            var frame = new CharacterMovementStateFrame { State = state, Counter = counter };
            if (UpdateState(frame))
            {
                UpdateGrid(state);
                if (important)
                    __SyncMovementStateReliable = frame;
                else
                    await On__SyncMovementStateUnreliable(frame);
                _movementStateChanged.Invoke(state, counter);

                var watchdog = _watchdog;
                if (watchdog != null)
                    await watchdog.Update(state.Position);
            }
        }

        public Task NotifyThatClientIsGoneImpl()
        {
            CharacterMovementState state;
            long counter;
            bool onGround;
            lock (_lock)
            {
                counter = _state.Counter;
                onGround = _hasLastOnGroundState;
                if (_hasLastOnGroundState)
                    state = _lastOnGroundState;
                else
                    state = _state.State;
            }
            var newState = new CharacterMovementState(
                position: state.Position,
                orientation: state.Orientation,
                velocity: Vector3.zero,
                flags: 0);
            counter++;
            Logger.IfDebug()
                ?.Message(parentEntity.Id, $"Client Is Gone | Position:{state.Position} Counter:{counter} OnGround:{onGround}")
                .Write();
            return UpdateMovementImpl(newState, true, counter);
        }

        public Task TeleportDoneImpl(long teleportFrameTimestamp, long clientNowTimestamp)
        {
            //DbgLog.Log($"TeleportDoneImpl ({teleportFrameTimestamp},{clientNowTimestamp})");
            var currSent = Interlocked.Read(ref _lastSentTeleportTimestamp);
            if (currSent < 0 || currSent > teleportFrameTimestamp) //is off || more recent teleport has been done
            {
                //DbgLog.Log($"TeleportDoneImpl REJECT, 'cos: (currSent < 0 || currSent > teleportFrameTimestamp) (cS:{currSent},tFT:{teleportFrameTimestamp})");
                return Task.CompletedTask;
            }

            //! Order matters, 'cos at opposite order some packets need to be rejected could be not in beatween of these to pieces of code 
            //#order 1of2
            TimeUnits ignoreCurr, ignorePrev;
            do
            {
                ignoreCurr = Interlocked.Read(ref _ignoreClFramesEarlierThan);
                if (ignoreCurr < clientNowTimestamp)
                    ignorePrev = Interlocked.CompareExchange(ref _ignoreClFramesEarlierThan, clientNowTimestamp, ignoreCurr);
                else
                    break;
            } while (ignorePrev < ignoreCurr); // && ignorePrev < clientNowTimestamp); - no need, 'cos "ignoreCurr < clientNowTimestamp" here
            //#order 2of2
            TimeUnits lastSentPrev = Interlocked.CompareExchange(ref _ignoreClFramesEarlierThan, -1, teleportFrameTimestamp);
            if (lastSentPrev < teleportFrameTimestamp)
                Logger.IfError()?.Message($"Unexpected lastSentPrev({lastSentPrev}) < teleportFrameTimestamp({teleportFrameTimestamp}).").Write();

            return Task.CompletedTask;
        }


        private Task OnSyncMovementStateUnreliable(CharacterMovementStateFrame frame)
        {
            if (UpdateState(frame))
                _movementStateChanged.Invoke(frame.State, frame.Counter);
            return Task.CompletedTask;
        }

        private Task OnSyncMovementStateReliable(EntityEventArgs args)
        {
            var frame = (CharacterMovementStateFrame)args.NewValue;
            if (UpdateState(frame))
                _movementStateChanged.Invoke(frame.State, frame.Counter);
            return Task.CompletedTask;
        }

        private Task OnCharMoveReclaim(CharacterMovementStateFrame frame)
        {
            _movementStateReclaimed.Invoke(frame.State, frame.Counter);
            return Task.CompletedTask;
        }

        // Any change of position by Locomotion is executed via this call (on every side: Cl || S)
        private bool UpdateState(CharacterMovementStateFrame frame)
        {
            _history.Push(frame.State.Position, frame.State.Orientation, frame.Counter);
            lock (_lock)
            {
                if (frame.Counter < _state.Counter)
                {
                    if (_state.Counter - frame.Counter > 1000)
                        Logger.IfWarn()?.Message($"Too big order error: {_state.Counter} - {frame.Counter} = {_state.Counter - frame.Counter}").Write();
                    if (++_orderErrors > 10)
                        Logger.IfWarn()?.Message($"Too many order errors in a row: {_orderErrors}").Write();
                    return false;
                }
                _orderErrors = 0;
                _state = frame;
                if (!frame.State.Flags.Any(LocomotionFlags.Airborne) || frame.State.Flags.Any(LocomotionFlags.CheatMode))
                {
                    _lastOnGroundState = frame.State;
                    _hasLastOnGroundState = true;
                }

                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(parentEntity.Id, "Update State | Counter:{0} State:{1}", _state.Counter, _state.State)
                    .Write();

                if (_updateGridOnServer)
                {
                    //if (DbgLog.Enabled) DbgLog.Log($"X. ChMvnmntSyncImpl.UpdateState: pos:{frame.State.Position}, yaw:{frame.State.Orientation * SharedHelpers.Rad2Deg} (cntr:{frame.Counter})");
                    UpdateGrid(frame.State);
                }

                return true;
            }
        }

        private void UpdateGrid(CharacterMovementState state)
        {
            _grid = VisibilityGrid.Get(((IHasWorldSpaced)parentEntity).WorldSpaced.OwnWorldSpace.Guid, EntitiesRepository);
            state.Def = ((IEntityObject)parentEntity).Def;
            _grid?.SetGridData(state.Position, new OuterRef<IEntity>(parentEntity), state);
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

        bool _updateGridOnServer = false;
        public async Task OnReplicationLevelChangedAsync(long oldReplicationMask, long newReplicationMask)
        {
             if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Server)
                 || (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast) 
                     && !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Server)))
            { // Replica
                // Subscribe:
                using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                {
                    __SyncMovementStateUnreliable -= OnSyncMovementStateUnreliable;
                    __SyncMovementStateUnreliable += OnSyncMovementStateUnreliable;
                    SubscribePropertyChanged(nameof(ICharacterMovementSync.__SyncMovementStateReliable), OnSyncMovementStateReliable);
                    if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientFull))
                    {
                        // Это authority-client
                        __SyncMovementStateReclaim -= OnCharMoveReclaim;
                        __SyncMovementStateReclaim += OnCharMoveReclaim; 
                    }

                    _grid?.ForgetGridData<CharacterMovementState>(new OuterRef<IEntity>(parentEntity));
                    var frame = __SyncMovementStateReliable;
                    UpdateState(frame);
                    _movementStateChanged.Invoke(frame.State, frame.Counter);
                    _updateGridOnServer = BotCoordinator.IsActive;
                    _watchdog = null;
                }
            }
            else if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Always) 
                 && !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master) 
                 && !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast))
            { // Replica server
                // Subscribe & _updateGridOnServer (local grid) for connected servers:
                using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                {
                    __SyncMovementStateUnreliable -= OnSyncMovementStateUnreliable;
                    __SyncMovementStateUnreliable += OnSyncMovementStateUnreliable;
                    SubscribePropertyChanged(nameof(ICharacterMovementSync.__SyncMovementStateReliable), OnSyncMovementStateReliable);
                    

                    _grid?.ForgetGridData<CharacterMovementState>(new OuterRef<IEntity>(parentEntity));
                    var frame = __SyncMovementStateReliable;
                    UpdateState(frame);
                    _movementStateChanged.Invoke(frame.State, frame.Counter);
                    _updateGridOnServer = true;
                    _watchdog = null;
                }
            }
            else if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master))
            { // Master
                // Unsubscribe & update grid (here once & at `UpdateMovement()`):
                using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
                {
                    __SyncMovementStateUnreliable -= OnSyncMovementStateUnreliable;
                    UnsubscribePropertyChanged(nameof(ICharacterMovementSync.__SyncMovementStateReliable), OnSyncMovementStateReliable);
                    __SyncMovementStateReclaim -= OnCharMoveReclaim;
                    var frame = MovementState;
                    UpdateGrid(frame.State);
                    _history.Push(frame.State.Position, frame.State.Orientation, frame.Counter);
                    _movementStateChanged.Invoke(frame.State, frame.Counter);
                    _updateGridOnServer = false;
                    var scenicEntity = (IScenicEntity) parentEntity;
                    _watchdog = await new WorldBoundsWatchdog().Initialize(new OuterRef<IHasMortalServer>(parentEntity.Id, parentEntity.TypeId), EntitiesRepository, scenicEntity.MapOwner);
                }
            }
            if(newReplicationMask == 0)
                _grid?.ForgetGridData<CharacterMovementState>(new OuterRef<IEntity>(parentEntity));
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
    }

    public class CharacterMovementStateEvent : ICharacterMovementStateEvent
    {
        public event Action<CharacterMovementState, long> Action;
        public void Invoke(CharacterMovementState state, long counter) => Action?.Invoke(state, counter);
    }

}