using L10n;

namespace Uins
{
    public class QuestNotificationInfo : HudNotificationInfo
    {
        public bool IsNewNorDone;
        public LocalizedString QuestName;

        public QuestNotificationInfo(LocalizedString questName, bool isNewNorDone)
        {
            IsNewNorDone = isNewNorDone;
            QuestName = questName;
        }

        public override string ToString()
        {
            return $"({GetType()}: {nameof(QuestName)}={QuestName}, {nameof(IsNewNorDone)}{IsNewNorDone.AsSign()})";
        }
    }
}