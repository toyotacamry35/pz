using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class NavMeshGroundSensorDef : BaseResource
    {
        public float RaycastGroundTolerance { get; set; }
    }
}
