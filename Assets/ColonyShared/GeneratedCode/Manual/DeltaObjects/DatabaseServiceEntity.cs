using GeneratedCode.DeltaObjects.Chain;
using GeneratedCode.Repositories;
using MongoDB.Bson;
using SharedCode.EntitySystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NLog;
using SharedCode.Utils;
using SharedCode.Utils.BsonSerialization;
using SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using SharedCode.Wizardry;
using Assets.Src.ResourcesSystem.Base;
using Core.Reflection;
using GeneratedCode.DatabaseUtils;
using SharedCode.MovementSync;
using MongoDB.Bson.Serialization.Attributes;
using Src.Aspects.Impl.Stats.Proxy;
using SharedCode.Entities.Engine;
using GeneratedCode.Network.Statistic;
using SharedCode.Config;
using SharedCode.EntitySystem.Delta;
using System.Reflection;
using MongoDB.Bson.Serialization.Conventions;
using SharedCode.Serializers;
using GeneratedCode.MapSystem;
using ColonyShared.SharedCode;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using ProtoBuf;
using SharedCode.Repositories;
using MongoDB.Bson.IO;
using SharedCode.Serializers.Protobuf;
using SharedCode.Entities.Cloud;
using SharedCode.Refs;
using System.Threading;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    using EntityKey = ValueTuple<int, Guid>;

    public partial class DatabaseServiceEntity : IHasLoadFromJObject, IHookOnInit, IHookOnDestroy, IHookOnUnload
    {
        enum OperationType
        {
            Add,
            Update,
            Unload,
            Delete
        }

        private readonly QueueDictionary<EntityKey, OperationType> _entitiesChanged = new QueueDictionary<EntityKey, OperationType>();
        private readonly ConcurrentDictionary<EntityKey, CacheEntry> _entitiesCache = new ConcurrentDictionary<EntityKey, CacheEntry>();
        
        private MongoClient mongoClient;
        private IMongoDatabase db;

        public static bool TestEnverovement { get; set; }

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger ConsoleLogger = LogManager.GetLogger("Console");
        private DatabaseServiceEntityConfig _config;

        private static readonly string _buildNumberKey = "__buildNumber";
        private static readonly string _entityHashKey = "__version";
        private static readonly string _typeFieldName = "_t";
        private static readonly string _deltaObjectsFieldName = "__deltaObjects";

        private readonly CancellationTokenSource _chainCts = new CancellationTokenSource();
        private SuspendingAwaitable SaveTask;
        private SuspendingAwaitable CleanCacheTask;

        private static IEnumerable<Assembly> GetCustomAssemblies()
        {
            var allowedAssembliesSubstrs = new List<string>() { "SharedCode", "GeneratedCode" };
            var assemblies = AppDomain.CurrentDomain.GetAssembliesSafe().Where(x => !x.IsDynamic && allowedAssembliesSubstrs.Any(y => x.FullName.Contains(y)));
            return assemblies;
        }

        private class RemoveSerializedElementNameConvention : ConventionBase, IMemberMapConvention
        {
            private string _suffix = "__Serialized";
            public void Apply(BsonMemberMap memberMap)
            {
                string name = memberMap.MemberName;
                name = GetElementName(name);
                memberMap.SetElementName(name);
                // memberMap.SetIgnoreIfDefault(false);
            }
            private string GetElementName(string memberName)
            {
                if (memberName.Length > _suffix.Length + 1 && memberName.EndsWith(_suffix))
                    return memberName.Substring(0, memberName.Length - _suffix.Length);
                else
                    return memberName;
            }
        }

        private static void BsonRegisterClasses()
        {
            var pack = new ConventionPack();
            pack.Add(new RemoveSerializedElementNameConvention());

            ConventionRegistry.Register("Remove Serialized conversionPack", pack, t => true);

            BsonSerializer.RegisterSerializationProvider(new IResourcesBsonSerializationProvider());

            //BsonSerializer.RegisterSerializer(typeof(Guid), new StringGuidSerializer());
            BsonSerializer.RegisterSerializer(typeof(Vector3Int), new BsonVector3IntSerializer());
            BsonSerializer.RegisterSerializer(typeof(Vector2Int), new BsonVector2IntSerializer());
            BsonSerializer.RegisterSerializer(typeof(Vector3), new BsonVector3Serializer());
            BsonSerializer.RegisterSerializer(typeof(Vector2), new BsonVector2Serializer());
            BsonSerializer.RegisterSerializer(typeof(Quaternion), new BsonQuaternionSerializer());
            BsonSerializer.RegisterSerializer(typeof(OnSceneObjectNetId), new BsonOnSceneObjectNetIdSerializer());
            BsonSerializer.RegisterSerializer(typeof(StatModifier), new BsonStatModifierSerializer());
            BsonSerializer.RegisterSerializer(typeof(SpellId), new BsonSpellIdSerializer());
            BsonSerializer.RegisterSerializer(typeof(ResourceIDFull), new BsonResourceIDFullSerializer());
            BsonSerializer.RegisterSerializer(typeof(Transform), new BsonTransformSerializer());
            BsonSerializer.RegisterSerializer(typeof(CharacterMovementState), new BsonCharacterMovementStateSerializer());
            BsonSerializer.RegisterSerializer(typeof(CharacterMovementStateFrame), new BsonCharacterMovementStateFrameSerializer());
            BsonSerializer.RegisterSerializer(typeof(TimeStatState), new BsonTimeStatStateSerializer());
            BsonSerializer.RegisterSerializer(typeof(SceneLoadableObjectsData), new SceneLoadableObjectsDataSerializer());
            BsonSerializer.RegisterSerializer(typeof(ModifierCauser), new BsonModifierCauserSerializer());
            BsonSerializer.RegisterSerializer(typeof(RealmRulesQueryState), new BsonRealmRulesQueryStateSerializer());
            
            BsonSerializer.RegisterSerializer(typeof(ResourceSystem.Utils.OuterRef), new OuterRefSerializer());
            BsonSerializer.RegisterSerializer(typeof(Value), new ValueSerializer());

            BsonSerializer.RegisterGenericSerializerDefinition(typeof(OuterRef<>), typeof(BsonOuterRefSerializer<>));
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(IDeltaList<>), typeof(BsonDeltaListSerializer<>));
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(DeltaList<>), typeof(BsonDeltaListSerializer<>));
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(IDeltaDictionary<,>), typeof(BsonDeltaDictionarySerializer<,>));
            BsonSerializer.RegisterGenericSerializerDefinition(typeof(DeltaDictionary<,>), typeof(BsonDeltaDictionarySerializer<,>));
            
            var bsonDiscriminatorType = typeof(BsonDiscriminatorAttribute);

            foreach (var assembly in GetCustomAssemblies())
            {
                foreach (var type in assembly.GetTypesSafe())
                {
                    if (!type.IsAbstract && !type.IsInterface && !type.IsGenericType)
                    {
                        try
                        {
                            var attribute = type.GetCustomAttributes(bsonDiscriminatorType, true).FirstOrDefault();
                            if (attribute != null)
                            {
                                BsonClassMap.LookupClassMap(type);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.IfError()?.Message(e, "BsonClassMap.LookupClassMap error").Write();;
                        }
                    }
                }
            }
        }

        static DatabaseServiceEntity()
        {
            BsonRegisterClasses();
        }

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _config = (DatabaseServiceEntityConfig)config;
            DatabaseServiceType = (DatabaseServiceType)Enum.Parse(typeof(DatabaseServiceType), _config.DatabaseType);
            return Task.CompletedTask;
        }

        public async Task OnInit()
        {
            if (_config == null)
            {
                Logger.IfError()?.Message("{repo_id} Database config is null", Id).Write();
                return;
            }

            var connString = DatabaseServiceType == DatabaseServiceType.GameData ? _config?.ConnectionAddresses.Target?.MongoShardConnectionString : _config?.ConnectionAddresses.Target?.MongoMetaConnectionString;
            var dbName = DatabaseServiceType == DatabaseServiceType.GameData ? _config?.ConnectionAddresses.Target?.MongoShardDataBaseName : _config?.ConnectionAddresses.Target?.MongoMetaDataBaseName;

            if (string.IsNullOrWhiteSpace(connString))
            {
                Logger.IfWarn()?.Message("{repo_id} Database service disabled via empty connection string", Id).Write();
                return;
            }

            MigrationHelper.InitializeMigrations();

            if (_config.GenerateVersionsSnapshot)
                MigrationHelper.SaveCurrentEntityVersionsSnapshot();
            if (_config.CheckMigrations)
                await MigrationHelper.CheckMigrations();

            this.EntitiesRepository.EntityUpdated += EntitiesRepository_EntityUpdated;
            this.EntitiesRepository.NewEntityUploaded += EntitiesRepository_EntityAdded;
            this.EntitiesRepository.EntityUnloaded += EntitiesRepository_EntityUnloaded;
            if (!TestEnverovement)
            {
                SaveTask = AsyncUtils.RunAsyncTask(SaveChain);
                CleanCacheTask = AsyncUtils.RunAsyncTask(CleanCacheChain);
            }
            Logger.IfInfo()?.Message("Database service {repo_id} try connect to mongodb {mongo_db_name}", Id, dbName).Write();

            bool cosmosMode = connString.IndexOf("cosmos.azure.com", StringComparison.InvariantCultureIgnoreCase) >= 0;

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connString);
            if(settings.UseTls)
            {
                settings.SslSettings = new SslSettings() { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            }

            settings.ConnectTimeout = TimeSpan.FromSeconds(_config.MongoTimeout);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(_config.MongoTimeout);
            mongoClient = new MongoClient(settings);
            db = mongoClient.GetDatabase(dbName);

            if(cosmosMode)
                ConsoleLogger.IfWarn()?.Message("Detected cosmos db for service {repo_id}, skipping connection check", Id).Write();

            while (!cosmosMode && !EntitiesRepository.StopToken.IsCancellationRequested)
            {
                var delayTask = Task.Delay(TimeSpan.FromSeconds(5), EntitiesRepository.StopToken);
                try
                {
                    await db.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                    break;
                }
                catch (Exception e)
                {
                    ConsoleLogger.IfError()?.Message(e, "Cant connect to mongo").Write();;
                }

                await delayTask;
            }
        }

        private async Task SaveChain()
        {
            while (!_chainCts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_config.SaveDelay), _chainCts.Token);
                    await AsyncUtils.RunAsyncTask(InvokeSave);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }

            async Task InvokeSave()
            {
                using (await this.GetThisWrite())
                {
                    await Save();
                }
            }
        }

        private async Task CleanCacheChain()
        {
            while (!_chainCts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_config.CacheLifeTime), _chainCts.Token);
                    await AsyncUtils.RunAsyncTask(InvokeClean);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }

            async Task InvokeClean()
            {
                using (await this.GetThisWrite())
                {
                    await CleanCache();
                }
            }
        }

        private Task EntitiesRepository_EntityChanged(EntityKey key, OperationType operationType)
        {
            var databaseSaveType = DatabaseSaveTypeChecker.GetDatabaseSaveType(key.Item1);
            if (databaseSaveType == DatabaseSaveType.None)
                return Task.CompletedTask;
            lock (_entitiesChanged)
            {
                _entitiesChanged[key] = operationType;
            }
            return Task.CompletedTask;
        }

        public Task EntitiesRepository_EntityAdded(int typeId, Guid guid)
        {
            var key = new EntityKey(typeId, guid);
            _entitiesCache.TryRemove(key, out var _);
            return EntitiesRepository_EntityChanged(key, OperationType.Add);
        }

        public Task EntitiesRepository_EntityUpdated(int typeId, Guid guid)
        {
            var key = new EntityKey(typeId, guid);
            return EntitiesRepository_EntityChanged(key, OperationType.Update);
        }

        public Task EntitiesRepository_EntityUnloaded(int typeId, Guid guid, bool destroy, IEntity entity)
        {
            var key = new EntityKey(typeId, guid);
            if (destroy)
            {
                _entitiesCache.TryRemove(key, out var _);
                return EntitiesRepository_EntityChanged(key, OperationType.Delete);
            }
            else
            {
                var databaseSaveType = DatabaseSaveTypeChecker.GetDatabaseSaveType(entity.TypeId);
                if (databaseSaveType == DatabaseSaveType.None)
                    return Task.CompletedTask;
                _entitiesCache[key] = new CacheEntry(ConvertToDoc(entity), false, DateTime.UtcNow);
                return EntitiesRepository_EntityChanged(key, OperationType.Unload);
            }
        }

        public Task CleanCacheImpl()
        {
            DateTime currentTime = DateTime.UtcNow;
            foreach (var key in _entitiesCache.Keys)
            {
                if (_entitiesCache.TryGetValue(key, out var value))
                {
                    if (value.Unloaded && (currentTime - value.Date) > TimeSpan.FromSeconds(_config.CacheLifeTime))
                    {
                        _entitiesCache.TryRemove(key, out _);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public async Task<(IEntity entity, string commitId, bool converted)> LoadInternal(int typeId, Guid entityId)
        {
            Type entityType = ReplicaTypeRegistry.GetTypeById(typeId);
            BsonDocument deserializedDoc;
            var key = new EntityKey(typeId, entityId);
            if (_entitiesCache.TryGetValue(key, out var cacheValue))
            {
                deserializedDoc = (BsonDocument)cacheValue.SerializedEntity.Clone();
            }
            else
            {
                var typeName = entityType.Name;
                IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(typeName);

                var filter = Builders<BsonDocument>.Filter.Eq("_id", entityId);
                deserializedDoc = await collection.Find(filter).FirstOrDefaultAsync();
            }

            if (deserializedDoc == null)
                return (null, "", false);

            using (_EntityContext.Pool.Set())
            using (_SerializerContext.Pool.Set())
            {
                _EntityContext.Pool.Current.FullCreating = true;
                
                _SerializerContext.Pool.Current.EntitiesRepository = EntitiesRepository;
                _SerializerContext.Pool.Current.Deserialization = true;

                if (_config.VerboseLogs)
                    Logger.IfInfo()?.Message("DatabaseServiceEntity::LoadInternal before ConvertFromDoc {0}", entityType).Write();
                var result = ConvertFromDoc(deserializedDoc, entityType);
                if (_config.VerboseLogs)
                    Logger.IfInfo()?.Message("DatabaseServiceEntity::LoadInternal after ConvertFromDoc {0}", entityType).Write();
                return result;
            }
        }

        public ValueTask<bool> DataSetExistsImpl(int typeId, Guid entityId)
        {
            Type entityType = ReplicaTypeRegistry.GetTypeById(typeId);
            var key = new EntityKey(typeId, entityId);
            if (_entitiesCache.ContainsKey(key))
                return new ValueTask<bool>(true);

            var typeName = entityType.Name;
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(typeName);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", entityId);
            return new ValueTask<bool>(collection.Find(filter).Any());
        }

        public async Task<SerializedEntityBatch> LoadImpl(int typeId, Guid entityId)
        {
            if (db == null)
                return new SerializedEntityBatch();

            var entities = new List<(IEntity entity, string commitId, bool converted)>();
            if (_config.VerboseLogs)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::LoadImpl before entity load {0}:{1}", typeId, entityId).Write();
            await AsyncUtils.RunAsyncTask(() => LoadRecursive(typeId, entityId, entities));
            if (_config.VerboseLogs)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::LoadImpl after entity load {0}:{1}", typeId, entityId).Write();

            foreach (var entity in entities)
            {
                if (entity.converted)
                {
                    if (_config.VerboseLogs)
                        Logger.IfInfo()?.Message("DatabaseServiceEntity::LoadImpl migration, before entity save {0}", entity.entity.GetType()).Write();
                    await Save(entity.Item1);
                    if (_config.VerboseLogs)
                        Logger.IfInfo()?.Message("DatabaseServiceEntity::LoadImpl migration, after entity save {0}", entity.entity.GetType()).Write();
                }
            }

            var batchContainer = new SerializedEntityBatch();
            var serializer = ((IEntitiesRepositoryExtension)EntitiesRepository).EntitySerializer;
            foreach (var entity in entities)
            {
                var replicationSet = ((IEntityExt) entity.entity).TraverseObjects(ReplicationLevel.Master, false);
                var snapshot = serializer.SerializeReplicationSetFull(replicationSet, (long) ReplicationLevel.Master);
                
                batchContainer.Batches.Add(new SerializedEntityData
                {
                    EntityId = entity.entity.Id,
                    EntityTypeId = entity.entity.TypeId,
                    Version = entity.commitId,
                    Snapshot = snapshot
                });
            }

            return batchContainer;
        }

        private async Task LoadRecursive(int typeId, Guid entityId, List<(IEntity entity, string commitId, bool converted)> loadedEntities)
        {
            if (loadedEntities.Any(x => x.entity.TypeId == typeId && x.entity.Id == entityId))
                return;

            var entityTuple = await LoadInternal(typeId, entityId);

            if (entityTuple.entity != null)
            {
                loadedEntities.Add(entityTuple);

                var linkedEntities = new List<(long level, IEntityRef entityRef)>();
                entityTuple.Item1.GetAllLinkedEntities((long) ReplicationLevel.Master, linkedEntities, 0, false);
                foreach (var pair in linkedEntities)
                    await LoadRecursive(ReplicaTypeRegistry.GetIdByType(pair.entityRef.GetEntityInterfaceType()), pair.entityRef.Id, loadedEntities);
            }
        }

        public static Type GetActualType(BsonDocument bsonDocument, Type nominalType)
        {
            // we can skip looking for a discriminator if nominalType has no discriminated sub types
            if (BsonSerializer.IsTypeDiscriminated(nominalType))
            {
                var actualType = nominalType;
                BsonValue discriminator;
                if (bsonDocument.TryGetValue(_typeFieldName, out discriminator))
                {
                    if (discriminator.IsBsonArray)
                    {
                        discriminator = discriminator.AsBsonArray.Last(); // last item is leaf class discriminator
                    }
                    actualType = BsonSerializer.LookupActualType(nominalType, discriminator);
                }
                return actualType;
            }

            return nominalType;
        }

        private static BsonDocument ConvertToDoc(IEntity entity)
        {
            try
            {
                using (_SerializerContext.Pool.Set())
                {
                    _SerializerContext.Pool.Current.FullSerialize = true;
                    
                    var replicationSet = ((IEntityExt) entity).TraverseObjects(ReplicationLevel.Master, false);
                    // -1 -- потому что не добавляем Entity в список
                    var deltaObjects = new BsonDocument[replicationSet.Count - 1];

                    int i = 0;
                    foreach (var deltaObjectPair in replicationSet)
                    {
                        if (deltaObjectPair.Key != entity)
                        {
                            deltaObjects[i] = deltaObjectPair.Key.ToBsonDocument();
                            i++;
                        }
                    }
                
                    var entitySerialized = entity.ToBsonDocument();
                    entitySerialized.Add(_buildNumberKey, ((IEntityExt)entity).CodeVersion);
                    entitySerialized.Add(_entityHashKey, TypeHashCalculator.GetHashByType(entity.GetType()));
                    entitySerialized.Add(_deltaObjectsFieldName, new BsonArray(deltaObjects));
                    return entitySerialized;
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Error on saving entity {entityName} {entityType}", entity.Id, entity.GetType().Name).Write();
                throw e;
            }
        }

        private static (IEntity entity, string buildNumber, bool converted) ConvertFromDoc(BsonDocument bsonDocument,
            Type entityType)
        {
            string buildNumber = bsonDocument.GetValue(_buildNumberKey).AsString;
            bsonDocument.Remove(_buildNumberKey);

            string savedHash = bsonDocument.GetValue(_entityHashKey).AsString;
            bsonDocument.Remove(_entityHashKey);

            var serializedDeltaObjects = bsonDocument.GetValue(_deltaObjectsFieldName).AsBsonArray;
            var deserializedDeltaObjects = new IDeltaObject[serializedDeltaObjects.Count];
            for (var index = 0; index < serializedDeltaObjects.Count; index++)
            {
                var serializedDeltaObject = serializedDeltaObjects[index];
                var deltaObject = BsonSerializer.Deserialize<IDeltaObject>(serializedDeltaObject.AsBsonDocument);
                deserializedDeltaObjects[index] = deltaObject;
                _SerializerContext.Pool.Current.ChangedObjects.TryAdd(((IDeltaObjectExt) deltaObject).LocalId,
                    new DeserializedObjectInfo(deltaObject, null));
            }

            bsonDocument.Remove(_deltaObjectsFieldName);

            Type actualType = GetActualType(bsonDocument, entityType);

            string currentHash = TypeHashCalculator.GetHashByType(actualType);

            bool converted = false;
            //convert deserializedDoc if needed
            if (currentHash != savedHash)
            {
                IList<IMigrator> migrators = MigrationHelper.GetMigrators(actualType, savedHash, currentHash);
                if (migrators == null)
                    throw new Exception(
                        $"Migrators not found type:{actualType.Name} from:{savedHash} to:{currentHash}");
                for (int i = migrators.Count - 1; i >= 0; --i)
                    bsonDocument = migrators[i].Convert(bsonDocument);
                converted = true;
            }

            var entity = BsonSerializer.Deserialize<IEntity>(bsonDocument);
            foreach (var deltaObject in deserializedDeltaObjects)
            {
                ((IDeltaObjectExt) deltaObject).LinkChangedDeltaObjects(
                    _SerializerContext.Pool.Current.ChangedObjects, entity);
            }

            ((IDeltaObjectExt) entity).LinkChangedDeltaObjects(
                _SerializerContext.Pool.Current.ChangedObjects, entity);

            return (entity, buildNumber, converted);
        }

        private async Task Save(IEntity entity)
        {
            var typeName = ReplicaTypeRegistry.GetTypeById(entity.TypeId).Name;
            IMongoCollection<IEntity> collection = db.GetCollection<IEntity>(typeName);
            var filter = Builders<IEntity>.Filter.Eq("_id", entity.Id);
            await collection.ReplaceOneAsync(filter, entity, new UpdateOptions { IsUpsert = true });
        }

        private async Task<Task> Save(int typeId, IDictionary<Guid, OperationType> operations)
        {
            var list = new Dictionary<Guid, BsonDocument>();

            var typeName = ReplicaTypeRegistry.GetTypeById(typeId).Name;

            if (_config.VerboseLogs)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::Save before collect {0}", typeId).Write();
            List<EntityKey> unloadedKeys = new List<EntityKey>();

            foreach (var operation in operations)
            {
                switch (operation.Value)
                {
                    case OperationType.Add:
                    case OperationType.Update:
                        using (var wrapper = await EntitiesRepository.Get(typeId, operation.Key))
                        {
                            IEntity entity;
                            if (wrapper.TryGet<IEntity>(typeId, operation.Key, ReplicationLevel.Master, out entity))
                            {
                                if (entity is IBaseDeltaObjectWrapper)
                                    entity = (IEntity)((IBaseDeltaObjectWrapper)entity).GetBaseDeltaObject();

                                if (_config.VerboseLogs)
                                    Logger.IfInfo()?.Message("DatabaseServiceEntity::Save before ConvertToDoc {0}", entity.GetType()).Write();
                                list[entity.Id] = ConvertToDoc(entity);
                                if (_config.VerboseLogs)
                                    Logger.IfInfo()?.Message("DatabaseServiceEntity::Save after ConvertToDoc {0}", entity.GetType()).Write();
                            }
                            else
                            {
                                var entityRef = ((IEntitiesContainerExtension)wrapper).GetEntityRef(typeId, operation.Key);
                                var mask = entityRef != null ? entityRef.CurrentReplicationMask : 0;
                                Logger.IfWarn()?.Message("Save get error {typeName} id {operation} mask {mask}", typeName, operation.Key, mask).Write();
                            }
                        }
                        break;

                    case OperationType.Delete:
                        list[operation.Key] = null;
                        break;

                    case OperationType.Unload:
                        var key = new EntityKey(typeId, operation.Key);
                        unloadedKeys.Add(key);
                        if (_entitiesCache.TryGetValue(key, out  var cacheValue))
                        {
                            list[operation.Key] = cacheValue.SerializedEntity;
                        }
                        break;
                }
            }
            //entity.TypeName

            if (_config.VerboseLogs)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::Save after collect {0}", typeId).Write();

            if (list.Count > 0)
            {
                var models = new WriteModel<BsonDocument>[list.Count];
                int i = 0;

                Statistics<DataBaseStatistics>.Instance.SaveEntity(typeName, list.Count);

                foreach (var doc in list)
                {
                    if (doc.Value == null)
                    {
                        models[i++] = new DeleteOneModel<BsonDocument>(new BsonDocument("_id", doc.Key));
                    }
                    else
                    {
                        try
                        {
                            models[i++] = new ReplaceOneModel<BsonDocument>(new BsonDocument("_id", doc.Key), doc.Value) { IsUpsert = true };
                        }
                        catch (Exception e)
                        {
                            Logger.IfError()?.Message(e, "Save bson error {type}", ReplicaTypeRegistry.GetTypeById(typeId).Name).Write();
                        }
                    }
                }

                return DoSave(typeName, models, unloadedKeys);// collection.BulkWriteAsync(models);
            }
            return Task.CompletedTask;
        }

        private async Task DoSave(string typeName, WriteModel<BsonDocument>[] models, List<EntityKey> unloadedKeys)
        {
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(typeName);
            await collection.BulkWriteAsync(models);
            foreach (var key in unloadedKeys)
            {
                if (_entitiesCache.TryGetValue(key, out var cacheValue))
                {
                    _entitiesCache[key] = new CacheEntry(cacheValue.SerializedEntity, true, cacheValue.Date);
                }
            }
        }

        public async Task SaveImpl()
        {
            if (db == null)
                return;

            Dictionary<int, Dictionary<Guid, OperationType>> temp = new Dictionary<int, Dictionary<Guid, OperationType>>();

            int counter = 0;
            lock (_entitiesChanged)
            {
                while (counter <= _config.MaxItemsToWrite && _entitiesChanged.Count > 0)
                {
                    var pair = _entitiesChanged.Dequeue();

                    Dictionary<Guid, OperationType> dict;
                    if (!temp.TryGetValue(pair.Key.Item1, out dict))
                    {
                        dict = new Dictionary<Guid, OperationType>();
                        temp[pair.Key.Item1] = dict;
                    }
                    dict[pair.Key.Item2] = pair.Value;
                    ++counter;
                }

            }

            Task[] tasks = new Task[temp.Count];
            KeyValuePair<int, Dictionary<Guid, OperationType>>[] pairs = new KeyValuePair<int, Dictionary<Guid, OperationType>>[temp.Count];

            {
                int i = 0;
                foreach (var pair in temp)
                {
                    tasks[i] = await Save(pair.Key, pair.Value);
                    pairs[i] = pair;
                    ++i;
                }
            }

            if (_config.VerboseLogs && tasks.Length > 0)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::SaveImpl before save").Write();
            await Task.WhenAll(tasks);
            if (_config.VerboseLogs && tasks.Length > 0)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::SaveImpl after save").Write();

            for (int i = 0; i < tasks.Length; ++i)
            {
                if (tasks[i].IsFaulted && pairs[i].Value.Count > 0)
                {
                    lock (_entitiesChanged)
                    {
                        foreach (var pair in pairs[i].Value)
                        {
                            var key = new EntityKey(pairs[i].Key, pair.Key);
                            if (!_entitiesChanged.ContainsKey(key))
                            {
                                _entitiesChanged[key] = pair.Value;
                            }
                        }
                    }
                }
            }
        }

        public async Task<Guid> GetAccountIdByNameImpl(string accountName)
        {
            if (db == null)
                return Guid.Empty;

            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>("IAccountEntity");

            var filter = Builders<BsonDocument>.Filter.Eq("AccountId", accountName);
            if (_config.VerboseLogs)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::GetAccountIdByNameImpl before load {0}", accountName).Write();
            BsonDocument deserializedDoc = (await collection.FindAsync(filter)).FirstOrDefault();
            if (_config.VerboseLogs)
                Logger.IfInfo()?.Message("DatabaseServiceEntity::GetAccountIdByNameImpl after load {0}", accountName).Write();

            if (deserializedDoc != null)
            {
                BsonValue guid;
                if (deserializedDoc.TryGetValue("_id", out guid))
                {
                    var temp = new Guid(guid.AsBsonBinaryData.Bytes);
                    return temp;
                }
            }

            return Guid.Empty;
        }

        public async Task OnDestroy()
        {
            _chainCts.Cancel();
            try
            {
                await SaveTask;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
            try
            {
                await CleanCacheTask;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
            _chainCts.Dispose();
        }

        public async Task OnUnload()
        {
            _chainCts.Cancel();
            try
            {
                await SaveTask;
            }
            catch(Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
            try
            {
                await CleanCacheTask;
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
            _chainCts.Dispose();
        }

        private readonly struct CacheEntry
        {
            public CacheEntry(BsonDocument serializedEntity, bool unloaded, DateTime date)
            {
                SerializedEntity = serializedEntity;
                Unloaded = unloaded;
                Date = date;
            }

            public BsonDocument SerializedEntity { get; }
            public bool Unloaded { get; }
            public DateTime Date { get; }
        }
        
        // private class EntityContainer
        // {
        //     public EntityContainer(Guid mongoID, int buildNumber, string version,
        //         IDeltaObject[] deltaObjects)
        //     {
        //         BuildNumber = buildNumber;
        //         Version = version;
        //         DeltaObjects = deltaObjects;
        //         MongoID = mongoID;
        //     }
        //     
        //     [BsonId]
        //     public Guid MongoID { get; set; }
        //
        //     public int BuildNumber { get; set; }
        //
        //     public string Version { get; set; }
        //     
        //     public IDeltaObject[] DeltaObjects { get; set; }
        // }
        //
        // private class BsonEntityContainerSerializer: SerializerBase<EntityContainer>
        // {
        //     public static readonly int Version = 1;
        //
        //     private DictionaryInterfaceImplementerSerializer<Dictionary<ulong, IDeltaObject>, ulong, IDeltaObject> serializer;
        //
        //     public BsonEntityContainerSerializer()
        //     {
        //         serializer = new DictionaryInterfaceImplementerSerializer<Dictionary<ulong, IDeltaObject>, ulong, IDeltaObject>(DictionaryRepresentation.ArrayOfArrays);
        //     }
        //
        //     public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, EntityContainer value)
        //     {
        //         context.Writer.WriteStartDocument();
        //         
        //         context.Writer.writeid(new ObjectId(value.MongoID.ToString()));
        //         context.Writer.WriteInt32(nameof(EntityContainer.BuildNumber), value.BuildNumber);
        //         context.Writer.WriteString(nameof(EntityContainer.Version), value.Version);
        //         
        //         context.Writer.WriteName(nameof(EntityContainer.DeltaObjects));
        //         serializer.Serialize(context, value.DeltaObjects);
        //         
        //         context.Writer.WriteEndDocument();
        //     }
        //
        //     public override EntityContainer Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        //     {
        //         context.Reader.ReadStartDocument();
        //
        //         var mongoId = Guid.Parse(context.Reader.ReadObjectId().ToString());
        //         var buildNumber = context.Reader.ReadInt32(nameof(EntityContainer.BuildNumber));
        //         var version = context.Reader.ReadString(nameof(EntityContainer.Version));
        //
        //         context.Reader.ReadName();
        //         var deltaObjects = serializer.Deserialize(context);
        //         
        //         context.Reader.ReadEndDocument();
        //
        //         return new EntityContainer(mongoId, buildNumber, version, deltaObjects);
        //     }
        // }
    }
}
