using System;
using TimeUnits = System.Int64;

namespace ColonyShared.SharedCode.Utils
{
    public static class TimeUnitsHelpers
    {
        public const TimeUnits Infinite = TimeUnits.MaxValue;
        
        // Выдаёт строку вида +1234 со значением относительно relativeAt, или "Inf" или "<-1>"
        public static string RelTimeToString(this TimeUnits time, TimeUnits relativeAt)
        {
            return time > 0 ? (time != Infinite ? string.Format("{0:+0;-0;+0}", time - relativeAt) : "Inf") : $"<{time}>";
        }

        public static string RelTimeToString(this TimeUnits time)
        {
            return RelTimeToString(time, SyncTime.Now);
        }
        
        // Выдаёт строку вида 99999999(+1234) с абсолютным значением и со значением относительно relativeAt, или "Inf" или "<-1>"
        public static string TimeToString(this TimeUnits time, TimeUnits relativeAt)
        {
            //return time > 0 ? time != long.MaxValue ? $"{time}({time - start})" : "Inf" : time.ToString();
            return time > 0 ? (time != Infinite ? string.Format("{0}({1:+0;-0;+0})", time, time - relativeAt) : "Inf") : $"<{time}>";
        }
        
        // Выдаёт строку вида 99999999(+1234) с абсолютным значением, или "Inf" или "<-1>"
        public static string TimeToString(this TimeUnits time)
        {
            return TimeToString(time, SyncTime.Now);
        }

        public static TimeUnits TimeSafeAdd(this TimeUnits time, TimeUnits delta)
        {
            return time == Infinite || time < 0 ? time : delta >= 0 ? (TimeUnits)Math.Min((ulong)time + (ulong)delta, Infinite) : time + delta;
        }
    }
}
