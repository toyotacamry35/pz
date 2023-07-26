using UnityEngine;

public class CanvasScaleReverter : MonoBehaviour
{
    private RectTransform _rectTransform;

    private RectTransform _canvasRectTransform;

    private float _lastScaleX;


    //=== Props ===============================================================

    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = transform.GetRectTransform();
            return _rectTransform;
        }
    }


    //=== Unity ===============================================================

    private void Awake()
    {
        _canvasRectTransform = transform.GetComponentInParent<Canvas>().transform.GetRectTransform();
        _lastScaleX = _canvasRectTransform.localScale.x;
        Rescale();
    }

//    private void OnEnable()
//    {
//        if (_canvasRectTransform == null)
//            return;
//
//        DebugInfo();
//    }

    private void Update()
    {
        if (Mathf.Approximately(_canvasRectTransform.localScale.x, _lastScaleX))
            return;

        _lastScaleX = _canvasRectTransform.localScale.x;
        Rescale();
    }


    //=== Private =============================================================

    private void Rescale()
    {
        transform.localScale = Vector3.one / _lastScaleX;
        DebugInfo();
    }

    private void DebugInfo()
    {
//        Debug.Log($"'{transform.FullName()}': " +
//                  $"canvasSize=({_canvasRectTransform.rect.width * _canvasRectTransform.localScale.x}, " +
//                  $"{_canvasRectTransform.rect.height * _canvasRectTransform.localScale.y}) " +
//                  $"selfRect={RectTransform.rect.size}");
    }
}