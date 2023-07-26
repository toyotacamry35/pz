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
    public class EntityCountStatisticsManager : CoreBaseStatistic
    {
        private readonly ConcurrentDictionary<IEntitiesRepositoryExtension, EntityCountStatistics> _statistics = new ConcurrentDictionary<IEntitiesRepositoryExtension, EntityCountStatistics>();

        public EntityCountStatistics GetOrAdd(IEntitiesRepositoryExtension repo, EntityCountStatisticsPrometheus statisticsPrometheus) =>
            _statistics.GetOrAdd(repo, (newId) => new EntityCountStatistics(repo, statisticsPrometheus));

        protected override void LogStatistics()
        {
            foreach (var statistics in _statistics)
                statistics.Value.LogStatistics();
        }
    }

    public class EntityCountStatistics
    {
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-EntityCounts");

        private IEntitiesRepositoryExtension _repo;
        private readonly EntityCountStatisticsPrometheus _statisticsPrometheus;

        public readonly struct TypeCount
        {
            public string Type { get; }
            public int Count { get; }

            public TypeCount(Type type, int count)
            {
                Type = type.Name;
                Count = count;
            }
        }

        private readonly List<TypeCount> _countsCache = new List<TypeCount>();

        public EntityCountStatistics(IEntitiesRepositoryExtension repo, EntityCountStatisticsPrometheus statisticsPrometheus)
        {
            _repo = repo;
            _statisticsPrometheus = statisticsPrometheus;
        }

        private void Visit(Type type, int count)
        {
            _countsCache.Add(new TypeCount(type, count));
            _statisticsPrometheus.SetCount(((IEntitiesRepository)_repo).Id, type, count);
        }

        public void LogStatistics()
        {
            _countsCache.Clear();
            _repo.VisitEntityCounts(Visit);
            var totalCount = _countsCache.Sum(v => v.Count);
            var counts = _countsCache.Where(v => v.Count > 1).OrderByDescending(v => v.Count).ToList();

            _overallStatisticsLog.Info().Message(
                "Repo {repo_id}: " +
                "total entity count {total_entity_count}",
                ((IEntitiesRepository)_repo).Id,
                totalCount)
                .Property("entity_count_by_type", counts)
                .Write();

            _countsCache.Clear();
        }
    }
}


public class EntityCountStatisticsPrometheus
{
    private static readonly Gauge EntityCounts = Metrics.CreateGauge("entity_counts", "number of entities of type", new GaugeConfiguration
    {
        LabelNames = new[] { "repository_id", "entity_type" }
    });

    private readonly ConcurrentDictionary<(Guid repoId, Type entityType), Gauge.Child> Submetrics = new ConcurrentDictionary<(Guid repoId, Type entityType), Gauge.Child>();

    [Conditional("ENABLE_NETWORK_STATISTICS")]
    public void SetCount(Guid repoId, Type entityType, int count)
    {
        Submetrics.GetOrAdd((repoId, entityType), (key) => EntityCounts.WithLabels(key.repoId.ToString(), key.entityType.Name)).Set(count);
    }
}