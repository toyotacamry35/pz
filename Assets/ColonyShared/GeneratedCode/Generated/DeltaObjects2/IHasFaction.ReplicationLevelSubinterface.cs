// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 1787496064, typeof(ColonyShared.SharedCode.Entities.IHasFaction))]
    public interface IHasFactionAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -2124363202, typeof(ColonyShared.SharedCode.Entities.IHasFaction))]
    public interface IHasFactionClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Assets.Src.Aspects.Impl.Factions.Template.FactionDef Faction
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 535456109, typeof(ColonyShared.SharedCode.Entities.IHasFaction))]
    public interface IHasFactionClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1424347835, typeof(ColonyShared.SharedCode.Entities.IHasFaction))]
    public interface IHasFactionClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Assets.Src.Aspects.Impl.Factions.Template.FactionDef Faction
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 489761791, typeof(ColonyShared.SharedCode.Entities.IHasFaction))]
    public interface IHasFactionServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1514913262, typeof(ColonyShared.SharedCode.Entities.IHasFaction))]
    public interface IHasFactionServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        Assets.Src.Aspects.Impl.Factions.Template.FactionDef Faction
        {
            get;
        }
    }
}