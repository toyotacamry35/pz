using System;
using System.Threading.Tasks;
using GeneratedCode.Custom.Config;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface IWorldNodeServiceEntity : IEntity
    {

        [ReplicationLevel(ReplicationLevel.Server)]
        EndpointAddress ExternalAddress { get; }


        [ReplicationLevel(ReplicationLevel.Server)]
        MapDef Map { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Guid MapInstanceId { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Guid MapChunkId { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        WorldNodeServiceState State { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        bool ClientNode { get; }

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> IsReady();
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> HostUnityMapChunk(MapDef mapChunk);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> HostUnityMapChunk(MapDef mapChunk, Guid mapChunkId, Guid mapInstanceId, Guid mapInstanceRepositoryId);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> HostedUnityMapChunk(MapDef mapChunk, Guid mapChunkId, Guid mapInstanceId);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> SetState(WorldNodeServiceState state);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> InitializePorts();

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        ValueTask<bool> CanBuildHere(IEntityObjectDef entityObjectDef, OuterRef<IEntity> ent, Vector3 position, Vector3 scale, Quaternion rotation);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Lockfree)]
        ValueTask<Vector3> GetDropPosition(Vector3 playerPosition, Quaternion playerRotation);
    }

    public enum WorldNodeServiceState
    {
        None,
        Loading,
        Loaded,
        Unloading,
        Empty,
        Failed
    }
}
