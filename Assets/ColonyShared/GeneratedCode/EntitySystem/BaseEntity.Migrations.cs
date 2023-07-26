using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using GeneratedCode.Manual.Repositories;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.Entities.Cloud;
using SharedCode.Refs;
using SharedCode.Serializers;

namespace SharedCode.EntitySystem
{
    public abstract partial class BaseEntity
    {
        protected ConcurrentQueue<Func<TaskCompletionSource<bool>, Task>> _deferredMigratingRpc;

        private List<TaskCompletionSource<bool>> _waitingTaskCompletionSourcesDefault;

        private List<TaskCompletionSource<bool>> _waitingTaskCompletionSourcesMigrationId;

        private object _migrationWaitRpcLocker = new object();

        private bool _needUpdateOwnerNode;

        private EntityMigrationState _Migrating;

        [BsonIgnore]
        public EntityMigrationState Migrating => _Migrating;

        private Guid _migratingId;

        [BsonIgnore]
        public override ref Guid MigratingId => ref _migratingId;

        int _executedMethodsCounterDefault;
        
        int _executedMethodsCounterMigrationId;

        [BsonIgnore]
        public int ExecutedMethodsCounterDefault => _executedMethodsCounterDefault;

        [BsonIgnore]
        public int ExecutedMethodsCounterMigrationId => _executedMethodsCounterMigrationId;

        public override MigrationIncrementCounterType IncrementExecutedMethodsCounter(out IEntity parentEntityToDecrement)
        {
            parentEntityToDecrement = this;

            if (MigratingId == Guid.Empty)
            {
                Interlocked.Increment(ref _executedMethodsCounterDefault);
                return MigrationIncrementCounterType.WithoutMigrationId;
            }
            else
            {
                Interlocked.Increment(ref _executedMethodsCounterMigrationId);
                return MigrationIncrementCounterType.HaveMigrationId;

            }
        }

        public void DecrementExecutedMethodsCounter(MigrationIncrementCounterType migrationCounterType)
        {
            if (migrationCounterType == MigrationIncrementCounterType.WithoutMigrationId)
            {
                Interlocked.Decrement(ref _executedMethodsCounterDefault);
                if (_executedMethodsCounterDefault == 0 && _waitingTaskCompletionSourcesDefault != null)
                    lock (_migrationWaitRpcLocker)
                    {
                        if (_executedMethodsCounterDefault == 0 && _waitingTaskCompletionSourcesDefault != null)
                        {
                            foreach (var tcs in _waitingTaskCompletionSourcesDefault)
                                tcs.SetResult(true);
                            _waitingTaskCompletionSourcesDefault.Clear();
                            _waitingTaskCompletionSourcesDefault = null;
                        }
                    }
            }
            else if (migrationCounterType == MigrationIncrementCounterType.HaveMigrationId)
            {
                Interlocked.Decrement(ref _executedMethodsCounterMigrationId);
                if (_executedMethodsCounterMigrationId == 0 && _waitingTaskCompletionSourcesMigrationId != null)
                    lock (_migrationWaitRpcLocker)
                    {
                        if (_executedMethodsCounterMigrationId == 0 && _waitingTaskCompletionSourcesMigrationId != null)
                        {
                            foreach (var tcs in _waitingTaskCompletionSourcesMigrationId)
                                tcs.SetResult(true);
                            _waitingTaskCompletionSourcesMigrationId.Clear();
                            _waitingTaskCompletionSourcesMigrationId = null;
                        }
                    }
            }
        }

        public void SetMigrating(EntityMigrationState migrating)
        {
            _Migrating = migrating;
        }

        public void SetMigratingId(Guid migratingId)
        {
            _migratingId = migratingId;
        }

        protected override Guid GetActualMigratingId()
        {
            return MigrationIdHolder.CurrentMigrationId != Guid.Empty
                ? MigrationIdHolder.CurrentMigrationId
                : MigratingId;
        }

