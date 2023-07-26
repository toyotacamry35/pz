using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeneratedCode.Custom.Cloud.Communications;
using GeneratedCode.Custom.Config;
using GeneratedCode.Custom.Network;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Manual.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SharedCode.Cloud;
using SharedCode.Config;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Interfaces;
using SharedCode.Network;
using SharedCode.Refs;
using SharedCode.Serializers;
using SharedCode.Utils;
using NLog.Fluent;
using ResourcesSystem.Loader;
using GeneratedCode.DeltaObjects.Chain;
using GeneratedCode.EntitySystem;
using GeneratedCode.Network.Statistic;
using SharedCode.Refs.Operations;
using System.Runtime.Serialization;
using GeneratedCode.DatabaseUtils;
using SharedCode.Serializers.Protobuf;
using SharedCode.Extensions;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Repositories;
using Core.Reflection;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem.Migrating;
using GeneratedCode.EntitySystem.Statistics;
using SharedCode.EntitySystem.ChainCalls;
using LiteNetLib;
using ResourceSystem.Utils;

namespace GeneratedCode.Repositories
{
    public partial class EntitiesRepository : IEntitiesRepository, IEntitiesRepositoryExtension,
        IEntitiesRepositoryDataExtension, IEntitiesRepositoryDebugExtension
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly NLog.Logger ConsoleLogger = LogManager.GetLogger("Console");

        protected static readonly NLog.Logger ReplicationsLogger = LogManager.GetLogger("EntityReplicationsHistory");

        public ICommunicationNode _internalCommunicationNode;

        protected ICommunicationNode _externalCommunicationNode;

        public event Func<int, Guid, Task> NewEntityUploaded;

        public event Func<int, Guid, Task> EntityUpdated;

        public event Func<int, Guid, bool, IEntity, Task> EntityUnloaded;

        public event Func<int, Guid, Task> EntityCreated;

        public event Func<int, Guid, Task> EntityLoaded;

        public event Func<int, Guid, IEntity, Task> EntityDestroy;

        public event UserDisconnectedDelegate UserDisconnected;

        public event Func<Task> CloudRequirementsMet;

        protected CloudNodeType _cloudNodeType;

        public CloudNodeType CloudNodeType => _cloudNodeType;

        protected ConcurrentBag<IEntityRef> _serviceEntities = new ConcurrentBag<IEntityRef>();

        private Assets.Src.ResourcesSystem.Base.ResourceRef<CloudRequirement>[] _cloudRequirements => _sharedConfig.CloudRequirements;

        public CancellationToken StopToken { get; }

        private CancellationTokenSource _stopTokenSource;

        public string RepositoryConfigId => _config.ConfigId;

        public int RepositoryNum { get; private set; }

        private ConcurrentDictionary<Guid, ChainCancellationToken> _pingChainCalls =
            new ConcurrentDictionary<Guid, ChainCancellationToken>();

        public bool SuppressEntityInitialization => _config.SuppressEntityInitialization;
        EntityRef<IRepositoryCommunicationEntity> _repoComRef;
        private EntitiesRepositoryConfig _config { get; }
        private CloudSharedDataConfig _sharedConfig { get; }

        private static IPAddress GetAddressForHost(string cfg)
        {
            var host = string.IsNullOrEmpty(cfg) ? NetworkUtils.GetMyIp(false) : cfg;
            var ipAddress = NetUtils.ResolveAddress(host);
            return ipAddress;
        }

        private static (EndpointAddress bind, EndpointAddress announce) ConfigureAddresses(string bindAddressCfg, int bindPortCfg, string announceAddressCfg, int announcePortCfg)
        {
            var bindAddress = new EndpointAddress(
                string.IsNullOrEmpty(bindAddressCfg) ? IPAddress.Any.ToString() : bindAddressCfg,
                bindPortCfg);

            EndpointAddress announceAddress;
            if (string.IsNullOrEmpty(announceAddressCfg))
            {
                var address = NetworkUtils.GetMyIp(false);
                announceAddress = new EndpointAddress(address, bindAddress.Port);
            }
            else
            {
                announceAddress = new EndpointAddress(announceAddressCfg, announcePortCfg);
            }

            return (bindAddress, announceAddress);
        }

        public async Task InitCommunicationNode()
        {
            EndpointAddress internalBindAddress = new EndpointAddress();
            EndpointAddress internalAnnounceAddress = new EndpointAddress();
            EndpointAddress externalAnnounceAddress = new EndpointAddress();
            if (_cloudNodeType == SharedCode.Cloud.CloudNodeType.Server)
            {
                (internalBindAddress, internalAnnounceAddress) =
                    ConfigureAddresses(_config.Addresses.Target?.InternalBindAddress, _config.Ports.Target.InternalBindPort, _config.Addresses.Target?.InternalAnnounceAddress, _config.Ports.Target.InternalAnnouncePort);
                (_, externalAnnounceAddress) =
                    ConfigureAddresses(_config.Addresses.Target?.ExternalBindAddress, _config.Ports.Target.ExternalBindPort, _config.Addresses.Target?.ExternalAnnounceAddress, _config.Ports.Target.ExternalAnnouncePort);
            }

            var communicationRef = _repoComRef = await Create<IRepositoryCommunicationEntity>(this.Id, (entity) =>
            {
                entity.Config = _config;
                entity.ConfigId = _config.ConfigId;
                entity.InternalAddress = internalAnnounceAddress;
                entity.ExternalAddress = externalAnnounceAddress;
                entity.Num = RepositoryNum;
                entity.CloudNodeType = _cloudNodeType;
                entity.ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
                entity.CloudRequirementsMet = _cloudNodeType == SharedCode.Cloud.CloudNodeType.Client;
                entity.InitializationTasksCompleted = _cloudNodeType == SharedCode.Cloud.CloudNodeType.Client;
                entity.ExternalCommunicationNodeOpen = _cloudNodeType == SharedCode.Cloud.CloudNodeType.Client;

                return Task.CompletedTask;
            });

            using (var ent = await Get<IRepositoryCommunicationEntity>(communicationRef.Id))
            {
                var entity = ent.Get<IRepositoryCommunicationEntity>(communicationRef.Id);
                entity.WantsToDisconnect.RepoWantsToDisconnect += async (id) =>
                {
                    if (_connectedRepos.TryRemove(entity.OwnerRepositoryId, out var _))
                        await OnUserDisconnected(entity.OwnerRepositoryId);
                    foreach (var serviceEntity in _serviceEntities)
                    {
                        //Logger.IfError()?.Message($"Unsubscribe {serviceEntity.TypeName} {serviceEntity.Id} {cdType}").Write();
                        if (serviceEntity.TypeId == GetIdByType(typeof(IClusterAddressResolverServiceEntity)))
                            continue;
                        await UnsubscribeReplication(serviceEntity.TypeId, serviceEntity.Id, id,
                            ReplicationLevel.Server);
                    }
                };
            }

            await LoadDefaultServices();

            foreach (var serviceEntityConfig in _config.ServiceEntities)
            {
                var type = GetTypeById(GetIdByTypeName(serviceEntityConfig.Target.CloudEntityType));
                await CreateServiceEntity(type, _sharedConfig, serviceEntityConfig.Target.CustomConfig);
            }

            if (_cloudNodeType == CloudNodeType.Server)
            {
                var hostEntry = NetUtils.ResolveAddress(internalBindAddress.Address);
                _internalCommunicationNode = new CommunicationNode(this,
                    internalBindAddress.Port,
                    hostEntry,
                    RepositoryNum,
                    (x) => OnRemoteNodeConnected(x, false),
                    (x, g) => OnRemoteNodeDisconnected(x, false, g),
                    CloudNodeType.Server);
            }
            else
            {
                _externalCommunicationNode = new CommunicationNode(this,
                    0,
                    IPAddress.Any,
                    RepositoryNum,
                    (x) => OnRemoteNodeConnected(x, true),
                    (x, g) => OnRemoteNodeDisconnected(x, true, g),
                    CloudNodeType.Client);
            }

            PeriodicСheckClearOldUsages();
        }

        private void InitExternalCommNode()
        {
            (var externalBindAddress, _) = ConfigureAddresses(_config.Addresses.Target?.ExternalBindAddress, _config.Ports.Target.ExternalBindPort, _config.Addresses.Target?.ExternalAnnounceAddress, _config.Ports.Target.ExternalAnnouncePort);

            var hostEntry = NetUtils.ResolveAddress(externalBindAddress.Address);
            _externalCommunicationNode?.Dispose();
            _externalCommunicationNode = new CommunicationNode(this,
                _config.Ports.Target.ExternalBindPort,
                hostEntry,
                RepositoryNum,
                (x) => OnRemoteNodeConnected(x, true),
                (x, graceful) => OnRemoteNodeDisconnected(x, true, graceful),
                CloudNodeType.Client);
        }

        protected async Task LoadDefaultServices()
        {
            var services = ReplicaTypeRegistry.GetServiceEntitiesForNodeType(_cloudNodeType);

            foreach (var service in services)
            {
                await CreateServiceEntity(service, null, null);
            }
        }

        protected async Task CreateServiceEntity(Type type, CloudSharedDataConfig dataConfig, CustomConfig customConfig)
        {
            var serviceEntityRef = await Create(GetIdByType(type), this.Id, async (entity) =>
            {
                var loadEntity = entity as IHasLoadFromJObject;
                if (loadEntity != null)
                    await loadEntity.Load(dataConfig, customConfig, this);
            });
            Logger.IfDebug()?.Message("Repository: {repo_id} service entity created {entity_type}", Id, serviceEntityRef.TypeName).Write();

            Func<CloudNodeType, bool> checkNeedReplicateFn = ReplicaTypeRegistry.NeedReplicateEntityTypeTo(type);
            //Logger.IfError()?.Message($"Created service entity {Id} {type.Name}").Write();
            using (var ent = await GetExclusive(GetIdByType(type), this.Id, nameof(CreateServiceEntity)))
            {
                await SubscribeServiceToExistingConnections(serviceEntityRef, checkNeedReplicateFn);
                _serviceEntities.Add(serviceEntityRef);
            }
        }

