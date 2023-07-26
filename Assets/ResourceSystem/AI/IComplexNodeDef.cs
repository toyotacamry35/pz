using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public interface IComplexNodeDef
    {
        List<ResourceRef<BehaviourNodeDef>> SubNodes { get; }
    }
}
