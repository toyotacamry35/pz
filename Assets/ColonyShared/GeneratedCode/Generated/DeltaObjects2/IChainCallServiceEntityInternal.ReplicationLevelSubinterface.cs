// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, -950704389, typeof(SharedCode.Entities.Service.IChainCallServiceEntityInternal))]
    public interface IChainCallServiceEntityInternalAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 1622427650, typeof(SharedCode.Entities.Service.IChainCallServiceEntityInternal))]
    public interface IChainCallServiceEntityInternalClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, -1207078443, typeof(SharedCode.Entities.Service.IChainCallServiceEntityInternal))]
    public interface IChainCallServiceEntityInternalClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, -720000455, typeof(SharedCode.Entities.Service.IChainCallServiceEntityInternal))]
    public interface IChainCallServiceEntityInternalClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1718203894, typeof(SharedCode.Entities.Service.IChainCallServiceEntityInternal))]
    public interface IChainCallServiceEntityInternalServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 1708334123, typeof(SharedCode.Entities.Service.IChainCallServiceEntityInternal))]
    public interface IChainCallServiceEntityInternalServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task ChainCall(GeneratedCode.DeltaObjects.ReplicationInterfaces.IEntityMethodsCallsChainServer chainCall);
        System.Threading.Tasks.Task ChainCallBatch(System.Collections.Generic.List<SharedCode.Entities.Core.IEntityMethodsCallsChain> chainCalls);
        System.Threading.Tasks.Task CancelChain(int typeId, System.Guid entityId, System.Guid chainId);
        System.Threading.Tasks.Task<bool> CancelAllChain(int typeId, System.Guid entityId);
    }
}