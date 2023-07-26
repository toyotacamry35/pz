using ColonyShared.SharedCode.Aspects.Locomotion;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public class LocomotionNetworkSenderSettings : LocomotionNetworkSender.ISettings
    {
        private readonly LocomotionNetworkSenderDef _def;

        public LocomotionNetworkSenderSettings(LocomotionNetworkSenderDef def)
        {
            _def = def;
        }

        float LocomotionNetworkSender.ISettings.SendInterval(float relevancyLevel)
            => Lerp(_def.SendIntervalForMinRelevanceLevel, _def.SendIntervalForMaxRelevanceLevel, relevancyLevel);

        float LocomotionNetworkSender.ISettings.SendPositionDiffThreshold(float relevancyLevel) =>
            Lerp(_def.PositionDiffThresholdForMinRelevanceLevel, _def.PositionDiffThresholdForMaxRelevanceLevel, relevancyLevel);

        float LocomotionNetworkSender.ISettings.SendVelocityDiffThreshold(float relevancyLevel) =>
            Lerp(_def.VelocityDiffThresholdForMinRelevanceLevel, _def.VelocityDiffThresholdForMaxRelevanceLevel, relevancyLevel);

        float LocomotionNetworkSender.ISettings.SendRotationDiffThreshold(float relevancyLevel) =>
            Lerp(_def.RotationDiffThresholdForMinRelevanceLevel, _def.RotationDiffThresholdForMaxRelevanceLevel, relevancyLevel) * Deg2Rad;

        float LocomotionNetworkSender.ISettings.ZeroVelocityThreshold => _def.ZeroVelocityThreshold;

        bool LocomotionNetworkSender.ISettings.SendOnlyImportantFlags(float relevancyLevel) =>
            relevancyLevel < _def.SendOnlyImportantFlagsRelevanceLevel;
    }
}