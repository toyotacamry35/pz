using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactUseGatewayDef : SpellImpactDef
    {
        public ResourceRef<MapDef> Map { get; set; }
        public ResourceRef<SpellEntityDef> Source { get; set; } = new SpellCasterDef();
        public ResourceRef<SpellEntityDef> Target { get; set; } = new SpellTargetDef();
    }
    public class PredicateIsMapDef : SpellPredicateDef
    {
        public ResourceRef<MapDef> Map { get; set; }
    }
}
