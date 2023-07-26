using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ChatWidget : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private ChatMessageViewModel _chatMessageViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _messagesRoot;

        [SerializeField, UsedImplicitly]
        private Image _lameBg;

        [SerializeField, UsedImplicitly]
        private ChatWindow _chatWindow;

        private Queue<ChatMessageViewModel> _messages = new Queue<ChatMessageViewModel>();

        private ChatMessageViewModel _freeMessageViewModel;
        private DateTime _lastMessageTime;
        private ChatPanel _chatPanel;
        private float _showTime;


        //=== Props ===============================================================

        private ReactiveProperty<bool> _isVisibleByTimeRp = new ReactiveProperty<bool>();

        [Binding]
        public bool IsVisibleByTime { get; private set; }

        [Binding]
        public bool IsVisible { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _chatMessageViewModelPrefab.AssertIfNull(nameof(_chatMessageViewModelPrefab));
            _messagesRoot.AssertIfNull(nameof(_messagesRoot));
            _lameBg.AssertIfNull(nameof(_lameBg));
            _chatWindow.AssertIfNull(nameof(_chatWindow));
            _isVisibleByTimeRp.Value = false;
        }


        //=== Public ==========================================================

        public void Init(ChatPanel chatPanel, WindowsManager windowsManager)
        {
            _chatPanel = chatPanel;
            _showTime = _chatPanel.ChatSettingsDef.WidgetShowTime;
            chatPanel.NewMessage += OnNewMessage;

            TimeTicker.Instance.GetLocalTimer(0.5f)
                .Zip(D, _isVisibleByTimeRp)
                .Where(D, (dt, isVisibleByTime) => isVisibleByTime && (dt - _lastMessageTime).TotalSeconds > _showTime)
                .Action(D, tuple =>_isVisibleByTimeRp.Value = false);

            var isOpenChatWindowStream = _chatWindow.State.Func(D, s => s != GuiWindowState.Closed);
            Bind(_isVisibleByTimeRp, () => IsVisibleByTime);
            var isVisibleStream = isOpenChatWindowStream.Zip(D, _isVisibleByTimeRp)
                .Func(D, (isOpenChatWindow, isVisibleByTime) => isVisibleByTime && !isOpenChatWindow);
            Bind(isVisibleStream, () => IsVisible);
        }


        //=== Private =========================================================

        private void OnNewMessage(string senderName, string message)
        {
            _lastMessageTime = DateTime.Now;
            _isVisibleByTimeRp.Value = true;
            if (_messages.Count >= _chatPanel.ChatSettingsDef.WidgetMessagesCount)
                HideFirstMessage();

            var chatMessageViewModel = GetChatMessageViewModel();
            if (chatMessageViewModel.AssertIfNull(nameof(chatMessageViewModel)))
                return;

            chatMessageViewModel.Set(
                ChatPanel.CropLength(senderName, _chatPanel.ChatSettingsDef.MaxNameLength),
                ChatPanel.CropLength(message, _chatPanel.ChatSettingsDef.MaxMessageLength));
            chatMessageViewModel.IsVisible = true;
            _messages.Enqueue(chatMessageViewModel);

            int i = _messages.Count - 1;
            foreach (var vm in _messages)
                vm.OrderIndex = i--;

            _lameBg.enabled = false;
            _lameBg.enabled = true;
        }

        private ChatMessageViewModel GetChatMessageViewModel()
        {
            if (_freeMessageViewModel == null)
            {
                var newChatMessageViewModel = Instantiate(_chatMessageViewModelPrefab, _messagesRoot);
                newChatMessageViewModel.name = $"{_chatMessageViewModelPrefab.name}{_messages.Count}";
                return newChatMessageViewModel;
            }

            _freeMessageViewModel.transform.SetSiblingIndex(_chatPanel.ChatSettingsDef.WidgetMessagesCount);
            return _freeMessageViewModel;
        }

        private void HideFirstMessage()
        {
            _freeMessageViewModel = _messages.Dequeue();
            _freeMessageViewModel.IsVisible = false;
        }
    }
}