using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateGreaterOrEqualsDef : PredicateDef
    {
        public ResourceRef<CalcerDef<float>> Lhs { get; set; }
        public ResourceRef<CalcerDef<float>> Rhs { get; set; }
    }
}