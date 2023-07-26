using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;

namespace Assets.ColonyShared.SharedCode.Aspects.Item
{
    public static class ItemHelper
    {
        public static bool IsAParentOf([NotNull] this ItemTypeResource item, [NotNull] ItemTypeResource child)
        {
            if (child == item)
                return true;

            return child.Parent != null && item.IsAParentOf(child.Parent);
        }

        public static bool IsAChildOf([NotNull] this ItemTypeResource item, [NotNull] ItemTypeResource parent)
        {
            return IsAParentOf(parent, item);
        }

        public static bool Accepts([NotNull] this SlotDef slot, ItemTypeResource group)
        {
            return slot.AcceptsItems.Any(grp => grp.Target.IsAParentOf(group));
        }

        public static bool Accepts([NotNull] BaseItemResource item, ItemTypeResource group)
        {
            return IsAParentOf(group, item.ItemType);
        }

        //#TODO(2018.03.28): tmp solution ('cos changing Doll.Items from <int, ..> to <ResourceIDFull, ..> is a long way & we'll do it later.
        // Remove this method, when shifting 'll be done.
        public static int TmpPlug_GetIndexBySlotResourceIdFull(ResourceIDFull slotResId)
        {
            var slotDef = GameResourcesHolder.Instance.LoadResource<SlotDef>(slotResId);
            return slotDef.SlotId;
        }

        public static SlotDef GetSlotResourceId(ResourceIDFull slotResId)
        {
            return GameResourcesHolder.Instance.LoadResource<SlotDef>(slotResId);
        }

        public static IEnumerable<SlotDef> GetSlotsByItemTypes(SlotsListDef slots, IEnumerable<ItemTypeResource> itemTypes)
        {
            return slots?.Slots.Select(x => x.Target).Where(x => x != null && itemTypes.Any(y => x.Accepts(y))) ?? Enumerable.Empty<SlotDef>();
        }

#region InventoryFiltrableTypeDef

        static InventoryFiltrableTypeDef _armor;
        public static InventoryFiltrableTypeDef Armor
        {
            get
            {
                if (_armor == null)
                {
                    _armor = GameResourcesHolder.Instance.LoadResource<InventoryFiltrableTypeDef>("/Inventory/InventoryFiltrableType/Armor");
                }
                return _armor;
            }
        }

        static InventoryFiltrableTypeDef _building;
        public static InventoryFiltrableTypeDef Building
        {
            get
            {
                if (_building == null)
                {
                    _building = GameResourcesHolder.Instance.LoadResource<InventoryFiltrableTypeDef>("/Inventory/InventoryFiltrableType/Building");
                }
                return _building;
            }
        }

        static InventoryFiltrableTypeDef _food;
        public static InventoryFiltrableTypeDef Food
        {
            get
            {
                if (_food == null)
                {
                    _food = GameResourcesHolder.Instance.LoadResource<InventoryFiltrableTypeDef>("/Inventory/InventoryFiltrableType/Food");
                }
                return _food;
            }
        }

        static InventoryFiltrableTypeDef _medicine;
        public static InventoryFiltrableTypeDef Medicine
        {
            get
            {
                if (_medicine == null)
                {
                    _medicine = GameResourcesHolder.Instance.LoadResource<InventoryFiltrableTypeDef>("/Inventory/InventoryFiltrableType/Medicine");
                }
                return _medicine;
            }
        }

        static InventoryFiltrableTypeDef _weapon;
        public static InventoryFiltrableTypeDef Weapon
        {
            get
            {
                if (_weapon == null)
                {
                    _weapon = GameResourcesHolder.Instance.LoadResource<InventoryFiltrableTypeDef>("/Inventory/InventoryFiltrableType/Weapon");
                }
                return _weapon;
            }
        }

        static InventoryFiltrableTypeDef _component;
        public static InventoryFiltrableTypeDef Component
        {
            get
            {
                if (_component == null)
                {
                    _component = GameResourcesHolder.Instance.LoadResource<InventoryFiltrableTypeDef>("/Inventory/InventoryFiltrableType/Component");
                }
                return _component;
            }
        }

#endregion
    }
}
