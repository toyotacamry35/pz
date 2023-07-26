using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class LocomotionExtrapolatorWithCorrectionDef : BaseResource
    {
        public float PositionErrorCorrectionVelocity { get; set; }= 10f;
        public float MaxExtrapolationTime            { get; set; }= 0.4f;
        public float MaxVelocity                     { get; set; }= 10f;
    }
}
