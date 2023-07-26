using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateOrDef : PredicateDef
    {
        public List<ResourceRef<PredicateDef>> Predicates { get; set; }
    }
}