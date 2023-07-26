namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public interface ILocomotionNetworkSenderDef
    {
        /// <summary>
        /// Порог разницы позиций между кадрами достаточный для отправки нового кадра
        /// </summary>
        float SendPositionDiffThreshold(float relevanceLevel);
        
        /// <summary>
        /// Порог разницы скоростей между кадрами достаточный для отправки нового кадра
        /// </summary>
        float SendVelocityDiffThreshold(float relevanceLevel);
        
        /// <summary>
        /// Порог разницы ориентации между кадрами достаточный для отправки нового кадра (радианы)
        /// </summary>
        float SendRotationDiffThreshold(float relevanceLevel);

        /// <summary>
        /// Порог разницы угла наклона между кадрами достаточный для отправки нового кадра (радианы)
        /// </summary>
        float SendPitchDiffThreshold(float relevanceLevel);
        
        /// <summary>
        /// Минимальный период отсылки кадров
        /// </summary>
        float SendInterval(float relevancyLevel);
        
        /// <summary>
        /// Порог значения скорости ниже которого скорость считается нулевой  
        /// </summary>
        float ZeroVelocityThreshold { get; }
        
        bool SendOnlyImportantFlags(float relevanceLevel);
    }
}