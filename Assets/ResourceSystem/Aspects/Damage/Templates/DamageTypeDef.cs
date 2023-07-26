using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    [Localized]
    public class DamageTypeDef : BaseResource, IResourceWithSoundSwitch
    {
        public string DisplayName { get; set; } //DEBUG
        public LocalizedString DisplayNameLs { get; set; }

        public UnityRef<Sprite> Sprite { get; set; }
        public string SoundSwitch { get; set; }
    }
}