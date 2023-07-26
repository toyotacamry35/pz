using System;
using System.Collections.Generic;

namespace Assets.Src.RubiconAI.BehaviourTree
{
    // Only def can't can't be unique id of a `BehavNode` ('cos same def could be used in diff.places)
    // Behaviour-tree object representation (from def) is building from root to leafs. First (root) `NodeToChild` == (null, def)
    internal struct NodeToChild : IEquatable<NodeToChild>
    {
        public BehaviourNode ParentNode;
        public BehaviourNodeDef ChildDef;

        public bool Equals(NodeToChild other)
        {
            return other.ParentNode == ParentNode && other.ChildDef == ChildDef;

        }

        public override int GetHashCode()
        {
            var hashCode = -1971491278;
            hashCode = hashCode * -1521134295 + EqualityComparer<BehaviourNode>.Default.GetHashCode(ParentNode);
            hashCode = hashCode * -1521134295 + EqualityComparer<BehaviourNodeDef>.Default.GetHashCode(ChildDef);
            return hashCode;
        }
    }
}
