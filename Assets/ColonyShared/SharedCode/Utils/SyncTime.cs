using System;
using JetBrains.Annotations;
using NLog;

using TimeUnits = System.Int64;


namespace ColonyShared.SharedCode.Utils
{
    public static class SyncTime
    {
        // ReSharper disable once UnusedMember.Local
        //[NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("SyncTime");

        // --- Fields [Static] ------------------------------------------------------------------------------

        public static TimeUnits Second = 1000; //ms per second
        public static TimeUnits Hour = Second * 60 * 60; //ms per second
        public static TimeUnits AVeryLongTime = TimeSpan.TicksPerDay * 5;//five days should be enough?

        static bool _outerUpdate = false;
        private static TimeUnits _lastActualTime;

        // This is in ms
        // Could be freezed (by `_outerUpdate`) only on Host
        public static TimeUnits NowUnsynced => (_outerUpdate ? _lastActualTime : DateTime.UtcNow.Ticks) / TimeSpan.TicksPerMillisecond;

        private static TimeUnits _globalClockwatchStart;
        public static void ResetGlobalClockWatch() { _globalClockwatchStart = NowUnsynced; }
        public static TimeUnits GlobalClockwatchNow => NowUnsynced - _globalClockwatchStart;
        public static DateTime TimeUnitsToDateTime(TimeUnits units) => DateTime.MinValue + TimeSpan.FromMilliseconds(units);
        public static TimeUnits DateTimeToTimeUnits(DateTime dateTime) => (TimeUnits) (dateTime - DateTime.MinValue).TotalMilliseconds;

        public static TimeUnits DeltaToServer = 0;
        // This is in ms
        public static TimeUnits Now
        {
            get
            {
                return NowUnsynced + DeltaToServer;
            }

        }


        public static Func<TimeUnits> SyncedClock => () => NowUnsynced + DeltaToServer;

        public static Func<TimeUnits> StableClock 
        {
            get
            {
                var origin = Now;
                var originUnsunced = NowUnsynced;
                return () => origin + (NowUnsynced - originUnsunced);
            }            
        }
        
        // --- Methods [Static] ------------------------------------------------------------------------------

        public static void StartUpdating(TimeUnits newTime)
        {
            _outerUpdate = false;
            _lastActualTime = newTime;
        }
        public static void NewTime(TimeUnits newTime)
        {
            _lastActualTime = newTime;
        }
        public static TimeUnits FromSeconds(float seconds)
        {
            return (TimeUnits)Math.Round(Second * seconds);
        }
        public static TimeUnits FromHours(float hours)
        {
            return (TimeUnits)Math.Round(Hour * hours);
        }
        public static TimeUnits FromSeconds(double seconds)
        {
            return (TimeUnits)Math.Round(Second * seconds);
        }
        public static long LeftTo(TimeUnits time)
        {
            return time - Now;
        }
        public static long PassedSince(TimeUnits time)
        {
            return Now - time;
        }
        public static bool InThePast(TimeUnits time)
        {
            return Now >= time;
        }

        public static bool InThePast(TimeUnits time, TimeUnits staticCurrent)
        {
            return staticCurrent >= time;
        }
        
        public static bool Earlier(TimeUnits earlier, TimeUnits later)
        {
            return earlier < later;
        }
        public static long FromNow(TimeUnits time)
        {
            return time - Now;
        }

        public static bool InTheFuture(TimeUnits time)
        {
            return Now < time;
        }

        public static bool InTheFuture(TimeUnits time, TimeUnits staticCurrent)
        {
            return staticCurrent < time;
        }

        public static float ToSeconds(long time)
        {
            return (float)time / (float)Second;
        }
        
        public static float ToSecondsSafe(long time)
        {
            return time != TimeUnits.MaxValue ? (float)time / (float)Second : float.MaxValue;
        }

        internal static long ToTicks(long startTime)
        {
            return startTime * TimeSpan.TicksPerMillisecond;
        }
    }
}
