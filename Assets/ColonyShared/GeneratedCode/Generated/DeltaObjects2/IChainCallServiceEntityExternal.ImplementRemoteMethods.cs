// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IChainCallServiceEntityExternalImplementRemoteMethods
    {
        System.Threading.Tasks.Task ChainCallImpl(SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch batch);
        System.Threading.Tasks.Task CancelChainImpl(int typeId, System.Guid entityId, System.Guid chainId);
        System.Threading.Tasks.Task<bool> CancelAllChainImpl(int typeId, System.Guid entityId);
    }
}