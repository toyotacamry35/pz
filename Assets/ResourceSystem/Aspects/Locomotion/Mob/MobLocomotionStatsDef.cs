using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    /// <summary>
    /// Изменяющиеся параметры перемещения субъекта (например, зависящие от статов)
    /// </summary>
    public class MobLocomotionStatsDef : BaseResource
    {
        public float MinSpeed             { get; set; }
        public float WalkingSpeedFwd      { get; set; }
        public float WalkingSpeedSide     { get; set; }
        public float WalkingSpeedBwd      { get; set; }
        public float RunningSpeed         { get; set; }
        public float JumpVerticalImpulse       { get; set; }
        public float JumpHorizontalImpulseFwd  { get; set; }
        public float JumpHorizontalImpulseSide { get; set; }
        public float JumpHorizontalImpulseBwd  { get; set; }
        public float JumpToTargetMinDistance { get; set; }
        public float JumpToTargetMaxDistance { get; set; }
        public float JumpToTargetMaxHeight   { get; set; }
        public float DodgeSpeed       { get; set; } 
        public float AirControlSpeed  { get; set; }
        public float StandingYawSpeed { get; set; }
        public float WalkingYawSpeed  { get; set; }
        public float RunningYawSpeed  { get; set; }
        public float JumpYawSpeed     { get; set; }
        public float WalkingAccel     { get; set; }
        public float RunningAccel     { get; set; }
        public float Decel            { get; set; }
    }
}