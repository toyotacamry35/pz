using System;
using System.Collections.Concurrent;
using Prometheus;
using SharedCode.Monitoring;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class PrometheusLockRepositoryStatisticsFactory : ILockRepositoryStatisticsFactory
    {
        public static ILockRepositoryStatisticsFactory It { get; } = new PrometheusLockRepositoryStatisticsFactory();

        private readonly MetricsStorage<Key, LabeledMetrics, Metrics> _metricStorage;

        private readonly ConcurrentDictionary<Guid, LabeledMetrics[]> _labeledMetrics =
            new ConcurrentDictionary<Guid, LabeledMetrics[]>();

        private static readonly int OperationLength =
            Enum.GetValues(typeof(LockRepositoryStatistics.LockRepositoryOperation)).Length;

        private PrometheusLockRepositoryStatisticsFactory()
        {
            var labelNames = new[] {"rid", "op"};
            var metrics = new Metrics(
                Prometheus.Metrics.CreateSummary("repository_wait_time", "repository_wait_time",
                    new SummaryConfiguration
                    {
                        LabelNames = labelNames
                    }),
                Prometheus.Metrics.CreateSummary("repository_use_time", "repository_use_time",
                    new SummaryConfiguration
                    {
                        LabelNames = labelNames
                    })
            );

            _metricStorage = new MetricsStorage<Key, LabeledMetrics, Metrics>(
                metrics, (key, rawMetrics) => new LabeledMetrics(
                    rawMetrics.WaitTime.WithLabels(key.RepositoryId.ToString(), key.Operation.ToString()),
                    rawMetrics.UseTime.WithLabels(key.RepositoryId.ToString(), key.Operation.ToString())));
        }

        public LockRepositoryStatisticsPrometheus GetOrAdd(Guid repositoryId)
        {
            var metrics = _labeledMetrics.GetOrAdd(repositoryId, (idArg) =>
            {
                var metricsForOperations = new LabeledMetrics[OperationLength];
                for (var index = 0; index < metricsForOperations.Length; index++)
                {
                    var operation = (LockRepositoryStatistics.LockRepositoryOperation) index;
                    metricsForOperations[index] = _metricStorage.GetOrCreate(new Key(idArg, operation));
                }

                return metricsForOperations;
            });
            return new LockRepositoryStatisticsPrometheus(metrics);
        }

        public readonly struct Key : IEquatable<Key>
        {
            public Key(Guid repositoryId, LockRepositoryStatistics.LockRepositoryOperation operation)
            {
                RepositoryId = repositoryId;
                Operation = operation;
            }

            public Guid RepositoryId { get; }

            public LockRepositoryStatistics.LockRepositoryOperation Operation { get; }

            public bool Equals(Key other)
            {
                return RepositoryId.Equals(other.RepositoryId) && Operation == other.Operation;
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (RepositoryId.GetHashCode() * 397) ^ (int) Operation;
                }
            }
        }

        private readonly struct Metrics
        {
            public Metrics(
                Summary waitTime,
                Summary useTime)
            {
                WaitTime = waitTime;
                UseTime = useTime;
            }

            public Summary WaitTime { get; }
            public Summary UseTime { get; }
        }

        public readonly struct LabeledMetrics
        {
            public LabeledMetrics(
                Summary.Child waitTime,
                Summary.Child useTime)
            {
                WaitTime = waitTime;
                UseTime = useTime;
            }

            public Summary.Child WaitTime { get; }
            public Summary.Child UseTime { get; }
        }
    }
}