using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactDestroyObjectDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}