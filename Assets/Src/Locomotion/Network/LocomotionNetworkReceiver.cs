using System;
using System.Threading;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.Src.Locomotion.Debug;
using Assets.Src.Locomotion.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;
using UnityEngine;
using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.DebugTag;
using ILogger = NLog.ILogger;

namespace Src.Locomotion
{
    public class LocomotionNetworkReceiver : ILocomotionPipelineFetchNode, IDisposable, IStopAndRestartable
    {
        private readonly object _lock = new object();
        private readonly ILocomotionNetworkReceive _transport;
        private readonly ILocomotionNetworkExtrapolator _extrapolator;
        private readonly ILocomotionClock _clock;
        private readonly Guid _entityId;
        private readonly ICurveLoggerProvider _curveLogProv;
        private readonly IFrameIdNormalizer _frameIdNormalizer;
        private LocomotionVariables _appliedVars = LocomotionVariables.None;
        private LocomotionVariables _receivedFrame = LocomotionVariables.None;
        private float _receivedFrameAt;
        private LocomotionVariables _newReceivedFrame = LocomotionVariables.None;
        // Int instead of bool, to be able use interlocked.Exchange
        private volatile int _hasNewReceivedFrame = 0; // 0 - False
        private volatile bool _isReady;
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        public LocomotionNetworkReceiver(
            ILocomotionNetworkReceive transport,
            ILocomotionClock clock,
            Guid entityId,
            [CanBeNull] ILocomotionNetworkExtrapolator extrapolator,
            [CanBeNull] ILocoCurveLoggerProvider curveLogProv = null)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _extrapolator = extrapolator;
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _entityId = entityId;
            _transport.Acquire(Received);
            _curveLogProv = curveLogProv;
            _frameIdNormalizer = (IFrameIdNormalizer)curveLogProv ?? DefaultFrameIdNormalizer.Instance;
        }

        public void Stop()
        {
            _transport.Release(Received);

            _appliedVars   = LocomotionVariables.None;
            _receivedFrame = LocomotionVariables.None;
            _receivedFrameAt = default;
            lock (_lock)
            {
                _newReceivedFrame = LocomotionVariables.None;
                _hasNewReceivedFrame = 0;
                _isReady = false;
            }
        }
        public void Restart()
        {
#if DEBUG
            Debug.Assert(_appliedVars.Equals(LocomotionVariables.None));
            Debug.Assert(_receivedFrame.Equals(LocomotionVariables.None));
            Debug.Assert(_receivedFrameAt == default);
            lock (_lock)
            {
                Debug.Assert(_newReceivedFrame.Equals(LocomotionVariables.None));
                Debug.Assert(_hasNewReceivedFrame == 0);
                Debug.Assert(_isReady == false);
            }
#endif

            _transport.Acquire(Received);
        }

        bool ILocomotionPipelineFetchNode.IsReady => _isReady;

        LocomotionVariables ILocomotionPipelineFetchNode.Fetch(float dt)
        {
            if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## LocomotionEngine.Execute: 2) _fetchNode.Fetch [[LocomotionNetworkReceiver]]");

            if (!_isReady) throw new InvalidOperationException($"{nameof(LocomotionNetworkReceiver)} is not ready");
            bool hasNewReceivedFrame = Interlocked.Exchange(ref _hasNewReceivedFrame, 0) != 0;

            if (hasNewReceivedFrame)
            {
                var firstReceivedFrame = !_receivedFrame.Timestamp.Valid;
                lock (_lock)
                    _receivedFrame = _newReceivedFrame;                
                _receivedFrameAt = _clock.Time;
                if (firstReceivedFrame || (_receivedFrame.Flags & (LocomotionFlags.Teleport | LocomotionFlags.CheatMode)) != 0)
                    _appliedVars = _receivedFrame;

                // _logger.Info($"{_entityId} Received frame with velocity={_receivedFrame.Velocity} Orientation={_receivedFrame.Orientation} Position={_receivedFrame.Position}");
                if (DebugAgent.IsNotNullAndActive())
                { 
                    DebugAgent.Set(NetworkReceivedPosition, _receivedFrame.Position);
                    DebugAgent.Set(NetworkReceivedVelocity, _receivedFrame.Velocity);
                    DebugAgent.Set(NetworkReceivedFrameId,  _receivedFrame.Timestamp);
                }

                if (_curveLogProv?.CurveLogger?.IfActive != null)
                {
                    _curveLogProv?.CurveLogger?.IfActive?.AddData("1)Cl_Received.Pos", SyncTime.Now, _receivedFrame.Position);
                    _curveLogProv?.CurveLogger?.IfActive?.AddData("1)Cl_Received.Velo", SyncTime.Now, _receivedFrame.Velocity);
                    _curveLogProv?.CurveLogger?.IfActive?.AddData("1)Cl_Received.VeloAsDPos", SyncTime.Now, CurveLoggerExt.VeloAsDltPos(ref _receivedFrame));
                    _curveLogProv?.CurveLogger?.IfActive?.AddData("1)Cl_Received.FrameId", SyncTime.Now, _frameIdNormalizer.NormalizeFrameId(_receivedFrame.Timestamp));
                }
            }

            if (_extrapolator != null)
                _appliedVars = _extrapolator.Extrapolate(_receivedFrame, _receivedFrameAt, _appliedVars, _clock);
            else
                _appliedVars =  _receivedFrame;

            LocomotionProfiler.EndSample();

            return _appliedVars;
        }

        void IDisposable.Dispose()
        {
            _transport.Release(Received);
        }

        private void Received(LocomotionVariables frame)
        {
            lock (_lock)
            {
                if (frame.Timestamp < _newReceivedFrame.Timestamp)
                    return;
                _newReceivedFrame = frame;
                _hasNewReceivedFrame = 1;
                _isReady = true;
            }
        }
    }
}