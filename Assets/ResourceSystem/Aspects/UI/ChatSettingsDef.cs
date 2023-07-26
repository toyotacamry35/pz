using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.UI
{
    public class ChatSettingsDef : BaseResource
    {
        public int MaxNameLength { get; set; }
        public int MaxMessageLength { get; set; }
        public int WidgetShowTime { get; set; }
        public int WidgetMessagesCount { get; set; }
        public int ChatWindowMessagesCount { get; set; }
    }
}
