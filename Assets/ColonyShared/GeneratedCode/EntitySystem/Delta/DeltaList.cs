using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;
using SharedCode.OurSimpleIoC;
using SharedCode.Refs;
using SharedCode.Serializers;
using SharedCode.Logging;
using Assets.Src.ResourcesSystem.Base;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SharedCode.Serializers.Protobuf;
using GeneratedCode.EntitySystem;
using JetBrains.Annotations;
using ResourcesSystem.Base;
using SharedCode.Refs.Operations;

namespace SharedCode.EntitySystem.Delta
{
    [ProtoContract(IgnoreListHandling = true)]
    [JsonObject]
    [BsonDiscriminator("DeltaList")]
    public class DeltaList<T> : IDeltaList<T>, IDeltaObjectExt, IDeltaListExt<T>, IDeltaListExt, IReadOnlyList<T>
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

        public static readonly bool IsDeltaObject = typeof(IDeltaObject).IsAssignableFrom(typeof(T));

        private static readonly bool _isEntityRef =
            typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(EntityRef<>);

        private static readonly bool _isValueType =
            typeof(T).IsValueType || typeof(T).IsSubclassOf(typeof(BaseResource));

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

        [ProtoIgnore] [JsonIgnore] public long LastParentEntityReplicationMask { get; set; } = 0;

        public short ParentEntityRefCount { get; set; }

        private long _replicationChangedMask = 0;

        public DeltaList()
        {
        }

        public DeltaList(IEnumerable<T> list) : this()
        {
            if (list != null)
            {
                foreach (var l in list)
                {
                    Add(l);
                }
            }
        }

        public static DeltaList<T> CreateFromIds(List<ulong?> deltaObjectsIds)
        {
            var deltaList = new DeltaList<T>
            {
                _deltaObjectsLocalIds = deltaObjectsIds
            };
            
            return deltaList;
        }
        
        public static DeltaList<T> CreateFromRawObjects(List<T> deltaObjectsIds)
        {
            var deltaList = new DeltaList<T>
            {
                _linkedValuesList = deltaObjectsIds
            };
            
            return deltaList;
        }

        [ProtoMember(3, OverwriteList = true)]
        public List<T> _rawValueslist
        {
            get => _linkedValuesList;
            set => _linkedValuesList = value;
        }

        [ProtoMember(4, OverwriteList = true)] public List<ulong?> _deltaObjectsLocalIds { get; set; }

        [ProtoIgnore]
        private List<T> _linkedValuesList = new List<T>();

        [ProtoIgnore] public List<T> Items => _linkedValuesList;

        [ProtoMember(5, OverwriteList = true)]
        public List<ListDeltaOperation<T>> _deltaOperations = new List<ListDeltaOperation<T>>();

        // сюда попадают ДельтаОбьекты, которы удалились из листа
        // мы не убираем их парента сразу, чтобы они попали в репликацию, иначе возможна ситуация
        // что было Add, Remove и реплика не сможет их накатить
        [ProtoIgnore] private List<IDeltaObject> _deferredRemoveDeltaObjects = new List<IDeltaObject>();

        // public readonly struct OldDeltaOperation
        // {
        //     public int OperationListIndex { get; }
        //     public ListDeltaOperation<T> Operation { get; }
        //
        //     public OldDeltaOperation(int operationListIndex, ListDeltaOperation<T> operation)
        //     {
        //         OperationListIndex = operationListIndex;
        //         Operation = operation;
        //     }
        // }
        
        // [ProtoIgnore]
        // private Dictionary<int, OldDeltaOperation> _deltaOperationsMap = new Dictionary<int, OldDeltaOperation>();

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
        private List<ValueTuple<object, MethodInfo, DeltaListChangedDelegate<T>>> _onItemAddedCustomActions;

        [ProtoIgnore]
        private List<ValueTuple<object, MethodInfo, DeltaListChangedDelegate<T>>> _onItemRemovedCustomActions;

