// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -695703614, typeof(SharedCode.DeltaObjects.IPermanentPerks))]
    public interface IPermanentPerksAlways : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksAlways, IItemsContainerAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1747166435, typeof(SharedCode.DeltaObjects.IPermanentPerks))]
    public interface IPermanentPerksClientBroadcast : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksClientBroadcast, IItemsContainerClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1094449653, typeof(SharedCode.DeltaObjects.IPermanentPerks))]
    public interface IPermanentPerksClientFullApi : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksClientFullApi, IItemsContainerClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -2099536701, typeof(SharedCode.DeltaObjects.IPermanentPerks))]
    public interface IPermanentPerksClientFull : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksClientFull, IItemsContainerClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -212925210, typeof(SharedCode.DeltaObjects.IPermanentPerks))]
    public interface IPermanentPerksServerApi : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksServerApi, IItemsContainerServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1477061316, typeof(SharedCode.DeltaObjects.IPermanentPerks))]
    public interface IPermanentPerksServer : SharedCode.EntitySystem.IDeltaObject, ICharacterPerksServer, IItemsContainerServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }
}