using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.BehaviourTree;
using Assets.Src.RubiconAI.BehaviourTree.NodeTypes;

namespace Assets.ResourceSystem.AI
{
    public class HighFrequentlyUpdatedDef : BehaviourNodeDef, IDecoratorDef
    {
        public ResourceRef<BehaviourNodeDef> SubNode { get; set; }
    }
}
