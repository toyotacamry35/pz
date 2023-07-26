using System;
using JetBrains.Annotations;

namespace ColonyDI
{
    /// <summary>
    /// Свойство, помеченное данным атрибутом будет инициализировано в потомке DependencyNode к моменту AfterDependenciesInjected()
    /// на этапе загрузки сцен
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [MeansImplicitUse]
    class DependencyAttribute : Attribute
    {
    }
}