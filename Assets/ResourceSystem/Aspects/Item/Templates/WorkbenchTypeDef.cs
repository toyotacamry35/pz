using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class WorkbenchTypeDef : SaveableBaseResource
    {
        public UnityRef<Sprite> Icon { get; set; }
        public LocalizedString DisplayNameLs { get; set; }
        public  int SortingIndex { get; set; }
    }
}