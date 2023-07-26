using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.Templates
{
    public class InGameTimeIntervalDef : BaseResource
    {
        public int DayFrom    { get; set; } = MinVal;
        public int DayTill    { get; set; } = MaxVal;
        public int HourFrom   { get; set; } = MinVal;
        public int HourTill   { get; set; } = MaxVal;
        public int MinuteFrom { get; set; } = MinVal;
        public int MinuteTill { get; set; } = MaxVal;

        public static int MinVal = int.MinValue;
        public static int MaxVal = int.MaxValue;
    }
}
