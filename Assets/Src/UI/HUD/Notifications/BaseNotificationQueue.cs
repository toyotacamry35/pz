using System;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public abstract class BaseNotificationQueue : HasDisposablesMonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        protected NotificationViewModel[] Prefabs;

        [SerializeField, UsedImplicitly]
        protected Transform NotificationsTransform;

        [SerializeField, UsedImplicitly]
        protected float TimerPeriod = 0.3f;

        /// <summary>
        /// прячутся боковые элементы HUD
        /// </summary>
        [SerializeField, UsedImplicitly]
        protected float BeforeShowDelay = 0.3f;

        /// <summary>
        /// Сообщение появляется
        /// </summary>
        [SerializeField, UsedImplicitly]
        protected float FadeIn = 0.5f;

        /// <summary>
        /// Сообщение висит
        /// </summary>
        [SerializeField, UsedImplicitly]
        protected float ShowPeriod = 2;

        /// <summary>
        /// Сообщение исчезает
        /// </summary>
        [SerializeField, UsedImplicitly]
        protected float FadeOut = 0.5f;

        private Dictionary<Type, Type> _bindedInfoAndVmTypes;

        protected Queue<HudNotificationInfo> NotificationItems = new Queue<HudNotificationInfo>();

        private Dictionary<Type, NotificationViewModel> _viewModels = new Dictionary<Type, NotificationViewModel>();

        private DateTime _lastDequeueDateTime;

        /// <summary>
        /// Флаг периода показа сообщений (в это время боковые элементы HUD должны быть скрыты)
        /// </summary>
        protected ReactiveProperty<bool> IsShownSomeNotification = new ReactiveProperty<bool>();


        //=== Props ===========================================================

        protected float TotalShowCycle => BeforeShowDelay + FadeIn + ShowPeriod + FadeOut;

        protected float TotalShowCyclePlus => TotalShowCycle + TimerPeriod * 2;

        public bool IsHudOpen { get; private set; }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            Prefabs.IsNullOrEmptyOrHasNullElements(nameof(Prefabs));
            NotificationsTransform.AssertIfNull(nameof(NotificationsTransform));
            _bindedInfoAndVmTypes = new Dictionary<Type, Type>()
            {
                {typeof(AdminNotificationInfo), typeof(AdminNotificationViewModel)},
                {typeof(AchievedPointsNotificationInfo), typeof(AchievedPointsNotificationViewModel)},
                {typeof(NewRegionNotificationInfo), typeof(NewRegionNotificationViewModel)},
                {typeof(QuestNotificationInfo), typeof(QuestNotificationViewModel)},
                {typeof(PerkNotificationInfo), typeof(PerkNotificationViewModel)},
            };

            IsShownSomeNotification.Value = false;
        }


        //=== Public ==========================================================

        public virtual void Init(IPawnSource pawnSource, WindowsManager windowsManager)
        {
            var timeStream = TimeTicker.Instance.GetLocalTimer(TimerPeriod);

            var isHudOpenStream = pawnSource.PawnChangesStream
                .Zip(D, windowsManager.HasOpenedWindowsButUnclosableStream)
                .Func(D, (prevEgo, newEgo, hasOpenedWindow) => newEgo != null && !hasOpenedWindow);

            isHudOpenStream.Action(D, b => IsHudOpen = b);

            var timeToDequeueStream = timeStream
                .Zip(D, isHudOpenStream)
                .Where(D, (dt, isHudOpen) => (dt - _lastDequeueDateTime).TotalSeconds > TotalShowCycle && isHudOpen && NotificationItems.Count > 0);

            timeToDequeueStream.Action(D, (dt, b) => ShowNextInfo(dt));

            timeStream
                .Where(D, dt => IsShownSomeNotification.Value && (dt - _lastDequeueDateTime).TotalSeconds > TotalShowCyclePlus)
                .Action(D, dt => { IsShownSomeNotification.Value = false; }); //завершение режима отображения
        }

        public abstract void SendNotification(HudNotificationInfo info);


        //=== Private =========================================================

        private string Since(DateTime from, DateTime to)
        {
            return $"{(from - to).TotalSeconds:f3}";
        }

        private void ShowNextInfo(DateTime dateTime)
        {
            if (NotificationItems.Count == 0)
            {
                UI.Logger.IfError()?.Message($"{nameof(NotificationItems)} is empty!").Write();
                return;
            }

            //UI.CallerLog($"dt={dateTime:mm:ss.fff} Since={Since(dateTime,_lastDequeueDateTime)}s, TotalShowCycle={TotalShowCycle:f3}"); //DEBUG
            IsShownSomeNotification.Value = true;
            _lastDequeueDateTime = dateTime;
            var notificationInfo = NotificationItems.Dequeue();
            var vm = GetNotificationViewModel(notificationInfo);
            //UI.CallerLog($"<<<<< {notificationInfo}"); //DEBUG
            vm.Show(notificationInfo);
        }

        private NotificationViewModel GetNotificationViewModel(HudNotificationInfo hudNotificationInfo)
        {
            if (hudNotificationInfo.AssertIfNull(nameof(hudNotificationInfo)))
                return null;

            if (!_bindedInfoAndVmTypes.TryGetValue(hudNotificationInfo.GetType(), out var desiredVmType))
            {
                UI.Logger.IfError()?.Message($"Unhandled type: {hudNotificationInfo.GetType().NiceName()}").Write();
                return null;
            }

            var suitablePrefab = Prefabs.FirstOrDefault(p => p.GetType() == desiredVmType);
            if (suitablePrefab.AssertIfNull(nameof(suitablePrefab)))
                return null;

            return GetViewModel(suitablePrefab);
        }

        private NotificationViewModel GetViewModel(NotificationViewModel prefab)
        {
            var desiredType = prefab.GetType();
            if (!_viewModels.TryGetValue(desiredType, out var vm))
            {
                vm = Instantiate(prefab, NotificationsTransform);
                vm.Init(BeforeShowDelay, ShowPeriod, FadeIn, FadeOut);
                _viewModels[desiredType] = vm;
            }

            return vm;
        }
    }
}