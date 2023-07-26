namespace Src.Locomotion
{
    public interface ILocomotionHistory
    {
        /// <summary>
        /// Время нахождения в отрыве от земли
        /// </summary>
        float AirborneTime { get; }

        /// <summary>
        /// Высшая точка траектории полёта
        /// </summary>
        LocomotionVector AirborneApex { get; }
        
        /// <summary>
        /// Вертикальное расстояние от наивысшей точки текущего падения. Становится равным 0 в момент касания земли!
        /// </summary>
        float FallingDistance { get; }

        /// <summary>
        /// Высота последнего падения. В отличии от FallingDistance не обнуляется при касании земли.
        /// </summary>
        float LastFallHeight { get; }

        void ResetAirborne();
    }
}