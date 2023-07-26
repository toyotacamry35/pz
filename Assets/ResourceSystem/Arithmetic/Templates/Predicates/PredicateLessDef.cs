using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateLessDef : PredicateDef
    {
        public ResourceRef<CalcerDef<float>> Lhs { get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> Rhs { get; [UsedImplicitly] set; }
    }
}