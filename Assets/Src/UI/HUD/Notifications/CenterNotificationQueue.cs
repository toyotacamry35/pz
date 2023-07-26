using System.Linq;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public class CenterNotificationQueue : BaseNotificationQueue
    {
        [SerializeField, UsedImplicitly]
        private SideNotificationQueue _sideNotificationQueue;

        public static CenterNotificationQueue Instance { get; private set; }

        private readonly string _usageHudTag = nameof(CenterNotificationQueue);
        private bool _hasUsageRequest;


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            Instance = SingletonOps.TrySetInstance(this, Instance);

            _sideNotificationQueue.AssertIfNull(nameof(_sideNotificationQueue));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        public override void Init(IPawnSource pawnSource, WindowsManager windowsManager)
        {
            base.Init(pawnSource, windowsManager);

            IsShownSomeNotification.Action(
                D,
                b =>
                {
                    if (b)
                    {
                        if (!_hasUsageRequest)
                        {
                            _hasUsageRequest = true;
                            windowsManager.HudCenterPartUsageNotifier.UsageRequest(_usageHudTag, this);
                            windowsManager.HudLeftPartUsageNotifier.UsageRequest(_usageHudTag, this);
                            windowsManager.HudRightPartUsageNotifier.UsageRequest(_usageHudTag, this);
                        }
                    }
                    else
                    {
                        if (_hasUsageRequest)
                        {
                            _hasUsageRequest = false;
                            windowsManager.HudCenterPartUsageNotifier.UsageRevoke(_usageHudTag, this);
                            windowsManager.HudLeftPartUsageNotifier.UsageRevoke(_usageHudTag, this);
                            windowsManager.HudRightPartUsageNotifier.UsageRevoke(_usageHudTag, this);
                        }
                    }
                });
            _sideNotificationQueue.Init(pawnSource, windowsManager);
        }

        public override void SendNotification(HudNotificationInfo info)
        {
            //UI.CallerLog($">>>>> {info}"); //DEBUG
            if (info is AchievedPointsNotificationInfo achievedPointsNotificationInfo && IsMerged(achievedPointsNotificationInfo))
                return;

            if (info is NewRegionNotificationInfo newRegionNotificationInfo && !newRegionNotificationInfo.IsFirstTime)
            {
                _sideNotificationQueue.SendNotification(info); //передаем в очередь боковых уведомлений
                return;
            }

            NotificationItems.Enqueue(info);
        }


        //=== Private =========================================================

        private bool IsMerged(AchievedPointsNotificationInfo achievedPointsNotificationInfo)
        {
            var existsSameItem = NotificationItems.FirstOrDefault(e => e is AchievedPointsNotificationInfo);
            if (existsSameItem == null)
                return false;

            return ((AchievedPointsNotificationInfo) existsSameItem).IsMerged(achievedPointsNotificationInfo);
        }
    }
}