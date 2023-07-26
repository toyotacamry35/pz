using JetBrains.Annotations;
using L10n;
using UnityEngine;
using UnityWeld.Binding;
using Assets.Src.ResourceSystem;
using ReactivePropsNs;

namespace Uins
{
    [Binding]
    public class TimeLeftCtrl : BindingControllerWithUsingProp<TimeLeftVM>
    {
        [SerializeField, UsedImplicitly]
        private TimeUnitsDefRef TimeUnitsDef;

        [Binding, UsedImplicitly]
        public string TimeLeftString { get; set; }

        protected override void Awake()
        {
            base.Awake();

            var timeLeftMinutes = Vmodel
                .SubStream(D, vm => vm.TimeLeftSeconds)
                .Func(D, timeSec => timeSec / 60);
            var timeLeftText = timeLeftMinutes.Func(
                D,
                timeMinutes => TimeUnitsLocalization.GetLocalizedStringFromMinutes(TimeUnitsDef.Target, timeMinutes)
            );

            Bind(timeLeftText, () => TimeLeftString);
        }
    }
}