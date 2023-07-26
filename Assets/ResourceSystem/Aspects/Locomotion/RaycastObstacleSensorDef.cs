using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class RaycastObstacleSensorDef : BaseResource
    {
        public float ColliderTolerance { get; set; }
        public float MinStairHeightFactor { get; set; }   
    }
}