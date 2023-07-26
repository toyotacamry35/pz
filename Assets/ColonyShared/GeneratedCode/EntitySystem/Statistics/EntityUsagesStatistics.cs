using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Core.Environment.Logging.Extension;
using GeneratedCode.Network.Statistic;
using NLog;
using SharedCode.Monitoring;
using SharedCode.Repositories;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class EntityUsagesStatistics : CoreBaseStatistic
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Frame Active = new Frame();

        private Frame Background = new Frame();

        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-EntityUsagesStatistics");
        private readonly  MetricsStorage<Key, LabeledMetrics, Metrics> _metricStorage;

        private const int TopCount = 20;

        public EntityUsagesStatistics()
        {
            var labelNames = new[] {"entitytid", "callerName"};
            var metrics = new Metrics(Prometheus.Metrics.CreateCounter("entity_up_read", "entity_up_read", labelNames),
                Prometheus.Metrics.CreateCounter("entity_down_read", "entity_down_read", labelNames),
                Prometheus.Metrics.CreateCounter("entity_up_write", "entity_up_write", labelNames),
                Prometheus.Metrics.CreateCounter("entity_down_write", "entity_down_write", labelNames),
                Prometheus.Metrics.CreateCounter("entity_up_from_read_to_exclusive", "entity_up_from_read_to_exclusive",
                    labelNames),
                Prometheus.Metrics.CreateCounter("entity_down_from_read_to_exclusive",
                    "entity_down_from_read_to_exclusive", labelNames));

            _metricStorage = new MetricsStorage<Key, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) =>
                {
                    var type = ReplicaTypeRegistry.GetTypeById(key.TypeId);
                    return new LabeledMetrics(rawMetrics.UpRead.WithLabels(type.Name, key.CallerName),
                        rawMetrics.DownRead.WithLabels(type.Name, key.CallerName),
                        rawMetrics.UpWrite.WithLabels(type.Name, key.CallerName),
                        rawMetrics.DownWrite.WithLabels(type.Name, key.CallerName),
                        rawMetrics.UpFromReadToExclusive.WithLabels(type.Name, key.CallerName),
                        rawMetrics.DownFromReadToExclusive.WithLabels(type.Name, key.CallerName)
                    );
                });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void increment(ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> dictionary, int typeId, Guid entityId, string key)
        {
            try
            {
                if (key == null)
                    throw new Exception("Null key");

                var typeDictionary = dictionary.GetOrAdd(typeId, (_) => new ConcurrentDictionary<string, Counter>(StringComparer.Ordinal));
                var counter = typeDictionary.GetOrAdd(key, (k) => new Counter() {CallerField = k});
                Interlocked.Increment(ref counter.CountField);
            }
            catch(Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void UpRead(string callerName, int typeId, Guid entityId)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(typeId, callerName));
            metrics.UpRead.Inc();
            
            increment(Active.UpReadCnt, typeId, entityId, callerName);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void DownRead(string callerName, int typeId, Guid entityId)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(typeId, callerName));
            metrics.DownRead.Inc();
            
            increment(Active.DownReadCnt, typeId, entityId, callerName);
        }
        
        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void UpWrite(string callerName, int typeId, Guid entityId)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(typeId, callerName));
            metrics.UpWrite.Inc();
            
            increment(Active.UpWriteCnt, typeId, entityId, callerName);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void DownWrite(string callerName, int typeId, Guid entityId)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(typeId, callerName));
            metrics.DownWrite.Inc();
            
            increment(Active.DownWriteCnt, typeId, entityId, callerName);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void UpFromReadToExclusive(string callerName, int typeId, Guid entityId)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(typeId, callerName));
            metrics.UpFromReadToExclusive.Inc();
            
            increment(Active.UpFromReadToExclusiveCnt, typeId, entityId, callerName);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void DownFromReadToExclusive(string callerName, int typeId, Guid entityId)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(typeId, callerName));
            metrics.DownFromReadToExclusive.Inc();
            
            increment(Active.DownFromReadToExclusiveCnt, typeId, entityId, callerName);
        }

        private void swapStatistricsDict()
        {
            Background = Interlocked.Exchange(ref Active, Background);
        }


        protected override void LogStatistics()
        {
            swapStatistricsDict();
            Thread.Sleep(5);//ждем, вдруг из другого потока прямо сейчас хотят дописать в старые коллекции
            copyStats();
            logStatistics();
            clearStats();
        }

        private void logStatistics()
        {
            var overallUpRead = Background.UpReadCnt.Values.Sum(x => x.Values.Sum(y => y.CountFieldTmp));
            var overallUpWrite = Background.UpWriteCnt.Values.Sum(x => x.Values.Sum(y => y.CountFieldTmp)) + Background.UpFromReadToExclusiveCnt.Values.Sum(x => x.Values.Sum(y => y.CountFieldTmp));
            var upReadStats = Background.UpReadCnt.Where(x => x.Value.Any(z => z.Value.CountFieldTmp > 0)).OrderByDescending(x => x.Value.Sum(z => z.Value.CountFieldTmp)).ToDictionary(x => ReplicaTypeRegistry.GetTypeById(x.Key).Name, y => y.Value.Where(z => z.Value.CountFieldTmp > 0).OrderByDescending(z => z.Value.CountFieldTmp).Take(TopCount).Select(z => z.Value).ToList());
            var downReadStats = Background.DownReadCnt.Where(x => x.Value.Any(z => z.Value.CountFieldTmp > 0)).OrderByDescending(x => x.Value.Sum(z => z.Value.CountFieldTmp)).ToDictionary(x => ReplicaTypeRegistry.GetTypeById(x.Key).Name, y => y.Value.Where(z => z.Value.CountFieldTmp > 0).OrderByDescending(z => z.Value.CountFieldTmp).Take(TopCount).Select(z => z.Value).ToList());
            var upWriteStats = Background.UpWriteCnt.Where(x => x.Value.Any(z => z.Value.CountFieldTmp > 0)).OrderByDescending(x => x.Value.Sum(z => z.Value.CountFieldTmp)).ToDictionary(x => ReplicaTypeRegistry.GetTypeById(x.Key).Name, y => y.Value.Where(z => z.Value.CountFieldTmp > 0).OrderByDescending(z => z.Value.CountFieldTmp).Take(TopCount).Select(z => z.Value).ToList());
            var downWriteStats = Background.DownWriteCnt.Where(x => x.Value.Any(z => z.Value.CountFieldTmp > 0)).OrderByDescending(x => x.Value.Sum(z => z.Value.CountFieldTmp)).ToDictionary(x => ReplicaTypeRegistry.GetTypeById(x.Key).Name, y => y.Value.Where(z => z.Value.CountFieldTmp > 0).OrderByDescending(z => z.Value.CountFieldTmp).Take(TopCount).Select(z => z.Value).ToList());
            var upFromReadToExclusiveStats = Background.UpFromReadToExclusiveCnt.Where(x => x.Value.Any(z => z.Value.CountFieldTmp > 0)).OrderByDescending(x => x.Value.Sum(z => z.Value.CountFieldTmp)).ToDictionary(x => ReplicaTypeRegistry.GetTypeById(x.Key).Name, y => y.Value.Where(z => z.Value.CountFieldTmp > 0).OrderByDescending(z => z.Value.CountFieldTmp).Take(TopCount).Select(z => z.Value).ToList());
            var downFromReadToExclusiveStats = Background.DownFromReadToExclusiveCnt.Where(x => x.Value.Any(z => z.Value.CountFieldTmp > 0)).OrderByDescending(x => x.Value.Sum(z => z.Value.CountFieldTmp)).ToDictionary(x => ReplicaTypeRegistry.GetTypeById(x.Key).Name, y => y.Value.Where(z => z.Value.CountFieldTmp > 0).OrderByDescending(z => z.Value.CountFieldTmp).Take(TopCount).Select(z => z.Value).ToList());
                
            _overallStatisticsLog.IfInfo()?.Message(
                "Overall up read {overall_up_read}, " +
                "overall up write {overall_up_write}, " +
                "up read {@up_read_stats}, " +
                "down read {@down_read_stats}, " +
                "up write {@up_write_stats}, " +
                "down write {@down_write_stats}, " +
                "up from read to exclusive {@up_from_read_to_exclusive_stats}, " +
                "down from read to exclusive {@down_from_read_to_exclusive_stats}",
                overallUpRead,
                overallUpWrite,
                upReadStats,
                downReadStats,
                upWriteStats,
                downWriteStats,
                upFromReadToExclusiveStats,
                downFromReadToExclusiveStats
            ).Write();
        }

        private void copyDictCounters(ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> dictionary)
        {
            lock (dictionary)
                foreach (var typeDict in dictionary)
                foreach (var cnt in typeDict.Value)
                    Interlocked.Exchange(ref cnt.Value.CountFieldTmp, cnt.Value.CountField);
        }

        private void clearDictCounters(ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> dictionary)
        {
            lock (dictionary)
                foreach (var typeDict in dictionary)
                    foreach (var cnt in typeDict.Value)
                        cnt.Value.CountField = 0;
        }

        private void copyStats()
        {
            copyDictCounters(Background.UpReadCnt);
            copyDictCounters(Background.DownReadCnt);
            copyDictCounters(Background.UpWriteCnt);
            copyDictCounters(Background.DownWriteCnt);
            copyDictCounters(Background.UpFromReadToExclusiveCnt);
            copyDictCounters(Background.DownFromReadToExclusiveCnt);
        }

        private void clearStats()
        {
            clearDictCounters(Background.UpReadCnt);
            clearDictCounters(Background.DownReadCnt);
            clearDictCounters(Background.UpWriteCnt);
            clearDictCounters(Background.DownWriteCnt);
            clearDictCounters(Background.UpFromReadToExclusiveCnt);
            clearDictCounters(Background.DownFromReadToExclusiveCnt);
        }

        class Counter
        {
            public string CallerField;
            public int CountField;
            public int CountFieldTmp;

            public string Caller => CallerField;
            public int Count => CountFieldTmp;
        }

        class StatInfo
        {
            public string Timestamp { get; set; }
            public long OverallUpRead { get; set; }
            public long OverallUpWrite { get; set; }
            public Dictionary<string, List<Counter>> UpRead { get; set; }
            public Dictionary<string, List<Counter>> DownRead { get; set; }
            public Dictionary<string, List<Counter>> UpWrite { get; set; }
            public Dictionary<string, List<Counter>> DownWrite { get; set; }
            public Dictionary<string, List<Counter>> UpFromReadToExclusive { get; set; }
            public Dictionary<string, List<Counter>> DownFromReadToExclusive { get; set; }
        }

        private class Frame
        {
            public readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> UpReadCnt = new ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>>();
            public readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> DownReadCnt = new ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>>();
            public readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> UpWriteCnt = new ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>>();
            public readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> DownWriteCnt = new ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>>();
            public readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> UpFromReadToExclusiveCnt = new ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>>();
            public readonly ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>> DownFromReadToExclusiveCnt = new ConcurrentDictionary<int, ConcurrentDictionary<string, Counter>>();
        }
        
        private readonly struct Metrics
        {
            public Metrics(Prometheus.Counter upRead, Prometheus.Counter downRead, Prometheus.Counter upWrite, Prometheus.Counter downWrite, Prometheus.Counter upFromReadToExclusive, Prometheus.Counter downFromReadToExclusive)
            {
                UpRead = upRead;
                DownRead = downRead;
                UpWrite = upWrite;
                DownWrite = downWrite;
                UpFromReadToExclusive = upFromReadToExclusive;
                DownFromReadToExclusive = downFromReadToExclusive;
            }

            public Prometheus.Counter UpRead { get; }
            public Prometheus.Counter DownRead { get; }
            public Prometheus.Counter UpWrite { get; }
            public Prometheus.Counter DownWrite { get; }
            public Prometheus.Counter UpFromReadToExclusive { get; }
            public Prometheus.Counter DownFromReadToExclusive { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics(Prometheus.Counter.Child upRead, Prometheus.Counter.Child downRead, Prometheus.Counter.Child upWrite, Prometheus.Counter.Child downWrite, Prometheus.Counter.Child upFromReadToExclusive, Prometheus.Counter.Child downFromReadToExclusive)
            {
                UpRead = upRead;
                DownRead = downRead;
                UpWrite = upWrite;
                DownWrite = downWrite;
                UpFromReadToExclusive = upFromReadToExclusive;
                DownFromReadToExclusive = downFromReadToExclusive;
            }

            public Prometheus.Counter.Child UpRead { get; }
            public Prometheus.Counter.Child DownRead { get; }
            public Prometheus.Counter.Child UpWrite { get; }
            public Prometheus.Counter.Child DownWrite { get; }
            public Prometheus.Counter.Child UpFromReadToExclusive { get; }
            public Prometheus.Counter.Child DownFromReadToExclusive { get; }
        }
        
        private readonly struct Key : IEquatable<Key>
        {
            public Key(int typeId, string callerName)
            {
                TypeId = typeId;
                CallerName = callerName;
            }

            public int TypeId { get; }

            public string CallerName { get; }

            public bool Equals(Key other)
            {
                return TypeId == other.TypeId && CallerName == other.CallerName;
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (TypeId * 397) ^ (CallerName != null ? CallerName.GetHashCode() : 0);
                }
            }
        }
    }
}
