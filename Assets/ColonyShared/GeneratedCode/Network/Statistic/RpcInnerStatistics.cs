using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Repositories;
using NLog;
using Prometheus;
using SharedCode.Monitoring;

namespace GeneratedCode.Network.Statistic
{
    public class RpcInnerStatistics : CoreBaseStatistic
    {
        private ConcurrentDictionary<(Type type, byte msgId), RpcInnerStat> _currentStats =
            new ConcurrentDictionary<(Type type, byte msgId), RpcInnerStat>();

        private const int TopCount = 20;
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-RpcInnerStatistics");
        private readonly MetricsStorage<Key, LabeledMetrics, Metrics> _metricStorage;

        public RpcInnerStatistics()
        {
            var metrics =
                new Metrics(Prometheus.Metrics.CreateCounter("rpc_inner_count", "rpc_inner_count", "message"));

            _metricStorage = new MetricsStorage<Key, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) =>
                {
                    var messageName = ReplicaTypeRegistry.GetDispatcher(key.Type, key.MessageId).messageName;
                    return new LabeledMetrics(rawMetrics.Count.WithLabels(messageName));
                });
        }

        protected override void LogStatistics()
        {
            var stats = _currentStats;
            _currentStats = new ConcurrentDictionary<(Type type, byte msgId), RpcInnerStat>();
            Thread.Sleep(5); //ждем, вдруг из другого потока прямо сейчас хотят дописать в старый RpcStat

            var count = stats.Values.Count;
            if (count == 0)
                return;

            var takeCount = Math.Min(TopCount, count);
            var overall = stats.Values.Sum(x => x.CountField);
            var topUsagesStats = stats.Values.OrderByDescending(x => x.Count).Take(takeCount).ToList();

            _overallStatisticsLog.IfDebug()?.Message(
                "Overall {overall}, " +
                "top usages {@top_usages}",
                overall.ToString(),
                topUsagesStats
            ).Write();
        }

        RpcInnerStat getStat(Type type, byte methodId)
        {
            return _currentStats.GetOrAdd((type, methodId), (key) => new RpcInnerStat
            {
                MessageTypeField = ReplicaTypeRegistry.GetDispatcher(key.type, key.msgId).messageName
            });
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void Used(Type type, byte msgId)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(type, msgId));
            metrics.Count.Inc();
            
            var stat = getStat(type, msgId);
            Interlocked.Increment(ref stat.CountField);
        }

        class StatInfo
        {
            public string Timestamp { get; set; }
            public long Overall { get; set; }
            public List<RpcInnerStat> TopUsages { get; set; }
        }

        private readonly struct Metrics
        {
            public Metrics(Counter count)
            {
                Count = count;
            }

            public Counter Count { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics(Counter.Child count)
            {
                Count = count;
            }

            public Counter.Child Count { get; }
        }
        
        public readonly struct Key : IEquatable<Key>
        {
            public Key(Type type, byte messageId)
            {
                Type = type;
                MessageId = messageId;
            }

            public Type Type { get; }
            
            public byte MessageId { get; }
            
            public bool Equals(Key other)
            {
                return Equals(Type, other.Type) && MessageId == other.MessageId;
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ MessageId.GetHashCode();
                }
            }
        }
    }
}
