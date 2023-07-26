using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using ColonyShared.SharedCode.InputActions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class InputActionsDef : BehaviourNodeDef
    {
        public ResourceRef<InputActionTriggerDef>[] InputActions { get; set; }
        public float DurationSeconds { get; set; } = 1f;
    }
}
