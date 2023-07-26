using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class LocomotionDamperNodeDef : BaseResource
    {
        public float DamperMinDeltaPosition          { get; set; } //= 0.1f;
        public float DamperMinDeltaRotationDeg       { get; set; } //= 5f; //3.5f
        public float DamperSmoothTime                { get; set; } //= 1f;
        public float DamperMaxSpeedFactor_TmpHere    { get; set; } //= 1f; //0.7f;
        public float ObjectMinSpeed    { get; set; }
        public float ObjectMaxSpeed    { get; set; }
        public float ObjectMinYawSpeedDeg    { get; set; }
        public float ObjectMaxYawSpeedDeg    { get; set; }
    }
}