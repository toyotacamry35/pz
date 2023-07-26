using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Logging;
using Assets.Src.ResourcesSystem.Base;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedCode.OurSimpleIoC;
using SharedCode.Refs;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;
using GeneratedCode.EntitySystem;
using ResourcesSystem.Base;
using SharedCode.Refs.Operations;

namespace SharedCode.EntitySystem.Delta
{
    internal static class DictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, in TKey key, in TValue value)
        {
            if (dictionary.ContainsKey(key))
                return false;
            dictionary.Add(key, value);
            return true;
        }
    }

    [ProtoContract(IgnoreListHandling = true)]
    [JsonObject]
    [BsonDiscriminator("DeltaDictionary")]
    public class DeltaDictionary<TKey, TValue> : IDeltaDictionary<TKey, TValue>, IDeltaDictionaryExt<TKey, TValue>,
        IDeltaObjectExt, IDeltaDictionaryExt
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private ulong _localId;

        [ProtoMember(2)]
        public ulong LocalId
        {
            get => _localId;
            set => DeltaObjectHelper.LocalIdSetterImpl(this, ref _localId, value);
        }

        public string TypeName => GetType().ToString();

        public static readonly bool IsDeltaObject = typeof(IDeltaObject).IsAssignableFrom(typeof(TValue));

        private static readonly bool _isEntityRef =
            typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(EntityRef<>);

        private static readonly bool _isValueType =
            typeof(TValue).IsValueType || typeof(TValue).IsSubclassOf(typeof(BaseResource));

        [ProtoIgnore] [JsonIgnore] [BsonIgnore]
        private IEntity _parentEntity;

        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        public IEntity parentEntity
        {
            get => _parentEntity;
            set => DeltaObjectHelper.ParentEntitySetterImpl(this, ref _parentEntity, value);
        }

        [ProtoIgnore] public IDeltaObjectExt parentDeltaObject { get; set; }

        [ProtoIgnore] [JsonIgnore] public long LastParentEntityReplicationMask { get; set; }

        public short ParentEntityRefCount { get; set; }

        public void DecrementParentRefs()
        {
            DeltaObjectHelper.DecrementParentRefs(this);
            if (!DeltaObjectHelper.HasParentRef(this) && IsDeltaObject)
            {
                foreach (var value in _linkedValuesDictionary.Values)
                {
                    if (value != null)
                    {
                        ((IDeltaObjectExt) value).DecrementParentRefs();
                    }
                }
            }
        }

        public void IncrementParentRefs(IEntity parentEntity, bool trackChanged)
        {
            DeltaObjectHelper.IncrementParentRefs(this, parentEntity, trackChanged);
            if (ParentEntityRefCount == 1 && IsDeltaObject)
            {
                foreach (var value in _linkedValuesDictionary.Values)
                {
                    if (value != null)
                    {
                        ((IDeltaObjectExt) value).IncrementParentRefs(parentEntity, trackChanged);
                    }
                }
                
                foreach (var deltaOperation in _deltaOperations)
                {
                    if (deltaOperation.DeltaObjectLocalId == 0)
                    {
                        deltaOperation.DeltaObjectLocalId = deltaOperation.RuntimeDeltaObject.LocalId;
                    }
                }
            }
        }


        public bool HasParentRef()
        {
            return DeltaObjectHelper.HasParentRef(this);
        }

        public void ReplicationLevelActualize(ReplicationLevel? actualParentLevel, ReplicationLevel? oldParentLevel)
        {
            if (IsDeltaObject)
            {
                foreach (var value in _linkedValuesDictionary)
                {
                    if (value.Value != null)
                    {
                        DeltaObjectHelper.ReplicationLevelActualize(parentEntity, (IDeltaObject) value.Value,
                            ReplicationLevel.Always, actualParentLevel, oldParentLevel);
                    }
                }
            }
        }

        private long _replicationChangedMask = 0;

        public DeltaDictionary()
        {
        }

        public DeltaDictionary(IDictionary<TKey, TValue> dictionary) : this()
        {
            foreach (var pair in dictionary)
                Add(pair.Key, pair.Value);
        }
        
        public static DeltaDictionary<TKey, TValue> CreateFromIds(Dictionary<TKey, ulong?> deltaObjectsIds)
        {
            var deltaList = new DeltaDictionary<TKey, TValue>
            {
                _deltaObjectsLocalIds = deltaObjectsIds
            };
            
            return deltaList;
        }
        
        public static DeltaDictionary<TKey, TValue> CreateFromRawObjects(Dictionary<TKey, TValue> deltaObjectsIds)
        {
            var deltaDictionary = new DeltaDictionary<TKey, TValue>
            {
                _linkedValuesDictionary = deltaObjectsIds
            };
            
            return deltaDictionary;
        }

        [ProtoMember(3, OverwriteList = true)] 
        public Dictionary<TKey, TValue> _rawValuesDictionary
        {
            get => _linkedValuesDictionary;
            set => _linkedValuesDictionary = value;
        }

        [ProtoMember(4, OverwriteList = true)] 
        public Dictionary<TKey, ulong?> _deltaObjectsLocalIds { get; set; }
        
        private Dictionary<TKey, TValue> _linkedValuesDictionary = new Dictionary<TKey, TValue>();

        private Dictionary<TKey, OldDictionaryOperation> _operationsHistory = new Dictionary<TKey, OldDictionaryOperation>();

        [ProtoIgnore] public Dictionary<TKey, TValue> Items => _linkedValuesDictionary;

        [ProtoMember(5, OverwriteList = true)]
        public List<DictionaryDeltaOperation<TKey, TValue>> _deltaOperations =
            new List<DictionaryDeltaOperation<TKey, TValue>>();

        [ProtoIgnore] private List<Func<Task>> _eventsList;

        [ProtoIgnore] private bool _needInvokeChanged;
        
        private static Guid DefaultMigrationgid = Guid.Empty;

        public virtual ref Guid MigratingId
        {
            get
            {
                if (parentEntity != null)
                    return ref ((IDeltaObjectExt) parentEntity).MigratingId;
                return ref DefaultMigrationgid;
            }
        }

        [ProtoIgnore]
        private List<ValueTuple<object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>>>
            _onItemAddedOrUpdatedCustomActions;

        [ProtoIgnore]
        private List<ValueTuple<object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>>>
            _onItemRemovedCustomActions;

        [ProtoIgnore]
        public string OnItemAddedOrUpdatedSubscribers
        {
            get
            {
                var sb = new StringBuilder();
                if (_onItemAddedOrUpdatedCustomActions != null)
                    lock (_onItemAddedOrUpdatedCustomActions)
                        if (_onItemAddedOrUpdatedCustomActions.Count > 0)
                        {
                            sb.AppendFormat("{1}subscribers count: {0}", _onItemAddedOrUpdatedCustomActions.Count,
                                _onItemAddedOrUpdatedCustomActions.Count >=
                                ServerCoreRuntimeParameters.EventSubscribersWarnCount
                                    ? "WARN "
                                    : "").AppendLine();
                            foreach (var customAction in _onItemAddedOrUpdatedCustomActions)
                                sb.AppendFormat("<obj:{0} method:{1} hashcode:{2}>",
                                    customAction.Item3.Target.GetType().Name, customAction.Item3.Method.Name,
                                    customAction.Item3.Target.GetHashCode());
                        }

                sb.Append("-");

                if (OnItemAddedOrUpdated != null)
                {
                    var subscribers = OnItemAddedOrUpdated.GetInvocationList();
                    sb.AppendFormat("{1}subscribers count: {0}", subscribers.Length,
                            subscribers.Length >= ServerCoreRuntimeParameters.EventSubscribersWarnCount ? "WARN " : "")
                        .AppendLine();
                    foreach (var subscriber in subscribers.Cast<DeltaDictionaryChangedDelegate<TKey, TValue>>())
                        sb.AppendFormat("<obj:{0} method:{1} hashcode:{2}>", subscriber.Target.GetType().Name,
                            subscriber.Method.Name, subscriber.Target.GetHashCode());
                }

                return sb.ToString();
            }
        }

        [ProtoIgnore]
        public string OnItemRemovedSubscribers
        {
            get
            {
                var sb = new StringBuilder();
                if (_onItemRemovedCustomActions != null)
                    lock (_onItemRemovedCustomActions)
                        if (_onItemRemovedCustomActions.Count > 0)
                        {
                            sb.AppendFormat("{1}subscribers count: {0}", _onItemRemovedCustomActions.Count,
                                _onItemRemovedCustomActions.Count >=
                                ServerCoreRuntimeParameters.EventSubscribersWarnCount
                                    ? "WARN "
                                    : "").AppendLine();
                            foreach (var customAction in _onItemRemovedCustomActions)
                                sb.AppendFormat("<obj:{0} method:{1} hashcode:{2}>",
                                    customAction.Item3.Target.GetType().Name, customAction.Item3.Method.Name,
                                    customAction.Item3.Target.GetHashCode());
                        }

                sb.Append("-");

                if (OnItemRemoved != null)
                {
                    var subscribers = OnItemRemoved.GetInvocationList();
                    sb.AppendFormat("{1}subscribers count: {0}", subscribers.Length,
                            subscribers.Length >= ServerCoreRuntimeParameters.EventSubscribersWarnCount ? "WARN " : "")
                        .AppendLine();
                    foreach (var subscriber in subscribers.Cast<DeltaDictionaryChangedDelegate<TKey, TValue>>())
                        sb.AppendFormat("<obj:{0} method:{1} hashcode:{2}>", subscriber.Target.GetType().Name,
                            subscriber.Method.Name, subscriber.Target.GetHashCode());
                }

                return sb.ToString();
            }
        }

        [ProtoIgnore]
        public string OnChangedSubscribers
        {
            get
            {
                var sb = new StringBuilder();

                if (OnChanged != null)
                {
                    var subscribers = OnChanged.GetInvocationList();
                    sb.AppendFormat("{1}subscribers count: {0}", subscribers.Length,
                            subscribers.Length >= ServerCoreRuntimeParameters.EventSubscribersWarnCount ? "WARN " : "")
                        .AppendLine();
                    foreach (var subscriber in subscribers.Cast<DeltaDictionaryChangedDelegate>())
                        sb.AppendFormat("<obj:{0} method:{1} hashcode:{2}>", subscriber.Target.GetType().Name,
                            subscriber.Method.Name, subscriber.Target.GetHashCode());
                }

                return sb.ToString();
            }
        }

        public event DeltaDictionaryChangedDelegate<TKey, TValue> OnItemAddedOrUpdated;

        public event DeltaDictionaryChangedDelegate<TKey, TValue> OnItemRemoved;

        public event DeltaDictionaryChangedDelegate OnChanged;

        public IDeltaDictionary<TKey, T1> ToDeltaDictionary<T1>() where T1 : IBaseDeltaObjectWrapper
        {
            var wrapper = (IDeltaDictionary<TKey, T1>) Activator.CreateInstance(typeof(DeltaDictionaryWrapper<,,>)
                    .MakeGenericType(typeof(TKey), typeof(TValue), typeof(T1)),
                this);
            return wrapper;
        }
        
        void IDeltaDictionaryExt<TKey, TValue>.SubscribeCustomAddOrUpdateAction(object key, MethodInfo key2,
            DeltaDictionaryChangedDelegate<TKey, TValue> action)
        {
            if (_onItemAddedOrUpdatedCustomActions == null)
                lock (_linkedValuesDictionary)
                    if (_onItemAddedOrUpdatedCustomActions == null)
                        _onItemAddedOrUpdatedCustomActions =
                            new List<(object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>)>();
            lock (_onItemAddedOrUpdatedCustomActions)
                _onItemAddedOrUpdatedCustomActions.Add(
                    new ValueTuple<object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>>(key, key2,
                        action));
        }

        void IDeltaDictionaryExt<TKey, TValue>.UnsubscribeCustomAddOrUpdateAction(object key, MethodInfo key2)
        {
            if (_onItemAddedOrUpdatedCustomActions == null)
                return;

            lock (_onItemAddedOrUpdatedCustomActions)
            {
                var tuple = _onItemAddedOrUpdatedCustomActions.FirstOrDefault(x => x.Item1 == key && x.Item2 == key2);
                if (tuple.Item1 != null)
                    _onItemAddedOrUpdatedCustomActions.Remove(tuple);
            }
        }

        void IDeltaDictionaryExt<TKey, TValue>.SubscribeCustomRemoveAction(object key, MethodInfo key2,
            DeltaDictionaryChangedDelegate<TKey, TValue> action)
        {
            if (_onItemRemovedCustomActions == null)
                lock (_linkedValuesDictionary)
                    if (_onItemRemovedCustomActions == null)
                        _onItemRemovedCustomActions =
                            new List<(object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>)>();
            lock (_onItemRemovedCustomActions)
                _onItemRemovedCustomActions.Add(
                    new ValueTuple<object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>>(key, key2,
                        action));
        }

        void IDeltaDictionaryExt<TKey, TValue>.UnsubscribeCustomRemoveAction(object key, MethodInfo key2)
        {
            if (_onItemRemovedCustomActions == null)
                return;

            lock (_onItemRemovedCustomActions)
            {
                var tuple = _onItemRemovedCustomActions.FirstOrDefault(x => x.Item1 == key && x.Item2 == key2);
                if (tuple.Item1 != null)
                    _onItemRemovedCustomActions.Remove(tuple);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            return _linkedValuesDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void setOperationValue(DictionaryDeltaOperation<TKey, TValue> operation, TValue value)
        {
            if (IsDeltaObject)
            {
                var deltaObject = value as IDeltaObjectExt;
                if (deltaObject?.LocalId == 0)
                {
                    // откладываем простановку LocalId на потом (когда установится ParentEntity)
                    operation.RuntimeDeltaObject = deltaObject;
                }
                else
                {
                    operation.DeltaObjectLocalId = deltaObject.LocalId;
                }
            }
            else if (_isValueType)
            {
                operation.Value = new DeltaValueWrapper<TValue>(value);
            }
            else
            {
                operation.ValueRef = new DeltaReferenceWrapper<TValue>(value);
            }
        }

       TValue getOperationValue(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects, IEntity newParentEntity, DictionaryDeltaOperation<TKey, TValue> deltaOperation)
        {
            if (deltaOperation.Value != null)
            {
                return deltaOperation.Value.Value;
            }

            if (deltaOperation.ValueRef != null)
            {
                return deltaOperation.ValueRef.Value;
            }

            return DeltaObjectHelper.ResolveDeltaObjectWhileDeserialize<TValue>(newParentEntity, deserializedObjects,
                deltaOperation.DeltaObjectLocalId);
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        private void addDeltaOperation(DictionaryDeltaOperation<TKey, TValue> newOperation, bool oldValueExisted)
        {
            if (newOperation.Operation == DictionaryDeltaOperationType.Clear)
            {
                _deltaOperations.Clear();
                _deltaOperations.Add(newOperation);
            }
            else
            {
                if (_operationsHistory.TryGetValue(newOperation.Key, out var oldOperation))
                {
                    // чтобы не посылать пустой Remove
                    if (newOperation.Operation == DictionaryDeltaOperationType.Remove && !oldOperation.OldValueExisted)
                    {
                        _deltaOperations.RemoveAt(oldOperation.OperationIndex);
                        
                        // перепрошиваем индексы в истории
                        for (int operationIndex = oldOperation.OperationIndex; operationIndex < _deltaOperations.Count; operationIndex++)
                        {
                            var deltaOperation = _deltaOperations[operationIndex];
                            if(_operationsHistory.TryGetValue(deltaOperation.Key, out var historyOperation))
                            {
                                _operationsHistory[deltaOperation.Key] = new OldDictionaryOperation(
                                    historyOperation.OperationIndex - 1,
                                    historyOperation.Operation,
                                    historyOperation.OldValueExisted);
                            }
                        }
                    }
                    else
                    {
                        _deltaOperations[oldOperation.OperationIndex] = newOperation;   
                    }
                }
                else
                {
                    _deltaOperations.Add(newOperation);
                    _operationsHistory.Add(newOperation.Key,
                        new OldDictionaryOperation(_deltaOperations.Count - 1, newOperation, oldValueExisted));
                }
            }
            
            SetDirty(true);
        }

        public void Clear()
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);

            foreach (var pair in _linkedValuesDictionary)
            {
                var copyPair = pair;
                invokeItemRemoved(copyPair.Key, copyPair.Value);
                checkUnsubscribePart(pair.Value);
                CheckRemoveFromReplicationSet(pair.Value);
                CheckDecrementParentEntityRef(pair.Value);
            }

            _linkedValuesDictionary.Clear();

            addDeltaOperation(new DictionaryDeltaOperation<TKey, TValue>
            {
                Operation = DictionaryDeltaOperationType.Clear,
            }, false);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            return _linkedValuesDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            ((ICollection<KeyValuePair<TKey, TValue>>) _linkedValuesDictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        [JsonIgnore]
        public int Count
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return _linkedValuesDictionary.Count;
            }
        }

        [JsonIgnore]
        public bool IsReadOnly
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return ((ICollection<KeyValuePair<TKey, TValue>>) _linkedValuesDictionary).IsReadOnly;
            }
        }

        public bool ContainsKey(TKey key)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            return _linkedValuesDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);

            if (key == null)
                throw new Exception("Do not add null key Pidr!!!");

            _linkedValuesDictionary.Add(key, value);

            CheckAddParentEntityRef(value, true);
            CheckAddToReplicationSet(value);
            var operation = new DictionaryDeltaOperation<TKey, TValue>
            {
                Operation = DictionaryDeltaOperationType.SetNew,
                Key = key
            };
            setOperationValue(operation, value);
            addDeltaOperation(operation, false);
            invokeItemAddedOrUpdated(key, default, value);
            checkSubscribePart(value);
        }

        public bool Remove(TKey key)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);

            TValue removedValue;
            _linkedValuesDictionary.TryGetValue(key, out removedValue);
            bool result = _linkedValuesDictionary.Remove(key);
            if (!result)
                return false;

            addDeltaOperation(new DictionaryDeltaOperation<TKey, TValue>
            {
                Operation = DictionaryDeltaOperationType.Remove,
                Key = key,
            }, true);

            invokeItemRemoved(key, removedValue);

            checkUnsubscribePart(removedValue);
            CheckRemoveFromReplicationSet(removedValue);
            CheckDecrementParentEntityRef(removedValue);

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            return _linkedValuesDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return _linkedValuesDictionary[key];
            }
            set
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);
                if (key == null)
                    throw new Exception("Do not add null key Pidr!!!");

                var keyExists = _linkedValuesDictionary.TryGetValue(key, out var removedValue);

                if (keyExists && EqualityComparer<TValue>.Default.Equals(value, removedValue))
                    return;

                _linkedValuesDictionary[key] = value;

                checkUnsubscribePart(removedValue);
                CheckRemoveFromReplicationSet(removedValue);
                CheckDecrementParentEntityRef(removedValue);

                CheckAddParentEntityRef(value, true);
                CheckAddToReplicationSet(value);
                var operation = new DictionaryDeltaOperation<TKey, TValue>
                {
                    Operation = DictionaryDeltaOperationType.SetNew,
                    Key = key,
                };
                setOperationValue(operation, value);
                addDeltaOperation(operation, keyExists);

                invokeItemAddedOrUpdated(key, removedValue, value);
                checkSubscribePart(value);
            }
        }

        [JsonIgnore]
        public ICollection<TKey> Keys
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return _linkedValuesDictionary.Keys;
            }
        }

        [JsonIgnore]
        public ICollection<TValue> Values
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return _linkedValuesDictionary.Values;
            }
        }
        
        public bool _rawValuesDictionarySpecified
        {
            get
            {
                return _SerializerContext.Pool.Current.FullSerialize && !IsDeltaObject;
            }
            set
            {
                SetDirty(false);
            }
        }
        
        public bool _deltaObjectsLocalIdsSpecified
        {
            get
            {
                return _SerializerContext.Pool.Current.FullSerialize && IsDeltaObject;
            }
            set
            {
                SetDirty(false);
            }
        }

        public bool ShouldSerialize_deltaOperations()
        {
            if (_SerializerContext.Pool.Current.FullSerialize)
                return false;

            return _deltaOperations.Count > 0;
        }

        [ProtoBeforeSerialization]
        public void beforeSerialization()
        {
            if (_SerializerContext.Pool.Current.FullSerialize && IsDeltaObject)
            {
                _deltaObjectsLocalIds = GetDeltaObjectsLocalIdsInternal();
            }
        }

        // делается только при полной сериализации и сохранении в бд, поэтому делается отдельно,
        // а не наполняется в процессе изменения коллекции
        private Dictionary<TKey, ulong?> GetDeltaObjectsLocalIdsInternal()
        {
            var deltaObjectsLocalIds = new Dictionary<TKey, ulong?>(_linkedValuesDictionary.Count);
            foreach (var linkedValue in _linkedValuesDictionary)
            {
                deltaObjectsLocalIds.Add(linkedValue.Key, (linkedValue.Value as IDeltaObjectExt)?.LocalId);
            }

            return deltaObjectsLocalIds;
        }
        
        Dictionary<TKey, ulong?> IDeltaDictionaryExt<TKey, TValue>.GetDeltaObjectsLocalIds()
        {
            return GetDeltaObjectsLocalIdsInternal();
        }
        
        [ProtoAfterSerialization]
        public void afterSerialization()
        {
            if (_SerializerContext.Pool.Current.FullSerialize && IsDeltaObject)
            {
                _deltaObjectsLocalIds = null;
            }
        }
        
        public void LinkChangedDeltaObjects(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects, IEntity parentEntity)
        {
            // была полная сериализация deltaObjects
            if (_deltaObjectsLocalIds != null)
            {
                _linkedValuesDictionary = new Dictionary<TKey, TValue>(_deltaObjectsLocalIds.Count);
                foreach (var localIdPair in _deltaObjectsLocalIds)
                {
                    var deltaObject = DeltaObjectHelper.ResolveDeltaObjectWhileDeserialize<TValue>(parentEntity, deserializedObjects,
                        localIdPair.Value);
                    _linkedValuesDictionary.Add(localIdPair.Key, deltaObject);
                    CheckAddParentEntityRef(deltaObject, false);
                }

                _deltaObjectsLocalIds = null;
            }
            else
            {
                ProcessDelta(deserializedObjects, parentEntity);
            }
            
            if (!_EntityContext.Pool.Current.FullCreating)
            {
                SetDirty(false);
            }
        }

        private void ProcessDelta(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects,
            IEntity newParentEntity)
        {
            _needInvokeChanged = false;

            if (_deltaOperations == null)
                _deltaOperations = new List<DictionaryDeltaOperation<TKey, TValue>>();

            if (_deltaOperations.Count == 0)
                return;

            if (_linkedValuesDictionary == null)
                _linkedValuesDictionary = new Dictionary<TKey, TValue>();

            var opIndex = 0;
            foreach (var deltaOperation in _deltaOperations)
            {
                switch (deltaOperation.Operation)
                {
                    case DictionaryDeltaOperationType.Clear:
                    {
                        foreach (var pair in _linkedValuesDictionary)
                        {
                            var copyPair = pair;
                            invokeItemRemoved(pair.Key, pair.Value);
                            CheckDecrementParentEntityRef(pair.Value);
                        }

                        _linkedValuesDictionary.Clear();
                    }
                        break;
                    case DictionaryDeltaOperationType.Remove:
                    {
                        var key = deltaOperation.Key;
                        if (!_linkedValuesDictionary.TryGetValue(key, out var removedValue))
                        {
                            Logger.Error(
                                "DictionaryDeltaOperationType.Remove key {0} not exists. count {1}. DeltaOperationsCount {2}, opIndex {3} DeltaOperations {4} parentEntity {5} parentEntityId {6} parentDeltaObject {7} LocalId {8}",
                                deltaOperation.Key, _linkedValuesDictionary.Count, _deltaOperations.Count, opIndex,
                                string.Join(",", _deltaOperations.Select(x => x.ToString())),
                                parentEntity?.TypeName ?? "null",
                                parentEntity?.Id.ToString() ?? "null", parentDeltaObject?.GetType().Name ?? "null",
                                LocalId);
                        }

                        _linkedValuesDictionary.Remove(deltaOperation.Key);
                        invokeItemRemoved(key, removedValue);
                        if (!_linkedValuesDictionary.Any(x =>
                            EqualityComparer<TValue>.Default.Equals(removedValue, x.Value)))
                            CheckDecrementParentEntityRef(removedValue);
                    }
                        break;
                    case DictionaryDeltaOperationType.SetNew:
                    {
                        var key = deltaOperation.Key;
                        var value = getOperationValue(deserializedObjects, newParentEntity, deltaOperation);

                        if (_linkedValuesDictionary.TryGetValue(key, out var removedValue))
                        {
                            if (!_linkedValuesDictionary.Any(x =>
                                EqualityComparer<TValue>.Default.Equals(removedValue, x.Value)))
                                CheckDecrementParentEntityRef(removedValue);
                        }

                        _linkedValuesDictionary[deltaOperation.Key] = value;
                        invokeItemAddedOrUpdated(key, removedValue, value);
                        CheckAddParentEntityRef(value, false);
                    }
                        break;
                }

                opIndex++;
            }

            _deltaOperations.Clear();
        }

        private void SetDirty(bool trackChanged)
        {
            if (trackChanged)
            {
                DeltaObjectHelper.AddChangedObject(parentEntity, this);
            }
            
            // if dictionary ended up in replica set then it should be treated as changed
            // because dictionary doesn't have replication levels for every element
            // and if one of element changed then dictionary changed for every replicated set
            _replicationChangedMask = (long)ReplicationLevel.Master;
        }
        
        public void SetDirtyReplicationMask(long val)
        {
            _replicationChangedMask |= val;
        }
        
        public bool IsReplicationMaskDirty(long checkedReplicationMask)
        {
            return (_replicationChangedMask & checkedReplicationMask) > 0;
        }

        public bool IsChanged()
        {
            return _replicationChangedMask > 0;
        }

        public int ParentTypeId => parentEntity?.TypeId ?? 0;
        public Guid ParentEntityId => parentEntity?.ParentEntityId ?? Guid.Empty;
        [JsonIgnore] public IEntitiesRepository EntitiesRepository => parentEntity?.EntitiesRepository;

        [JsonIgnore] public Guid OwnerRepositoryId => parentEntity?.OwnerRepositoryId ?? Guid.Empty;

        [JsonIgnore]
        public int TypeId
        {
            get { throw new NotImplementedException(); }
        }

        void invokeItemAddedOrUpdated(TKey key, TValue oldValue, TValue value)
        {
            _needInvokeChanged = true;

            var parentEntityTypeId = parentEntity?.TypeId ?? 0;
            var parentEntityId = parentEntity?.Id ?? Guid.Empty;

            if (OnItemAddedOrUpdated != null)
            {
                foreach (var subscriber in OnItemAddedOrUpdated.GetInvocationList()
                    .Cast<DeltaDictionaryChangedDelegate<TKey, TValue>>())
                {
                    var subscriberCopy = subscriber;
                    addToEventList(() =>
                    {
                        return AsyncUtils.RunAsyncWithCheckTimeout(
                            () => subscriberCopy(
                                new DeltaDictionaryChangedEventArgs<TKey, TValue>(key, oldValue, value, this)),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () =>
                                $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}");
                    });
                }
            }

            if (_onItemAddedOrUpdatedCustomActions != null)
            {
                List<ValueTuple<object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>>> copyList = null;
                lock (_onItemAddedOrUpdatedCustomActions)
                    copyList = _onItemAddedOrUpdatedCustomActions.ToList();

                foreach (var pair in copyList)
                {
                    var subscriberCopy = pair.Item3;
                    addToEventList(() =>
                    {
                        return subscriberCopy(
                            new DeltaDictionaryChangedEventArgs<TKey, TValue>(key, oldValue, value, this));
                    });
                }
            }
        }

        void invokeItemRemoved(TKey key, TValue value)
        {
            _needInvokeChanged = true;

            var parentEntityTypeId = parentEntity?.TypeId ?? 0;
            var parentEntityId = parentEntity?.Id ?? Guid.Empty;

            if (OnItemRemoved != null)
            {
                foreach (var subscriber in OnItemRemoved.GetInvocationList())
                {
                    var subscriberCopy = (DeltaDictionaryChangedDelegate<TKey, TValue>) subscriber;
                    addToEventList(() =>
                    {
                        return AsyncUtils.RunAsyncWithCheckTimeout(
                            async () => await subscriberCopy(
                                new DeltaDictionaryChangedEventArgs<TKey, TValue>(key, value, value, this)),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () =>
                                $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}");
                    });
                }
            }

            if (_onItemRemovedCustomActions != null)
            {
                List<ValueTuple<object, MethodInfo, DeltaDictionaryChangedDelegate<TKey, TValue>>> copyList = null;
                lock (_onItemRemovedCustomActions)
                    copyList = _onItemRemovedCustomActions.ToList();

                foreach (var pair in copyList)
                {
                    var subscriberCopy = pair.Item3;
                    addToEventList(() =>
                    {
                        return subscriberCopy(
                            new DeltaDictionaryChangedEventArgs<TKey, TValue>(key, value, value, this));
                    });
                }
            }
        }

        void invokeChanged()
        {
            if (OnChanged != null)
            {
                var parentEntityTypeId = parentEntity?.TypeId ?? 0;
                var parentEntityId = parentEntity?.Id ?? Guid.Empty;

                foreach (var subscriber in OnChanged.GetInvocationList().Cast<DeltaDictionaryChangedDelegate>())
                {
                    var subscriberCopy = subscriber;
                    addToEventList(() =>
                    {
                        return AsyncUtils.RunAsyncWithCheckTimeout(
                            async () => await subscriberCopy(new DeltaDictionaryChangedEventArgs(this)),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () =>
                                $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}");
                    });
                }
            }
        }

        private void addToEventList(Func<Task> task)
        {
            if (_eventsList == null)
                _eventsList = new List<Func<Task>>();
            _eventsList.Add(task);
        }

        bool IDeltaObject.NeedFireEvents()
        {
            if (_needInvokeChanged || _eventsList != null && _eventsList.Count > 0 || _replicationChangedMask > 0)
                return true;

            return false;
        }

        void IDeltaObject.ClearDelta()
        {
            if (_replicationChangedMask == 0)
                return;

            _needInvokeChanged = false;
            _deltaOperations.Clear();
            _replicationChangedMask = 0;
            _deltaObjectsLocalIds = null;
            
            _operationsHistory.Clear();
        }

        void IDeltaObject.GetAllLinkedEntities(long replicationMask,
            List<(long level, IEntityRef entityRef)> entities,
            long currentLevel,
            bool onlyDbEntities)
        {
            if (IsDeltaObject)
            {
                foreach (var value in _linkedValuesDictionary.Values)
                {
                    if (value != null)
                    {
                        ((IDeltaObject) value).GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
                    }
                }
            }
            
            if (_isEntityRef)
            {
                foreach (var value in _linkedValuesDictionary.Values)
                {
                    if (value != null)
                    {
                        entities.Add((currentLevel, (IEntityRef)value));
                    }
                }
            }
        }

        void IDeltaObjectExt.LinkEntityRefs(IEntitiesRepository repository)
        {
            if (_isEntityRef)
                foreach (var pair in _linkedValuesDictionary)
                {
                    if (pair.Value == null)
                        continue;

                    var oldRef = (IEntityRef) pair.Value;
                    var newRef =
                        (TValue) ((IEntitiesRepositoryExtension) repository).GetRef(oldRef.GetEntityInterfaceType(),
                            oldRef.Id);
                    _linkedValuesDictionary[pair.Key] = newRef;
                }
        }

        public void MarkAllChanged()
        {
        }

        void IDeltaObject.SubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            throw new NotImplementedException();
        }

        void IDeltaObject.UnsubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            throw new NotImplementedException();
        }

        void IDeltaObject.UnsubscribePropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        void IDeltaObject.UnsubscribeAll()
        {
            throw new NotImplementedException();
        }

        void IDeltaObject.ProcessEvents(List<Func<Task>> container)
        {
            if (_needInvokeChanged)
            {
                _needInvokeChanged = false;
                invokeChanged();
            }

            if (_eventsList != null && _eventsList.Count > 20)
                Logger.Error("{3} Too many events actions {0} on entity {1} id {2}", _eventsList.Count,
                    parentEntity?.TypeName ?? "none", parentEntity?.Id.ToString() ?? "none",
                    this.GetType().GetFriendlyName());
            
            if (_eventsList != null)
            {
                container.AddRange(_eventsList);
                _eventsList?.Clear();
            }
        }

        public bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            var index = address;
            if (this.Count <= index)
                return false;

            var list = _linkedValuesDictionary.OrderBy(x => x.Key).ToList();
            object currProperty = list[index].Value;
            if (currProperty == null)
                return false;

            property = (T) currProperty;
            return true;
        }

        public T To<T>() where T : IDeltaObject
        {
            throw new NotImplementedException();
        }

        public IDeltaObject GetReplicationLevel(ReplicationLevel replicationLevel)
        {
            throw new NotImplementedException();
        }


        public IEntity GetParentEntity() => parentEntity;
        public void FillReplicationSetRecursive(Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSets, HashSet<ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            if (IsDeltaObject)
            {
                foreach (var pair in _linkedValuesDictionary)
                {
                    DeltaObjectHelper.FillReplicationSetRecursive(replicationSets,
                        traverseReplicationLevels,
                        (IDeltaObject) pair.Value, 
                        currentLevel, 
                        true,
                        withBsonIgnore);
                }
            }
        }

        public IDeltaObjectExt GetParentObject() => parentDeltaObject;

        public void Visit(Action<IDeltaObject> visitor)
        {
            visitor(this);

            if (!IsDeltaObject)
                return;

            foreach (var pair in _linkedValuesDictionary)
                if (pair.Value != null)
                    ((IDeltaObjectExt) pair.Value).Visit(visitor);
        }

        public void SetParentDeltaObject(IDeltaObjectExt deltaObject)
        {
            DeltaObjectHelper.SetParentDeltaObject(this, deltaObject);
        }

        void IDeltaObjectExt.Downgrade(long mask)
        {
            if (IsDeltaObject)
                foreach (var pair in _linkedValuesDictionary)
                    ((IDeltaObjectExt) pair.Value).Downgrade(mask);
        }

        public bool ContainsReplicationLevel(long mask)
        {
            if (parentEntity == null)
                return (LastParentEntityReplicationMask & mask) == mask;

            return ((IDeltaObjectExt) parentEntity).ContainsReplicationLevel(mask);
        }

        void IHasRandomFill.Fill(int depthCount, Random random, bool withReadonly)
        {
            //throw new NotImplementedException();
        }

        void ICanRandomFill.Fill(int depthCount, bool withReadonly)
        {
            //throw new NotImplementedException();
        }

        void checkSubscribePart(TValue value)
        {
            if (value != null)
            {
                if (_isEntityRef)
                {
                    var replicationLevel = DeltaObjectHelper.GetReplicationLevel(parentEntity, this);
                    if (replicationLevel != null)
                    {
                        ((IEntityExt) parentEntity).AddEntityRef(((IEntityRefExt) value).GetEntity(), replicationLevel.Value);
                    }
                }
            }
        }

        void checkUnsubscribePart(TValue value)
        {
            if (value != null)
            {
                if (_isEntityRef)
                {
                    var replicationLevel = DeltaObjectHelper.GetReplicationLevel(parentEntity, this);
                    if (replicationLevel != null)
                    {
                        ((IEntityExt) parentEntity).RemoveEntityRef(((IEntityRefExt) value).GetEntity(), replicationLevel.Value);
                    }
                }
            }
        }

        void CheckRemoveFromReplicationSet(TValue value)
        {
            if (parentEntity != null && IsDeltaObject)
            {
                if (value != null)
                {
                    DeltaObjectHelper.RemoveDeltaObjectFromReplicationSet(parentEntity, this, (IDeltaObject) value,
                        ReplicationLevel.Always);
                }
            }
        }

        void CheckAddToReplicationSet(TValue value)
        {
            if (parentEntity != null && IsDeltaObject)
            {
                if (value != null)
                {
                    DeltaObjectHelper.AddDeltaObjectToReplicationSet(parentEntity, this, (IDeltaObject) value,
                        ReplicationLevel.Always);
                }
            }
        }

        void CheckAddParentEntityRef(TValue value, bool trackChanged)
        {
            if (IsDeltaObject)
            {
                ((IDeltaObjectExt) value)?.SetParentDeltaObject(this);
                if (parentEntity != null)
                {
                    ((IDeltaObjectExt) value)?.IncrementParentRefs(parentEntity, trackChanged);
                }
            }
        }

        void CheckDecrementParentEntityRef(TValue value)
        {
            if (IsDeltaObject && _parentEntity != null)
            {
                ((IDeltaObjectExt) value)?.DecrementParentRefs();
            }
        }
        
        private readonly struct OldDictionaryOperation
        {
            public OldDictionaryOperation(int operationIndex,DictionaryDeltaOperation<TKey, TValue> operation, bool oldValueExisted)
            {
                OperationIndex = operationIndex;
                Operation = operation;
                OldValueExisted = oldValueExisted;
            }
            public int OperationIndex { get; }
            
            public DictionaryDeltaOperation<TKey, TValue> Operation { get; }
            
            public bool OldValueExisted { get; }
        }
    }

    [ProtoContract]
    public enum DictionaryDeltaOperationType
    {
        [ProtoEnum] None,
        [ProtoEnum] SetNew,
        [ProtoEnum] Remove,
        [ProtoEnum] Clear,
    }

    [ProtoContract]
    public class DictionaryDeltaOperation<TKey, TValue>
    {
        private ulong? _deserializedLocalId;
        private bool _deserializedLocalIdSet;

        [ProtoMember(1)] public DictionaryDeltaOperationType Operation;

        [ProtoMember(2)] public TKey Key;

        [ProtoMember(3)] public DeltaValueWrapper<TValue> Value;

        [ProtoMember(4)] public DeltaReferenceWrapper<TValue> ValueRef;

        [ProtoMember(5)]
        public ulong? DeltaObjectLocalId
        {
            get => _deserializedLocalIdSet ? _deserializedLocalId : RuntimeDeltaObject?.LocalId;
            set
            {
                _deserializedLocalIdSet = true;
                _deserializedLocalId = value;
            }
        }

        // нужен так как во время наполнения delta operations DeltaObject может быть без parent и не иметь LocalId
        public IDeltaObjectExt RuntimeDeltaObject;

        public override string ToString()
        {
            return
                $"<Operation {Operation.ToString()} key {Key} keytype {typeof(TKey).GetFriendlyName()} valuetype {typeof(TValue).GetFriendlyName()}>";
        }
    }

    public interface IDeltaDictionaryExt
    {
    }

    public interface IDeltaDictionaryExt<TKey, TValue>
    {
        void SubscribeCustomAddOrUpdateAction(object key, MethodInfo key2,
            DeltaDictionaryChangedDelegate<TKey, TValue> action);

        void UnsubscribeCustomAddOrUpdateAction(object key, MethodInfo key2);

        void SubscribeCustomRemoveAction(object key, MethodInfo key2,
            DeltaDictionaryChangedDelegate<TKey, TValue> action);

        void UnsubscribeCustomRemoveAction(object key, MethodInfo key2);
        
        Dictionary<TKey, ulong?> GetDeltaObjectsLocalIds();
    }
}