using Assets.Src.ResourcesSystem.Base;
using System;

namespace Assets.ResourceSystem.Arithmetic.Templates.Predicates
{
    public class PredicateIgnoreGroupDef : BaseResource
    {
        public Type[] IgnorePredicatesOfType { get; set; }
    }
}