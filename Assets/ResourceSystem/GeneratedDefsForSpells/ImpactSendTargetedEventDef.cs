using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;

namespace GeneratedDefsForSpells
{
    public class ImpactSendTargetedEventDef : SharedCode.Wizardry.SpellImpactDef
    {
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> Caster { get; set; }
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> Target { get; set; }
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> EventTarget { get; set; }
        public ResourceRef<AIEventDef> PathToEventStatisDataType { get; set; }
        public override bool UnityAuthorityServerImpact => false;
    }
}
