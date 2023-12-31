// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -646300101, typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal))]
    public interface IChainCallServiceEntityExternalAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -566236888, typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal))]
    public interface IChainCallServiceEntityExternalClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 1902069113, typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal))]
    public interface IChainCallServiceEntityExternalClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 343532554, typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal))]
    public interface IChainCallServiceEntityExternalClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task ChainCall(SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch batch);
        System.Threading.Tasks.Task CancelChain(int typeId, System.Guid entityId, System.Guid chainId);
        System.Threading.Tasks.Task<bool> CancelAllChain(int typeId, System.Guid entityId);
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, -1697024442, typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal))]
    public interface IChainCallServiceEntityExternalServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, -1040174502, typeof(SharedCode.Entities.Service.IChainCallServiceEntityExternal))]
    public interface IChainCallServiceEntityExternalServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task ChainCall(SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch batch);
        System.Threading.Tasks.Task CancelChain(int typeId, System.Guid entityId, System.Guid chainId);
        System.Threading.Tasks.Task<bool> CancelAllChain(int typeId, System.Guid entityId);
    }
}