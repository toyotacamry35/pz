using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using JetBrains.Annotations;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class LocomotionColliderResizerDef : BaseResource
    {
        public float BlendTime { get; [UsedImplicitly] set; } = 0.1f;
        public LocomotionColliderResizerPreset[] Presets { get; [UsedImplicitly] set; }
    }
    
    [KnownToGameResources]
    public class LocomotionColliderResizerPreset
    {
        public LocomotionFlags WithFlags { get; [UsedImplicitly] set; }
        public LocomotionFlags WithoutFlags { get; [UsedImplicitly] set; }
        public bool Enabled { get; [UsedImplicitly] set; } = true;
        public Vector3 Center  { get; [UsedImplicitly] set; }
        public float Height  { get; [UsedImplicitly] set; }
        public float BlendTime  { get; [UsedImplicitly] set; }
    }
}