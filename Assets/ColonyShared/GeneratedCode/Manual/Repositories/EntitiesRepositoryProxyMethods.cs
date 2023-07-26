using GeneratedCode.Manual.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Refs;
using SharedCode.Repositories;
using SharedCode.Serializers;
using System.Collections.Concurrent;
using System.Linq;
using GeneratedCode.EntitySystem.Statistics;
using Newtonsoft.Json;
using GeneratedCode.Network.Statistic;
using SharedCode.Config;
using SharedCode.Cloud;
using System.Collections.Immutable;
using Core.Environment.Logging.Extension;
using NLog;
using ResourceSystem.Utils;
using SharedCode.Monitoring;

namespace GeneratedCode.Repositories
{
    public partial class EntitiesRepository
    {
        protected readonly NLog.Logger SubscriptionsLogger;
        
        public Guid Id { get; }

        [JsonProperty]
        protected readonly ConcurrentDictionary<Type, RepositoryEntityTypeStates> _typeStates = new ConcurrentDictionary<Type, RepositoryEntityTypeStates>();
        [JsonProperty]
        protected ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IEntityRef>> _typeToCollection { get; } = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IEntityRef>>();

        [Newtonsoft.Json.JsonIgnore]
        private readonly ISerializer _serializer;
        
        [Newtonsoft.Json.JsonIgnore]
        protected IEntitySerializer _entitySerializer;

        private readonly IEntitySubscriptionsProcessor _entitySubscriptionsProcessor;
        private readonly SubscriptionChangesStatistics _subscriptionsChangesStatistics;

        public EntitiesRepository(CloudSharedDataConfig sharedDataConfig, EntitiesRepositoryConfig repositoryConfig,
            CloudNodeType cloudNodeType, int num, ISerializer serializer, IEntitySerializer entitySerializer,
            IEntitySubscriptionsProcessor entitySubscriptionsProcessor)
        {
            _cloudNodeType = cloudNodeType;
            RepositoryNum = num;
            _config = repositoryConfig;
            _sharedConfig = sharedDataConfig;

            _stopTokenSource = new System.Threading.CancellationTokenSource();
            StopToken = _stopTokenSource.Token;
            Id = Guid.NewGuid();
            SubscriptionsLogger = LogManager.GetLogger("EntitySubscriptionsHistory");
            _entitySerializer = entitySerializer;
            _entitySubscriptionsProcessor = entitySubscriptionsProcessor;
            _serializer = serializer;

            LockRepositoryStatistics = Statistics<LockRepositoryStatisticsManager>.Instance.GetOrAdd(this.Id, PrometheusLockRepositoryStatisticsFactory.It.GetOrAdd(Id));
            EntityCountStatistics = Statistics<EntityCountStatisticsManager>.Instance.GetOrAdd(this, new EntityCountStatisticsPrometheus());
            EntityQueueSizeStatistics = Statistics<EntityQueueSizeStatisticsManager>.Instance.GetOrAdd(this, new EntityQueueSizeStatisticsPrometheus());
            _subscriptionsChangesStatistics = new SubscriptionChangesStatistics(Id);

            StartSubscriptionsProcessing();
            StartDetectingFreezedUpdates();
        }

        protected IEntity CreateImpl(int typeId, Guid id)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            return ReplicaTypeRegistry.CreateImpl(type, id);
        }
        
        protected RepositoryEntityTypeStates GetTypeState(int typeId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            return _typeStates.GetOrAdd(type, (key) => new RepositoryEntityTypeStates());
        }
        
