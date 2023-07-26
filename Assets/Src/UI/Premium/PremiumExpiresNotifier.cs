using Assets.Src.Lib.DOTweenAdds;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PremiumExpiresNotifier : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private TimeUnitsDefRef _timeUnitsDefRef;

        [SerializeField, UsedImplicitly]
        private TweenComponentBase _tweenComponent;

        ReactiveProperty<int> _minutesBeforeRp = new ReactiveProperty<int>();


        //=== Props ===========================================================

        public static PremiumExpiresNotifier Instance { get; private set; }

        [Binding]
        public string HoursMinutesBeforeAsString { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
            _timeUnitsDefRef.Target.AssertIfNull(nameof(_timeUnitsDefRef));
            _tweenComponent.AssertIfNull(nameof(_tweenComponent));

            var minutesBeforeStringStream = _minutesBeforeRp.Func(D, GetHoursMinutesBeforeAsString);
            Bind(minutesBeforeStringStream, () => HoursMinutesBeforeAsString);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        public void Show(int minutesBefore)
        {
            _minutesBeforeRp.Value = minutesBefore;
            _tweenComponent.Play(true);
        }


        //=== Private =========================================================

        private string GetHoursMinutesBeforeAsString(int minutesBefore)
        {
            return TimeUnitsLocalization.GetLocalizedStringFromMinutes(_timeUnitsDefRef.Target, minutesBefore);
        }
    }
}