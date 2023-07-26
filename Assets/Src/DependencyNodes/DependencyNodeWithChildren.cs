using System.Collections.Generic;
using ColonyDI;
using JetBrains.Annotations;
using Uins;
using UnityEngine;

public abstract class DependencyNodeWithChildren : DependencyNode
{
    [UsedImplicitly, SerializeField]
    protected DependencyNode[] _childNodes;

    public override IEnumerable<IDependencyNode> Children => _childNodes;
}