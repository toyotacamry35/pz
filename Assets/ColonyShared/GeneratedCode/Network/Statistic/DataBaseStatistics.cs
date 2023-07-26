using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NLog;
using NLog.Fluent;
using Prometheus;
using SharedCode.Monitoring;

namespace GeneratedCode.Network.Statistic
{
    public class DataBaseStatistics : CoreBaseStatistic
    {
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-DataBaseStatistics");

        private ConcurrentDictionary<string, StatInfoItem> _currentStats = new ConcurrentDictionary<string, StatInfoItem>();
        private readonly MetricsStorage<string, LabeledMetrics, Metrics> _metricStorage;

        public DataBaseStatistics()
        {
            var metrics = new Metrics(Prometheus.Metrics.CreateCounter("database_save", "database_save","entity"));

            _metricStorage = new MetricsStorage<string, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) => new LabeledMetrics(rawMetrics.Save.WithLabels(key)));
        }

        protected override float PeriodSeconds { get; } = 60f;
        
        public void SaveEntity(string entityName, int count)
        {
            SaveEntityInternal(entityName, count);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void SaveEntityInternal(string entityName, int count)
        {
            var metrics = _metricStorage.GetOrCreate(entityName);
            metrics.Save.Inc(count);
            var stat = _currentStats.GetOrAdd(entityName, x => new StatInfoItem(x));
            stat.Add(count);
        }

        protected override void LogStatistics()
        {
            var stats = _currentStats;
            _currentStats = new ConcurrentDictionary<string, StatInfoItem>();
            Thread.Sleep(5);//ждем, вдруг из другого потока прямо сейчас хотят дописать в старый SpellCastStat
            
            var overall = stats.Values.Sum(x => x.Count);

            var topUsages = stats.Values.OrderByDescending(x => x.Count).ToList();

            _overallStatisticsLog.Info().Message("Overall {overall}", overall).Property("db_top_usages", topUsages).Write();
        }

        class StatInfoItem
        {
            long CountField;//outdoing

            public string Name { get; }
            public long Count => CountField;

            public StatInfoItem(string name)
            {
                Name = name;
            }

            public void Add(int count)
            {
                Interlocked.Add(ref CountField, count);
            }
        }

        class StatInfo
        {
            public string Timestamp { get; set; }
            public long Overall { get; set; }
            public List<StatInfoItem> Items { get; set; }
        }
        
        private readonly struct Metrics
        {
            public Metrics(Counter save)
            {
                Save = save;
            }

            public Counter Save { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics( Counter.Child save)
            {
                Save = save;
            }

            public Counter.Child Save { get; }
        }
    }
}
