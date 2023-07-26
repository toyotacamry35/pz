using Assets.Src.Src.NetworkedMovement;
using ColonyShared.SharedCode.Aspects.Locomotion;

namespace Src.Locomotion
{
    public class MobLocomotionNetworkServerSettings :
        LocomotionNetworkSenderSettings,
        IRelevanceLevelProviderSettings,
        LocomotionInterpolationBufferNode.ISettings
    {
        private readonly MobLocomotionNetworkServerDef _def;

        public MobLocomotionNetworkServerSettings(MobLocomotionNetworkServerDef def)
            : base(def)
        {
            _def = def;
        }

        float IRelevanceLevelProviderSettings.DistanceForMaxRelevanceLevel => _def.DistanceForMaxRelevanceLevel;
        float IRelevanceLevelProviderSettings.DistanceForMinRelevanceLevel => _def.DistanceForMinRelevanceLevel;
        long LocomotionInterpolationBufferNode.ISettings.PrefetchTime => LocomotionTimestamp.FromSeconds(_def.PrefetchTime);
        float LocomotionInterpolationBufferNode.ISettings.PrefetchChangingTime => _def.PrefetchChangingTime;
    }
}