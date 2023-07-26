using SharedCode.Aspects.Item.Templates;

namespace Uins
{
    public class PerkNotificationInfo : HudNotificationInfo
    {
        public BaseItemResource ItemResource;

        public PerkNotificationInfo(BaseItemResource itemResource)
        {
            ItemResource = itemResource;
        }

        public override string ToString()
        {
            return $"({GetType()}: {nameof(ItemResource)}={ItemResource})";
        }
    }
}