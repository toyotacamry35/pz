using Assets.ColonyShared.SharedCode.Aspects.UI;
using Assets.Src.ResourceSystem;
using ColonyDI;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    public class ChatPanel : DependencyEndNode
    {
        public delegate void NewMessageDelegate(string senderName, string message);

        public event NewMessageDelegate NewMessage;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _testMessageHotkeyListener;

        [SerializeField, UsedImplicitly]
        private ChatWidget _chatWidget;

        [SerializeField, UsedImplicitly]
        private ChatWindow _chatWindow;

        [SerializeField, UsedImplicitly]
        private ChatSettingsDefRef _chatSettingsDefRef;


        //=== Props ===========================================================

        [Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        public static ChatPanel Instance { get; private set; }

        public ChatSettingsDef ChatSettingsDef { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _testMessageHotkeyListener.AssertIfNull(nameof(_testMessageHotkeyListener));
            _chatWidget.AssertIfNull(nameof(_chatWidget));
            _chatWindow.AssertIfNull(nameof(_chatWindow));
            Instance = SingletonOps.TrySetInstance(this, Instance);

            ChatSettingsDef = (_chatSettingsDefRef?.Target).AssertIfNull(nameof(_chatSettingsDefRef))
                ? new ChatSettingsDef()
                : _chatSettingsDefRef.Target;

            ChatSettingsDef.MaxNameLength = Mathf.Max(5, ChatSettingsDef.MaxNameLength);
            ChatSettingsDef.MaxMessageLength = Mathf.Max(10, ChatSettingsDef.MaxMessageLength);
            ChatSettingsDef.ChatWindowMessagesCount = Mathf.Max(1, ChatSettingsDef.ChatWindowMessagesCount);
            ChatSettingsDef.WidgetMessagesCount = Mathf.Max(1, ChatSettingsDef.WidgetMessagesCount);
            ChatSettingsDef.WidgetShowTime = Mathf.Max(1, ChatSettingsDef.WidgetShowTime);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            _chatWidget.Init(this, WindowsManager);
            _chatWindow.Init(this, SurvivalGui);
        }

        public static string CropLength(string message, int maxLen)
        {
            return message.Length > maxLen ? message.Substring(0, maxLen) + "..." : message;
        }

        public void AddNewMessage(string senderName, string message)
        {
            NewMessage?.Invoke(CropLength(senderName, ChatSettingsDef.MaxNameLength), CropLength(message, ChatSettingsDef.MaxMessageLength));
        }
    }
}