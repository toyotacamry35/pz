using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Misc;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class ImpactAttackObjectDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Attacker { get; set; }
        public ResourceRef<SpellEntityDef> Victim { get; set; }
        public ResourceRef<AttackDef> Attack { get; set; }
    }
}