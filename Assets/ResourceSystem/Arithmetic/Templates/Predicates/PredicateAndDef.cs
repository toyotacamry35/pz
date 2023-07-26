using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateAndDef : PredicateDef
    {
        public List<ResourceRef<PredicateDef>> Predicates { get; [UsedImplicitly] set; }
    }
}