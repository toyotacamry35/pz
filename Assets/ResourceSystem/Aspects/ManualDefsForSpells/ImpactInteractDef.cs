using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactInteractDef : SpellImpactDef
    {
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public ResourceRef<SpellTargetDef> Target { get; set; }
    }
}