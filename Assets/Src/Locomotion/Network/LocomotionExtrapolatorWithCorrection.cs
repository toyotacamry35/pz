using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion
{
    public class LocomotionExtrapolatorWithCorrection : ILocomotionNetworkExtrapolator
    {
        private readonly ISettings _settings;

        public LocomotionExtrapolatorWithCorrection(ISettings settings)
        {
            _settings = settings;
        }

        public LocomotionVariables Extrapolate(LocomotionVariables receivedFrame, float receivedFrameAt, LocomotionVariables appliedVars, ILocomotionClock clock)
        {
            var timeSinceLastReceivedFrame = (clock.Time - receivedFrameAt);
            var remotePosition = receivedFrame.Position +
                                 receivedFrame.Velocity * Min(timeSinceLastReceivedFrame, _settings.MaxExtrapolationTime);
            var localPosition = appliedVars.Position + appliedVars.Velocity * clock.DeltaTime;
            DebugAgent?.Set(NetworkPredictedPosition, remotePosition);
            var positionError = remotePosition - localPosition;
            var velocityCorrectionLimit = _settings.PositionErrorCorrectionVelocity;
            if (velocityCorrectionLimit.Sqr() < receivedFrame.Velocity.SqrMagnitude)
                velocityCorrectionLimit = receivedFrame.Velocity.Magnitude;
            var velocityCorrection =
                (positionError / clock.DeltaTime - receivedFrame.Velocity).Clamp(velocityCorrectionLimit);
            var newVelocity = (receivedFrame.Velocity + velocityCorrection).Clamp(_settings.MaxVelocity);
            var newPosition = appliedVars.Position + newVelocity * clock.DeltaTime;
            receivedFrame.Position = newPosition;
            return receivedFrame;
        }
        
        public interface ISettings
        {
            /// <summary>
            /// Максимальное изменение скорости при корректировке ошибки предсказания. 
            /// </summary>
            float PositionErrorCorrectionVelocity { get; }

            /// <summary>
            /// Максимальное время с момента получения последнего кадра в течении которого интерполируется текущая позиция
            /// </summary>
            float MaxExtrapolationTime { get; }

            /// <summary>
            /// Максимально допустимая скорость перемещения
            /// </summary>
            float MaxVelocity { get; }
        }
    }
}