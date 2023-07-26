using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class CharacterLocomotionNetworkDef : BaseResource
    {
        public float DamperSmoothFactor                { get; set; } //= 1f;
        public float DamperMaxSpeed    { get; set; }
        public float DestuckerThreshold    { get; set; }

        public float ReceiveTimeout { get; set; } = 1;
        
        public AuthorityClientDef AuthorityClient { get; private set; }

        public class AuthorityClientDef : LocomotionNetworkSenderDef
        {
            public float DistanceForMaxRelevanceLevel { get; private set; } = 5f;
            public float DistanceForMinRelevanceLevel { get; private set; } = 25f;
        }
    }
}