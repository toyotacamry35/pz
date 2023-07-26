using SharedCode.Repositories;
using System;

namespace SharedCode.EntitySystem.EntityPropertyResolvers
{
    public static class EntityPropertyResolver
    {
        public static T Resolve<T>(IEntity entity, PropertyAddress address) where T : IDeltaObject
        {
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(T));
            return Resolve<T>(entity, address, replicationLevel);
        }

        public static T Resolve<T>(IEntity entity, PropertyAddress address, ReplicationLevel replicationLevel)
        {
            if (entity == null)
                throw new ArgumentException("EntityPropertyResolver entity is null", nameof(entity));

            IEntity masterEntity;
            if (entity is IBaseDeltaObjectWrapper wrap)
                masterEntity = (IEntity)wrap.GetBaseDeltaObject();
            else
                masterEntity = entity;

            if (address.EntityId != masterEntity.Id || address.EntityTypeId != masterEntity.TypeId)
                throw new InvalidOperationException($"Property address {address} does not belong to entity {entity}");

            IDeltaObject obj = ((IEntityExt)masterEntity).ResolveDeltaObject(address.DeltaObjectLocalId);

            if (address.DeltaObjectFieldId != -1)
            {
                obj.GetReplicationLevel(replicationLevel).TryGetProperty<T>(address.DeltaObjectFieldId, out var prop);
                return prop;
            }
            else
                return (T)obj.GetReplicationLevel(replicationLevel);
        }

        public static PropertyAddress GetPropertyAddress(IDeltaObject deltaObject)
        {
            TryGetPropertyAddress(deltaObject, out var address);
            return address;
        }

        public static bool TryGetPropertyAddress(IDeltaObject deltaObject, out PropertyAddress address)
        {
            address = null;
            IDeltaObject deltaObjectMaster;
            if (deltaObject is IBaseDeltaObjectWrapper)
                deltaObjectMaster = ((IBaseDeltaObjectWrapper)deltaObject).GetBaseDeltaObject();
            else
                deltaObjectMaster = deltaObject;

            if (!(deltaObjectMaster is IDeltaObjectExt))
                throw new ArgumentException("incorrect deltaObject");

            var parentEntity = ((IDeltaObjectExt)deltaObjectMaster).GetParentEntity();
            if (parentEntity == null)
                return false;

            var localId = ((IDeltaObjectExt)deltaObjectMaster).LocalId;

            address = new PropertyAddress
            {
                EntityId = parentEntity.Id,
                EntityTypeId = parentEntity.TypeId,
                DeltaObjectLocalId = localId
            };
            return true;
        }
    }
}
