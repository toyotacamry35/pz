namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public interface ILocomotionNetworkRelevanceDef
    {
        /// <summary>
        /// Дистанция при котрой relevance level должен быть равен 1 
        /// </summary>
        float DistanceForMaxRelevanceLevel { get; } 

        /// <summary>
        /// Дистанция при котрой relevance level должен быть равен 0 
        /// </summary>
        float DistanceForMinRelevanceLevel { get; }
    }
}