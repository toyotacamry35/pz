using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Manual.Repositories;
using NLog;
using ResourceSystem.Utils;
using SharedCode.ActorServices;
using SharedCode.Cloud;
using SharedCode.Entities.Cloud;
using SharedCode.Network;
using SharedCode.Refs;
using SharedCode.Serializers;

namespace SharedCode.EntitySystem
{
    public struct EntityDesc
    {
        public Guid Guid;
        public int TypeId;

        public EntityDesc(Guid guid, int typeId)
        {
            Guid = guid;
            TypeId = typeId;
        }

        public bool IsValid => Guid != Guid.Empty;
        public static EntityDesc Invalid = new EntityDesc(Guid.Empty, 0);
    }
   

    public delegate Task UserDisconnectedDelegate(Guid repoId);
    public interface IEntitiesRepository
    {
        Guid Id { get; }
        ValueTask<IEntitiesContainer> Get(IEntityBatch batch);
        ValueTask<IEntitiesContainer> Get(IEntityBatch requestedBatch, object tag);
        ValueTask<IEntitiesContainer> Get<T>(Guid entityId, [CallerMemberName] string callerTag = "") where T: IEntity;
        ValueTask<IEntitiesContainer> Get<T>(Guid entityId, object tag, [CallerMemberName] string callerTag = "") where T : IEntity;
        ValueTask<IEntitiesContainer> Get(int typeId, Guid entityId, [CallerMemberName] string callerTag = "");
        ValueTask<IEntitiesContainer> Get<T>(OuterRef<T> entOuterRef, [CallerMemberName] string callerTag = "");
        ValueTask<IEntitiesContainer> Get(int typeId, Guid entityId, object tag, [CallerMemberName] string callerTag = "");
        ValueTask<IEntitiesContainer> Get(Type type, Guid entityId, [CallerMemberName] string callerTag = null);
        ValueTask<IEntitiesContainer> Get(Type type, Guid entityId, object tag, [CallerMemberName] string callerTag = null);
        ValueTask<IEntitiesContainer> GetMasterService<T>([CallerMemberName] string callerTag = "") where T : IEntity;
        ValueTask<IEntitiesContainer> GetMasterService(Type type, [CallerMemberName] string callerTag = "");
        ValueTask<IEntitiesContainer> GetMasterService(Type type, object tag, [CallerMemberName] string callerTag = "");
        ValueTask<IEntitiesContainer> GetMasterService<T>(object tag, [CallerMemberName] string callerTag = "") where T : IEntity;
        ValueTask<IEntitiesContainer> GetFirstService<T>([CallerMemberName] string callerTag = "") where T : IEntity;
        ValueTask<IEntitiesContainer> GetFirstService(Type type, [CallerMemberName] string callerTag = "");
        ValueTask<IEntitiesContainer> GetFirstService<T>(object tag, [CallerMemberName] string callerTag = "") where T : IEntity;
        Task<EntityRef<T>> Load<T>(Guid entityId, Func<T, Task> initializeAction = null) where T : IEntity;
        Task<IEntityRef> Load(int typeId, Guid entityId, Func<IEntity, Task> initializeAction = null);

        Task<Guid> GetAddressResolverServiceEntityId(int typeId, Guid entityId);
        ValueTask<IEntitiesContainer> GetInternal(IEntityBatch batch, object callerTag, bool checkExclusiveOnReplication);

        Task<EntityRef<T>> Create<T>(Guid entityId, Func<T, Task> initializeAction = null) where T : IEntity;
        Task<IEntityRef> Create(int typeId, Guid entityId, Func<IEntity, Task> initializeAction = null);

        Task<bool> Destroy<T>(Guid entityId, bool unload = false) where T : IEntity;
        Task<bool> Destroy(int typeId, Guid entityId, bool unload = false);
        CancellationToken StopToken { get; }
        Task SubscribeReplication(int typeId, Guid entityId, Guid repositoryId, ReplicationLevel replicationLevel, [CallerMemberName] string callerTag = null);
        Task UnsubscribeReplication(int typeId, Guid entityId, Guid repositoryId, ReplicationLevel replicationLevel);
        Task UnsubscribeReplicationBatch(int typeId, Guid entityId, List<KeyValuePair<Guid, ReplicationLevel>> subscriptions);

        event Func<int, Guid, Task> NewEntityUploaded;
        event Func<int, Guid, Task> EntityUpdated;
        event Func<int, Guid, bool, IEntity, Task> EntityUnloaded;
        event Func<int, Guid, Task> EntityCreated;
        event Func<int, Guid, Task> EntityLoaded;
        event Func<int, Guid, IEntity, Task> EntityDestroy;
        event UserDisconnectedDelegate UserDisconnected;
        event Func<Task> CloudRequirementsMet;

