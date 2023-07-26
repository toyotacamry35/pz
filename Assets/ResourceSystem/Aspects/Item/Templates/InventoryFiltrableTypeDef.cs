using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class InventoryFiltrableTypeDef : BaseResource
    {
        public LocalizedString DisplayNameLs { get; set; }

        public UnityRef<Sprite> Icon { get; set; }
        public UnityRef<Sprite> InventoryBgSign { get; set; }
    }
}