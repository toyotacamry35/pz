// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 219108451, typeof(SharedCode.DeltaObjects.ITemporaryPerks))]
    public interface ITemporaryPerksAlways : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksAlways, IItemsContainerAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1894710470, typeof(SharedCode.DeltaObjects.ITemporaryPerks))]
    public interface ITemporaryPerksClientBroadcast : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksClientBroadcast, IItemsContainerClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -861384004, typeof(SharedCode.DeltaObjects.ITemporaryPerks))]
    public interface ITemporaryPerksClientFullApi : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksClientFullApi, IItemsContainerClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 257188621, typeof(SharedCode.DeltaObjects.ITemporaryPerks))]
    public interface ITemporaryPerksClientFull : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksClientFull, IItemsContainerClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 115977981, typeof(SharedCode.DeltaObjects.ITemporaryPerks))]
    public interface ITemporaryPerksServerApi : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksServerApi, IItemsContainerServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1833057390, typeof(SharedCode.DeltaObjects.ITemporaryPerks))]
    public interface ITemporaryPerksServer : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksServer, IItemsContainerServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }
}