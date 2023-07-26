using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class LocomotionNetworkSenderDef : BaseResource
    {
        public float SendIntervalForMaxRelevanceLevel { get; private set; }= 0.05f;
        public float SendIntervalForMinRelevanceLevel { get; private set; }= 2f;
        public float PositionDiffThresholdForMaxRelevanceLevel { get; private set; }= 0.1f;
        public float PositionDiffThresholdForMinRelevanceLevel { get; private set; }= 0.1f;
        public float VelocityDiffThresholdForMaxRelevanceLevel{ get; private set; } = 0.1f;
        public float VelocityDiffThresholdForMinRelevanceLevel{ get; private set; } = 0.1f;
        public float RotationDiffThresholdForMaxRelevanceLevel{ get; private set; } = 5f;
        public float RotationDiffThresholdForMinRelevanceLevel{ get; private set; } = 5f;
        public float ZeroVelocityThreshold{ get; private set; } = 0.03f;
        public float SendOnlyImportantFlagsRelevanceLevel{ get; private set; } = 0.2f;      
    }
}
