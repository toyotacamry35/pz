using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactDamageObjectDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Caster { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<CalcerDef<float>> Damage { get; set; }
    }
}