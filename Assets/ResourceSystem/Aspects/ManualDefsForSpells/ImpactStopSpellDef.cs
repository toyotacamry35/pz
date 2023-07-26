using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using GeneratedDefsForSpells;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactStopSpellDef : SpellImpactDef
    {
        public ResourceRef<SpellOrBuffDef> Spell { get; set; }
        public List<ResourceRef<SpellOrBuffDef>> Spells { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public FinishReasonType Reason { get; set; }
    }
}
