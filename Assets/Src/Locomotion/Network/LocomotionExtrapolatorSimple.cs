using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion
{
    public class LocomotionExtrapolatorSimple : ILocomotionNetworkExtrapolator
    {
        public LocomotionVariables Extrapolate(LocomotionVariables receivedFrame, float receivedFrameAt, LocomotionVariables appliedVars, ILocomotionClock clock)
        {
            var timeSinceLastReceivedFrame = clock.Time - receivedFrameAt;
            var remotePosition = receivedFrame.Position + receivedFrame.Velocity * timeSinceLastReceivedFrame; // extrapolation
            DebugAgent?.Set(NetworkPredictedPosition, remotePosition);
            return new LocomotionVariables(receivedFrame) { Position = remotePosition };
        }
    }
}
