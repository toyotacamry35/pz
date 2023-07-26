using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Regions;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectStaticAOEDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<GeoRegionDef> RegionDef { get; set; }
    }
}
