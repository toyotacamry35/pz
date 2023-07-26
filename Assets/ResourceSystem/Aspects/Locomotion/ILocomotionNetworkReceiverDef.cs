namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public interface ILocomotionNetworkReceiverDef
    {
        /// <summary>
        /// Максимальная разница между локальной позицией субъекта и предсказанной позицией на сервере (ошибка предсказания), которую не надо корректировать. 
        /// </summary>
        float PositionErrorTolerance { get; }

        /// <summary>
        /// Ориентировочное время за которое должна быть скорректирована ошибка предсказания 
        /// </summary>
        float PositionErrorCorrectionTime { get; }
        
        /// <summary>
        /// Максимальное изменение скорости при корректировке ошибки предсказания. 
        /// </summary>
        float PositionErrorCorrectionVelocity { get; }

        /// <summary>
        /// Максимальное время с момента получения последнего кадра в течении которого интерполируется текущая позиция
        /// </summary>
        float MaxExtrapolationTime { get; }

        /// <summary>
        /// Максимально допустимая скорость перемещения
        /// </summary>
        float MaxVelocity { get; }
    }
}
