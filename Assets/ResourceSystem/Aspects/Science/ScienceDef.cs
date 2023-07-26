using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Science
{
    [Localized]
    public class ScienceDef : BaseResource
    {
        public LocalizedString NameLs { get; set; }
        public LocalizedString DescriptionLs { get; set; }

        public UnityRef<Sprite> Sprite { get; set; }
        public UnityRef<Sprite> MiniSprite { get; set; }
        public UnityRef<Sprite> InactiveMiniSprite { get; set; }
        public int SortingOrder { get; set; }
    }
}
