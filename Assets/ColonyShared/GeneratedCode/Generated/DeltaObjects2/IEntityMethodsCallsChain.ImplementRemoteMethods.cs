// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public interface IEntityMethodsCallsChainImplementRemoteMethods
    {
        System.Threading.Tasks.Task<SharedCode.EntitySystem.ChainCalls.ChainBlockBase> GetCurrentChainBlockImpl();
        System.Threading.Tasks.Task<SharedCode.Entities.Core.NextEntityToCallResult> TryGetNextEntityToCallImpl();
        System.Threading.Tasks.Task<SharedCode.Entities.Core.IEntityMethodsCallsChain> CreateForkImpl(int index);
        System.Threading.Tasks.Task ForkFinishedImpl(System.Guid forkId);
        System.Threading.Tasks.Task<string> GetDescriptionImpl();
        System.Threading.Tasks.Task SetNextTimeToCallImpl(long nextTimeToCall);
        System.Threading.Tasks.Task IncrementCurrentChainIndexImpl();
        System.Threading.Tasks.Task DecrementCurrentChainIndexImpl();
    }
}