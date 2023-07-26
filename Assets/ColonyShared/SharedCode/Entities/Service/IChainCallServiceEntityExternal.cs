using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Network;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server | CloudNodeType.Client, addedByDefaultToNodeType: CloudNodeType.Server | CloudNodeType.Client)]
    public interface IChainCallServiceEntityExternal : IEntity
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task ChainCall(EntityMethodsCallsChainBatch batch);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task CancelChain(int typeId, Guid entityId, Guid chainId);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> CancelAllChain(int typeId, Guid entityId);
    }
}
