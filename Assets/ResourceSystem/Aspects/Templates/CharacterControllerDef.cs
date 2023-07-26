using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Utils;

namespace Assets.Src.Aspects.Templates
{
    [DefToType("UnityEngine.CharacterController")]
    public class CharacterControllerDef : ComponentDef
    {
        [UsedImplicitly] public float SlopeLimit { get; set; }
        [UsedImplicitly] public float StepOffset { get; set; }
        [UsedImplicitly] public float SkinWidth { get; set; }
        [UsedImplicitly] public float MinMoveDistance { get; set; }
        [UsedImplicitly] public Vector3 Center { get; set; }
        [UsedImplicitly] public float Radius { get; set; }
        [UsedImplicitly] public float Height { get; set; }
    }
}