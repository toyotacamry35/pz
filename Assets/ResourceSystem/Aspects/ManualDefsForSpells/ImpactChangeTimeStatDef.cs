using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactChangeTimeStatDef : SpellImpactDef
    {
        public ResourceRef<StatResource> StatName { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<CalcerDef<float>> Calcer;

        public float Value { get; set; }
    }
}
