using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class UserNewMarkerContextMenu : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private RectTransform _parentRectTransform;

        [SerializeField, UsedImplicitly]
        private RectTransform _selfRectTransform;

        [SerializeField, UsedImplicitly]
        private Image _bgImage;

        [SerializeField, UsedImplicitly]
        private UserNewMarkerContextMenuItem _itemPrefab;

        [SerializeField, UsedImplicitly]
        private UserMarkers _userMarkers;

        [SerializeField, UsedImplicitly]
        private MapGuiWindow _mapGuiWindow;

        [SerializeField, UsedImplicitly]
        private RectTransform _mapRectTransform;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _updateInterval;

        private Vector2 _lastMapPosition;
        private Vector2 _lastClickedMapAnchorPosition;
        private bool _isSubitemsInited;

        private Vector2 _outOfPosition = new Vector2(-30, -30);


        //=== Props ===========================================================

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                    if (!_isVisible)
                        _selfRectTransform.anchoredPosition = _outOfPosition;
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_userMarkers.AssertIfNull(nameof(_userMarkers)) ||
                _mapGuiWindow.AssertIfNull(nameof(_mapGuiWindow)) ||
                _mapRectTransform.AssertIfNull(nameof(_mapRectTransform)) ||
                _itemPrefab.AssertIfNull(nameof(_itemPrefab)) ||
                _bgImage.AssertIfNull(nameof(_bgImage)))
                return;

            _bgImage.DisableSpriteOptimizations();
            if (_parentRectTransform == null)
                _parentRectTransform = transform.parent.GetRectTransform();

            if (_selfRectTransform == null)
                _selfRectTransform = transform.GetRectTransform();

            _userMarkers.ShowContextMenu += OnShowContextMenu;
            _userMarkers.HideContextMenu += OnHideContextMenu;
            _mapGuiWindow.PropertyChanged += OnSomePropertyChanged;
        }

        private void Update()
        {
            if (!_updateInterval.IsItTime() ||
                _mapRectTransform.anchoredPosition == _lastMapPosition)
                return;

            _lastMapPosition = _mapRectTransform.anchoredPosition;
            OnHideContextMenu();
        }


        //=== Public ==========================================================

        public void OnSubitemClick(UserNewMarkerContextMenuItem item)
        {
            if (!IsVisible || item.AssertIfNull(nameof(item)))
                return;

            _userMarkers.SetUserMarkerNotifier(_lastClickedMapAnchorPosition, false, item.NavIndicatorDef);
        }


        //=== Private =========================================================

        private void OnSomePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(_mapGuiWindow.CurrentZoomRatio))
                return;

            OnHideContextMenu();
        }

        private void OnShowContextMenu(Vector2 mapAnchorPosition)
        {
            if (IsVisible && _lastClickedMapAnchorPosition == mapAnchorPosition)
                return;

            _lastClickedMapAnchorPosition = mapAnchorPosition;
            IsVisible = true;
            ShowMenu();
        }

        private void ShowMenu()
        {
            if (!_isSubitemsInited)
                SubitemsInit();

            InventoryContextMenu.SetPositionWithGuaranteedFullDisplayability(_selfRectTransform, _parentRectTransform);
        }

        private void SubitemsInit()
        {
            _isSubitemsInited = true;
            for (int i = 0, len = _userMarkers.UserIndicatorDefs.Length; i < len; i++)
            {
                var subitem = Instantiate(_itemPrefab, _selfRectTransform.transform);
                subitem.name = $"{_itemPrefab.name}{i + 1}";
                subitem.Init(this, _userMarkers.UserIndicatorDefs[i]);
            }
        }

        private void OnHideContextMenu()
        {
            IsVisible = false;
        }
    }
}