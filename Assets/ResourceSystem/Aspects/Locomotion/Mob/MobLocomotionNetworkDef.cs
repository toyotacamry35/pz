    using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class MobLocomotionNetworkDef : BaseResource
    {
        public MobLocomotionNetworkClientDef Client { get; set; }
        public MobLocomotionNetworkServerDef Server { get; set; }
    }
    
    
    public class MobLocomotionNetworkClientDef : LocomotionDamperNodeDef
    {
        public float MaxExtrapolationTime            { get; set; } = 0.4f;
        public float DistanceForMaxRelevanceLevel { get; set; } = 5f;
        public float DistanceForMinRelevanceLevel { get; set; } = 25f;
        public float PrefetchTime { get; set; } = 0.5f; // sec
        public float PrefetchChangingTime { get; set; } = 0.1f; // sec
    }

    
    public class MobLocomotionNetworkServerDef : LocomotionNetworkSenderDef
    {
        public float DistanceForMaxRelevanceLevel { get; set; } = 5f;
        public float DistanceForMinRelevanceLevel { get; set; } = 25f;
        public float PrefetchTime { get; set; } = 0.5f; // sec
        public float PrefetchChangingTime { get; set; } = 0.1f; // sec
    }
}