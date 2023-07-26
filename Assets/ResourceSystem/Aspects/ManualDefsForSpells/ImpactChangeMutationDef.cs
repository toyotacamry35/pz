using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactChangeMutationDef : SpellImpactDef
    {
        public ResourceRef<SpellTargetDef> Target { get; set; }
        public float DeltaValue { get; set; }
        public ResourceRef<MutatingFactionDef> Faction { get; set; }
        public float CoolDownTime { get; set; } = 0;
    }
}
