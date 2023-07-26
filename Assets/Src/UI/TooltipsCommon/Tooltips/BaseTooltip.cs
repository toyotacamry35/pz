using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins.Tooltips
{
    [Binding]
    public abstract class BaseTooltip : BindingViewModel
    {
        public CanvasGroup CanvasGroup;

        [SerializeField, UsedImplicitly]
        private Image _backgroundImage;

        private RectTransform _rectTransform;
        private Vector2 _outPosition = new Vector2(0, 2500);

        private RectTransform _parentRectTransform;

        //=== Props ===============================================================

        public Vector2 Size => _backgroundImage.rectTransform.sizeDelta + _backgroundImage.rectTransform.anchoredPosition * 2;

        public bool IsPositionTracking { get; set; }

        public Vector2 Position
        {
            get { return _rectTransform.anchoredPosition; }

            set { _rectTransform.anchoredPosition = value; }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            _rectTransform = (RectTransform) transform;
            _backgroundImage.AssertIfNull(nameof(_backgroundImage));
            CanvasGroup.AssertIfNull(nameof(CanvasGroup));
            MoveOut();
            OnAwake();
        }

        private void Start()
        {
            _parentRectTransform = transform.parent.GetRectTransform();
        }


        //=== Public ==========================================================

        public void MoveOut()
        {
            Position = _outPosition;
            IsPositionTracking = false;
            CanvasGroup.alpha = 0;
        }

        public static void Show(MonoBehaviour monoBehaviour)
        {
            TooltipSelector.Instance?.ShowTooltip(monoBehaviour);
        }

        public static void Hide()
        {
            TooltipSelector.Instance?.ShowTooltip();
        }

        /// <summary>
        /// Выставление содержимого
        /// </summary>
        public void Setup(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour.AssertIfNull(nameof(monoBehaviour)))
                return;

            IsPositionTracking = true;
            OnSetup(monoBehaviour);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            InventoryContextMenu.SetPositionWithGuaranteedFullDisplayability(_rectTransform, _parentRectTransform);
            AfterLayoutRecalculated();
        }

        public void SetBackgroundHeight(float height)
        {
            _backgroundImage.rectTransform.sizeDelta = new Vector2(_backgroundImage.rectTransform.sizeDelta.x, height);
        }


        //=== Protected ============================================================

        protected abstract void OnAwake();

        protected abstract void OnSetup(MonoBehaviour monoBehaviour);

        protected virtual void AfterLayoutRecalculated() { }
    }
}