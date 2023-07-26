using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;

namespace SharedCode.Entities.Cloud
{
    [ProtoContract]
    public class SerializedEntityBatch
    {
        [ProtoMember(1)]
        public List<SerializedEntityData> Batches { get; set; } = new List<SerializedEntityData>();
    }

    [ProtoContract]
    public class SerializedEntityData
    {
        [ProtoMember(1)]
        public int EntityTypeId { get; set; }

        [ProtoMember(2)]
        public Guid EntityId { get; set; }

        [ProtoMember(3)]
        public Dictionary<ulong, byte[]> Snapshot { get; set; }

        [ProtoMember(5)]
        public string Version { get; set; }
    }

    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.Server)]
    public interface IDatabaseServiceEntity : IEntity
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        [LockFreeReadonlyProperty]
        DatabaseServiceType DatabaseServiceType { get; }

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        [RemoteMethod(30)]
        Task<SerializedEntityBatch> Load(int typeId, Guid entityId);

        ValueTask<bool> DataSetExists(int typeId, Guid entityId);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task Save();
        [ReplicationLevel(ReplicationLevel.Master)]
        Task CleanCache();

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Server)]
        Task<Guid> GetAccountIdByName(string accountName);
    }
}