        bool SubscribeOnDestroyOrUnload(int typeId, Guid entityId, Func<int, Guid, IEntity, Task> callback);
        bool SubscribeOnDestroyOrUnload(IEnumerable<EntityDesc> entities, bool allowPartial, Func<int, Guid, IEntity, Task> callback);
        bool UnsubscribeOnDestroyOrUnload(int typeId, Guid entityId, Func<int, Guid, IEntity, Task> callback);

        CloudNodeType CloudNodeType { get; }
    }
    public class UnlockTimeoutBlocker : IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<object, bool> _blockTimeoutRequests;
        private readonly Guid _id;

        private readonly object _obj = new StackTrace();

        public UnlockTimeoutBlocker(ConcurrentDictionary<object, bool> blockTimeoutRequests, Guid id)
        {
            _id = id;
            _blockTimeoutRequests = blockTimeoutRequests;

            Logger.IfDebug()?.Message("Repository {0} unlock timeout enabled", id).Write();
            _blockTimeoutRequests.TryAdd(_obj, false);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Logger.IfDebug()?.Message("Repository {0} unlock timeout disabled", _id).Write();

                bool nothing;
                _blockTimeoutRequests.TryRemove(_obj, out nothing);

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }


    public interface IEntitiesRepositoryExtension
    {
        IEntitySerializer EntitySerializer { get; }
        ISerializer Serializer { get; }
        ValueTask<IEntitiesContainer> GetExclusive<T>(Guid entityId, object tag, [CallerMemberName] string callerTag = null) where T : IEntity;
        ValueTask<IEntitiesContainer> GetExclusive<T>(Guid entityId, [CallerMemberName] string callerTag = null) where T : IEntity;
        ValueTask<IEntitiesContainer> GetExclusive(int typeId, Guid entityId, object tag, [CallerMemberName] string callerTag = null);
        ValueTask<IEntitiesContainer> GetExclusive(int typeId, Guid entityId, [CallerMemberName] string callerTag = null);
        IEntityRef GetRef<T>(Guid id) where T : IEntity;
        IEntityRef GetRef(int typeId, Guid id);
        IEntityRef GetRef(Type type, Guid id);
        IEntityRef GetServiceRef<T>();
        IEntityRef CheckAndGetSubscriberRepositoryCommunicationRef(Guid subscriberRepositoryId, int subscribedEntityTypeId, Guid subscribedEntityId);
        UnlockTimeoutBlocker DisableRepositoryEntityUnlockTimeout();
        string GetEntityStatus(int typeId, Guid entityId);
        void VisitEntityQueueLengths(QueueLengthVisitor visitor);
        void VisitEntityCounts(CountVisitor visitor);
        string GetAllServiceEntitiesOperationLog();
        string RepositoryConfigId { get; }
        int RepositoryNum { get; }
        bool SuppressEntityInitialization { get; }
        bool IsEntityTypeAvailableInEntityGraph(Type type);
        Task ConnectExternal(string host, int port, CancellationToken ct);
        void DisconnectExternal(Guid remoteRepo);
        bool IsTimeoutBlocked();
        RepositoryEntityContainsStatus GetRepositoryEntityContainsStatus(int typeId, Guid entityId);
        bool CheckRemoteEntity(IEntity entity);
        Task<MigrateEntityResult> MigrateEntity(int typeId, Guid entityId, Guid toRepositoryId, ReplicationLevel remainedReplicationLevel);
        bool IsMigratingOrNeedRedirecting(IEntity entity, byte[] data, Guid callback);
        Task<StartMigrateEntityResult> StartMigrateEntity(int entityTypeId, Guid entityId, Guid fromRepositoryId);
        Task<FinishMigrateEntityResult> FinishMigrateEntity(int entityTypeId, Guid entityId, Dictionary<ValueTuple<int, Guid>, Dictionary<Guid, int>> replicateRefsVersions);
        Task DispatchMigratedEntityDeferredRpc(int entityTypeId, Guid entityId);

        void UpdateSubscriptions(
            int typeId,
            Guid entityId,
            Guid repositoryId,
            List<(bool subscribed, ReplicationLevel level)> subscriptions,
            Dictionary<Guid, List<DeferredEntityModel>> newLinkedEntities,
            ref EntityCollections collections);
    }

    public delegate void CountVisitor(Type entityType, int count);
    public delegate void QueueLengthVisitor(Type entityType, Guid id, int count);

    public enum RepositoryEntityContainsStatus
    {
        None,
        NotContains,
        Replication,
        Master
    }

    public enum MigrateEntityResult
    {
        None,
        Success,
        ErrorRepositoryNotFound,
        ErrorDestinationRepositoryNotFoundOnFinish,
        ErrorEntityNotFound,
        ErrorCantMigrateToClientRepository,
        ErrorAlreadyMigrating,
        ErrorDestinationRepositorySubscription,
        ErrorStartMigrate,
        ErrorDestinationRepositoryErrorOnFinish,
        ErrorUnknown,
    }

    public enum FinishMigrateEntityResult
    {
        None,
        Success,
        ErrorEntityNotFound,
        ErrorUnknown,
    }
}
