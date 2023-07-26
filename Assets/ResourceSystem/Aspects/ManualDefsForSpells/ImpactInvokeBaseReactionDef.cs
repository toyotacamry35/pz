using Assets.Src.ResourcesSystem.Base;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class ImpactInvokeBaseReactionDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Caster { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public Vector3 Direction { get; set; }
        public override bool UnityAuthorityServerImpact => true;
    }
}
