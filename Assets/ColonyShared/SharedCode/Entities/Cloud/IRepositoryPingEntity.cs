using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities.Cloud
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server, addedByDefaultToNodeType: CloudNodeType.Server)]
    public interface IRepositoryPingEntity : IEntity
    {
        [RemoteMethod(MessageSendOptions.ReliableOrdered, 5)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> Ping();

        [RemoteMethod(MessageSendOptions.ReliableOrdered, 5)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task PingRepository(Guid repositoryId);
    }
}
