using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Building
{
    [Localized]
    public class BuildRecipeGroupDef : BaseResource
    {
        public LocalizedString NameLs { get; set; }

        public int OrderIndex { get; set; } //порядок сортировки группы

        public UnityRef<Sprite> Icon { get; set; }
    }
}