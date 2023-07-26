using ColonyShared.SharedCode.Aspects.Locomotion;

namespace Src.Locomotion
{
    public class CharacterLocomotionNetworkServerSettings : 
        LocomotionSmoothingNode.ISettings,
        LocomotionDestuckerNode.ISettings,
        LocomotionNetworkReceiverPass.ISettings
    {
        private readonly CharacterLocomotionNetworkDef _network;

        public CharacterLocomotionNetworkServerSettings(CharacterLocomotionNetworkDef network)
        {
            _network = network;
        }

        float LocomotionSmoothingNode.ISettings.SmoothFactor => _network.DamperSmoothFactor;
        float LocomotionSmoothingNode.ISettings.MaxSpeed => _network.DamperMaxSpeed;
        float LocomotionDestuckerNode.ISettings.Threshold => _network.DestuckerThreshold;
        float LocomotionNetworkReceiverPass.ISettings.Timeout => _network.ReceiveTimeout;
    }
}
