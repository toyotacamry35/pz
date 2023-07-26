using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;
using System.Collections.Generic;
namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateOfLegionDef : PredicateDef
    {
        public ResourceRef<LegionDef> OfLegion { get; set; }
    }
}