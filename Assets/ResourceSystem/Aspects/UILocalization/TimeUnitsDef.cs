using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    [Localized]
    public class TimeUnitsDef : BaseResource
    {
        public LocalizedString Years { get; set; }
        public LocalizedString Months { get; set; }
        public LocalizedString Weeks { get; set; }
        public LocalizedString Days { get; set; }
        public LocalizedString Hours { get; set; }
        public LocalizedString Minutes { get; set; }
        public LocalizedString Seconds { get; set; }
    }
}