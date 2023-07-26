using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class RememberValueDef : BehaviourNodeDef
    {
        public enum ChangeType
        {
            Add,
            Set,
            Remove
        }
        public ChangeType Change { get; set; } = ChangeType.Set;
        public ResourceRef<MetricDef> Time { get; set; }
        public ResourceRef<MetricDef> Flat { get; set; }
        public ResourceRef<StatModifierDef> ModDef { get; set; }
        public ResourceRef<MemorizedStatDef> StatDef { get; set; }
        public ResourceRef<TargetSelectorDef> Target { get; set; }
        public ResourceRef<TargetSelectorDef> Memory { get; set; }
    }
}
