using System;
using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.Manual.AsyncStack;
using ResourceSystem.Utils;
using SharedCode.Repositories;

namespace SharedCode.EntitySystem
{
    public static class EntitiesContainerExts
    {
        public static T Get<T>(this IEntitiesContainer container, Guid entityId) where T : IEntity
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(T));
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(T));
            return container.Get<T>(typeId, entityId, replicationLevel);
        }

        public static bool TryGet<T>(this IEntitiesContainer container, Guid entityId, out T entity) where T : IEntity
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(T));
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(T));
            return container.TryGet(typeId, entityId, replicationLevel, out entity);
        }

        public static T Get<T>(this IEntitiesContainer container, int typeId, Guid entityId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(type);
            return container.Get<T>(typeId, entityId, replicationLevel);
        }

        public static bool TryGet<T>(this IEntitiesContainer container, int typeId, Guid entityId, out T entity)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(type);
            return container.TryGet(typeId, entityId, replicationLevel, out entity);
        }

        public static bool TryGet<T>(this IEntitiesContainer container, OuterRef<T> entOuterRef, out T entity)
        {
            var type = ReplicaTypeRegistry.GetTypeById(entOuterRef.TypeId);
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(type);
            return container.TryGet(entOuterRef.TypeId, entOuterRef.Guid, replicationLevel, out entity);
        }

        public static bool TryGet<T>(this IEntitiesContainer container, OuterRef entOuterRef, out T entity)
        {
            var type = ReplicaTypeRegistry.GetTypeById(entOuterRef.TypeId);
            var replicationLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(type);
            return container.TryGet(entOuterRef.TypeId, entOuterRef.Guid, replicationLevel, out entity);
        }

        public static bool TryGet<T>(this IEntitiesContainer container, OuterRef entOuterRef, ReplicationLevel replicationLevel, out T entity)
        {
            return container.TryGet(entOuterRef.TypeId, entOuterRef.Guid, replicationLevel, out entity);
        }
 
        public static T Get<T>(this IEntitiesContainer container, OuterRef<T> entOuterRef, ReplicationLevel replicationLevel)
        {
            return container.Get<T>(entOuterRef.TypeId, entOuterRef.Guid, replicationLevel);
        }

        public static T Get<T>(this IEntitiesContainer container, OuterRef entOuterRef, ReplicationLevel replicationLevel)
        {
            return container.Get<T>(entOuterRef.TypeId, entOuterRef.Guid, replicationLevel);
        }

        public static T Get<T>(this IEntitiesContainer container, OuterRef<T> outerRef)
        {
            return container.Get<T>(outerRef.TypeId, outerRef.Guid);
        }

        public static T Get<T>(this IEntitiesContainer container, OuterRef outerRef)
        {
            return container.Get<T>(outerRef.TypeId, outerRef.Guid);
        }

        public static T GetMasterService<T>(this IEntitiesContainer container) where T : IEntity
        {
            return Get<T>(container, ((EntitiesContainer)container).Context.RepositoryId);
        }

        public static bool TryGetMasterService<T>(this IEntitiesContainer container, out T entity) where T : IEntity
        {
            return TryGet<T>(container, ((EntitiesContainer)container).Context.RepositoryId, out entity);
        }

        public static bool TryGetMasterService(this IEntitiesContainer container, Type type, out IEntity entity)
        {
            var masterType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            var masterTypeId = ReplicaTypeRegistry.GetIdByType(masterType);

            return TryGet(container, masterTypeId, ((EntitiesContainer)container).Context.RepositoryId, out entity);
        }

        public static T GetFirstService<T>(this IEntitiesContainer container) where T : IEntity
        {
            var masterTypeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(typeof(T)));

            var entityId = GetEntityId(container, masterTypeId);
            if (entityId == default)
                return default;

            return Get<T>(container, entityId);
        }

        private static Guid GetEntityId(IEntitiesContainer container, int masterTypeId)
        {
            foreach (var parentContainer in AsyncStackEnumerable.ToParents((EntitiesContainer) container))
            {
                if (parentContainer.Batch != null)
                {
                    foreach (var batch in parentContainer.Batch.Items)
                    {
                        if (batch.EntityMasterTypeId == masterTypeId)
                        {
                            return batch.EntityId;
                        }
                    }
                }
            }

            return default;
        }

        public static IEntity GetFirstService(this IEntitiesContainer container, Type type, ReplicationLevel level)
        {
            var masterType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            var masterTypeId = ReplicaTypeRegistry.GetIdByType(masterType);
            var entityId = AsyncStackEnumerable.ToParents((EntitiesContainer)container).Where(v => v.Batch != null).SelectMany(v => v.Batch.Items).Where(x => x.EntityMasterTypeId == masterTypeId).Select(v => v.EntityId).FirstOrDefault();
            if (entityId == default)
                return default;

            return container.Get<IEntity>(masterTypeId, entityId, level);
        }

        public static Task<T> GetOrSubscribe<T>(this IEntitiesContainer container, Guid entityId)
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(T));
            return container.GetOrSubscribe<T>(typeId, entityId, ReplicaTypeRegistry.GetReplicationLevelByReplicaType(typeof(T)));
        }

        public static Task<T> GetOrSubscribe<T>(this IEntitiesContainer container, int typeId, Guid entityId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            return container.GetOrSubscribe<T>(typeId, entityId, ReplicaTypeRegistry.GetReplicationLevelByReplicaType(type));
        }


    }
}
