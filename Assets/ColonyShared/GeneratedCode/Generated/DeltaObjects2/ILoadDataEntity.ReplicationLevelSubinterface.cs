// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 944686597, typeof(SharedCode.Entities.Cloud.ILoadDataEntity))]
    public interface ILoadDataEntityAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -685404337, typeof(SharedCode.Entities.Cloud.ILoadDataEntity))]
    public interface ILoadDataEntityClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1772287839, typeof(SharedCode.Entities.Cloud.ILoadDataEntity))]
    public interface ILoadDataEntityClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 585611292, typeof(SharedCode.Entities.Cloud.ILoadDataEntity))]
    public interface ILoadDataEntityClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -353117569, typeof(SharedCode.Entities.Cloud.ILoadDataEntity))]
    public interface ILoadDataEntityServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 65145692, typeof(SharedCode.Entities.Cloud.ILoadDataEntity))]
    public interface ILoadDataEntityServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task<byte[]> LoadEntity(int typeId, System.Guid entityId);
    }
}