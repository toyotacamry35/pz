using System;
using System.Linq;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PremiumHudNotifier : BindingViewModel
    {
        public readonly bool IsActivePremiumUi = true;
        private const int DayInSeconds = 60 * 60 * 24;

        [SerializeField, UsedImplicitly]
        private PremiumWindow _premiumWindow;

        [SerializeField, UsedImplicitly]
        private TimeUnitsDefRef _timeUnitsDefRef;

        private ReactiveProperty<bool> _isVisibleRp = new ReactiveProperty<bool>();

        private IStream<bool> _hasPremiumStream;

        private DateTime _lastNotificationTime;


        //=== Props ==============================================================

        [Binding]
        public bool IsVisible { get; set; }

        [Binding]
        public string LocalizedSecondsBefore { get; set; }

        [Binding]
        public bool HasPremium { get; set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _premiumWindow.AssertIfNull(nameof(_premiumWindow));
            //_premiumDefRef.Target.AssertIfNull(nameof(_premiumDefRef));
            _timeUnitsDefRef.Target.AssertIfNull(nameof(_timeUnitsDefRef));
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource)
        {
            if (!IsActivePremiumUi)
                return;

            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);

            /*var expiration = pawnSource.TouchableEntityProxy.Child(D, character => character.PremiumStatus).ToStream(D, prem => prem.Expiration);

            //    pawnSource.PawnChangesStream.SubscribeOnInterfaceTreadPool<IHasPremiumClientFull>(D, ReplicationLevel.ClientFull);

            Bind(_isVisibleRp, () => IsVisible);

            expiration
                .Zip(D, TimeTicker.Instance.GetUtcTimer(60))
                .Func(D, (exp, dt) => (int) (exp - dt).TotalMinutes)
                .Where(D, m => m >= 0)
                .PrevAndCurrent(D)
                //пауза между сообщениями
                .Where(D, tuple => (DateTime.Now - _lastNotificationTime).TotalMinutes > _premiumDefRef.Target.NotificationSilencePeriod)
                //пересечение одной точек нотификации
                .Where(D, (prev, curr) => _premiumDefRef.Target.NotificationPoints.Any(i => prev > i && curr <= i))
                .Action(D, OnMinutesBeforeChanged);

            var secondsBeforeStream = expiration
                .Zip(D, TimeTicker.Instance.GetUtcTimer(1))
                .Zip(D, _isVisibleRp) // ((_expirationRp, UtcTimer), _isVisibleRp)
                .Where(D, tuple => tuple.Item2)
                .Func(D, (exp, dt, isVisible) => (float) (exp - dt).TotalSeconds);

            var localizedDhmsStream = secondsBeforeStream.Func(D, GetLocalizedDhms);
            Bind(localizedDhmsStream, () => LocalizedSecondsBefore);
            _hasPremiumStream = secondsBeforeStream.Func(D, seconds => seconds > 0);
            Bind(_hasPremiumStream, () => HasPremium);

            _premiumWindow.Init(pawnSource, expiration, _hasPremiumStream);*/
        }


        //=== Private =========================================================

        private void OnMinutesBeforeChanged(int prevMinutesBefore, int minutesBefore)
        {
            _lastNotificationTime = DateTime.Now;
            PremiumExpiresNotifier.Instance.Show(minutesBefore);
        }

        private void OnOurPawnChanged(EntityGameObject prevEntityGameObject, EntityGameObject newEntityGameObject)
        {
            if (prevEntityGameObject != null)
            {
                _isVisibleRp.Value = false;
            }

            if (newEntityGameObject != null)
            {
                _isVisibleRp.Value = true;
            }
        }

        private string GetLocalizedDhms(float secondsBefore)
        {
            return TimeUnitsLocalization.GetDhmsMajorUnitOnlyFromSeconds(_timeUnitsDefRef.Target, secondsBefore);
//            return secondsBefore >= DayInSeconds
//                ? TimeUnitsLocalization.GetDaysHoursLocalizedStringFromSeconds(_timeUnitsDefRef.Target, secondsBefore, true)
//                : TimeUnitsLocalization.GetDhmsLocalizedStringFromSeconds(_timeUnitsDefRef.Target, secondsBefore);
        }
    }
}