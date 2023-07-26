using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class MobLocomotionDef : BaseResource
    {
        public ResourceRef<MobLocomotionStatsDef> Stats { get; private set; }
        public ResourceRef<MobLocomotionSettingsDef> Settings { get; private set; }
        public ResourceRef<NavMeshGroundSensorDef> GroundSensor { get; private set; }
        public ResourceRef<MobLocomotionNetworkDef> Network { get; private set; }
        public ResourceRef<LocomotionConstantsDef> Constants { get; private set; }

        public ResourceRef<MobLocomotionBindingsDef> Bindings { get; private set; }
    }
}