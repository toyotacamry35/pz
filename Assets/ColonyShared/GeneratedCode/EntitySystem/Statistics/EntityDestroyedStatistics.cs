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
using GeneratedCode.Network.Statistic;
using NLog;
using Prometheus;
using SharedCode.Monitoring;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class EntityDestroyedStatistics : CoreBaseStatistic
    {
        ConcurrentDictionary<Type, EntityDestroyedStat> _currentStats = new ConcurrentDictionary<Type, EntityDestroyedStat>();

        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-EntityDestroyedStatistics");
        private readonly MetricsStorage<string, LabeledMetrics, Metrics> _metricStorage;

        public EntityDestroyedStatistics()
        {
            var metrics = new Metrics(Prometheus.Metrics.CreateCounter("entity_destroyed", "entity_destroyed", "entity"));

            _metricStorage = new MetricsStorage<string, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) => new LabeledMetrics(rawMetrics.Destroyed.WithLabels(key)));
        }

        protected override void LogStatistics()
        {
            var stats = _currentStats;
            _currentStats = new ConcurrentDictionary<Type, EntityDestroyedStat>();
            Thread.Sleep(5);//ждем, вдруг из другого потока прямо сейчас хотят дописать

            var count = stats.Values.Count;
            if (count == 0)
                return;

            var overall = stats.Values.Sum(x => x.CountField);
            var topUsagesStats = stats.Values.OrderByDescending(x => x.Count).ToList();

            _overallStatisticsLog.IfInfo()?.Message("Overall {overall}, Top usages {@top_usages}",
                overall.ToString(),
                topUsagesStats
            ).Write();
        }

        EntityDestroyedStat getStat(Type entityType)
        {
            EntityDestroyedStat result = null;
            if (!_currentStats.TryGetValue(entityType, out result))
            {
                lock (_currentStats)
                {
                    if (!_currentStats.TryGetValue(entityType, out result))
                    {
                        result = new EntityDestroyedStat
                        {
                            EntityField = entityType.Name
                        };
                        _currentStats[entityType] = result;
                    }
                }
            }
            return result;
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void Destroyed(Type entityType)
        {
            DestroyedInternal(entityType);
        }

        private void DestroyedInternal(Type entityType)
        {
            var metrics = _metricStorage.GetOrCreate(entityType.Name);
            metrics.Destroyed.Inc();
            
            var stat = getStat(entityType);
            Interlocked.Increment(ref stat.CountField);
        }

        public class StatInfo
        {
            public string Timestamp { get; set; }
            public long Overall { get; set; }
            public List<EntityDestroyedStat> TopUsages { get; set; }
        }

        public class EntityDestroyedStat
        {
            public string EntityField;
            public long CountField;//outdoing

            public string Entity => EntityField;
            public long Count => CountField;
        }
        
        private readonly struct Metrics
        {
            public Metrics(Counter destroyed)
            {
                Destroyed = destroyed;
            }

            public Counter Destroyed { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics(Counter.Child destroyed)
            {
                Destroyed = destroyed;
            }

            public Counter.Child Destroyed { get; }
        }
    }
}
