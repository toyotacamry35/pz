using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using System.Collections.Generic;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactEquipItemsDef : SpellImpactDef
    {
        public List<ResourceRef<BaseItemResource>> Items { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public int Count { get; set; } = 1;
    }
}