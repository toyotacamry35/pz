using Assets.ResourceSystem.Aspects.Mob;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.NetworkedMovement
{
    public class PawnDef : ComponentDef
    {
        public float MaxSpeedForward { get; set; } = 5;
        public float MaxSpeedBack { get; set; } = 5;
        public float MaxSpeedSide { get; set; } = 5;
        public float MaxRot { get; set; } = 360;
        public bool IsGrounded { get; set; } = true;
        public float RelevancyLevelPercent { get; set; } = 1f;
        public bool SyncRotationByNetworkTransform { get; set; } = true;
        public bool IsControlledByServer { get; set; } = false;
        public ResourceRef<DirectMotionProducerDef> DirectMotionProducer { get; set; }
    }
}
