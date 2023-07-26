using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using System.Collections.Generic;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactAddItemsDef : SpellImpactDef
    {
        public ContainerType Container { get; set; } = ContainerType.Inventory;
        public int Slot { get; set; } = -1;
        public int Count { get; set; } = 1;
        public ItemsBatchType ItemsBatchType { get; set; } = ItemsBatchType.All;
        public List<ResourceRef<BaseItemResource>> Items { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<CalcerDef> AmountOfResourcesMultiplier { get; set; }
    }

    public enum ItemsBatchType
    {
        All, 
        First,
        OneOfItem
    }
}