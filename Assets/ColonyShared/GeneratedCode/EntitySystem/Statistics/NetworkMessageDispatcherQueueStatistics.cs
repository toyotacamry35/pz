using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using GeneratedCode.Network.Statistic;
using NLog;
using NLog.Fluent;
using Prometheus;
using SharedCode.Cloud;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class NetworkMessageDispatcherQueueStatistics : CoreBaseStatistic
    {
        private const int SkipMinimumCountValue = 10;

        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-NetworkMessageQueueStatistics");
        private static readonly Counter _messagesReceived = Prometheus.Metrics.CreateCounter("network_message_received", "Count of network messages received", "repository_id", "communication_node_type");
        private static readonly Counter _messagesProcessed = Prometheus.Metrics.CreateCounter("network_message_processed", "Count of network messages processed", "repository_id", "communication_node_type");

        private readonly ConcurrentDictionary<(Guid repoId, CloudNodeType commNodeType), NetworkMessageDispatcherQueueSize> _objects =
            new ConcurrentDictionary<(Guid repoId, CloudNodeType commNodeType), NetworkMessageDispatcherQueueSize>();

        public NetworkMessageDispatcherQueueStatistics()
        {
        }

        protected override float PeriodSeconds { get; } = 1f;

        protected override void LogStatistics()
        {
            var longQueues = _objects
                .Select(v => new QueueInfo(v.Key.repoId, v.Key.commNodeType, Volatile.Read(ref v.Value.Incoming)))
                .Where(v => v.QueueLength > SkipMinimumCountValue)
                .OrderByDescending(v => v.QueueLength)
                .ToList();
            
            _overallStatisticsLog.Info().Message("Long Queue count: {long_queue_count}", longQueues.Count).Property("long_queues", longQueues).Write();
        }

        public void Register(NetworkMessageDispatcherQueueSize queueSize)
        {
            if (!_objects.TryAdd((queueSize.Id, queueSize.CommunicationNodeType), queueSize))
                throw new InvalidOperationException($"NetworkMessageDispatcherQueueSize is already registered for {queueSize.Id}/{queueSize.CommunicationNodeType}");
        }

        public void Unregister(NetworkMessageDispatcherQueueSize queueSize)
        {
            _objects.TryRemove((queueSize.Id, queueSize.CommunicationNodeType), out _);
        }

        readonly struct QueueInfo
        {
            public QueueInfo(Guid repositoryId, CloudNodeType communicationNodeType, long queueLength)
            {
                RepositoryId = repositoryId;
                CommunicationNodeType = communicationNodeType;
                QueueLength = queueLength;
            }

            public Guid RepositoryId { get; }
            public CloudNodeType CommunicationNodeType { get; }
            public long QueueLength { get; }
        }

        public class NetworkMessageDispatcherQueueSize
        {
            public Guid Id { get; } //Remote repository type and id

            public CloudNodeType CommunicationNodeType { get; }

            private readonly Counter.Child _received;
            private readonly Counter.Child _processed;

            public long Incoming;
            public long Processed;

            public NetworkMessageDispatcherQueueSize(Guid repoId, CloudNodeType commNodeType)
            {
                Id = repoId;
                CommunicationNodeType = commNodeType;
                _received = _messagesReceived.WithLabels(repoId.ToString(), commNodeType.ToString());
                _processed = _messagesProcessed.WithLabels(repoId.ToString(), commNodeType.ToString());
            }

            public void MessageIncoming()
            {
                _received.Inc();

                Interlocked.Increment(ref Incoming);
            }

            public void MessageProcessed()
            {
                _processed.Inc();

                Interlocked.Increment(ref Processed);
                Interlocked.Decrement(ref Incoming);
            }
        }
    }
}
