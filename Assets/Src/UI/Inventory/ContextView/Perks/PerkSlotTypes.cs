using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using System;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Uins
{
    [CreateAssetMenu(menuName = "Uins/PerkTypes")]
    public class PerkSlotTypes : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private ItemTypeResourceRef[] _perkSlotTypes;

        [SerializeField, UsedImplicitly]
        private ItemTypeResourceRef _specialFactionPerkType;


        //=== Props ===========================================================

        public bool IsInited { get; private set; }

        public ItemTypeResource[] ItemTypes { get; private set; }

        public ItemTypeResource SpecialFactionPerkType { get; private set; }

        public ItemTypeResource FirstType => GetElementByIndex(0);

        public ItemTypeResource MiddleType => GetElementByIndex(1);

        public ItemTypeResource BestType => GetElementByIndex(2);


        //=== Public ==========================================================

        public void Init()
        {
            if (_perkSlotTypes.IsNullOrEmptyOrHasNullElements(nameof(_perkSlotTypes)) ||
                _specialFactionPerkType.AssertIfNull(nameof(_specialFactionPerkType)))
                return;

            try
            {
                ItemTypes = new ItemTypeResource[_perkSlotTypes.Length];
                for (int i = 0; i < _perkSlotTypes.Length; i++)
                    ItemTypes[i] = _perkSlotTypes[i].Target;

                SpecialFactionPerkType = _specialFactionPerkType.Target;
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message($"Init exception: {e}").Write();
                return;
            }

            if (!ItemTypes.IsNullOrEmptyOrHasNullElements(nameof(ItemTypes)) &&
                !SpecialFactionPerkType.AssertIfNull(nameof(SpecialFactionPerkType)))
                IsInited = true;
        }


        /// <summary>
        /// Индекс по типу перка/слота
        /// </summary>
        /// <returns> -1 undefined, 0 Normal, 1 Rare, 2 Epic, 3 Special (faction)</returns>
        public int GetTypeIndex(ItemTypeResource itemType, bool forNextType = false)
        {
            if (!IsInited)
                return -1;

            if (itemType == null)
                return forNextType ? 0 : -1;

            if (itemType == SpecialFactionPerkType)
                return 3;

            int res = -1;
            for (int i = 0; i < ItemTypes.Length; i++)
            {
                if (ItemTypes[i] == itemType)
                {
                    res = forNextType ? i + 1 : i;
                    break;
                }
            }

            if (res >= ItemTypes.Length)
                res = -1;
            return res;
        }

        public ItemTypeResource GetNextItemType(ItemTypeResource itemType)
        {
            int idx = GetTypeIndex(itemType, true);
            return idx < 0 ? null : ItemTypes[idx];
        }

        private ItemTypeResource GetElementByIndex(int index)
        {
            if (!IsInited)
                return null;

            if (index < 0 || index >= _perkSlotTypes.Length)
            {
                UI.Logger.IfError()?.Message($"Out of order: {nameof(index)}={index}").Write();
                return null;
            }

            return ItemTypes[index];
        }
    }
}