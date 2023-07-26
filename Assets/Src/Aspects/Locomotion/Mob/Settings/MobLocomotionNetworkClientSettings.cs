using ColonyShared.SharedCode.Aspects.Locomotion;

namespace Src.Locomotion
{
    public class MobLocomotionNetworkClientSettings : 
        LocomotionSimpleExtrapolationNode.ISettings,
        IRelevanceLevelProviderSettings,
        LocomotionInterpolationBufferNode.ISettings
    {
        private readonly MobLocomotionNetworkClientDef _def;

        public MobLocomotionNetworkClientSettings(MobLocomotionNetworkClientDef def)
        {
            _def = def;
        }
        
        float LocomotionSimpleExtrapolationNode.ISettings.MaxExtrapolationTime => _def.MaxExtrapolationTime;
        public float DamperMinDeltaPosition => _def.DamperMinDeltaPosition;
        public float DamperMinDeltaRotationDeg => _def.DamperMinDeltaRotationDeg;
        public float DamperSmoothTime       => _def.DamperSmoothTime;
        public float DamperMaxSpeedFactor_TmpHere   => _def.DamperMaxSpeedFactor_TmpHere; //#todo(when needed): it's should be mob-specific, so move it to mob stats.
        float IRelevanceLevelProviderSettings.DistanceForMaxRelevanceLevel => _def.DistanceForMaxRelevanceLevel;
        float IRelevanceLevelProviderSettings.DistanceForMinRelevanceLevel => _def.DistanceForMinRelevanceLevel;
        long LocomotionInterpolationBufferNode.ISettings.PrefetchTime => LocomotionTimestamp.FromSeconds(_def.PrefetchTime);
        float LocomotionInterpolationBufferNode.ISettings.PrefetchChangingTime => _def.PrefetchChangingTime;
    }
}