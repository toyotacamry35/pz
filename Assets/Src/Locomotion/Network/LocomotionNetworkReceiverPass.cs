using System;
using System.Threading;
using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion
{
    public class LocomotionNetworkReceiverPass : ILocomotionPipelinePassNode, IDisposable
    {
        private static readonly LocomotionLogger Logger = LocomotionLogger.GetLogger(nameof(LocomotionNetworkReceiverPass));
        
        private readonly object _lock = new object();
        private readonly ILocomotionNetworkReceive _transport;
        private volatile bool _isReady;
        private LocomotionVariables _receivedFrame = LocomotionVariables.None;
        private volatile int _hasReceivedFrame = 0;
        private readonly ISettings _settings;
        private float _timeSinceLastReceive;

        bool ILocomotionPipelinePassNode.IsReady => _isReady;

        public LocomotionNetworkReceiverPass(ILocomotionNetworkReceive transport, ISettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _transport.Acquire(Received);
        }

        public LocomotionVariables Pass(LocomotionVariables vars, float dt)
        {
            LocomotionProfiler.BeginSample("## LocomotionEngine.Execute: 2) _fetchNode.Fetch [[LocomotionNetworkReceiver]]");

            if (!_isReady) throw new InvalidOperationException($"{nameof(LocomotionNetworkReceiver)} is not ready");

            if (Interlocked.Exchange(ref _hasReceivedFrame, 0) != 0)
            {
                LocomotionVariables receivedFrame;
                lock (_lock)
                    receivedFrame = _receivedFrame;

                _timeSinceLastReceive = 0;

                Logger.IfTrace()?.Message($"Frame received | Frame:{receivedFrame}").Write();

                if (DebugAgent.IsNotNullAndActive())
                {
                    DebugAgent.Set(NetworkReceivedPosition, receivedFrame.Position);
                    DebugAgent.Set(NetworkReceivedVelocity, receivedFrame.Velocity);
                    DebugAgent.Set(NetworkReceivedOrientation, receivedFrame.Orientation);
                    DebugAgent.Set(NetworkReceivedFrameId, receivedFrame.Timestamp);
                }

                LocomotionProfiler.EndSample();
                return receivedFrame;
            }
            else
            {
                _timeSinceLastReceive += dt;
                if (_timeSinceLastReceive > _settings.Timeout)
                {
                    // данные сильно устарели, поэтому искусственно гасим скорость, чтобы предотвратить бесконечное улетание тела 
                    vars.Velocity *= 0.5f;
                    vars.ExtraVelocity *= 0.5f;
                    vars.AngularVelocity *= 0.5f;
                }
            }

            LocomotionProfiler.EndSample();
            return vars;
        }
        
        void IDisposable.Dispose()
        {
            _transport.Release(Received);
        }

        private void Received(LocomotionVariables frame)
        {
            lock (_lock)
            {
                if (frame.Timestamp < _receivedFrame.Timestamp)
                    return;
                _receivedFrame = frame;
                _hasReceivedFrame = 1;
                _isReady = true;
            }
        }

        public interface ISettings
        {
            float Timeout { get; }
        }
    }
}