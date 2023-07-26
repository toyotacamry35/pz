using System;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Utils
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    [MeansImplicitUse]
    public class VerifyAfterCompilationAttribute : Attribute
    {
    }
}
