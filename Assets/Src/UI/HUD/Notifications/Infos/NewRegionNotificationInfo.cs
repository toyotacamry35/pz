using L10n;

namespace Uins
{
    public class NewRegionNotificationInfo : HudNotificationInfo
    {
        public bool IsFirstTime;
        public LocalizedString RegionName;
        public int ExploringRatio;

        public NewRegionNotificationInfo(LocalizedString regionName, bool isFirstTime = true, int exploringRatio = 50)
        {
            IsFirstTime = isFirstTime;
            RegionName = regionName;
            ExploringRatio = exploringRatio;
        }

        public override string ToString()
        {
            return $"({GetType()}: {nameof(RegionName)}={RegionName}, {nameof(IsFirstTime)}{IsFirstTime.AsSign()})";
        }
    }
}