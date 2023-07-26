using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class UseStrategyDef : BehaviourNodeDef, IDecoratorDef
    {
        public ResourceRef<ConditionDef> GoalCondition { get; set; }
        public ResourceRef<StrategyDef> Strategy { get; set; }
        public Dictionary<string, TargetSelectorDef> ValidState { get; set; } = new Dictionary<string, TargetSelectorDef>();

        public ResourceRef<BehaviourNodeDef> SubNode => Strategy.Target.Plan;
    }
}
