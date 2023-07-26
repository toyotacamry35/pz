using Core.Environment.Logging.Extension;
using DG.Tweening;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class AchievedItemContr : BindingControllerWithUsingProp<AchievedItemVmodel>
    {
        public const float ItemShowTime = 3;
        public const float ItemFadeTime = .5f;

        [Binding]
        public Sprite ResourceSprite { get; private set; }

        [Binding]
        public LocalizedString Description { get; private set; }

        [Binding]
        public int AchievedItemsCount { get; private set; }

        [Binding]
        public int TotalItemsCount { get; private set; }

        [Binding]
        public float Alpha { get; protected set; }

        private ReactiveProperty<float> _alphaRp = new ReactiveProperty<float>() {Value = 0};

        private Sequence _sequence;

        protected override void Awake()
        {
            base.Awake();
            Bind(Vmodel.Func(D, vm => vm.ResourceSprite), () => ResourceSprite);
            Bind(Vmodel.Func(D, vm => vm.Description), () => Description);
            Bind(Vmodel.Func(D, vm => vm.AchievedItemsCount), () => AchievedItemsCount);
            Bind(Vmodel.Func(D, vm => vm.TotalItemsCount), () => TotalItemsCount);
            Bind(_alphaRp, () => Alpha);

            _sequence = DOTween.Sequence()
                .Append(
                    DOTween.To(
                        () => _alphaRp.Value,
                        f => _alphaRp.Value = f,
                        1,
                        ItemFadeTime))
                .AppendInterval(ItemShowTime - 2 * ItemFadeTime)
                .Append(
                    DOTween.To(
                        () => _alphaRp.Value,
                        f => _alphaRp.Value = f,
                        0,
                        ItemFadeTime))
                .AppendCallback(OnEndSequence)
                .SetAutoKill(false);
            _sequence.Pause();

            Vmodel.Action(
                D,
                vm =>
                {
                    if (vm.IsEmpty)
                        return;

                    _alphaRp.Value = 0;
                    _sequence.Restart();
                });
        }

        /// <summary>
        /// Удаление vm из ShownVmodels влечет за собой возврат в пул данного контрола
        /// </summary>
        private void OnEndSequence()
        {
            var vm = Vmodel.Value;
            if (vm.ShownVmodels.AssertIfNull(nameof(vm.ShownVmodels)))
                return;

            if (!vm.ShownVmodels.Remove(vm))
                UI.Logger.IfError()?.Message($"{transform.FullName()} Nothing to remove: {vm}").Write();
        }
    }
}