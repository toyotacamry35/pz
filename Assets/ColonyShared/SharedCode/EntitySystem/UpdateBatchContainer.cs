using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratedCode.Manual.Repositories;
using JetBrains.Annotations;
using ProtoBuf;
using ResourceSystem.Utils;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public class UpdateBatchContainer
    {
        [ProtoMember(1)]
        public List<UpdateBatch> Batches = new List<UpdateBatch>();
    }
    
    [ProtoContract]
    public class UpdateBatch
    {
        [UsedImplicitly]
        public UpdateBatch()
        {
        }

        public UpdateBatch(
            int entityTypeId,
            Guid entityId, 
            Dictionary<ulong, byte[]> snapshot, 
            long replicationMask,
            int version,
            int previousVersion,
            List<DeferredEntityModel> deferredEntities)
        {
            EntityTypeId = entityTypeId;
            EntityId = entityId;
            Snapshot = snapshot;
            ReplicationMask = replicationMask;
            Version = version;
            PreviousVersion = previousVersion;
            DeferredEntities = deferredEntities;
        }

        [ProtoMember(1)] public int EntityTypeId { get; [UsedImplicitly] set; }

        [ProtoMember(2)] public Guid EntityId { get; [UsedImplicitly] set; }

        [ProtoMember(3)] public Dictionary<ulong, byte[]> Snapshot { get; [UsedImplicitly] set; }

        [ProtoMember(4)] public long ReplicationMask { get; [UsedImplicitly] set; }

        [ProtoMember(5)] public int Version { get; [UsedImplicitly] set; }

        [ProtoMember(6)] public int PreviousVersion { get; [UsedImplicitly] set; }

        [ProtoMember(7)] public List<DeferredEntityModel> DeferredEntities { get; [UsedImplicitly] set; }
    }
}
