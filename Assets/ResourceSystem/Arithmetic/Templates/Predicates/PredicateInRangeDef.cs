using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateInRangeDef : PredicateDef
    {
        public ResourceRef<CalcerDef<float>> Min { get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> Max { get; [UsedImplicitly] set; }
        public ResourceRef<CalcerDef<float>> Value { get; [UsedImplicitly] set; }
    }
}