using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedCode.Logging;
using Assets.ColonyShared.SharedCode.Utils;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NLog;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.OurSimpleIoC;
using SharedCode.Refs.Operations;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;
using SharedCode.Utils;
using SharedCode.Repositories;
using ResourcesSystem.Base;
using ResourceSystem.Utils;
using System.Threading.Channels;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.AsyncStack;
using SharedCode.Utils.Threads;

namespace SharedCode.Refs
{
    [ProtoContract]
    public class EntityRef<T>: IEntityRef, IEntityRefExt, IHasRandomFill where T: IEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        [ProtoMember(1)]
        public Guid Id { get; set; }

        public string TypeName => Entity?.TypeName ?? typeof(T).GetFriendlyName();

        [BsonIgnore]
        [ProtoIgnore]
        public EntityState State => ((IEntityExt)Entity)?.State ?? EntityState.None;

        [BsonIgnore] 
        [ProtoIgnore] 
        [JsonIgnore]
        private Dictionary<(ReplicationLevel, long), Dictionary<ulong, byte[]>> _serializedData =
            new Dictionary<(ReplicationLevel, long), Dictionary<ulong, byte[]>>();

        private object _locker = new object();

        public object Locker => _locker;

        private bool _needCheckProcessDeltaSnapshot;

        IEntity IEntityRefExt.GetEntity()
        {
            return Entity;
        }

        public void CheckNotNull()
        {
            if (Id != Guid.Empty && Entity == null)
                Logger.IfError()?.Message("EntityRef typeId {0} id {1} link error", TypeId, Id).Write();
        }

        [ProtoIgnore]
        [BsonIgnore]
        [JsonProperty]
        private T Entity { get; set; }

        [ProtoIgnore]
        private int _version;

        [ProtoIgnore]
        public int Version
        {
            get { return _version; }
        }

        public int TypeId
        {
            get { return Entity?.TypeId ?? ReplicaTypeRegistry.GetIdByType(typeof(T)); }
        }

        public EntityMigrationState Migrating
        {
            get => Entity?.Migrating ?? EntityMigrationState.None;
            set => ((IEntityExt)Entity)?.SetMigrating(value);
        }

        public long CurrentReplicationMask
        {
            get { return ((IEntityExt) Entity).CurrentReplicationMask; }
        }

        ConcurrentQueue<BaseEntityRefOperation> _entityOperation = new ConcurrentQueue<BaseEntityRefOperation>();

        private bool _reorderingOperationQueue = false;

        [ProtoIgnore]
        [BsonIgnore]
        public IEnumerable<BaseEntityRefOperation> IncomingOperations => _entityOperation.ToList();

        public int GetOperationCount()
        {
            return _entityOperation.Count;
        }

        private bool _deltaSnapshotProcessing = false;

        private OuterRef<IEntity> _outerRef;
        public OuterRef<IEntity> OuterRef => _outerRef.IsValid ? _outerRef : _outerRef = new OuterRef<IEntity>(Id, TypeId);

        public Task Destroying { get; set; }

        public event Func<int, Guid, IEntity, Task> OnDestroyOrUnload;

        public Dictionary<IEntityRef, EntityRefChange> EntityRefChanges { get; set; } 

        public EntityRef()
        {
        }

        public EntityRef(T entity, long replicationMask, int version)
        {
            Id = entity.Id;
            Entity = entity;
            _version = version;
            ((IEntityExt)Entity).CurrentReplicationMask = replicationMask;
        }

        public int IncrementVersion()
        {
            return Interlocked.Increment(ref _version);
        }

        public void SetVersion(int version)
        {
            _version = version;
        }

        public void ChangeEntityRef(EntityRefChange change)
        {
            var entityRefExt = (IEntityRefExt) this;
            if (entityRefExt.EntityRefChanges == null)
            {
                entityRefExt.EntityRefChanges = new Dictionary<IEntityRef, EntityRefChange>();
            }

            // collapse changes that occurs in one using
            if (entityRefExt.EntityRefChanges.TryGetValue(change.Value, out var previousChange))
            {
                // no point in change that doesn't change anything
                if (previousChange.OldReachabilityReplicationLevel == change.NewReachabilityReplicationLevel)
                {
                    entityRefExt.EntityRefChanges.Remove(change.Value);
                }
                else
                {
                    change = new EntityRefChange(previousChange.OldReachabilityReplicationLevel, change.NewReachabilityReplicationLevel, change.Value);
                }
            }

            entityRefExt.EntityRefChanges[change.Value] = change;
        }

