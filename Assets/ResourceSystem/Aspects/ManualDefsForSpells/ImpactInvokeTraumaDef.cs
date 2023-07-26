using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactInvokeTraumaDef : SpellImpactDef
    {
        public string TraumaType { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
