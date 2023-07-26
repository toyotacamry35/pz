using Assets.Src.ResourcesSystem.Base;

namespace Assets.ResourceSystem.Aspects.Effects
{
    public class AnimationEventInfoDef : BaseResource
    {
        public ResourceRef<BaseResource> Marker { get; set; }
        public float DeltaTime { get; set; }

        public bool MarkerIsParent { get; set; }
        public bool PositionFromMarker { get; set; }

        public bool RotationForwardFromMarker { get; set; }
        public bool RotationAllFromMarker { get; set; }

        public bool NotSpawnInIdle { get; set; }
    }
}