        public Dictionary<long, Dictionary<ulong, byte[]>> TakeDeltaSnapshot()
        {
            lock (_locker)
            {
                IncrementVersion();
                var delta = new Dictionary<long, Dictionary<ulong, byte[]>>();
                var serializer = ((IEntitiesRepositoryExtension) Entity.EntitiesRepository).EntitySerializer;
                
                var replicationMasks = ((IEntityExt) Entity).GetReplicationMasks();
                foreach (var replicationMask in replicationMasks)
                {
                    // при Downgrade маска проставляется в 0, а не удаляется из _replicateRepositoryIds
                    if (replicationMask != 0)
                    {
                        var serializedDeltaObjects = serializer.SerializeEntityChanged((IEntityExt) Entity,
                            (ReplicationLevel) replicationMask, replicationMask);
                        delta.Add(replicationMask, serializedDeltaObjects);
                    }
                }

                return delta;
            }
        }
        
        public void SerializeAndSaveNewSubscriber(ReplicationLevel newReplicationLevel, ReplicationLevel oldReplicationLevel, long changedReplicationMask)
        {
            if (changedReplicationMask == 0)
                return;

            lock (_locker)
            {
                if (!_serializedData.ContainsKey((newReplicationLevel, changedReplicationMask)))
                {
                    SerializeFullByMaskAndSave(newReplicationLevel,  oldReplicationLevel, changedReplicationMask);
                }
            }
        }

        public Dictionary<ulong, byte[]> GetSerialized(ReplicationLevel replicationLevel,  ReplicationLevel oldReplicationLevel, long changedReplicationMask, bool checkExisting = true)
        {
            lock (_locker)
            {
                if (!_serializedData.TryGetValue((replicationLevel, changedReplicationMask), out var data))
                {
                    if (checkExisting)
                    {
                        Logger.IfError()?.Message("!!! GetSerialized not found data for typeId {0} id {1} mask {2}", this.TypeName, Id, changedReplicationMask).Write();
                    }

                    data = SerializeFullByMaskAndSave(replicationLevel, oldReplicationLevel, changedReplicationMask);
                }
                
                return data;
            }
        }
        
        private Dictionary<ulong, byte[]> SerializeFullByMaskAndSave(ReplicationLevel replicationLevel, ReplicationLevel oldReplicationLevel, long serializeMask)
        {
            var serializer = ((IEntitiesRepositoryExtension) Entity.EntitiesRepository).EntitySerializer;
            var serializedDeltaObjects = serializer.SerializeEntityFull((IEntityExt) Entity,
                replicationLevel, oldReplicationLevel, serializeMask);
            _serializedData[(replicationLevel, serializeMask)] = serializedDeltaObjects;
            return serializedDeltaObjects;
        }

        void IEntityRefExt.AfterExclusiveUsing()
        {
            lock (_locker)
            {
                _serializedData.Clear();
            }
        }
        
        public Type GetEntityInterfaceType()
        {
            return typeof(T);
        }

        public void AssertNotRemoteEntity()
        {
            if (!((IEntityExt)Entity).IsMaster())
            {
                Logger.IfError()?.Message("Entity is remoted typeId {0} id {1} repository {2}", Entity.TypeId, Entity.Id, Entity.EntitiesRepository.Id).Write();
                throw new Exception(string.Format("Entity is remoted typeId {0} id {1} repository {2}", Entity.TypeId, Entity.Id, Entity.EntitiesRepository.Id));
            }
        }

        public void AddReplicationMask(long replicationMask)
        {
            ((IEntityExt)Entity).CurrentReplicationMask = ((IEntityExt)Entity).CurrentReplicationMask | replicationMask;
        }

        public void RemoveReplicationMask(long replicationMask)
        {
            ((IEntityExt)Entity).CurrentReplicationMask = (((IEntityExt)Entity).CurrentReplicationMask ^ replicationMask) & ((IEntityExt)Entity).CurrentReplicationMask;
        }

        public bool ContainsReplicationLevel(ReplicationLevel level)
        {
            var mask = (long)level;
            return (((IEntityExt)Entity).CurrentReplicationMask & mask) == mask;
        }

