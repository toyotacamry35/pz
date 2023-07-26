using SharedCode.Wizardry;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;

namespace Shared.ManualDefsForSpells
{
    public enum ComprasionType
    {
        More,
        Less,
        Equal
    }

    public class PredicateCompareStatDef : SpellPredicateDef
    {
        public ComprasionType Type { get; set; }
        public ResourceRef<CalcerDef<float>> Value { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<StatResource> Stat { get; set; }
    }
}
