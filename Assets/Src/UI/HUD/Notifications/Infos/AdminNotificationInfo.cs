namespace Uins
{
    public class AdminNotificationInfo : HudNotificationInfo
    {
        public string Title;
        public string MainText;

        public AdminNotificationInfo(string title, string mainText)
        {
            Title = title;
            MainText = mainText;
        }

        public override string ToString()
        {
            return $"({GetType()}: {nameof(Title)}='{Title}', {nameof(MainText)}='{MainText}')";
        }
    }
}