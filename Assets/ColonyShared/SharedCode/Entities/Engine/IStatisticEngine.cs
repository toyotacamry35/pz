using GeneratorAnnotations;
using SharedCode.EntitySystem;
using SharedCode.Quests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedCode.Entities.Engine
{

    [GenerateDeltaObjectCode]
    public interface IStatisticEngine : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        event Func<StatisticEventArgs, Task> StatisticsEvent;

        [ReplicationLevel(ReplicationLevel.Server)]
        Task PostStatisticsEvent(StatisticEventArgs arg);
    }

    public interface IHasStatisticsRouterInternal
    {
        IDictionary<Type, IStatisticRouter> Routers { get; }
    }

    public interface IStatisticRouter
    {
        Task InvokeRoutedEvent(StatisticEventArgs arg, IEntitiesRepository repo);
    }
}
