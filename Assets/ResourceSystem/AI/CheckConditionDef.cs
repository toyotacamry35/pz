using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class CheckConditionDef : BehaviourNodeDef
    {
        public ResourceRef<ConditionDef> Condition { get; set; }
    }
}
