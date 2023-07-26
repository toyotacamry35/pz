using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using System.Collections.Generic;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactUseSlotsDef : SpellImpactDef
    {
        public List<ResourceRef<SlotDef>> Slots { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
    public class PredicateCanUseSlotsDef : SpellPredicateDef
    {
        public List<ResourceRef<SlotDef>> Slots { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}