using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactSetInteractiveActiveDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Caster { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public bool Enable { get; set; } = false;
    }
}