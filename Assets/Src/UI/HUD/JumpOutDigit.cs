using Assets.Src.Lib.DOTweenAdds;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class JumpOutDigit : MonoBehaviour
{
    public TextMeshProUGUI DamageTmp;
    public CanvasGroup CanvasGroup;

    private TweenSettingsAlpha _fadeInSettings;
    private TweenSettingsAlpha _fadeOutSettings;
    private TweenSettingsVector3 _jumpOutSettings;
    private TweenSettingsFloat _scaleSettings;

    private Sequence _alphaSequence;
    private Tweener _jumpOutTweener;
    private Tweener _scaleTweener;


    //=== Props ===============================================================

    public bool IsShown { get; private set; }


    //=== Unity ===============================================================

    private void Awake()
    {
        CanvasGroup.AssertIfNull(nameof(CanvasGroup));
        DamageTmp.AssertIfNull(nameof(DamageTmp));

        CanvasGroup.alpha = 0;
    }


    //=== Public ==============================================================

    public void Setup(TweenSettingsAlpha fadeInSettings, TweenSettingsAlpha fadeOutSettings, 
        TweenSettingsVector3 jumpOutSettings, TweenSettingsFloat scaleSettings)
    {
        _fadeInSettings = fadeInSettings;
        _fadeOutSettings = fadeOutSettings;
        _jumpOutSettings = jumpOutSettings;
        _scaleSettings = scaleSettings;
    }

    public void StartShowDamage(int negativeDamage)
    {
        IsShown = true;
        DamageTmp.text = negativeDamage.ToString();

        //alpha
        _alphaSequence.KillIfExistsAndActive();
        _alphaSequence = DOTween.Sequence();

        if (_fadeInSettings.Duration > 0)
        {
            CanvasGroup.alpha = _fadeInSettings.From;
            _alphaSequence.Append(
                DOTween.To(
                    () => CanvasGroup.alpha,
                    f => CanvasGroup.alpha = f,
                    _fadeInSettings.To,
                    _fadeInSettings.Duration));
        }
        else
        {
            CanvasGroup.alpha = _fadeInSettings.To;
        }

        if (_fadeOutSettings.Delay > 0)
            _alphaSequence.AppendInterval(_fadeOutSettings.Delay);

        if (_fadeOutSettings.Duration > 0)
            _alphaSequence.Append(
                DOTween.To(
                    () => CanvasGroup.alpha,
                    f => CanvasGroup.alpha = f,
                    _fadeOutSettings.To,
                    _fadeOutSettings.Duration));

        _alphaSequence.AppendCallback(End); //финал на весь объект

        //position
        _jumpOutTweener.KillIfExistsAndActive();
        transform.localPosition = Vector3.zero;
        _jumpOutTweener = transform.DOLocalMove(_jumpOutSettings.To, _jumpOutSettings.Duration)
            .SetEase(_jumpOutSettings.Ease);

        //scale
        _scaleTweener.KillIfExistsAndActive();
        transform.localScale = Vector3.one;
        _scaleTweener = transform.DOScale(_scaleSettings.To, _scaleSettings.Duration)
            .SetDelay(_scaleSettings.Delay)
            .SetEase(_scaleSettings.Ease);
    }


    //=== Private ==============================================================

    private void End()
    {
        CanvasGroup.alpha = 0;
        IsShown = false;
    }
}