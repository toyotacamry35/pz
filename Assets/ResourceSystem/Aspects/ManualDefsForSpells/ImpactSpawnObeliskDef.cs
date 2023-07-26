using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Misc;
using ResourceSystem.Aspects.Templates;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class ImpactSpawnObeliskDef : SpellImpactDef
    {
        public ResourceRef<ObeliskDef> Obelisk { get; set; }
        public ResourceRef<SpellEntityDef> On { get; set; } = new SpellCasterDef();
    }
}