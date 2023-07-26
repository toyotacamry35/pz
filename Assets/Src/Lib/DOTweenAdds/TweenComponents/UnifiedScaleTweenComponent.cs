using Assets.Src.Lib.DOTweenAdds;
using UnityEngine;

public class UnifiedScaleTweenComponent : TweenComponentBase
{
    private RectTransform _rectTransform;

    protected override float Parameter
    {
        get { return _rectTransform.sizeDelta.x; }
        set { _rectTransform.sizeDelta = new Vector2(value, value); }
    }

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
    }
}