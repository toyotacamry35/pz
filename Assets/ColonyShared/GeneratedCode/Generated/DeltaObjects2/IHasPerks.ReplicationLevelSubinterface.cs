// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1257999303, typeof(SharedCode.Entities.IHasPerks))]
    public interface IHasPerksAlways : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -198692982, typeof(SharedCode.Entities.IHasPerks))]
    public interface IHasPerksClientBroadcast : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1708389874, typeof(SharedCode.Entities.IHasPerks))]
    public interface IHasPerksClientFullApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1046440307, typeof(SharedCode.Entities.IHasPerks))]
    public interface IHasPerksClientFull : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.ITemporaryPerksClientFull TemporaryPerks
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.IPermanentPerksClientFull PermanentPerks
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.ISavedPerksClientFull SavedPerks
        {
            get;
        }
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1979804947, typeof(SharedCode.Entities.IHasPerks))]
    public interface IHasPerksServerApi : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 402720931, typeof(SharedCode.Entities.IHasPerks))]
    public interface IHasPerksServer : SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        GeneratedCode.DeltaObjects.ReplicationInterfaces.ITemporaryPerksServer TemporaryPerks
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.IPermanentPerksServer PermanentPerks
        {
            get;
        }

        GeneratedCode.DeltaObjects.ReplicationInterfaces.ISavedPerksServer SavedPerks
        {
            get;
        }
    }
}