using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactSynchronizeMutationDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
