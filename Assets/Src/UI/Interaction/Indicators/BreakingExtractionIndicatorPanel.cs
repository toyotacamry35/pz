using Assets.Src.Lib.DOTweenAdds;
using Core.Environment.Logging.Extension;
using DG.Tweening;
using UnityEngine;

namespace Uins
{
    public class BreakingExtractionIndicatorPanel : MonoBehaviour
    {
        public Transform[] MovedParts;
        public CanvasGroup CanvasGroup;
        public TweenSettingsAlpha AlphaTweenSettings;
        public TweenSettingsFloat OffsetTweenSettings;

        private Tweener _alphaTweener;
        private Tweener _offsetTweener;


        //=== Props ===============================================================

        private float MovedPartsOffset
        {
            get { return ((RectTransform) MovedParts[0]).anchoredPosition.y; }
            set
            {
                for (int i = 0, len = MovedParts.Length; i < len; i++)
                {
                    ((RectTransform) MovedParts[i]).anchoredPosition = new Vector2(0, value);
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            MovedParts.IsNullOrEmptyOrHasNullElements(nameof(MovedParts));
            CanvasGroup.AssertIfNull(nameof(CanvasGroup));
            AlphaTweenSettings.AssertIfNull(nameof(AlphaTweenSettings));
            OffsetTweenSettings.AssertIfNull(nameof(OffsetTweenSettings));
            ((RectTransform) transform).anchoredPosition = Vector2.zero;
            CanvasGroup.alpha = 0;
        }


        //=== Public ==============================================================

        public void HideAndReset()
        {
            _offsetTweener.KillIfExistsAndActive();
            _alphaTweener.KillIfExistsAndActive();
            CanvasGroup.alpha = 0;
        }

        public void Appearing(TweenCallback onEnd = null, float duration = 0)
        {
            CanvasGroup.alpha = AlphaTweenSettings.From;
            _alphaTweener.KillIfExistsAndActive();
            _alphaTweener = DOTween.To(
                    () => CanvasGroup.alpha,
                    a => CanvasGroup.alpha = a,
                    AlphaTweenSettings.To,
                    duration > 0 ? duration : AlphaTweenSettings.Duration)
                .SetEase(AlphaTweenSettings.Ease)
                .SetDelay(AlphaTweenSettings.Delay);

            if (onEnd != null)
                _alphaTweener.OnComplete(onEnd);

            MovedPartsOffset = OffsetTweenSettings.From;
            _offsetTweener.KillIfExistsAndActive();
            _offsetTweener = DOTween.To(
                    () => MovedPartsOffset,
                    f => MovedPartsOffset = f,
                    OffsetTweenSettings.To,
                    duration > 0 ? duration : OffsetTweenSettings.Duration)
                .SetEase(OffsetTweenSettings.Ease)
                .SetDelay(OffsetTweenSettings.Delay);
        }

        public void Debug_AppearingTest()
        {
            Appearing(() => UI.Logger.IfDebug()?.Message($"[{Time.frameCount}] <{GetType()}> OnAppearingComplete()").Write()); //DEBUG
        }
    }
}