
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Aspects.Templates
{
    [DefToType("UnityEngine.Rigidbody")]
    public class RigidbodyDef : ComponentDef
    {
        public float Mass { get; set; }
        public float Drag { get; set; }
        public float AngularDrag { get; set; }
        public bool UseGravity { get; set; }
        public bool IsKinematic { get; set; }
    }
}