        public void TryAddDeferredEntities(List<DeferredEntityModel> deferredEntities)
        {
            if (deferredEntities == null)
                return;
            
            foreach (var deferredEntity in deferredEntities)
            {
                if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                {
                    SubscriptionsLogger.IfInfo()?.Message("Added deferredEntity entityTypeId={entityTypeId} entityId={entityId}", deferredEntity.Entity.TypeId, deferredEntity.Entity.Guid).Write();
                }
                
                var typeState = GetTypeState(deferredEntity.Entity.TypeId);
                var collection = GetEntitiesCollection(ReplicaTypeRegistry.GetTypeById(deferredEntity.Entity.TypeId));
                // it is possible that entity arrived before then deferred info
                // есть вероятность, что ентити есть в коллекции на момент проверки,
                // но будет вызван unsubscribe и будет удалена ентити, после чего using не найдет entity и deferred,
                // хотя будет ентити с валидным рефом   
                if (!collection.TryGetValue(deferredEntity.Entity.Guid, out var existedRef) 
                    || !existedRef.ContainsReplicationLevel(deferredEntity.ReplicationLevel))
                {
                    var existedLevel = (ReplicationLevel)(existedRef?.CurrentReplicationMask ?? 0);
                    if (typeState.DeferredEntities.TryGetValue(deferredEntity.Entity.Guid, out var existedDeferredLevel))
                    {
                        if (existedDeferredLevel.DeferredLevel < deferredEntity.ReplicationLevel)
                        {
                            typeState.DeferredEntities[deferredEntity.Entity.Guid] = new DeferredEntityState(deferredEntity.ReplicationLevel, existedLevel); 
                        }
                    }
                    else
                    {
                        typeState.DeferredEntities[deferredEntity.Entity.Guid] = new DeferredEntityState(deferredEntity.ReplicationLevel, existedLevel);
                    }
                }
            }
        }

        public IReadOnlyDictionary<Guid, IEntityRef> GetEntitiesCollection(Type type)
        {
            if (_typeToCollection.TryGetValue(type, out var collection))
                return collection;

            return ImmutableDictionary.Create<Guid, IEntityRef>();
        }

        private ConcurrentDictionary<Guid, IEntityRef> GetOrCreateEntitiesCollection(Type type)
        {
            if (ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type) != type)
                throw new KeyNotFoundException($"Type {type} is not a master entity type");

            return _typeToCollection.GetOrAdd(type, (key) => new ConcurrentDictionary<Guid, IEntityRef>());
        }

        protected IEntityRef GetInternal(Type type, Guid entityId)
        {
            var collection = GetEntitiesCollection(type);
            collection.TryGetValue(entityId, out var result);
            return result;
        }

        protected bool RemoveInternal(int typeId, Guid entityId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            
            if (!_typeToCollection.TryGetValue(type, out var collection))
                return false;

            return collection.TryRemove(entityId, out var _);
        }

        protected IEntityRef SetInternal(int typeId, IEntity entity, long replicationMask, int version)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            var collection = GetOrCreateEntitiesCollection(type);

            var entityRef = ReplicaTypeRegistry.Set(type, entity, replicationMask, version, collection);
            ((IEntityExt)entity).SetEntitiesRepository(this);
            return entityRef;
        }

        protected Task UnloadAllImpl()
        {
            var tasks = _typeToCollection.Values.SelectMany(v => v.Values).Select(v => OnEntityUnload(v)).ToList();
            return Task.WhenAll(tasks);
        }

        public static Type GetTypeById(int id) => ReplicaTypeRegistry.GetTypeById(id);
        public static int GetMasterTypeIdByReplicationLevelType(int typeId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            var masterType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            var masterId = ReplicaTypeRegistry.GetIdByType(masterType);
            return masterId;
        }
        public static long GetReplicationMask(int typeId) => (long)ReplicaTypeRegistry.GetReplicationLevelByReplicaType(ReplicaTypeRegistry.GetTypeById(typeId));
        public static int GetReplicationTypeId(int masterId, ReplicationLevel replicationLevel)
        {
            var type = ReplicaTypeRegistry.GetTypeById(masterId);
            var replicaType = ReplicaTypeRegistry.GetReplicationTypeId(type, replicationLevel);
            var replicaTypeId = ReplicaTypeRegistry.GetIdByType(replicaType);
            return replicaTypeId;
        }

        public static int GetIdByType(Type type) => ReplicaTypeRegistry.GetIdByType(type);

        public static int GetIdByTypeName(string typeName) => ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(typeName));

        public static bool IsServiceEntityType(int typeId)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            return Attribute.IsDefined(type, typeof(EntityServiceAttribute), true);
        }

        protected IEnumerable<IEntityRef> GetAllEntityDebug()
        {
            return _typeToCollection.Values.SelectMany(v => v.Values);
        }

        public string GetObjectsCount()
        {
            var sb = new System.Text.StringBuilder();
            foreach(var entityType in _typeToCollection)
            {
                sb.AppendFormat("{0} : {1} ", entityType.Key, entityType.Value.Count).AppendLine();
            }
            return sb.ToString();
        }
    }
}
