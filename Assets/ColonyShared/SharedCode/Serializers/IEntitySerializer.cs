using System;
using System.Collections.Generic;
using SharedCode.EntitySystem;

namespace SharedCode.Serializers
{
    public interface IEntitySerializer
    {
        void DeserializeDeltaObjects(IEntity existedEntity, Dictionary<ulong, byte[]> snapshot);

        IEntity DeserializeEntity(IEntitiesRepository repository, int rootEntityTypeId, Guid rootEntityId,
            Dictionary<ulong, byte[]> snapshot);

        Dictionary<ulong, byte[]> SerializeEntityChanged(IEntityExt entity,
            ReplicationLevel replicationLevel,
            long serializeMask);

        Dictionary<ulong, byte[]> SerializeReplicationSetFull(
            Dictionary<IDeltaObject, DeltaObjectReplicationInfo> replicationSet,
            long serializeMask);

        Dictionary<ulong, byte[]> SerializeEntityFull(IEntityExt entity,
            ReplicationLevel newReplicationLevel,
            ReplicationLevel oldReplicationLevel,
            long serializeMask);
    }
}