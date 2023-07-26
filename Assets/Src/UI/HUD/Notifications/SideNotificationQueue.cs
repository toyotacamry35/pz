namespace Uins
{
    public class SideNotificationQueue : BaseNotificationQueue
    {
        public override void SendNotification(HudNotificationInfo info)
        {
            NotificationItems.Enqueue(info);
        }
    }
}