        bool IEntityExt.WaitFinishExecutingRpcMethods(MigrationIncrementCounterType migrationCounterType, out Task task)
        {
            task = null;

            lock (_migrationWaitRpcLocker)
            {
                if (migrationCounterType == MigrationIncrementCounterType.WithoutMigrationId)
                {
                    if (_executedMethodsCounterDefault == 0)
                        return false;

                    var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    if (_waitingTaskCompletionSourcesDefault == null)
                        _waitingTaskCompletionSourcesDefault = new List<TaskCompletionSource<bool>>();

                    _waitingTaskCompletionSourcesDefault.Add(tcs);

                    task = tcs.Task;
                    return true;
                }

                if (migrationCounterType == MigrationIncrementCounterType.HaveMigrationId)
                {

                    if (_executedMethodsCounterMigrationId == 0)
                        return false;

                    var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                    if (_waitingTaskCompletionSourcesMigrationId == null)
                        _waitingTaskCompletionSourcesMigrationId = new List<TaskCompletionSource<bool>>();

                    _waitingTaskCompletionSourcesMigrationId.Add(tcs);
                    task = tcs.Task;

                    return true;
                }
            }

            Logger.IfError()?.Message("WaitFinishExecutingRpcMethods incorrect counterType {0}", migrationCounterType).Write();
            return false;
        }

        void IEntityExt.CheckMigratingBeforeLink(IEntityRefExt entityRef)
        {
            if (entityRef.Migrating != EntityMigrationState.None)
            {
                Logger.IfError()?.Message("Entity typeId {0} id {1} cant link migrated entity typeId {2} id {3}", TypeName, Id, ((IEntityRef)entityRef)?.TypeName ?? "none", ((IEntityRef)entityRef)?.Id.ToString() ?? "none").Write();
                throw new Exception(string.Format("Entity typeId {0} id {1} cant link migrated entity typeId {2} id {3}", TypeName, Id, ((IEntityRef)entityRef)?.TypeName ?? "none", ((IEntityRef)entityRef)?.Id.ToString() ?? "none"));
            }

            if (Migrating != EntityMigrationState.None)
                entityRef.Migrating = Migrating;
        }

        void IEntityExt.CheckMigratingBeforeLinkedEntities(IDeltaObject linkedDeltaObject)
        {
            var linkedEntities = new List<(long level, IEntityRef entityRef)>();
            linkedDeltaObject.GetAllLinkedEntities((long) ReplicationLevel.Master, linkedEntities, 0, false);
            foreach (var pair in linkedEntities)
            {
                var entityRef = pair.Item2;
                if (((IEntityRefExt)entityRef).Migrating != EntityMigrationState.None)
                {
                    Logger.IfError()?.Message("Entity typeId {0} id {1} cant link migrated entity typeId {2} id {3}", TypeName, Id, entityRef?.TypeName.ToString() ?? "none", entityRef?.Id.ToString() ?? "none").Write();
                    throw new Exception(string.Format("Entity typeId {0} id {1} cant link migrated entity typeId {2} id {3}", TypeName, Id, entityRef?.TypeName.ToString() ?? "none", entityRef?.Id.ToString() ?? "none"));
                }

                if (Migrating != EntityMigrationState.None)
                    ((IEntityRefExt)entityRef).Migrating = Migrating;
            }
        }

        public void UpdateSubscribersOnMigration(Guid toRepositoryId, ReplicationLevel remainedReplicationLevel)
        {
            ReplicateRefsContainer toRemove;
            if (!_replicateRepositoryIds.TryGetValue(toRepositoryId, out toRemove))
            {
                lock (_replicateRepositoryIds)
                    if (!_replicateRepositoryIds.TryGetValue(toRepositoryId, out toRemove))
                    {
                        Logger.IfError()?.Message("UpdateSubscribersOnMigration {0} {1} not found destination repository ReplicateRefsContainer {2}, added default", TypeName, Id, toRepositoryId).Write();
                        toRemove = new ReplicateRefsContainer();
                        _replicateRepositoryIds[toRepositoryId] = toRemove;
                        toRemove.Change(new List<(bool subscribed, ReplicationLevel level)> { (true, ReplicationLevel.Master) });
                    }
            }

            toRemove.MaskAsMigrated();

            ReplicateRefsContainer toAdd;
            if (!_replicateRepositoryIds.TryGetValue(EntitiesRepository.Id, out toAdd))
            {
                lock (_replicateRepositoryIds)
                    if (!_replicateRepositoryIds.TryGetValue(EntitiesRepository.Id, out toAdd))
                    {
                        toAdd = new ReplicateRefsContainer();
                        _replicateRepositoryIds[EntitiesRepository.Id] = toAdd;
                    }
            }
            toAdd.Change(new List<(bool subscribed, ReplicationLevel level)> { (true, remainedReplicationLevel) });
            ReplicateRepositoryIds = _replicateRepositoryIds;//SetDirty
            ((IEntityExt)this).AddNewReplicationSets();
        }

