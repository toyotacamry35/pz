using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace Assets.ResourceSystem.Account
{
    [Localized]
    public class AccountTitleDef : BaseResource
    {
        public LocalizedString Name { get; set; }
        // Small icon variant
        public UnityRef<Sprite> StripeIcon { get; set; }
        // Big icon variant
        public UnityRef<Sprite> StripeIconBig { get; set; }
    }
}