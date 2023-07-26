using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class RewardElementCtrl : BindingControllerWithUsingProp<RewardElementVM>
    {
        [SerializeField, UsedImplicitly]
        private LocalizationKeyProp MultiplyRewardValueHolder;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyProp AddRewardValueHolder;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyProp RewardCountHolder;


        [Binding, UsedImplicitly]
        public LocalizedString Title { get; set; }

        [Binding, UsedImplicitly]
        public bool CountVisible { get; set; }

        [Binding, UsedImplicitly]
        public LocalizedString CountText => RewardCountHolder.LocalizedString;

        [Binding, UsedImplicitly]
        public int Count { get; set; }

        [Binding, UsedImplicitly]
        public LocalizedString ValueText { get; set; }

        [Binding, UsedImplicitly]
        public object Value { get; set; }

        protected override void Awake()
        {
            base.Awake();

            var defStream = Vmodel.Func(D, vm => vm?.Def);
            var countStream = Vmodel.SubStream(D, vm => vm.Count);
            var isAddStream = Vmodel.Func(D, vm => !(vm?.IsMultiply ?? false));

            Bind(defStream.Func(D, def => def?.Title ?? default), () => Title);

            Bind(isAddStream, () => CountVisible);
            NotifyPropertyChanged(nameof(CountText));
            Bind(countStream, () => Count);

            Bind(isAddStream.Func(D, b => b ? AddRewardValueHolder.LocalizedString : MultiplyRewardValueHolder.LocalizedString),
                () => ValueText
            );
            Bind(defStream
                    .Zip(D, countStream)
                    .Zip(D, isAddStream)
                    .Func(
                        D,
                        (def, count, isAdd) => (object) (isAdd ? (def?.Experience ?? 0) * count : def?.ExperienceMultiplier ?? 0)
                    ),
                () => Value
            );
        }
    }
}