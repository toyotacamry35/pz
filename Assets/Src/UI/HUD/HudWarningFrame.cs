using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HudWarningFrame : MonoBehaviour
{
    public Image FrameImage;

    [Range(0, 1)]
    public float DefaultMinAlpha;

    [Range(0, 1)]
    public float DefaultMaxAlpha;

    public Color WarningColor;

    [Range(0, 20)]
    public float WarningFreq;

    [Range(0, 10)]
    public int WarningLoopCount;

    public Color CriticalColor;

    [Range(0, 20)]
    public float CriticalFreq;

    [Range(0, 10)]
    public int CriticaLoopCount;

    private Sequence _alphaSequence;


    //=== Unity ===============================================================

    private void Awake()
    {
        FrameImage.AssertIfNull(nameof(FrameImage));

        FrameImage.SetAlpha(0);
    }


    //=== Public ==============================================================

    public void CriticaBlinking()
    {
        Blinking(CriticalColor, CriticalFreq, CriticaLoopCount, DefaultMinAlpha, DefaultMaxAlpha);
    }

    public void WarningBlinking()
    {
        Blinking(WarningColor, WarningFreq, WarningLoopCount, DefaultMinAlpha, DefaultMaxAlpha);
    }

    public void Blinking(Color color, float freq, int loopCount, float minAlpha, float maxAlpha)
    {
        var blinkCount = loopCount * 2;
        var blinkDuration = 1 / freq / 2;
        var fadeOutDuration = minAlpha * blinkDuration / 2;
//        DebugLog($"{nameof(freq)}={freq}, {nameof(loopCount)}={loopCount}" +
//                 $"   =>   {nameof(blinkCount)}={blinkCount}, {nameof(blinkDuration)}={blinkDuration:f1}, " +
//                 $"{nameof(fadeOutDuration)}={fadeOutDuration:f1}, ");

        _alphaSequence.KillIfExistsAndActive();
        FrameImage.enabled = true;
        FrameImage.color = color;
        FrameImage.SetAlpha(minAlpha);
        _alphaSequence = DOTween.Sequence()
            .Append(FrameImage.TweenAlpha(maxAlpha, blinkDuration).SetLoops(blinkCount, LoopType.Yoyo));

        if (Mathf.Approximately(fadeOutDuration, 0))
            FrameImage.enabled = false;
        else
            _alphaSequence.Append(FrameImage.TweenAlpha(0, fadeOutDuration, () => FrameImage.enabled = false));
    }
}