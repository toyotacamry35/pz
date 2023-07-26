using System;

namespace L10n
{
    /// <summary>
    /// Класс содержащий локализуемые данные
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LocalizedAttribute : Attribute
    {
    }
}