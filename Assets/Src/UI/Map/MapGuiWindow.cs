using System;
using ColonyDI;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using Uins.Cursor;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class MapGuiWindow : DependencyEndNode, IGuiWindow
    {
        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _mapHotkey;

        [UsedImplicitly, SerializeField]
        private HotkeyListener _escHotkey;

        [UsedImplicitly, SerializeField]
        private HotkeyListener _zoomInHotkey;

        [UsedImplicitly, SerializeField]
        private HotkeyListener _zoomOutHotkey;

        [UsedImplicitly, SerializeField]
        [Range(0.01f, 1)]
        private float _minZoom;

        /// <summary>
        /// Число шагов зума
        /// </summary>
        [UsedImplicitly, SerializeField]
        [Range(1, 20)]
        private int _zoomStepCount;

        /// <summary>
        /// Индекс zoom-позиции: с 0 (максимальное приближение), по _zoomStepCount (минимальное)
        /// </summary>
        [UsedImplicitly, SerializeField]
        [Range(1, 20)]
        private int _startZoomStepIndex;

        [UsedImplicitly, SerializeField]
        private LocalizationKeysHolderHi _localizationKeysHolder;

        [UsedImplicitly, SerializeField]
        private ScrollRect _mapScrollRect;

        [UsedImplicitly, SerializeField]
        private MapIndicatorsPositions _mapPositions;

        private int _currentZoomStepIndex;
        private LinearRelation _zoomRatioLinearRelation;


        //=== Props ===========================================================

        [Dependency]
        private GameState GameState { get; set; }

        [Dependency]
        private InventoryNode InventoryNode { get; set; }

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => CurrentWindowStack.InputBindings ?? UI.BlockedActionsAndCamera;

        private bool _isWorking;

        private CursorControl.Token _token;

        [Binding]
        public bool IsWorking
        {
            get => _isWorking;
            set
            {
                if (_isWorking != value)
                {
                    _isWorking = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isOpen;

        [Binding]
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _currentZoomStepRatio;

        [Binding]
        public float CurrentZoomStepRatio
        {
            get => _currentZoomStepRatio;
            set
            {
                if (!Mathf.Approximately(_currentZoomStepRatio, value))
                {
                    _currentZoomStepRatio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ReactiveProperty<float> CurrentZoomRatioRp { get; }= new ReactiveProperty<float>();

        [Binding]
        public float CurrentZoomRatio { get; private set; }

        private LocalizedString _mapTitle1;

        [Binding]
        public LocalizedString MapTitle1
        {
            get => _mapTitle1;
            set
            {
                if (!_mapTitle1.Equals(value))
                {
                    _mapTitle1 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _mapTitle2;

        [Binding]
        public LocalizedString MapTitle2
        {
            get => _mapTitle2;
            set
            {
                if (!_mapTitle2.Equals(value))
                {
                    _mapTitle2 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isZoomInButtonInteractive;

        [Binding]
        public bool IsZoomInButtonInteractive
        {
            get => _isZoomInButtonInteractive;
            set
            {
                if (_isZoomInButtonInteractive != value)
                {
                    _isZoomInButtonInteractive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isZoomOutButtonInteractive;

        [Binding]
        public bool IsZoomOutButtonInteractive
        {
            get => _isZoomOutButtonInteractive;
            set
            {
                if (_isZoomOutButtonInteractive != value)
                {
                    _isZoomOutButtonInteractive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isLegendOpen;

        [Binding]
        public bool IsLegendOpen
        {
            get => _isLegendOpen;
            set
            {
                if (_isLegendOpen != value)
                {
                    _isLegendOpen = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _windowId.AssertIfNull(nameof(_windowId));
            _mapHotkey.AssertIfNull(nameof(_mapHotkey));
            _escHotkey.AssertIfNull(nameof(_escHotkey));
            _zoomInHotkey.AssertIfNull(nameof(_zoomInHotkey));
            _zoomOutHotkey.AssertIfNull(nameof(_zoomOutHotkey));
            _localizationKeysHolder.AssertIfNull(nameof(_localizationKeysHolder));
            _mapScrollRect.AssertIfNull(nameof(_mapScrollRect));
            _mapPositions.AssertIfNull(nameof(_mapPositions));

            State.Value = GuiWindowState.Closed;
            Bind(CurrentZoomRatioRp, () => CurrentZoomRatio);
            ZoomInit();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ==========================================================

        public void OnOpen(object arg)
        {
            IsOpen = true;
            _token = CursorControl.AddCursorFreeRequest(this);
            MapCentering(updatePlayerPosition: true);
            MapTitle1 = _localizationKeysHolder.Ls1;
//            MapTitle2 = _localizationKeysHolder.Ls2;
        }

        public void OnClose()
        {
            IsOpen = false;
            _token.Dispose();
            _token = null;
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            if (_mapHotkey.IsFired() || _escHotkey.IsFired())
                WindowsManager.Close(this);

            if (_zoomInHotkey.IsFired())
                ZoomInOrOut(true);

            if (_zoomOutHotkey.IsFired())
                ZoomInOrOut(false);

            InventoryNode.MapWindowOpener.OpenMapUpdate();
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
            if (IsWorking && _mapHotkey.IsFired())
            {
                additionalAction?.Invoke();
                WindowsManager.Open(this);
            }
        }


        //=== Public ==========================================================

        [UsedImplicitly]
        public void OnZoomInButton()
        {
            ZoomInOrOut(true);
        }

        [UsedImplicitly]
        public void OnZoomOutButton()
        {
            ZoomInOrOut(false);
        }

        [UsedImplicitly]
        public void OnLegendButton()
        {
            IsLegendOpen = !IsLegendOpen;
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
            GameState.IsInGameRp.Action(D, b => IsWorking = b);
        }


        //=== Private =========================================================

        private void ZoomInit()
        {
            if (_zoomStepCount <= 0)
            {
                UI.Logger.IfError()?.Message($"Wrong {nameof(_zoomStepCount)}={_zoomStepCount}. Fixed").Write();
                _zoomStepCount = 1;
            }

            if (_minZoom <= 0)
            {
                UI.Logger.IfError()?.Message($"Wrong {nameof(_minZoom)}={_minZoom}. Fixed").Write();
                _minZoom = 0.1f;
            }

            if (_startZoomStepIndex > _zoomStepCount)
            {
                UI.Logger.IfError()?.Message($"Wrong {nameof(_startZoomStepIndex)}={_startZoomStepIndex}, {nameof(_zoomStepCount)}={_zoomStepCount}").Write();
                _startZoomStepIndex = 0;
            }

            _zoomRatioLinearRelation = new LinearRelation(0, 1, 1, _minZoom);
            _currentZoomStepIndex = _startZoomStepIndex;

            SetZoom(_currentZoomStepIndex);
        }

        private void SetZoom(int zoomStepIndex)
        {
            var prevMouseAnchoredPosition = GetMouseAnchoredPosition();
            CurrentZoomStepRatio = zoomStepIndex / (float) _zoomStepCount;
            CurrentZoomRatioRp.Value = _zoomRatioLinearRelation.GetClampedY(CurrentZoomStepRatio);
            var newMouseAnchoredPosition = GetMouseAnchoredPosition();
            ReturnMapPointToCursor(prevMouseAnchoredPosition, newMouseAnchoredPosition);
        }

        private void ReturnMapPointToCursor(Vector2 prevMouseAnchoredPosition, Vector2 newMouseAnchoredPosition)
        {
            var mapRectTransform = _mapScrollRect.content;
            var viewRectTransform = _mapScrollRect.viewport;

            if (!Mathf.Approximately(prevMouseAnchoredPosition.x, newMouseAnchoredPosition.x))
            {
                ChangeScrollRectNormalizedPosition(mapRectTransform.rect.width, viewRectTransform.rect.width,
                    prevMouseAnchoredPosition.x - newMouseAnchoredPosition.x,
                    () => _mapScrollRect.horizontalNormalizedPosition,
                    np => _mapScrollRect.horizontalNormalizedPosition = np);
            }

            if (!Mathf.Approximately(prevMouseAnchoredPosition.y, newMouseAnchoredPosition.y))
            {
                ChangeScrollRectNormalizedPosition(mapRectTransform.rect.height, viewRectTransform.rect.height,
                    prevMouseAnchoredPosition.y - newMouseAnchoredPosition.y,
                    () => _mapScrollRect.verticalNormalizedPosition,
                    np => _mapScrollRect.verticalNormalizedPosition = np);
            }
        }

        /// <summary>
        /// Выставляет contentSize внутри viewSize относительно anchoredPos (-0...1 отн. начала contentSize)
        /// </summary>
        private void ChangeScrollRectNormalizedPosition(float contentSize, float viewSize, float anchoredDir, Func<float> getter,
            Action<float> setter)
        {
            if (contentSize <= viewSize) //нельзя скроллить
                return;

            var halfScrollRun = (contentSize - viewSize) / 2;
            var normalizedPositionLr = new LinearRelation(-halfScrollRun, 0, halfScrollRun, 1);

            var localPosRelativeToContentCenter = normalizedPositionLr.GetX(getter()) + anchoredDir * contentSize;
            if (Math.Abs(localPosRelativeToContentCenter) > halfScrollRun)
            {
                localPosRelativeToContentCenter = localPosRelativeToContentCenter < 0 ? -halfScrollRun : halfScrollRun;
            }

            setter.Invoke(normalizedPositionLr.GetY(localPosRelativeToContentCenter));
        }

        /// <summary>
        /// Возвращает позицию курсора мыши в локальных отн координатах -0,5...0,5 (отн. центра) карты
        /// </summary>
        private Vector2 GetMouseAnchoredPosition()
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _mapScrollRect.content, Input.mousePosition, null, out var localCursorPos))
                return new Vector2(0.5f, 0.5f);

            return new Vector2(localCursorPos.x / _mapScrollRect.content.rect.width, localCursorPos.y / _mapScrollRect.content.rect.height);
        }

        private void MapCentering(bool updatePlayerPosition = false)
        {
            if (_mapPositions?.PlayerIndicator == null)
                return;

            var mapRectTransform = _mapScrollRect.content;
            var viewRectTransform = _mapScrollRect.viewport;

            if (updatePlayerPosition)
                _mapPositions.PlayerIndicator.UpdatePosition(true);

            SetScrollRectNormalizedPosition(mapRectTransform.rect.width, viewRectTransform.rect.width,
                _mapPositions.PlayerIndicator.MapRelPosition.x, np => _mapScrollRect.horizontalNormalizedPosition = np);

            SetScrollRectNormalizedPosition(mapRectTransform.rect.height, viewRectTransform.rect.height,
                _mapPositions.PlayerIndicator.MapRelPosition.y, np => _mapScrollRect.verticalNormalizedPosition = np);
        }

        /// <summary>
        /// Выставляет contentSize внутри viewSize относительно anchoredPos (-0...1 отн. начала contentSize)
        /// </summary>
        private void SetScrollRectNormalizedPosition(float contentSize, float viewSize, float anchoredPos, Action<float> setter)
        {
            if (contentSize <= viewSize) //нельзя скроллить
                return;

            var halfScrollRun = (contentSize - viewSize) / 2;
            var normalizedPositionLr = new LinearRelation(-halfScrollRun, 0, halfScrollRun, 1);

            var localPosRelativeToContentCenter = (anchoredPos - 0.5f) * contentSize;
            if (Math.Abs(localPosRelativeToContentCenter) > halfScrollRun)
            {
                localPosRelativeToContentCenter = localPosRelativeToContentCenter < 0 ? -halfScrollRun : halfScrollRun;
            }

            setter.Invoke(normalizedPositionLr.GetY(localPosRelativeToContentCenter));
        }

        private void ZoomInOrOut(bool isZoomIn)
        {
            var newZoomStepIndex = _currentZoomStepIndex + (isZoomIn ? -1 : 1);
            newZoomStepIndex = Mathf.Max(0, newZoomStepIndex);
            newZoomStepIndex = Mathf.Min(_zoomStepCount, newZoomStepIndex);

            if (newZoomStepIndex != _currentZoomStepIndex)
            {
                _currentZoomStepIndex = newZoomStepIndex;
                IsZoomInButtonInteractive = _currentZoomStepIndex > 0;
                IsZoomOutButtonInteractive = _currentZoomStepIndex < _zoomStepCount;
                SetZoom(_currentZoomStepIndex);
            }
        }
    }
}