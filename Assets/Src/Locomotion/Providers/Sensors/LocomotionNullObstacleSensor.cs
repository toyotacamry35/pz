namespace Src.Locomotion
{
    public class LocomotionNullObstacleSensor : ILocomotionObstacleSensor
    {
        public ObstacleInfo DetectObstacle(LocomotionVector direction, float distance, float maxStepUp)
        {
            return new ObstacleInfo();
        }
    }
}