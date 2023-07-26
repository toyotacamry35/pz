using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Assets.Src.ManualDefsForSpells
{
    public class ImpactDeactivatePreDeathStateDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
