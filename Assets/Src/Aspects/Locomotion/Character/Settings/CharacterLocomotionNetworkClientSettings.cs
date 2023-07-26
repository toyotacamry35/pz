using ColonyShared.SharedCode.Aspects.Locomotion;
using Assets.Src.Aspects.Impl;

namespace Src.Locomotion
{
    public class CharacterLocomotionNetworkClientSettings : 
        LocomotionNetworkSenderSettings,
        IRelevanceLevelProviderSettings,
        LocomotionSmoothingNode.ISettings,
        LocomotionDestuckerNode.ISettings,
        LocomotionNetworkReceiverPass.ISettings
    {
        private readonly CharacterLocomotionNetworkDef _network;
        private readonly CharacterLocomotionNetworkDef.AuthorityClientDef _authorityClient;

        public CharacterLocomotionNetworkClientSettings(CharacterLocomotionNetworkDef network)
        : base (network.AuthorityClient)
        {
            _network = network;
            _authorityClient = _network.AuthorityClient;
        }

        float LocomotionSmoothingNode.ISettings.SmoothFactor => _network.DamperSmoothFactor;
        float LocomotionSmoothingNode.ISettings.MaxSpeed => _network.DamperMaxSpeed;
        float LocomotionDestuckerNode.ISettings.Threshold => _network.DestuckerThreshold;
        float IRelevanceLevelProviderSettings.DistanceForMaxRelevanceLevel => _authorityClient.DistanceForMaxRelevanceLevel;
        float IRelevanceLevelProviderSettings.DistanceForMinRelevanceLevel => _authorityClient.DistanceForMinRelevanceLevel;
        float LocomotionNetworkReceiverPass.ISettings.Timeout => _network.ReceiveTimeout;
    }
}