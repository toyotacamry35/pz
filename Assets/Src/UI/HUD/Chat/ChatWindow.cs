using System;
using System.Collections.Generic;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Input;
using JetBrains.Annotations;
using ReactivePropsNs;
using Src.Input;
using TMPro;
using Uins.Cursor;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ChatWindow : DependencyEndNode, IGuiWindow
    {
        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _escHotkey;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _sendMessageHotkey;

        /// <summary> Блокировки и/или переопределения привязок клавиш к командам активируемые при открытии данного окна </summary>    
        [SerializeField, UsedImplicitly]
        private InputBindingsRef _inputBindingsWhenChatWindowOpened;

        [SerializeField, UsedImplicitly]
        private ChatMessageViewModel _chatMessageViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _messagesRoot;

        [SerializeField, UsedImplicitly]
        private TMP_InputField _inputField;

        [SerializeField, UsedImplicitly]
        private ScrollRect _messagesScrollRect;

        private Queue<ChatMessageViewModel> _messages = new Queue<ChatMessageViewModel>();
        private ChatMessageViewModel _freeMessageViewModel;

        private CharacterChatComponent _characterChatComponent;
        private ChatPanel _chatPanel;


        //=== Props ===========================================================

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => UI.BlockedActionsMovementAndCamera;

        private bool _isVisible;

        private CursorControl.Token _token;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();

                    if (_isVisible) {
                        if (_inputBindingsWhenChatWindowOpened?.Target != null)
                            InputManager.Instance.PushBindings(this, _inputBindingsWhenChatWindowOpened.Target);
                    } else {
                        if (_inputBindingsWhenChatWindowOpened?.Target != null)
                            InputManager.Instance.PopBindings(this);
                    }
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            _windowId.AssertIfNull(nameof(_windowId));
            _sendMessageHotkey.AssertIfNull(nameof(_sendMessageHotkey));
            _escHotkey.AssertIfNull(nameof(_escHotkey));
            _chatMessageViewModelPrefab.AssertIfNull(nameof(_chatMessageViewModelPrefab));
            _messagesRoot.AssertIfNull(nameof(_messagesRoot));
            _inputField.AssertIfNull(nameof(_inputField));
            _messagesScrollRect.AssertIfNull(nameof(_messagesScrollRect));
            State.Value = GuiWindowState.Closed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }


        //=== Public ==========================================================

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }

        public void OnOpen(object arg)
        {
            _token = CursorControl.AddCursorFreeRequest(this);
            IsVisible = true;
            ScrollDown();
            _inputField.ActivateInputField();
        }

        public void OnClose()
        {
            IsVisible = false;
            _token.Dispose();
            _token = null;
            _inputField.DeactivateInputField();
            _inputField.text = "";
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            if (_escHotkey.IsFired())
            {
                WindowsManager.Close(this);
                return;
            }

            if (_sendMessageHotkey.IsFired())
            {
                OnSendMessageHotkey();
            }
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
            if (_sendMessageHotkey.IsFired())
                WindowsManager.Open(this);
        }

        public void OnSendMessageHotkey()
        {
            SendMessageIfExists();
            WindowsManager.Close(this);
        }

        public void Init(ChatPanel chatPanel, IPawnSource pawnSource)
        {
            _chatPanel = chatPanel;
            chatPanel.NewMessage += OnNewMessage;
            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        private void OnNewMessage(string senderName, string message)
        {
            if (_messages.Count >= _chatPanel.ChatSettingsDef.ChatWindowMessagesCount)
                HideFirstMessage();

            var chatMessageViewModel = GetChatMessageViewModel();
            if (chatMessageViewModel.AssertIfNull(nameof(chatMessageViewModel)))
                return;

            chatMessageViewModel.Set(
                ChatPanel.CropLength(senderName, _chatPanel.ChatSettingsDef.MaxNameLength),
                ChatPanel.CropLength(message, _chatPanel.ChatSettingsDef.MaxMessageLength));
            chatMessageViewModel.IsVisible = true;
            ScrollDown();
            _messages.Enqueue(chatMessageViewModel);
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _characterChatComponent = null;
            }

            if (newEgo != null)
            {
                _characterChatComponent = newEgo.GetComponent<CharacterChatComponent>();
                _characterChatComponent.AssertIfNull(nameof(_characterChatComponent));
            }
        }

        private ChatMessageViewModel GetChatMessageViewModel()
        {
            if (_freeMessageViewModel == null)
            {
                var newChatMessageViewModel = Instantiate(_chatMessageViewModelPrefab, _messagesRoot);
                newChatMessageViewModel.name = $"{_chatMessageViewModelPrefab.name}{_messages.Count}";
                return newChatMessageViewModel;
            }

            _freeMessageViewModel.transform.SetSiblingIndex(_chatPanel.ChatSettingsDef.ChatWindowMessagesCount);
            return _freeMessageViewModel;
        }

        private void HideFirstMessage()
        {
            _freeMessageViewModel = _messages.Dequeue();
            _freeMessageViewModel.IsVisible = false;
        }

        private void SendMessageIfExists()
        {
            if (string.IsNullOrWhiteSpace(_inputField.text))
                return;

            _characterChatComponent.SendChatMessage(ChatPanel.CropLength(_inputField.text, _chatPanel.ChatSettingsDef.MaxMessageLength));
        }

        private void ScrollDown()
        {
            _messagesScrollRect.verticalNormalizedPosition = 0;
        }
    }
}