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
using NLog;
using Prometheus;
using SharedCode.Monitoring;
using SharedCode.Repositories;
using SharedCode.Wizardry;

namespace GeneratedCode.Network.Statistic
{
    public class SpellCastStatistics : CoreBaseStatistic
    {
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-SpellCastStatistics");

        ConcurrentDictionary<SpellDef, SpellCastStat> _currentStats = new ConcurrentDictionary<SpellDef, SpellCastStat>();
        private readonly MetricsStorage<SpellDef, LabeledMetrics, Metrics> _metricStorage;

        private const int TopCount = 20;

        public SpellCastStatistics()
        {
            var metrics = new Metrics(Prometheus.Metrics.CreateCounter("spell_used", "spell_used", "spell_id"),
                Prometheus.Metrics.CreateCounter("spell_casted", "spell_casted", "spell_id"));

            _metricStorage = new MetricsStorage<SpellDef, LabeledMetrics, Metrics>(metrics,
                (spellId, rawMetrics) => new LabeledMetrics(rawMetrics.Used.WithLabels(spellId.ToString()),
                    rawMetrics.Casted.WithLabels(spellId.ToString())
                ));
        }
        
        protected override void LogStatistics()
        {
            var stats = _currentStats;
            _currentStats = new ConcurrentDictionary<SpellDef, SpellCastStat>();
            Thread.Sleep(5);//ждем, вдруг из другого потока прямо сейчас хотят дописать в старый SpellCastStat

            var count = stats.Values.Count;
            var takeCount = Math.Min(TopCount, count);
            var overallUsed = stats.Values.Sum(x => x.UsedField);
            var overallCasted = stats.Values.Sum(x => x.CastedField);
            var topUsagesStats = stats.Values.OrderByDescending(x => x.UsedField + x.CastedField).Take(takeCount).ToList();

            _overallStatisticsLog.IfInfo()?.Message("Used overall {used_overall}, " +
                                                       "cast overall {cast_overall}, " +
                                                       "top usages {@top_usages}",
                overallUsed.ToString(),
                overallCasted.ToString(),
                topUsagesStats
            ).Write();
        }

        SpellCastStat getStat(SpellDef spellId)
        {
            SpellCastStat result = null;
            if (!_currentStats.TryGetValue(spellId, out result))
            {
                lock (_currentStats)
                {
                    if (!_currentStats.TryGetValue(spellId, out result))
                    {
                        result = new SpellCastStat
                        {
                            SpellIdField = spellId
                        };
                        _currentStats[spellId] = result;
                    }
                }
            }
            return result;
        }

        public void Casted(SpellDef spellId)
        {
            CastedInternal(spellId);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void CastedInternal(SpellDef spellId)
        {
            var metrics = _metricStorage.GetOrCreate(spellId);
            metrics.Casted.Inc();
            
            var stat = getStat(spellId);
            Interlocked.Increment(ref stat.CastedField);
        }

        public void Used(SpellDef spellId)
        {
            UsedInternal(spellId);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void UsedInternal(SpellDef spellId)
        {
            var metrics = _metricStorage.GetOrCreate(spellId);
            metrics.Used.Inc();
            
            var stat = getStat(spellId);
            Interlocked.Increment(ref stat.UsedField);
        }

        class StatInfo
        {
            public string Timestamp { get; set; }
            public long UsedOverall { get; set; }
            public long CastedOverall { get; set; }
            public List<SpellCastStat> TopUsages { get; set; }
        }
        
            private readonly struct Metrics
            {
                public Metrics(Counter used, Counter casted)
                {
                    Used = used;
                    Casted = casted;
                }

                public Counter Used { get; }
                public Counter Casted { get; }
            }
        
            public readonly struct LabeledMetrics
            {
                public LabeledMetrics(Counter.Child used, Counter.Child casted)
                {
                    Used = used;
                    Casted = casted;
                }

                public Counter.Child Used { get; }
                public Counter.Child Casted { get; }
            }
    }
}