        protected async Task SubscribeServiceToExistingConnections(IEntityRef service,
            Func<CloudNodeType, bool> checkNeedReplicate)
        {
            foreach (var repoCon in GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)))
            {
                if (repoCon.Value.Id == Id)
                    continue;
                var entity = (IRepositoryCommunicationEntity)((IEntityRefExt)repoCon.Value).GetEntity();
                var serviceE = ((IEntityRefExt)service).GetEntity();
                if (checkNeedReplicate(entity.CloudNodeType))
                {
                    if (!((BaseEntity)serviceE).IsReplicatedTo(repoCon.Value.Id))
                    {
                        await SubscribeReplication(service.TypeId, service.Id, repoCon.Value.Id,
                            ReplicationLevel.Server);
                    }
                }
            }
        }

        protected async Task SubscribeServicesToNewConnection(CloudNodeType nodeType, Guid id)
        {
            foreach (var service in _serviceEntities)
            {
                var needRepl =
                    ReplicaTypeRegistry.NeedReplicateEntityTypeTo(ReplicaTypeRegistry.GetTypeById(service.TypeId));
                if (!needRepl(nodeType))
                    continue;

                using (var ent = await GetExclusive(service.TypeId, this.Id, nameof(SubscribeServicesToNewConnection)))
                {
                    var serviceE = ((IEntityRefExt)service).GetEntity();
                    if (!((BaseEntity)serviceE).IsReplicatedTo(id))
                    {
                        await SubscribeReplication(service.TypeId, service.Id, id, ReplicationLevel.Server);
                    }
                }
            }
        }

        public bool IsCloudEntryPoint
        {
            get
            {
                if (_cloudNodeType != CloudNodeType.Server)
                    return false;
                var cloudEntryPoint = _sharedConfig.CloudEntryPoint;
                var address = GetAddressForHost(cloudEntryPoint.Target.Host);
                var port = cloudEntryPoint.Target.Port;
                return port == _internalCommunicationNode.Port && NetworkUtils.IsLocalIp(address);
            }
        }

        public async Task ConnectToCluster(CancellationToken ct)
        {
            try
            {
                if (_cloudNodeType == CloudNodeType.Server)
                {
                    if (!IsCloudEntryPoint)
                    {
                        var cloudEntryPoint = _sharedConfig.CloudEntryPoint;
                        var address = GetAddressForHost(cloudEntryPoint.Target.Host);
                        var port = cloudEntryPoint.Target.Port;
                        if (port == _internalCommunicationNode.Port && NetworkUtils.IsLocalIp(address))
                            throw new InvalidOperationException("No, it is an entry point somehow");

                        var endpoint = new IPEndPoint(address, port);
                        await _internalCommunicationNode.ConnectNode(endpoint, ct);
                    }

                    await CheckClusterIsReady(ct);
                }
                else
                {
                    var address = GetAddressForHost(_sharedConfig.ClientEntryPoint.Target.Host);
                    var port = _sharedConfig.ClientEntryPoint.Target.Port;
                    var endpoint = new IPEndPoint(address, port);
                    await SharedCode.Utils.Threads.TaskEx.Run(() => _externalCommunicationNode.ConnectNode(endpoint, ct));
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "ConnectToCluster exception").Write();;
                throw;
            }
        }

        ISerializer IEntitiesRepositoryExtension.Serializer => _serializer;

        IEntitySerializer IEntitiesRepositoryExtension.EntitySerializer => _entitySerializer;

        IEntityRef IEntitiesRepositoryExtension.GetRef<T>(Guid id)
        {
            var result = GetInternal(ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(typeof(T)), id);
            return result;
        }

        IEntityRef IEntitiesRepositoryExtension.GetRef(int typeId, Guid id)
        {
            var type = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(ReplicaTypeRegistry.GetTypeById(typeId));
            var result = GetInternal(type, id);
            return result;
        }

        RepositoryEntityContainsStatus IEntitiesRepositoryExtension.GetRepositoryEntityContainsStatus(int typeId,
            Guid entityId)
        {
            var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
            if (entityRef == null)
                return RepositoryEntityContainsStatus.NotContains;
            var entity = (BaseEntity)((IEntityRefExt)entityRef).GetEntity();
            return entity.IsMaster()
                ? RepositoryEntityContainsStatus.Master
                : RepositoryEntityContainsStatus.Replication;
        }

        IEntityRef IEntitiesRepositoryExtension.GetRef(Type type, Guid id)
        {
            var result = GetInternal(ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type), id);
            return result;
        }

        IEntityRef IEntitiesRepositoryExtension.GetServiceRef<T>()
        {
            var type = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(typeof(T));
            if (!Attribute.IsDefined(type, typeof(EntityServiceAttribute)))
                throw new InvalidOperationException($"Type {typeof(T)} is not a service entity type");

            var collection = GetEntitiesCollection(type);
            return collection.Values.FirstOrDefault();
        }

        public IEntityRef CheckAndGetSubscriberRepositoryCommunicationRef(Guid subscriberRepositoryId,
            int subscribedEntityTypeId, Guid subscribedEntityId)
        {
            var entityRef =
                ((IEntitiesRepositoryExtension)this).GetRef<IRepositoryCommunicationEntity>(subscriberRepositoryId);
            if (entityRef == null)
            {
                AsyncUtils.RunAsyncTask(async () =>
                {
                    var batch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusive(subscribedEntityTypeId,
                        subscribedEntityId);
                    using (var wrapper = await this.get(batch, checkExclusiveOnReplication: false))
                    {
                        var entity = wrapper.Get<IEntity>(subscribedEntityTypeId, subscribedEntityId);
                        if (entity == null)
                        {
                            Logger.Warn(
                                "CheckAndGetSubscriberRepositoryCommunicationRef unsubscribe not found entity {0} id {1}",
                                GetTypeById(subscribedEntityTypeId).Name, subscribedEntityId);
                            return;
                        }

                        ((IEntityExt)entity).UnsubscribeDisconnectedRepository(subscriberRepositoryId);
                    }
                });
            }

            return entityRef;
        }

        public bool CheckRemoteEntity(IEntity entity)
        {
            if (!((IEntityExt)entity).IsMaster())
            {
                var networkProxy = getNetworkProxyByNodeId(((IEntityExt)entity).OwnerNodeId);
                if (networkProxy == null)
                    Logger.Error("Repository {0}: Not found remote node connection for entity {1}", this.Id,
                        entity.ToString());
                ((IRemoteEntity)entity).SetNetworkProxy(networkProxy);
                return true;
            }

            return false;
        }

        public Task EntityDestroyed(DestroyBatchContainer destroyBatchContainer)
        {
            if (destroyBatchContainer.Batches != null)
            {
                foreach (var db in destroyBatchContainer.Batches)
                {
                    var destroyBatch = db;
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        IEntityRef destroyRef = null;
                        var batch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusive(destroyBatch.EntityTypeId,
                            destroyBatch.EntityId);
                        using (var wrapper = await this.get(batch, checkExclusiveOnReplication: false))
                        {
                            if (wrapper == null)
                                return;

                            destroyRef =
                                ((IEntitiesContainerExtension)wrapper).GetEntityRef(destroyBatch.EntityTypeId,
                                    destroyBatch.EntityId);
                            if (destroyRef != null)
                            {
                                TryProcessDeltaSnapshots(destroyRef);
                                RemoveInternal(GetMasterTypeIdByReplicationLevelType(destroyBatch.EntityTypeId),
                                    destroyBatch.EntityId);
                            }
                            else
                                Logger.Warn("Destroy entity not found typeId {0} entityId {1}",
                                    GetTypeById(destroyBatch.EntityTypeId).Name, destroyBatch.EntityId);
                        }

                        if (destroyRef != null)
                            await OnEntityUnload(destroyRef);
                    }, this);
                }
            }

            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            try
            {
                foreach (var s in _serviceEntities)
                {
                    if (s.TypeId == GetIdByType(typeof(IClusterAddressResolverServiceEntity)))
                        continue;
                    await Destroy(s.TypeId, s.Id, true);
                }

                foreach (var ent in GetAllEntityDebug())
                {
                    if (ent.TypeId == _repoComRef.TypeId)
                        continue;
                    if (ent.TypeId == GetIdByType(typeof(IClusterAddressResolverServiceEntity)))
                        continue;
                    if (((IEntityRefExt)ent).GetEntity().IsMaster())
                    {
                        Logger.Error(
                            $"master {ent.TypeName} {ent.Id} is still here {_config.ConfigId} {CloudNodeType}, forcing unload");
                        await Destroy(ent.TypeId, ent.Id, true);
                    }
                }

                foreach (var repo in GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)))
                {
                    if (repo.Key == Id)
                        continue;
                    using (var repoComW = await Get(repo.Value.TypeId, repo.Value.Id))
                    {
                        await repoComW.Get<IRepositoryCommunicationEntityServer>(repo.Value.Id).FireOnDisconnect();
                    }
                }
            }
            finally
            {
                _stopTokenSource.Cancel();
                _internalCommunicationNode?.Dispose();
                _internalCommunicationNode = null;
                _externalCommunicationNode?.Dispose();
                _externalCommunicationNode = null;
            }

            foreach (var ent in GetAllEntityDebug())
            {
                if (ent.TypeId == _repoComRef.TypeId)
                    continue;
                if (ent.TypeId == GetIdByType(typeof(IClusterAddressResolverServiceEntity)))
                    continue;
                if (((IEntityRefExt)ent).GetEntity().IsMaster())
                {
                    Logger.Error(
                        $"Master {ent.TypeName} {ent.Id} is still here after everything was disconnected {_config.ConfigId} {CloudNodeType}, forcing detroy this is BAD");
                    await Destroy(ent.TypeId, ent.Id, true);
                }
                else
                {
                    Logger.Error(
                        $"Replica {ent.TypeName} {ent.Id} is still here after everything was disconnected {_config.ConfigId} {CloudNodeType}, forcing unload");
                    await OnEntityUnload(ent);
                    RemoveInternal(ent.TypeId, ent.Id);
                }
            }
        }

        public Task<bool> Destroy<T>(Guid entityId, bool unload = false) where T : IEntity
        {
            return DestroyInternal<T>(entityId, unload);
        }

        async Task<bool> DestroyInternal<T>(Guid entityId, bool unload = false) where T : IEntity
        {
            var typeId = GetIdByType(typeof(T));
            return await DestroyInternal(typeId, entityId, unload);
        }

        public Task<bool> Destroy(int typeId, Guid entityId, bool unload = false)
        {
            var eref = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
            if (eref == null)
                return Task.FromResult(true);
            lock (eref)
            {
                var dt = ((IEntityRefExt)eref).Destroying;
                if (dt == null)
                {
                    var t = DestroyInternal(typeId, entityId, unload);
                    ((IEntityRefExt)eref).Destroying = t;
                    return t;
                }
                else
                    return (Task<bool>)dt;
            }
        }

        async Task<bool> DestroyInternal(int typeId, Guid entityId, bool unload)
        {
            if (typeId == GetIdByType(typeof(IClusterAddressResolverServiceEntityServer)))
                Logger.IfError()?.Message("DESTROYING IClusterAddressResolverServiceEntityServer").Write();

            var entityExts =
                ((IEntityExt) ((IEntityRefExt) ((IEntitiesRepositoryExtension) this).GetRef(typeId, entityId))
                    ?.GetEntity());
            if (entityExts == null)
                return true;

            entityExts.SetState(EntityState.Destroying);

        

            var linkedEntitiesList = _linkedEntitiesPool.Take();
            try
            {
                var batch = ((IEntityBatchExt) EntityBatch.Create()).AddExclusive(typeId, entityId);
                using (var wrapper = await get(batch, checkExclusiveOnReplication: false))
                {
                    var entityRef = wrapper.GetEntityRef(typeId, entityId);
                    if (entityRef == null)
                    {
                        Logger.Error("Destroy entity not found. typeId {0} {2} entityId {1}", typeId, entityId,
                            GetTypeById(typeId));
                        return false;
                    }

                    if (!entityRef.ContainsReplicationLevel(ReplicationLevel.Master))
                    {
                        Logger.Error("Destroy entity is not master. typeId {0} {2} entityId {1}", typeId, entityId,
                            GetTypeById(typeId));
                        return false;
                    }

                    var entity = wrapper.Get<IEntity>(typeId, entityId);
                    var entityExt = (IEntityExt) entity;

               
                    entityExt.FireOnReplicationLevelChanged((long)ReplicationLevel.Master, 0);
                    await RemoveEntity(unload, entityExts);
                

                    await entityExt.CancellAllChainCalls();
                    await UnsubscribeReplicationBatch(typeId, entityId,
                        entityExt.ReplicateTo().SelectMany(
                                x => x.Value.SubscribeIds.SelectMany(
                                    y => Enumerable.Repeat(
                                        new KeyValuePair<Guid, ReplicationLevel>(x.Key, (ReplicationLevel) y.Key),
                                        y.Value)))
                            .ToList());

                    //foreach (var replicateTo in entityExt.ReplicateTo())
                    //    await

                    entityExt.SetState(EntityState.Destroyed);
                    entity.GetAllLinkedEntities((long) ReplicationLevel.Master, linkedEntitiesList, 0, false);

                    await afterDestroyEntity(entityRef, unload);
                    Statistics<EntityDestroyedStatistics>.Instance.Destroyed(entity.GetType());

                    if (_cloudNodeType == CloudNodeType.Server && !IsServiceEntityType(typeId) && !StopToken.IsCancellationRequested)
                        _ = AsyncUtils.RunAsyncTask(async () =>
                        {
                            using (var wrapper2 = await this.GetFirstService<IClusterAddressResolverServiceEntityServer>())
                            {
                                if (wrapper2 == null)
                                {
                                    Logger.IfError()?.Message("Destroy IClusterAddressResolverServiceEntityServer not found").Write();
                                    return;
                                }

                                var addressResolver = wrapper2.GetFirstService<IClusterAddressResolverServiceEntityServer>();
                                if (_connectedRepos.ContainsKey(addressResolver.OwnerRepositoryId))
                                    await addressResolver.RemoveEntityAddressRepositoryId(typeId, entityId);
                            }
                        });
                }

                var distinctLinkedEntities = new HashSet<IEntityRef>();
                foreach (var linkedEntityInfo in linkedEntitiesList)
                {
                    if (distinctLinkedEntities.Add(linkedEntityInfo.entityRef))
                    {
                        await DestroyInternal(linkedEntityInfo.entityRef.TypeId, linkedEntityInfo.entityRef.Id, unload);
                    }
                }

                return true;
            }
            finally
            {
                _linkedEntitiesPool.Return(linkedEntitiesList);
            }
        }

        private static async Task RemoveEntity(bool unload, IEntityExt entityExts)
        {
            if (unload)
            {
                if (ServerCoreRuntimeParameters.CollectEntityLifecycleHistory)
                {
                    Logger.IfInfo()?.Message("Entity is unloading {type} {entityId}", ReplicaTypeRegistry.GetTypeById(((IEntity) entityExts).TypeId),
                        ((IEntity) entityExts).Id).Write();
                }

                await ((BaseEntity) entityExts).FireOnUnload();
            }
            else
            {
                if (ServerCoreRuntimeParameters.CollectEntityLifecycleHistory)
                {
                    Logger.IfInfo()?.Message("Entity is destroing {type} {entityId}", ReplicaTypeRegistry.GetTypeById(((IEntity)entityExts).TypeId), ((IEntity)entityExts).Id).Write();
                }

                await ((BaseEntity) entityExts).FireOnDestroy();
            }
        }

        public ValueTask<IEntitiesContainer> GetMasterService<T>([CallerMemberName] string callerTag = null)
            where T : IEntity
        {
            return Get<T>(Id, callerTag);
        }

        public ValueTask<IEntitiesContainer> GetMasterService(Type type, object tag, [CallerMemberName] string callerTag = null)
        {
            return Get(type, Id, tag, callerTag);
        }

        public ValueTask<IEntitiesContainer> GetMasterService(Type type, [CallerMemberName] string callerTag = null)
        {
            return Get(type, Id, callerTag);
        }

        public ValueTask<IEntitiesContainer> GetMasterService<T>(object tag, [CallerMemberName] string callerTag = null) where T : IEntity
        {
            return Get<T>(Id, tag, callerTag);
        }

        public ValueTask<IEntitiesContainer> GetFirstService<T>([CallerMemberName] string callerTag = null)
            where T : IEntity
        {
            var type = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(typeof(T));
            if (!Attribute.IsDefined(type, typeof(EntityServiceAttribute)))
                throw new InvalidOperationException($"Type {typeof(T)} is not a service entity type");

            var collection = GetEntitiesCollection(type);
            var service = collection.Values.FirstOrDefault();

            if (service == null)
                return default;
            return Get<T>(service.Id, callerTag);
        }

        public ValueTask<IEntitiesContainer> GetFirstService(Type type, [CallerMemberName] string callerTag = null)
        {
            type = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            if (!Attribute.IsDefined(type, typeof(EntityServiceAttribute)))
                throw new InvalidOperationException($"Type {type} is not a service entity type");

            var collection = GetEntitiesCollection(type);
            var service = collection.Values.FirstOrDefault();

            if (service == null)
                return default;
            var id = ReplicaTypeRegistry.GetIdByType(type);
            return Get(id, service.Id, callerTag);
        }

        public ValueTask<IEntitiesContainer> GetFirstService<T>(object tag, [CallerMemberName] string callerTag = null) where T : IEntity
        {
            var type = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(typeof(T));
            if (!Attribute.IsDefined(type, typeof(EntityServiceAttribute)))
                throw new InvalidOperationException($"Type {typeof(T)} is not a service entity type");

            var collection = GetEntitiesCollection(type);
            var service = collection.Values.FirstOrDefault();
            if (service == null)
                return default;
            return Get<T>(service.Id, tag, callerTag);
        }

        public void OnRemoteNodeConnected(INetworkProxy networkProxy, bool external)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                var typeId = GetIdByType(typeof(IRepositoryCommunicationEntity));
                var remoteRepositoryId = networkProxy.RemoteRepoId;
                var nodeEndpoint = networkProxy.RemoteEndpoint;
                var remoteProxy = new RepositoryCommunicationEntity(remoteRepositoryId);
                ((IEntityExt)remoteProxy).SetEntitiesRepository(this);
                ((IRemoteEntity)remoteProxy).SetNetworkProxy(networkProxy);
                ((IEntityExt)remoteProxy).SetOwnerNodeId(remoteRepositoryId);

                if (((IEntitiesRepositoryExtension)this).GetRef<IRepositoryCommunicationEntity>(remoteRepositoryId) ==
                    null)
                    SetTemporalyRemoteRepositoryCommunicationEntity(remoteProxy, external);

                if (this.Id == remoteRepositoryId)
                    throw new InvalidOperationException($"Self connection {this.Id}");
                await SubscribeReplication(typeId, this.Id, remoteRepositoryId, ReplicationLevel.Server);

                if (!external)
                {
                    OnInternalRemoteNodeConnected(remoteRepositoryId);
                }

                Logger.IfInfo()?.Message("{0} New node connected host {1} port {2} external {3} repositoryId {4}", Id,
                    nodeEndpoint.Address, nodeEndpoint.Port, external, remoteRepositoryId).Write();
            });
        }

        private void OnInternalRemoteNodeConnected(Guid repositoryId)
        {
            if (CloudNodeType == CloudNodeType.Client)
                return;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await Get<IRepositoryPingEntity>(Id))
                {
                    var communicationEnity = wrapper.Get<IRepositoryPingEntity>(Id);
                    var pingDuration = 10f;
                    checkStopPingRepository(repositoryId);
                    var token = communicationEnity.Chain().Delay(30).Delay(pingDuration, true)
                        .PingRepository(repositoryId).Run();
                    _pingChainCalls[repositoryId] = token;
                    Logger.IfInfo()?.Message("{0} Starting pinging repository {1} duration {2:0.####} seconds",
                        communicationEnity.ToString(), repositoryId.ToString(), pingDuration.ToString()).Write();
                }
            }, this);
        }

        private void checkStopPingRepository(Guid repositoryId)
        {
            ChainCancellationToken token;
            if (_pingChainCalls.TryRemove(repositoryId, out token))
            {
                Logger.IfInfo()?.Message("Repository {0} stopping pinging repository {1}", Id, repositoryId).Write();
                token.Cancel(this);
            }
        }

        public IRepositoryCommunicationEntity GetRepositoryCommunicationEntityByNetworkProxy(INetworkProxy networkProxy)
        {
            var entityRef = GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)).FirstOrDefault(x =>
                !((IEntityExt)((IEntityRefExt)x.Value).GetEntity()).IsMaster()
                && ((IRemoteEntity)((IEntityRefExt)x.Value).GetEntity()).GetNetworkProxy() == networkProxy);
            if (entityRef.Value == null)
            {
                Logger.Error(
                    "Not found IRepositoryCommunicationEntity for networkProxy"); //TODO придумать какой-нить ChannelId
                return null;
            }

            return (IRepositoryCommunicationEntity)((IEntityRefExt)entityRef.Value).GetEntity();
        }

        public void OnRemoteNodeDisconnected(INetworkProxy networkProxy, bool external, bool graceful)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                var repositoryEntityRef = GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)).FirstOrDefault(
                    x => !((IEntityExt)((IEntityRefExt)x.Value).GetEntity()).IsMaster()
                         && ((IRemoteEntity)((IEntityRefExt)x.Value).GetEntity()).GetNetworkProxy() == networkProxy);
                if (repositoryEntityRef.Value == null)
                {
                    var proxyEndpoint = networkProxy.RemoteEndpoint;
                    Logger.Error(
                        "OnRemoteNodeDisconnected host {0} port {1} not contains repositoryCommunicationEntity",
                        proxyEndpoint.Address, proxyEndpoint.Port);
                    return;
                }

                var remoteRefId = repositoryEntityRef.Key;
                if (!external && _cloudNodeType == CloudNodeType.Server && !graceful)
                {
                    IPEndPoint endpoint = null;
                    EndpointAddress host;
                    using (var wrapper = await Get<IRepositoryCommunicationEntityServer>(remoteRefId))
                    {
                        var repositoryEntity = wrapper.Get<IRepositoryCommunicationEntityServer>(remoteRefId);
                        if (repositoryEntity == null)
                            return;

                        host = repositoryEntity.InternalAddress;
                        if (!string.IsNullOrWhiteSpace(host.Address))
                        {
                            var ipAddress = NetUtils.ResolveAddress(host.Address);
                            endpoint = new IPEndPoint(ipAddress, host.Port);
                        }
                        else
                            Logger.IfError()?.Message("Remote node {node_id} internal address is not set, assuming disconnect during connection process", remoteRefId).Write();
                    }

                    _ = AsyncUtils.RunAsyncTask(async () =>
                    {
                        try
                        {
                            await Task.Delay(
                                TimeSpan.FromSeconds(ServerCoreRuntimeParameters
                                    .CloudNodeReconnectDelaySecondsAfterDisconnecting), this.StopToken);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }

                        if (this.StopToken.IsCancellationRequested)
                            return;

                        Logger.IfInfo()?.Message("Repository {0} try reconnecting to server node {1} Id {2}", Id, host, remoteRefId).Write();
                        try
                        {
                            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                            await _internalCommunicationNode.ConnectNode(endpoint, cts.Token);
                        }
                        catch (Exception e)
                        {
                            Logger.IfError()?.Message(e, "Exception on reconnect repository {0} to {1} id {2}", Id, host, remoteRefId).Write();
                        }
                    });
                }

                checkStopPingRepository(remoteRefId);

                var batch = ((IEntityBatchExt) EntityBatch.Create()).AddExclusive(repositoryEntityRef.Value.TypeId, repositoryEntityRef.Value.Id);
                using (var wrapper = await get(batch, false))
                {
                    RemoveInternal(repositoryEntityRef.Value.TypeId, remoteRefId);
                }

                try
                {
                    await OnEntityUnload(repositoryEntityRef.Value);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message("Exception in OnEntityUnload while removing IRepositoryCommunicationEntity {repositoryId}",
                        repositoryEntityRef.Key).Exception(e).Write();
                }
                
                foreach (var ent in GetAllEntityDebug())
                {
                    if (ent.TypeId == GetIdByType(typeof(IClusterAddressResolverServiceEntity)))
                        continue;
                    if (((IEntityRefExt)ent).GetEntity().OwnerRepositoryId == remoteRefId)
                    {
                        try
                        {
                            Logger.IfError()?.Message($"Forcefully removed entity on hard disconnect {ent.TypeName} {ent.Id}").Write();
                            await OnEntityUnload(ent);
                            RemoveInternal(ent.TypeId, ent.Id);
                        }
                        catch (Exception e)
                        {
                            Logger.IfError()?.Message("Exception in OnEntityUnload while removing {entityType} {entityId} for {repositoryId}",
                                ent.TypeName, ent.Id, repositoryEntityRef.Key)
                                .Exception(e).Write();
                        }
                    }
                    else
                    {
                        //очищаем из списков репликаций отключенную ноду
                        var replicateRepositoryIds =
                            ((IEntityExt)((IEntityRefExt)ent).GetEntity()).ReplicateRepositoryIds;
                        replicateRepositoryIds.TryRemove(remoteRefId, out _);
                    }
                }
            });
        }

        public void NewInternalNodeAddedToCluster(List<EndpointAddress> knownNodes)
        {
            if (_cloudNodeType != CloudNodeType.Server)
                throw new InvalidOperationException("Not supposed to be called on client");

            AsyncUtils.RunAsyncTask(async () =>
            {
                foreach (var node in knownNodes)
                {
                    var nodeIpAddress = NetUtils.ResolveAddress(node.Address);
                    var endpoint = new IPEndPoint(nodeIpAddress, node.Port);
                    await _internalCommunicationNode.ConnectNode(endpoint, CancellationToken.None);
                }
            });
        }

        INetworkProxy getNetworkProxyByNodeId(Guid nodeId)
        {
            var entityRef = ((IEntitiesRepositoryExtension)this).GetRef<IRepositoryCommunicationEntity>(nodeId);
            if (entityRef == null)
                return null;

            return ((IRemoteEntity)((IEntityRefExt)entityRef).GetEntity()).GetNetworkProxy();
        }

        protected virtual void OnNewEntityUploaded(IEntityRef entityRef)
        {
            if (NewEntityUploaded != null)
            {
                var typeId = entityRef.TypeId;
                var entintyId = entityRef.Id;

                var invocationList = NewEntityUploaded.GetInvocationList();
                if (invocationList.Length > ServerCoreRuntimeParameters.RepositoryEventSubscribersWarnCount)
                    Logger.IfError()?.Message("NewEntityUploaded too many subscribers {0}", invocationList.Length).Write();

                foreach (var subscriber in invocationList.Cast<Func<int, Guid, Task>>())
                {
                    var subscriberCopy = subscriber;
                    AsyncUtils.RunAsyncTask(() =>
                        AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(typeId, entintyId),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () => $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    );
                }
            }
        }

        protected void OnEntityUpdated(IEntityRef entityRef)
        {
            if (EntityUpdated != null)
            {
                var typeId = entityRef.TypeId;
                var entintyId = entityRef.Id;

                var invocationList = EntityUpdated.GetInvocationList();
                if (invocationList.Length > ServerCoreRuntimeParameters.RepositoryEventSubscribersWarnCount)
                    Logger.IfError()?.Message("EntityUpdated too many subscribers {0}", invocationList.Length).Write();

                foreach (var subscriber in invocationList.Cast<Func<int, Guid, Task>>())
                {
                    var subscriberCopy = subscriber;
                    AsyncUtils.RunAsyncTask(() =>                    
                        AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(typeId, entintyId),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () => $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    );
                }
            }
        }

        ConcurrentDictionary<Guid, bool> _connectedRepos = new ConcurrentDictionary<Guid, bool>();

        protected async Task OnEntityUnload(IEntityRef entityRef)
        {
            //Logger.IfError()?.Message($"Unloaded {entityRef.TypeName} {entityRef.Id} here {CloudNodeType}").Write();
            var typeId = entityRef.TypeId;
            var entintyId = entityRef.Id;
            var ext = (IEntityRefExt)entityRef;
            var entity = ext.GetEntity();
            ((IEntityExt)entity).SetState(EntityState.Destroyed);
            var ownerNode = ((IEntityRefExt)entityRef).GetEntity().OwnerRepositoryId;
            if (entityRef.TypeId == GetIdByType(typeof(IRepositoryCommunicationEntity)) && ownerNode != Id &&
                _connectedRepos.TryRemove(ownerNode, out var _))
            {
                await OnUserDisconnected(ownerNode);
            }


            var tasks = new List<SuspendingAwaitable>
            {
                AsyncUtils.RunAsyncTask(
                    () => ext.CallOnDestroyOrUnload(typeId, entintyId, entity, this))
            };

            if (EntityUnloaded != null)
            {
                var invocationList = EntityUnloaded.GetInvocationList();
                if (invocationList.Length > ServerCoreRuntimeParameters.RepositoryEventSubscribersWarnCount)
                    Logger.IfError()?.Message("EntityUnloaded too many subscribers {0}", invocationList.Length).Write();

                foreach (var subscriber in invocationList.Cast<Func<int, Guid, bool, IEntity, Task>>())
                {
                    var subscriberCopy = subscriber;
                    tasks.Add(AsyncUtils.RunAsyncTask(
                        () =>
                            AsyncUtils.RunAsyncWithCheckTimeout(
                                () => subscriberCopy(typeId, entintyId, false, entity),
                                ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                                () => $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                        ));
                }
            }

            await SuspendingAwaitable.WhenAll(tasks);
        }

        protected async Task OnEntityDestroy(IEntityRef entityRef)
        {
            var typeId = entityRef.TypeId;
            var entintyId = entityRef.Id;
            var ext = (IEntityRefExt)entityRef;
            var entity = ext.GetEntity();

            var tasks = new List<SuspendingAwaitable>();

            tasks.Add(AsyncUtils.RunAsyncTask(() =>
                ext.CallOnDestroyOrUnload(typeId, entintyId, entity, this)));

            if (EntityDestroy != null)
            {
                var invocationList = EntityDestroy.GetInvocationList();
                if (invocationList.Length > ServerCoreRuntimeParameters.RepositoryEventSubscribersWarnCount)
                    Logger.IfError()?.Message("OnEntityDestroy too many subscribers {0}", invocationList.Length).Write();

                foreach (var subscriber in invocationList.Cast<Func<int, Guid, IEntity, Task>>())
                {
                    var subscriberCopy = subscriber;
                    tasks.Add(AsyncUtils.RunAsyncTask(() =>
                    {
                        return AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(typeId, entintyId, entity),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () =>
                                $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}");
                    }));
                }
            }

            await SuspendingAwaitable.WhenAll(tasks);
        }

        protected async Task OnUserDisconnected(Guid repoId)
        {
            if (UserDisconnected != null)
            {
                var invocationList = UserDisconnected.GetInvocationList();
                if (invocationList.Length > ServerCoreRuntimeParameters.RepositoryEventSubscribersWarnCount)
                    Logger.IfError()?.Message("UserDisconnected too many subscribers {0}", invocationList.Length).Write();

                foreach (var subscriber in invocationList.Cast<UserDisconnectedDelegate>())
                {
                    var subscriberCopy = subscriber;
                    await AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(repoId),
                        ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                        () =>
                            $"OnUserDisconnected repoId={repoId} obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}");
                }
            }
        }

        protected void OnEntityCreated(IEntityRef entityRef)
        {
            if (EntityCreated != null)
            {
                var typeId = entityRef.TypeId;
                var entintyId = entityRef.Id;
                foreach (var subscriber in EntityCreated.GetInvocationList().Cast<Func<int, Guid, Task>>())
                {
                    var subscriberCopy = subscriber;
                    AsyncUtils.RunAsyncTask(() =>                    
                        AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(typeId, entintyId),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () => $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    );
                }
            }
        }

        protected void OnEntityLoaded(IEntityRef entityRef)
        {
            if (EntityLoaded != null)
            {
                var typeId = entityRef.TypeId;
                var entintyId = entityRef.Id;
                foreach (var subscriber in EntityLoaded.GetInvocationList().Cast<Func<int, Guid, Task>>())
                {
                    var subscriberCopy = subscriber;
                    AsyncUtils.RunAsyncTask(() =>
                        AsyncUtils.RunAsyncWithCheckTimeout(() => subscriberCopy(typeId, entintyId),
                            ServerCoreRuntimeParameters.EntityEventTimeoutSeconds,
                            () => $"obj {subscriberCopy.Target?.GetType().Name ?? "unknown"} method {subscriberCopy.Method.Name}")
                    );
                }
            }
        }

        private void OnEntityUploaded(IEntityRef entityRef)
        {
            if (entityRef.Id == this.Id)
                throw new InvalidOperationException($"OnEntityUploaded IRepositoryCommunicationEntity self. Id {this.Id}");
            if (entityRef.TypeId == GetIdByType(typeof(IRepositoryCommunicationEntity)))
            {
                var newNodeId = entityRef.Id;
                AsyncUtils.RunAsyncTask(() => NotifyNewNodeOfExistingConnections(newNodeId));
            }

            OnNewEntityUploaded(entityRef);
        }

        private async Task NotifyNewNodeOfExistingConnections(Guid newNodeId)
        {
            using (var wrapper = await Get<IRepositoryCommunicationEntityServer>(newNodeId))
            {
                var entity = wrapper.Get<IRepositoryCommunicationEntityServer>(newNodeId);
                _connectedRepos.TryAdd(entity.OwnerRepositoryId, true);
                await SubscribeServicesToNewConnection(entity.CloudNodeType, entity.Id);

                if (entity.CloudNodeType != CloudNodeType.Server || _cloudNodeType != CloudNodeType.Server || !IsCloudEntryPoint)
                    return;

                using (var currentCommunicationEntityWrapper = await Get<IRepositoryCommunicationEntity>(_repoComRef.Id))
                {
                    var currentCommunicationEntity = currentCommunicationEntityWrapper.Get<IRepositoryCommunicationEntity>(_repoComRef.Id);
                    await currentCommunicationEntity.NewNodeConnected(newNodeId);
                }
            }
        }

        protected void afterCreateEntity(IEntityRef entityRef)
        {
            OnEntityCreated(entityRef);
        }

        private Task afterDestroyEntity(IEntityRef entityRef, bool unload)
        {
            if (unload)
                return OnEntityUnload(entityRef);
            else
                return OnEntityDestroy(entityRef);
        }

        public void uploadCreatedEntities(ref EntityCollections collections)
        {
            var uploadContainers = collections.UploadContainers;
            if (uploadContainers != null)
            {
                foreach (var pair in uploadContainers)
                {
                    if (pair.Value.UploadContainer.Batches.Count > 0)
                    {
                        if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                        {
                            foreach (var uploadBatch in pair.Value.UploadContainer.Batches)
                            {
                                Logger.IfInfo()?.Message(
                                    "ReceiverRepositoryId={ReceiverRepositoryId} Uploading entityTypeId={entityTypeId} entityId={entityId} replicationMask={replicationMask} version={version}",
                                    pair.Value.CommunicationEntity.Id, uploadBatch.EntityTypeId, uploadBatch.EntityId, uploadBatch.ReplicationMask, uploadBatch.Version)
                                    .Write();
                            }
                        }
                        
                        pair.Value.CommunicationEntity.EntityUpload(pair.Value.UploadContainer).WrapErrors();
                    }
                }

                foreach (var pair in uploadContainers)
                {
                    if (pair.Value.UpdateContainer.Batches.Count > 0)
                    {
                        if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                        {
                            foreach (var updateBatch in pair.Value.UpdateContainer.Batches)
                            {
                                SubscriptionsLogger.IfInfo()?.Message(
                                    "ReceiverRepositoryId={ReceiverRepositoryId} Updating entityTypeId={entityTypeId} entityId={entityId} replicationMask={replicationMask} version={version}",
                                    pair.Value.CommunicationEntity.Id, updateBatch.EntityTypeId, updateBatch.EntityId, updateBatch.ReplicationMask, updateBatch.Version)
                                    .Write();
                            }
                        }
                        
                        pair.Value.CommunicationEntity.EntityUpdate(pair.Value.UpdateContainer).WrapErrors();
                    }
                }
            }
        }

        public void uploadDestroyEntities(ref EntityCollections collections)
        {
            var destroyContainers = collections.DestroyContainers;
            if (destroyContainers != null)
            {
                foreach (var pair in destroyContainers)
                {
                    if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                    {
                        foreach (var destroyBatch in pair.Value.DestroyContainer.Batches)
                        {
                            SubscriptionsLogger.IfInfo()?.Message(
                                "ReceiverRepositoryId={ReceiverRepositoryId} Destroing entityTypeId={entityTypeId} entityId={entityId}",
                                pair.Value.CommunicationEntity.Id, destroyBatch.EntityTypeId, destroyBatch.EntityId)
                                .Write();
                        }
                    }
                    
                    pair.Value.CommunicationEntity.EntityDestroyed(pair.Value.DestroyContainer).WrapErrors();
                }

                foreach (var pair in destroyContainers)
                {
                    if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                    {
                        foreach (var downgradeBatch in pair.Value.DowngradeContainer.Batches)
                        {
                            SubscriptionsLogger.IfInfo()?.Message(
                                "ReceiverRepositoryId={ReceiverRepositoryId} Downgrading entityTypeId={entityTypeId} entityId={entityId} replicationMask={DowngradeMask} version={version}",
                                pair.Value.CommunicationEntity.Id, downgradeBatch.EntityTypeId, downgradeBatch.EntityId, downgradeBatch.DowngradeMask, downgradeBatch.Version)
                                .Write();
                        }
                    }

                    pair.Value.CommunicationEntity.EntityDowngrade(pair.Value.DowngradeContainer).WrapErrors();
                }
            }
        }

        void IEntitiesRepositoryExtension.UpdateSubscriptions(
            int typeId, 
            Guid entityId, 
            Guid repositoryId,
            List<(bool subscribed, ReplicationLevel level)> subscriptions,
            Dictionary<Guid, List<DeferredEntityModel>> newLinkedEntities,
            ref EntityCollections collections)
        {
            if (subscriptions != null && subscriptions.Count > 0)
            {
                subscribeReplication(typeId, entityId, repositoryId, out var entityToUploadSerialized, out var entityToDestroy, subscriptions);
                
                if (entityToUploadSerialized != null)
                {
                    if (collections.UploadContainers == null)
                    {
                        collections.UploadContainers = new Dictionary<Guid, EntityCollections.UploadInfo>();
                    }

                    EntityCollections.UploadInfo uploadInfo;
                    if (!collections.UploadContainers.TryGetValue(repositoryId, out uploadInfo))
                    {
                        var repositoryRef = ((IEntitiesRepositoryExtension) this).CheckAndGetSubscriberRepositoryCommunicationRef(
                                repositoryId, typeId, entityId);
                        if (repositoryRef != null)
                        {
                            uploadInfo = new EntityCollections.UploadInfo(
                                (IRepositoryCommunicationEntity) ((IEntityRefExt) repositoryRef).GetEntity(),
                                new UploadBatchContainer(),
                                new UpdateBatchContainer());

                            collections.UploadContainers.Add(repositoryId, uploadInfo);
                        }
                        else
                        {
                            Logger.IfWarn()?.Message("Not found repository {0} for upload created entity type {1} id {2}", repositoryId, typeId, entityId).Write();
                        }
                    }

                    List<DeferredEntityModel> repositoryNewLinkedEntities = null;
                    newLinkedEntities?.TryGetValue(repositoryId, out repositoryNewLinkedEntities);
                    if (uploadInfo.CommunicationEntity != null)
                    {
                        if (entityToUploadSerialized.Value.Upload)
                        {
                            var bytesCount = entityToUploadSerialized.Value.Data.Sum(o => o.Value.Length);
                            Statistics<EntityUploadStatistics>.Instance.EntityUploadSend(
                                entityToUploadSerialized.Value.TypeId, bytesCount);
                            uploadInfo.UploadContainer.Batches.Add(new UploadBatch(entityToUploadSerialized.Value.TypeId,
                                entityToUploadSerialized.Value.EntityId,
                                entityToUploadSerialized.Value.Data,
                                entityToUploadSerialized.Value.UploadMask,
                                entityToUploadSerialized.Value.Version,
                                repositoryNewLinkedEntities));
                            if (ServerCoreRuntimeParameters.CollectReplicationHistory)
                                Logger.IfInfo()?.Message(
                                    $"UPLOAD from:{Id} to:{uploadInfo.CommunicationEntity.Id} type:{GetTypeById(entityToUploadSerialized.Value.TypeId)?.Name ?? "unknown"} id:{entityToUploadSerialized.Value.EntityId} mask:{entityToUploadSerialized.Value.UploadMask} version:{entityToUploadSerialized.Value.Version} length:{bytesCount}")
                                    .Write();
                        }
                        else
                        {
                            var bytesCount = entityToUploadSerialized.Value.Data.Sum(o => o.Value.Length);
                            Statistics<EntityUpdateStatistics>.Instance.EntityUpdateSend(
                                entityToUploadSerialized.Value.TypeId, bytesCount);
                            uploadInfo.UpdateContainer.Batches.Add(new UpdateBatch(
                                entityToUploadSerialized.Value.TypeId, 
                                entityToUploadSerialized.Value.EntityId, 
                                entityToUploadSerialized.Value.Data,
                                entityToUploadSerialized.Value.UploadMask,
                                entityToUploadSerialized.Value.Version,
                                entityToUploadSerialized.Value.SendedVersion,
                                repositoryNewLinkedEntities));
                            if (ServerCoreRuntimeParameters.CollectReplicationHistory)
                                Logger.IfInfo()?.Message(
                                    $"UPDATE from:{Id} to:{uploadInfo.CommunicationEntity.Id} type:{GetTypeById(entityToUploadSerialized.Value.TypeId)?.Name ?? "unknown"} id:{entityToUploadSerialized.Value.EntityId} mask:{entityToUploadSerialized.Value.UploadMask} version:{entityToUploadSerialized.Value.Version} previousVersion:{entityToUploadSerialized.Value.SendedVersion} length:{bytesCount}")
                                    .Write();
                        }
                    }
                }

                if (entityToDestroy != null)
                {
                    if (collections.DestroyContainers == null)
                    {
                        collections.DestroyContainers = new Dictionary<Guid, EntityCollections.DestroyInfo>();
                    }

                    EntityCollections.DestroyInfo destroyInfo;
                    if (!collections.DestroyContainers.TryGetValue(repositoryId, out destroyInfo))
                    {
                        var repositoryRef =
                            ((IEntitiesRepositoryExtension) this).CheckAndGetSubscriberRepositoryCommunicationRef(repositoryId, typeId, entityId);
                        if (repositoryRef != null)
                        {
                            destroyInfo = new EntityCollections.DestroyInfo(
                                (IRepositoryCommunicationEntity) ((IEntityRefExt) repositoryRef).GetEntity(),
                                new DestroyBatchContainer(),
                                new DowngradeBatchContainer());
                            collections.DestroyContainers.Add(repositoryId, destroyInfo);
                        }
                        else
                        {
                            Logger.Warn("Not found repository {0} for destroy created entity type {1} id {2}",
                                repositoryId, typeId, entityId);
                        }
                    }

                    if (destroyInfo.CommunicationEntity != null)
                    {
                        if (entityToDestroy.Value.Destroy)
                        {
                            destroyInfo.DestroyContainer.Batches.Add(new DestroyBatch
                            {
                                EntityTypeId = entityToDestroy.Value.TypeId,
                                EntityId = entityToDestroy.Value.EntityId,
                            });
                            if (ServerCoreRuntimeParameters.CollectReplicationHistory)
                                Logger.IfInfo()?.Message(
                                    $"DESTROY from:{Id} to:{destroyInfo.CommunicationEntity.Id} type:{GetTypeById(entityToDestroy.Value.TypeId)?.Name ?? "unknown"} id:{entityToDestroy.Value.EntityId}")
                                    .Write();
                        }
                        else
                        {
                            destroyInfo.DowngradeContainer.Batches.Add(new DowngradeBatch
                            {
                                EntityTypeId = entityToDestroy.Value.TypeId,
                                EntityId = entityToDestroy.Value.EntityId,
                                DowngradeMask = entityToDestroy.Value.UnloadMask,
                                Version = entityToDestroy.Value.Version,
                                PreviousVersion = entityToDestroy.Value.SendedVersion
                            });
                            if (ServerCoreRuntimeParameters.CollectReplicationHistory)
                                Logger.IfInfo()?.Message(
                                    $"DOWNGRADE from:{Id} to:{destroyInfo.CommunicationEntity.Id} type:{GetTypeById(entityToDestroy.Value.TypeId)?.Name ?? "unknown"} id:{entityToDestroy.Value.EntityId} downgrademask:{entityToDestroy.Value.UnloadMask} version:{entityToDestroy.Value.Version} previousVersion:{entityToDestroy.Value.SendedVersion}")
                                    .Write();
                        }
                    }
                }
            }
        }
        
        protected Dictionary<long, Dictionary<ulong, byte[]>> CollectDelta(IEntityRef entityRef, EntitiesContainer container)
        {
            var entity = ((IEntityRefExt)entityRef).GetEntity();
            var changedObjects = ((IEntityExt)entity).ChangedObjects;
            if (changedObjects == null || changedObjects.Count == 0)
            {
                return null;
            }

            ((IEntityExt) entity).ChangedObjects.Freeze(container);
            var deltaSnapshot = entityRef.TakeDeltaSnapshot();
            var eventsActions = new List<Func<Task>>();

            foreach (var serializedObject in changedObjects)
            {
                if (((IDeltaObjectExt)serializedObject).LocalId != 0)
                {
                    serializedObject.ProcessEvents(eventsActions);
                }

                serializedObject.ClearDelta();
            }

            ((IEntityExt) entity).ChangedObjects.UnFreeze();
            ((IEntityExt)entity).ClearChangedObjects();

            if (eventsActions.Count > 0)
            {
                ((IEntityRefExt)entityRef).AddEvents(eventsActions);
            }

            OnEntityUpdated(entityRef);
            return deltaSnapshot;
        }

        protected void sendDelta(
            IEntityRef entityRef, 
            Dictionary<long, Dictionary<ulong, byte[]>> serializedDeltaSnapshot, 
            int version,
            Dictionary<Guid, List<DeferredEntityModel>> newLinkedEntities)
        {
            try
            {
                var typeId = entityRef.TypeId;
                foreach (var replicationDestination in ((IEntityExt)((IEntityRefExt)entityRef).GetEntity())
                    .ReplicateTo())
                {
                    var replicationMask = replicationDestination.Value.GetReplicationMask();
                    if (replicationDestination.Key == this.Id
                    ) //Migrating process. Temporaly replicate container to self.
                        continue;

                    if (!serializedDeltaSnapshot.TryGetValue(replicationMask, out var serializedDeltas))
                        continue;

                    var repositoryRef =
                        ((IEntitiesRepositoryExtension)this).CheckAndGetSubscriberRepositoryCommunicationRef(
                            replicationDestination.Key, entityRef.TypeId, entityRef.Id);
                    if (repositoryRef != null)
                    {
                        var bytesCount = serializedDeltas.Sum(d => d.Value.Length);
                        Statistics<EntityUpdateStatistics>.Instance.EntityUpdateSend(typeId, bytesCount);

                        var sendedVersion = replicationDestination.Value.SendedVersion;

                        List<DeferredEntityModel> repositoryNewLinkedEntities = null;
                        newLinkedEntities?.TryGetValue(replicationDestination.Key, out repositoryNewLinkedEntities);
                        ((IRepositoryCommunicationEntity) ((IEntityRefExt) repositoryRef).GetEntity()).EntityUpdate(
                            new UpdateBatch(typeId, entityRef.Id, serializedDeltas, replicationMask, version, sendedVersion, repositoryNewLinkedEntities));
                        replicationDestination.Value.SetSendedVersion(version);
                        if (ServerCoreRuntimeParameters.CollectReplicationHistory)
                            Logger.IfInfo()?.Message(
                                $"DELTA from:{Id} to:{repositoryRef.Id} type:{GetTypeById(typeId)?.Name ?? "unknown"} id:{entityRef.Id} mask:{replicationMask} version:{version} previousVersion:{sendedVersion} length:{bytesCount} ")
                                .Write();
                    }
                    else
                    {
                        Logger.Warn("Not found repository {0} for send delta entity typeId {1} entityId {2}",
                            replicationDestination.Key, GetTypeById(typeId).Name, entityRef.Id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "sendDelta exception").Write();;
                throw;
            }
        }

        private async Task CheckClusterIsReady(CancellationToken ct)
        {
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, StopToken))
            {
                await CheckRepoCloudRequirementsMet(linkedCts.Token);
                await CheckClusterRequirementsMet(linkedCts.Token);
                await FireInitializationTasks(linkedCts.Token);
                await CheckClusterInitTasksCompleted(linkedCts.Token);
            }
        }

        private async Task CheckRepoCloudRequirementsMet(CancellationToken ct)
        {
            while (true)
            {
                try
                {
                    var batch = EntityBatch.Create();
                    var currentRepositoryCommunicationRefs = GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)).Values.ToList();
                    foreach (var repositoryCommunicationRef in currentRepositoryCommunicationRefs)
                        batch.Add<IRepositoryCommunicationEntity>(repositoryCommunicationRef.Id);

                    using (var wrapper2 = await Get(batch))
                    {
                        bool isReady = true;
                        foreach (var cloudRequirement in _cloudRequirements)
                        {
                            if (currentRepositoryCommunicationRefs.Count(
                                    x => ((IRepositoryCommunicationEntity) ((IEntityRefExt) x).GetEntity()).ConfigId ==
                                         cloudRequirement.Target.RepositoryId) < cloudRequirement.Target.Count)
                            {
                                // Logger.IfInfo()?.Message("Repository Cloud requirements doesn't met: CurrentRepository: {0} CurrentConfigId: {1} RequirementConfigId {2}", Id, _config.ConfigId, cloudRequirement.Target.RepositoryId).Write();
                                isReady = false;
                                break;
                            }
                        }

                        if (isReady)
                        {
                            ConsoleLogger.IfInfo()?.Message("Repository Cloud requirements met: {0} {1}", Id, _config.ConfigId).Write();
                            var ourCommEntity = wrapper2.Get<IRepositoryCommunicationEntity>(this.Id);
                            await ourCommEntity.SetCloudRequirementsMet();

                            return;
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
                }
                catch (Exception e)
                {
                    Logger.IfFatal()?.Exception(e).Write();
                    throw;
                }
            }
        }

        private async Task CheckClusterRequirementsMet(CancellationToken ct)
        {
            while (true)
            {
                try
                {
                    var batch = EntityBatch.Create();
                    var currentRepositoryCommunicationRefs = GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)).Values.ToList();
                    foreach (var repositoryCommunicationRef in currentRepositoryCommunicationRefs)
                        batch.Add<IRepositoryCommunicationEntity>(repositoryCommunicationRef.Id);

                    using (var wrapper2 = await Get(batch))
                    {
                        bool requirementsMet = currentRepositoryCommunicationRefs.All(v => ((IRepositoryCommunicationEntity)((IEntityRefExt)v).GetEntity()).CloudRequirementsMet);

                        if (requirementsMet)
                        {
                            ConsoleLogger.IfInfo()?.Message("Cluster Cloud requirements met: {0} {1}", Id, _config.ConfigId).Write();
                            var ourCommEntity = wrapper2.Get<IRepositoryCommunicationEntity>(this.Id);
                            await ourCommEntity.SetCloudRequirementsMet();

                            return;
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
                }
                catch (Exception e)
                {
                    Logger.IfFatal()?.Exception(e).Write();
                    throw;
                }
            }
        }

        private async Task FireInitializationTasks(CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                await CloudRequirementsMet.InvokeAsyncWithExceptions();

                ConsoleLogger.IfInfo()?.Message("Repo init tasks completed {0} {1}", Id, _config.ConfigId).Write();

                using (var entityWrapper = await Get<IRepositoryCommunicationEntity>(this.Id))
                {
                    var entity = entityWrapper.Get<IRepositoryCommunicationEntity>(this.Id);
                    await entity.SetInitializationTasksCompleted();
                }
            }
            catch(Exception e)
            {
                Logger.Fatal(e);
                throw;
            }
        }

        private async Task CheckClusterInitTasksCompleted(CancellationToken ct)
        {
            while (true)
            {
                try
                {
                    var batch = EntityBatch.Create();
                    var currentRepositoryCommunicationRefs = GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)).Values.ToList();
                    foreach (var repositoryCommunicationRef in currentRepositoryCommunicationRefs)
                        batch.Add<IRepositoryCommunicationEntity>(repositoryCommunicationRef.Id);

                    using (var wrapper2 = await Get(batch))
                    {
                        bool initDone = currentRepositoryCommunicationRefs.All(v => ((IRepositoryCommunicationEntity)((IEntityRefExt)v).GetEntity()).InitializationTasksCompleted);

                        if (initDone)
                        {
                            ConsoleLogger.IfInfo()?.Message("Opening external comm node {0} {1}", Id, _config.ConfigId).Write();
                            InitExternalCommNode();

                            var ourCommEntity = wrapper2.Get<IRepositoryCommunicationEntity>(this.Id);
                            await ourCommEntity.SetExternalCommNodeOpen();

                            return;
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
                }
                catch (Exception e)
                {
                    Logger.IfFatal()?.Exception(e).Write(); 
                    throw;
                }
            }
        }

        public async Task<ClusterStatusReport> IsClusterReady()
        {
            var csr = new ClusterStatusReport();
            var batch = EntityBatch.Create();
            foreach (var entityRef in GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)))
                batch.Add(entityRef.Value.TypeId, entityRef.Value.Id);

            csr.Label = RepositoryConfigId;
            csr.IsReady = true;
            using (var wrapper = await Get(batch))
            {
                foreach (var entityRef in GetEntitiesCollection(typeof(IRepositoryCommunicationEntity)))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityServer>(entityRef.Value.Id);

                    if (entity == null)
                    {
                        csr.IsReady = false;
                        continue;
                    }

                    if (entity.CloudNodeType == CloudNodeType.Client)
                        continue;

                    csr.HasRepoComEntities.Add(new RepoCommEntityStatus(entity.InternalAddress.Address, entity.InternalAddress.Port, entity.ConfigId, entity.ExternalCommunicationNodeOpen));
                    if (!entity.ExternalCommunicationNodeOpen)
                    {
                        csr.IsReady = false;
                        continue;
                    }
                }
            }

            return csr;
        }

        public virtual void InvokeRemoteCallResult(Action action)
        {
            action?.Invoke();
        }

        public async Task ConnectExternal(string host, int port, CancellationToken ct)
        {
            if (_externalCommunicationNode == null)
            {
                Logger.IfError()?.Message("Repository Id {0} Type {1} has no exteranl node", Id, _cloudNodeType.ToString()).Write();
                return;
            }

            var nodeIpAddress = NetUtils.ResolveAddress(host);
            var endpoint = new IPEndPoint(nodeIpAddress, port);
            await _externalCommunicationNode.ConnectNode(endpoint, ct);
        }

        public void DisconnectExternal(Guid remoteRepo)
        {
            if (_externalCommunicationNode == null)
            {
                Logger.IfError()?.Message("Repository Id {0} Type {1} has no exteranl node", Id, _cloudNodeType.ToString()).Write();
                return;
            }

            //Logger.IfError()?.Message("PRE TASK {0} Try disconnect from repo  host {1} port {2}", this.Id, host, port).Write();
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await Get<IRepositoryCommunicationEntityServer>(remoteRepo))
                {
                    var repositoryEntity = wrapper.Get<IRepositoryCommunicationEntityServer>(remoteRepo);
                    ((IRemoteEntity)repositoryEntity.GetBaseDeltaObject()).GetNetworkProxy().Close();
                }
            });
        }

        public virtual void DumpImpl(Stream stream)
        {
            using (_SerializerContext.Pool.Set())
            {
                _SerializerContext.Pool.Current.FullSerialize = true;
                _SerializerContext.Pool.Current.ReplicationMask = long.MaxValue;
                _SerializerContext.Pool.Current.IsDump = true;
                using (StreamWriter writer = new StreamWriter(stream))
                using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var resources = new GameResources(null);

                    serializer.Converters.Add(new DefReferenceConverter(resources.Deserializer, false));
                    serializer.Converters.Add(new DefConverter(resources.LoadedResources, resources.Deserializer));
                    serializer.Converters.Add(new Vector2IntJsonConverter());
                    serializer.Converters.Add(new Vector3IntJsonConverter());
                    serializer.Converters.Add(new GameObjectDirectReferenceConverter());
                    serializer.Formatting = Formatting.Indented;
                    serializer.TypeNameHandling = TypeNameHandling.All;
                    serializer.DefaultValueHandling = DefaultValueHandling.Include;
                    serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    serializer.Error += Serializer_Error;
                    serializer.Serialize(jsonWriter, this);
                    jsonWriter.Flush();
                }
            }
        }

        public async Task DumpEntity(int typeId, Guid entityId, Stream stream)
        {
            using (var wrapper = await Get(typeId, entityId))
            {
                using (_SerializerContext.Pool.Set())
                {
                    _SerializerContext.Pool.Current.FullSerialize = true;
                    _SerializerContext.Pool.Current.ReplicationMask = long.MaxValue;
                    _SerializerContext.Pool.Current.IsDump = true;
                    try
                    {
                        DumpEntityInternal(typeId, entityId, stream);
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "DumpEntity exception").Write();;
                    }
                }
            }
        }

        protected virtual void DumpEntityInternalInternal(int typeId, Guid entityId, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer serializer = new JsonSerializer();
                var resources = new GameResources(null);

                serializer.Converters.Add(new DefReferenceConverter(resources.Deserializer, false));
                serializer.Converters.Add(new DefConverter(resources.LoadedResources, resources.Deserializer));
                serializer.Converters.Add(new Vector2IntJsonConverter());
                serializer.Converters.Add(new Vector3IntJsonConverter());
                serializer.Converters.Add(new GameObjectDirectReferenceConverter());
                serializer.Formatting = Formatting.Indented;
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.DefaultValueHandling = DefaultValueHandling.Include;
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                serializer.Error += Serializer_Error;

                var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
                if (entityRef == null)
                {
                    Logger.IfError()?.Message("entity not found").Write();
                    serializer.Serialize(jsonWriter, new { EntityRef = $"{typeId} {entityId} not found" });
                }
                else
                    serializer.Serialize(jsonWriter, entityRef);

                jsonWriter.Flush();
            }
        }

        private void Serializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            Logger.IfError()?.Message("Dump error: {0}", e?.ErrorContext?.Error?.Message ?? "none").Write();
            e.ErrorContext.Handled = true;
        }

        public virtual async Task DumpEntitySerializedData(Stream stream, int entityTypeId, Guid entityId,
            ReplicationLevel replicationMask)
        {
            using (var wrapper = await Get(entityTypeId, entityId))
            {
                var entityRef = (IEntityRefExt)((IEntitiesContainerExtension)wrapper).GetEntityRef(entityTypeId, entityId);

                byte[] data;
                if (entityRef == null)
                {
                    data = new byte[1];
                }
                else
                {
                    var snapshot =
                        entityRef.GetSerialized(replicationMask, ReplicationLevel.None, (long)replicationMask, false);
                    data = _serializer.Serialize(snapshot);
                }

                await stream.WriteAsync(data, 0, data.Length, StopToken);
                await stream.FlushAsync(StopToken);
            }
        }

        void subscribeReplication(int typeId,
            Guid entityId,
            Guid repositoryId,
            out SerializedUploadEntity? entityToUploadSerialized,
            out DestroyEntity? entityToDestroy,
            List<(bool subscribed, ReplicationLevel level)> subscriptions)
        {
            entityToUploadSerialized = null;
            entityToDestroy = null;
            var eRef = ((IEntitiesRepositoryExtension) this).GetRef(typeId, entityId);
            var entityRef = (IEntityRefExt) eRef;
            if (entityRef != null)
            {
                lock (entityRef.Locker)
                {
                    long uploadMask;
                    long unloadMask = 0;
                    int sendedVersion = 0;
                    var version = entityRef.IncrementVersion();
                    var entity = (IEntityExt)entityRef.GetEntity();
                    var result = entity.SubscribeReplication(repositoryId, version, subscriptions, out uploadMask,
                        out unloadMask, out sendedVersion, out var newReplicationLevel, out var oldReplicationLevel);
                    if (!result)
                        Logger.Warn(
                            "Subscribe/Unsubscribe replications not changed anything. Type {0} entityId {1} repositoryId {2} subscriptions {3}",
                            eRef.TypeName, entityId, repositoryId,
                            string.Join(",",
                                subscriptions.Select(x => x.subscribed ? "add" : "remove" + "_" + x.level.ToString())));

                    if (uploadMask > 0)
                    {
                        bool upload = (uploadMask & (long)ReplicationLevel.Always) == (long)ReplicationLevel.Always;
                        entityToUploadSerialized = new SerializedUploadEntity(typeId, entityId,
                            entityRef.GetSerialized(newReplicationLevel, oldReplicationLevel, uploadMask), upload, uploadMask, version,
                            sendedVersion);
                    }

                    if (unloadMask > 0)
                    {
                        bool destroy = unloadMask == long.MaxValue;
                        entityToDestroy = new DestroyEntity(typeId, entityId, unloadMask,
                            destroy, version, sendedVersion);
                    }
                }
            }
            else
                Logger.Warn("Cant subscribe replication - entity not found. TypeId {0} entityId {1} repositoryId {2}",
                    typeId, entityId, repositoryId);
        }
        
        public async Task<Guid> GetAddressResolverServiceEntityId(int typeId, Guid entityId)
        {
            int tryCount = 0;
            do
            {
                var collection = GetEntitiesCollection(typeof(IClusterAddressResolverServiceEntity));
                var any = collection.FirstOrDefault();
                if (any.Value != null)
                    return any.Key;

                tryCount++;
                await Task.Delay(TimeSpan.FromSeconds(1));
            } while (tryCount < 3);

            Logger.IfError()?.Message("IClusterAddressResolverServiceEntity not found in cluster").Write();
            return Guid.Empty;
        }
        
        /// <summary>
        /// Subscribes to entity and linked entities
        /// </summary>
        public  Task SubscribeReplication(int typeId, Guid entityId, Guid repositoryId,
            ReplicationLevel replicationLevel, [CallerMemberName] string callerTag = null)
        {
            var type = ReplicaTypeRegistry.GetTypeById(typeId);
            if (repositoryId == Id)
            {
                Logger.IfWarn()?.Message("Got request to replicate entity {enitity_type}:{entity_id} to own repo {repository_id}, ignoring", type, entityId, repositoryId).Write();
                return Task.CompletedTask;
            }

            var masterType = ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(type);
            var masterTypeId = ReplicaTypeRegistry.GetIdByType(masterType);
            var subscribeChanges = new List<(bool subscribe, ReplicationLevel replicationLevel)>
            {
                (true, replicationLevel)
            };
            return ChangeSubscription(masterTypeId,
                entityId,
                repositoryId,
                subscribeChanges,
                false,
                callerTag);
        }

        private async Task ChangeSubscription(
            int typeId,
            Guid entityId,
            Guid repositoryId,
            List<(bool subscribe, ReplicationLevel replicationLevel)> subscribeChanges,
            bool subscribeToDatabase,
            [CallerMemberName] string callerTag = null)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            AddSubscriptionChange(() => ProcessSubscriptionsChanges(typeId,
                entityId,
                repositoryId,
                subscribeChanges,
                subscribeToDatabase,
                tcs,
                callerTag));

            var context = AsyncEntitiesRepositoryRequestContext.Head?.Context;
            context?.Release();

            try
            {
                await tcs.Task;
            }
            finally
            {
                if (context != null)
                {
                    await context.Relock();
                }
            }
        }

        private async Task ProcessSubscriptionsChanges(
            int typeId,
            Guid entityId,
            Guid repositoryId,
            List<(bool subscribe, ReplicationLevel replicationLevel)> subscribeChanges,
            bool subscribeToDatabase,
            TaskCompletionSource<object> taskCompletionSource,
            string callerTag = null)
        {
            try
            {
                var collections = new EntityCollections();

                var entitiesInfoToSubscribe = new Queue<EntitySubscribeInfo>();
                entitiesInfoToSubscribe.Enqueue(new EntitySubscribeInfo(new OuterRef(entityId, typeId), subscribeChanges, null, false, false));

                while (entitiesInfoToSubscribe.Count != 0)
                {
                    var subscribingEntityInfo = entitiesInfoToSubscribe.Dequeue();
                    var batch = ((IEntityBatchExt) EntityBatch.Create()).AddExclusive(subscribingEntityInfo.Entity.TypeId, subscribingEntityInfo.Entity.Guid)
                        .Add<IRepositoryCommunicationEntityAlways>(repositoryId);
                    using (var wrapper = await get(batch, callerTag, false))
                    {
                        if (wrapper == null)
                        {
                            Logger.Warn(
                                "SubscribeReplication wrapper is null entity typeId {0} id {1} repo {2}",
                                GetTypeById(subscribingEntityInfo.Entity.TypeId).Name, subscribingEntityInfo.Entity.Guid, repositoryId);
                            continue;
                        }

                        var subscribeTargetRef = (IEntityRefExt) ((IEntitiesContainerExtension) wrapper).GetEntityRef(subscribingEntityInfo.Entity.TypeId,
                            subscribingEntityInfo.Entity.Guid);
                        if (subscribeTargetRef == null)
                        {
                            Logger.Warn(
                                "SubscribeReplication GetEntityRef is null entity typeId {0} id {1} repo {2}",
                                GetTypeById(subscribingEntityInfo.Entity.TypeId).Name, subscribingEntityInfo.Entity.Guid, repositoryId);
                            continue;
                        }

                        if (!wrapper.IsEntityExist(RepositoryCommunicationEntity.StaticTypeId, repositoryId))
                        {
                            Logger.Info("SubscribeReplication RepositoryCommunicationEntity is null entity typeId {0} id {1} repo {2}",
                                GetTypeById(subscribingEntityInfo.Entity.TypeId).Name, subscribingEntityInfo.Entity.Guid, repositoryId);
                            return;
                        }

                        ProcessEntitySubscriptionsChange(subscribeTargetRef,
                            repositoryId,
                            subscribeToDatabase,
                            subscribingEntityInfo,
                            entitiesInfoToSubscribe,
                            ref collections);
                    }
                }

                uploadCreatedEntities(ref collections);
                uploadDestroyEntities(ref collections);
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
            finally
            {
                taskCompletionSource.TrySetResult(null);
            }
        }

        private void ProcessEntitySubscriptionsChange(
            IEntityRefExt entityRef,
            Guid repositoryId,
            bool subscribeToDatabase,
            EntitySubscribeInfo subscribingEntityInfo,
            Queue<EntitySubscribeInfo> entitiesInfoToSubscribe,
            ref EntityCollections collections)
        {
            var entity = entityRef.GetEntity();

            var repositorySubscriptions = ((BaseEntity) entity).ReplicateRepositoryIds.GetOrAdd(repositoryId, r => new ReplicateRefsContainer());
            var currentReplicationMask = repositorySubscriptions.GetReplicationMask();
            var replicationMaskAfterSubscribe = repositorySubscriptions.GetChangedReplicationMask(subscribingEntityInfo.SubscribeChanges);
            // TODO По хорошему не должны завязываться на включение масок
            // Можно унифицировать с entityrefChanges
            var serializationMaskGotBigger = replicationMaskAfterSubscribe > currentReplicationMask;

            if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
            {
                Logger.IfInfo()?.Message(
                    $"ChangeSubscription request from:{this.Id} to:{repositoryId} db:{subscribeToDatabase} type:{GetTypeById(entity.TypeId)?.Name ?? entity.TypeId.ToString()} id:{entity.Id} new replevel:{replicationMaskAfterSubscribe} old replevel {currentReplicationMask}")
                    .Write();
            }

            _linkedEntitiesSingleThreadBuffer.Value.Clear();
            // TODO VOVA Сейчас линкованные ентити, которые не сохраняются в базу, не проверяются на наличие линков на ентити, которые сохраняются в базу. Раньше было по другому
            if (serializationMaskGotBigger)
            {
                // we have to subscribe entities that could not be reachable by old mask
                entity.GetAllLinkedEntities(replicationMaskAfterSubscribe, _linkedEntitiesSingleThreadBuffer.Value, 0, subscribeToDatabase);
            }
            else
            {
                // if mask stayed the same or got lower we get entities by old mask
                entity.GetAllLinkedEntities(currentReplicationMask, _linkedEntitiesSingleThreadBuffer.Value, 0, subscribeToDatabase);
            }

            var subscribeChangesGivenParent = GetSubscribersChangesGivenParent(subscribingEntityInfo);

            List<ReplicationLevel> parentSubscribersBeforeChange = null;
            if (_linkedEntitiesSingleThreadBuffer.Value.Count != 0)
            {
                parentSubscribersBeforeChange = repositorySubscriptions.GetFlatSubscribers();
            }

            _newLinkedEntitiesBuffer.Value.Clear();
            List<DeferredEntityModel> repositoryNewLinkedEntitiesBuffer = null;
            
            try
            {
                foreach (var (level, linkedEntityRef) in _linkedEntitiesSingleThreadBuffer.Value)
                {
                    if (serializationMaskGotBigger)
                    {
                        if (_newLinkedEntitiesBuffer.Value.Count == 0)
                        {
                            repositoryNewLinkedEntitiesBuffer = new List<DeferredEntityModel>();
                            _newLinkedEntitiesBuffer.Value[repositoryId] = repositoryNewLinkedEntitiesBuffer;
                        }

                        var repositoryNewLinkedEntities = _newLinkedEntitiesBuffer.Value[repositoryId];
                        repositoryNewLinkedEntities.Add(new DeferredEntityModel(
                            new OuterRef(linkedEntityRef.Id, linkedEntityRef.TypeId),
                            (ReplicationLevel) replicationMaskAfterSubscribe));
                    }

                    var wasSubscribed = (level & currentReplicationMask) == level;
                    var shouldBeSubscribed = (level & replicationMaskAfterSubscribe) == level;
                    entitiesInfoToSubscribe.Enqueue(new EntitySubscribeInfo(
                        new OuterRef(linkedEntityRef.Id, linkedEntityRef.TypeId),
                        subscribeChangesGivenParent,
                        parentSubscribersBeforeChange,
                        shouldBeSubscribed && !wasSubscribed,
                        wasSubscribed && !shouldBeSubscribed));
                }
                
                // TODO VOva it is useless to have pre serialization
                var entityExt = (IEntityExt) entity;
                if (serializationMaskGotBigger)
                {
                    var subscribeMask = (currentReplicationMask ^ replicationMaskAfterSubscribe) & replicationMaskAfterSubscribe;

                    entityExt.CreateReplicationSetIfNotExists((ReplicationLevel) replicationMaskAfterSubscribe);
                    entityRef.SerializeAndSaveNewSubscriber(
                        (ReplicationLevel) replicationMaskAfterSubscribe,
                        (ReplicationLevel) currentReplicationMask,
                        subscribeMask);
                }

                ((IEntitiesRepositoryExtension) this).UpdateSubscriptions(
                    entity.TypeId,
                    entity.Id,
                    repositoryId,
                    subscribeChangesGivenParent,
                    _newLinkedEntitiesBuffer.Value,
                    ref collections);
            }
            finally
            {
                _linkedEntitiesSingleThreadBuffer.Value.Clear();
                _newLinkedEntitiesBuffer.Value.Clear();
            }
        }

        private readonly struct EntitySubscribeInfo
        {
            public EntitySubscribeInfo(OuterRef entity, 
                List<(bool subscribe, ReplicationLevel replicationLevel)> subscribeChanges,
                List<ReplicationLevel> parentSubscribers,
                bool newSubscriber, 
                bool removeSubscriber)
            {
                Entity = entity;
                SubscribeChanges = subscribeChanges;
                ParentSubscribers = parentSubscribers;
                NewSubscriber = newSubscriber;
                RemoveSubscriber = removeSubscriber;
            }

            public OuterRef Entity { get; }
            
            public List<(bool subscribe, ReplicationLevel replicationLevel)> SubscribeChanges { get; }

            public List<ReplicationLevel> ParentSubscribers { get; }
            
            // first time when entity is replicated by parent entity
            public bool NewSubscriber { get; }
            
            // entity should not be by parent entity
            public bool RemoveSubscriber { get; }
        }
        
        private static List<(bool subscribe, ReplicationLevel replicationLevel)> GetSubscribersChangesGivenParent(
            EntitySubscribeInfo subscribingEntityInfo)
        {
            // TODO Vova можно подписывать только на подходящие уровни, чтобы не передавать родительских подписчиков
            List<(bool subscribe, ReplicationLevel replicationLevel)> subscribeChangesGivenParent;
            if (subscribingEntityInfo.ParentSubscribers != null)
            {
                // first time when this entity should be replicated
                if (subscribingEntityInfo.NewSubscriber)
                {
                    subscribeChangesGivenParent = new List<(bool subscribe, ReplicationLevel replicationLevel)>();
                    foreach (var parentSubscriber in subscribingEntityInfo.ParentSubscribers)
                    {
                        subscribeChangesGivenParent.Add((true, parentSubscriber));
                    }

                    subscribeChangesGivenParent.AddRange(subscribingEntityInfo.SubscribeChanges);
                }
                // entity should be unreplicated
                else if (subscribingEntityInfo.RemoveSubscriber)
                {
                    subscribeChangesGivenParent = new List<(bool subscribe, ReplicationLevel replicationLevel)>();
                    foreach (var parentSubscriber in subscribingEntityInfo.ParentSubscribers)
                    {
                        subscribeChangesGivenParent.Add((false, parentSubscriber));
                    }
                }
                else
                {
                    subscribeChangesGivenParent = subscribingEntityInfo.SubscribeChanges;
                }
            }
            else
            {
                subscribeChangesGivenParent = subscribingEntityInfo.SubscribeChanges;
            }

            return subscribeChangesGivenParent;
        }

        bool skipSubscribeToDatabaseEntityRefPredicate(IEntityRef entityRef)
        {
            return DatabaseSaveTypeChecker.GetDatabaseSaveType(entityRef.TypeId) == DatabaseSaveType.None;
        }

        public async Task UnsubscribeReplication(int typeId, Guid entityId, Guid repositoryId, ReplicationLevel replicationLevel)
        {
            if (repositoryId == Id)
            {
                Logger.IfWarn()?.Message("Got request to unreplicate entity {enitity_type}:{entity_id} from own repo {repository_id}, ignoring", ReplicaTypeRegistry.GetTypeById(typeId), entityId, repositoryId).Write();
                return;
            }

            if (typeId == GetIdByType(typeof(IClusterAddressResolverServiceEntity)))
                Logger.IfError()?.Message("UNSUBSCRIBE ICLUSTERADDRESS").Write();

            var subscribeChanges = new List<(bool subscribe, ReplicationLevel replicationLevel)>
            {
                (false, replicationLevel)
            };
            
            await ChangeSubscription(typeId, entityId, repositoryId, subscribeChanges, false);
        }

        public async Task UnsubscribeReplicationBatch(int typeId, Guid entityId,
            List<KeyValuePair<Guid, ReplicationLevel>> subscriptions)
        {
            if (ServerCoreRuntimeParameters.CollectSubscriptionsHistory)
                foreach (var subscription in subscriptions)
                    Logger.IfInfo()?.Message(
                        $"UNSUBSCRIBEREQUEST from:{this.Id} to:{subscription.Key} type:{GetTypeById(typeId)?.Name ?? typeId.ToString()} id:{entityId} replevel:{subscription.Value.ToString()}")
                        .Write();
            using (var wrapper = await ((IEntitiesRepositoryExtension)this).GetExclusive(typeId, entityId))
            {
                if (wrapper == null)
                {
                    //Logger.IfError()?.Message("UnsubscribeReplication wrapper is null entity typeId {0} id {1} repo {2} replicationLevel {3}", GetTypeById(typeId).Name, entityId, repositoryId, replicationLevel.ToString()).Write();
                    return;
                }

                var entityRef = (IEntityRefExt)((IEntitiesContainerExtension)wrapper).GetEntityRef(typeId, entityId);
                if (entityRef == null)
                {
                    //Logger.IfError()?.Message("UnsubscribeReplication GetEntityRef is null entity typeId {0} id {1} repo {2} replicationLevel {3}", GetTypeById(typeId).Name, entityId, repositoryId, replicationLevel.ToString()).Write();
                    return;
                }
                
                var masterTypeId = EntitiesRepository.GetMasterTypeIdByReplicationLevelType(typeId);
                foreach (var subscription in subscriptions)
                {
                    var subscriptionsChange = new List<(bool subscribe, ReplicationLevel replicationLevel)>
                    {
                        (false, subscription.Value)
                    };
                    await ChangeSubscription(masterTypeId,
                        entityId,
                        subscription.Key,
                        subscriptionsChange,
                        false);
                }
            }
        }

        public static bool IsImplements<T>(int entityTypeId)
        {
            return typeof(T).IsAssignableFrom(GetTypeById(entityTypeId));
        }

        public bool IsEntityTypeAvailableInEntityGraph(Type type)
        {
            if (!SuppressEntityInitialization)
                return true;
            return DatabaseSaveTypeChecker.GetDatabaseSaveType(type) != DatabaseSaveType.None;
        }

        public INetworkProxy GetNetworkProxy(Guid callbackId)
        {
            IEntityRef remoteRepositoryEntity = null;
            if (!GetEntitiesCollection(typeof(IRepositoryCommunicationEntity))
                .TryGetValue(callbackId, out remoteRepositoryEntity))
            {
                Logger.IfError()?.Message("GetNetworkProxy {0} not found", callbackId).Write();
                return null;
            }

            return ((IRemoteEntity)((IEntityRefExt)remoteRepositoryEntity).GetEntity()).GetNetworkProxy();
        }

        public readonly struct SerializedUploadEntity
        {
            public SerializedUploadEntity(int typeId, Guid entityId, Dictionary<ulong, byte[]> data, bool upload, long uploadMask,
                int version, int sendedVersion)
            {
                TypeId = typeId;
                EntityId = entityId;
                Data = data;
                Upload = upload;
                UploadMask = uploadMask;
                Version = version;
                SendedVersion = sendedVersion;
            }

            public int TypeId { get; }
            public Guid EntityId { get; }
            public Dictionary<ulong, byte[]> Data { get; }
            public bool Upload { get; }
            public long UploadMask { get; }
            public int Version { get; }
            public int SendedVersion { get; }
        }

        public readonly struct DestroyEntity
        {
            public DestroyEntity(int typeId, Guid entityId, long unloadMask, bool destroy, int version,
                int sendedVersion)
            {
                TypeId = typeId;
                EntityId = entityId;
                UnloadMask = unloadMask;
                Destroy = destroy;
                Version = version;
                SendedVersion = sendedVersion;
            }

            public int TypeId { get; }
            public Guid EntityId { get; }
            public long UnloadMask { get; }
            public bool Destroy { get; }
            public int Version { get; }
            public int SendedVersion { get; }
        }
    }

    public class RepositoryTimeoutException : Exception
    {
        public RepositoryTimeoutException()
        {
        }

        public RepositoryTimeoutException(string message) : base(message)
        {
        }

        public RepositoryTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RepositoryTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class RepositoryRemoveFromQueueException : RepositoryTimeoutException
    {
        public RepositoryRemoveFromQueueException()
        {
        }

        public RepositoryRemoveFromQueueException(string message) : base(message)
        {
        }

        public RepositoryRemoveFromQueueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RepositoryRemoveFromQueueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class RepositoryCriticalException : Exception
    {
        public RepositoryCriticalException()
        {
        }

        public RepositoryCriticalException(string message) : base(message)
        {
        }

        public RepositoryCriticalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RepositoryCriticalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class AsyncContextException : Exception
    {
        public AsyncContextException()
        {
        }

        public AsyncContextException(string message) : base(message)
        {
        }

        public AsyncContextException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AsyncContextException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}