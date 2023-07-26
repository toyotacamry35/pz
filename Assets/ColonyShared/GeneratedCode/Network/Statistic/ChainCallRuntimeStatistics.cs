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
    public class ChainCallRuntimeStatistics : CoreBaseStatistic
    {
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-ChainCallRuntimeStatistics");
        private const int TopCount = 50;
        ConcurrentDictionary<(Type type, byte messageId), Stat> _currentStats = new ConcurrentDictionary<(Type type, byte messageId), Stat>();
        private readonly MetricsStorage<string, LabeledMetrics, Metrics> _metricStorage;

        public ChainCallRuntimeStatistics()
        {
            var metrics = new Metrics(
                Prometheus.Metrics.CreateCounter("chain_call_before_get", "repository_wait_time", "message"),
                Prometheus.Metrics.CreateCounter("chain_call_execute", "repository_use_time", "message")
            );

            _metricStorage = new MetricsStorage<string, LabeledMetrics, Metrics>(
                metrics, (messageName, rawMetrics) => new LabeledMetrics(
                    rawMetrics.Execute.WithLabels(messageName),
                    rawMetrics.BeforeGet.WithLabels(messageName)));
        }

        protected override void LogStatistics()
        {
            var stats = _currentStats;
            _currentStats = new ConcurrentDictionary<(Type type, byte messageId), Stat>();
            Thread.Sleep(5);//ждем, вдруг из другого потока прямо сейчас хотят дописать в старый _currentStats

            var count = stats.Values.Count;
            var takeCount = Math.Min(TopCount, count);
            var overallBeforeGet = stats.Values.Sum(x => x.BeforeGetField);
            var overallExecute = stats.Values.Sum(x => x.ExecuteField);
            var topUsagesStats = stats.Values.OrderByDescending(x => x.BeforeGetField + x.ExecuteField).Take(takeCount).ToList();

            _overallStatisticsLog.IfInfo()?.Message("Overall before get {overall_before_get}, " +
                                                       "overall execute {overall_execute}, " +
                                                       "top usages {@top_usages}",
                overallBeforeGet,
                overallExecute,
                topUsagesStats)
                .Write();
        }

        Stat getStat(Type type, byte methodId)
        {
            return _currentStats.GetOrAdd((type, methodId), (key) =>
            {
                var message = ReplicaTypeRegistry.GetDispatcher(key.type, key.messageId).messageName;
                return new Stat
                {
                    Metrics = _metricStorage.GetOrCreate(message),
                    MessageTypeField = message
                };
            });
        }


        public void BeforeGet(Type type, byte methodId)
        {
            BeforeGetInternal(type, methodId);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void BeforeGetInternal(Type type, byte methodId)
        {
            var stat = getStat(type, methodId);
            stat.Metrics.BeforeGet.Inc();
            Interlocked.Increment(ref stat.BeforeGetField);
        }

        public void Execute(Type type, byte methodId)
        {
            ExecuteInternal(type, methodId);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void ExecuteInternal(Type type, byte methodId)
        {
            var stat = getStat(type, methodId);
            stat.Metrics.Execute.Inc();
            Interlocked.Increment(ref stat.ExecuteField);
        }

        class Stat
        {
            public string MessageTypeField;
            public long BeforeGetField;
            public long ExecuteField;
            public LabeledMetrics Metrics;

            public string MessageType => MessageTypeField;
            public long BeforeGet => BeforeGetField;
            public long Execute => ExecuteField;
        }

        class StatInfo
        {
            public string Timestamp { get; set; }
            public long OverallBeforeGet { get; set; }
            public long OverallExecute { get; set; }
            public List<Stat> TopUsages { get; set; }
        }
        
        private readonly struct Metrics
        {
            public Metrics(Counter beforeGet, Counter execute)
            {
                BeforeGet = beforeGet;
                Execute = execute;
            }

            public Counter BeforeGet { get; }
            public Counter Execute { get; }
        }
        
        private readonly struct LabeledMetrics
        {
            public LabeledMetrics(Counter.Child beforeGet, Counter.Child execute)
            {
                BeforeGet = beforeGet;
                Execute = execute;
            }

            public Counter.Child BeforeGet { get; }
            public Counter.Child Execute { get; }
        }
    }
}
