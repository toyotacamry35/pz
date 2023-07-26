using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class DoWithCooldownDef : BehaviourNodeDef, IDecoratorDef
    {
        public string CooldownName { get; set; }
        public ResourceRef<MetricDef> CooldownOnSuccess { get; set; }
        public ResourceRef<MetricDef> CooldownOnFail { get; set; }
        public ResourceRef<BehaviourNodeDef> Action { get; set; }
        public bool FromStart { get; set; } = false;
        public ResourceRef<BehaviourNodeDef> SubNode => Action;
    }
}
