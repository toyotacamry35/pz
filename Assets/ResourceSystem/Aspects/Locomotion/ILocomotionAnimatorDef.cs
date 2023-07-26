namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public interface ILocomotionAnimatorDef
    {
        float MovementDirectionSmoothness { get; }

        float MovementSpeedSmoothness { get; }
        
        float MotionThreshold { get; }
    }
}