using System;
using ColonyDI;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Settings
{
    [RequireComponent(typeof(SettingsGuiWindowCtrl))]
    [Binding]
    public class SettingsGuiWindow : BaseGuiWindow
    {
        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("SettingsGuiWindow");

        [SerializeField, UsedImplicitly] private HotkeyListener _escHotkey;
        [SerializeField, UsedImplicitly] private HotkeyListener _applyHotkey;
        [SerializeField, UsedImplicitly] private HotkeyListener _defaultsHotkey;
        //#Tmp затык, пока не сделано Lobby окном, чтобы прикрывать этим фоном UI Lobby:
        [SerializeField, UsedImplicitly] private GameObject _tmp_BG_to_hide_LobbyUI;
        [Dependency]
        private ConfirmationDialog ConfirmationDialog { get; set; }

        private event Action EscHotkeyPressed;
        private event Action ApplyHotkeyPressed;
        private event Action DefaultsHotkeyPressed;

        private SettingsGuiWindowCtrl _ctrl;

        [UsedImplicitly] 
        protected override void Awake()
        {
            base.Awake();
        
            _escHotkey             .AssertIfNull(nameof(_escHotkey));
            _applyHotkey           .AssertIfNull(nameof(_applyHotkey));
            _defaultsHotkey        .AssertIfNull(nameof(_defaultsHotkey));
            if (!_tmp_BG_to_hide_LobbyUI.AssertIfNull(nameof(_tmp_BG_to_hide_LobbyUI)))
                _tmp_BG_to_hide_LobbyUI.SetActive(false);

            _ctrl = GetComponent<SettingsGuiWindowCtrl>();
            _ctrl.AssertIfNull(nameof(SettingsGuiWindowCtrl));
        }

        protected override void Start()
        {
            base.Start();
            _ctrl.Init(EscHotkeyPressed, ApplyHotkeyPressed, DefaultsHotkeyPressed, CloseWindow, OpenConfirmationDialog);
        }

        private void OpenConfirmationDialog(ConfirmationDialogParams dialogParams)
        {
            WindowsManager.Open(ConfirmationDialog, /*CurrentWindowStack*/null, dialogParams);
        }

        public override void OnOpen(object arg)
        {
             // BlurredBackground.Instance.SwitchCameraFullBlur(this, true, MainCamera.Camera);
            base.OnOpen(arg);
            //#Tmp затык, пока не сделано Lobby окном, чтобы прикрывать этим фоном UI Lobby:
            //_tmp_BG_to_hide_LobbyUI.SetActive(!WindowsManager.IsInGame);

            if (!_ctrl.Vmodel.HasValue)
                _ctrl.SetVmodel(new SettingsGuiWindowVM());
            
            _ctrl.SetActive(true);
        }

        public override void OnClose()
        {
            _ctrl.SetActive(false);
            base.OnClose();
            // BlurredBackground.Instance.SwitchCameraFullBlur(this, false, MainCamera.Camera);
        }

        public override void OpenUpdate()
        {
            base.OpenUpdate();
            if (State.Value != GuiWindowState.Closed)
            {
                if (_escHotkey.IsFired())
                    EscHotkeyPressed?.Invoke();//WindowsManager.Close(this);
                if (_defaultsHotkey.IsFired())
                    DefaultsHotkeyPressed?.Invoke();
                if (_applyHotkey.IsFired())
                    ApplyHotkeyPressed?.Invoke();
            }
        }

        private void CloseWindow()
        {
            WindowsManager.Close(this);
        }

    }
}
