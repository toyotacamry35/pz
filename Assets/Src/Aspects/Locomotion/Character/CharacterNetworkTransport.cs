using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;

namespace Src.Locomotion
{
    
    public class CharacterNetworkTransport 
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(CharacterNetworkTransport));

        private readonly AsyncTaskRunner _taskRunner;

        public CharacterNetworkTransport([NotNull] AsyncTaskRunner taskRunner)
        {
            _taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
        }

        public ILocomotionNetworkSend CreateMasterSend(OuterRef<IEntityObject> entityRef, IEntitiesRepository repository)
        {
            return new MasterSend(entityRef.To<IEntity>(), repository, _taskRunner);
        }

        public ILocomotionNetworkReceive CreateMasterReceive(OuterRef<IEntityObject> entityRef, IEntitiesRepository repository)
        {
            return new MasterReceive(entityRef.To<IEntity>(), repository, _taskRunner);
        }
        
        public ILocomotionNetworkReceive CreateSlaveReceive(OuterRef<IEntityObject> entityRef, IEntitiesRepository repository)
        {
            return new SlaveReceive(entityRef.To<IEntity>(), repository, _taskRunner);
        }

        private static LocomotionVariables StateToFrame(CharacterMovementState message, long timestamp)
        {
            return new LocomotionVariables(
                timestamp: timestamp,
                flags: message.Flags,
                position: LocomotionHelpers.WorldToLocomotionVector(message.Position),
                velocity: LocomotionHelpers.WorldToLocomotionVector(message.Velocity),
                orientation: -message.Orientation
                );
        }

        private static CharacterMovementState FrameToState(LocomotionVariables frame)
        {
            return new CharacterMovementState
            (
                flags: frame.Flags,
                position: LocomotionHelpers.LocomotionToWorldVector(frame.Position),
                velocity: LocomotionHelpers.LocomotionToWorldVector(frame.Velocity),
                orientation: -frame.Orientation
            );
        }        
        
        ///--------------------------------------------------------------------------------------------------------------------------------
        private class MasterSend : ILocomotionNetworkSend
        {
            private readonly OuterRef<IEntity> _entityRef;
            private readonly IEntitiesRepository _repository;
            private readonly AsyncTaskRunner _taskRunner;
            private readonly ConcurrentBag<DelegateBucket> _delegatesPool = new ConcurrentBag<DelegateBucket>();

            public MasterSend(OuterRef<IEntity> entityRef, IEntitiesRepository repository, AsyncTaskRunner taskRunner)
            {
                _entityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
            }

            bool ILocomotionNetworkSend.IsReady => true;

            void ILocomotionNetworkSend.SendFrame(LocomotionVariables frame, bool important, ReasonForSend reason)
            {
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_entityRef.Guid, "Send due {0}: #{1} [{2}]{3}", reason, frame.Timestamp, frame, important?" IMPORTANT":"")
                    .Write();
                var bucket = AcquireBucket();
                bucket.Data = FrameToState(frame);
                bucket.Timestamp = frame.Timestamp;
                bucket.Important = important;
                _taskRunner(bucket.Action);
            }

            private async Task SendFrameAsync(DelegateBucket bucket)
            {
                try
                {
                    using (var wrapper = await _repository.Get(_entityRef))
                    {
                        var movementSync = wrapper.Get<IHasCharacterMovementSyncClientFull>(_entityRef, ReplicationLevel.ClientFull);
                        await movementSync.MovementSync.UpdateMovement(bucket.Data, bucket.Important, bucket.Timestamp);
                    }
                }
                finally
                {
                    ReleaseBucket(bucket);
                }
            }
            
            void ILocomotionNetworkSend.Acquire() {}

            void ILocomotionNetworkSend.Release() {}

            private DelegateBucket AcquireBucket()
            {
                if (!_delegatesPool.TryTake(out var bucket))
                {
                    var newBucket = new DelegateBucket();
                    newBucket.Action = () => SendFrameAsync(newBucket);
                    bucket = newBucket;
                }
                return bucket;
            }

            private void ReleaseBucket(DelegateBucket bucket)
            {
                if (_delegatesPool.Count > 32)
                    if (Logger.IsErrorEnabled) Logger.IfError()?.Message($"Delegates pool is too large: {_delegatesPool.Count}").Write();
                else
                    _delegatesPool.Add(bucket);
            }

            private class DelegateBucket
            {
                public Func<Task> Action;
                public CharacterMovementState Data;
                public bool Important;
                public long Timestamp;
            }
        }
        
        
        
        ///--------------------------------------------------------------------------------------------------------------------------------
        private class MasterReceive : ILocomotionNetworkReceive
        {
            private readonly OuterRef<IEntity> _entityRef;
            private readonly IEntitiesRepository _repository;
            private readonly AsyncTaskRunner _taskRunner;
            private bool _isReady;

            private event LocomotionNetworkReceiveDelegate FrameReceived;

            public MasterReceive(OuterRef<IEntity> entityRef, IEntitiesRepository repository, AsyncTaskRunner taskRunner)
            {
                _entityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
            }

            bool ILocomotionNetworkReceive.IsReady => _isReady;
            
            void ILocomotionNetworkReceive.Acquire(LocomotionNetworkReceiveDelegate @delegate)
            {
                var subscribe = FrameReceived == null || FrameReceived.GetInvocationList().Length == 0; 
                FrameReceived += @delegate;
                if (subscribe)
                    _taskRunner(async () =>
                    {
                        using (var wrapper = await _repository.Get(_entityRef))
                        {
                            var movementSync = wrapper.Get<IHasCharacterMovementSyncClientFull>(_entityRef, ReplicationLevel.ClientFull);
                            movementSync.MovementSync.OnMovementStateReclaimed.Action += OnReceive;
                            _isReady = true;
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityRef.Guid, "Subscribed on OnMovementStateReclaimed").Write();;
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
                            var movementSync = wrapper.Get<IHasCharacterMovementSyncClientFull>(_entityRef, ReplicationLevel.ClientFull);
                            movementSync.MovementSync.OnMovementStateReclaimed.Action -= OnReceive;
                            _isReady = false;
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityRef.Guid, "Unsubscribed from OnMovementStateReclaimed").Write();;
                        }
                    });
            }
            
            private void OnReceive(CharacterMovementState message, long timestamp)
            {
                var frame = StateToFrame(message, timestamp);
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_entityRef.Guid, "Receive reclaimed: #{0} [{1}]", timestamp, frame)
                    .Write();
                FrameReceived?.Invoke(frame);
            }
        }
        
        
        ///--------------------------------------------------------------------------------------------------------------------------------
        private class SlaveReceive : ILocomotionNetworkReceive
        {
            private readonly OuterRef<IEntity> _entityRef;
            private readonly IEntitiesRepository _repository;
            private readonly AsyncTaskRunner _taskRunner;
            private bool _isReady;

            private event LocomotionNetworkReceiveDelegate FrameReceived;

            public SlaveReceive(OuterRef<IEntity> entityRef, IEntitiesRepository repository, AsyncTaskRunner taskRunner)
            {
                _entityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                _taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
            }

            bool ILocomotionNetworkReceive.IsReady => _isReady;

            void ILocomotionNetworkReceive.Acquire(LocomotionNetworkReceiveDelegate @delegate)
            {
                var subscribe = FrameReceived == null || FrameReceived.GetInvocationList().Length == 0; 
                FrameReceived += @delegate;
                if (subscribe)
                    _taskRunner(async () =>
                    {
                        using (var wrapper = await _repository.Get(_entityRef))
                        {
                            var movementSync = wrapper.Get<IHasCharacterMovementSyncAlways>(_entityRef, ReplicationLevel.Always);
                            movementSync.MovementSync.OnMovementStateChanged.Action += OnReceive;
                            _isReady = true;
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityRef.Guid, "Subscribed on OnMovementStateChanged").Write();;
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
                            var movementSync = wrapper.Get<IHasCharacterMovementSyncClientBroadcast>(_entityRef, ReplicationLevel.ClientBroadcast);
                            if (movementSync != null) // entity к этому моменту уже могла разреплицироваться
                                movementSync.MovementSync.OnMovementStateChanged.Action -= OnReceive;
                            _isReady = true;
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityRef.Guid, "Unsubscribed from OnMovementStateChanged").Write();;
                        }
                    });
            }
            
            private void OnReceive(CharacterMovementState message, long timestamp)
            {
                var frame = StateToFrame(message, timestamp);
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message(_entityRef.Guid, "Received: #{0} [{1}]", timestamp, frame)
                    .Write();
                FrameReceived?.Invoke(frame);
            }
        }
    }
}
