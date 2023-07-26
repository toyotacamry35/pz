using Assets.Src.Lib.DOTweenAdds;
using DG.Tweening;
using UnityEngine;

namespace Uins
{
    public class PanelAppearing : MonoBehaviour
    {
        public ProgressPanelBase TargetProgressPanel;

        public TweenSettingsFloat MovingTweenSettings;
        public TweenSettingsAlpha FadingTweenSettings;

        private Tweener _fadingTweener;
        private Tweener _movingYTweener;
        private LinearRelation _durationRatioLinearRelation;


        //=== Unity ===========================================================

        private void Awake()
        {
            TargetProgressPanel.AssertIfNull(nameof(TargetProgressPanel));
            FadingTweenSettings.AssertIfNull(nameof(FadingTweenSettings));
            MovingTweenSettings.AssertIfNull(nameof(MovingTweenSettings));
            HideAndReset();
        }


        //=== Public ==============================================================

        public void HideAndReset()
        {
            _fadingTweener.KillIfExistsAndActive();
            _movingYTweener.KillIfExistsAndActive();
            _durationRatioLinearRelation = new LinearRelation(0, FadingTweenSettings.From, 1, FadingTweenSettings.To);
        }

        public void Appearing(bool isForward, TweenCallback onEnd = null)
        {
            RectTransform movingTransform = TargetProgressPanel.transform as RectTransform;
            CanvasGroup fadingCanvasGroup = TargetProgressPanel.CanvasGroup;
            if (movingTransform.AssertIfNull(nameof(movingTransform)) ||
                fadingCanvasGroup.AssertIfNull(nameof(fadingCanvasGroup)))
                return;

            var anchoredPositionX = movingTransform.anchoredPosition.x;
            //Проигрываем строго To...From
            if (isForward)
            {
                _fadingTweener.KillIfExistsAndActive();
                _movingYTweener.KillIfExistsAndActive();

                fadingCanvasGroup.alpha = FadingTweenSettings.From;
                _fadingTweener = DOTween.To(
                        () => fadingCanvasGroup.alpha,
                        f => fadingCanvasGroup.alpha = f,
                        FadingTweenSettings.To,
                        FadingTweenSettings.Duration)
                    .SetEase(FadingTweenSettings.Ease);

                movingTransform.anchoredPosition
                    = new Vector2(anchoredPositionX, MovingTweenSettings.From);
                _movingYTweener = DOTween.To(
                        () => movingTransform.anchoredPosition,
                        v2 => movingTransform.anchoredPosition = v2,
                        new Vector2(anchoredPositionX, MovingTweenSettings.To),
                        MovingTweenSettings.Duration)
                    .SetEase(MovingTweenSettings.Ease);
                if (onEnd != null)
                    _movingYTweener.OnComplete(onEnd);
            }
            else
            {
                //Проигрываем остаток до From, а не строго с To. Отслеживаем по позиции MovingTransform
                _fadingTweener.KillIfExistsAndActive();
                _movingYTweener.KillIfExistsAndActive();
                var durationRatio = Mathf.Min(_durationRatioLinearRelation.GetX(fadingCanvasGroup.alpha), 1);

                if (durationRatio <= 0)
                {
                    fadingCanvasGroup.alpha = FadingTweenSettings.From;
                    movingTransform.anchoredPosition = new Vector2(anchoredPositionX, MovingTweenSettings.From);
                    onEnd?.Invoke();
                }
                else
                {
                    _fadingTweener = DOTween.To(
                            () => fadingCanvasGroup.alpha,
                            f => fadingCanvasGroup.alpha = f,
                            FadingTweenSettings.From,
                            FadingTweenSettings.Duration * durationRatio)
                        .SetEase(FadingTweenSettings.Ease.GetBackwardEase());


                    _movingYTweener = DOTween.To(
                            () => movingTransform.anchoredPosition,
                            v2 => movingTransform.anchoredPosition = v2,
                            new Vector2(anchoredPositionX, MovingTweenSettings.From),
                            MovingTweenSettings.Duration * durationRatio)
                        .SetEase(MovingTweenSettings.Ease.GetBackwardEase());

                    if (onEnd != null)
                        _movingYTweener.OnComplete(onEnd);
                }
            }
        }

        public void SetDuration(float newDuration)
        {
            MovingTweenSettings.Duration = newDuration;
            FadingTweenSettings.Duration = newDuration;
        }
    }
}