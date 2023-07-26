using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace ColonyShared.ManualDefsForSpells
{
    public class ImpactAddPerkDef : SpellImpactDef
    {
        // Will be used 1 of them (priority: 1 - SubTable, 2 - ItemPack)
        public ResourceRef<BaseItemResource> Perk { get; set; }
        public ResourceRef<LootTableBaseDef> LootTable { get; set; }

        public ResourceRef<SpellEntityDef> Target { get; set; }

        public ItemPackOrSubTable IsItemPackOrSubTable => (LootTable.Target != null)
           ? ItemPackOrSubTable.SubTable
           : ItemPackOrSubTable.ItemPack;
    }
}
