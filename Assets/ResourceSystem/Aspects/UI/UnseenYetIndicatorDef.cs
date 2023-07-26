using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace Assets.ColonyShared.SharedCode.Aspects.UI
{
    [Localized]
    public class UnseenYetIndicatorDef : BaseResource
    {
        public UnityRef<Sprite> Icon { get; set; }
        public LocalizedString Title { get; set; }
        public UnityRef<Sprite> ShortcutIcon { get; set; }
    }
}