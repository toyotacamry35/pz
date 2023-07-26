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
using GeneratedCode.Repositories;
using NLog;
using Prometheus;
using SharedCode.Monitoring;
using SharedCode.Repositories;

namespace GeneratedCode.Network.Statistic
{
    public class EntityUploadStatistics : CoreBaseStatistic
    {
        ConcurrentDictionary<int, Stat> _currentSendedStats = new ConcurrentDictionary<int, Stat>();

        ConcurrentDictionary<int, Stat> _currentSendedStats2 = new ConcurrentDictionary<int, Stat>();

        ConcurrentDictionary<int, Stat> _currentReceivedStats = new ConcurrentDictionary<int, Stat>();

        ConcurrentDictionary<int, Stat> _currentReceivedStats2 = new ConcurrentDictionary<int, Stat>();

        private const int TopCount = 20;

        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-EntityUploadStatistics");

        private int useSecondDict = 0;
        private readonly MetricsStorage<int, LabeledMetrics, Metrics> _metricStorage;

        public EntityUploadStatistics()
        {
            var metrics = new Metrics(Prometheus.Metrics.CreateSummary("entity_upload_sent", "entity_upload_sent", "entitytid"),
                Prometheus.Metrics.CreateSummary("entity_upload_received", "entity_upload_received", "entitytid"));

            _metricStorage = new MetricsStorage<int, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) =>
                {
                    var type = ReplicaTypeRegistry.GetTypeById(key);
                    return new LabeledMetrics(rawMetrics.Sent.WithLabels(type.Name),
                        rawMetrics.Received.WithLabels(type.Name)
                    );
                });
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
            var sendedStats = useSecondDict == 0 ? _currentSendedStats2 :  _currentSendedStats;
            var receivedStats = useSecondDict == 0 ? _currentReceivedStats2 : _currentReceivedStats;

            var overallSendCount = sendedStats.Values.Sum(x => x.CountFieldTmp);
            var overallReceiveCount = receivedStats.Values.Sum(x => x.CountFieldTmp);
            var overallSendBytes = sendedStats.Values.Sum(x => x.BytesFieldTmp);
            var overallReceiveBytes = receivedStats.Values.Sum(x => x.BytesFieldTmp);
            var topSendedBytesStats = sendedStats.Values.Where(x => x.CountFieldTmp > 0).OrderByDescending(x => x.BytesFieldTmp).Take(TopCount).ToList();
            var topReceivedBytesStats = receivedStats.Values.Where(x => x.CountFieldTmp > 0).OrderByDescending(x => x.BytesFieldTmp).Take(TopCount).ToList();

            _overallStatisticsLog.IfInfo()?.Message("Overall send count {overall_send_count}, " +
                                                              "overall send bytes {overall_send_bytes}, " +
                                                              "overall receive count {overall_receive_count}, " +
                                                              "overall receive bytes {overall_receive_bytes}, " +
                                                              "top sent bytes {@top_sent_bytes}, " +
                                                              "top received bytes {@top_received_bytes}",
                overallSendCount,
                overallSendBytes,
                overallReceiveCount,
                overallReceiveBytes,
                topSendedBytesStats,
                topReceivedBytesStats)
                .Write();
        }

        private void copyDictCounters(ConcurrentDictionary<int, Stat> dictionary)
        {
            foreach (var stat in dictionary)
            {
                Interlocked.Exchange(ref stat.Value.CountFieldTmp, stat.Value.CountField);
                Interlocked.Exchange(ref stat.Value.BytesFieldTmp, stat.Value.BytesField);
            }
        }

        private void clearDictCounters(ConcurrentDictionary<int, Stat> dictionary)
        {
            foreach (var stat in dictionary)
            {
                Interlocked.Exchange(ref stat.Value.CountField, 0);
                Interlocked.Exchange(ref stat.Value.BytesField, 0);
            }
        }

        private void copyStats()
        {
            if (useSecondDict == 0)
            {
                copyDictCounters(_currentSendedStats2);
                copyDictCounters(_currentReceivedStats2);
            }
            else
            {
                copyDictCounters(_currentSendedStats);
                copyDictCounters(_currentReceivedStats);
            }
        }

        private void clearStats()
        {
            if (useSecondDict == 0)
            {
                clearDictCounters(_currentSendedStats2);
                clearDictCounters(_currentReceivedStats2);
            }
            else
            {
                clearDictCounters(_currentSendedStats);
                clearDictCounters(_currentReceivedStats);
            }
        }

        private void swapStatistricsDict()
        {
            if (useSecondDict == 0)
                Interlocked.Exchange(ref useSecondDict, 1);
            else
                Interlocked.Exchange(ref useSecondDict, 0);
        }

        Stat getStat(ConcurrentDictionary<int, Stat> dictionary, int entityType)
        {
            Stat result = null;
            if (!dictionary.TryGetValue(entityType, out result))
            {
                lock (dictionary)
                {
                    if (!dictionary.TryGetValue(entityType, out result))
                    {
                        result = new Stat
                        {
                            EntityTypeField = ReplicaTypeRegistry.GetTypeById(entityType).Name
                        };
                        dictionary[entityType] = result;
                    }
                }
            }
            return result;
        }

        public void EntityUploadSend(int typeId, long bytesCount)
        {
            EntityUploadSendInternal(typeId, bytesCount);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void EntityUploadSendInternal(int typeId, long bytesCount)
        {
            var metrics = _metricStorage.GetOrCreate(typeId);
            metrics.Sent.Observe(bytesCount);
            
            var stat = getStat(useSecondDict == 0 ? _currentSendedStats : _currentSendedStats2, typeId);
            Interlocked.Increment(ref stat.CountField);
            Interlocked.Add(ref stat.BytesField, bytesCount);
        }

        public void EntityUploadReceived(int typeId, long bytesCount)
        {
            EntityUploadReceivedInternal(typeId, bytesCount);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void EntityUploadReceivedInternal(int typeId, long bytesCount)
        {
            var metrics = _metricStorage.GetOrCreate(typeId);
            metrics.Received.Observe(bytesCount);
            
            var stat = getStat(useSecondDict == 0 ? _currentReceivedStats : _currentReceivedStats2, typeId);
            Interlocked.Increment(ref stat.CountField);
            Interlocked.Add(ref stat.BytesField, bytesCount);
        }

        class Stat
        {
            public string EntityTypeField;
            public long CountField;
            public long BytesField;
            public long CountFieldTmp;
            public long BytesFieldTmp;

            public string EntityType => EntityTypeField;
            public long Count => CountFieldTmp;
            public long Bytes => BytesFieldTmp;
        }

        class StatInfo
        {
            public string Timestamp { get; set; }
            public long OverallSendCount { get; set; }
            public long OverallSendBytes { get; set; }
            public long OverallReceiveCount { get; set; }
            public long OverallReceiveBytes { get; set; }
            public List<EntityUploadStatistics.Stat> TopSendedBytes { get; set; }
            public List<EntityUploadStatistics.Stat> TopReceivedBytes { get; set; }
        }
        
        private readonly struct Metrics
        {
            public Metrics(Summary sent, Summary received)
            {
                Sent = sent;
                Received = received;
            }

            public Summary Sent { get; }
            
            public Summary Received { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics(Summary.Child sent, Summary.Child received)
            {
                Sent = sent;
                Received = received;
            }

            public Summary.Child Sent { get; }
            
            public Summary.Child Received { get; }
        }
    }
}
