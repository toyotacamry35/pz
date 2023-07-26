using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static partial class Extensions
{
    public static Tweener TweenAlpha(this Graphic graphic, float toAlpha, float duration, TweenCallback onComplete = null,
        Ease ease = Ease.Linear)
    {
        return DOTween.To(
             () => graphic.color.a,
             graphic.SetAlpha,
             toAlpha,
             duration)
         .OnComplete(onComplete)
         .SetEase(ease);
    }

    public static Tweener TweenAlpha(this CanvasGroup canvasGroup, float toAlpha, float duration, TweenCallback onComplete = null,
        Ease ease = Ease.Linear)
    {
        return DOTween.To(
             () => canvasGroup.alpha,
             (val) => canvasGroup.alpha = val,
             toAlpha,
             duration)
         .OnComplete(onComplete)
         .SetEase(ease);
    }

    public static void KillIfExistsAndActive(this Tweener tweener)
    {
        if (tweener != null && tweener.IsActive())
            tweener.Kill();
    }

    public static void KillIfExistsAndActive(this Sequence sequence)
    {
        if (sequence != null && sequence.IsActive())
            sequence.Kill();
    }

    public static Ease GetBackwardEase(this Ease ease)
    {
        switch (ease)
        {
            case Ease.InBack:
                return Ease.OutBack;

            case Ease.InSine:
                return Ease.OutSine;

            case Ease.InQuad:
                return Ease.OutQuad;

            case Ease.InCubic:
                return Ease.OutCubic;

            case Ease.InQuart:
                return Ease.OutQuart;

            case Ease.InQuint:
                return Ease.OutQuint;

            case Ease.InExpo:
                return Ease.OutExpo;

            case Ease.InCirc:
                return Ease.OutCirc;

            case Ease.InElastic:
                return Ease.OutElastic;

            case Ease.InBounce:
                return Ease.OutBounce;

            case Ease.InFlash:
                return Ease.OutFlash;


            case Ease.OutBack:
                return Ease.InBack;

            case Ease.OutSine:
                return Ease.InSine;

            case Ease.OutQuad:
                return Ease.InQuad;

            case Ease.OutCubic:
                return Ease.InCubic;

            case Ease.OutQuart:
                return Ease.InQuart;

            case Ease.OutQuint:
                return Ease.InQuint;

            case Ease.OutExpo:
                return Ease.InExpo;

            case Ease.OutCirc:
                return Ease.InCirc;

            case Ease.OutElastic:
                return Ease.InElastic;

            case Ease.OutBounce:
                return Ease.InBounce;

            case Ease.OutFlash:
                return Ease.InFlash;

            default:
                return ease;
        }
    }
}
