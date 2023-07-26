using System;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Utils;

namespace Src.Locomotion
{
    // Is stateless, except 2 fields: event `FrameReceived` & `_receiveReady`. But both of them correctly cleared, when call .Release symmetrically to .Acquire calls
    public class MobNetworkTransport : ILocomotionNetworkSend, ILocomotionNetworkReceive
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(MobNetworkTransport));
        
        private readonly AsyncTaskRunner _taskRunner;
        private readonly OuterRef<IEntity> _entityRef;
        private readonly IEntitiesRepository _repository;
        private readonly ICurveLoggerProvider _curveLogProv;

        public MobNetworkTransport(OuterRef<IEntityObject> entityRef, IEntitiesRepository repository, AsyncTaskRunner taskRunner, ICurveLoggerProvider curveLogProv)
        {
            if (!entityRef.IsValid) throw new ArgumentException(nameof(entityRef));
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (taskRunner == null) throw new ArgumentNullException(nameof(taskRunner));
            _entityRef = entityRef.To<IEntity>();
            _repository = repository;
            _taskRunner = taskRunner;
            _curveLogProv = curveLogProv;
        }
        
        ///--------------------------------------------------------------------------------------------------------------------------------
            
        bool ILocomotionNetworkSend.IsReady => true;

        void ILocomotionNetworkSend.SendFrame(LocomotionVariables frame, bool important, ReasonForSend reason)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("Loco Commit: e)LocoN/wSender: .Send .SendFrame: runAsyncTask");
            _taskRunner(async () =>
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_entityRef.Guid, "Send due {0}: #{1} [{2}]{3}", reason, frame.Timestamp, frame, important?" IMPORTANT":"")
                    .Write();
                var packedData = FrameToPackedState(frame);

                _curveLogProv?.CurveLogger?.IfActive?.AddData("SendFrame (before using).Pos", SyncTime.Now, frame.Position); 

                using (var wrapper = await _repository.Get(_entityRef))
                {
                    var movementSync = wrapper.Get<IHasMobMovementSyncAlways>(_entityRef, ReplicationLevel.Always)?.MovementSync;
                    if (movementSync == null)
                    {
                        //w/o this check wrong fires if no entity (right after destroyed). We should error here only if hasn't interface (has'n repl.lvl errors from inside .Get)
                        if (wrapper.Get<IEntity>(_entityRef, ReplicationLevel.Always) != null)
                            Logger.IfError()?.Message($"Can't get `{nameof(IMobMovementSync)}` by id:{_entityRef.Guid}, type:{_entityRef.TypeId} in repo:{_repository.CloudNodeType}.").Write();
                        return;
                    }

                    _curveLogProv?.CurveLogger?.IfActive?.AddData("SendFrame (inside using).Pos", SyncTime.Now, frame.Position);

                    await movementSync.UpdateMovement(packedData, frame.Timestamp, important);
                }
            });
            LocomotionProfiler.EndSample();
        }

        void ILocomotionNetworkSend.Acquire()
        {
        }

        void ILocomotionNetworkSend.Release()
        {
        }


        ///--------------------------------------------------------------------------------------------------------------------------------

        private event LocomotionNetworkReceiveDelegate FrameReceived;
        private bool _receiveReady;
        bool ILocomotionNetworkReceive.IsReady => _receiveReady;
        private bool _isCurveLoggerEnable;

        void ILocomotionNetworkReceive.Acquire(LocomotionNetworkReceiveDelegate @delegate)
        {
            var subscribe = FrameReceived == null || FrameReceived.GetInvocationList().Length == 0; 
            FrameReceived += @delegate;
            if (subscribe)
                _taskRunner(async () =>
                {
                    using (var wrapper = await _repository.Get(_entityRef))
                    {
                        if (wrapper.TryGet<IHasMobMovementSyncAlways>(_entityRef.TypeId, _entityRef.Guid, ReplicationLevel.Always, out var hasMovement))
                        {
                            hasMovement.MovementSync.OnMovementStateChanged.Action += OnReceive;
                            _receiveReady = true;
                            //Logger.IfDebug()?.Message(_entityRef.Guid, "#P3-11221: SubscribePropertyChanged (I hope..)").Write();
                            Logger.IfDebug()?.Message(_entityRef.Guid, "Subscribed on OnMovementStateChanged").Write();
                        }
                    }
                });
        }

        void ILocomotionNetworkReceive.Release(LocomotionNetworkReceiveDelegate @delegate)
        {
            FrameReceived -= @delegate;
            if(FrameReceived == null || FrameReceived.GetInvocationList().Length == 0)
                _taskRunner(async () =>
                {
                    using (var wrapper = await _repository.Get(_entityRef))
                    {
                        if (wrapper.TryGet<IHasMobMovementSyncAlways>(_entityRef.TypeId, _entityRef.Guid, ReplicationLevel.Always, out var hasMovement)) // entity к этому моменту уже могла разреплицироваться
                        {
                            hasMovement.MovementSync.OnMovementStateChanged.Action -= OnReceive;
                            //if (DbgLog.Enabled) DbgLog.Log("#P3-11221: !UN!-SubscribePropertyChanged ( X X X )");
                        }
                        _receiveReady = false;
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityRef.Guid, "Unsubscribed from OnMovementStateChanged").Write();
                    }
                });
        }
        
        private void OnReceive(MobMovementState message, long timestamp)
        {
            var frame = StateToFrame(message, timestamp);

            _curveLogProv?.CurveLogger?.IfActive?.AddData("MobTransport OnReceive (event).Pos", SyncTime.Now, frame.Position);

            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_entityRef.Guid, "Received: #{0} [{1}]", timestamp, frame).Write();
            FrameReceived?.Invoke(frame);
        }
        
        
        ///--------------------------------------------------------------------------------------------------------------------------------

        private static LocomotionVariables StateToFrame(MobMovementState message, long timestamp)
        {
            return new LocomotionVariables(
                    timestamp: timestamp,
                    flags: message.Flags,
                    position: LocomotionHelpers.WorldToLocomotionVector(message.Position),
                    velocity: LocomotionHelpers.WorldToLocomotionVector(message.Velocity),
                    angularVelocity: message.AngularVelocity,
                    orientation: -message.Orientation
                    );
        }

        private static MobMovementStatePacked FrameToPackedState(LocomotionVariables frame) => MobMovementStatePacked.Pack(FrameToState(frame), frame.Timestamp);
        private static MobMovementState FrameToState(LocomotionVariables frame)
        {
            return new MobMovementState
            (
                flags: frame.Flags,
                position: LocomotionHelpers.LocomotionToWorldVector(frame.Position),
                velocity: LocomotionHelpers.LocomotionToWorldVector(frame.Velocity),
                angularVelocity: frame.AngularVelocity,
                orientation: -frame.Orientation
            );
        }        
    }
}
