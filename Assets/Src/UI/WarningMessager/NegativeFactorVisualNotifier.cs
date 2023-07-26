using Assets.Src.Lib.DOTweenAdds;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class NegativeFactorVisualNotifier : MonoBehaviour
{
    [UsedImplicitly]
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [UsedImplicitly]
    [SerializeField]
    private TweenComponentBase[] _tweenComponents;

    [UsedImplicitly]
    [SerializeField]
    private Color _coloring;

    [UsedImplicitly]
    [SerializeField]
    private Graphic[] _graphics;

    private void Awake()
    {
        _canvasGroup.AssertIfNull(nameof(_canvasGroup));
        _tweenComponents.IsNullOrEmptyOrHasNullElements(nameof(_tweenComponents));
        _canvasGroup.alpha = 0;

        foreach (var graphic in _graphics)
            graphic.SetColoring(_coloring);
    }

    public void Play()
    {
        _canvasGroup.alpha = 1;
        foreach (var tweenComponentBase in _tweenComponents)
            tweenComponentBase.Play(true);
    }
}