using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class DoForTimeDef : BehaviourNodeDef, IDecoratorDef
    {
        public ResourceRef<MetricDef> Time { get; set; }
        public ResourceRef<BehaviourNodeDef> Action { get; set; }
        public bool DoUntilEnd { get; set; } = true;
        public bool FailOnTimeout { get; set; } = false;
        public bool TryAgain { get; set; } = false;
        public ResourceRef<BehaviourNodeDef> SubNode => Action;


    }
}
