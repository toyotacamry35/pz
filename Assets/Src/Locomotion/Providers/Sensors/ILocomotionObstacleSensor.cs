namespace Src.Locomotion
{
    public interface ILocomotionObstacleSensor
    {
        /// <summary>
        /// Обнаружение помехи движению в заданном направлении на заданном расстоянии от исходной точки.   
        /// </summary>
        ObstacleInfo DetectObstacle(LocomotionVector direction, float distance, float maxStepUp);
    }
}