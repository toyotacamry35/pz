using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using SharedCode.Entities;
using System.Collections.Generic;

namespace Assets.ColonyShared.SharedCode.Arithmetic
{
    public class ProbabilisticLootItemData
    {
        // Will be used 1 of them (priority: 1 - SubTable, 2 - ItemPack)
        public IEnumerable<ProbabilisticLootItemData> SubTable { get; }
        public ItemResourcePack ItemPack { get; set; }
        public float Weight { get; set; }
        public bool Hidden { get; }
        public bool Forced => Weight < 0f;
        public ItemPackOrSubTable IsItemPackOrSubTable => (SubTable != null)
            ? ItemPackOrSubTable.SubTable
            : ItemPackOrSubTable.ItemPack;

        public ProbabilisticLootItemData(IEnumerable<ProbabilisticLootItemData> subTable, float weight, bool hidden = false)
        {
            Weight = weight;
            SubTable = subTable;
            Hidden = hidden;
        }

        public ProbabilisticLootItemData(ItemResourcePack itemPack, float weight, bool hidden = false)
        {
            Weight = weight;
            ItemPack = itemPack;
            Hidden = hidden;
        }
    }
}
