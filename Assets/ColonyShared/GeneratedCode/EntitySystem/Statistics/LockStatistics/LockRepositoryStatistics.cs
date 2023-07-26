using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem.Statistics;
using GeneratedCode.Network.Statistic;
using Newtonsoft.Json;
using NLog;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class LockRepositoryStatisticsManager : CoreBaseStatistic
    {
        private readonly ConcurrentDictionary<Guid, LockRepositoryStatistics> _statistics =
            new ConcurrentDictionary<Guid, LockRepositoryStatistics>();

        public LockRepositoryStatistics GetOrAdd(Guid id, LockRepositoryStatisticsPrometheus statisticsPrometheus) =>
            _statistics.GetOrAdd(id, (newId) => new LockRepositoryStatistics(newId, statisticsPrometheus));

        protected override void LogStatistics()
        {
            foreach (var statistics in _statistics)
                statistics.Value.LogStatistics();
        }
    }

    public class LockRepositoryStatistics
    {
        public enum LockRepositoryOperation
        {
            Get,
            Release,
            GetEntityStatusInternal,
            SubscribeOnDestroyOrUnload,
            SubscribeOnDestroyOrUnloadBatch,
            UnsubscribeOnDestroyOrUnload,
            Load,
            LoadSerialize,
            GetTooLongEntityWaitQueues,
            InstallGetRequestTimeout,
            ThrowBatchRemoveFromQueueException,
            LockEntities,
            WaitForBatchWrapper,
            GetBatchContainerUsagesInfo,
            CheckClearOldUsages,
            EntityUpload,
            Dump,
            DumpEntity,
        }

        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-LockRepositoryStatistics");

        private static readonly NLog.Logger _commonLogger = LogManager.GetCurrentClassLogger();

        private DateTime _lastCheckUseTime;
        private Guid _id;
        private readonly LockRepositoryStatisticsPrometheus _statisticsPrometheus;
        private bool _useSecond;

        struct LockData
        {
            public long Overall;
            public long Min;
            public long Max;
            public long Count;

            public void Reset()
            {
                Overall = 0;
                Min = long.MaxValue;
                Max = 0;
                Count = 0;
            }

            public override string ToString()
            {
                return $"{Overall} @ {Count} ({Min}-{Max})";
            }
        }

        struct FrameData
        {
            public LockData[] Use;
            public LockData[] Wait;

            public void Init()
            {
                Use = new LockData[CountersCount];
                Wait = new LockData[CountersCount];
                Reset();
            }

            public void Reset()
            {
                for(int i=0; i< CountersCount; ++i)
                {
                    Use[i].Reset();
                    Wait[i].Reset();
                }
            }
        }

        private readonly FrameData[] _frameData = new FrameData[2];

        private static readonly int CountersCount = Enum.GetValues(typeof(LockRepositoryOperation)).Length;

        public LockRepositoryStatistics(Guid id, LockRepositoryStatisticsPrometheus statisticsPrometheus)
        {
            _id = id;
            _statisticsPrometheus = statisticsPrometheus;
            _lastCheckUseTime = DateTime.UtcNow;

            _frameData[0].Init();
            _frameData[1].Init();
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void AddWait(in TimeSpan duration, LockRepositoryOperation operation)
        {
            _statisticsPrometheus.AddWait(duration, operation);
            AddWaitInternal(duration, operation);
        }

        private void IncrementInternal(LockRepositoryOperation operation)
        {
            ref var data = ref _frameData[_useSecond ? 1 : 0].Wait[(int)operation];

            var ticks = 1;

            InterlockedExchangeIfGreaterThan(ref data.Max, ticks);
            InterlockedExchangeIfLessThan(ref data.Min, ticks);
            Interlocked.Add(ref data.Overall, ticks);
            Interlocked.Increment(ref data.Count);
        }
        private void AddWaitInternal(in TimeSpan duration, LockRepositoryOperation operation)
        {
            ref var data = ref _frameData[_useSecond ? 1 : 0].Wait[(int)operation];

            var ticks = duration.Ticks;

            InterlockedExchangeIfGreaterThan(ref data.Max, ticks);
            InterlockedExchangeIfLessThan(ref data.Min, ticks);
            Interlocked.Add(ref data.Overall, ticks);
            Interlocked.Increment(ref data.Count);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void AddUse(in TimeSpan duration, LockRepositoryOperation operation)
        {
            _statisticsPrometheus.AddUse(duration, operation);
            AddUseInternal(duration, operation);
        }

        private void AddUseInternal(in TimeSpan duration, LockRepositoryOperation operation)
        {
            ref var data = ref _frameData[_useSecond ? 1 : 0].Use[(int)operation];

            var ticks = duration.Ticks;

            InterlockedExchangeIfGreaterThan(ref data.Max, ticks);
            InterlockedExchangeIfLessThan(ref data.Min, ticks);
            Interlocked.Add(ref data.Overall, ticks);
            Interlocked.Increment(ref data.Count);
        }

        public void LogStatistics()
        {
            var currentTicks = DateTime.UtcNow;
            var delta = currentTicks - _lastCheckUseTime;
            if (delta <= TimeSpan.Zero)
                return;

            ref var frame = ref _frameData[_useSecond ? 1 : 0];

            _useSecond = !_useSecond;
            _lastCheckUseTime = currentTicks;

            var useOverall = frame.Use.Sum(v => v.Overall) / (double)delta.Ticks;
            var waitOverall = frame.Wait.Sum(v => v.Overall) / (double) delta.Ticks;

            var UseLockByType = new List<LockInfo>(CountersCount);
            var WaitLockByType = new List<LockInfo>(CountersCount);

            for (var i = 0; i < CountersCount; i++)
            {
                if (frame.Use[i].Overall > 0)
                    UseLockByType.Add(new LockInfo(in frame.Use[i], (LockRepositoryOperation)i, in delta));
                if (frame.Wait[i].Count > 0)
                    WaitLockByType.Add(new LockInfo(in frame.Wait[i], (LockRepositoryOperation)i, in delta));
            }

            UseLockByType.Sort(lockInfoCompare);
            WaitLockByType.Sort(lockInfoCompare);

            _overallStatisticsLog.IfInfo()?.Message("Repo {repo_id}: " +
                "delta {delta_ms}, " +
                "wait percent overall {wait_overall}, " +
                "use percent overall {use_overall}, " +
                "use by type: {@use_by_type}, " +
                "wait by type: {@wait_by_type}",
                _id,
                delta.TotalMilliseconds,
                waitOverall,
                useOverall,
                UseLockByType,
                WaitLockByType
                ).Write();

            frame.Reset();
        }

        private static int lockInfoCompare(LockInfo l1, LockInfo l2)
        {
            return (int) (l2.Overall - l1.Overall);
        }

        void logTicksPercent(double percentOverall, in TimeSpan period)
        {
            if (percentOverall >= 0.8)
                _commonLogger.IfError()?.Message("Critical usage repository {repo_id} lock percent {use_overall:0.##}%. Period {delta_ms}", _id, percentOverall, period.TotalMilliseconds).Write();
            else if (percentOverall >= 0.5)
                _commonLogger.IfWarn()?.Message("Large usage repository {repo_id} lock percent {use_overall:0.##}%. Period {delta_ms}", _id, percentOverall, period.TotalMilliseconds).Write();
            else if (percentOverall >= 0.1)
                _commonLogger.IfDebug()?.Message("Warn usage repository {repo_id} lock percent {use_overall:0.##}%. Period {delta_ms}", _id, percentOverall, period.TotalMilliseconds).Write();
        }

        public static bool InterlockedExchangeIfGreaterThan(ref long location, long newValue)
        {
            long initialValue;
            do
            {
                initialValue = location;
                if (initialValue >= newValue)
                    return false;
            } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

            return true;
        }

        public static bool InterlockedExchangeIfLessThan(ref long location, long newValue)
        {
            long initialValue;
            do
            {
                initialValue = location;
                if (initialValue <= newValue)
                    return false;
            } while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);

            return true;
        }

        readonly struct LockInfo
        {
            [JsonIgnore] public long Overall { get; }
            public LockRepositoryOperation Type { get; }
            public float Percent { get; }
            public double Avg { get; }
            public double Min { get; }
            public double Max { get; }
            public long Count { get; }

            public LockInfo(in LockData data, LockRepositoryOperation opType, in TimeSpan delta)
            {
                Overall = data.Overall;
                Type = opType;
                Percent = data.Overall / (float)delta.Ticks;
                Avg = (data.Count > 0
                    ? TimeSpan.FromTicks((long)((double)data.Overall / data.Count))
                    : TimeSpan.Zero).TotalMilliseconds;
                Max = TimeSpan.FromTicks(data.Max).TotalMilliseconds;
                Min = (data.Min < long.MaxValue
                    ? TimeSpan.FromTicks(data.Min)
                    : TimeSpan.Zero).TotalMilliseconds;
                Count = data.Count;
            }
            public override string ToString()
            {
                return $"Type {Type}: {Percent}% @ {Count}, {Min}/{Avg}/{Max}";
            }
        }
    }
}


public class LockRepositoryStatisticsPrometheus
{
    private readonly PrometheusLockRepositoryStatisticsFactory.LabeledMetrics[] _operationMetrics;

    public LockRepositoryStatisticsPrometheus(
        PrometheusLockRepositoryStatisticsFactory.LabeledMetrics[] operationMetrics)
    {
        _operationMetrics = operationMetrics;
    }

    [Conditional("ENABLE_NETWORK_STATISTICS")]
    public void AddWait(in TimeSpan duration, LockRepositoryStatistics.LockRepositoryOperation operation)
    {
        var metrics = _operationMetrics[(int) operation];
        metrics.WaitTime.Observe(duration.TotalMilliseconds);
    }

    [Conditional("ENABLE_NETWORK_STATISTICS")]
    public void AddUse(in TimeSpan duration, LockRepositoryStatistics.LockRepositoryOperation operation)
    {
        var metrics = _operationMetrics[(int) operation];
        metrics.UseTime.Observe(duration.TotalMilliseconds);
    }
}