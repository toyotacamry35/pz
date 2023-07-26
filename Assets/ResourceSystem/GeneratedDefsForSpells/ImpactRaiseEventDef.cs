using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;

namespace GeneratedDefsForSpells
{
    public class ImpactRaiseEventDef : SharedCode.Wizardry.SpellImpactDef
    {
        public ResourceRef<SharedCode.Wizardry.SpellEntityDef> From { get; set; }
        public float Radius { get; set; }
        public ResourceRef<AIEventDef> PathToEventStatisDataType { get; set; }
        public override bool UnityAuthorityServerImpact => false;
    }
}
