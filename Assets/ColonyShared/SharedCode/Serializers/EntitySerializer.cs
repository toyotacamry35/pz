using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Serializers.Protobuf;

namespace SharedCode.Serializers
{
    public class EntitySerializer : IEntitySerializer
    {
        private readonly ISerializer _serializer;
        
        public EntitySerializer(ISerializer serializer)
        {
            _serializer = serializer;
        }
        
        public void DeserializeDeltaObjects(IEntity existedEntity, Dictionary<ulong, byte[]> snapshot)
        {
            using (_SerializerContext.Pool.Set())
            {
                _SerializerContext.Pool.Current.Deserialization = true;
                foreach (var deltaObjectSnapshot in snapshot)
                {
                    var existedObject = ((IEntityExt) existedEntity).ResolveDeltaObject(deltaObjectSnapshot.Key);
                    var deserialized = _serializer.Merge(existedObject, deltaObjectSnapshot.Value);

                    // Если у обьекта есть свойства, то он уже будет в ChangedObjects (потому что логика добавления происходит в сетерах десерализации)
                    // если обьект без свойств, то добавляем его тут
                    _SerializerContext.Pool.Current.ChangedObjects.TryAdd(deltaObjectSnapshot.Key,
                        new DeserializedObjectInfo(deserialized, null));
                }

                // связываем свойства
                foreach (var deserializedObject in _SerializerContext.Pool.Current.ChangedObjects)
                {
                    // так как с листом мы посылаем удаленные обьекты
                    // то во время линковки листа, елемент листа может потерять родителя и превратиться в тыкву с LocalId=0
                    // это временный хак, нужно подумать как посылать DeltaList
                    if (((IDeltaObjectExt) deserializedObject.Value.DeltaObject).LocalId != 0)
                    {
                        ((IDeltaObjectExt) deserializedObject.Value.DeltaObject).LinkChangedDeltaObjects(
                            _SerializerContext.Pool.Current.ChangedObjects, existedEntity);
                    }
                }
            }
        }

        public IEntity DeserializeEntity(IEntitiesRepository repository, int rootEntityTypeId, Guid rootEntityId,
            Dictionary<ulong, byte[]> snapshot)
        {
            using (_SerializerContext.Pool.Set())
            {
                _SerializerContext.Pool.Current.Deserialization = true;

                IEntity entity = null;
                foreach (var deltaObjectSnapshot in snapshot)
                {
                    var deserialized = ((IEntitiesRepositoryExtension) repository).Serializer.Merge<IDeltaObject>(
                        null,
                        deltaObjectSnapshot.Value);
                    if (deserialized is IEntity deserializedEntity)
                    {
                        if (deserializedEntity.Id == rootEntityId && deserializedEntity.TypeId == rootEntityTypeId)
                        {
                            entity = deserializedEntity;
                        }
                    }

                    // Если у обьекта есть свойства, то он уже будет в ChangedObjects (потому что логика добавления происходит в сетерах десерализации)
                    // если обьект без свойств, то добавляем его тут
                    // не создаем всегда чтобы не было лишней аллокации на dictionary
                    _SerializerContext.Pool.Current.ChangedObjects.TryAdd(deltaObjectSnapshot.Key,
                        new DeserializedObjectInfo(deserialized, null));
                }

                if (entity == null)
                {
                    throw new Exception(
                        $"Entity wasn't found in snapshot EntityTypeId={rootEntityTypeId} EntityId={rootEntityId}");
                }

                // связываем свойства
                foreach (var deserializedObject in _SerializerContext.Pool.Current.ChangedObjects)
                {
                    ((IDeltaObjectExt) deserializedObject.Value.DeltaObject).LinkChangedDeltaObjects(
                        _SerializerContext.Pool.Current.ChangedObjects, entity);
                }

                return entity;
            }
        }