        [ProtoIgnore]
        public string OnItemAddedSubscribers
        {
            get
            {
                var sb = new StringBuilder();
                if (_onItemAddedCustomActions != null)
                    lock (_onItemAddedCustomActions)
                        if (_onItemAddedCustomActions.Count > 0)
                        {
                            sb.AppendFormat("{1}subscribers count: {0}", _onItemAddedCustomActions.Count,
                                _onItemAddedCustomActions.Count >= ServerCoreRuntimeParameters.EventSubscribersWarnCount
                                    ? "WARN "
                                    : "").AppendLine();
                            foreach (var customAction in _onItemAddedCustomActions)
                                sb.AppendFormat("<obj:{0} method:{1} hashcode:{2}>",
                                    customAction.Item3.Target.GetType().Name, customAction.Item3.Method.Name,
                                    customAction.Item3.Target.GetHashCode());
                        }

                sb.Append("-");

                if (OnItemAdded != null)
                {
                    var subscribers = OnItemAdded.GetInvocationList();
                    sb.AppendFormat("{1}subscribers count: {0}", subscribers.Length,
                            subscribers.Length >= ServerCoreRuntimeParameters.EventSubscribersWarnCount ? "WARN " : "")
                        .AppendLine();
                    foreach (var subscriber in subscribers.Cast<DeltaListChangedDelegate<T>>())
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
                    foreach (var subscriber in subscribers.Cast<DeltaListChangedDelegate<T>>())
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
                    foreach (var subscriber in subscribers.Cast<DeltaListChangedDelegate>())
                        sb.AppendFormat("<obj:{0} method:{1} hashcode:{2}>", subscriber.Target.GetType().Name,
                            subscriber.Method.Name, subscriber.Target.GetHashCode());
                }

                return sb.ToString();
            }
        }

        public event DeltaListChangedDelegate<T> OnItemAdded;

        public event DeltaListChangedDelegate<T> OnItemRemoved;

        public event DeltaListChangedDelegate OnChanged;

        public IDeltaList<T1> ToDeltaList<T1>() where T1 : IBaseDeltaObjectWrapper
        {
            var wrapper = (IDeltaList<T1>) Activator.CreateInstance(typeof(DeltaListWrapper<,>)
                    .MakeGenericType(typeof(T), typeof(T1)),
                this);
            return wrapper;
        }
        
        void IDeltaListExt<T>.SubscribeCustomAddAction(object key, MethodInfo key2, DeltaListChangedDelegate<T> action)
        {
            if (_onItemAddedCustomActions == null)
                lock (_linkedValuesList)
                    if (_onItemAddedCustomActions == null)
                        _onItemAddedCustomActions = new List<(object, MethodInfo, DeltaListChangedDelegate<T>)>();

            lock (_onItemAddedCustomActions)
                _onItemAddedCustomActions.Add(
                    new ValueTuple<object, MethodInfo, DeltaListChangedDelegate<T>>(key, key2, action));
        }

        void IDeltaListExt<T>.UnsubscribeCustomAddAction(object key, MethodInfo key2)
        {
            if (_onItemAddedCustomActions == null)
                return;

            lock (_onItemAddedCustomActions)
            {
                var tuple = _onItemAddedCustomActions.FirstOrDefault(x => x.Item1 == key && x.Item2 == key2);
                if (tuple.Item1 != null)
                    _onItemAddedCustomActions.Remove(tuple);
            }
        }

        void IDeltaListExt<T>.SubscribeCustomRemoveAction(object key, MethodInfo key2,
            DeltaListChangedDelegate<T> action)
        {
            if (_onItemRemovedCustomActions == null)
                lock (_linkedValuesList)
                    if (_onItemRemovedCustomActions == null)
                        _onItemRemovedCustomActions = new List<(object, MethodInfo, DeltaListChangedDelegate<T>)>();

            lock (_onItemRemovedCustomActions)
                _onItemRemovedCustomActions.Add(
                    new ValueTuple<object, MethodInfo, DeltaListChangedDelegate<T>>(key, key2, action));
        }

        void IDeltaListExt<T>.UnsubscribeCustomRemoveAction(object key, MethodInfo key2)
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

