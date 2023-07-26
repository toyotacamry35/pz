using SharedCode.EntitySystem;
using SharedCode.Refs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeneratedCode.DatabaseUtils;
using MongoDB.Bson;
using SharedCode.Refs.Operations;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;

namespace GeneratedCode.EntitySystem
{
    internal static class DeltaObjectHelper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static EntityRef<T> LinkEntityRef<T>(Type containingType, EntityRef<T> entityref, string name,
            IEntitiesRepository repository) where T : IEntity
        {
            if (entityref != null && entityref.IsEntityAvailableInEntityGraphOnCurrentRepository(repository))
            {
                var __entityRef__ = (EntityRef<T>)((IEntitiesRepositoryExtension)repository).GetRef<T>(entityref.Id);
                if (__entityRef__ != null)
                {
                    ((IEntityRefExt)__entityRef__).CheckNotNull();
                    return __entityRef__;
                }
                else
                {
                    Logger.Error("LinkEntityRefsRecursive {0} {1} {2} entityRef not found", containingType, name,
                        entityref.Id);
                }
            }

            return entityref;
        }

        public static void RegisterLifeCycleCallbacks(object obj, IEntityExt newParent)
        {
            if (obj is IHookOnInit hasOnInit)
            {
                newParent.OnInitEvent += hasOnInit.OnInit;
            }

            if (obj is IHookOnDatabaseLoad hasOnDatabaseLoad)
            {
                newParent.OnDatabaseLoadEvent += hasOnDatabaseLoad.OnDatabaseLoad;
            }

            if (obj is IHookOnStart hasOnStart)
            {
                newParent.OnStartEvent += hasOnStart.OnStart;
            }

            if (obj is IHookOnReplicationLevelChanged hasOnReplicationLevelChanged)
            {
                newParent.OnReplicationLevelChangedEvent += hasOnReplicationLevelChanged.OnReplicationLevelChanged;
            }

            if (obj is IHookOnUnload hasOnUnload)
            {
                newParent.OnUnloadEvent += hasOnUnload.OnUnload;
            }

            if (obj is IHookOnDestroy hasOnDestroy)
            {
                newParent.OnDestroyEvent += hasOnDestroy.OnDestroy;
            }
        }

        public static void UnregisterLifeCycleCallbacks(object obj, IEntityExt parent)
        {
            if (obj is IHookOnInit hasOnInit)
            {
                parent.OnInitEvent -= hasOnInit.OnInit;
            }

            if (obj is IHookOnDatabaseLoad hasOnDatabaseLoad)
            {
                parent.OnDatabaseLoadEvent -= hasOnDatabaseLoad.OnDatabaseLoad;
            }

            if (obj is IHookOnStart hasOnStart)
            {
                parent.OnStartEvent -= hasOnStart.OnStart;
            }

            if (obj is IHookOnReplicationLevelChanged hasOnReplicationLevelChanged)
            {
                parent.OnReplicationLevelChangedEvent -= hasOnReplicationLevelChanged.OnReplicationLevelChanged;
            }

            if (obj is IHookOnUnload hasOnUnload)
            {
                parent.OnUnloadEvent -= hasOnUnload.OnUnload;
            }

            if (obj is IHookOnDestroy hasOnDestroy)
            {
                parent.OnDestroyEvent -= hasOnDestroy.OnDestroy;
            }
        }

