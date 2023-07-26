using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneratedCode.Network.Statistic;
using NLog;
using NLog.Fluent;
using Prometheus;
using SharedCode.EntitySystem;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class EntityQueueSizeStatisticsManager : CoreBaseStatistic
    {
        private readonly ConcurrentDictionary<IEntitiesRepositoryExtension, EntityQueueSizeStatistics> _statistics = new ConcurrentDictionary<IEntitiesRepositoryExtension, EntityQueueSizeStatistics>();

        public EntityQueueSizeStatistics GetOrAdd(IEntitiesRepositoryExtension repo, EntityQueueSizeStatisticsPrometheus statisticsPrometheus) =>
            _statistics.GetOrAdd(repo, (newId) => new EntityQueueSizeStatistics(repo, statisticsPrometheus));

        protected override void LogStatistics()
        {
            foreach (var statistics in _statistics)
                statistics.Value.LogStatistics();
        }
    }

    public class EntityQueueSizeStatistics
    {
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-EntityQueueSize");

        private IEntitiesRepositoryExtension _repo;
        private readonly EntityQueueSizeStatisticsPrometheus _statisticsPrometheus;

        public readonly struct TypeCount
        {
            public string Type { get; }
            public int Count { get; }

            public Guid Id { get; }

            public TypeCount(Type type, Guid id, int count)
            {
                Type = type.Name;
                Count = count;
                Id = id;
            }
        }

        private readonly Dictionary<Type, (Guid id, int count)> _countsCache = new Dictionary<Type, (Guid id, int count)>();

        public EntityQueueSizeStatistics(IEntitiesRepositoryExtension repo, EntityQueueSizeStatisticsPrometheus statisticsPrometheus)
        {
            _repo = repo;
            _statisticsPrometheus = statisticsPrometheus;
        }

        private void Visit(Type type, Guid id, int count)
        {
            if (!_countsCache.TryGetValue(type, out var existing) || count > existing.count)
                _countsCache[type] = (id, count);
        }

        public void LogStatistics()
        {
            _countsCache.Clear();
            _repo.VisitEntityQueueLengths(Visit);
            foreach(var entry in _countsCache)
                _statisticsPrometheus.SetCount(((IEntitiesRepository)_repo).Id, entry.Key, entry.Value.count);

            var lengths = _countsCache.Where(v => v.Value.count > 0).OrderByDescending(v => v.Value).Select(v => new TypeCount(v.Key, v.Value.id, v.Value.count)).ToList();
            var maxLen = lengths.FirstOrDefault();

            if (maxLen.Type != null)
            {
                _overallStatisticsLog.Info().Message(
                    "Repo {repo_id}: " +
                    "longest queue: Type {longest_queue_type}, " +
                    "Id {longest_queue_id}, " +
                    "length {longest_queue_length}",
                    ((IEntitiesRepository)_repo).Id,
                    maxLen.Type,
                    maxLen.Id,
                    maxLen.Count)
                    .Property("max_queue_length_by_type", lengths)
                    .Write();
            }

            _countsCache.Clear();
        }
    }
}


public class EntityQueueSizeStatisticsPrometheus
{
    private static readonly Gauge EntityQueueSize = Metrics.CreateGauge("entity_queue_size", "Max Length of entity wait queue", new GaugeConfiguration
    {
        LabelNames = new[] { "repository_id", "entity_type" }
    });

    private readonly ConcurrentDictionary<(Guid repoId, Type entityType), Gauge.Child> Submetrics = new ConcurrentDictionary<(Guid repoId, Type entityType), Gauge.Child>();

    [Conditional("ENABLE_NETWORK_STATISTICS")]
    public void SetCount(Guid repoId, Type entityType, int count)
    {
        Submetrics.GetOrAdd((repoId, entityType), (key) => EntityQueueSize.WithLabels(key.repoId.ToString(), key.entityType.Name)).Set(count);
    }
}