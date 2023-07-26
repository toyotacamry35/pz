// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -1200370976, typeof(SharedCode.Entities.Cloud.IDatabaseServiceEntity))]
    public interface IDatabaseServiceEntityAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.ValueTask<bool> DataSetExists(int typeId, System.Guid entityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -623399863, typeof(SharedCode.Entities.Cloud.IDatabaseServiceEntity))]
    public interface IDatabaseServiceEntityClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.ValueTask<bool> DataSetExists(int typeId, System.Guid entityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1278226712, typeof(SharedCode.Entities.Cloud.IDatabaseServiceEntity))]
    public interface IDatabaseServiceEntityClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1391693818, typeof(SharedCode.Entities.Cloud.IDatabaseServiceEntity))]
    public interface IDatabaseServiceEntityClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.ValueTask<bool> DataSetExists(int typeId, System.Guid entityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1657949954, typeof(SharedCode.Entities.Cloud.IDatabaseServiceEntity))]
    public interface IDatabaseServiceEntityServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 60642858, typeof(SharedCode.Entities.Cloud.IDatabaseServiceEntity))]
    public interface IDatabaseServiceEntityServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        SharedCode.EntitySystem.DatabaseServiceType DatabaseServiceType
        {
            get;
        }

        System.Threading.Tasks.Task<SharedCode.Entities.Cloud.SerializedEntityBatch> Load(int typeId, System.Guid entityId);
        System.Threading.Tasks.ValueTask<bool> DataSetExists(int typeId, System.Guid entityId);
        System.Threading.Tasks.Task<System.Guid> GetAccountIdByName(string accountName);
    }
}