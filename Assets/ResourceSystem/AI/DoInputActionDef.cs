using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using ColonyShared.SharedCode.InputActions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class DoInputActionDef : BehaviourNodeDef
    {
        public float BreakBeforeWindowDelayIn { get; set; } = 0.05f;
        public ResourceRef<CalcerDef<float>> TimeToHold { get; set; }
        public ResourceRef<InputActionTriggerDef> Trigger { get; set; }
        public ResourceRef<InputActionTriggerDef> BreakOnTriggerWindow { get; set; }
        public ResourceRef<TargetSelectorDef> Target { get; set; }
    }
}