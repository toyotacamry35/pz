using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    /// <summary>
    /// Изменяющиеся параметры перемещения субъекта (например, зависящие от статов)
    /// </summary>
    public class CharacterLocomotionStatsDef : BaseResource
    {
        public ResourceRef<CalcerDef<float>> MinSpeed 			{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> WalkingSpeedFwd 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> WalkingSpeedSide 	{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> WalkingSpeedBwd 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> RunningSpeedFwd 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> RunningSpeedSide 	{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> RunningSpeedBwd 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> BlockingSpeedFwd 	{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> BlockingSpeedSide 	{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> BlockingSpeedBwd 	{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> SprintSpeedFwd 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> JumpVerticalImpulse 	{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> JumpHorizontalImpulseFwd  { get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> JumpHorizontalImpulseSide { get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> JumpHorizontalImpulseBwd  { get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> SlippingControlSpeed { get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> AirControlSpeed 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> StandingYawSpeed 	{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> WalkingYawSpeed 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> SprintYawSpeed 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> SlipYawSpeed 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> JumpYawSpeed 		{ get; [UsedImplicitly] set; }
    }
}