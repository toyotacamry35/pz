using System;
using Assets.Src.ResourceSystem;
using Assets.Src.ResourceSystem.L10n;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Input;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using Uins.Cursor;
using Uins.GuiWindows;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PremiumWindow : DependencyEndNode, IGuiWindow
    {
        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _openHotkey;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _escHotkey;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _urlButtonTextLocalizationKeyPairsDefRef;

        private ReactiveProperty<bool> _isOpenRp = new ReactiveProperty<bool>();

        private ReactiveProperty<bool> _hasPawnRp = new ReactiveProperty<bool>();

        private CursorControl.Token _token;
        private ITouchable<IWorldCharacterClientFull> _touchableEntityProxy;


        //=== Props ===========================================================

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => UI.BlockedActionsMovementAndCamera;

        [Binding]
        public bool IsVisible { get; set; }

        [Binding]
        public bool HasPremium { get; set; }

        [Binding]
        public DateTime ExpiredDateTime { get; set; }

        [Binding]
        public bool IsDisplayExpiredDateTime { get; set; }

        [Binding]
        public LocalizedString UrlButtonText { get; set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _windowId.AssertIfNull(nameof(_windowId));
            _openHotkey.AssertIfNull(nameof(_openHotkey));
            _escHotkey.AssertIfNull(nameof(_escHotkey));
            //_premiumDefRef.Target.AssertIfNull(nameof(_premiumDefRef));
            _urlButtonTextLocalizationKeyPairsDefRef.Target.AssertIfNull(nameof(_urlButtonTextLocalizationKeyPairsDefRef));
            State.Value = GuiWindowState.Closed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
            _isOpenRp.Dispose();
            _hasPawnRp.Dispose();
        }


        //=== Public ==========================================================

        public void OnOpen(object arg)
        {
            _isOpenRp.Value = true;
            _token = CursorControl.AddCursorFreeRequest(this);
            BlurredBackground.Instance.SwitchCameraFullBlur(this, true);
            //UI.CallerLogInfo("Sound TODO: UI_Prem_Open");
            AkSoundEngine.PostEvent("UI_Prem_Open", transform.root.gameObject);
        }

        public void OnClose()
        {
            _isOpenRp.Value = false;
            _token.Dispose();
            _token = null;
            BlurredBackground.Instance.SwitchCameraFullBlur(this, false);
            //UI.CallerLogInfo("Sound TODO: UI_Prem_Close");
            AkSoundEngine.PostEvent("UI_Prem_Close", transform.root.gameObject);
        }

        public void OnFade()
        {
            _isOpenRp.Value = false;
        }

        public void OnUnfade()
        {
            _isOpenRp.Value = true;
        }

        public void OpenUpdate()
        {
            if (_escHotkey.IsFired() || _openHotkey.IsFired())
            {
                WindowsManager.Close(this);
            }
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
            if (_hasPawnRp.Value && _openHotkey.IsFired())
                WindowsManager.Open(this);
        }

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }

        public void Init(IPawnSource pawnSource, IStream<DateTime> expiredStream, IStream<bool> hasPremiumStream)
        {
            var expiredLocalStream = expiredStream.Func(D, dt => dt.ToLocalTime());
            var isDisplayExpiredDateTimeStream = expiredLocalStream.Func(D, dt => dt.Year > 2000);
            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
            _touchableEntityProxy = pawnSource.TouchableEntityProxy;
            var isVisibleStream = _hasPawnRp.Zip(D, _isOpenRp).Func(D, tuple => tuple.Item1 && tuple.Item2);
            Bind(isVisibleStream, () => IsVisible);
            Bind(hasPremiumStream, () => HasPremium);
            Bind(hasPremiumStream
                    .Func(D, b => b ? _urlButtonTextLocalizationKeyPairsDefRef.Target.Ls1 : _urlButtonTextLocalizationKeyPairsDefRef.Target.Ls2),
                () => UrlButtonText);
            Bind(expiredLocalStream, () => ExpiredDateTime);
            Bind(isDisplayExpiredDateTimeStream, () => IsDisplayExpiredDateTime);
        }

        [UsedImplicitly]
        public void OnOpenUrl()
        {
            //UI.CallerLogInfo("Sound TODO: UI_Prem_Activate");
            AkSoundEngine.PostEvent("UI_Prem_Activate", transform.root.gameObject);
            //Application.OpenURL(_premiumDefRef.Target.PremiumShopUrl);
        }

        [UsedImplicitly]
        public void OnCancelBtn()
        {
            WindowsManager.Close(this);
        }

        [UsedImplicitly]
        public void OnUpdateBtn()
        {
            SoundControl.Instance?.ButtonSmall?.Post(transform.root.gameObject);
            D.Add(_touchableEntityProxy.Subscribe(new PremiumRefreshStatusToucher()));
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEntityGameObject, EntityGameObject newEntityGameObject)
        {
            if (prevEntityGameObject != null)
            {
                if (IsVisible)
                    WindowsManager.Close(this);

                _hasPawnRp.Value = false;
            }

            if (newEntityGameObject != null)
            {
                _hasPawnRp.Value = true;
            }
        }
    }
}