        private void reorderEntityOperationQueue()
        {
            lock (_entityOperation)
            {
                _reorderingOperationQueue = true;

                var operationsCopy = new List<BaseEntityRefOperation>();

                BaseEntityRefOperation operation;
                while (_entityOperation.TryDequeue(out operation))
                    operationsCopy.Add(operation);

                operationsCopy.Sort(sortEntityOperations);

                foreach (var sortedOperation in operationsCopy)
                    _entityOperation.Enqueue(sortedOperation);

                _reorderingOperationQueue = false;
            }
        }

        private static int sortEntityOperations(BaseEntityRefOperation op1, BaseEntityRefOperation op2)
        {
            if (!(op1 is IOperationWithVersion))
                return 1;
            if (!(op2 is IOperationWithVersion))
                return -1;
            return ((IOperationWithVersion)op1).GetVersion() - ((IOperationWithVersion)op2).GetVersion();
        }

        public EntityRefOperationResult? ProcessDeltaSnapshots(IEntitiesRepository repository, bool secondaryUpdate = false)
        {
            bool needUpdateOneMoreTime = false;
            var queueOperationHasReordered = false;
            if (Entity == null)
            {
                Logger.IfError()?.Message("ProcessDeltaSnapshots entity is null {0}", Id).Write();
                return null;
            }

            if (_deltaSnapshotProcessing)
            {
                _needCheckProcessDeltaSnapshot = true;
                Logger.IfError()?.Message("ProcessDeltaSnapshots already processing {0}", Id).Write();
                return null;
            }

            EntityRefOperationResult? entityRefOperationResultCombined = null; 
            if (!_entityOperation.IsEmpty)
            {
                _deltaSnapshotProcessing = true;
                try
                {
                    while (_entityOperation.TryDequeue(out var operation))
                    {
                        var operationProcessed = operation.Do(this, out var entityRefOperationResult);
                        entityRefOperationResultCombined = entityRefOperationResultCombined.Combine(entityRefOperationResult);
                        if (!operationProcessed)
                        {
                            if (operation is IOperationWithVersion)
                            {
                                _entityOperation.Enqueue(operation);
                                reorderEntityOperationQueue();
                                if (!secondaryUpdate)
                                {
                                    needUpdateOneMoreTime = true;
                                    queueOperationHasReordered = true;
                                }
                            }
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "ProcessDeltaSnapshots typeId {0} Id {1} exception", Entity?.TypeName, Id).Write();
                }
                finally
                {
                    _deltaSnapshotProcessing = false;
                    needUpdateOneMoreTime = needUpdateOneMoreTime || (_needCheckProcessDeltaSnapshot && _entityOperation.Count > 0);
                    _needCheckProcessDeltaSnapshot = false;
                }

                if (needUpdateOneMoreTime)
                {
                   var entityRefOperationResult=  ProcessDeltaSnapshots(repository, queueOperationHasReordered);
                   entityRefOperationResultCombined = entityRefOperationResult.Combine(entityRefOperationResult);
                }
            }

            return entityRefOperationResultCombined;
        }

        public Task TryAddWaitOperation()
        {
            if (_reorderingOperationQueue)
                lock (_entityOperation)
                {
                    if (_entityOperation == null || _entityOperation.Count == 0)
                        return null;
                    var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _entityOperation.Enqueue(new CompleteTaskAfterProcessingOperation(tcs));
                    return tcs.Task;
                }
            else
            {
                if (_entityOperation == null || _entityOperation.Count == 0)
                    return null;
                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                _entityOperation.Enqueue(new CompleteTaskAfterProcessingOperation(tcs));
                return tcs.Task;
            }
        }

        public bool NeedWaitProcessingDeltaSnapshot()
        {
            lock (_entityOperation)
                return _entityOperation != null || _entityOperation.Count > 0;
        }


        public void AddDeltaSnapshot(Dictionary<ulong, byte[]> snapshot, List<DeferredEntityModel> deferredEntities, int bytesCount, long replicationMask, int version, int previousVersion)
        {
            if (_reorderingOperationQueue)
                lock (_entityOperation)
                    _entityOperation.Enqueue(new ProcessDeltaSnapshotOperation(snapshot, deferredEntities, bytesCount, replicationMask, version, previousVersion));
            else
                _entityOperation.Enqueue(new ProcessDeltaSnapshotOperation(snapshot, deferredEntities, bytesCount, replicationMask, version, previousVersion));
        }

        public void AddDowngrade(long downgradeMask, int version, int previousVersion)
        {
            if (_reorderingOperationQueue)
                lock (_entityOperation)
                    _entityOperation.Enqueue(new DowngradeOperation(downgradeMask, version, previousVersion));
            else
                _entityOperation.Enqueue(new DowngradeOperation(downgradeMask, version, previousVersion));
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            //Id = Guid.NewGuid();
        }

        public async Task CallOnDestroyOrUnload(int typeId, Guid guid, IEntity entity, IEntitiesRepository repo)
        {
            BaseEntity be = Entity as BaseEntity;

            var tasks = new List<SuspendingAwaitable>();

            var mask = CurrentReplicationMask;
            tasks.Add(AsyncUtils.RunAsyncTask(() =>
                be.FireOnReplicationLevelChanged(mask, 0)
                //не должно тут вызываться, это колбек для мастер копии. 
                //Да, я знаю, что у нас идиотское совпадение между названием процесса выгрузки мастер-копии и выгрузки реплики с репозитория, 
                //хз че с этим сейчас сделать
                //return be.FireOnUnload();
            ));

            if (OnDestroyOrUnload != null)
            {
                foreach (var subscriber in OnDestroyOrUnload.GetInvocationList().Cast<Func<int, Guid, IEntity, Task>>())
                {
                    var subscriberCopy = subscriber;
                    tasks.Add(AsyncUtils.RunAsyncTask(() =>
                        AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(typeId, guid, entity),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                                () => $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    ));
                }
            }
            await SuspendingAwaitable.WhenAll(tasks);
        }

        private Channel<List<Func<Task>>> _channel;

        void IEntityRefExt.AddEvents(List<Func<Task>> eventsList)
        {
            if(_channel == null)
            {
                var channel = Channel.CreateUnbounded<List<Func<Task>>>(new UnboundedChannelOptions() { SingleReader = true, SingleWriter = true });

                if(Interlocked.CompareExchange(ref _channel, channel, null ) == null)
                    TaskEx.Run(ProcessEntityEvents);
            }

            if (!_channel.Writer.TryWrite(eventsList))
                _channel.Writer.WriteAsync(eventsList).AsTask().Wait();
        }

        private async Task ProcessEntityEvents()
        {
            while (await _channel.Reader.WaitToReadAsync())
            {
                try
                {
                    using (AsyncStackIsolator.IsolateContext())
                    {
                        using (var wrapper = await this.Entity.EntitiesRepository.Get(TypeId, Id))
                        {
                            while (_channel.Reader.TryRead(out var eventsActions))
                            {
                                if (eventsActions.Count > ServerCoreRuntimeParameters.MaxProcessEventsCount)
                                {
                                    var sb = new StringBuilder();
                                    sb.AppendLine();
                                    foreach (var func in eventsActions)
                                        sb.AppendLine($"<obj {func.Target?.GetType().Name ?? "unknown"} method {func.Method.Name}>,");
                                    Logger.IfError()?.Message("To many events actions {0} on entity {1} id {2}. subscribers {3}: ", eventsActions.Count, TypeName, Id, sb.ToString()).Write();
                                }

                                bool needReIsolate = false;
                                foreach (var func in eventsActions)
                                {
                                    var funcCopy = func;
                                    try
                                    {
                                        await AsyncUtils.RunAsyncWithCheckTimeout(funcCopy,
                                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                                            () => $"obj {funcCopy.Target?.GetType().Name ?? "unknown"} method {funcCopy.Method.Name}");
                                        AsyncStackHolder.AssertNoChildren();
                                    }
                                    catch (AsyncContextException e)
                                    {
                                        Logger.IfError()?.Message(e, "Exception while calling subscriber {0}", funcCopy).Write();
                                        needReIsolate = true;
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.IfError()?.Message(e, "Exception while calling subscriber {0}", funcCopy).Write();
                                    }
                                }

                                if (needReIsolate)
                                    break;
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        public bool IsEntityAvailableInEntityGraphOnCurrentRepository(IEntitiesRepository repository)
        {
            return ((IEntitiesRepositoryExtension) repository).IsEntityTypeAvailableInEntityGraph(typeof(T));
        }
    }
}
