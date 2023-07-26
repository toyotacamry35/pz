using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities.Core;
using SharedCode.EntitySystem;
using SharedCode.Refs;

namespace SharedCode.EntitySystem
{
    public interface IEntity: IDeltaObject
    {
        Guid Id { get; }
        
        EntityMigrationState Migrating { get; }
    }

    public interface IEntityExt
    {
        string CodeVersion { get; }

        EntityState State { get; }

        Guid OwnerNodeId { get; }

        Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>>  ReplicationSets { get; }
        
        FreezableSet ChangedObjects { get; }

        void ClearChangedObjects();
        
        void SetOwnerNodeId(Guid nodeId);

        void SetEntitiesRepository(IEntitiesRepository repository);

        Task CancellAllChainCalls();

        IDictionary<Guid, IEntityMethodsCallsChain> GetChainCalls();

        Task AddChainCall(IEntityMethodsCallsChain chainCall);

        Task<TryRemoveChainCallResult> TryRemoveChainCall(Guid id);

        IEntityMethodsCallsChain GetChainCall(Guid id);

        bool SubscribeReplication(Guid repositoryId, int currentVersion,
            List<(bool subscribe, ReplicationLevel level)> subscriptions, out long uploadMask,
            out long unloadMask, out int sendedVersion, out ReplicationLevel newReplicationLevel,out ReplicationLevel oldReplicationLevel);

        IDictionary<Guid, ReplicateRefsContainer> ReplicateTo();

        IEnumerable<long> GetReplicationMasks();
        
        void AddEntityRef(IEntity addedEntity, ReplicationLevel replicationLevel);

        void RemoveEntityRef(IEntity removedEntity, ReplicationLevel fieldReplicationLevel);

        long CurrentReplicationMask { get; set; }

        int ExecutedMethodsCounterDefault { get; }

        int ExecutedMethodsCounterMigrationId { get; }

        bool IsMaster();

        bool IsMigratingDestination();

        Task FireOnStart();

        Task FireOnDestroy();

        void FireOnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask);

        bool UnsubscribeDisconnectedRepository(Guid repositoryId);

        void CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType operationType);

        long GetDiffUploadMask(Guid repositoryId, ReplicationLevel repLevel, out ReplicationLevel newReplicationLevel,
            out ReplicationLevel oldReplicationLevel);

        void SetState(EntityState state);

        IDeltaObject ResolveDeltaObject(ulong localId);
        
        void AddChangedObject(IDeltaObject deltaObject);

        event Func<Task> OnInitEvent;
        event Func<Task> OnDatabaseLoadEvent;
        event Func<Task> OnStartEvent;
        event Func<Task> OnUnloadEvent;
        event Func<Task> OnDestroyEvent;
        event Action<long, long> OnReplicationLevelChangedEvent;
        MigrationIncrementCounterType IncrementExecutedMethodsCounter(out IEntity __parentEntity__);

        void DecrementExecutedMethodsCounter(MigrationIncrementCounterType counterType);
        ConcurrentDictionary<Guid, ReplicateRefsContainer> ReplicateRepositoryIds { get; }
        void SetMigrating(EntityMigrationState migrating);
        void SetMigratingId(Guid migratingId);
        bool WaitFinishExecutingRpcMethods(MigrationIncrementCounterType migrationCounterType, out Task task);
        Task DispatchDeferredMigratingRpc();
        void UpdateSubscribersOnMigration(Guid toRepositoryId, ReplicationLevel remainedReplicationLevel);
        void CheckMigratingBeforeLink(IEntityRefExt entityRef);
        void CheckMigratingBeforeLinkedEntities(IDeltaObject linkedDeltaObject);
        ValueTask<bool> NeedPutToDeferredRpcQueue();
        bool NeedDeferredRpcOnMigrating();
        ValueTask<T> AddDeferredMigratingRpc<T>(Func<Task<T>> func, string functionName);
        ValueTask AddDeferredMigratingRpc(Func<Task> func, string functionName);
        void ClearMigratedReplicationsContainers();
        void ClearReplicationsContainers();

        void CheckUpdateOwnerNode();
        
        void CreateReplicationSetIfNotExists(ReplicationLevel replicationLevel);
        void AddNewReplicationSets();

        Dictionary<IDeltaObject, DeltaObjectReplicationInfo> TraverseObjects(ReplicationLevel replicationLevel, bool withBsonIgnore);
    }

    public class TryRemoveChainCallResult
    {
        public bool Result { get; set; }

        public IEntityMethodsCallsChain ChainCall { get; set; }

    }
}


    public class FreezableSet : ISet<IDeltaObject>
    {
        private readonly HashSet<IDeltaObject> _objects;
        private bool _freezed;
        private StackTrace _currentStackTrace;
        private StackTrace _createdStackTrace;
 
        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        public FreezableSet(HashSet<IDeltaObject> objects)
        {
            _objects = objects;
        }

        public void Freeze(EntitiesContainer container)
        {
            _currentStackTrace = container.CurrentStackTrace;
            _createdStackTrace = container.CreatedStackTrace;
            _freezed = true;
        }

        public void UnFreeze()
        {
            _freezed = false;
        }
        
        public IEnumerator<IDeltaObject> GetEnumerator()
        {
            return _objects.GetEnumerator();
        }
        
        IEnumerator<IDeltaObject> IEnumerable<IDeltaObject>.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        void ICollection<IDeltaObject>.Add(IDeltaObject item)
        {
            CheckFreeze();
            _objects.Add(item);
        }

        private void CheckFreeze()
        {
            if (_freezed)
            {
                Logger.IfError()
                    ?.Message($"Changed Object was modified when it was freezed Current stack: {new StackTrace(true)}" +
                              $"container CurrentStackTrace stack: {_currentStackTrace} " +
                              $"container CreatedStackTrace stack: {_createdStackTrace}")
                    .Write();
            }
        }

        public void Add(IDeltaObject item)
        {
            CheckFreeze();
            _objects.Add(item);
        }

        public void UnionWith(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<IDeltaObject> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<IDeltaObject>.Add(IDeltaObject item)
        {
            CheckFreeze();
           return _objects.Add(item);
        }

        public void Clear()
        {
            CheckFreeze();
            _objects.Clear();
        }

        public bool Contains(IDeltaObject item)
        {
           return _objects.Contains(item);
        }

        public void CopyTo(IDeltaObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IDeltaObject item)
        {
            CheckFreeze();
            return _objects.Remove(item);
        }

        public int Count
        {
            get => _objects.Count;
        }
        public bool IsReadOnly
        {
            get => false;
        }
    }