using System.Collections.Generic;
using System.Linq;
using ColonyDI;
using Uins;
using UnityEngine;

public abstract class DependencyNode : BindingViewModel, IDependencyNode
{
    public IDependencyNode Parent { get; set; }

//    private bool _showDebugInfo = false;


    //=== Props ===============================================================

    [Dependency]
    protected WindowsManager WindowsManager { get; set; }


    //=== Public ==============================================================

    public virtual IEnumerable<IDependencyNode> Children => Enumerable.Empty<IDependencyNode>();

    /// <summary>
    /// Момент после назначения свойств с атрибутом 'Dependency'. Начинать активность с этого метода, вместо Awake()
    /// </summary>
    public virtual void AfterDependenciesInjected()
    {
    }

    /// <summary>
    /// Момент после назначения свойств с атрибутом 'Dependency' на всех провайдерах. 
    /// Если нужно обратиться к другому провайдеру в начале активности
    /// </summary>
    public virtual void AfterDependenciesInjectedOnAllProviders()
    {
    }

    //=== Private =============================================================

    private string GetDependencyProviderMembersDebugInfo()
    {
        return $"[{Time.frameCount}] " +
               $"{this.FieldsAndPropsToString($"'{transform.FullName()}'", true, true, typeof(IDependencyNode))}";
    }
}