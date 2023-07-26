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
    public class EntityCreatedStatistics : CoreBaseStatistic
    {
        ConcurrentDictionary<Type, EntityCreatedStat> _currentStats =
            new ConcurrentDictionary<Type, EntityCreatedStat>();

        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-EntityCreatedStatistics");
        private readonly MetricsStorage<string, LabeledMetrics, Metrics> _metricStorage;

        public EntityCreatedStatistics()
        {
            var metrics = new Metrics(Prometheus.Metrics.CreateCounter("entity_created", "entity_created", "entity"));

            _metricStorage = new MetricsStorage<string, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) => new LabeledMetrics(rawMetrics.Created.WithLabels(key)));
        }

        protected override void LogStatistics()
        {
            var stats = _currentStats;
            _currentStats = new ConcurrentDictionary<Type, EntityCreatedStat>();
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

        EntityCreatedStat getStat(Type entityType)
        {
            EntityCreatedStat result = null;
            if (!_currentStats.TryGetValue(entityType, out result))
            {
                lock (_currentStats)
                {
                    if (!_currentStats.TryGetValue(entityType, out result))
                    {
                        result = new EntityCreatedStat
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
        public void Created(Type entityType)
        {
            CreatedInternal(entityType);
        }

        private void CreatedInternal(Type entityType)
        {
            var metric = _metricStorage.GetOrCreate(entityType.Name);
            metric.Created.Inc();
            
            var stat = getStat(entityType);
            Interlocked.Increment(ref stat.CountField);
        }

        public class StatInfo
        {
            public string Timestamp { get; set; }
            public long Overall { get; set; }
            public List<EntityCreatedStat> TopUsages { get; set; }
        }

        public class EntityCreatedStat
        {
            public string EntityField;
            public long CountField; //outdoing

            public string Entity => EntityField;
            public long Count => CountField;
        }

        private readonly struct Metrics
        {
            public Metrics(Counter created)
            {
                Created = created;
            }

            public Counter Created { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics(Counter.Child created)
            {
                Created = created;
            }

            public Counter.Child Created { get; }
        }
    }
}