using Assets.Src.Lib.Cheats;
using Assets.Src.ResourceSystem;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class NotificationQueueTest : BindingViewModel
    {
        private const int PerksLength = 4;
        private const string IsVisibleKey = "HudNotificationQueueTest_IsVisible";

        [SerializeField, UsedImplicitly]
        private CenterNotificationQueue _centerNotificationQueue;

        [SerializeField, UsedImplicitly]
        private TechPointDefRef _techPointDefRef1;

        [SerializeField, UsedImplicitly]
        private ScienceDefRef _scienceDefRef1;

        [SerializeField, UsedImplicitly]
        private TechPointDefRef _techPointDefRef2;

        [SerializeField, UsedImplicitly]
        private ScienceDefRef _scienceDefRef2;

        [SerializeField, UsedImplicitly]
        private ItemResourceRef[] _perks;

        private AdminNotificationInfo[] _adminInfos;
        private AchievedPointsNotificationInfo[] _achievedPointsInfos;
        private NewRegionNotificationInfo[] _newRegionInfos;
        private QuestNotificationInfo[] _questInfos;
        private PerkNotificationInfo[] _perkInfos;


        //=== Props ===========================================================

        public static NotificationQueueTest Instance { get; private set; }

        [Binding]
        public bool IsShowTime { get; set; }

        public LocalizedString RegionName1 { get; set; }

        public LocalizedString RegionName2 { get; set; }

        public LocalizedString QuestName { get; set; }

        [Binding]
        public bool IsVisible
        {
            get => UniquePlayerPrefs.GetBool(IsVisibleKey, false);
            set
            {
                if (value != IsVisible)
                {
                    UniquePlayerPrefs.SetBool(IsVisibleKey, value);
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        private void Start()
        {
            _centerNotificationQueue.AssertIfNull(nameof(_centerNotificationQueue));
            _techPointDefRef1.Target.AssertIfNull(nameof(_techPointDefRef1));
            _techPointDefRef2.Target.AssertIfNull(nameof(_techPointDefRef2));
            _scienceDefRef1.Target.AssertIfNull(nameof(_scienceDefRef1));
            _scienceDefRef2.Target.AssertIfNull(nameof(_scienceDefRef2));
            _perks.IsNullOrEmptyOrHasNullElements(nameof(_perks));

            if (_perks.Length != PerksLength)
                UI.Logger.IfError()?.Message($"Unexpected {nameof(_perks)} length: {_perks.Length}, must be {PerksLength}").Write();

            _adminInfos = new[]
            {
                new AdminNotificationInfo("This is title", "This is main text"),
                new AdminNotificationInfo("This is title 2", "This is another main text")
            };

            var techPointCounts1 = new[] {new TechPointCount() {TechPoint = _techPointDefRef1.Target, Count = 1}};
            var techPointCounts2 = new[] {new TechPointCount() {TechPoint = _techPointDefRef2.Target, Count = 2}};
            var scienceCounts1 = new[] {new ScienceCount() {Science = _scienceDefRef1.Target, Count = 2}};
            var scienceCounts2 = new[] {new ScienceCount() {Science = _scienceDefRef2.Target, Count = 1}};
            _achievedPointsInfos = new[]
            {
                new AchievedPointsNotificationInfo(techPointCounts1),
                new AchievedPointsNotificationInfo(scienceCounts1),
                new AchievedPointsNotificationInfo(techPointCounts2, scienceCounts2),
                new AchievedPointsNotificationInfo(techPointCounts1, scienceCounts1)
            };

            _newRegionInfos = new[]
            {
                new NewRegionNotificationInfo(RegionName1, false), //TODOM false
                new NewRegionNotificationInfo(RegionName2, true)
            };

            _questInfos = new[]
            {
                new QuestNotificationInfo(QuestName, true),
                new QuestNotificationInfo(QuestName, false)
            };

            _perkInfos = new[]
            {
                new PerkNotificationInfo(_perks[0].Target),
                new PerkNotificationInfo(_perks[1].Target),
                new PerkNotificationInfo(_perks[2].Target),
                new PerkNotificationInfo(_perks[3].Target)
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        [Cheat]
        public static void HudNotificationsTestButtons(bool isOn = true)
        {
            Instance.IsVisible = isOn;
        }

        public void Init(WindowsManager windowsManager)
        {
            Bind(windowsManager.HudCenterPartUsageNotifier.InUsage, () => IsShowTime);
        }

        [UsedImplicitly]
        public void OnAdmin(bool isOn)
        {
            CenterNotificationQueue.Instance.SendNotification(isOn ? _adminInfos[0] : _adminInfos[1]);
        }

        [UsedImplicitly]
        public void OnAchievedPointsSingle(bool isOn)
        {
            CenterNotificationQueue.Instance.SendNotification(isOn ? _achievedPointsInfos[0] : _achievedPointsInfos[1]);
        }

        [UsedImplicitly]
        public void OnAchievedPointsMultiple(bool isOn)
        {
            CenterNotificationQueue.Instance.SendNotification(isOn ? _achievedPointsInfos[2] : _achievedPointsInfos[3]);
        }

        [UsedImplicitly]
        public void OnNewRegion(bool isOn)
        {
            CenterNotificationQueue.Instance.SendNotification(isOn ? _newRegionInfos[0] : _newRegionInfos[1]);
        }

        [UsedImplicitly]
        public void OnPerks1(bool isOn)
        {
            CenterNotificationQueue.Instance.SendNotification(isOn ? _perkInfos[0] : _perkInfos[1]);
        }

        [UsedImplicitly]
        public void OnPerks2(bool isOn)
        {
            CenterNotificationQueue.Instance.SendNotification(isOn ? _perkInfos[2] : _perkInfos[3]);
        }

        [UsedImplicitly]
        public void OnQuest(bool isOn)
        {
            CenterNotificationQueue.Instance.SendNotification(isOn ? _questInfos[0] : _questInfos[1]);
        }
    }
}