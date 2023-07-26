using System.Linq;
using Assets.Src.Aspects.Impl.Traumas.Template;
using Assets.Src.Lib.DOTweenAdds;
using Assets.Src.ResourceSystem;
using DG.Tweening;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public class NegativeIndicatorsVisibility : HasDisposablesMonoBehaviour
    {
        public CanvasGroup OverheatIndicator;
        public CanvasGroup HypothermiaIndicator;
        public CanvasGroup IntoxicationIndicator;

        [SerializeField, UsedImplicitly]
        private NegativeFactorWarningsDefRef _overheatWarningsDefRef;

        [SerializeField, UsedImplicitly]
        private NegativeFactorWarningsDefRef _hypothermiaWarningsDefRef;

        [SerializeField, UsedImplicitly]
        private NegativeFactorWarningsDefRef _intoxicationWarningsDefRef;

        public WarningMessager WarningMessager;

        public TweenSettingsAlpha TweenSettings;

        [SerializeField, UsedImplicitly]
        private NegativeFactorVisualNotifier _overheatVisualNotifier;

        [SerializeField, UsedImplicitly]
        private NegativeFactorVisualNotifier _hypothermiaVisualNotifier;

        [SerializeField, UsedImplicitly]
        private NegativeFactorVisualNotifier _intoxicationVisualNotifier;

        [SerializeField, UsedImplicitly]
        private PlayerMainStatsViewModel _playerMainStatsViewModel;

        private NegativeFactorWarningsDef _overheatWarningsDef;
        private NegativeFactorWarningsDef _hypothermiaWarningsDef;
        private NegativeFactorWarningsDef _intoxicationWarningsDef;

        private float _overheatMaxRatio;
        private float _hypothermiaMaxRatio;
        private float _intoxicationMaxRatio;

        private Tweener _tweenerOverheat;
        private Tweener _tweenerHypothermia;
        private Tweener _tweenerIntoxication;


        //=== Unity ==============================================================

        private void Awake()
        {
            OverheatIndicator.AssertIfNull(nameof(OverheatIndicator));
            HypothermiaIndicator.AssertIfNull(nameof(HypothermiaIndicator));
            IntoxicationIndicator.AssertIfNull(nameof(IntoxicationIndicator));
            _playerMainStatsViewModel.AssertIfNull(nameof(_playerMainStatsViewModel));
            _overheatWarningsDefRef.Target.AssertIfNull(nameof(_overheatWarningsDefRef));
            _hypothermiaWarningsDefRef.Target.AssertIfNull(nameof(_hypothermiaWarningsDefRef));
            _intoxicationWarningsDefRef.Target.AssertIfNull(nameof(_intoxicationWarningsDefRef));
            WarningMessager.AssertIfNull(nameof(WarningMessager));
            TweenSettings.AssertIfNull(nameof(TweenSettings));

            _overheatWarningsDef = _overheatWarningsDefRef.Target;
            _overheatWarningsDef.AssertIfNull(nameof(_overheatWarningsDef));
            _hypothermiaWarningsDef = _hypothermiaWarningsDefRef.Target;
            _hypothermiaWarningsDef.AssertIfNull(nameof(_hypothermiaWarningsDef));
            _intoxicationWarningsDef = _intoxicationWarningsDefRef.Target;
            _intoxicationWarningsDef.AssertIfNull(nameof(_intoxicationWarningsDef));

            _overheatMaxRatio = _overheatWarningsDef.Warnings.Keys.Max() / 100f;
            _hypothermiaMaxRatio = _hypothermiaWarningsDef.Warnings.Keys.Max() / 100f;
            _intoxicationMaxRatio = _intoxicationWarningsDef.Warnings.Keys.Max() / 100f;
        }

        public void Init()
        {
            var overheatRatioPcStream = _playerMainStatsViewModel.OverheatRatioStream.PrevAndCurrent(D);
            overheatRatioPcStream.Action(D, (prev, curr) =>
                Check(OverheatIndicator, prev, curr, _overheatWarningsDef, ref _tweenerOverheat, _overheatVisualNotifier, _overheatMaxRatio));

            var hypothermiaRatioPcStream = _playerMainStatsViewModel.HypothermiaRatioStream.PrevAndCurrent(D);
            hypothermiaRatioPcStream.Action(D, (prev, curr) =>
                Check(HypothermiaIndicator, prev, curr, _hypothermiaWarningsDef, ref _tweenerHypothermia, _hypothermiaVisualNotifier, _hypothermiaMaxRatio));

            var intoxicationRatioPcStream = _playerMainStatsViewModel.IntoxicationRatioStream.PrevAndCurrent(D);
            intoxicationRatioPcStream.Action(D, (prev, curr) =>
                Check(IntoxicationIndicator, prev, curr, _intoxicationWarningsDef, ref _tweenerIntoxication, _intoxicationVisualNotifier,
                    _intoxicationMaxRatio));
        }


        //=== Private =============================================================

        private void Check(CanvasGroup indicatorCanvasGroup, float prevRatio, float currentRatio, NegativeFactorWarningsDef nfwDef,
            ref Tweener tweener, NegativeFactorVisualNotifier visualNotifier, float maxRatio)
        {
            var warnings = nfwDef.Warnings;
            if (warnings != null && warnings.Count > 0)
            {
                foreach (var kvp in warnings)
                {
                    var point = kvp.Key / 100f;
                    var warningInfo = kvp.Value;
                    if (IsTimeForMessage(prevRatio, currentRatio, point))
                    {
                        var messageColor = ColorUtility.TryParseHtmlString(nfwDef.MessageColor, out var color) ? color : Color.white;
                        WarningMessager.ShowWarningMessage(warningInfo.Message.GetText(), messageColor, nfwDef.MessageIcon?.Target,
                            warningInfo.WaringLevelIcon?.Target, messageColor);
                        visualNotifier?.Play();
                        break;
                    }
                }

                BlinkCheck(indicatorCanvasGroup, currentRatio, maxRatio, ref tweener);
            }
        }

        private void BlinkCheck(CanvasGroup indicatorCanvasGroup, float currentRatio, float borderPoint, ref Tweener tweener)
        {
            if (currentRatio >= borderPoint)
            {
                if (tweener == null)
                {
                    indicatorCanvasGroup.alpha = TweenSettings.From;
                    tweener = DOTween.To(
                            () => indicatorCanvasGroup.alpha,
                            (f) => indicatorCanvasGroup.alpha = f,
                            TweenSettings.To,
                            TweenSettings.Duration)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(TweenSettings.Ease);
                }
            }
            else
            {
                if (tweener != null)
                {
                    tweener.Kill();
                    tweener = null;
                    indicatorCanvasGroup.alpha = 1;
                }
            }
        }

        private bool IsTimeForMessage(float prevRatio, float currentRatio, float borderPoint)
        {
            if (prevRatio > currentRatio)
                return false;

            return prevRatio < borderPoint && currentRatio >= borderPoint;
        }
    }
}