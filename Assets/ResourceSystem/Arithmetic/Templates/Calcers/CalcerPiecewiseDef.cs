using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    public struct Range
    {
        [UsedImplicitly] public ResourceRef<PredicateDef> Condition;
        [UsedImplicitly] public ResourceRef<CalcerDef<float>> Value;
    }

    public class CalcerPiecewiseDef : CalcerDef<float>
    {
        public Range[] Ranges { get; [UsedImplicitly] set; } = { };

        public ResourceRef<CalcerDef<float>> Else { get; [UsedImplicitly] set; }
    }
}
