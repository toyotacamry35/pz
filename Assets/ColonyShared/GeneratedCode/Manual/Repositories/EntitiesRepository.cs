using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DatabaseUtils;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratedCode.EntitySystem.Statistics;
using GeneratedCode.Manual.AsyncStack;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Network.Statistic;
using NLog;
using ResourceSystem.Utils;
using SharedCode.Cloud;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Extensions;
using SharedCode.Network;
using SharedCode.Refs;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;
using SharedCode.Utils;
using SharedCode.Utils.Threads;

namespace GeneratedCode.Repositories
{
    public partial class EntitiesRepository
    {
        private readonly ConcurrentDictionary<object, bool> BlockTimeoutRequests = new ConcurrentDictionary<object, bool>();

        private const float ToLongWaitEntityErrorLoggingSeconds = 5.0f;

        private readonly object _batchLocker = new object();

        private bool _lockTaken = false;

        private readonly LockRepositoryStatistics LockRepositoryStatistics;
        private readonly EntityCountStatistics EntityCountStatistics;
        private readonly EntityQueueSizeStatistics EntityQueueSizeStatistics;

        private static ThreadLocal<HashSet<EntityQueue>> _nextQueuesToCheck = new ThreadLocal<HashSet<EntityQueue>>(() => new HashSet<EntityQueue>());
        
        private static ThreadLocal<Dictionary<Guid, List<DeferredEntityModel>>> _newLinkedEntitiesBuffer =
            new ThreadLocal<Dictionary<Guid, List<DeferredEntityModel>>>(() => new Dictionary<Guid, List<DeferredEntityModel>>());
        
        private static ThreadLocal<List<(long level, IEntityRef entityRef)>> _linkedEntitiesSingleThreadBuffer =
            new ThreadLocal<List<(long level, IEntityRef)>>(() => new List<(long level, IEntityRef)>());

        private static ThreadLocal<List<(
                BatchItem entity,
                Dictionary<long, Dictionary<ulong, byte[]>> snapshot,
                Dictionary<Guid, List<DeferredEntityModel>>
                defferedEntities)>>
            _snapshotsBuffer = new ThreadLocal<List<(
                BatchItem entity,
                Dictionary<long, Dictionary<ulong, byte[]>> snapshot,
                Dictionary<Guid, List<DeferredEntityModel>>defferedEntities)>>(() =>
                new List<(BatchItem entity,
                    Dictionary<long, Dictionary<ulong, byte[]>> snapshot,
                    Dictionary<Guid, List<DeferredEntityModel>> defferedEntities)>());
            

        private Pool<List<(long level, IEntityRef entityRef)>> _linkedEntitiesPool = new Pool<List<(long level, IEntityRef entityRef)>>(
            1024,
            128,
            () => new List<(long level, IEntityRef entityRef)>(),
            el => el.Clear());
        

