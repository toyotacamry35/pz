using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public class DoInSequenceDef : BehaviourNodeDef, IComplexNodeDef
    {
        public List<ResourceRef<BehaviourNodeDef>> Actions { get; set; } = new List<ResourceRef<BehaviourNodeDef>>();

        public List<ResourceRef<BehaviourNodeDef>> SubNodes => Actions;
    }
}