        public IEnumerator<T> GetEnumerator()
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            return _linkedValuesList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void setOperationValue(ListDeltaOperation<T> operation, T value)
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
                operation.Value = new DeltaValueWrapper<T>(value);
            }
            else
            {
                operation.ValueRef = new DeltaReferenceWrapper<T>(value);
            }
        }

        T getOperationValue(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects,
            IEntity newParentEntity,
            ListDeltaOperation<T> deltaOperation)
        {
            if (deltaOperation.Value != null)
            {
                return deltaOperation.Value.Value;
            }

            if (deltaOperation.ValueRef != null)
            {
                return deltaOperation.ValueRef.Value;
            }

            return DeltaObjectHelper.ResolveDeltaObjectWhileDeserialize<T>(newParentEntity, deserializedObjects,
                deltaOperation.DeltaObjectLocalId);
        }

        public void Add(T item) => Insert(Count, item);

        public void Clear()
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);

            foreach (var value in _linkedValuesList)
            {
                var copyValue = value;
                invokeItemRemoved(value, 0);
                checkUnsubscribePart(value);
                CheckRemoveFromReplicationSet(value);
                CheckDecrementParentEntityRef(value);
            }

            _linkedValuesList.Clear();

            // потому что не важно все, что происходило до Clear
            _deltaOperations.Clear();
            addDeltaOperation(new ListDeltaOperation<T>
            {
                Operation = ListDeltaOperationType.Clear,
            });
        }

        public bool Contains(T item)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);

            return _linkedValuesList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);

            _linkedValuesList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);

            var index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);

            return true;
        }

        [JsonIgnore]
        public int Count
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return _linkedValuesList.Count;
            }
        }

        [JsonIgnore]
        public bool IsReadOnly
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return ((ICollection<T>) _linkedValuesList).IsReadOnly;
            }
        }

        public int IndexOf(T item)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
            return _linkedValuesList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);
            _linkedValuesList.Insert(index, item);

            CheckAddParentEntityRef(item, true);
            CheckAddToReplicationSet(item);
            var operation = new ListDeltaOperation<T>
            {
                Operation = ListDeltaOperationType.InsertNew,
                Index = index
            };
            setOperationValue(operation, item);
            addDeltaOperation(operation);
            invokeItemAdded(item, index);
            checkSubscribePart(item);
        }

        public void RemoveAt(int index)
        {
            ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);
            var itemRemoved = _linkedValuesList[index];
            _linkedValuesList.RemoveAt(index);

            addDeltaOperation(new ListDeltaOperation<T>
            {
                Operation = ListDeltaOperationType.Remove,
                Index = index
            });
            invokeItemRemoved(itemRemoved, index);
            
            checkUnsubscribePart(itemRemoved);
            CheckDeferDecrementParentEntityRef(itemRemoved);
        }

        private void ProcessDeferredRemoves()
        {
            foreach (var deferredRemoveDeltaObject in _deferredRemoveDeltaObjects)
            {
                CheckRemoveFromReplicationSet((T) deferredRemoveDeltaObject);
                ((IDeltaObjectExt) deferredRemoveDeltaObject)?.DecrementParentRefs();
            }

            _deferredRemoveDeltaObjects.Clear();
        }

        public T this[int index]
        {
            get
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Read);
                return _linkedValuesList[index];
            }
            set
            {
                ((IEntityExt) parentEntity)?.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType.Write);
                var oldValue = _linkedValuesList[index];
                if (EqualityComparer<T>.Default.Equals(oldValue, value))
                    return;

                var itemRemoved = oldValue;

                _linkedValuesList[index] = value;
                invokeItemRemoved(itemRemoved, index);
                
                checkUnsubscribePart(itemRemoved);
                CheckDeferDecrementParentEntityRef(itemRemoved);

                CheckAddParentEntityRef(value, true);
                var operation = new ListDeltaOperation<T>
                {
                    Operation = ListDeltaOperationType.ReplaceNew,
                    Index = index,
                };
                setOperationValue(operation, value);
                addDeltaOperation(operation);
                invokeItemAdded(value, index);
                checkSubscribePart(value);
            }
        }

        private void addDeltaOperation(ListDeltaOperation<T> operation)
        {
            _deltaOperations.Add(operation);
            SetDirty(true);
        }

        public bool _rawValueslistSpecified
        {
            get { return _SerializerContext.Pool.Current.FullSerialize && !IsDeltaObject; }
            set {  }
        }

        public bool _deltaObjectsLocalIdsSpecified
        {
            get { return _SerializerContext.Pool.Current.FullSerialize && IsDeltaObject; }
            set { }
        }

        public bool ShouldSerialize_deltaOperations()
        {
            if (_SerializerContext.Pool.Current.FullSerialize)
                return false;

            return _deltaOperations.Count > 0;
        }

        private void SetDirty( bool trackChanged)
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

        public int ParentTypeId => parentEntity?.ParentTypeId ?? 0;
        public Guid ParentEntityId => parentEntity?.ParentEntityId ?? Guid.Empty;
        [JsonIgnore] public IEntitiesRepository EntitiesRepository => parentEntity?.EntitiesRepository;

        [JsonIgnore] public Guid OwnerRepositoryId => parentEntity?.OwnerRepositoryId ?? Guid.Empty;

        [JsonIgnore]
        public int TypeId
        {
            get { throw new NotImplementedException(); }
        }

        void invokeItemAdded(T value, int index)
        {
            _needInvokeChanged = true;

            var parentEntityTypeId = parentEntity?.TypeId ?? 0;
            var parentEntityId = parentEntity?.Id ?? Guid.Empty;

            if (OnItemAdded != null)
            {
                foreach (var subscriber in OnItemAdded.GetInvocationList().Cast<DeltaListChangedDelegate<T>>())
                {
                    var subscriberCopy = subscriber;
                    addToEventList(() =>
                        AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(new DeltaListChangedEventArgs<T>(value, index,this)),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () =>
                                $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    );
                }
            }

            if (_onItemAddedCustomActions != null)
            {
                List<ValueTuple<object, MethodInfo, DeltaListChangedDelegate<T>>> copyList = null;
                lock (_onItemAddedCustomActions)
                    copyList = _onItemAddedCustomActions.ToList();

                foreach (var pair in copyList)
                {
                    var subscriberCopy = pair.Item3;
                    addToEventList(() =>
                        subscriberCopy(new DeltaListChangedEventArgs<T>(value, index,this))
                    );
                }
            }
        }

        void invokeItemRemoved(T value, int index)
        {
            _needInvokeChanged = true;

            var parentEntityTypeId = parentEntity?.TypeId ?? 0;
            var parentEntityId = parentEntity?.Id ?? Guid.Empty;

            if (OnItemRemoved != null)
            {
                foreach (var subscriber in OnItemRemoved.GetInvocationList().Cast<DeltaListChangedDelegate<T>>())
                {
                    var subscriberCopy = subscriber;
                    addToEventList(() =>
                        AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(new DeltaListChangedEventArgs<T>(value, index, this)),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () =>
                                $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    );
                }
            }

            if (_onItemRemovedCustomActions != null)
            {
                List<ValueTuple<object, MethodInfo, DeltaListChangedDelegate<T>>> copyList = null;
                lock (_onItemRemovedCustomActions)
                    copyList = _onItemRemovedCustomActions.ToList();

                foreach (var pair in copyList)
                {
                    var subscriberCopy = pair.Item3;
                    addToEventList(() =>
                        subscriberCopy(new DeltaListChangedEventArgs<T>(value, index, this))
                    );
                }
            }
        }

        void invokeChanged()
        {
            if (OnChanged != null)
            {
                var parentEntityTypeId = parentEntity?.TypeId ?? 0;
                var parentEntityId = parentEntity?.Id ?? Guid.Empty;

                foreach (var subscriber in OnChanged.GetInvocationList().Cast<DeltaListChangedDelegate>())
                {
                    var subscriberCopy = subscriber;
                    addToEventList(() =>
                        AsyncUtils.RunAsyncWithCheckTimeout(
                            () => subscriberCopy(new DeltaListChangedEventArgs(this)),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () =>
                                $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    );
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
            
            ProcessDeferredRemoves();
        }

        void IDeltaObject.GetAllLinkedEntities(long replicationMask,
            List<(long level, IEntityRef entityRef)> entities,
            long currentLevel,
            bool onlyDbEntities)
        {
            if (IsDeltaObject)
            {
                foreach (var value in _linkedValuesList)
                {
                    if (value != null)
                        ((IDeltaObject) value).GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
                }
            }

            if (_isEntityRef)
            {
                foreach (var value in _linkedValuesList)
                {
                    if (value != null)
                    {
                        entities.Add((currentLevel, (IEntityRef) value));
                    }
                }
            }
        }

        void IDeltaObjectExt.LinkEntityRefs(IEntitiesRepository repository)
        {
            if (_isEntityRef)
                for (int i = 0; i < _linkedValuesList.Count; i++)
                {
                    if (_linkedValuesList[i] == null)
                        continue;

                    var oldRef = (IEntityRef) _linkedValuesList[i];
                    var newRef =
                        (T) ((IEntitiesRepositoryExtension) repository).GetRef(oldRef.GetEntityInterfaceType(),
                            oldRef.Id);
                    _linkedValuesList[i] = newRef;
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
                _eventsList.Clear();
            }
        }

        public bool TryGetProperty<T1>(int address, out T1 property)
        {
            property = default(T1);
            if (this.Count <= address)
                return false;

            object currProperty = this[address];
            property = (T1) currProperty;
            return true;
        }

        public IDeltaObject GetReplicationLevel(ReplicationLevel replicationLevel)
        {
            throw new NotImplementedException();
        }


        public T1 To<T1>() where T1 : IDeltaObject
        {
            throw new NotImplementedException();
        }

        public IEntity GetParentEntity() => parentEntity;

        public void FillReplicationSetRecursive(
            Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSets, 
            HashSet<ReplicationLevel> traverseReplicationLevels, 
            ReplicationLevel currentLevel,
            bool withBsonIgnore)
        {
            if (IsDeltaObject)
            {
                foreach (var element in _linkedValuesList)
                {
                    DeltaObjectHelper.FillReplicationSetRecursive(
                        replicationSets,
                        traverseReplicationLevels,
                        (IDeltaObject) element,
                        currentLevel,
                        // ye;yj
                        true,
                        withBsonIgnore);
                }

                if (withBsonIgnore)
                {
                    foreach (var deferredRemoveDeltaObject in _deferredRemoveDeltaObjects)
                    {
                        DeltaObjectHelper.FillReplicationSetRecursive(
                            replicationSets,
                            traverseReplicationLevels,
                            deferredRemoveDeltaObject,
                            currentLevel,
                            true,
                            false);
                    }
                }
            }
        }

        [ProtoBeforeSerialization]
        public void beforeSerialization()
        {
            if (_SerializerContext.Pool.Current.FullSerialize && IsDeltaObject)
            {
                _deltaObjectsLocalIds = GetDeltaObjectsLocalIdsInternal();
            }
        }
        
        [ProtoAfterSerialization]
        public void afterSerialization()
        {
            if (_SerializerContext.Pool.Current.FullSerialize && IsDeltaObject)
            {
                _deltaObjectsLocalIds = null;
            }
        }

        private  List<ulong?> GetDeltaObjectsLocalIdsInternal()
        {
            var deltaObjectsLocalIds = new List<ulong?>(_linkedValuesList.Count);
            foreach (var linkedValue in _linkedValuesList)
            {
                deltaObjectsLocalIds.Add((linkedValue as IDeltaObjectExt)?.LocalId);
            }

            return deltaObjectsLocalIds;
        }
        
        // делается только при полной сериализации и сохранении в бд, поэтому делается отдельно,
        // а не наполняется в процессе изменения коллекции
        List<ulong?> IDeltaListExt<T>.GetDeltaObjectsLocalIds()
        {
            return GetDeltaObjectsLocalIdsInternal();
        }
        
        public void LinkChangedDeltaObjects(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects,
            IEntity parentEntity)
        {
            // была полная сериализация deltaObjects
            if (_deltaObjectsLocalIds != null)
            {
                _linkedValuesList = new List<T>(_deltaObjectsLocalIds.Count);
                foreach (var localId in _deltaObjectsLocalIds)
                {
                    var deltaObject = DeltaObjectHelper.ResolveDeltaObjectWhileDeserialize<T>(parentEntity,
                        deserializedObjects,
                        localId);
                    _linkedValuesList.Add(deltaObject);
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
                _deltaOperations = new List<ListDeltaOperation<T>>();

            if (_deltaOperations.Count == 0)
                return;

            if (_linkedValuesList == null)
                _linkedValuesList = new List<T>();

            var opIndex = 0;
            foreach (var deltaOperation in _deltaOperations)
            {
                switch (deltaOperation.Operation)
                {
                    case ListDeltaOperationType.Clear:
                    {
                        foreach (var value in _linkedValuesList)
                        {
                            CheckDecrementParentEntityRef(value);
                            invokeItemRemoved(value, 0);
                        }

                        _linkedValuesList.Clear();
                    }
                        break;
                    case ListDeltaOperationType.InsertNew:
                    {
                        if (deltaOperation.Index > _linkedValuesList.Count)
                        {
                            Logger.Error(
                                "ListDeltaOperationType.Add  index {0} > count {1}. DeltaOperationsCount {2}, opIndex {3} DeltaOperations {4} parentEntity {5} parentEntityId {6} parentDeltaObject {7} LocalId {8}",
                                deltaOperation.Index, _linkedValuesList.Count, _deltaOperations.Count, opIndex,
                                string.Join(",", _deltaOperations.Select(x => x.ToString())),
                                parentEntity?.TypeName ?? "null", parentEntity?.Id.ToString() ?? "null",
                                parentDeltaObject?.GetType().Name ?? "null", LocalId);
                        }

                        var value = getOperationValue(deserializedObjects, newParentEntity, deltaOperation);
                        _linkedValuesList.Insert(deltaOperation.Index, value);
                        invokeItemAdded(value, deltaOperation.Index);
                        CheckAddParentEntityRef(value, false);
                    }
                        break;

                    case ListDeltaOperationType.Remove:
                    {
                        if (deltaOperation.Index >= _linkedValuesList.Count)
                        {
                            Logger.Error(
                                "ListDeltaOperationType.Remove  index {0} >= count {1}. DeltaOperationsCount {2}, opIndex {3} DeltaOperations {4} parentEntity {5} parentEntityId {6} parentDeltaObject {7} LocalId {8}",
                                deltaOperation.Index, _linkedValuesList.Count, _deltaOperations.Count, opIndex,
                                string.Join(",", _deltaOperations.Select(x => x.ToString())),
                                parentEntity?.TypeName ?? "null", parentEntity?.Id.ToString() ?? "null",
                                parentDeltaObject?.GetType().Name ?? "null", LocalId);
                        }

                        var value = _linkedValuesList[deltaOperation.Index];
                        _linkedValuesList.RemoveAt(deltaOperation.Index);
                        if (_linkedValuesList.IndexOf(value) == -1)
                            CheckDecrementParentEntityRef(value);
                        invokeItemRemoved(value, deltaOperation.Index);
                    }
                        break;
                    case ListDeltaOperationType.ReplaceNew:
                    {
                        if (deltaOperation.Index >= _linkedValuesList.Count)
                        {
                            Logger.Error(
                                "ListDeltaOperationType.ReplaceNew  Index {0} >= count {1}. DeltaOperationsCount {2}, opIndex {3} DeltaOperations {4} parentEntity {5} parentEntityId {6} parentDeltaObject {7} LocalId {8}",
                                deltaOperation.Index, _linkedValuesList.Count, _deltaOperations.Count, opIndex,
                                string.Join(",", _deltaOperations.Select(x => x.ToString())),
                                parentEntity?.TypeName ?? "null", parentEntity?.Id.ToString() ?? "null",
                                parentDeltaObject?.GetType().Name ?? "null", LocalId);
                        }

                        var oldVal = _linkedValuesList[deltaOperation.Index];
                        var value = getOperationValue(deserializedObjects, newParentEntity, deltaOperation);

                        _linkedValuesList[deltaOperation.Index] = value;

                        if (_linkedValuesList.IndexOf(oldVal) == -1)
                            CheckDecrementParentEntityRef(oldVal);
                        invokeItemRemoved(oldVal, deltaOperation.Index);

                        CheckAddParentEntityRef(value, false);
                        invokeItemAdded(value, deltaOperation.Index);
                    }
                        break;
                }

                opIndex++;
            }

            _deltaOperations.Clear();
            SetDirty(false);
        }

        public IDeltaObjectExt GetParentObject() => parentDeltaObject;

        public void Visit(Action<IDeltaObject> visitor)
        {
            visitor(this);

            if (!IsDeltaObject)
                return;

            foreach (var value in _linkedValuesList)
                if (value != null)
                    ((IDeltaObjectExt) value).Visit(visitor);
        }

        public void SetParentDeltaObject(IDeltaObjectExt parentDeltaObj)
        {
            DeltaObjectHelper.SetParentDeltaObject(this, parentDeltaObj);
        }

        public void DecrementParentRefs()
        {
            DeltaObjectHelper.DecrementParentRefs(this);
            if (!DeltaObjectHelper.HasParentRef(this) && IsDeltaObject)
            {
                foreach (var value in _linkedValuesList)
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
                foreach (var value in _linkedValuesList)
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
                foreach (var value in _linkedValuesList)
                {
                    DeltaObjectHelper.ReplicationLevelActualize(parentEntity, (IDeltaObject) value, ReplicationLevel.Always, actualParentLevel, oldParentLevel);
                }
            }
        }
        
        void IDeltaObjectExt.Downgrade(long mask)
        {
            if (IsDeltaObject)
                foreach (var value in _linkedValuesList)
                    ((IDeltaObjectExt) value).Downgrade(mask);
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

        void checkSubscribePart(T value)
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

        void checkUnsubscribePart(T value)
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
        
        void CheckRemoveFromReplicationSet(T value)
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

        void CheckAddToReplicationSet(T value)
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

        private void CheckAddParentEntityRef(T value, bool trackChanged)
        {
            if (IsDeltaObject)
            {
                ((IDeltaObjectExt) value)?.SetParentDeltaObject(this);
                if (_parentEntity != null)
                {
                    ((IDeltaObjectExt) value)?.IncrementParentRefs(_parentEntity, trackChanged);
                }
            }
        }

        private void CheckDeferDecrementParentEntityRef(T value)
        {
            if (IsDeltaObject && _parentEntity != null)
            {
                _deferredRemoveDeltaObjects.Add((IDeltaObject)value);
            }
        }
        
        private void CheckDecrementParentEntityRef(T value)
        {
            if (IsDeltaObject && _parentEntity != null)
            {
                ((IDeltaObjectExt) value)?.DecrementParentRefs();
            }
        }

    }

    [ProtoContract]
    public enum ListDeltaOperationType
    {
        [ProtoEnum] None,
        [ProtoEnum] InsertNew,
        [ProtoEnum] ReplaceNew,
        [ProtoEnum] Remove,
        [ProtoEnum] Clear,
    }

    [ProtoContract]
   public  class ListDeltaOperation<T>
    {
        private ulong? _deserializedLocalId;
        private bool _deserializedLocalIdSet;

        [ProtoMember(1)] public ListDeltaOperationType Operation;

        [ProtoMember(2)] public int Index;

        [ProtoMember(3)] public DeltaValueWrapper<T> Value;

        [ProtoMember(4)] public DeltaReferenceWrapper<T> ValueRef;

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
                $"<Operation {Operation.ToString()} index {Index} type {typeof(T).GetFriendlyName()}>";
        }
    }

    public interface IDeltaListExt
    {
    }

    public interface IDeltaListExt<T>
    {
        void SubscribeCustomAddAction(object key, MethodInfo key2, DeltaListChangedDelegate<T> action);

        void UnsubscribeCustomAddAction(object key, MethodInfo key2);

        void SubscribeCustomRemoveAction(object key, MethodInfo key2, DeltaListChangedDelegate<T> action);

        void UnsubscribeCustomRemoveAction(object key, MethodInfo key2);
        
        List<ulong?> GetDeltaObjectsLocalIds();
    }

    [ProtoContract]
    public class DeltaValueWrapper<T>
    {
        [ProtoMember(1)] public T Value { get; set; }

        [UsedImplicitly]
        public DeltaValueWrapper()
        {
        }

        public DeltaValueWrapper(T value)
        {
            Value = value;
        }
    }

    [ProtoContract]
    public class DeltaReferenceWrapper<T>
    {
        [ProtoMember(1)] public T Value { get; set; }

        [UsedImplicitly]
        public DeltaReferenceWrapper()
        {
        }

        public DeltaReferenceWrapper(T value)
        {
            Value = value;
        }
    }
}