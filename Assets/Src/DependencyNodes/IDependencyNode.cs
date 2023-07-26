using System.Collections.Generic;

namespace ColonyDI
{
    public interface IDependencyNode
    {
        IDependencyNode Parent { get; set; }
        IEnumerable<IDependencyNode> Children { get; }

        void AfterDependenciesInjected();
        void AfterDependenciesInjectedOnAllProviders(); // Remove me
    }
}
