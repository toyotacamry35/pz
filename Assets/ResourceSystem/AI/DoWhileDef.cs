using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class DoWhileDef : BehaviourNodeDef, IDecoratorDef
    {
        public ResourceRef<ConditionDef> Condition { get; set; }
        public ResourceRef<BehaviourNodeDef> Action { get; set; }

        public ResourceRef<BehaviourNodeDef> SubNode => Action;
    }
}
