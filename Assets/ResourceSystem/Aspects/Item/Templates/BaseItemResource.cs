using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class BaseItemResource : SaveableBaseResource
    {
        public LocalizedString ItemNameLs { get; set; }

        /// <summary>
        /// Картинка предмета для слотов
        /// </summary>
        public UnityRef<Sprite> Icon { get; set; }

        /// <summary>
        /// Картинка предмета для тултипа
        /// </summary>
        public UnityRef<Sprite> BigIcon { get; set; }

        
        public LocalizedString DescriptionLs { get; set; }

        public float Weight { get; set; }

        public int MaxStack { get; set; }

        /// <summary>
        /// Группа доступности слотов куклы
        /// </summary>
        public ResourceRef<ItemTypeResource> ItemType { get; set; }
    }
}
