using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;

namespace GeneratedDefsForSpells
{
    public class ImpactRaiseTargetedEventDef : SharedCode.Wizardry.SpellImpactDef
    {
        public ResourceRef<SharedCode.Wizardry.SpellCasterDef> Caster { get; set; }
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> Target { get; set; }
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> From { get; set; }
        public ResourceRef<AIEventDef> PathToEventStatisDataType { get; set; }
        public float Radius { get; set; }
        public override bool UnityAuthorityServerImpact => false;
    }
}
