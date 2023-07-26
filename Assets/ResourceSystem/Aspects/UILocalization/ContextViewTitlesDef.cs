using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace L10n
{
    [Localized]
    public class ContextViewTitlesDef : BaseResource
    {
        public SpriteAndLocalizedString ItemSlot { get; set; }
        public SpriteAndLocalizedString Handcraft { get; set; }
        public SpriteAndLocalizedString BenchDefault { get; set; }
        public SpriteAndLocalizedString MachineDefault { get; set; }

        [Localized]
        public struct SpriteAndLocalizedString
        {
            public LocalizedString Ls { get; set; }
            public UnityRef<Sprite> Sprite { get; set; }
        }
    }
}