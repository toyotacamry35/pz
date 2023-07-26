using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using ResourcesSystem.Loader;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    [KnownToGameResources]
    public struct WeightedAction
    {
        public ResourceRef<BehaviourNodeDef> Action { get; set; }
        public ResourceRef<MetricDef> Weight { get; set; }
    }
}