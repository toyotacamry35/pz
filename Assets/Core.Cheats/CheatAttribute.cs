using System;

namespace Core.Cheats
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class CheatAttribute : Attribute
    {
    }
}