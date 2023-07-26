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
    public class RpcNetworkStatistics : CoreBaseStatistic
    {
        ConcurrentDictionary<(Type type, byte messageId), RpcStat> _currentStats =
            new ConcurrentDictionary<(Type type, byte messageId), RpcStat>();

        private long _unknownExceptionCount;
        private long _unknownCompletionCount;
        private long _unknownExceptionBytes;
        private long _unknownCompletionBytes;
        private const int TopCount = 10;

        private readonly MetricsStorage<Key, LabeledMetrics, Metrics> _metricStorage;
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-RpcStatistics");

        private readonly Summary _receivedUnknown;
        private readonly Summary _receivedExceptionUnknown;

        public RpcNetworkStatistics()
        {
            _receivedUnknown =
                Prometheus.Metrics.CreateSummary("rpc_network_received_unknown", "rpc_network_received_unknown");
            _receivedExceptionUnknown = Prometheus.Metrics.CreateSummary("rpc_network_received_unknown_exception",
                "rpc_network_received_unknown_exception");

            var metrics = new Metrics(
                Prometheus.Metrics.CreateSummary("rpc_network_sent", "rpc_network_sent", "message"),
                Prometheus.Metrics.CreateSummary("rpc_network_received", "rpc_network_received", "message"),
                Prometheus.Metrics.CreateSummary("rpc_network_received_response", "rpc_network_received_response", "message"),
                Prometheus.Metrics.CreateSummary("rpc_network_sent_response", "rpc_network_sent_response", "message")
            );

            _metricStorage = new MetricsStorage<Key, LabeledMetrics, Metrics>(metrics,
                (key, rawMetrics) =>
                {
                    var messageName = ReplicaTypeRegistry.GetDispatcher(key.Type, key.MethodId).messageName;
                    return new LabeledMetrics(rawMetrics.Sent.WithLabels(messageName),
                        rawMetrics.Received.WithLabels(messageName),
                        rawMetrics.ReceivedResponse.WithLabels(messageName),
                        rawMetrics.SentResponse.WithLabels(messageName)
                    );
                });
        }

        protected override void LogStatistics()
        {
            var stats = _currentStats;
            var unknownExceptionCount = Interlocked.Exchange(ref _unknownExceptionCount, 0);
            var unknownCompletionCount = Interlocked.Exchange(ref _unknownCompletionCount, 0);
            var unknownExceptionBytes = Interlocked.Exchange(ref _unknownExceptionBytes, 0);
            var unknownCompletionBytes = Interlocked.Exchange(ref _unknownCompletionBytes, 0);
            _currentStats = new ConcurrentDictionary<(Type type, byte messageId), RpcStat>();
            Thread.Sleep(5); //ждем, вдруг из другого потока прямо сейчас хотят дописать в старый RpcStat

            var overallSend = stats.Values.Sum(x => x.SentCount);
            var overallReceive = stats.Values.Sum(x => x.ReceiveCount);
            var topIncomingBytesStats = stats.Values.OrderByDescending(x => x.ReceiveBytes + x.SentResponseBytes)
                .Take(TopCount).ToList();
            var topOutdoingBytesStats = stats.Values.OrderByDescending(x => x.SentBytes + x.ReceiveResponseBytes)
                .Take(TopCount).ToList();
            var topIncomingUsagesCountStats =
                stats.Values.OrderByDescending(x => x.ReceiveCount).Take(TopCount).ToList();
            var topOutdoingUsagesCountStats = stats.Values.OrderByDescending(x => x.SentCount).Take(TopCount).ToList();

            _overallStatisticsLog.IfInfo()?.Message(
                "Overall Send: {overall_send_count}, " +
                "overall receive: {overall_receive_count}, " +
                "unknown exception count {unknown_exception_count}, " +
                "unknown completion count {unknown_completion_count}, " +
                "unknown exception bytes {unknown_exception_bytes}, " +
                "unknown completion bytes {unknown_completion_bytes}, " +
                "top incoming bytes {@top_incoming_bytes}," +
                "top outgoing bytes {@top_outgoing_bytes}, " +
                "top incoming usages {@top_incoming_usages}, " +
                "top outgoing usages {@top_outgoing_usages}", 
                overallSend,
                overallReceive,
                unknownExceptionCount,
                unknownCompletionCount,
                unknownExceptionBytes,
                unknownCompletionBytes,
                topIncomingBytesStats,
                topOutdoingBytesStats,
                topIncomingUsagesCountStats,
                topOutdoingUsagesCountStats
                )
                .Write();
        }

        RpcStat getStat(Type type, byte methodId)
        {
            return _currentStats.GetOrAdd((type, methodId), (key) => new RpcStat
            {
                MessageTypeField = ReplicaTypeRegistry.GetDispatcher(key.type, key.messageId).messageName
            });
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void SentBytes(Type type, byte methodId, long bytesCount)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(type, methodId));
            metrics.Sent.Observe(bytesCount);
            
            var stat = getStat(type, methodId);
            Interlocked.Add(ref stat.SentBytesField, bytesCount);
            Interlocked.Increment(ref stat.SentCountField);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void ReceiveResponseBytes(Type type, byte methodId, long bytesCount)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(type, methodId));
            metrics.ReceivedResponse.Observe(bytesCount);
            
            var stat = getStat(type, methodId);
            Interlocked.Add(ref stat.ReceiveResponseBytesField, bytesCount);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void ReceiveResponseUnknownExceptionBytes(long bytesCount)
        {
            _receivedExceptionUnknown.Observe(bytesCount);
            
            Interlocked.Increment(ref _unknownExceptionCount);
            Interlocked.Add(ref _unknownExceptionBytes, bytesCount);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void ReceiveResponseUnknownCompletedBytes(long bytesCount)
        {
            _receivedUnknown.Observe(bytesCount);
            
            Interlocked.Increment(ref _unknownCompletionCount);
            Interlocked.Add(ref _unknownCompletionBytes, bytesCount);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void ReceiveBytes(Type type, byte methodId, long bytesCount)
        {
            var metrics = _metricStorage.GetOrCreate(new Key(type, methodId));
            metrics.Received.Observe(bytesCount);
            
            var stat = getStat(type, methodId);
            Interlocked.Add(ref stat.ReceiveBytesField, bytesCount);
            Interlocked.Increment(ref stat.ReceiveCountField);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        public void SentResponseBytes(Type type, byte methodId, long bytesCount)
        {   
            var metrics = _metricStorage.GetOrCreate(new Key(type, methodId));
            metrics.SentResponse.Observe(bytesCount);
            
            var stat = getStat(type, methodId);
            Interlocked.Add(ref stat.SentResponseBytesField, bytesCount);
        }

        class StatInfo
        {
            public string Timestamp { get; set; }
            public long OverallSendCount { get; set; }
            public long OverallReceiveCount { get; set; }
            public long UnknownExceptionCount { get; set; }
            public long UnknownCompletionCount { get; set; }
            public long UnknownExceptionBytes { get; set; }
            public long UnknownCompletionBytes { get; set; }
            public List<RpcStat> TopIncomingBytes { get; set; }
            public List<RpcStat> TopOutdoingBytes { get; set; }
            public List<RpcStat> TopIncomingUsages { get; set; }
            public List<RpcStat> TopOutdoingUsages { get; set; }
        }

        private readonly struct Metrics
        {
            public Metrics(Summary sent, Summary received, Summary receivedResponse, Summary sentResponse)
            {
                Sent = sent;
                Received = received;
                ReceivedResponse = receivedResponse;
                SentResponse = sentResponse;
            }

            public Summary Sent { get; }
            public Summary Received { get; }
            public Summary ReceivedResponse { get; }
            public Summary SentResponse { get; }
        }

        private readonly struct LabeledMetrics
        {
            public LabeledMetrics(Summary.Child sent, Summary.Child received, Summary.Child receivedResponse, Summary.Child sentResponse)
            {
                Sent = sent;
                Received = received;
                ReceivedResponse = receivedResponse;
                SentResponse = sentResponse;
            }

            public Summary.Child Sent { get; }
            public Summary.Child Received { get; }
            public Summary.Child ReceivedResponse { get; }
            public Summary.Child SentResponse { get; }
        }
        
        private readonly struct Key : IEquatable<Key>
        {
            public Key(Type type, byte methodId)
            {
                Type = type;
                MethodId = methodId;
            }
            
            public Type Type { get; }

            public byte MethodId { get; }

            public bool Equals(Key other)
            {
                return Equals(Type, other.Type) && MethodId == other.MethodId;
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ MethodId.GetHashCode();
                }
            }
        }
    }
}
