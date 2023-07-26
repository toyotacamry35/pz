using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactLeftAreaDef : SpellImpactDef
    {
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public ResourceRef<SpellTargetDef> Target { get; set; }
        public override bool UnityAuthorityServerImpact => true;
    }
}
