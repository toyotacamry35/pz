using SharedCode.Utils;

namespace Src.Locomotion.Delegates
{
    public delegate float SpeedByDirFn(Vector2 direction);
    public delegate float SlopeByDirFn(Vector2 direction);
    public delegate float SlipAccelBySlopeFn(float slopeAngle);
    public delegate float SlipSpeedBySlopFn(float slopeAngle);
    public delegate float SpeedByTimeFn(float time);
    public delegate ObstacleInfo ObstacleByDirFn(LocomotionVector direction, float distance, float maxStepUp);
    public delegate float SpeedByDirAndTimeFn(Vector2 dir, float airborneTime);
    public delegate float AccelByTimeFn(float airborneTime);
    public delegate float ImpulseByDirFn(Vector2 dir);
    public delegate float AccelBySlopeFn(float slope);
}