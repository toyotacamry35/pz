using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Utils;

namespace Assets.Src.Aspects.Templates
{
    [DefToType("UnityEngine.CapsuleCollider")]
    public class CapsuleColliderDef : ComponentDef
    {
        [UsedImplicitly] public bool IsTrigger { get; set; }
        [UsedImplicitly] public Vector3 Center { get; set; }
        [UsedImplicitly] public float Radius { get; set; }
        [UsedImplicitly] public float Height { get; set; }
        [UsedImplicitly] public int Direction { get; set; } = 1;
    }
}