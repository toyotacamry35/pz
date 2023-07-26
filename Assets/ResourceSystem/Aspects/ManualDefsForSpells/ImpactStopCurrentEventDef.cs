using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using GeneratedDefsForSpells;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class EffectCastImactOnEndDef : SpellEffectDef
    {
        public ResourceRef<SpellImpactDef> Impact { get; set; }
    }
    public class ImpactStopCurrentEventDef : SpellImpactDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; } = new SpellTargetDef();
    }
}