        public static T GetDeltaObjField<T>(IEntity parentEntity, ref T field, bool isLockFree, bool useSystemLock = false)
        {
            if (!isLockFree)
                ((IEntityExt)parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            if (useSystemLock)
                lock (((BaseEntity)parentEntity)._LOCK_FOR_THREADSAFE_USINGFREE_PROPS)
                    return field;
            return field;
        }

        public static void IncrementParentRefs(IDeltaObjectExt deltaObjectExt, IEntity parentEntity, bool trackChanged)
        {
            deltaObjectExt.ParentEntityRefCount++;
            deltaObjectExt.parentEntity = parentEntity;

            // если у обьекта появился Parent, то добавляем обьект в измененные
            // чтобы кинуть по нему события 
            if (!_EntityContext.Pool.Current.FullCreating
                && trackChanged && deltaObjectExt.ParentEntityRefCount == 1)
            {
                ((IEntityExt)parentEntity).AddChangedObject((IDeltaObject)deltaObjectExt);
            }
        }

        public static void DecrementParentRefs(IDeltaObjectExt deltaObjectExt)
        {
            deltaObjectExt.ParentEntityRefCount--;
            // это была последняя ссылка на обьект
            if (!HasParentRef(deltaObjectExt))
            {
                deltaObjectExt.parentEntity = null;
                deltaObjectExt.parentDeltaObject = null;
            }
        }

        public static bool HasParentRef(IDeltaObjectExt deltaObjectExt)
        {
            return deltaObjectExt.ParentEntityRefCount != 0;
        }

        public static void SetDeltaObjField<T>(IEntity parentEntity, BaseDeltaObject parentDeltaObject, ref T field, T value,
            ReplicationLevel fieldRepLevel, int fieldId, bool isLockFreeReadonly, bool useSystemLock = false)
        {
            if (isLockFreeReadonly && parentEntity != null)
            {
                var state = ((BaseEntity)parentEntity).State;
                if (state == EntityState.Ready || state == EntityState.Destroying || state == EntityState.Destroyed)
                    throw new Exception("Field has been set after init even though it is marked as LockFreeREADONLY");
            }

            ((IEntityExt)parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);

            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            
            if (parentEntity != null && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(EntityRef<>))
            {
                ReplicationLevel? parentDeltaObjectReplicationLevel = GetReplicationLevel(parentEntity, parentDeltaObject);
                if (parentDeltaObjectReplicationLevel != null)
                {
                    var entityRefReachabilityLevel = parentDeltaObjectReplicationLevel.Value | fieldRepLevel;
                    
                    if (field != null && ((IEntityRefExt) field).GetEntity() != null)
                    {
                        ((IEntityExt) parentEntity).RemoveEntityRef(((IEntityRefExt) field).GetEntity(), entityRefReachabilityLevel);
                    }

                    if (value != null && ((IEntityRefExt) value).GetEntity() != null)
                    {
                        ((IEntityExt) parentEntity).CheckMigratingBeforeLink((IEntityRefExt) value);
                        ((IEntityExt) parentEntity).AddEntityRef(((IEntityRefExt) value).GetEntity(), entityRefReachabilityLevel);
                    }
                }
            }

            var valueAsDeltaObj = value as IDeltaObject;
            if (valueAsDeltaObj != null)
            {
                if (parentEntity != null)
                {
                    var newDeltaObject = ((IDeltaObjectExt)valueAsDeltaObj);
                    ((IEntityExt)parentEntity).CheckMigratingBeforeLinkedEntities(valueAsDeltaObj);
                    newDeltaObject.IncrementParentRefs(parentEntity, true);
                }

                // TODO Если  этого не оставить, то разьебет ссылки на parent
                ((IDeltaObjectExt) valueAsDeltaObj).SetParentDeltaObject(parentDeltaObject);
            }
            
            var oldFieldAsDeltaObj = field as IDeltaObject;
            ReplaceFieldInReplicationSet(parentEntity, parentDeltaObject, valueAsDeltaObj, oldFieldAsDeltaObj, fieldRepLevel);
            
            // идет после изменения ReplicationLevel иначе не получится найти replication level oldFieldAsDeltaObj
            if (oldFieldAsDeltaObj != null)
            {
                var oldFieldAsDeltaObjExt = oldFieldAsDeltaObj as IDeltaObjectExt;
                if (oldFieldAsDeltaObjExt.parentEntity != null)
                {
                    oldFieldAsDeltaObjExt.DecrementParentRefs();
                }
            }
            if (useSystemLock)
                lock (((BaseEntity)parentEntity)._LOCK_FOR_THREADSAFE_USINGFREE_PROPS)
                    field = value;
            else
                field = value;

            if (!_EntityContext.Pool.Current.FullCreating)
            {
                parentDeltaObject.SetDirty(fieldId, fieldRepLevel);
            }
        }

        public static void AddDeltaObjectToReplicationSet(
            IEntity parentEntity,
            IDeltaObject parentDeltaObject,
            IDeltaObject addedDeltaObject,
            ReplicationLevel fieldRepLevel)
        {
            ReplaceFieldInReplicationSet(parentEntity, parentDeltaObject, addedDeltaObject, null, fieldRepLevel);
        }

        public static void RemoveDeltaObjectFromReplicationSet(
            IEntity parentEntity,
            IDeltaObject parentDeltaObject,
            IDeltaObject removedDeltaObject,
            ReplicationLevel fieldRepLevel)
        {
            ReplaceFieldInReplicationSet(parentEntity, parentDeltaObject, null, removedDeltaObject, fieldRepLevel);
        }

        public static void ReplaceFieldInReplicationSet(
            IEntity parentEntity,
            IDeltaObject parentDeltaObject,
            IDeltaObject newDeltaObject,
            IDeltaObject oldDeltaObject,
            ReplicationLevel fieldRepLevel)
        {
            if (newDeltaObject != null || oldDeltaObject != null)
            {
                if (parentEntity != null)
                {
                    ReplicationLevel? parentDeltaObjectReplicationLevel = null;
                    ReplicationLevel? newValueOldReplicationLevel = null;
                    ReplicationLevel? oldValueOldReplicationLevel = null;
                    foreach (var replicationSet in ((IEntityExt)parentEntity).ReplicationSets)
                    {
                        parentDeltaObjectReplicationLevel = CompareMinimumReplicationLevel(parentDeltaObject, replicationSet,
                            parentDeltaObjectReplicationLevel);

                        if (newDeltaObject != null)
                        {
                            newValueOldReplicationLevel = CompareMinimumReplicationLevel(newDeltaObject,
                                replicationSet,
                                newValueOldReplicationLevel);
                        }

                        if (oldDeltaObject != null)
                        {
                            oldValueOldReplicationLevel = CompareMinimumReplicationLevel(oldDeltaObject,
                                replicationSet,
                                oldValueOldReplicationLevel);
                        }
                    }

                    if (parentDeltaObjectReplicationLevel != null)
                    {
                        var propertyReplicationReachLevel = parentDeltaObjectReplicationLevel.Value | fieldRepLevel;

                        foreach (var replicationSet in ((IEntityExt)parentEntity).ReplicationSets)
                        {
                            if (replicationSet.Key >= propertyReplicationReachLevel)
                            {
                                if (newDeltaObject != null)
                                {
                                    AddReplicationRef((IEntityExt)parentEntity, replicationSet.Value, newDeltaObject);
                                }

                                if (oldValueOldReplicationLevel != null)
                                {
                                    RemoveReplicationRef(replicationSet.Value, oldDeltaObject);
                                }
                            }
                        }

                        if (newDeltaObject != null)
                        {
                            var newValueActualReplicationLevel = GetReplicationLevel(parentEntity, newDeltaObject);
                            if (newValueActualReplicationLevel != newValueOldReplicationLevel)
                            {
                                ((IDeltaObjectExt)newDeltaObject).ReplicationLevelActualize(newValueActualReplicationLevel,
                                    newValueOldReplicationLevel);
                            }
                        }

                        if (oldValueOldReplicationLevel != null)
                        {
                            var oldValueActualReplicationLevel = GetReplicationLevel(parentEntity, oldDeltaObject);
                            // не распространяем удаление уровня, если это был не актуальный для обьекта уровень (в дочернем обьекте хранится только актуальный уровень)
                            if (oldValueActualReplicationLevel != oldValueOldReplicationLevel)
                            {
                                ((IDeltaObjectExt)oldDeltaObject).ReplicationLevelActualize(oldValueActualReplicationLevel, oldValueOldReplicationLevel);
                            }
                        }
                    }
                }
            }
        }

        private static void AddReplicationRef(IEntityExt parentEntity,
            Dictionary<IDeltaObject, DeltaObjectReplicationInfo> replicationSet, IDeltaObject deltaObject)
        {
            replicationSet.TryGetValue(deltaObject, out var deltaObjectOldReplicationInfo);
            var firstRef = deltaObjectOldReplicationInfo.ReferenceCount == 0;
            replicationSet[deltaObject] =
                new DeltaObjectReplicationInfo(deltaObjectOldReplicationInfo.ReferenceCount + 1, firstRef);
            if (firstRef)
            {
                parentEntity.AddChangedObject(deltaObject);
            }
        }

        private static void RemoveReplicationRef(Dictionary<IDeltaObject, DeltaObjectReplicationInfo> replicationSet, IDeltaObject deltaObject)
        {
            replicationSet.TryGetValue(deltaObject, out var deltaObjectReplicationInfo);
            if (deltaObjectReplicationInfo.ReferenceCount == 1)
            {
                replicationSet.Remove(deltaObject);
            }
            else
            {
                replicationSet[deltaObject] = new DeltaObjectReplicationInfo(
                    deltaObjectReplicationInfo.ReferenceCount - 1,
                    deltaObjectReplicationInfo.NewObject);
            }
        }

        public static bool DbEntity(IEntityRef entityRef)
        {
            return DatabaseSaveTypeChecker.GetDatabaseSaveType(entityRef.TypeId) != DatabaseSaveType.None;
        }

        public static void ReplicationLevelActualize(
            IEntitiesRepositoryExtension entitiesRepository,
            IEntity parentEntity,
            IEntityRef field,
            ReplicationLevel fieldReplicationLevel,
            ReplicationLevel? actualParentLevel,
            ReplicationLevel? oldParentLevel)
        {
            if (field == null)
            {
                return;
            }
            
            ReplicationLevel? oldFieldReachableReplicationLevel = null;
            if (oldParentLevel != null)
            {
                oldFieldReachableReplicationLevel = oldParentLevel | fieldReplicationLevel;
            }

            ReplicationLevel? actualFieldReachableReplicationLevel = null;
            if (actualParentLevel != null)
            {
                actualFieldReachableReplicationLevel = actualParentLevel | fieldReplicationLevel;
            }

            if (oldFieldReachableReplicationLevel != actualFieldReachableReplicationLevel)
            {
                var entityRef = entitiesRepository.GetRef(parentEntity.TypeId, parentEntity.Id);
                ((IEntityRefExt) entityRef).ChangeEntityRef(
                    new EntityRefChange(oldFieldReachableReplicationLevel,
                        actualFieldReachableReplicationLevel,
                        field));
            }
        }

        /// <summary>
        /// Рекурсивно обновляем изменившиеся репликационные уровни
        /// </summary>
        public static void ReplicationLevelActualize(
            IEntity parentEntity,
            IDeltaObject field,
            ReplicationLevel fieldReplicationLevel,
            ReplicationLevel? actualParentLevel,
            ReplicationLevel? oldParentLevel)
        {
            if (field == null)
            {
                return;
            }

            var oldFieldReplicationLevel = GetReplicationLevel(parentEntity, field);

            ReplicationLevel? oldFieldReachableReplicationLevel = null;
            if (oldParentLevel != null)
            {
                oldFieldReachableReplicationLevel = oldParentLevel | fieldReplicationLevel;
            }

            ReplicationLevel? actualFieldReachableReplicationLevel = null;
            if (actualParentLevel != null)
            {
                actualFieldReachableReplicationLevel = actualParentLevel | fieldReplicationLevel;
            }

            // уровень родителя стал ниже (ex. Server -> ClientFull)
            if (actualParentLevel != null && (oldParentLevel == null || actualParentLevel < oldParentLevel))
            {
                foreach (var replicationSet in ((IEntityExt)parentEntity).ReplicationSets)
                {
                    if (actualFieldReachableReplicationLevel <= replicationSet.Key)
                    {
                        // если раньше по этому полю обьект уже был в репликации, по сути заменяем старый уровень на новый
                        // поэтому инкрементим только новые уровни (replicationSet.Key < oldFieldReachableReplicationLevel)
                        if (oldFieldReachableReplicationLevel == null ||
                            replicationSet.Key < oldFieldReachableReplicationLevel)
                        {
                            AddReplicationRef((IEntityExt)parentEntity, replicationSet.Value, field);
                        }
                    }
                }
            }
            // уровень родителя стал выше (ex. ClientFull -> Server)
            else
            {
                foreach (var replicationSet in ((IEntityExt)parentEntity).ReplicationSets)
                {
                    if (oldFieldReachableReplicationLevel <= replicationSet.Key)
                    {
                        if (actualFieldReachableReplicationLevel == null ||
                            replicationSet.Key < actualFieldReachableReplicationLevel)
                        {
                            RemoveReplicationRef(replicationSet.Value, field);
                        }
                    }
                }
            }

            var actualFieldReplicationLevel = GetReplicationLevel(parentEntity, field);
            if (actualFieldReplicationLevel != oldFieldReplicationLevel)
            {
                ((IDeltaObjectExt)field).ReplicationLevelActualize(actualFieldReplicationLevel, oldFieldReplicationLevel);
            }
        }

        private static ReplicationLevel? CompareMinimumReplicationLevel(IDeltaObject deltaObject,
            KeyValuePair<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSet,
            ReplicationLevel? currentMinimumReplicationLevel)
        {
            if (replicationSet.Value.ContainsKey(deltaObject))
            {
                if (currentMinimumReplicationLevel == null || replicationSet.Key < currentMinimumReplicationLevel)
                {
                    currentMinimumReplicationLevel = replicationSet.Key;
                }
            }

            return currentMinimumReplicationLevel;
        }

        public static ReplicationLevel? GetReplicationLevel(IEntity parentEntity, IDeltaObject deltaObject)
        {
            ReplicationLevel? deltaObjectReplicationLevel = null;
            if (parentEntity != null)
            {
                foreach (var replicationSet in ((IEntityExt) parentEntity).ReplicationSets)
                {
                    deltaObjectReplicationLevel = CompareMinimumReplicationLevel(deltaObject, replicationSet, deltaObjectReplicationLevel);
                }
            }

            return deltaObjectReplicationLevel;
        }

        public static void SetDeltaObjFieldFromSerialization<T>(IEntity parentEntity, BaseDeltaObject deltaObject,
            ref T field, T value, ReplicationLevel fieldRepLevel, int fieldId, bool isLockFreeReadonly, bool useSystemLock = false)
        {
            if (!_EntityContext.Pool.Current.FullCreating)
            {
                deltaObject.SetDirty(fieldId, fieldRepLevel);
            }

            if ((field == null && value == null) || (field != null && value != null && EqualityComparer<T>.Default.Equals(field, value)))
                return;

            if (field is IDeltaObjectExt oldFieldAsDeltaObjExt)
            {
                if (oldFieldAsDeltaObjExt.parentEntity != null)
                {
                    oldFieldAsDeltaObjExt.DecrementParentRefs();
                }
            }

            if (value is IDeltaObjectExt newDeltaObject)
            {
                if (parentEntity != null)
                {
                    newDeltaObject.IncrementParentRefs(parentEntity, false);
                }

                // TODO Если  этого не оставить, то разьебет ссылки на parent
                newDeltaObject.SetParentDeltaObject(deltaObject);
            }

            if (useSystemLock)
                lock (((BaseEntity)parentEntity)._LOCK_FOR_THREADSAFE_USINGFREE_PROPS)
                    field = value;
            else
                field = value;
        }

        public static void LinkChangedDeltaObject<T>(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects,
            IEntity parentEntity,
            BaseDeltaObject deltaObject,
            ref T deltaObjectField,
            int fieldId,
            bool isLockFreeReadonly,
            ReplicationLevel replicationLevel) where T : IDeltaObject
        {
            var deserializedObject = deserializedObjects[deltaObject.LocalId];
            if (deserializedObject.ChangedFields == null ||
                !deserializedObject.ChangedFields.TryGetValue(fieldId, out var newLocalId))
            {
                return;
            }

            var newValue = ResolveDeltaObjectWhileDeserialize<T>(parentEntity, deserializedObjects, newLocalId);

            SetDeltaObjFieldFromSerialization(parentEntity, deltaObject, ref deltaObjectField,
                newValue, replicationLevel, fieldId, isLockFreeReadonly);
        }

        public static T ResolveDeltaObjectWhileDeserialize<T>(IEntity parentEntity,
            Dictionary<ulong, DeserializedObjectInfo> deserializedObjects, ulong? newLocalId)
        {
            if (!TryResolveDeltaObjectWhileDeserialize<T>(parentEntity, deserializedObjects, newLocalId,
                out var deltaObject))
            {
                throw new Exception($"Couldn't resolve delta object newLocalId={newLocalId}");
            }

            return deltaObject;
        }

        public static bool TryResolveDeltaObjectWhileDeserialize<T>(IEntity parentEntity,
            Dictionary<ulong, DeserializedObjectInfo> deserializedObjects, ulong? newLocalId, out T deltaObject)
        {
            deltaObject = default;
            if (newLocalId == null)
            {
                return true;
            }

            deltaObject = (T)((IEntityExt)parentEntity).ResolveDeltaObject(newLocalId.Value);
            if (deltaObject != null)
            {
                return true;
            }

            if (deserializedObjects.TryGetValue(newLocalId.Value, out var deserializedObject))
            {
                deltaObject = (T)deserializedObject.DeltaObject;
                return true;
            }

            return false;
        }

        public static void SetSerializedValue(IDeltaObject deltaObject, int fieldId, ulong? newValue)
        {
            var deltaObjectLocalId = ((IDeltaObjectExt)deltaObject).LocalId;
            if (!_SerializerContext.Pool.Current.ChangedObjects.TryGetValue(deltaObjectLocalId,
                out var deserializedObject))
            {
                deserializedObject = new DeserializedObjectInfo(deltaObject, new Dictionary<int, ulong?>());
                _SerializerContext.Pool.Current.ChangedObjects[deltaObjectLocalId] = deserializedObject;
            }

            deserializedObject.ChangedFields[fieldId] = newValue;
        }

        public static void SetParentDeltaObject(this IDeltaObject obj, IDeltaObjectExt parentDeltaObject)
        {
            ((IDeltaObjectExt)obj).parentDeltaObject = parentDeltaObject;
        }

        public static void AddChangedObject(IEntity parentEntity, IDeltaObject deltaObject)
        {
            if (!_EntityContext.Pool.Current.FullCreating)
            {
                // один и тот же обьек может быть добавлен и удален несколько раз 
                ((IEntityExt)parentEntity)?.AddChangedObject(deltaObject);
            }
        }

        public static bool FillReplicationSet(
            Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSets,
            HashSet<ReplicationLevel> traverseReplicationLevels,
            IDeltaObject deltaObject,
            ReplicationLevel propertyReplicationLevel)
        {
            if (deltaObject == null)
            {
                return false;
            }

            bool shouldContinueTraverse = false;
            foreach (var subscribedReplicationLevel in traverseReplicationLevels)
            {
                if (((long)propertyReplicationLevel & (long)subscribedReplicationLevel) ==
                    (long)propertyReplicationLevel)
                {
                    var replicaSet = replicationSets[subscribedReplicationLevel];
                    replicaSet.TryGetValue(deltaObject, out var refsCount);
                    replicaSet[deltaObject] = new DeltaObjectReplicationInfo(refsCount.ReferenceCount + 1, false);
                    shouldContinueTraverse = true;
                }
            }

            return shouldContinueTraverse;
        }

        public static void FillReplicationSetRecursive(
            Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSets,
            HashSet<ReplicationLevel> traverseReplicationLevels,
            IDeltaObject deltaObject,
            ReplicationLevel propertyReplicationLevel,
            bool propertyShouldBeSavedToDb,
            bool withBsonIgnore)
        {
            if (!withBsonIgnore && !propertyShouldBeSavedToDb)
            {
                return;
            }

            var shouldContinueTraverse = FillReplicationSet(replicationSets, traverseReplicationLevels, deltaObject,
                propertyReplicationLevel);

            if (shouldContinueTraverse)
            {
                ((IDeltaObjectExt)deltaObject).FillReplicationSetRecursive(replicationSets, traverseReplicationLevels,
                    propertyReplicationLevel, withBsonIgnore);
            }
        }

        public static void ParentEntitySetterImpl(IDeltaObjectExt obj, ref IEntity _parentEntity, IEntity value)
        {
            if (_parentEntity == value)
                return;

            if (_parentEntity != null && value != null)
                throw new InvalidOperationException(
                    $"Try override existing parent entity. Object {obj} is trying to reparent from {_parentEntity} to {value}");

            if (_parentEntity != null)
            {
                UnregisterLifeCycleCallbacks(obj, (IEntityExt)_parentEntity);
                obj.LocalId = 0;
                obj.LastParentEntityReplicationMask = ((IEntityExt)_parentEntity).CurrentReplicationMask;
            }

            _parentEntity = value;

            if (_parentEntity != null)
            {
                obj.LastParentEntityReplicationMask = 0;

                if (_parentEntity.IsMaster())
                    obj.LocalId = ((BaseEntity)_parentEntity).NextFreeLocalId++;
                else if (obj.LocalId != 0 &&
                         !((BaseEntity)_parentEntity).LocalIdsToObject.TryAdd(obj.LocalId, (IDeltaObject)obj))
                    throw new InvalidOperationException(
                        $"LocalId {obj.LocalId} for obj {obj} is already taken in parent entity {obj.parentEntity}");

                RegisterLifeCycleCallbacks(obj, (IEntityExt)_parentEntity);
            }
        }

        public static void LocalIdSetterImpl(IDeltaObjectExt obj, ref ulong _localId, ulong value)
        {
            if (value == _localId)
                return;

            if (_localId != 0)
                if (!((BaseEntity)obj.parentEntity).LocalIdsToObject.TryRemove(_localId, out var existing) ||
                    existing != obj)
                    throw new InvalidOperationException(
                        $"Object {obj} is not registerd in parent entity {obj.parentEntity}");

            _localId = value;

            if (_localId != 0 && obj.parentEntity != null)
                if (!((BaseEntity)obj.parentEntity).LocalIdsToObject.TryAdd(_localId, (IDeltaObject)obj))
                    throw new InvalidOperationException(
                        $"LocalId {_localId} for obj {obj} is already taken in parent entity {obj.parentEntity}");
        }
    }
}
