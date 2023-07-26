using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Refs.Operations;

namespace SharedCode.Refs
{
    public interface IEntityRef
    {
        Guid Id { get; }

        int TypeId { get; }

        string TypeName { get; }

        long CurrentReplicationMask { get; }

        EntityRefOperationResult? ProcessDeltaSnapshots(IEntitiesRepository repository, bool secondaryUpdate = false);

        void AddDeltaSnapshot(Dictionary<ulong, byte[]> snapshot, List<DeferredEntityModel> deferredEntities, int bytesCount, long replicationMask, int version, int previousVersion);

        void AddDowngrade(long downgradeMask, int version, int previousVersion);

        Task TryAddWaitOperation();

        bool NeedWaitProcessingDeltaSnapshot();

        Dictionary<long, Dictionary<ulong, byte[]>> TakeDeltaSnapshot();

        Type GetEntityInterfaceType();

        void AssertNotRemoteEntity();

        void AddReplicationMask(long replicationMask);

        void RemoveReplicationMask(long replicationMask);

        bool ContainsReplicationLevel(ReplicationLevel mask);
    }

    public interface IEntityRefExt
    {
        EntityState State { get; }
        int Version { get; }
        Task Destroying { get; set; }
        EntityMigrationState Migrating { get; set; }
        void SetVersion(int version);

        IEntity GetEntity();

        void CheckNotNull();

        void SerializeAndSaveNewSubscriber(ReplicationLevel newReplicationLevel, ReplicationLevel oldReplicationLevel, long changedReplicationMask);

        void AfterExclusiveUsing();

        Task CallOnDestroyOrUnload(int typeId, Guid guid, IEntity entity, IEntitiesRepository repo);

        int IncrementVersion();

        Dictionary<ulong, byte[]> GetSerialized(ReplicationLevel replicationLevel,  ReplicationLevel oldReplicationLevel, long changedReplicationMask, bool checkExisting = true);

        void ChangeEntityRef(EntityRefChange change);

        object Locker { get; }

        event Func<int, Guid, IEntity, Task> OnDestroyOrUnload;

        void AddEvents(List<Func<Task>> eventsList);

        bool IsEntityAvailableInEntityGraphOnCurrentRepository(IEntitiesRepository repository);

        int GetOperationCount();
        
        Dictionary<IEntityRef, EntityRefChange> EntityRefChanges { get; set; }
    }

    public readonly struct EntityRefChange
    {
        public EntityRefChange(ReplicationLevel? oldReachabilityReplicationLevel, ReplicationLevel? newReachabilityReplicationLevel, IEntityRef value)
        {
            OldReachabilityReplicationLevel = oldReachabilityReplicationLevel;
            NewReachabilityReplicationLevel = newReachabilityReplicationLevel;
            Value = value;
        }

        public ReplicationLevel? OldReachabilityReplicationLevel { get; }

        public ReplicationLevel? NewReachabilityReplicationLevel { get; }

        public IEntityRef Value { get; }
    }
    
    public readonly struct EntityRefSubscriptionsChange
    {
        public EntityRefSubscriptionsChange(ReplicationLevel? oldReachabilityReplicationLevel, ReplicationLevel? newReachabilityReplicationLevel, IEntityRef value,
            List<RepositorySubscriptionsChanges> repositoriesSubscriptions)
        {
            OldReachabilityReplicationLevel = oldReachabilityReplicationLevel;
            NewReachabilityReplicationLevel = newReachabilityReplicationLevel;
            Value = value;
            RepositoriesSubscriptions = repositoriesSubscriptions;
        }

        public ReplicationLevel? OldReachabilityReplicationLevel { get; }

        public ReplicationLevel? NewReachabilityReplicationLevel { get; }

        public IEntityRef Value { get; }

        public List<RepositorySubscriptionsChanges> RepositoriesSubscriptions { get; }
    }

    public readonly struct RepositorySubscriptionsChanges
    {
        public RepositorySubscriptionsChanges(Guid repositoryId, 
            List<(bool, ReplicationLevel)> subscriptionsChanges, 
            ReplicationLevel replicationMask,
            bool newSubscriber,
            bool unSubscribe)
        {
            RepositoryId = repositoryId;
            SubscriptionsChanges = subscriptionsChanges;
            ReplicationMask = replicationMask;
            NewSubscriber = newSubscriber;
            UnSubscribe = unSubscribe;
        }

        public Guid RepositoryId { get; }
        public List<(bool, ReplicationLevel)> SubscriptionsChanges { get; }
        
        public ReplicationLevel ReplicationMask { get; }
        public bool NewSubscriber { get; }
        public bool UnSubscribe { get; }
    }
}
