using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;

namespace GeneratedDefsForSpells
{
    public class ImpactSendEventDef : SharedCode.Wizardry.SpellImpactDef
    {
        public ResourceRef<SharedCode.Wizardry.SpellCasterDef> Caster { get; set; }
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> EventTarget { get; set; }
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> From { get; set; }
        public string PathToNoiseCalcer { get; set; }
        public ResourceRef<AIEventDef> PathToEventStatisDataType { get; set; }
        public float Radius { get; set; }
        public override bool UnityAuthorityServerImpact => false;
    }
}
