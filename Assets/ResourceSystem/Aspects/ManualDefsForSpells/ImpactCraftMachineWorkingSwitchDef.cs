using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactCraftMachineWorkingSwitchDef : SpellImpactDef
    {
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public ResourceRef<SpellTargetDef> Target { get; set; }
    }
}
