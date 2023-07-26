using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateEqualsDef<T> : PredicateDef
    {
        public ResourceRef<CalcerDef<T>> Lhs { get; set; }
        public ResourceRef<CalcerDef<T>> Rhs { get; set; }
    }
}