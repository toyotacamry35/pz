using Assets.Src.Aspects.Templates;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class CharacterLocomotionDef : BaseResource
    {
        public ResourceRef<CharacterLocomotionStatsDef> Stats 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CharacterLocomotionSettingsDef> Settings { get; [UsedImplicitly] set; }
        public ResourceRef<CharacterLocomotionBindingsDef> Bindings { get; [UsedImplicitly] set; }
        public ResourceRef<RaycastGroundSensorDef> GroundSensor 			{ get; [UsedImplicitly] set; }
        public ResourceRef<RaycastObstacleSensorDef> ObstaclesSensor 		{ get; [UsedImplicitly] set; }
        public ResourceRef<CharacterLocomotionNetworkDef> Network   { get; [UsedImplicitly] set; }
        public ResourceRef<LocomotionConstantsDef> Constants 		{ get; [UsedImplicitly] set; }
        public ResourceRef<RigidbodyDef> Rigidbody { get; [UsedImplicitly] set; }
        public ResourceRef<CapsuleColliderDef> Collider { get; [UsedImplicitly] set; }
        public ResourceRef<CharacterControllerDef> CharacterController { get; [UsedImplicitly] set; }
        public ResourceRef<LocomotionColliderResizerDef> ColliderResizer { get; [UsedImplicitly] set; }
        public ResourceRef<CapsuleColliderDef> SoftCollider { get; [UsedImplicitly] set; }
    }
}