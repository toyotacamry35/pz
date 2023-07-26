using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers.Cluster;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.Cluster;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;

namespace Assets.ColonyShared.SharedCode.Aspects.WorldObjects
{

    public abstract class LootTableBaseDef : BaseResource
    {
        public LootTablePredicateDef DefaultPredicate { get; set; }
        public ResourceRef<CalcerDef<float>> AmountOfResourcesMultiplier { get; set; } = null;
    }

    public class LootTableDef : LootTableBaseDef
    {
        public List<LootItemData> LootTable { get; set; }
    }

    // Inheritance is needed to polymorphism in json
    public class LootTableByDamageTypesDef : LootTableBaseDef
    {
        //Can't: public Dictionary<ResourceRef<DamageType>, ResourceRef<LootTableDef>> LootTablesByDamageTypes;
        public List<LootTableAndDamageType> TablesByDamageTypes { get; set; }
    }

    // --- Util types: -----------------------------------------------------------

    public struct LootTableAndDamageType
    {
        // Means `LootTableTable` is relevant for *ANY* of `DamageTypes`
        public List<ResourceRef<DamageTypeDef>> DamageTypes;
        public ResourceRef<LootTableBaseDef> LootTableTable;

        public bool IsDefault => Equals(default(LootTableAndDamageType));
    }

    public struct LootItemData
    {
        // Will be used 1 of them (priority: 1 - SubTable, 2 - ItemPack)
        public ItemResourceRefPack ItemResRefPack;
        public ResourceRef<LootTableBaseDef> SubTable;
        
        public ResourceRef<LootTablePredicateDef> Predicate;
        public ResourceRef<LootItemChanceWeightCalcerDef> WeightCalcer;
        public bool Hidden;

        public ItemPackOrSubTable IsItemPackOrSubTable => (SubTable.Target != null) 
            ? ItemPackOrSubTable.SubTable 
            : ItemPackOrSubTable.ItemPack;
    }

    public enum ItemPackOrSubTable : byte
    {
        ItemPack,
        SubTable
    }

    public struct ItemResourceRefPack
    {
        public ResourceRef<BaseItemResource> ItemResource { get; set; }
        public uint Count { get; set; }

        public static explicit operator ItemResourcePack(ItemResourceRefPack refPack)
        {
            return new ItemResourcePack(refPack.ItemResource.Target, refPack.Count);
        }

        public bool IsEqualToItemResPack(ItemResourcePack irp)
        {
            if (irp == null)
                return IsDefault;

            return irp.ItemResource == ItemResource.Target
                && irp.Count == Count;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemResourceRefPack))
                return false;
            var irrp = (ItemResourceRefPack)obj;
            return irrp.ItemResource == ItemResource && irrp.Count == Count;
        }

        //(? to Misha): "c# non readonly property referenced in GetHashCode"
        public override int GetHashCode()
        {
            var hashCode = -1492983518;
            hashCode = hashCode * -1521134295 + EqualityComparer<ResourceRef<BaseItemResource>>.Default.GetHashCode(ItemResource);
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ItemResourceRefPack a, ItemResourceRefPack b) => a.Equals(b);
        public static bool operator !=(ItemResourceRefPack a, ItemResourceRefPack b) => !(a == b);

        public bool IsDefault => ItemResource.Target == null && Count == 0;
    }

}

