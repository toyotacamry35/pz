// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 944625009, typeof(SharedCode.Entities.IHasStatistics))]
    public interface IHasStatisticsAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Aspects.Item.Templates.PerkActionsPricesDef PerkActionsPrices
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -112089805, typeof(SharedCode.Entities.IHasStatistics))]
    public interface IHasStatisticsClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.Aspects.Item.Templates.PerkActionsPricesDef PerkActionsPrices
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1717495647, typeof(SharedCode.Entities.IHasStatistics))]
    public interface IHasStatisticsClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -677512687, typeof(SharedCode.Entities.IHasStatistics))]
    public interface IHasStatisticsClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Aspects.Item.Templates.BaseItemResource, int> PerksDestroyCount
        {
            get;
        }

        SharedCode.Aspects.Item.Templates.PerkActionsPricesDef PerkActionsPrices
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1872536724, typeof(SharedCode.Entities.IHasStatistics))]
    public interface IHasStatisticsServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -522655001, typeof(SharedCode.Entities.IHasStatistics))]
    public interface IHasStatisticsServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.ColonyShared.SharedCode.Aspects.Statictic.StatisticType, SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.ColonyShared.SharedCode.Aspects.Statictic.StatisticType, float>> Statistics
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Aspects.Item.Templates.BaseItemResource, int> PerksDestroyCount
        {
            get;
        }

        SharedCode.Aspects.Item.Templates.PerkActionsPricesDef PerkActionsPrices
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.IStatisticEngineServer StatisticEngine
        {
            get;
        }

        System.Threading.Tasks.Task ChangeStatistic(Assets.ColonyShared.SharedCode.Aspects.Statictic.StatisticType statistic, Assets.ColonyShared.SharedCode.Aspects.Statictic.StatisticType target, float value);
    }
}