        public void ClearMigratedReplicationsContainers()
        {
            _replicateRepositoryIds.RemoveAll((k , v) => v.MigratedContainer);
        }

        public void ClearReplicationsContainers()
        {
            _replicateRepositoryIds.Clear();
        }
        void IEntityExt.CheckUpdateOwnerNode()
        {
            if (!_needUpdateOwnerNode)
                return;

            _needUpdateOwnerNode = false;

            if (EntitiesRepository == null)
                return;

            ((IEntitiesRepositoryExtension)this.EntitiesRepository).CheckRemoteEntity(this);
        }

        private TaskCompletionSource<bool> _dispatchingMigrationRpc;

        public async Task DispatchDeferredMigratingRpc()
        {
            if (_deferredMigratingRpc == null)
                return;

            _dispatchingMigrationRpc = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task previousTask = null;
            Func<TaskCompletionSource<bool>, Task> func;
            while (_deferredMigratingRpc.TryDequeue(out func))
            {
                if (previousTask != null)
                    await previousTask;
                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                previousTask = tcs.Task;
                var funcCopy = func;
                _ = AsyncUtils.RunAsyncTask(() => funcCopy(tcs));
            }
            if (previousTask != null)
                await previousTask;
            _deferredMigratingRpc = null;
            _dispatchingMigrationRpc.SetResult(true);
            _dispatchingMigrationRpc = null;
        }

        ValueTask<bool> IEntityExt.NeedPutToDeferredRpcQueue()
        {
            //Если прямо сейчас обрабатываются отложенные RPC после телепорта, то ждем пока они обработаются
            if (_dispatchingMigrationRpc != null)
                return NeedPutToDeferredRpcQueueIfMigrating();

            return new ValueTask<bool>(NeedPutToDeferredRpcQueueInternal());
        }

        private async ValueTask<bool> NeedPutToDeferredRpcQueueIfMigrating()
        {
            var dispatchingMigrationRpcCopy = _dispatchingMigrationRpc;
            var task = dispatchingMigrationRpcCopy?.Task ?? Task.CompletedTask;
            await task;

            return NeedPutToDeferredRpcQueueInternal();
        }

        private bool NeedPutToDeferredRpcQueueInternal()
        {
            //если энтити никуда не мигрирует и в очереди нет отложенных вызовов то не добавляем
            if (Migrating != EntityMigrationState.Migrating && _deferredMigratingRpc == null)
                return false;

            //проверяем что это входящее сообщение с таким же Id миграции - значит закольцованный вызов и надо обработать сейчас а не откладывать
            if (MigratingId == MigrationIdHolder.CurrentMigrationId)
                return false;

            return true;
        }

        public bool NeedDeferredRpcOnMigrating()
        {
            return Migrating == EntityMigrationState.Migrating || Migrating == EntityMigrationState.MigratingDestination || _deferredMigratingRpc != null;
        }

        ValueTask<T> IEntityExt.AddDeferredMigratingRpc<T>(Func<Task<T>> func, string functionName)
        {
            if (_deferredMigratingRpc == null)
                lock (_migrationWaitRpcLocker)
                    if (_deferredMigratingRpc == null)
                        _deferredMigratingRpc = new System.Collections.Concurrent.ConcurrentQueue<Func<TaskCompletionSource<bool>, Task>>();

            return RpcHelper.DeferredMigratingRpc(func, _deferredMigratingRpc, ServerCoreRuntimeParameters.WaitFinishExecutingRpcMethodsTimeoutSeconds, functionName, TypeName, Id);
        }

        ValueTask IEntityExt.AddDeferredMigratingRpc(Func<Task> func, string functionName)
        {
            if (_deferredMigratingRpc == null)
                lock (_migrationWaitRpcLocker)
                    if (_deferredMigratingRpc == null)
                        _deferredMigratingRpc = new System.Collections.Concurrent.ConcurrentQueue<Func<TaskCompletionSource<bool>, Task>>();

            return RpcHelper.DeferredMigratingRpc(func, _deferredMigratingRpc, ServerCoreRuntimeParameters.WaitFinishExecutingRpcMethodsTimeoutSeconds, functionName, TypeName, Id);
        }
    }

    public enum MigrationIncrementCounterType
    {
        None,
        WithoutMigrationId,
        HaveMigrationId
    }
}
