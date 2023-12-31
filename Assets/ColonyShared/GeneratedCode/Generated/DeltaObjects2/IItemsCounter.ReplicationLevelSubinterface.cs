// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1122453459, typeof(GeneratedCode.DeltaObjects.IItemsCounter))]
    public interface IItemsCounterAlways : SharedCode.EntitySystem.IDeltaObject, IQuestCounterAlways, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -1458457020, typeof(GeneratedCode.DeltaObjects.IItemsCounter))]
    public interface IItemsCounterClientBroadcast : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientBroadcast, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -2071443315, typeof(GeneratedCode.DeltaObjects.IItemsCounter))]
    public interface IItemsCounterClientFullApi : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientFullApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 317536152, typeof(GeneratedCode.DeltaObjects.IItemsCounter))]
    public interface IItemsCounterClientFull : SharedCode.EntitySystem.IDeltaObject, IQuestCounterClientFull, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1064480195, typeof(GeneratedCode.DeltaObjects.IItemsCounter))]
    public interface IItemsCounterServerApi : SharedCode.EntitySystem.IDeltaObject, IQuestCounterServerApi, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1230605277, typeof(GeneratedCode.DeltaObjects.IItemsCounter))]
    public interface IItemsCounterServer : SharedCode.EntitySystem.IDeltaObject, IQuestCounterServer, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task SetCount(int count);
    }
}