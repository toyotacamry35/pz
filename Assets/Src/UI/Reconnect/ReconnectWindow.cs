using System;
using System.Collections;
using ColonyShared.SharedCode.Input;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins.Cursor;
using Uins.GuiWindows;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ReconnectWindow : DependencyEndNode, IGuiWindow
    {
        private const float DebugReconnectingTime = 10;

        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private RectTransform _backgroundRectTransform;

        private Coroutine _reconnectingCoroutine;


        //=== Props ===========================================================

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => UI.BlockedActions;

        private bool _isVisible;

        private CursorControl.Token _token;

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
                }
            }
        }

        private bool _isReconnecting;

        [Binding]
        public bool IsReconnecting
        {
            get => _isReconnecting;
            set
            {
                if (_isReconnecting != value)
                {
                    _isReconnecting = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            _backgroundRectTransform.AssertIfNull(nameof(_backgroundRectTransform));
            State.Value = GuiWindowState.Closed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ==========================================================

        public void OnOpen(object arg)
        {
            IsVisible = true;
            _token = CursorControl.AddCursorFreeRequest(this);
            var bgBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform.root, _backgroundRectTransform);
            BlurredBackground.Instance.SwitchCameraPartialBlur(this, true,
                new Rect(new Vector2(bgBounds.min.x / Screen.width, bgBounds.min.y / Screen.height),
                    new Vector2(bgBounds.size.x / Screen.width, bgBounds.size.y / Screen.height)));
        }

        public void OnClose()
        {
            IsVisible = false;
            StopReconnecting();
            _token.Dispose();
            _token = null;
            BlurredBackground.Instance.SwitchCameraPartialBlur(this, false, new Rect());
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
//            if (_openHotkeyListener.IsFired())
//                WindowsManager.Open(this);
        }

        public void OnQuitButton()
        {
            WindowsManager.Close(this);
            Application.Quit(); //TODOM Go to Lobby
        }

        public void OnReconnectButton()
        {
            IsReconnecting = true;
            _reconnectingCoroutine = this.StartInstrumentedCoroutine(Reconnecting(), nameof(OnReconnectButton));
        }


        //=== Protected =======================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }


        //=== Private =========================================================

        private IEnumerator Reconnecting() //эмуляция реконнекта
        {
            yield return new WaitForSeconds(DebugReconnectingTime);
            StopReconnecting();
        }

        private void StopReconnecting()
        {
            if (_reconnectingCoroutine != null)
                StopCoroutine(_reconnectingCoroutine);
            _reconnectingCoroutine = null;
            IsReconnecting = false;
        }
    }
}