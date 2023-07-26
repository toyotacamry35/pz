using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.Environment.Logging.Extension;
using GeneratedCode.Network.Statistic;
using NLog;
using Prometheus;
using SharedCode.Monitoring;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class EntityLoadStatistics : CoreBaseStatistic
    {
        ConcurrentDictionary<Type, EntityLoadStat> _currentStats = new ConcurrentDictionary<Type, EntityLoadStat>();

        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-EntityLoadStatistics");
        private readonly MetricsStorage<string, LabeledMetrics, Metrics> _metricStorage;

        public EntityLoadStatistics()
        {
            var metrics =
                new Metrics(Prometheus.Metrics.CreateCounter("entity_loaded", "entity_loaded", "entity"));

            _metricStorage = new MetricsStorage<string, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) => new LabeledMetrics(rawMetrics.Loaded.WithLabels(key)));
        }

        protected override void LogStatistics()
        {
            var stats = _currentStats;
            _currentStats = new ConcurrentDictionary<Type, EntityLoadStat>();
            Thread.Sleep(5); //ждем, вдруг из другого потока прямо сейчас хотят дописать

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

        EntityLoadStat getStat(Type entityType)
        {
            EntityLoadStat result = null;
            if (!_currentStats.TryGetValue(entityType, out result))
            {
                lock (_currentStats)
                {
                    if (!_currentStats.TryGetValue(entityType, out result))
                    {
                        result = new EntityLoadStat
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
        public void Load(Type entityType)
        {
            LoadInternal(entityType);
        }

        private void LoadInternal(Type entityType)
        {
            var metrics = _metricStorage.GetOrCreate(entityType.Name);
            metrics.Loaded.Inc();
            
            var stat = getStat(entityType);
            Interlocked.Increment(ref stat.CountField);
        }

        public class StatInfo
        {
            public string Timestamp { get; set; }
            public long Overall { get; set; }
            public List<EntityLoadStat> TopUsages { get; set; }
        }

        public class EntityLoadStat
        {
            public string EntityField;
            public long CountField; //outdoing

            public string Entity => EntityField;
            public long Count => CountField;
        }

        private readonly struct Metrics
        {
            public Metrics(Counter loaded)
            {
                Loaded = loaded;
            }

            public Counter Loaded { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics(Counter.Child loaded)
            {
                Loaded = loaded;
            }

            public Counter.Child Loaded { get; }
        }
    }
}
