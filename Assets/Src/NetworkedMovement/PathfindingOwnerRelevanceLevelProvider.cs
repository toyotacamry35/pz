using Src.Locomotion;

namespace Src.NetworkedMovement
{
    public class PathfindingOwnerRelevanceLevelProvider : IMobRelevanceProvider, ILocomotionRelevancyLevelProvider
    {
        public float ClosestObserverDistance { get; } = 0;
        public float Dbg_ClosestObserverDistance_Forced_DANGER { get; } = 0;
        public float RelevancyLevelForNetwork { get; } = 1;
    }
}