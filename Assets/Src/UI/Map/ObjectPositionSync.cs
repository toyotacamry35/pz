using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    public class ObjectPositionSync : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private RectTransform _target;

        [SerializeField, UsedImplicitly]
        private bool _doXSync = true;

        [SerializeField, UsedImplicitly]
        private bool _doYSync = true;

        private bool _isInited;

        private RectTransform _selfRectTransform;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_target.AssertIfNull(nameof(_target)) ||
                (!_doXSync && !_doYSync))
                return;

            _selfRectTransform = transform.GetRectTransform();
            _isInited = true;
        }

        private void Update()
        {
            if (!_isInited)
                return;

            if ((_doXSync && !Mathf.Approximately(_selfRectTransform.anchoredPosition.x, _target.anchoredPosition.x)) ||
                (_doYSync && !Mathf.Approximately(_selfRectTransform.anchoredPosition.y, _target.anchoredPosition.y)))
            {
                _selfRectTransform.anchoredPosition = new Vector2(
                    _doXSync ? _target.anchoredPosition.x : _selfRectTransform.anchoredPosition.x,
                    _doYSync ? _target.anchoredPosition.y : _selfRectTransform.anchoredPosition.y);
            }
        }
    }
}