using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server, addedByDefaultToNodeType: CloudNodeType.None)]
    public interface IClusterAddressResolverServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        ValueTask<Guid> GetEntityAddressRepositoryId(int typeId, Guid entityId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        ValueTask<IReadOnlyList<(Guid entityId, Guid repoId)>> GetAllEntitiesByType(int typeId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task SetEntityAddressRepositoryId(int typeId, Guid entityId, Guid repositoryId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task SetEntitiesAddressRepositoryId(Dictionary<int, Guid> entities, Guid repositoryId);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task RemoveEntityAddressRepositoryId(int typeId, Guid entityId);
    }
}
