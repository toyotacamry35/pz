using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactActivateBakenDef : SpellImpactDef
    {
        public bool CommonBaken { get; set; } = false;
        public ResourceRef<SpellEntityDef> Source { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
