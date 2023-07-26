using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Templates;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;


namespace Assets.ColonyShared.GeneratedCode.Time
{
    public class InGameTime
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("InGameTime");

        public static readonly TimeSpan DayDuration = TimeSpan.FromMinutes(10260.0f);
        public static readonly TimeSpan StartupHours = TimeSpan.FromHours(3.5f);

        private static bool CheckPair(int currVal, int intervalFrom, int intervalTill)
        {
            bool pairIsNotDefault = intervalFrom > InGameTimeIntervalDef.MinVal
                                 && intervalTill < InGameTimeIntervalDef.MaxVal;
            if (pairIsNotDefault && !SharedHelpers.InRangeCycled(currVal, intervalFrom, intervalTill))
                return false;
            else
                return true;
        }

        public static async Task<bool> IsNowWithinInterval([NotNull] InGameTimeIntervalDef interval, IEntitiesRepository repo)
        {
            using (var wrapper = await repo.GetFirstService<IInGameTimeServiceEntityClientBroadcast>())
            {
                var inGameTimeEntity = wrapper.GetFirstService<IInGameTimeServiceEntityClientBroadcast>();

                if (inGameTimeEntity == null)
                {
                    //#"get_gameObject can only be called from the main thread.":       Logger.Log(LogLevel.Error).UnityObj(gameObject).Message("!inGameTimeEntity (repo.Id == " + repo.Id).Write();
                    Logger.IfError()?.Message("!inGameTimeEntity (repo.Id == " + repo.Id).Write();
                    return false;
                }

                var now = await inGameTimeEntity.GetCurrentIngameTime();

                if (!CheckPair(now.Day, interval.DayFrom, interval.DayTill)) return false;
                if (!CheckPair(now.Hour, interval.HourFrom, interval.HourTill)) return false;
                if (!CheckPair(now.Minute, interval.MinuteFrom, interval.MinuteTill)) return false;
            }
            return true;
        }

        public static DateTime GetIngameTimeAt(DateTime serverStartTime, DateTime serverstartIngametime, TimeSpan ingameDayDuration, DateTime wasTime)
        {
            var nowSynced = wasTime;
            var elapsed = nowSynced - serverStartTime;

            double timeRatio = (double)TimeSpan.FromDays(1.0f).Ticks / (double)ingameDayDuration.Ticks;

            var elapsedIngameTicks = elapsed.Ticks * timeRatio;
            var elapsedIngame = TimeSpan.FromTicks((long)elapsedIngameTicks);

            var timeIngame = serverstartIngametime + elapsedIngame;

            return timeIngame;

        }
        public static DateTime GetCurrentIngameTime(DateTime serverStartTime, DateTime serverstartIngametime, TimeSpan ingameDayDuration)
        {
            var nowSynced = new DateTime(SyncTime.Now * 10000);
            return GetIngameTimeAt(serverStartTime, serverstartIngametime, ingameDayDuration, nowSynced);

        }

    }
}
