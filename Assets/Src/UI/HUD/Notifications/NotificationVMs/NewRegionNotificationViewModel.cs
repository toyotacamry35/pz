using L10n;
using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class NewRegionNotificationViewModel : NotificationViewModel
    {
        ReactiveProperty<LocalizedString> _titleRp = new ReactiveProperty<LocalizedString>();
        ReactiveProperty<int> _exploringRatioRp = new ReactiveProperty<int>();


        //=== Props ==============================================================

        [Binding]
        public LocalizedString Title { get; private set; }

        [Binding]
        public int ExploringRatio { get; private set; }


        //=== Unity ==============================================================

        private void Awake()
        {
            Bind(_titleRp, () => Title);
            Bind(_exploringRatioRp, () => ExploringRatio);
        }


        //=== Public ==============================================================

        public override void Show(HudNotificationInfo info)
        {
            base.Show(info);
            var newRegionNotificationInfo = info as NewRegionNotificationInfo;
            if (newRegionNotificationInfo.AssertIfNull(nameof(newRegionNotificationInfo)))
                return;

            _titleRp.Value = newRegionNotificationInfo.RegionName;
            _exploringRatioRp.Value = newRegionNotificationInfo.ExploringRatio;
            AkSoundEngine.PostEvent(newRegionNotificationInfo.IsFirstTime ? "UI_Notif_NewRegion" : "UI_Notif_SideMessage", transform.root.gameObject);
        }
    }
}