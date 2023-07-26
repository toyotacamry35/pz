namespace Src.Locomotion
{
    public interface ILocomotionConstants
    {
        /// <summary>
        /// Значение ввода при котором должно начинаться движение
        /// </summary>
        float InputMoveThreshold  { get; }

        /// <summary>
        /// Значение ввода при котором скорость ходьбы максимальна и далее она должна переходить в бег
        /// </summary>
        float InputRunThreshold  { get; }

        /// <summary>
        /// Значение ввода при котором ходьба должна переходить в standing
        /// </summary>
        float InputStandingThreshold { get; }
        
        /// <summary>
        /// Угол наклона поверхности начиная с которого она считается вертикальной и на ней нельзя стоять (радианы)
        /// </summary>
        float VerticalSlopeAngle { get; }
    }
}