using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactDieDef : SpellImpactDef
    {
        public Assets.Src.ResourcesSystem.Base.ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
