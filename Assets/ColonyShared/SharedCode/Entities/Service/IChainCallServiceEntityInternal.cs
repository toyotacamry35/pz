using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.Entities.Core;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server, addedByDefaultToNodeType: CloudNodeType.Server | CloudNodeType.Client)]
    public interface IChainCallServiceEntityInternal : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task ChainCall(IEntityMethodsCallsChain chainCall);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task AddExistingChainCalls(List<IEntityMethodsCallsChain> chainCalls, int typeId, Guid entityId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task ChainCallBatch(List<IEntityMethodsCallsChain> chainCalls);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task CancelChain(int typeId, Guid entityId, Guid chainId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> CancelAllChain(int typeId, Guid entityId);
    }
}
