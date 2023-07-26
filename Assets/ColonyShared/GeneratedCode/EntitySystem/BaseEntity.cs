using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Newtonsoft.Json;
using NLog;
using ProtoBuf;
using SharedCode.Entities.Core;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem.Delta;
using SharedCode.Network;
using SharedCode.Refs;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;
using SharedCode.Utils;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public abstract partial class BaseEntity : BaseDeltaObject, IEntity, IEntityExt, IRemoteEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        IEntitiesRepository _entitiesRepository;

        [ProtoIgnore]
        [BsonIgnore]
        [JsonIgnore]
        public object _LOCK_FOR_THREADSAFE_USINGFREE_PROPS = new object();
        [ProtoIgnore]
        [BsonIgnore]
        [JsonIgnore]
        private readonly Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> _replicationSets
            = new Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>>();

        [NotNull] //Boris: entity always IN a repo
        [JsonIgnore]
        [ProtoIgnore]
        [BsonIgnore]
        public override IEntitiesRepository EntitiesRepository => _entitiesRepository;


        [ProtoIgnore]
        [BsonIgnore]
        private EntityState _state = EntityState.None;

        [ProtoIgnore]
        [BsonIgnore]
        long IEntityExt.CurrentReplicationMask { get; set; }

        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        public readonly ConcurrentDictionary<ulong, IDeltaObject> LocalIdsToObject = new ConcurrentDictionary<ulong, IDeltaObject>();

        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> IEntityExt.ReplicationSets => _replicationSets;

        bool IEntityExt.IsMaster()
        {
            return !_SerializerContext.Pool.Current.Deserialization && (OwnerNodeId == default || OwnerNodeId == EntitiesRepository.Id || _Migrating == EntityMigrationState.Migrating);
        }

        bool IEntityExt.IsMigratingDestination()
        {
            return _Migrating == EntityMigrationState.MigratingDestination;
        }
        protected override bool containsReplicationLevelInner(long mask)
        {
            return (((IEntityExt)this).CurrentReplicationMask & mask) == mask;
        }


        [ProtoMember(1, DataFormat = DataFormat.WellKnown)]
        [BsonIgnore]
        public Guid Id { get; set; }

        [ProtoIgnore]
        [JsonIgnore]
        [BsonId]
        private Guid MongoID
        {
            get { return Id; }
            set { Id = value; }
        }

        [ProtoIgnore]
        [BsonIgnore]
        [JsonIgnore]
        protected ConcurrentDictionary<Guid, ReplicateRefsContainer> _replicateRepositoryIds = new ConcurrentDictionary<Guid, ReplicateRefsContainer>();

        public bool IsReplicatedTo(Guid repoId) => _replicateRepositoryIds.ContainsKey(repoId);

        [ProtoIgnore]
        [BsonIgnore]
        [JsonIgnore]
        protected INetworkProxy _networkProxy;

        void IRemoteEntity.SetNetworkProxy(INetworkProxy networkProxy)
        {
            _networkProxy = networkProxy;
        }

        INetworkProxy IRemoteEntity.GetNetworkProxy()
        {
            return _networkProxy;
        }

        public bool ShouldSerializeId()
        {
            if (_SerializerContext.Pool.Current.FullSerialize)
                return true;

            return false;
        }



        void IEntityExt.SetOwnerNodeId(Guid nodeId)
        {
            OwnerNodeId = nodeId;
        }
        void IEntityExt.SetEntitiesRepository(IEntitiesRepository repository)
        {
            _entitiesRepository = repository;
        }

        [BsonIgnore]
        [JsonIgnore]
        public override Guid OwnerRepositoryId => OwnerNodeId;

        async Task IEntityExt.CancellAllChainCalls()
        {
            if (ChainCalls.Count == 0)
                return;

            using (var wrapper = await EntitiesRepository.GetMasterService<IChainCallServiceEntityExternalClientFull>())
            {
                if (wrapper == null)
                {
                    Logger.IfError()?.Message("CancellAllChainCalls IChainCallServiceEntityExternal not found wrapper null").Write();
                    return;
                }
                var entity = wrapper.GetMasterService<IChainCallServiceEntityExternalClientFull>();
                if (entity == null)
                {
                    Logger.IfError()?.Message("CancellAllChainCalls IChainCallServiceEntityExternal not found").Write();
                    return;
                }

                var result = await entity.CancelAllChain(this.TypeId, this.Id);
            }
        }

        IDictionary<Guid, Entities.Core.IEntityMethodsCallsChain> IEntityExt.GetChainCalls()
        {
            return ChainCalls;
        }

        async Task IEntityExt.AddChainCall(IEntityMethodsCallsChain chainCall)
        {
            using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(this.TypeId, this.Id))
            {
                var entity = wrapper.Get<IEntity>(this.TypeId, this.Id);
                if (entity == null)
                {
                    Logger.IfError()?.Message("Not found entity {0} {1} to add chaincall", this.TypeName, this.Id).Write();
                    return;
                }
                ChainCalls[chainCall.Id] = chainCall;
            }
        }

        async Task<TryRemoveChainCallResult> IEntityExt.TryRemoveChainCall(Guid id)
        {
            IEntityMethodsCallsChain chainCall;
            if (!ChainCalls.TryGetValue(id, out chainCall))
                return new TryRemoveChainCallResult { Result = false };

            using (var wrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(this.TypeId, this.Id))
            {
                var entity = wrapper.Get<IEntity>(this.TypeId, this.Id);
                if (entity == null)
                {
                    Logger.IfError()?.Message("Not found entity {0} {1} to remove chaincall", this.TypeName, this.Id).Write();
                    return new TryRemoveChainCallResult { Result = true, ChainCall = chainCall };
                }

                ChainCalls.Remove(id);
                if (chainCall.ForkCreatorId != Guid.Empty)
                {
                    IEntityMethodsCallsChain forkCreator;
                    if (ChainCalls.TryGetValue(chainCall.ForkCreatorId, out forkCreator))
                        await forkCreator.ForkFinished(chainCall.Id);
                }

                return new TryRemoveChainCallResult { Result = true, ChainCall = chainCall };
            }
        }

        public IEntityMethodsCallsChain GetChainCall(Guid id)
        {
            IEntityMethodsCallsChain value;
            if (!ChainCalls.TryGetValue(id, out value))
                return null;
            return value;
        }

        public virtual Task Execute(byte remoteMethodId, byte[] remoteMethodParametersDataBytes, IChainContext chainContext, string saveResultKey, Dictionary<int, string> argumentRefs)
        {
            throw new InvalidOperationException("Not expecting method calls");
        }

        public bool SubscribeReplication(Guid repositoryId, int currentVersion,
            List<(bool subscribe, ReplicationLevel level)> subscriptions, out long uploadMask,
            out long unloadMask, out int sendedVersion, out ReplicationLevel newReplicationLevel, out ReplicationLevel oldReplicationLevel)
        {
            if (State != EntityState.Ready && State != EntityState.Destroying && subscriptions.Any(v => v.subscribe))
                throw new Exception(
                    $"Cant subscribe repository {repositoryId} to entity type {TypeName} id {Id} on state {State}");

            uploadMask = 0;
            unloadMask = 0;
            ReplicateRefsContainer refsContainer;
            if (!_replicateRepositoryIds.TryGetValue(repositoryId, out refsContainer))
            {
                lock (_replicateRepositoryIds)
                    if (!_replicateRepositoryIds.TryGetValue(repositoryId, out refsContainer))
                    {
                        refsContainer = new ReplicateRefsContainer();
                        _replicateRepositoryIds[repositoryId] = refsContainer;
                    }
            }

            var oldReplicationMask = refsContainer.GetReplicationMask();
            oldReplicationLevel = (ReplicationLevel)oldReplicationMask;
            sendedVersion = refsContainer.SendedVersion;

            bool result = refsContainer.Change(subscriptions);
            if (result)
            {
                if (refsContainer.IsEmpty())
                {
                    ReplicateRefsContainer toRemove;
                    _replicateRepositoryIds.TryRemove(repositoryId, out toRemove);
                    unloadMask = long.MaxValue;
                }
                else
                {
                    var newReplicationMask = refsContainer.GetReplicationMask();
                    uploadMask = (oldReplicationMask ^ newReplicationMask) & newReplicationMask;
                    unloadMask = (oldReplicationMask ^ newReplicationMask) & oldReplicationMask;
                }

                ((IEntityExt)this).AddNewReplicationSets();
            }

            if (uploadMask > 0 || unloadMask > 0)
                refsContainer.SetSendedVersion(currentVersion);

            newReplicationLevel = (ReplicationLevel)refsContainer.GetReplicationMask();

            return result;
        }

        long IEntityExt.GetDiffUploadMask(Guid repositoryId, ReplicationLevel repLevel, out ReplicationLevel newReplicationLevel, out ReplicationLevel oldReplicationLevel)
        {
            newReplicationLevel = repLevel;
            oldReplicationLevel = ReplicationLevel.None;
            ReplicateRefsContainer refsContainer;
            if (!_replicateRepositoryIds.TryGetValue(repositoryId, out refsContainer))
                return (long)repLevel;

            if (refsContainer.IsEmpty())
                return (long)repLevel;

            var oldReplicationMask = refsContainer.GetReplicationMask();
            oldReplicationLevel = (ReplicationLevel)oldReplicationMask;
            var newReplicationMask = refsContainer.GetChangedRecalculateReplicationMask(true, repLevel);
            var uploadMask = (oldReplicationMask ^ newReplicationMask) & newReplicationMask;
            newReplicationLevel = (ReplicationLevel)newReplicationMask;
            return uploadMask;
        }

        IDictionary<Guid, ReplicateRefsContainer> IEntityExt.ReplicateTo()
        {
            return _replicateRepositoryIds;
        }

        bool IEntityExt.UnsubscribeDisconnectedRepository(Guid repositoryId)
        {
            ReplicateRefsContainer toRemove;
            var result = _replicateRepositoryIds.TryRemove(repositoryId, out toRemove);
            return result;
        }

        public IEnumerable<long> GetReplicationMasks()
        {
            return _replicateRepositoryIds.Select(x => x.Value.GetReplicationMask()).Distinct();
        }

        public void AddEntityRef(IEntity addedEntity, ReplicationLevel entityRefReachabilityLevel)
        {
            var thisEntityRef = (IEntityRefExt)((IEntitiesRepositoryExtension) EntitiesRepository).GetRef(TypeId, Id);
            var addedEntityRef = ((IEntitiesRepositoryExtension) EntitiesRepository).GetRef(addedEntity.TypeId, addedEntity.Id);
            thisEntityRef.ChangeEntityRef(new EntityRefChange(null, entityRefReachabilityLevel, addedEntityRef));
        }

        public void RemoveEntityRef(IEntity removedEntity, ReplicationLevel entityRefReachabilityLevel)
        {
            var thisEntityRef = (IEntityRefExt)((IEntitiesRepositoryExtension) EntitiesRepository).GetRef(TypeId, Id);
            var removedEntityRef = ((IEntitiesRepositoryExtension) EntitiesRepository).GetRef(removedEntity.TypeId, removedEntity.Id);
            thisEntityRef.ChangeEntityRef(new EntityRefChange(entityRefReachabilityLevel, null, removedEntityRef));
        }
        
        protected override void RandomFill(int depthCount, Random random, bool withReadonly)
        {
            base.RandomFill(depthCount, random, withReadonly);
        }

        public override string ToString()
        {
            return string.Format("<Entity {0} Id:{1} OwnerNodeId:{2}>", GetType().Name, Id, (Guid)_OwnerNodeId);
        }

        public async Task RunChainCallsAfterLoad()
        {
            if (ChainCalls.Count == 0)
                return;

            using (var wrapper = await EntitiesRepository.GetMasterService<IChainCallServiceEntityInternal>())
            {
                var chainCallService = wrapper?.GetMasterService<IChainCallServiceEntityInternal>();
                if (chainCallService == null)
                {
                    Logger.IfError()?.Message("IChainCallServiceEntityInternal not found for execute chain calls after load. Entity {0} {1}", TypeName, Id).Write();
                    return;
                }

                await chainCallService.AddExistingChainCalls(ChainCalls.Values.ToList(), TypeId, Id);
            }
        }

        public void AddChangedObject(IDeltaObject deltaObject)
        {
            if (ChangedObjects == null)
            {
                ChangedObjects = new FreezableSet(new HashSet<IDeltaObject>());
            }
            
            ChangedObjects.Add(deltaObject);
        }

        public event Func<Task> OnInitEvent;
        public event Func<Task> OnDatabaseLoadEvent;
        public event Func<Task> OnStartEvent;
        public event Action<long, long> OnReplicationLevelChangedEvent;
        public event Func<Task> OnUnloadEvent;
        public event Func<Task> OnDestroyEvent;

        public Task FireOnInit()
        {
            var t = OnInitEvent.InvokeAsync();
            OnInitEvent = null;
            return t;
        }
        public virtual async Task FireOnDatabaseLoad()
        {
            await OnDatabaseLoadEvent.InvokeAsync();
            OnDatabaseLoadEvent = null;
            await RunChainCallsAfterLoad();
        }
        public Task FireOnStart()
        {
            var t = OnStartEvent.InvokeAsync();
            OnStartEvent = null;
            return t;
        }
        public Task FireOnDestroy() => OnDestroyEvent.InvokeAsync();
        public void FireOnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask) => AsyncUtils.RunAsyncTask(() => OnReplicationLevelChangedEvent?.Invoke(oldReplicationMask, newReplicationMask));
        public Task FireOnUnload() => OnUnloadEvent.InvokeAsync();

        public static bool IsReplicationLevelAdded(long oldReplicationMask, long newReplicationMask, ReplicationLevel checkLevel)
        {
            return ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, checkLevel);
        }

        public static bool IsReplicationLevelRemoved(long oldReplicationMask, long newReplicationMask, ReplicationLevel checkLevel)
        {
            return ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, checkLevel);
        }

        void IEntityExt.CheckValidateEntityInAsyncContext(ReadWriteEntityOperationType operationType)
        {
            if (_state == EntityState.None || _state == EntityState.Initializing || _state == EntityState.InitializingAfterLoading)
                return;

            if (operationType == ReadWriteEntityOperationType.Read && _state == EntityState.Destroyed)
                return;

            AsyncStackHolder.CheckValidateEntityInAsyncContext(TypeId, Id, operationType);
        }

        IDeltaObject IEntityExt.ResolveDeltaObject(ulong localId)
        {
            LocalIdsToObject.TryGetValue(localId, out var obj);
            return obj;
        }

        void IEntityExt.AddNewReplicationSets()
        {
            var newReplicationLevels = new HashSet<ReplicationLevel>();
            foreach (var replicateRepository in _replicateRepositoryIds)
            {
                // take max mask with which repository is subscribed to this entity
                var replicationMask = (ReplicationLevel)replicateRepository.Value.GetReplicationMask();
                if (!_replicationSets.ContainsKey(replicationMask))
                {
                    _replicationSets[replicationMask] = new Dictionary<IDeltaObject, DeltaObjectReplicationInfo>();
                    newReplicationLevels.Add(replicationMask);
                }
            }

            if (newReplicationLevels.Count > 0)
            {
                FillReplicationSetRecursive(_replicationSets, newReplicationLevels, ReplicationLevel.Always, true);
            }
        }

        void IEntityExt.CreateReplicationSetIfNotExists(ReplicationLevel replicationLevel)
        {
            if (!_replicationSets.ContainsKey(replicationLevel))
            {
                _replicationSets[replicationLevel] = new Dictionary<IDeltaObject, DeltaObjectReplicationInfo>();
                FillReplicationSetRecursive(_replicationSets, new HashSet<ReplicationLevel>
                {
                    replicationLevel
                }, ReplicationLevel.Always, true);
            }
        }

        Dictionary<IDeltaObject, DeltaObjectReplicationInfo> IEntityExt.TraverseObjects(ReplicationLevel replicationLevel, bool withBsonIgnore)
        {
            var replicationSet = new Dictionary<IDeltaObject, DeltaObjectReplicationInfo>();
            var container = new Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>>
            {
                [replicationLevel] = replicationSet
            };

            FillReplicationSetRecursive(container, new HashSet<ReplicationLevel>
            {
                replicationLevel
            }, ReplicationLevel.Always, withBsonIgnore);

            return replicationSet;
        }

        public void SetState(EntityState state)
        {
            _state = state;
        }

        [ProtoIgnore]
        [BsonIgnore]
        [JsonIgnore]
        public FreezableSet ChangedObjects { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        public EntityState State => _state;

        public abstract string CodeVersion { get; }
    }

    public readonly struct DeltaObjectReplicationInfo
    {
        public DeltaObjectReplicationInfo(int referenceCount, bool newObject)
        {
            ReferenceCount = referenceCount;
            NewObject = newObject;
        }

        public int ReferenceCount { get; }

        public bool NewObject { get; }
    }

    public enum EntityState
    {
        None,
        Created,
        Initializing,
        InitializingAfterLoading,
        Ready,
        Destroying,
        Destroyed
    }

    public enum EntityMigrationState
    {
        None,
        GiveRpcMigratingId,
        Migrating,
        MigratingDestination
    }
}
