using System;
using System.Collections.Generic;
using GeneratedCode.Manual.Repositories;
using JetBrains.Annotations;
using ProtoBuf;
using ResourceSystem.Utils;

namespace SharedCode.EntitySystem
{

    [ProtoContract]
    public class UploadBatchContainer
    {
        [ProtoMember(1)]
        public List<UploadBatch> Batches = new List<UploadBatch>();
    }

    [ProtoContract]
    public class UploadBatch
    {
        [UsedImplicitly]
        public UploadBatch()
        {
        }
        
        public UploadBatch(int entityTypeId, Guid entityId, Dictionary<ulong, byte[]> snapshot, long replicationMask, int version, List<DeferredEntityModel> deferredEntities)
        {
            EntityTypeId = entityTypeId;
            EntityId = entityId;
            Snapshot = snapshot;
            ReplicationMask = replicationMask;
            Version = version;
            DeferredEntities = deferredEntities;
        }
        
        [ProtoMember(1)] public int EntityTypeId { get; [UsedImplicitly]set; }

        [ProtoMember(2)] public Guid EntityId { get; [UsedImplicitly]set; }

        [ProtoMember(3)] public Dictionary<ulong, byte[]> Snapshot { get; [UsedImplicitly]set; }

        [ProtoMember(4)] public long ReplicationMask { get; [UsedImplicitly]set; }

        [ProtoMember(5)] public int Version { get; [UsedImplicitly]set; }

        [ProtoMember(6)] public List<DeferredEntityModel> DeferredEntities { get; [UsedImplicitly]set; }
    }
}
