using System;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class MoveEffectDef : SharedCode.Wizardry.SpellEffectDef
    {
        public enum RotationType
        {
            LookAtTarget,
            LookAtMoveDirection
        }
        [Flags]
        public enum MoveModifier
        {
            Run = 0x1
        }
        public enum EffectType
        {
//            MoveToInput,
            //LaunchToDirection,
            StrafeAroundTarget,
//            MoveToPosition, // Is not used now (straight motion ignoring navmesh)
//            FollowTarget,   // Is not used now (straight motion ignoring navmesh)
            FollowPathToPosition,
            FollowPathToTarget,
            JumpToTargetPosition,
            LookAt
        }
        public bool KeepDistance          { get; set; }
        public float KeepDistanceTreshold { get; set; }
        public string AnimationSubType    { get; set; }
        public int AnimationSubTypeValue  { get; set; }
        public EffectType MoveType        { get; set; }
        public MoveModifier Modifier      { get; private set; }
        public RotationType Rotation      { get; set; }
        public float SpeedFactor          { get; set; } = 1;

        public ResourceRef<SpellTargetDef> Target { get; set; }
        public ResourceRef<SpellVector3Def> Vec3 { get; set; }
        public SharedCode.Utils.Vector3 FixedDirection { get; set; }
        public float AcceptedRange { get; set; }
        public bool StopSpell { get; set; } = true;
        
//       public bool InverseMovement { get; set; }
//       public string AnimationBool { get; set; }
//       public bool UseAsInputToPlayer { get; set; }
//        public bool Levitate { get; set; }
//        public bool ApplyHeightSpeedCurve { get; set; }
//        public float TargetHeightSpeed { get; set; }
//        public bool ApplyHeightCurve { get; set; }
//        public float TargetHeight { get; set; }
//        public float Period { get; set; }
//        public bool FastFullStop { get; set; }
//        public float RotationTime { get; set; }
//        public float Speed { get; set; }
    }
}
