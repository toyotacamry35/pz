using Assets.ColonyShared.GeneratedCode.Time;
using Assets.ColonyShared.SharedCode.Aspects.Templates;
using Assets.ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using NLog;
using System;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    public class RegionTime
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("RegionTime");

        public static float CurrentDayInHourWithMinutes => IngameTime.DayOfYear * 24 + IngameTime.Hour + IngameTime.Minute / 60.0f + IngameTime.Second / 60.0f / 60.0f;
        public static DateTime IngameTime => InGameTime.GetCurrentIngameTime(TimeParams.ServerStartTime, TimeParams.ServerStartupIngameTime, TimeParams.IngameTimeDayDuration);

        public static TimeParamsData TimeParams { get; set; } = TimeParamsData.Empty;

        public static bool IsNowWithinInterval([NotNull] InGameTimeIntervalDef interval)
        {
            var now = IngameTime;

            if (!CheckPair(now.Day, interval.DayFrom, interval.DayTill)) return false;
            if (!CheckPair(now.Hour, interval.HourFrom, interval.HourTill)) return false;
            if (!CheckPair(now.Minute, interval.MinuteFrom, interval.MinuteTill)) return false;

            return true;
        }
        private static bool CheckPair(int currVal, int intervalFrom, int intervalTill)
        {
            bool pairIsNotDefault = intervalFrom > InGameTimeIntervalDef.MinVal
                                 && intervalTill < InGameTimeIntervalDef.MaxVal;
            if (pairIsNotDefault && !SharedHelpers.InRangeCycled(currVal, intervalFrom, intervalTill))
                return false;
            else
                return true;
        }

        public class TimeParamsData
        {
            public static readonly TimeParamsData Empty = new TimeParamsData(DateTime.Now, DateTime.Today + InGameTime.StartupHours, InGameTime.DayDuration);

            public readonly DateTime ServerStartTime;
            public readonly DateTime ServerStartupIngameTime;
            public readonly TimeSpan IngameTimeDayDuration;

            public TimeParamsData() { }

            public TimeParamsData(DateTime serverStartTime, DateTime serverStartupIngameTime, TimeSpan ingameTimeDayDuration)
            {
                ServerStartTime = serverStartTime;
                ServerStartupIngameTime = serverStartupIngameTime;
                IngameTimeDayDuration = ingameTimeDayDuration;
            }
        }
    }
}