        public Dictionary<ulong, byte[]> SerializeEntityChanged(IEntityExt entity,
            ReplicationLevel replicationLevel,
            long serializeMask)
        {
            if (!entity.ReplicationSets.TryGetValue(replicationLevel, out var replicationSet))
            {
                var entityReplicationSets = string.Join(", ", entity.ReplicationSets.Select(s => s.Key).ToArray());
                throw new Exception(
                    $"Unknown replicationLevel {replicationLevel}. Entity replicationSets={entityReplicationSets}");
            }
            
            var serializedDeltaObjects = new Dictionary<ulong, byte[]>();

            using (_SerializerContext.Pool.Set())
            {
                _SerializerContext.Pool.Current.ReplicationMask = serializeMask;

                foreach (var changedObject in entity.ChangedObjects)
                {
                    var changedObjectAsExt = (IDeltaObjectExt) changedObject;
                    // так как мы не удаляем из changedObjects обьекты, которые были удалены из дерева
                    if (changedObjectAsExt.LocalId != 0
                        && replicationSet.TryGetValue(changedObject, out var replicationInfo))
                    {
                        _SerializerContext.Pool.Current.FullSerialize = replicationInfo.NewObject;
                        if (replicationInfo.NewObject || changedObjectAsExt.IsReplicationMaskDirty(serializeMask))
                        {
                            // TODO Можно сериализовать в один массив и записывать индексы для каждого обьекта
                            // чтобы не сериализовать каждый обьект в отдельный массив
                            var serialized = _serializer.Serialize(changedObject);
                            serializedDeltaObjects.Add(changedObjectAsExt.LocalId, serialized);
                        }

                        if (replicationInfo.NewObject)
                        {
                            replicationSet[changedObject] =
                                new DeltaObjectReplicationInfo(replicationInfo.ReferenceCount, false);
                        }
                    }
                }
            }

            return serializedDeltaObjects;
        }

        public Dictionary<ulong, byte[]> SerializeReplicationSetFull(
            Dictionary<IDeltaObject, DeltaObjectReplicationInfo> replicationSet,
            long serializeMask)
        {
            var serializedDeltaObjects = new Dictionary<ulong, byte[]>();

            using (_SerializerContext.Pool.Set())
            {
                _SerializerContext.Pool.Current.FullSerialize = true;
                _SerializerContext.Pool.Current.ReplicationMask = serializeMask;
                foreach (var deltaObject in replicationSet.Keys)
                {
                    // TODO Можно сериализовать в один массив и записывать индексы для каждого обьекта
                    // чтобы не сериализовать каждый обьект в отдельный массив
                    var serialized = _serializer.Serialize(deltaObject);
                    serializedDeltaObjects.Add(((IDeltaObjectExt) deltaObject).LocalId, serialized);
                }
            }

            return serializedDeltaObjects;
        }

        public Dictionary<ulong, byte[]> SerializeEntityFull(IEntityExt entity,
            ReplicationLevel newReplicationLevel,
            ReplicationLevel oldReplicationLevel,
            long serializeMask)
        {
            var newReplicationSet = entity.ReplicationSets[newReplicationLevel];
            if (oldReplicationLevel == ReplicationLevel.None)
            {
                return SerializeReplicationSetFull(newReplicationSet, serializeMask);
            }
            else
            {
                var oldReplicationSet = entity.ReplicationSets[oldReplicationLevel];
                return SerializeReplicationSetDiffFull(newReplicationLevel, newReplicationSet, oldReplicationSet,
                    serializeMask);
            }
        }

        private Dictionary<ulong, byte[]> SerializeReplicationSetDiffFull(
            ReplicationLevel newReplicationLevel,
            Dictionary<IDeltaObject, DeltaObjectReplicationInfo> replicationSet,
            Dictionary<IDeltaObject, DeltaObjectReplicationInfo> oldReplicationSet,
            long serializeMask)
        {
            var serializedDeltaObjects = new Dictionary<ulong, byte[]>();
            var newReplicationLevelMask = (long)newReplicationLevel;
            
            using (_SerializerContext.Pool.Set())
            {
                _SerializerContext.Pool.Current.FullSerialize = true;
                foreach (var deltaObject in replicationSet.Keys)
                {
                    // сериализуем и свойства старого уровня, если обьект еще не реплицировался
                    _SerializerContext.Pool.Current.ReplicationMask = !oldReplicationSet.ContainsKey(deltaObject)
                        ? newReplicationLevelMask
                        : serializeMask;
                    
                    var serialized = _serializer.Serialize(deltaObject);
                    serializedDeltaObjects.Add(((IDeltaObjectExt) deltaObject).LocalId, serialized);
                }
            }

            return serializedDeltaObjects;
        }
    }
}