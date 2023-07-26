using System;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Debug;
using Assets.Tools;
using ColonyShared.SharedCode.Aspects.Locomotion;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Extensions;
using SharedCode.MovementSync;
using SharedCode.Serializers;

namespace Src.Locomotion
{
    public class LocomotionNetworkReclaimer : ILocomotionPipelinePassNode, IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(LocomotionNetworkReclaimer));

        private readonly object _lock = new object();
        private readonly ILocomotionNetworkReceive _transport;
        private LocomotionVariables _newReceivedFrame;
        private int _hasNewReceivedFrame;

        private readonly OuterRef<IEntityObject> _entityRef;
        private readonly IEntitiesRepository _repository;
        private readonly ILocomotionClock _clock;
        private readonly IDumpingLoggerProvider _loggerProvider;
        private readonly Type _thisType = typeof(LocomotionNetworkReclaimer);

        public LocomotionNetworkReclaimer([NotNull] ILocomotionNetworkReceive transport, 
            OuterRef<IEntityObject> entityRef, 
            IEntitiesRepository repository,
            IDumpingLoggerProvider loggerProvider)
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));
            _transport = transport;
            _transport.Acquire(Received);
            _entityRef = entityRef;
            _repository = repository;
            _loggerProvider = loggerProvider;
        }

        public void Dispose()
        {
            _transport.Release(Received);
        }

        bool ILocomotionPipelinePassNode.IsReady => true;

        LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float deltaTime)
        {
            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
            {
                --_loggerProvider.LogBackCounter;
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagIn1, vars);
            }

            if (Interlocked.Exchange(ref _hasNewReceivedFrame, 0) != 0)
            {
                LocomotionVariables frame;
                lock (_lock)
                    frame = _newReceivedFrame;

                {//#Dbg:
                    if (frame.Flags.Any(LocomotionFlags.Teleport) && _loggerProvider != null)
                        _loggerProvider.LogBackCounter = 5; //5- is just num.of cycles we want to gather data after it's set
                    if (_loggerProvider?.LogBackCounter > 0)
                        _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagIn2, frame);
                }

                if (frame.Flags.Any(LocomotionFlags.Teleport))
                {
                    //DbgLog.Log($"Reclaimer:: Pass. Got Teleport flag.");
                    var receivedFrameTimestamp  = frame.Timestamp;
                    var timestamp = vars.Timestamp;
                    AsyncUtils.RunAsyncTask(() => TeleportDone(receivedFrameTimestamp, timestamp));
                }

                frame.Timestamp = vars.Timestamp; // 'cos vars are already passed through TimestampNode

                //if (DbgLog.Enabled) DbgLog.Log($"X. Reclaimer.Pass(1-frame): pos:{frame.Position}, yaw:{frame.Orientation * SharedHelpers.Rad2Deg}");

                //#Dbg:
                if (_loggerProvider?.LogBackCounter > 0)
                    _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut1, frame);

                return frame;
            }


            //#Dbg:
            if (_loggerProvider?.LogBackCounter > 0)
                _loggerProvider?.DLogger?.IfActive?.Log(_thisType, _loggerProvider.LogBackCounter, Consts.DbgTagOut2, vars);

            return vars;
        }

        private void Received(LocomotionVariables frame)
        {
            lock (_lock)
            {
                _newReceivedFrame = frame;
                //#old: _newReceivedFrame.Timestamp = LocomotionTimestamp.None; // не нужен, и даже мешает 
                _hasNewReceivedFrame = 1;
                //if (DbgLog.Enabled) DbgLog.Log($"X. Reclaimer.Received: pos:{frame.Position}, yaw:{frame.Orientation * SharedHelpers.Rad2Deg}");
            }
        }
        
        async Task TeleportDone(long receivedFrameTimestamp, long varsTimestamp)
        {
            //if (DbgLog.Enabled) DbgLog.Log($"Reclaimer - TeleportDone call ({receivedFrameTimestamp}, {vars.Timestamp})");
            using (var wrapper = await _repository.Get(_entityRef))
            {
                var movementSync = wrapper.Get<IHasCharacterMovementSyncClientFull>(_entityRef, ReplicationLevel.ClientFull)?.MovementSync;
                if (movementSync == null)
                {
                    Logger.IfError()?.Message($"Can't get `{nameof(ICharacterMovementSync)}` by entRef:{_entityRef} in repo:{_repository.CloudNodeType}.").Write();
                    return;
                }
                await movementSync.TeleportDone(receivedFrameTimestamp, varsTimestamp);
                //DbgLog.Log($"Reclaimer:: Pass. Rpc TeleportDone sent ({receivedFrameTimestamp}, {vars.Timestamp}).");
            }
        }
    }
}