        private readonly Channel<SubscriptionsChange> _subscriptionsChanges = Channel.CreateUnbounded<SubscriptionsChange>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });

        public bool IsTimeoutBlocked()
        {
            return BlockTimeoutRequests.Count > 0;
        }
        
        public UnlockTimeoutBlocker DisableRepositoryEntityUnlockTimeout()
        {
            return new UnlockTimeoutBlocker(BlockTimeoutRequests, Id);
        }

        public string GetEntityStatus(int typeId, Guid entityId)
        {
            var sb = StringBuildersPool.Get;
            GetEntityStatusInternal(typeId, entityId, sb);
            return sb.ToStringAndReturn();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stopwatch prepareWaitLockStatistics()
        {
            var sw = StopWatchPool.Pool.Take();
            sw.Start();
            return sw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void finishWaitAndPrepareUseLockStatistics(ref Stopwatch sw, LockRepositoryStatistics.LockRepositoryOperation operation)
        {
            sw.Stop();
            LockRepositoryStatistics.AddWait(sw.Elapsed, operation);
            if (!_lockTaken)
            {
                sw.Restart();
                _lockTaken = true;
            }
            else
            {
                StopWatchPool.Pool.Return(sw);
                sw = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void finishUseLockStatistics(Stopwatch sw, LockRepositoryStatistics.LockRepositoryOperation operation)
        {
            if (sw != null)
            {
                sw.Stop();
                LockRepositoryStatistics.AddUse(sw.Elapsed, operation);
                _lockTaken = false;
                StopWatchPool.Pool.Return(sw);
            }
        }


        private void GetEntityStatusInternal(int typeId, Guid entityId, StringBuilder sb)
        {
            var sw = prepareWaitLockStatistics();
            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.GetEntityStatusInternal);

                try
                {
                    sb.Append($"type {typeId} typeName {GetTypeById(typeId)?.Name ?? "unknown"} entity {entityId}").AppendLine();

                    var typeState = GetTypeState(typeId);

                    EntityUsageRefsCount refsCount;
                    if (typeState.LockedEntities.TryGetValue(entityId, out refsCount))
                        refsCount.GetOperationLog(sb, refsCount.CloneUsedContexts());

                    EntityQueue queue;
                    if (typeState.WaitQueues.TryGetValue(entityId, out queue))
                    {
                        var copyQueueBatches = queue.GetBatchesToDump();
                        sb.AppendLine();
                        for (int i = 0; i < copyQueueBatches.Length; i++)
                        {
                            ref var pair = ref copyQueueBatches[i];
                            if (EqualityComparer<EntityQueue.EntityQueueElement>.Default.Equals(pair, default))
                                continue;
                            sb.AppendFormat("<{0}{1}:", pair.OperationType.ToString(), pair.ReadyToUse ? "*" : string.Empty);
                            pair.Batch.DumpToStringBuilder(sb);
                            sb.Append(">");
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine("-----------------------------------");

                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.GetEntityStatusInternal);
                }
            }
        }

        public bool SubscribeOnDestroyOrUnload(int typeId, Guid entityId, Func<int, Guid, IEntity, Task> callback)
        {
            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.SubscribeOnDestroyOrUnload);

                try
                {
                    var entRef = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId) as IEntityRefExt;
                    if (entRef == null)
                    {
                        Logger.IfError()?.Message("SubscribeOnDestroyOrUnload entity not found typeId {0} Id {1}", GetTypeById(typeId).Name, entityId).Write();
                        return false;
                    }

                    entRef.OnDestroyOrUnload += callback;
                    return true;
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.SubscribeOnDestroyOrUnload);
                }
            }
        }

        public bool SubscribeOnDestroyOrUnload(IEnumerable<EntityDesc> entities, bool allowPartial, Func<int, Guid, IEntity, Task> callback)
        {
            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.SubscribeOnDestroyOrUnloadBatch);

                try
                {
                    var refs = entities.Select(ent => (IEntityRefExt)((IEntitiesRepositoryExtension)this).GetRef(ent.TypeId, ent.Guid));

                    if (refs.Any(v => v == null) && !allowPartial)
                        return false;

                    foreach (var entRef in refs)
                    {
                        if (entRef == null)
                            continue;

                        entRef.OnDestroyOrUnload += callback;
                    }
                    return true;
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.SubscribeOnDestroyOrUnloadBatch);
                }
            }
        }

        public bool UnsubscribeOnDestroyOrUnload(int typeId, Guid entityId, Func<int, Guid, IEntity, Task> callback)
        {
            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.UnsubscribeOnDestroyOrUnload);

                try
                {
                    var entRef = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId) as IEntityRefExt;
                    if (entRef == null)
                        return false;

                    entRef.OnDestroyOrUnload -= callback;
                    return true;
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.UnsubscribeOnDestroyOrUnload);
                }
            }
        }

        public async Task<EntityRef<T>> Load<T>(Guid entityId, Func<T, Task> initializeAction = null) where T : IEntity
        {
            var typeId = GetIdByType(typeof(T));
            var result = await Load(typeId, entityId, initializeAction == null ? (Func<IEntity, Task>)null : (async (IEntity e) => await initializeAction((T)e)));
            return (EntityRef<T>)result;
        }

        public async Task<IEntityRef> Load(int typeId, Guid entityId, Func<IEntity, Task> initializeAction = null)
        {
            var result = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
            if (result != null)
                return result;

            var dBServeiceEntityid = await GetDataBaseServiceEntityid(typeId, entityId);
            SerializedEntityBatch uploadBatchContainer = null;
            using (var wrapper = await Get<IDatabaseServiceEntityServer>(dBServeiceEntityid))
            {
                var dbServiceEntity = wrapper.Get<IDatabaseServiceEntityServer>(dBServeiceEntityid);
                uploadBatchContainer = await dbServiceEntity.Load(typeId, entityId);
            }

            if (uploadBatchContainer == null)
            {
                Logger.IfError()?.Message("Not loaded").Write();
                return null;
            }

            var uploadedRefs = new List<IEntityRef>();

            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.Load);

                try
                {
                    foreach (var containerBatch in uploadBatchContainer.Batches)
                    {
                        if (ServerCoreRuntimeParameters.CollectEntityLifecycleHistory)
                        {
                            Logger.IfInfo()?.Message("Entity is being loaded from db {type} {entityId}", ReplicaTypeRegistry.GetTypeById(containerBatch.EntityTypeId), containerBatch.EntityId).Write();
                        }
                        
                        using (_EntityContext.Pool.Set())
                        {
                            _EntityContext.Pool.Current.FullCreating = true;
                            
                            var type = ReplicaTypeRegistry.GetTypeById(containerBatch.EntityTypeId);
                            var entity = ReplicaTypeRegistry.Deserialize(type, _entitySerializer, this,
                                containerBatch.EntityTypeId, containerBatch.EntityId, containerBatch.Snapshot);
                            ((IEntityExt) entity).SetOwnerNodeId(this.Id);
                            ((IEntityExt) entity).SetState(EntityState.InitializingAfterLoading);
                            var entityRef = SetInternal(containerBatch.EntityTypeId, entity,
                                (long)ReplicationLevel.Master, 1);
                            //use this for migration
                            //containerBatch.Version 
                            uploadedRefs.Add(entityRef);
                        }
                    }

                    foreach (var uploadedRef in uploadedRefs)
                    {
                        using (_EntityContext.Pool.Set())
                        {
                            _EntityContext.Pool.Current.FullCreating = true;

                            var entity = ((IEntityRefExt) uploadedRef).GetEntity();
                            ((IDeltaObjectExt) entity).Visit((v) => ((IDeltaObjectExt) v).LinkEntityRefs(this));
                        }
                    }
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.Load);
                }
            }

            foreach (var uploadedRef in uploadedRefs)
            {
                var entity = ((IEntityRefExt)uploadedRef).GetEntity();
                if (entity.TypeId == typeId && entity.Id == entityId && initializeAction != null)
                    await initializeAction(entity);
            }

            foreach (var uploadedRef in uploadedRefs)
            {
                var entity = ((IEntityRefExt)uploadedRef).GetEntity();
                await ((BaseEntity)entity).FireOnDatabaseLoad();
            }

            sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.LoadSerialize);

                try
                {
                    foreach (var uploadedRef in uploadedRefs)
                    {
                        var entity = ((IEntityRefExt)uploadedRef).GetEntity();
                        ((IEntityExt)entity).SetState(EntityState.Ready);
                        Statistics<EntityLoadStatistics>.Instance.Load(entity.GetType());
                    }
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.LoadSerialize);
                }
            }

            foreach (var uploadedRef in uploadedRefs)
            {
                var uploadedTypeId = uploadedRef.TypeId;
                var uploadedId = uploadedRef.Id;
                var uploadedRefCopy = uploadedRef;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await Get(uploadedTypeId, uploadedId))
                    {
                        var ent = wrapper.Get<IEntityExt>(uploadedTypeId, uploadedId);
                        await ent.FireOnStart();
                        ent.FireOnReplicationLevelChanged(0, (long)ReplicationLevel.Master);
                        OnEntityLoaded(uploadedRefCopy);
                    }
                }, this);

                var entity = ((IEntityRefExt)uploadedRef).GetEntity();
                AfterAddedToRepository(typeId, entityId, entity.GetType(), DatabaseSaveTypeChecker.GetDatabaseSaveType(entity.TypeId));
            }

            result = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
            return result;
        }

        public Task<EntityRef<T>> Create<T>(Guid entityId, Func<T, Task> initializeAction = null) where T : IEntity
        {
            return CreateInternal<T>(entityId, initializeAction);
        }

        async Task<EntityRef<T>> CreateInternal<T>(Guid entityId, Func<T, Task> initializeAction = null) where T : IEntity
        {
            var typeId = GetIdByType(typeof(T));

            return (EntityRef<T>)await CreateInternal(typeId, entityId, initializeAction != null ? async (IEntity entity) =>
               {
                   await initializeAction((T)entity);
               }
            : (Func<IEntity, Task>)null);
        }

        public Task<IEntityRef> Create(int typeId, Guid entityId, Func<IEntity, Task> initializeAction = null)
        {
            return CreateInternal(typeId, entityId, initializeAction);
        }

        public async Task<Guid> GetDataBaseServiceEntityid(int typeId, Guid _)
        {
            var serviceType = DatabaseSaveTypeChecker.GetDatabaseServiceType(typeId);
            int tryCount = 0;
            do
            {
                var collection = GetEntitiesCollection(typeof(IDatabaseServiceEntity));
                var id = collection?.FirstOrDefault(x => ((IDatabaseServiceEntity)((IEntityRefExt)x.Value).GetEntity()).DatabaseServiceType == serviceType).Key ?? Guid.Empty;
                if (id != Guid.Empty)
                    return id;

                tryCount++;
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            while (tryCount < 3);

            Logger.IfError()?.Message("DatabaseService {0} not found in cluster", serviceType).Write();
            return Guid.Empty;
        }

        async Task<IEntityRef> CreateInternal(int typeId, Guid entityId, Func<IEntity, Task> initializeAction = null)
        {
            if (ServerCoreRuntimeParameters.CollectEntityLifecycleHistory)
            {
                Logger.IfInfo()?.Message("Entity is beign created {type} {entityId}", ReplicaTypeRegistry.GetTypeById(typeId), entityId).Write();
            }
            
            IEntity newEntity;
            IEntityRef newEntityRef;
            using (_EntityContext.Pool.Set())
            {
                _EntityContext.Pool.Current.FullCreating = true;
                
                newEntity = CreateImpl(typeId, entityId);
                newEntityRef = SetInternal(typeId, newEntity, (long) ReplicationLevel.Master, 0);
                ((BaseEntity) newEntity).OwnerNodeId = this.Id;
                ((BaseEntity) newEntity).ClearDelta();
                ((IEntityExt) newEntity).SetState(EntityState.Initializing);
                ((IDeltaObjectExt) newEntity).Visit((v) => ((IDeltaObjectExt) v).LinkEntityRefs(this));
            }

            using (var wrapper = await Get(typeId, entityId))
            {
                // use lock because init can call rpc and other things
                if (initializeAction != null)
                {
                    await initializeAction(newEntity);
                }

                await ((BaseEntity) newEntity).FireOnInit();
                ((IEntityExt) newEntity).SetState(EntityState.Ready);
                Statistics<EntityCreatedStatistics>.Instance.Created(newEntity.GetType());
            }

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await Get(typeId, entityId))
                {
                    var ent = wrapper.Get<IEntityExt>(typeId, entityId);
                    await ent.FireOnStart();
                    ent.FireOnReplicationLevelChanged(0, (long)ReplicationLevel.Master);
                }
            }, this);

            afterCreateEntity(newEntityRef);
            AfterAddedToRepository(typeId, entityId, newEntity.GetType(), DatabaseSaveTypeChecker.GetDatabaseSaveType(typeId));

            return newEntityRef;
        }

        private void AfterAddedToRepository(int typeId, Guid entityId, Type type, DatabaseSaveType databaseSaveType)
        {
            var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
            if (_cloudNodeType == CloudNodeType.Server && !EntitiesRepository.IsServiceEntityType(masterTypeId))
            {
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await this.GetFirstService<IClusterAddressResolverServiceEntityServer>())
                    {
                        if (wrapper == null)
                        {
                            //#note: (2019.05.30): Not a problem - doesn't affect anything (by @Vitaly):
                            Logger.IfError()?.Message("IClusterAddressResolverServiceEntityServer not found by typeId {0} entityId {1}", EntitiesRepository.GetTypeById(masterTypeId)?.Name ?? masterTypeId.ToString(), entityId).Write();
                            return;
                        }
                        var addressResolver = wrapper.GetFirstService<IClusterAddressResolverServiceEntityServer>();
                        await addressResolver.SetEntityAddressRepositoryId(masterTypeId, entityId, Id);

                        if (databaseSaveType == DatabaseSaveType.Explicit)
                        {
                            var dBServiceEntityId = await GetDataBaseServiceEntityid(masterTypeId, entityId);
                            await ChangeSubscription(masterTypeId, entityId, dBServiceEntityId, new List<(bool subscribe, ReplicationLevel replicationLevel)>
                            {
                                (true, ReplicationLevel.Master)
                            }, true);
                        }
                    }
                });
            }
        }

        private void checkBatchGetExclusiveOnReplication(IEntityBatch batch)
        {
            var entityBatch = (EntityBatch)batch;
            for (int i = 0; i < entityBatch.Length; i++)
            {
                ref var item = ref entityBatch.Items[i];
                if (item.RequestOperationType == ReadWriteEntityOperationType.Write)
                {
                    var currentContainsStatus = ((IEntitiesRepositoryExtension)this).GetRepositoryEntityContainsStatus(item.EntityMasterTypeId, item.EntityId);
                    if (currentContainsStatus == RepositoryEntityContainsStatus.Replication)
                        throw new AsyncContextException($"Try lock write on entity {item}, status {currentContainsStatus}");
                }
            }
        }

        private struct ContainedInAsyncContextPred : ArrayExts.MutatingPredicate<BatchItem>
        {
            private readonly AsyncEntitiesRepositoryRequestContext ctx;

            public ContainedInAsyncContextPred(AsyncEntitiesRepositoryRequestContext ctx)
            {
                this.ctx = ctx;
            }

            public bool NeedRemove(ref BatchItem item)
            {
                ReadWriteEntityOperationType currentOperationType = ctx.CheckIsEntityAlreadyLocked(item.EntityMasterTypeId, item.EntityId);
                if (currentOperationType == ReadWriteEntityOperationType.None)
                    return false;

                if (currentOperationType >= item.RequestOperationType)
                    return true;
                item.UpFromReadToExclusive = true;

                return false;
            }
        }

        private static void removeFromEntitiesBatchAlreadyLockedEntities(IEntityBatch batch, AsyncEntitiesRepositoryRequestContext asyncEntitiesRepositoryRequestContext)
        {
            if (asyncEntitiesRepositoryRequestContext == null || !asyncEntitiesRepositoryRequestContext.IsLockedAny())
                return;

            var entityBatch = (EntityBatch)batch;
            ContainedInAsyncContextPred pred = new ContainedInAsyncContextPred(asyncEntitiesRepositoryRequestContext);
            entityBatch.Length = entityBatch.Items.RemoveAll(0, entityBatch.Length, ref pred);
        }

        private List<Task> WaitProcessDeltaSnapshots(EntitiesContainer container)
        {
            List<Task> result = null;
            foreach (var entitiesContainer in AsyncStackEnumerable.ToParents(container))
            {
                if (entitiesContainer.Batch == null)
                    continue;
                for (int i = 0; i < entitiesContainer.Batch.Length; i++)
                {
                    ref var batchItem = ref entitiesContainer.Batch.Items[i];
                    var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(batchItem.EntityMasterTypeId, batchItem.EntityId);
                    if (entityRef != null)
                    {
                        var task = entityRef.TryAddWaitOperation();
                        if (task != null)
                        {
                            if (result == null)
                                result = new List<Task>();
                            result.Add(task);
                        }
                    }
                }
            }

            return result;
        }

        public bool NeedWaitProcessDeltaSnapshots(EntitiesContainer wrapper)
        {
            foreach (var entitiesContainer in AsyncStackEnumerable.ToParents(wrapper))
            {
                if (entitiesContainer.Batch == null)
                    continue;
                for (int i = 0; i < entitiesContainer.Batch.Length; i++)
                {
                    ref var batchItem = ref entitiesContainer.Batch.Items[i];
                    var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(batchItem.EntityMasterTypeId, batchItem.EntityId);
                    if (entityRef != null)
                    {
                        if (entityRef.NeedWaitProcessingDeltaSnapshot())
                            return true;
                    }
                }
            }

            return false;
        }

        public void VisitEntityQueueLengths(QueueLengthVisitor visitor)
        {
            foreach (var entityTypeState in _typeStates)
            {
                foreach (var waitQueue in entityTypeState.Value.WaitQueues)
                {
                    var count = waitQueue.Value.Count;
                    visitor(entityTypeState.Key, waitQueue.Key, count);
                }
            }
        }

        public void VisitEntityCounts(CountVisitor visitor)
        {
            foreach (var typeCollection in _typeToCollection)
            {
                visitor(typeCollection.Key, typeCollection.Value.Count);
            }
        }

        public string GetAllServiceEntitiesOperationLog()
        {
            var sb = StringBuildersPool.Get;
            List<IEntityRef> serviceEntitiesCopy = null;
                serviceEntitiesCopy = _serviceEntities.ToList();
            GetEntityStatusInternal(EntitiesRepository.GetIdByType(typeof(IRepositoryCommunicationEntity)), this.Id, sb);
            foreach (var serviceEntity in serviceEntitiesCopy)
            {
                GetEntityStatusInternal(serviceEntity.TypeId, serviceEntity.Id, sb);
            }
            return sb.ToStringAndReturn();
        }

        private readonly struct UnlockTimeoutPayload : ITimeoutPayload
        {
            private readonly EntitiesRepository _repo;
            private readonly EntitiesBatchWaitWrapper _wrapper;

            public UnlockTimeoutPayload(EntitiesRepository repo, EntitiesBatchWaitWrapper wrapper)
            {
                _repo = repo;
                _wrapper = wrapper;
            }

            public bool Run()
            {
                if (ServerCoreRuntimeParameters.CanDisableRepositoryGetEntitiesTimeout && _repo.IsTimeoutBlocked())
                    return false;

                var sw = _repo.prepareWaitLockStatistics();

                lock (_repo._batchLocker)
                {
                    _repo.finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.InstallGetRequestTimeout);

                    try
                    {
                        _wrapper.SetTimeout();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    finally
                    {
                        _repo.finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.InstallGetRequestTimeout);
                    }
                }
                return true;
            }
        }

        private ValueTask<IEntitiesContainer> LockEntities(EntitiesContainer entitiesContainer, object tag, StackTrace stackTrace)
        {
            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.LockEntities);

                try
                {
                    var parentEntitiesContainer = entitiesContainer.Context.Tail;

                    // detecting whether any of the containers has entity that is in use or delayed 
                    var addToQueue = IsAnyEntityInUseOrDeferred(parentEntitiesContainer);
                    ValueTask taskToWait = new ValueTask();
                    if (addToQueue)
                    {
                        var batch = addToWaitQueue(parentEntitiesContainer, entitiesContainer);
                        taskToWait = WaitForBatchWrapper(entitiesContainer, batch, tag, stackTrace);
                    }
                    else
                    {
                        lockBatch(parentEntitiesContainer);
                    }
                    return WaitAndStartTimeout(entitiesContainer, taskToWait);
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.LockEntities);
                }
            }
        }

        public async ValueTask LockAgain(EntitiesContainer entitiesContainer)
        {
            var waitProcessDeltaSnapshots = WaitProcessDeltaSnapshots(entitiesContainer);
            if (waitProcessDeltaSnapshots != null && waitProcessDeltaSnapshots.Count > 0)
            {
                await Task.WhenAll(waitProcessDeltaSnapshots);
            }

            var stackTrace = StackTraceUtils.GetStackTrace();
            await LockEntities(entitiesContainer, null, stackTrace);
        }


        private bool IsAnyEntityInUseOrDeferred(EntitiesContainer tailContainer)
        {
            foreach (var entitiesContainer in AsyncStackEnumerable.ToChildren(tailContainer))
            {
                if (entitiesContainer.Batch == null)
                    continue;

                for (int i = 0; i < entitiesContainer.Batch.Length; i++)
                {
                    ref var batchItem = ref entitiesContainer.Batch.Items[i];
                    var typeState = GetTypeState(batchItem.EntityMasterTypeId);

                    var currentUsage = getCurrentUsage(batchItem.EntityId, typeState);
                    if (currentUsage != null)
                    {
                        if (!currentUsage.validateRefs())
                            throw new Exception($"Invalid refs exception {batchItem.EntityMasterTypeId} {batchItem.EntityId}");

                        if (currentUsage.IsWrite() || currentUsage.IsRead() && batchItem.RequestOperationType == ReadWriteEntityOperationType.Write)
                            return true;
                    }

                    EntityQueue waitQueue;
                    if (typeState.WaitQueues.TryGetValue(batchItem.EntityId, out waitQueue))
                    {
                        if (batchItem.RequestOperationType == ReadWriteEntityOperationType.Write)
                        {
                            if (!waitQueue.IsFree())
                                return true;
                        }
                        else if (batchItem.RequestOperationType == ReadWriteEntityOperationType.Read)
                        {
                            if (waitQueue.HasWrite())
                                return true;
                        }
                    }

                    var requestedLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(ReplicaTypeRegistry.GetTypeById(batchItem.EntityRequestedTypeId));
                    if (typeState.IsEntityDeferred(batchItem.EntityId, requestedLevel))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public ValueTask<IEntitiesContainer> Get<T>(Guid entityId, [CallerMemberName] string callerTag = null) where T : IEntity
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddTag<T>(entityId, callerTag);
            return Get(entityBatch);
        }

        public ValueTask<IEntitiesContainer> Get<T>(Guid entityId, object tag, [CallerMemberName] string callerTag = null) where T : IEntity
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddTag<T>(entityId, callerTag);
            return Get(entityBatch, tag);
        }

        public ValueTask<IEntitiesContainer> Get(int typeId, Guid entityId, [CallerMemberName] string callerTag = null)
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddTag(typeId, entityId, callerTag);
            return Get(entityBatch);
        }

        public ValueTask<IEntitiesContainer> Get<T>(OuterRef<T> entOuterRef, [CallerMemberName] string callerTag = null)
        {
            return Get(entOuterRef.TypeId, entOuterRef.Guid, callerTag);
        }

        public ValueTask<IEntitiesContainer> Get(int typeId, Guid entityId, object tag, [CallerMemberName] string callerTag = null)
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddTag(typeId, entityId, callerTag);
            return Get(entityBatch, tag);
        }

        public ValueTask<IEntitiesContainer> Get(Type type, Guid entityId, [CallerMemberName] string callerTag = null)
        {
            var masterType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            var typeId = ReplicaTypeRegistry.GetIdByType(masterType);

            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddTag(typeId, entityId, callerTag);
            return Get(entityBatch);
        }

        public ValueTask<IEntitiesContainer> Get(Type type, Guid entityId, object tag, [CallerMemberName] string callerTag = null)
        {
            var masterType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            var typeId = ReplicaTypeRegistry.GetIdByType(masterType);

            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddTag(typeId, entityId, callerTag);
            return Get(entityBatch, tag);
        }

        public ValueTask<IEntitiesContainer> GetExclusive<T>(Guid entityId, string callerTag) where T : IEntity
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusiveTag<T>(entityId, callerTag);
            return Get(entityBatch);
        }

        public ValueTask<IEntitiesContainer> GetExclusive(int typeId, Guid entityId, string callerTag)
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusiveTag(typeId, entityId, callerTag);
            return Get(entityBatch);
        }

        public ValueTask<IEntitiesContainer> GetExclusive<T>(Guid entityId, object tag, string callerTag) where T : IEntity
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusiveTag<T>(entityId, callerTag);
            return Get(entityBatch, tag);
        }

        public ValueTask<IEntitiesContainer> GetExclusive(int typeId, Guid entityId, object tag, string callerTag)
        {
            var entityBatch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusiveTag(typeId, entityId, callerTag);
            return Get(entityBatch, tag);
        }

        public ValueTask<IEntitiesContainer> Get(IEntityBatch requestedBatch)
        {
            return get(requestedBatch);
        }

        public ValueTask<IEntitiesContainer> Get(IEntityBatch requestedBatch, object tag)
        {
            return get(requestedBatch, tag);
        }

        protected ValueTask<IEntitiesContainer> get(IEntityBatch requestedBatch, bool checkExclusiveOnReplication = true)
        {
            return get(requestedBatch, null, checkExclusiveOnReplication);
        }

        protected ValueTask<IEntitiesContainer> get(IEntityBatch requestedBatch, object tag, bool checkExclusiveOnReplication = true)
        {
            return GetInternal(requestedBatch, tag, checkExclusiveOnReplication);
        }

        public ValueTask<IEntitiesContainer> GetInternal(IEntityBatch requestedBatch, object tag, bool checkExclusiveOnReplication)
        {
            try
            {
                var container = AsyncEntitiesRepositoryRequestContext.Head;
                var context = container?.Context;
                context?.CheckRepositoryId(this);
                context?.CheckValid();

                AsyncStackHolder.ThrowIfInUnityContext();

                removeFromEntitiesBatchAlreadyLockedEntities(requestedBatch, context);

                if(EntitySystemBlock.Locked && ((IEntityBatchExt)requestedBatch).HasWriteItem())
                    EntitySystemBlock.Throw();

                if (requestedBatch.Empty)
                {
                    var entitiesContainer = new EntitiesContainer(context ?? new AsyncEntitiesRepositoryRequestContext(this), (EntityBatch)requestedBatch, container, tag);

                    entitiesContainer.Push();

                    if (entitiesContainer.Parent == null)
                        return WaitAndStartTimeout(entitiesContainer, new ValueTask());
                    else
                        return new ValueTask<IEntitiesContainer>(entitiesContainer);
                }
                else
                {
                    if (checkExclusiveOnReplication)
                        checkBatchGetExclusiveOnReplication(requestedBatch);

                    var entitiesContainer = new EntitiesContainer(context ?? new AsyncEntitiesRepositoryRequestContext(this), (EntityBatch)requestedBatch, container, tag);

                    entitiesContainer.Context.Release();

                    entitiesContainer.Push();

                    return LockEntities(entitiesContainer, tag, StackTraceUtils.GetStackTrace());
                }

            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "getInternal exception").Write();
                throw;
            }
        }

        private async ValueTask WaitForBatchWrapper(EntitiesContainer entitiesContainer, EntitiesBatchWaitWrapper waiterBatchWrapper, object tag, StackTrace stackTrace)
        {
            var cts = ServerCoreRuntimeParameters.RepositoryEntityUnlockTimeoutSeconds > 0 ? TimeoutSystem.Install(new UnlockTimeoutPayload(this, waiterBatchWrapper), TimeSpan.FromSeconds(ServerCoreRuntimeParameters.RepositoryEntityUnlockTimeoutSeconds)) : null;
            var startTime = DateTime.UtcNow;
            try
            {
                await waiterBatchWrapper.Task;
            }
            catch (RepositoryTimeoutException)
            {
                entitiesContainer.State = EntitiesContainer.ContainerState.TimedOut;
                var batchesStr = getBatchContainerUsagesInfo(entitiesContainer);
                Logger.Error(
                    "Repository get task is canceled by timeout {0} seconds. EntitiesBatchWaitWrapper {7}, Repository Id {1} {2}. Tag {6}. Batch items count {3} items {4}. StackTrace: ---- {5} ------",
                    ServerCoreRuntimeParameters.RepositoryEntityUnlockTimeoutSeconds, Id, CloudNodeType, entitiesContainer.AllBatchItemsCount,
                    batchesStr.ToString(), stackTrace?.ToString() ?? string.Empty, tag?.ToString() ?? "null", waiterBatchWrapper.Id);

                var sw = prepareWaitLockStatistics();

                lock (_batchLocker)
                {
                    finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.WaitForBatchWrapper);

                    try
                    {
                        _nextQueuesToCheck.Value.Clear();

                        waiterBatchWrapper.RemoveFromQueuesByTimeout(_nextQueuesToCheck.Value);

                        RunNextEntityWaitQueueBatches(_nextQueuesToCheck.Value);
                        _nextQueuesToCheck.Value.Clear();
                    }
                    finally
                    {
                        finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.WaitForBatchWrapper);
                    }
                }

                throw;
            }

            cts?.Cancel();

            var duration = (DateTime.UtcNow - startTime).TotalSeconds;
            if (duration > ToLongWaitEntityErrorLoggingSeconds && Logger.IsWarnEnabled)
            {
                var sb = StringBuildersPool.Get;
                sb.AppendFormat("Entity batch waited too long {0:0.00}, ", duration);
                sb.AppendLine();
                if (entitiesContainer?.Batch != null)
                    entitiesContainer.Batch.DumpToStringBuilder(sb);
                else
                    sb.Append("Batch is empty");
                Logger.IfWarn()?.Message(sb.ToStringAndReturn()).Write();
            }
        }

        private static async ValueTask<IEntitiesContainer> WaitAndStartTimeout(EntitiesContainer entitiesContainer, ValueTask taskToWait)
        {
            if (!taskToWait.IsCompleted)
                await taskToWait;

            entitiesContainer.Context.StartTimeout();
            if (entitiesContainer.Context.LockState != AsyncEntitiesRepositoryRequestContext.ContextLockState.Locked)
                Logger.IfFatal()?.Message("Logic error: {0} is not locked", entitiesContainer.Context).Write();

            return entitiesContainer;
        }

        private string getBatchContainerUsagesInfo(EntitiesContainer container)
        {
            var sb = StringBuildersPool.Get;

            foreach (var entitiesContainer in AsyncStackEnumerable.ToParents(container))
            {
                if (entitiesContainer.Batch == null)
                    continue;
                for (int i = 0; i < entitiesContainer.Batch.Length; i++)
                    getBatchItemUsagesInfo(ref entitiesContainer.Batch.Items[i], sb);

                sb.AppendLine();
                sb.AppendLine("-------");
                sb.AppendLine();
            }

            return sb.ToStringAndReturn();
        }

        private void getBatchItemUsagesInfo(ref BatchItem batchItem, StringBuilder sb)
        {
            var repositoryEntityTypeStates = GetTypeState(batchItem.EntityMasterTypeId);
            EntityUsageRefsCount refsCount;
            repositoryEntityTypeStates.LockedEntities.TryGetValue(batchItem.EntityId, out refsCount);

            EntityQueue entityQueue;
            repositoryEntityTypeStates.WaitQueues.TryGetValue(batchItem.EntityId, out entityQueue);
            EntityQueue.EntityQueueElement[] copyQueueBatches = null;
            HashSet<long> usedContexts = null;

            var canGetEntityQueueLog = entityQueue?.CanGetLog() ?? false;
            var canGetOperationLog = refsCount?.CanGetOperationLog() ?? false;
            if (canGetEntityQueueLog || canGetOperationLog)
                lock (_batchLocker)
                {
                    if (canGetEntityQueueLog)
                        copyQueueBatches = entityQueue.GetBatchesToDump();
                    if (canGetOperationLog)
                        usedContexts = refsCount.CloneUsedContexts();
                }


            sb.AppendFormat("<entity {4}{0}, {1}, {2}, {3}",
                EntitiesRepository.GetTypeById(batchItem.EntityMasterTypeId).Name, batchItem.EntityId, batchItem.RequestOperationType.ToString(),
                batchItem.UpFromReadToExclusive.ToString(), canGetOperationLog ? string.Empty : "(log skipped)");

            if (canGetOperationLog)
            {
                sb.AppendFormat("currentRead: {0} currentWrite {1} currentUsages {2}. Operations:", refsCount.Read, refsCount.Write, refsCount.UsingCount);
                refsCount.GetOperationLog(sb, usedContexts);
            }
            else if (refsCount != null)
                sb.AppendFormat("currentRead: {0} currentWrite {1} currentUsages {2}. Operations:", refsCount.Read, refsCount.Write, refsCount.UsingCount);

            if (canGetEntityQueueLog)
            {
                sb.AppendLine();
                sb.AppendLine(", queue:");
                sb.AppendFormat("<Entity queue: {0} id:{1}, count:{2}, batchWaitWrappers:", EntitiesRepository.GetTypeById(entityQueue.TypeId).Name, entityQueue.EntityId, entityQueue.Count);
                sb.AppendLine();
                for (int i = 0; i < copyQueueBatches.Length; i++)
                {
                    ref var pair = ref copyQueueBatches[i];
                    if (EqualityComparer<EntityQueue.EntityQueueElement>.Default.Equals(pair, default))
                        continue;
                    sb.AppendFormat("<{0}{1}:", pair.OperationType.ToString(), pair.ReadyToUse ? "*" : string.Empty);
                    pair.Batch.DumpToStringBuilder(sb);
                    sb.Append(">");
                }
                sb.AppendLine(">");
            }
            else if (entityQueue != null)
                sb.AppendLine(", queue log skipped");
            sb.Append(">");
        }

        void lockBatch(EntitiesContainer tailContainer)
        {
            var requestContext = tailContainer.Context;
            foreach (var entitiesContainer in AsyncStackEnumerable.ToChildren(tailContainer))
            {
                if (entitiesContainer.Batch == null)
                    continue;
                for (int i = 0; i < entitiesContainer.Batch.Length; i++)
                {
                    ref var batchItem = ref entitiesContainer.Batch.Items[i];
                    var typeState = GetTypeState(batchItem.EntityMasterTypeId);

                    if (!typeState.LockedEntities.TryGetValue(batchItem.EntityId, out var entityUsage))
                    {
                        entityUsage = new EntityUsageRefsCount(batchItem.EntityMasterTypeId, batchItem.EntityId);
                        typeState.LockedEntities[batchItem.EntityId] = entityUsage;
                    }

                    if (batchItem.UpFromReadToExclusive)
                    {
                        entityUsage.UpFromReadToExclusive(entitiesContainer.Batch.Id, batchItem.CallerTag,
                            entitiesContainer.Tag, requestContext.Id);
                    }
                    else if (batchItem.RequestOperationType == ReadWriteEntityOperationType.Write)
                    {
                        entityUsage.UpWrite(entitiesContainer.Batch.Id, batchItem.CallerTag,
                            entitiesContainer.Tag, requestContext.Id);
                    }
                    else
                    {
                        entityUsage.UpRead(entitiesContainer.Batch.Id, batchItem.CallerTag,
                            entitiesContainer.Tag, requestContext.Id);
                    }
                }
            }

            requestContext.LockState = AsyncEntitiesRepositoryRequestContext.ContextLockState.Locked;
        }

        private ThreadLocal<List<EntityBatchOperationWrapper>> _addToQueueList =
            new ThreadLocal<List<EntityBatchOperationWrapper>>(() => new List<EntityBatchOperationWrapper>());
        EntitiesBatchWaitWrapper addToWaitQueue(EntitiesContainer parentEntitiesContainer, EntitiesContainer lastEntitiesContainer)
        {
            _addToQueueList.Value.Clear();

            var batchWrapper = new EntitiesBatchWaitWrapper(parentEntitiesContainer);
            var addedDict = new Dictionary<EntityId, ReadWriteEntityOperationType>();
            int waitCount = 0;
            foreach (var entitiesContainer in AsyncStackEnumerable.ToParents(lastEntitiesContainer))
            {
                if (entitiesContainer.Batch == null)
                    continue;

                for (int i = 0; i < entitiesContainer.Batch.Length; i++)
                {
                    ref var batchItem = ref entitiesContainer.Batch.Items[i];
                    if (addedDict.TryGetValue(new EntityId(batchItem.EntityMasterTypeId, batchItem.EntityId), out var opType))
                    {
                        if (!(opType == ReadWriteEntityOperationType.Write && batchItem.RequestOperationType == ReadWriteEntityOperationType.Read))
                            Logger.Error("addToWaitQueue duplicate add batch to queue. already added {0}, try add {1}. entity {2} id {3} tag {4}",
                                opType, batchItem.RequestOperationType, EntitiesRepository.GetTypeById(batchItem.EntityMasterTypeId).Name, batchItem.EntityId,
                                batchItem.CallerTag?.ToString() ?? "null");

                        continue;
                    }

                    addedDict.Add(new EntityId(batchItem.EntityMasterTypeId, batchItem.EntityId), batchItem.RequestOperationType);

                    var typeState = GetTypeState(batchItem.EntityMasterTypeId);

                    if (!typeState.WaitQueues.TryGetValue(batchItem.EntityId, out var waitQueue))
                    {
                        waitQueue = new EntityQueue(batchItem.EntityMasterTypeId, batchItem.EntityId);
                        typeState.WaitQueues[batchItem.EntityId] = waitQueue;
                    }

                    var currentUsage = getCurrentUsage(batchItem.EntityId, typeState);
                    var readyToUse = false;

                    var requestedLevel = ReplicaTypeRegistry.GetReplicationLevelByReplicaType(ReplicaTypeRegistry.GetTypeById(batchItem.EntityRequestedTypeId));
                    if (!typeState.IsEntityDeferred(batchItem.EntityId, requestedLevel))
                    {
                        if (batchItem.RequestOperationType == ReadWriteEntityOperationType.Write)
                        {
                            if ((currentUsage == null || currentUsage.IsFree()) && waitQueue.IsFree())
                                readyToUse = true;
                        }
                        else if (batchItem.RequestOperationType == ReadWriteEntityOperationType.Read)
                        {
                            if ((currentUsage == null || currentUsage.IsFree() || currentUsage.IsRead()) && !waitQueue.HasWrite())
                                readyToUse = true;
                        }
                    }

                    _addToQueueList.Value.Add(new EntityBatchOperationWrapper(batchItem.RequestOperationType, waitQueue));

                    if (!readyToUse)
                        waitCount++;

                    waitQueue.Enqueue(batchItem.RequestOperationType, batchWrapper, readyToUse, requestedLevel);
                }
            }

            batchWrapper.SetQueues(_addToQueueList.Value.ToArray(), waitCount);
            return batchWrapper;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        EntityUsageRefsCount getCurrentUsage(Guid entityId, RepositoryEntityTypeStates states)
        {
            EntityUsageRefsCount lockedEntities;
            if (!states.LockedEntities.TryGetValue(entityId, out lockedEntities))
                return null;

            return lockedEntities;
        }
        
        private void AddSubscriptionChange(Func<Task> change)
        {
            _subscriptionsChangesStatistics.IncChangeQueue();
            var changeInfo = new SubscriptionsChange(change, DateTime.Now);
            if (!_subscriptionsChanges.Writer.TryWrite(changeInfo))
            {
                _subscriptionsChanges.Writer.WriteAsync(changeInfo).GetAwaiter().GetResult();
                Logger.IfError()?.Message("_subscriptionsChanges couldn't sync write").Write();
            }
        }

        private void StartSubscriptionsProcessing()
        {
            Task.Run(async () =>
            {
                var sw = new Stopwatch();
                while (true)
                {
                    StopToken.ThrowIfCancellationRequested();
                    var subscriptionChangeInfo = await _subscriptionsChanges.Reader.ReadAsync(StopToken);
                    _subscriptionsChangesStatistics.DecChangeQueue();
                    await AsyncUtils.RunAsyncTask(async () =>
                    {
                        _subscriptionsChangesStatistics.AddWaitTime(DateTime.Now - subscriptionChangeInfo.StartDate);
                        sw.Restart();
                        await subscriptionChangeInfo.ChangeProcessor();
                        sw.Stop();
                        _subscriptionsChangesStatistics.AddProcessTime(sw.Elapsed);
                    });
                }
            });
        }

        public readonly struct RepositoryReplicationInfo
        {
            public RepositoryReplicationInfo(Guid repositoryId, ReplicationLevel replicationMask, List<ReplicationLevel> flatReplicationLevels)
            {
                RepositoryId = repositoryId;
                ReplicationMask = replicationMask;
                FlatReplicationLevels = flatReplicationLevels;
            }

            public Guid RepositoryId { get; }

            public ReplicationLevel ReplicationMask { get; }

            public List<ReplicationLevel> FlatReplicationLevels { get; }
        }

        private void CollectDiff(ref AsyncStackEnumerable containers)
        {
            try
            {
                _snapshotsBuffer.Value.Clear();
                var snapshots = _snapshotsBuffer.Value;
                HashSet<(int typeId, Guid entityId)> entitiesToDestroy = null;

                try
                {
                    foreach (var currentContainer in containers)
                    {
                        if (currentContainer.Batch == null)
                            continue;

                        for (int containerIndex = 0; containerIndex < currentContainer.Batch.Length; containerIndex++)
                        {
                            ref var entityContainerItem = ref currentContainer.Batch.Items[containerIndex];
                            if (entityContainerItem.RequestOperationType == ReadWriteEntityOperationType.Write)
                            {
                                var entityRef = ((IEntitiesRepositoryExtension) this).GetRef(entityContainerItem.EntityMasterTypeId,
                                    entityContainerItem.EntityId);
                                var entity = ((IEntityRefExt) entityRef)?.GetEntity();
                                var entityExt = (IEntityExt) entity;
                                if (entity != null && entityExt.IsMaster() && !entityExt.IsMigratingDestination())
                                {
                                    Dictionary<Guid, List<DeferredEntityModel>> newLinkedEntities = null;
                                    if (((IEntityRefExt) entityRef).EntityRefChanges?.Count > 0)
                                    {
                                        var rootEntityReplicationRepositories = new List<RepositoryReplicationInfo>();
                                        foreach (var replicationRepository in ((BaseEntity) entity).ReplicateRepositoryIds)
                                        {
                                            var subscriptions = replicationRepository.Value.GetFlatSubscribers();

                                            rootEntityReplicationRepositories.Add(
                                                new RepositoryReplicationInfo(replicationRepository.Key,
                                                    (ReplicationLevel) replicationRepository.Value.GetReplicationMask(),
                                                    subscriptions));
                                        }

                                        var entityRefSubscriptionsChanges =
                                            new Queue<EntityRefSubscriptionsChange>(((IEntityRefExt) entityRef).EntityRefChanges.Count);
                                        foreach (var entityRefChangePair in ((IEntityRefExt) entityRef).EntityRefChanges)
                                        {
                                            var entityRefChange = entityRefChangePair.Value;
                                            var subscriptionsChanges = GetSubscriptionsChanges(
                                                rootEntityReplicationRepositories,
                                                entityRefChange.OldReachabilityReplicationLevel,
                                                entityRefChange.NewReachabilityReplicationLevel);

                                            if (subscriptionsChanges != null && subscriptionsChanges.Count != 0)
                                            {
                                                foreach (var subscriptionChange in subscriptionsChanges)
                                                {
                                                    if (subscriptionChange.NewSubscriber)
                                                    {
                                                        if (newLinkedEntities == null)
                                                        {
                                                            newLinkedEntities = new Dictionary<Guid, List<DeferredEntityModel>>();
                                                        }

                                                        var repositoryNewLinkedEntities =
                                                            newLinkedEntities.GetOrCreate(subscriptionChange.RepositoryId,
                                                                r => new List<DeferredEntityModel>());
                                                        repositoryNewLinkedEntities.Add(new DeferredEntityModel(
                                                            new OuterRef(entityRefChange.Value.Id, entityRefChange.Value.TypeId),
                                                            subscriptionChange.ReplicationMask));
                                                    }
                                                }

                                                entityRefSubscriptionsChanges.Enqueue(new EntityRefSubscriptionsChange(
                                                    entityRefChange.OldReachabilityReplicationLevel,
                                                    entityRefChange.NewReachabilityReplicationLevel,
                                                    entityRefChange.Value,
                                                    subscriptionsChanges));
                                            }
                                        }

                                        ((IEntityRefExt) entityRef).EntityRefChanges.Clear();

                                        AddSubscriptionChange(() =>
                                            ProcessSubscriptions(rootEntityReplicationRepositories, entityRefSubscriptionsChanges, false));
                                    }

                                    var snapshot = CollectDelta(entityRef, currentContainer);
                                    if (snapshot != null)
                                    {
                                        snapshots.Add((entityContainerItem, snapshot, newLinkedEntities));
                                    }

                                    if (entityExt.State == EntityState.Destroyed)
                                    {
                                        if (entitiesToDestroy == null)
                                            entitiesToDestroy = new HashSet<(int typeId, Guid entityId)>();
                                        entitiesToDestroy.Add((entityContainerItem.EntityMasterTypeId, entityContainerItem.EntityId));
                                    }
                                }
                            }
                        }
                    }

                    foreach (var (entity, snapshot, newLinkedEntities) in snapshots)
                    {
                        var entityRef = ((IEntitiesRepositoryExtension) this).GetRef(entity.EntityMasterTypeId, entity.EntityId);
                        sendDelta(entityRef, snapshot, ((IEntityRefExt) entityRef).Version, newLinkedEntities);
                        ((IEntityRefExt) entityRef).AfterExclusiveUsing();
                    }

                    if (entitiesToDestroy != null)
                    {
                        foreach (var (type, guid) in entitiesToDestroy)
                            RemoveInternal(type, guid);
                    }
                }
                finally
                {
                    _snapshotsBuffer.Value.Clear();
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "CollectDiff exception. StackTrace: {0}", new StackTrace()).Write();
            }
        }

        /// <summary>
        /// Get information about subscriptions changes for linked entity
        /// </summary>
        public List<RepositorySubscriptionsChanges> GetSubscriptionsChanges(
            List<RepositoryReplicationInfo> rootEntityReplicationRepositories,
            ReplicationLevel? oldReachabilityReplicationLevel,
            ReplicationLevel? newReachabilityReplicationLevel)
        {
            List<RepositorySubscriptionsChanges> repositorySubscriptionsChanges = null;
            foreach (var repositoryReplicationInfo in rootEntityReplicationRepositories)
            {
                var wasSubscribed = oldReachabilityReplicationLevel != null
                                    && ((long)repositoryReplicationInfo.ReplicationMask & (long) oldReachabilityReplicationLevel.Value) ==
                                    (long) oldReachabilityReplicationLevel.Value;

                var shouldBeSubscribed = newReachabilityReplicationLevel != null
                                         && ((long)repositoryReplicationInfo.ReplicationMask & (long) newReachabilityReplicationLevel.Value) ==
                                         (long) newReachabilityReplicationLevel.Value;

                bool subscribe = false;
                bool unsubscribe = false;
                List<(bool, ReplicationLevel)> subscriptionsChanges = null;
                if (shouldBeSubscribed != wasSubscribed)
                {
                    subscriptionsChanges = new List<(bool, ReplicationLevel)>();

                    if (shouldBeSubscribed)
                    {
                        // copy all root subscriptions if it is first time when entity is reachable
                        subscribe = true;
                        foreach (var replicationLevel in repositoryReplicationInfo.FlatReplicationLevels)
                        {
                            subscriptionsChanges.Add((true, replicationLevel));
                        }
                    }
                    else if (wasSubscribed)
                    {
                        // delete all root subscriptions if entity now is unreachable
                        unsubscribe = true;
                        foreach (var replicationLevel in repositoryReplicationInfo.FlatReplicationLevels)
                        {
                            subscriptionsChanges.Add((false, replicationLevel));
                        }
                    }
                }

                if (subscriptionsChanges != null && subscriptionsChanges.Count != 0)
                {
                    if (repositorySubscriptionsChanges == null)
                    {
                        repositorySubscriptionsChanges = new List<RepositorySubscriptionsChanges>(rootEntityReplicationRepositories.Count);
                    }
                    
                    repositorySubscriptionsChanges.Add(new RepositorySubscriptionsChanges(
                        repositoryReplicationInfo.RepositoryId,
                        subscriptionsChanges,
                        repositoryReplicationInfo.ReplicationMask,
                        subscribe,
                        unsubscribe));
                }
            }

            return repositorySubscriptionsChanges;
        }
        
        private async Task ProcessSubscriptions(
            List<RepositoryReplicationInfo> rootEntityReplicationRepositories,
            Queue<EntityRefSubscriptionsChange> entitiesToProcess,
            bool subscribeToDatabase)
        {
            if (entitiesToProcess.Count == 0)
            {
                return;
            }

            var maxSubscriptionsMask = 0L;
            foreach (var rootEntityReplicationRepository in rootEntityReplicationRepositories)
            {
                maxSubscriptionsMask |= (long)rootEntityReplicationRepository.ReplicationMask;
            }

            var collections = new EntityCollections();
            while (entitiesToProcess.Count != 0)
            {
                var processingEntity = entitiesToProcess.Dequeue();
                using (var entityContainer = await GetExclusive(processingEntity.Value.TypeId, processingEntity.Value.Id, "ProcessSubscriptions"))
                {
                    if (!entityContainer.TryGet<BaseEntity>(processingEntity.Value.TypeId, processingEntity.Value.Id, out var entity))
                    {
                        Logger.Warn("Couldn't get entity when was trying to subscribe/unsubscribe {entityId} {entityTypeId}", processingEntity.Value.Id,
                            processingEntity.Value.TypeId);
                        return;
                    }

                    _linkedEntitiesSingleThreadBuffer.Value.Clear();
                    _newLinkedEntitiesBuffer.Value.Clear();
                    _entitySubscriptionsProcessor.Process(this,
                        entitiesToProcess,
                        _newLinkedEntitiesBuffer.Value,
                        _linkedEntitiesSingleThreadBuffer.Value,
                        rootEntityReplicationRepositories,
                        maxSubscriptionsMask,
                        subscribeToDatabase,
                        processingEntity,
                        entity,
                        ref collections);

                    _linkedEntitiesSingleThreadBuffer.Value.Clear();
                }
            }
            
            uploadCreatedEntities(ref collections);
            uploadDestroyEntities(ref collections);
        }

        // private static List<(bool, ReplicationLevel)> GetRootEntitySubscriptionsCopy(
        //     Dictionary<Guid, Dictionary<ReplicationLevel, List<(bool, ReplicationLevel)>>> subscriptionsCache,
        //     ReplicationInfo replicationRepository,
        //     EntityRefChange entityRefChange)
        // {
        //     if (!subscriptionsCache.TryGetValue(replicationRepository.RepositoryId, out var repositorySubscriptionsCopy))
        //     {
        //         repositorySubscriptionsCopy = new Dictionary<ReplicationLevel, List<(bool, ReplicationLevel)>>();
        //         subscriptionsCache.Add(replicationRepository.RepositoryId, repositorySubscriptionsCopy);
        //     }
        //
        //     if (!repositorySubscriptionsCopy.TryGetValue(entityRefChange.FieldReplicationLevel, out var repositoryLevelSubscriptionsCopy))
        //     {
        //         repositoryLevelSubscriptionsCopy = new List<(bool, ReplicationLevel)>();
        //         foreach (var replicationLevel in replicationRepository.ReplicationLevels)
        //         {
        //             var add = entityRefChange.Type == EntityRefChange.ChangeType.Added;
        //             repositoryLevelSubscriptionsCopy.Add((add, replicationLevel));
        //         }
        //
        //         repositorySubscriptionsCopy.Add(entityRefChange.FieldReplicationLevel, repositoryLevelSubscriptionsCopy);
        //     }
        //
        //     return repositoryLevelSubscriptionsCopy;
        // }

        public void Release(ref AsyncStackEnumerable containers)
        {
            CollectDiff(ref containers);

            try
            {
                var sw = prepareWaitLockStatistics();
                lock (_batchLocker)
                {
                    finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.Release);

                    try
                    {
                        _nextQueuesToCheck.Value.Clear();
                        foreach (var currentContainer in containers)
                        {
                            if (currentContainer.Batch == null)
                                continue;

                            for (int i = 0; i < currentContainer.Batch.Length; i++)
                            {
                                ref var entityContainerItem = ref currentContainer.Batch.Items[i];
                                var typeState = GetTypeState(entityContainerItem.EntityMasterTypeId);

                                EntityUsageRefsCount entityUsageRefsCount;
                                typeState.LockedEntities.TryGetValue(entityContainerItem.EntityId, out entityUsageRefsCount);

                                if (entityUsageRefsCount == null)
                                {
                                    Logger.IfError()?.Message("Release: {0} usages not found", entityContainerItem).Write();
                                    continue;
                                }

                                if (entityContainerItem.UpFromReadToExclusive)
                                {
                                    entityUsageRefsCount.DownFromReadToExclusive(currentContainer.Batch.Id, entityContainerItem.CallerTag, currentContainer.Tag, currentContainer.Context.Id);
                                }
                                else if (entityContainerItem.RequestOperationType == ReadWriteEntityOperationType.Write)
                                {
                                    entityUsageRefsCount.DownWrite(currentContainer.Batch.Id, entityContainerItem.CallerTag, currentContainer.Tag, currentContainer.Context.Id);
                                }
                                else
                                {
                                    entityUsageRefsCount.DownRead(currentContainer.Batch.Id, entityContainerItem.CallerTag, currentContainer.Tag, currentContainer.Context.Id);
                                }

                                // if someone is waiting for entity that we are currently releasing
                                // then add it's queue to the check list
                                if (typeState.WaitQueues.TryGetValue(entityContainerItem.EntityId, out var entityQueue))
                                    _nextQueuesToCheck.Value.Add(entityQueue);
                            }
                        }

                        RunNextEntityWaitQueueBatches(_nextQueuesToCheck.Value);
                        _nextQueuesToCheck.Value.Clear();
                    }
                    finally
                    {
                        finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.Release);
                    }
                }

            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Release exception").Write();;
            }
        }

        private void RunNextEntityWaitQueueBatches(HashSet<EntityQueue> queuesToCheck)
        {
            foreach (var entityQueues in queuesToCheck)
            {
                var typeState = GetTypeState(entityQueues.TypeId);
                
                var currentUsage = getCurrentUsage(entityQueues.EntityId, typeState);
                if (currentUsage != null && currentUsage.IsWrite())
                {
                    // entity is definitely is not ready when it is locked on write
                    continue;
                }

                // considering every lock on this entity for the purpose of unlocking several read locks 
                for (int i = 0; i < entityQueues.Count; i++)
                {
                    ref var entityLockRequest = ref entityQueues[i];
                    var requestIsWrite = entityLockRequest.OperationType == ReadWriteEntityOperationType.Write;

                    //entityLockRequest.ReadyToUse == true если в этой очереди батч уже был проверен и готов к использованию. Запустится по команде с другой очереди
                    if (!entityLockRequest.ReadyToUse)
                    {
                        var batchLockRequest = entityLockRequest.Batch;
                        var canUseEntity = true;

                        // я хз зачем это (если !entityLockRequest.ReadyToUse)
                        if (batchLockRequest.Task.IsCompleted)
                        {
                            canUseEntity = false;
                        }
                        else if (requestIsWrite && currentUsage != null && currentUsage.IsRead())
                        {
                            canUseEntity = false;
                        }
                        else
                        {
                            if (typeState.IsEntityDeferred(entityQueues.EntityId, entityLockRequest.RequestedLevel))
                            {
                                canUseEntity = false;
                            }
                            else if (requestIsWrite)
                            {
                                // checking that the next element in queue is write 
                                if (entityQueues.TryWrite(batchLockRequest))
                                {
                                    entityLockRequest.ReadyToUse = true;
                                }
                                else
                                {
                                    canUseEntity = false;
                                }
                            }
                            else 
                            {
                                // checking that before that element queue doesn't have write request
                                if (entityQueues.TryRead(batchLockRequest))
                                {
                                    entityLockRequest.ReadyToUse = true;
                                }
                                else
                                {
                                    canUseEntity = false;
                                }
                            }
                        }

                        // full batch should be ready, not only this entity
                        if (canUseEntity && batchLockRequest.IsReadyToUse())
                        {
                            batchLockRequest.RemoveFromQueues();
                            i--;

                            lockBatch(batchLockRequest.EntitiesContainer);
                            batchLockRequest.Complete();
                        }
                    }
 
                    // we don't consider read locks after write
                    // because otherwise write lock could starve
                    if (requestIsWrite)
                        break;
                }
            }
        }

        private const float CheckClearOldUsagesTimePeriodSeconds = 30;

        private readonly struct PeriodicCheckClearOldUsages : ITimeoutPayload
        {
            private readonly EntitiesRepository _repo;

            public PeriodicCheckClearOldUsages(EntitiesRepository repo)
            {
                _repo = repo;
            }

            public bool Run()
            {
                if (_repo.StopToken.IsCancellationRequested)
                    return true;

                try
                {
                    _repo.checkClearOldUsages();
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "PeriodicСheckClearOldUsages exception").Write();;
                }

                return false;
            }
        }

        protected void PeriodicСheckClearOldUsages()
        {
            TimeoutSystem.Install(new PeriodicCheckClearOldUsages(this), TimeSpan.FromSeconds(CheckClearOldUsagesTimePeriodSeconds));
        }

        void checkClearOldUsages()
        {
            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.CheckClearOldUsages);

                try
                {
                    long lockedClean = 0;
                    long waitClean = 0;
                    foreach (var typeStatePair in _typeStates)
                    {
                        var typeState = typeStatePair.Value;
                        foreach (var pair in typeState.LockedEntities)
                        {
                            if (pair.Value.IsFree())
                            {
                                EntityUsageRefsCount entityUsageRefsCountRemove;
                                typeState.LockedEntities.TryRemove(pair.Key, out entityUsageRefsCountRemove);
                                lockedClean++;
                            }
                        }

                        foreach (var pair in typeState.WaitQueues)
                        {
                            if (pair.Value.IsFree())
                            {
                                EntityQueue entityQueueRemove;
                                typeState.WaitQueues.TryRemove(pair.Key, out entityQueueRemove);
                                entityQueueRemove.Dispose();
                                waitClean++;
                            }
                        }
                    }
                    Logger.IfDebug()?.Message("Repository {0} clean EntityUsageRefsCount {1} EntityQueue {2}", Id, lockedClean, waitClean).Write();
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.CheckClearOldUsages);
                }
            }
        }

        private bool ApplyUpdate(UpdateBatch updateBatch, out IEntityRef validEntityRef)
        {
            var bytesCount = updateBatch.Snapshot?.Sum(o => o.Value.Length) ?? 0;
            Statistics<EntityUpdateStatistics>.Instance.EntityUpdateReceived(updateBatch.EntityTypeId, bytesCount);

            validEntityRef = null;
            // if update arrived before upload
            var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(updateBatch.EntityTypeId, updateBatch.EntityId);
            var entityIsNotReadyBeforeLock = entityRef == null || ((IEntityRefExt) entityRef).State == EntityState.Created ||
                                   ((IEntityRefExt) entityRef).State == EntityState.None;
            var tempEntityBeforeLock = entityRef != null && ((IEntityRefExt) entityRef).Version == -1;
            
            if (entityIsNotReadyBeforeLock || tempEntityBeforeLock)
            {
                var repositoryEntityTypeStates = GetTypeState(updateBatch.EntityTypeId);
                lock (repositoryEntityTypeStates)
                {
                    entityRef = ((IEntitiesRepositoryExtension)this).GetRef(updateBatch.EntityTypeId, updateBatch.EntityId);
                    var entityIsNotReady = entityRef == null || ((IEntityRefExt) entityRef).State == EntityState.Created ||
                                           ((IEntityRefExt) entityRef).State == EntityState.None;
                    var tempEntity = entityRef != null && ((IEntityRefExt) entityRef).Version == -1;
                    if (entityIsNotReady || tempEntity)
                    {
                        if (tempEntity)
                        {
                            Logger.IfWarn()?.Message("EntitiesRepository {0} EntityUpdate entity version is temporaly (-1) typeName {1} entityId {2} update version {3}. added to wait queue", this.Id, entityRef.TypeName, updateBatch.EntityId, updateBatch.Version).Write();
                        }
                        else
                        {
                            Logger.Warn("EntitiesRepository {0} EntityUpdate entity not found typeName {1} entityId {2} update version {3}. added to wait queue",
                                this.Id, EntitiesRepository.GetTypeById(updateBatch.EntityTypeId).GetFriendlyName(), updateBatch.EntityId, updateBatch.Version);
                        }
                        
                        var waitUpdateInfo = repositoryEntityTypeStates.WaitUpdates.GetOrAdd(updateBatch.EntityId, 
                            eId => new WaitUpdateInfo(new ConcurrentQueue<UpdateBatch>(), DateTime.UtcNow, false));
                        waitUpdateInfo.WaitUpdateQueue.Enqueue(updateBatch);
                        return false;
                    }
                }
            }

            entityRef.AddDeltaSnapshot(updateBatch.Snapshot, updateBatch.DeferredEntities, bytesCount, updateBatch.ReplicationMask, updateBatch.Version, updateBatch.PreviousVersion);
            validEntityRef = entityRef;
            return true;
        }

        public async Task EntityUpdate(UpdateBatch updateBatch)
        {
            var updateApplied = ApplyUpdate(updateBatch, out var entityRef);
            if (!updateApplied)
            {
                return;
            }
            
            var alwaysTypeId = GetReplicationTypeId(updateBatch.EntityTypeId, ReplicationLevel.Always);
            var batch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusive(alwaysTypeId, updateBatch.EntityId);

            var needProcessDeltaSnapshot = true;
            while (needProcessDeltaSnapshot)
            {
                needProcessDeltaSnapshot = false;
                try
                {
                    using (var container = await get(batch, checkExclusiveOnReplication: false))
                    {
                        var snapshotEntityRef = container?.GetEntityRef(updateBatch.EntityTypeId, updateBatch.EntityId);
                        if (snapshotEntityRef == null)
                        {
                            Logger.IfError()?.Message("EntityUpdate entityRef not found typeId {0} entityId {1}", EntitiesRepository.GetTypeById(updateBatch.EntityTypeId).Name, updateBatch.EntityId).Write();
                            return;
                        }

                        TryProcessDeltaSnapshots(snapshotEntityRef);
                    }
                }
                catch (RepositoryTimeoutException e)
                {
                    Logger.IfError()?.Message(e, "EntityUpdate ProcessDeltaSnapshots update timeout typeId {0} entityId {1}", EntitiesRepository.GetTypeById(updateBatch.EntityTypeId).Name, updateBatch.EntityId).Write();
                    needProcessDeltaSnapshot = true;
                }
            }

            OnEntityUpdated(entityRef);
        }

        private void TryProcessDeltaSnapshots(IEntityRef snapshotEntityRef)
        {
            var entityRefOperationResult = snapshotEntityRef.ProcessDeltaSnapshots(this);
            if (entityRefOperationResult.HasValue)
            {
                lock (_batchLocker)
                {
                    if (entityRefOperationResult.Value.DeferredEntities != null)
                    {
                        TryAddDeferredEntities(entityRefOperationResult.Value.DeferredEntities);
                    }
                    
                    if (entityRefOperationResult.Value.ReplicationLevelChanged)
                    {
                        var typeState = GetTypeState(snapshotEntityRef.TypeId);
                        if (typeState.TryRemoveDeferredEntity(snapshotEntityRef.Id, (ReplicationLevel) snapshotEntityRef.CurrentReplicationMask))
                        {
                            if (typeState.WaitQueues.TryGetValue(snapshotEntityRef.Id, out var entityQueue))
                            {
                                _nextQueuesToCheck.Value.Clear();
                                _nextQueuesToCheck.Value.Add(entityQueue);
                                RunNextEntityWaitQueueBatches(_nextQueuesToCheck.Value);
                            }
                        }
                    }
                }
            }
        }

        public async Task EntityUpdate(UpdateBatchContainer updateBatchContainer)
        {
            var updatedRefs = new List<IEntityRef>();
            foreach (var containerBatch in updateBatchContainer.Batches)
            {
                var updateApplied = ApplyUpdate(containerBatch, out var entityRef);
                if (updateApplied)
                {
                    updatedRefs.Add(entityRef);
                }
            }

            var needProcessDeltaSnapshot = true;
            while (needProcessDeltaSnapshot)
            {
                needProcessDeltaSnapshot = false;
                try
                {
                    var batch = EntityBatch.Create();
                    foreach (var updatedRef in updatedRefs)
                    {
                        var alwaysTypeId = GetReplicationTypeId(updatedRef.TypeId, ReplicationLevel.Always);
                        ((IEntityBatchExt)batch).AddExclusive(alwaysTypeId, updatedRef.Id);
                    }

                    using (var container = await get(batch, checkExclusiveOnReplication: false))
                    {
                        foreach (var updatedRef in updatedRefs)
                        {
                            var entityRef =
                                ((IEntitiesContainerExtension)container).GetEntityRef(updatedRef.TypeId, updatedRef.Id);
                            TryProcessDeltaSnapshots(entityRef);
                        }
                    }
                }
                catch (RepositoryTimeoutException e)
                {
                    Logger.IfError()?.Message(e, "EntityUpdateBatch ProcessDeltaSnapshots update timeout: {0}", string.Join(",", updatedRefs.Select(x => $"{x.TypeName}:{x.Id}"))).Write();
                    needProcessDeltaSnapshot = true;
                }
            }

            foreach (var updatedRef in updatedRefs)
            {
                OnEntityUpdated(updatedRef);
            }
        }

        public async Task EntityDowngrade(DowngradeBatchContainer downgradeBatchContainer)
        {
            var downgradedRefs = new List<IEntityRef>();
            foreach (var containerBatch in downgradeBatchContainer.Batches)
            {
                var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(containerBatch.EntityTypeId, containerBatch.EntityId);
                if (entityRef == null)
                {
                    Logger.IfError()?.Message("EntitiesRepository EntityUpdate entity not found typeId {0} entityId {1}", containerBatch.EntityTypeId, containerBatch.EntityId).Write();
                    continue;
                }
                entityRef.AddDowngrade(containerBatch.DowngradeMask, containerBatch.Version, containerBatch.PreviousVersion);
                downgradedRefs.Add(entityRef);
            }

            var batch = EntityBatch.Create();
            foreach (var downgradedRef in downgradedRefs)
                ((IEntityBatchExt)batch).AddExclusive(downgradedRef.TypeId, downgradedRef.Id);
            using (var container = await get(batch, checkExclusiveOnReplication: false))
            {
                foreach (var updatedRef in downgradedRefs)
                {
                    var entityRef = ((IEntitiesContainerExtension)container).GetEntityRef(updatedRef.TypeId, updatedRef.Id);
                    TryProcessDeltaSnapshots(entityRef);
                }
            }
        }

        protected void SetTemporalyRemoteRepositoryCommunicationEntity(RepositoryCommunicationEntity remoteProxy, bool external)
        {
            lock (_batchLocker)
            {
                if (((IEntitiesRepositoryExtension)this).GetRef<IRepositoryCommunicationEntity>(remoteProxy.Id) == null)
                {
                    Logger.IfDebug()?.Message("{0} Create proxy new node Internal: {1} External:{2} external {3} repositoryId {4}", Id, remoteProxy.InternalAddress, remoteProxy.ExternalAddress, external, remoteProxy.Id).Write();
                    SetInternal(remoteProxy.TypeId, remoteProxy, (long)ReplicationLevel.Server, -1);
                    remoteProxy.SetState(EntityState.Ready);
                }
            }
        }

        public Task EntityUpload(UploadBatchContainer uploadBatchContainer, INetworkProxy networkProxy)
        {
            var uploadedRefs = new List<IEntityRef>();

            foreach (var containerBatch in uploadBatchContainer.Batches)
            {
                var bytesCount= containerBatch.Snapshot?.Sum(o => o.Value.Length) ?? 0;
                Statistics<EntityUploadStatistics>.Instance.EntityUploadReceived(containerBatch.EntityTypeId,
                    bytesCount);
            }

            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.EntityUpload);

                try
                {

                    foreach (var containerBatch in uploadBatchContainer.Batches)
                    {
                        using (_EntityContext.Pool.Set())
                        {
                            _EntityContext.Pool.Current.FullCreating = true;

                            var type = ReplicaTypeRegistry.GetTypeById(containerBatch.EntityTypeId);
                            var entity = ReplicaTypeRegistry.Deserialize(type, _entitySerializer, this,
                                containerBatch.EntityTypeId, containerBatch.EntityId, containerBatch.Snapshot);
                            var entityRef = SetInternal(containerBatch.EntityTypeId, entity,
                                containerBatch.ReplicationMask, containerBatch.Version);
                            uploadedRefs.Add(entityRef);
                        }

                        TryAddDeferredEntities(containerBatch.DeferredEntities);
                    }

                    _nextQueuesToCheck.Value.Clear();
                    foreach (var uploadedRef in uploadedRefs)
                    {
                        using (_EntityContext.Pool.Set())
                        {
                            _EntityContext.Pool.Current.FullCreating = true;

                            var entity = ((IEntityRefExt) uploadedRef).GetEntity();
                            if (entity is RepositoryCommunicationEntity && !((IEntityExt) entity).IsMaster())
                                ((IRemoteEntity) entity).SetNetworkProxy(networkProxy);
                            else
                                CheckRemoteEntity(entity);

                            ((IEntityExt) entity).SetState(EntityState.Ready);

                            if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                            {

                                SubscriptionsLogger.IfInfo()?.Message("Uploaded entity entityTypeId={entityTypeId} entityId={entityId} mask={mask}", entity.TypeId,
                                    entity.Id, uploadedRef.CurrentReplicationMask)
                                    .Write();
                            }

                            var typeState = GetTypeState(entity.TypeId);
                            if (typeState.TryRemoveDeferredEntity(entity.Id, (ReplicationLevel) uploadedRef.CurrentReplicationMask))
                            {
                                if (typeState.WaitQueues.TryGetValue(entity.Id, out var entityQueue))
                                {
                                    _nextQueuesToCheck.Value.Add(entityQueue);
                                }
                            }
                        }
                    }
                    
                    RunNextEntityWaitQueueBatches(_nextQueuesToCheck.Value);
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.EntityUpload);
                }
            }

            List<IEntityRef> entitiesToProcessDeltaSnapshots = null;
            foreach (var uploadedRef in uploadedRefs)
            {
                var entity = ((IEntityRefExt)uploadedRef).GetEntity();
                if (!SuppressEntityInitialization)
                    ((IEntityExt)entity).FireOnReplicationLevelChanged(0, ((IEntityExt)entity).CurrentReplicationMask);

                OnEntityUploaded(uploadedRef);

                var repositoryEntityTypeStates = GetTypeState(uploadedRef.TypeId);
                lock (repositoryEntityTypeStates)
                {
                    //Проверяем есть ли в очереди дельты, которые пришли до самой энтити, чтобы накатить их
                    if (repositoryEntityTypeStates.WaitUpdates.TryRemove(uploadedRef.Id, out var waitUpdateInfo) && waitUpdateInfo.WaitUpdateQueue.Count > 0)
                    {
                        foreach (var updateBatch in waitUpdateInfo.WaitUpdateQueue)
                        {
                            // TODO Выпилить
                            var bytesCount = updateBatch.Snapshot?.Sum(o => o.Value.Length) ?? 0;
                            uploadedRef.AddDeltaSnapshot(
                                updateBatch.Snapshot, 
                                updateBatch.DeferredEntities,
                                bytesCount,
                                updateBatch.ReplicationMask,
                                updateBatch.Version,
                                updateBatch.PreviousVersion);
                        }

                        if (entitiesToProcessDeltaSnapshots == null)
                            entitiesToProcessDeltaSnapshots = new List<IEntityRef>();
                        entitiesToProcessDeltaSnapshots.Add(uploadedRef);
                    }
                }
            }

            if (entitiesToProcessDeltaSnapshots != null)
            {
                AsyncUtils.RunAsyncTask(async () =>
                {
                    var batch = EntityBatch.Create();
                    foreach (var uploadedRef in entitiesToProcessDeltaSnapshots)
                        ((IEntityBatchExt)batch).AddExclusive(uploadedRef.TypeId, uploadedRef.Id);

                    using (var container = await get(batch, checkExclusiveOnReplication: false))
                    {
                        foreach (var uploadedRef in entitiesToProcessDeltaSnapshots)
                        {
                            var snapshotEntityRef = container?.GetEntityRef(uploadedRef.TypeId, uploadedRef.Id);
                            if (snapshotEntityRef == null)
                            {
                                Logger.IfError()?.Message("ProcessDeltaSnapshot from queue after upload entityRef not found typeId {0} entityId {1}", uploadedRef.TypeId, uploadedRef.Id).Write();
                                continue;
                            }

                            TryProcessDeltaSnapshots(snapshotEntityRef);
                            OnEntityUpdated(snapshotEntityRef);
                        }
                    }
                });
            }

            return Task.CompletedTask;
        }

        private void StartDetectingFreezedUpdates()
        {
            Task.Run(async () =>
            {
                while (!StopToken.IsCancellationRequested)
                {
                    foreach (var typeState in _typeStates)
                    {
                        foreach (var waitUpdate in typeState.Value.WaitUpdates)
                        {
                            if (!waitUpdate.Value.Logged)
                            {
                                var waitTime = DateTime.UtcNow - waitUpdate.Value.FirstWaitDate;
                                if (waitTime > ServerCoreRuntimeParameters.WaitUpdateTime)
                                {
                                    typeState.Value.WaitUpdates.TryUpdate(waitUpdate.Key,
                                        new WaitUpdateInfo(waitUpdate.Value.WaitUpdateQueue, waitUpdate.Value.FirstWaitDate, true),
                                        waitUpdate.Value);
                                    Logger.IfWarn()?.Message(
                                        "Entity  {entityType} {entityId} has {updateCount} updates and still has not been created. Waitime is {waitTime}",
                                        typeState.Key, waitUpdate.Key, waitUpdate.Value.WaitUpdateQueue.Count, waitTime)
                                        .Write();
                                }
                            }
                        }
                    }

                    await Task.Delay(ServerCoreRuntimeParameters.HangDetectTimeout, StopToken);
                }
            });
        }

        public void Dump(Stream stream)
        {
            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.Dump);

                try
                {
                    DumpImpl(stream);
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.Dump);
                }
            }
        }

        protected void DumpEntityInternal(int typeId, Guid entityId, Stream stream)
        {
            var sw = prepareWaitLockStatistics();

            lock (_batchLocker)
            {
                finishWaitAndPrepareUseLockStatistics(ref sw, LockRepositoryStatistics.LockRepositoryOperation.DumpEntity);

                try
                {
                    DumpEntityInternalInternal(typeId, entityId, stream);
                }
                finally
                {
                    finishUseLockStatistics(sw, LockRepositoryStatistics.LockRepositoryOperation.DumpEntity);
                }
            }
        }

        protected Task UnloadAll()
        {
            return UnloadAllImpl();
        }

        public override string ToString()
        {
            return $"Repository {Id}, Type {CloudNodeType}";
        }

        public EntityQueue GetEntityQueue(int typeId, Guid entityId)
        {
            return GetTypeState(typeId).WaitQueues[entityId];
        }

        public IEnumerable<IEntityRef> GetAllEntitiesDebug()
        {
            return GetAllEntityDebug();
        }
        
        private readonly struct SubscriptionsChange
        {
            public SubscriptionsChange(Func<Task> changeProcessor, DateTime startDate)
            {
                ChangeProcessor = changeProcessor;
                StartDate = startDate;
            }

            public Func<Task> ChangeProcessor { get; }
            public DateTime StartDate { get; }
        }
    }

    public struct EntityId : IEquatable<EntityId>
    {
        public EntityId(int typeId, Guid guid)
        {
            TypeId = typeId;
            Guid = guid;
        }
        public int Key => TypeId;
        public Guid Value => Guid;
        public int TypeId;
        public Guid Guid;

        public bool Equals(EntityId other)
        {
            return other.TypeId == TypeId && other.Guid == Guid;
        }

        public override int GetHashCode()
        {
            var hashCode = -2124339260;
            hashCode = hashCode * -1521134295 + TypeId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Guid);
            return hashCode;
        }
    }

    public static class ExtEntityLockfreeGetExt
    {
        public static T TryGetLockfree<T>(this IEntitiesRepository repo, Guid id, ReplicationLevel level) where T : class
        {
            var typeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(EntitiesRepository.GetIdByType(typeof(T)));
            return repo.TryGetLockfree<T>(new OuterRef<IEntity>(id, typeId), level);

        }
        public static T TryGetLockfree<T>(this IEntitiesRepository repo, OuterRef<IEntity> entity, ReplicationLevel level) where T : class
        {
            if (entity == default)
                return null;
            var eref = ((IEntitiesRepositoryExtension)repo).GetRef(entity.TypeId, entity.Guid);
            if (eref == null)
                return null;
            var entityobj = ((IEntityRefExt)eref).GetEntity();
            if (!eref.ContainsReplicationLevel(level) || entityobj == null)
                return null;
            return entityobj.GetReplicationLevel(level) as T;
        }
        
        public static bool TryGetLockfree<T>(this IEntitiesRepository repo, OuterRef<IEntity> entityOutRef, ReplicationLevel level, out T entity) where T : class
        {
            entity = null;
            var eref = ((IEntitiesRepositoryExtension)repo).GetRef(entityOutRef.TypeId, entityOutRef.Guid);
            if (eref == null)
                return false;
            var entityobj = ((IEntityRefExt)eref).GetEntity();
            if (!eref.ContainsReplicationLevel(level) || entityobj == null)
                return false;
            if (entityobj.GetReplicationLevel(level) is T castedEntity)
            {
                entity = castedEntity;
                return true;
            }

            return false;
        }
    }
}
