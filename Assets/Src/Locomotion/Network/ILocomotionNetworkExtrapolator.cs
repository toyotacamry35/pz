namespace Src.Locomotion
{
    public interface ILocomotionNetworkExtrapolator
    {
        LocomotionVariables Extrapolate(LocomotionVariables receivedFrame, float receivedFrameAt, LocomotionVariables appliedVars, ILocomotionClock clock);
    }
}