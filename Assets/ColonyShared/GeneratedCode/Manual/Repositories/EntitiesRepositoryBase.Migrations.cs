using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratedCode.EntitySystem.Migrating;
using GeneratedCode.Manual.Repositories;
using SharedCode.Cloud;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Interfaces.Network;
using SharedCode.Network;
using SharedCode.Refs;

namespace GeneratedCode.Repositories
{
    public partial class EntitiesRepository
    {
        public async Task<MigrateEntityResult> MigrateEntity(int typeId, Guid entityId, Guid toRepositoryId, ReplicationLevel remainedReplicationLevel)
        {
            var migratingId = MigrationIdHolder.CurrentMigrationId;
            if (migratingId == Guid.Empty)
                migratingId = Guid.NewGuid();

            Logger.IfInfo()?.Message("Try MigrateEntity entityTypeId {0} Id {1} repo {2} to repository {3} remainedReplicationLevel {4} migratingId {5}", typeId, entityId, Id, toRepositoryId, remainedReplicationLevel.ToString(), migratingId).Write();

            var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
            if (entityRef == null)
            {
                Logger.IfError()?.Message("MigrateEntity entity not found typeId {0} entityId {1}", typeId, entityId).Write();
                return MigrateEntityResult.ErrorEntityNotFound;
            }

            using (var wrapper = await Get<IRepositoryCommunicationEntityServer>(toRepositoryId))
            {
                var toRepositoryEntity = wrapper?.Get<IRepositoryCommunicationEntityServer>(toRepositoryId);
                if (toRepositoryEntity == null)
                {
                    Logger.IfError()?.Message("MigrateEntity repository not found {0}", toRepositoryId).Write();
                    return MigrateEntityResult.ErrorRepositoryNotFound;
                }

                if (toRepositoryEntity.CloudNodeType == CloudNodeType.Client)
                {
                    Logger.IfError()?.Message("MigrateEntity cant migrate to client repository {0}", toRepositoryId).Write();
                    return MigrateEntityResult.ErrorCantMigrateToClientRepository;
                }
            }

            var linkedEntitiesRefs = new List<IEntityRef>();
            //первая фаза. дожидаемся завершения всех выполняющихся rpc и устанавливаем состояние раздачи миграционных ид
            var phase1Result = await setMigrationStateAndWaitExecutingRpc(entityRef, true, EntityMigrationState.GiveRpcMigratingId, MigrationIncrementCounterType.WithoutMigrationId, migratingId, linkedEntitiesRefs);
            if (phase1Result != MigrateEntityResult.Success)
            {
                await rollbackMigrating(linkedEntitiesRefs);
                return phase1Result;
            }

            //вторая фаза. дожидаемся завершения всех выполняющихся rpc с миграционными ид, но после этого еще останутся энтити очереди репозитория
            var phase2Result = await setMigrationStateAndWaitExecutingRpc(entityRef, true, EntityMigrationState.Migrating, MigrationIncrementCounterType.HaveMigrationId, migratingId, linkedEntitiesRefs);
            if (phase2Result != MigrateEntityResult.Success)
            {
                await rollbackMigrating(linkedEntitiesRefs);
                return phase1Result;
            }

            //третья фаза. дожидаемся завершения всех выполняющихся rpc, при этом очередей в репозитории уже нет - все входящие rpc попадают в очередь отложенных
            var phase3Result = await setMigrationStateAndWaitExecutingRpc(entityRef, false, EntityMigrationState.None, MigrationIncrementCounterType.HaveMigrationId, migratingId, linkedEntitiesRefs);
            if (phase3Result != MigrateEntityResult.Success)
            {
                await rollbackMigrating(linkedEntitiesRefs);
                return phase1Result;
            }

            using (var wrapper = await Get<IRepositoryCommunicationEntityServer>(toRepositoryId))
            {
                var toRepositoryEntity = wrapper?.Get<IRepositoryCommunicationEntityServer>(toRepositoryId);
                if (toRepositoryEntity == null)
                {
                    Logger.IfError()?.Message("MigrateEntity repository not found {0}", toRepositoryId).Write();
                    return MigrateEntityResult.ErrorRepositoryNotFound;
                }

                await SubscribeReplication(entityRef.TypeId, entityRef.Id, toRepositoryEntity.Id, ReplicationLevel.Master);
                var result = await toRepositoryEntity.StartMigrateEntity(entityRef.TypeId, entityRef.Id);

                if (result != StartMigrateEntityResult.Success)
                { 
                    Logger.IfError()?.Message("Failed StartMigrateEntity entityTypeId {0} Id {1} repo {2} to repository {3} result {4}", entityRef.Id, entityRef.TypeId, Id, toRepositoryId, result).Write();
                    await rollbackMigrating(linkedEntitiesRefs);
                    await UnsubscribeReplication(entityRef.TypeId, entityRef.Id, toRepositoryEntity.Id, ReplicationLevel.Master);
                    return MigrateEntityResult.ErrorStartMigrate;
                }

                Logger.IfInfo()?.Message("Success StartMigrateEntity entityTypeId {0} Id {1} repo {2} to repository {3} result {4}", entityRef.Id, entityRef.TypeId, Id, toRepositoryId, result).Write();
            }

            var batch = EntityBatch.Create().Add<IRepositoryCommunicationEntityServer>(toRepositoryId);
            foreach (var linkedEntitiesRef in linkedEntitiesRefs)
                ((IEntityBatchExt)batch).AddExclusive(linkedEntitiesRef.TypeId, linkedEntitiesRef.Id);

            using (var wrapper = await Get(batch))
            {
                foreach (var linkedEntityRef in linkedEntitiesRefs)
                {
                    var entity = wrapper.Get<IEntityExt>(linkedEntityRef.TypeId, linkedEntityRef.Id, ReplicationLevel.Master);
                    entity.UpdateSubscribersOnMigration(toRepositoryId, remainedReplicationLevel);
                    entity.SetOwnerNodeId(toRepositoryId);
                    entity.SetMigratingId(Guid.Empty);
                }
            }

            var sendedVersions = new Dictionary<ValueTuple<int, Guid>, Dictionary<Guid, int>>();//TODO временный костыль, пока не придумал как сделать нормально
            foreach (var linkedEntityRef in linkedEntitiesRefs)
            {
                var entity = (IEntityExt) ((IEntityRefExt) linkedEntityRef).GetEntity();
                entity.ReplicateRepositoryIds[this.Id].SetSendedVersion(((IEntityRefExt)linkedEntityRef).Version);
                var dictionary = new Dictionary<Guid, int>();
                sendedVersions.Add(new ValueTuple<int, Guid>(linkedEntityRef.TypeId, linkedEntityRef.Id), dictionary);
                foreach (var pair in entity.ReplicateTo())
                    dictionary.Add(pair.Key, pair.Value.SendedVersion);
            }

            using (var wrapper = await get(batch, checkExclusiveOnReplication: false))
            {
                foreach (var linkedEntityRef in linkedEntitiesRefs)
                {
                    var entity = (IEntityExt) ((IEntityRefExt) linkedEntityRef).GetEntity();
                    entity.ClearReplicationsContainers();
                    entity.SetMigrating(EntityMigrationState.None);
                    var currentVersion = ((IEntityRefExt) linkedEntityRef).Version;
                    var downgradeMask = ((long) ReplicationLevel.Master ^ (long) remainedReplicationLevel) & (long) ReplicationLevel.Master;
                    linkedEntityRef.AddDowngrade(downgradeMask, currentVersion, currentVersion);
                    
                    TryProcessDeltaSnapshots(linkedEntityRef);
                    
                    entity.ReplicationSets.Clear();
                }
            }

            using (var wrapper = await Get<IRepositoryCommunicationEntityServer>(toRepositoryId))
            {
                var toRepositoryEntity = wrapper?.Get<IRepositoryCommunicationEntityServer>(toRepositoryId);
                if (toRepositoryEntity == null)
                {
                    Logger.IfError()?.Message("MigrateEntity repository not found {0}", toRepositoryId).Write();
                    await rollbackMigrating(linkedEntitiesRefs);
                    return MigrateEntityResult.ErrorDestinationRepositoryNotFoundOnFinish;
                }

                var result = await toRepositoryEntity.FinishMigrateEntity(typeId, entityId, sendedVersions);
                if (result != FinishMigrateEntityResult.Success)
                {
                    Logger.IfError()?.Message("FinishMigrateEntity error {0} {1} to repository {2} result {3}", entityRef.TypeName, entityRef.Id, toRepositoryId, result).Write();
                    await rollbackMigrating(linkedEntitiesRefs);
                    return MigrateEntityResult.ErrorDestinationRepositoryErrorOnFinish;
                }

                await toRepositoryEntity.DispatchMigratedEntityDeferredRpc(typeId, entityId);
            }

            using (var wrapper = await get(batch, checkExclusiveOnReplication: false))
            {
                //TODO сейчас отложенные rpc выполняются в обратном порядке - сначала на таргете, потом на источнике. Возможно нужно придумать способ поменять порядок, например через проверку CurrentCallbackRepositoryId
                await Task.WhenAll(linkedEntitiesRefs.Select(x => ((IEntityExt)((IEntityRefExt)x).GetEntity()).DispatchDeferredMigratingRpc()));
            }

            return MigrateEntityResult.Success;
        }

        async Task<StartMigrateEntityResult> IEntitiesRepositoryExtension.StartMigrateEntity(int typeId, Guid entityId, Guid fromRepositoryId)
        {
            IEntityRef entityRef = null;
            int tryCount = 0;
            do
            {
                entityRef = ((IEntitiesRepositoryExtension) this).GetRef(typeId, entityId);
                if (entityRef == null)
                    await Task.Delay(20);
                tryCount++;
                if (tryCount > 10)
                    return StartMigrateEntityResult.ErrorEntityNotFound;

            } while (entityRef == null);


            var linkedEntitiesRefs = new List<IEntityRef> { entityRef };
                // TODO VOVA Не стал поддерживать
            // getAllLinkedEntitiesRecursive(entityRef, linkedEntitiesRefs, (long)ReplicationLevel.Master);

            var batch = EntityBatch.Create();
            foreach (var linkedEntitiesRef in linkedEntitiesRefs)
                ((IEntityBatchExt)batch).AddExclusive(linkedEntitiesRef.TypeId, linkedEntitiesRef.Id);

            using (var wrapper = await get(batch, checkExclusiveOnReplication: false))
            {
                foreach (var linkedEntityRef in linkedEntitiesRefs)
                {
                    var entity = (IEntityExt)((IEntityRefExt)linkedEntityRef).GetEntity();
                    entity.SetMigrating(EntityMigrationState.MigratingDestination);
                }
            }

            return StartMigrateEntityResult.Success;
        }

        async Task<FinishMigrateEntityResult> IEntitiesRepositoryExtension.FinishMigrateEntity(int typeId, Guid entityId, Dictionary<ValueTuple<int, Guid>, Dictionary<Guid, int>> replicateRefsVersions)
        {
            Logger.IfInfo()?.Message("IEntitiesRepositoryExtension.FinishMigrateEntity entityTypeId {0} Id {1} repo {2}", typeId, entityId, Id).Write();
            var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
            var linkedEntitiesRefs = new List<IEntityRef> { entityRef };
            // TODO VOVA Не стал поддерживать
            // getAllLinkedEntitiesRecursive(entityRef, linkedEntitiesRefs, (long)ReplicationLevel.Master);

            foreach (var linkedEntitiesRef in linkedEntitiesRefs)
            {
                Dictionary<Guid, int> versions;
                if (!replicateRefsVersions.TryGetValue((linkedEntitiesRef.TypeId, linkedEntitiesRef.Id), out versions))
                {
                    Logger.IfError()?.Message("FinishMigrateEntity replicateRefsVersions not contains linked entity {0} Id {1} repo {2}", linkedEntitiesRef.TypeName, linkedEntitiesRef.Id, Id).Write();
                    continue;
                }

                if (versions != null)
                {
                    var currentRefReplications = ((IEntityExt) ((IEntityRefExt) linkedEntitiesRef).GetEntity()).ReplicateRepositoryIds;
                    foreach (var pair in currentRefReplications)
                    {
                        int version;
                        if (versions.TryGetValue(pair.Key, out version))
                            pair.Value.SetSendedVersion(version);
                    }
                }
            }

            var batch = EntityBatch.Create();
            foreach (var linkedEntitiesRef in linkedEntitiesRefs)
                ((IEntityBatchExt)batch).AddExclusive(linkedEntitiesRef.TypeId, linkedEntitiesRef.Id);

            using (var wrapper = await get(batch, checkExclusiveOnReplication: false))
            {
                foreach (var linkedEntityRef in linkedEntitiesRefs)
                {
                    var entity = (IEntityExt)((IEntityRefExt)linkedEntityRef).GetEntity();
                    entity.ClearMigratedReplicationsContainers();
                    entity.SetMigrating(EntityMigrationState.None);
                    
                    ((IEntityExt)entity).AddNewReplicationSets();
                }
            }

            using (var wrapper = await this.GetFirstService<IClusterAddressResolverServiceEntityServer>())
            {
                if (wrapper != null)
                {
                    var addressResolver = wrapper.GetFirstService<IClusterAddressResolverServiceEntityServer>();
                    var addresses = linkedEntitiesRefs.ToDictionary(x => x.TypeId, x => x.Id);
                    await addressResolver.SetEntitiesAddressRepositoryId(addresses, Id);
                }
                else
                    Logger.IfError()?.Message("Migrate IClusterAddressResolverServiceEntity replication not found in repository {0}", this.Id).Write();
            }

            return FinishMigrateEntityResult.Success;
        }

        async Task IEntitiesRepositoryExtension.DispatchMigratedEntityDeferredRpc(int typeId, Guid entityId)
        {
            Logger.IfInfo()?.Message("DispatchMigratedEntityDeferredRpc entityTypeId {0} Id {1} repo {2}", typeId, entityId, Id).Write();
            var entityRef = ((IEntitiesRepositoryExtension)this).GetRef(typeId, entityId);
            var linkedEntitiesRefs = new List<IEntityRef> { entityRef };
            // TODO VOVA Не стал поддерживать
            // getAllLinkedEntitiesRecursive(entityRef, linkedEntitiesRefs, (long)ReplicationLevel.Master);

            var batch = EntityBatch.Create();
            foreach (var linkedEntitiesRef in linkedEntitiesRefs)
                ((IEntityBatchExt)batch).AddExclusive(linkedEntitiesRef.TypeId, linkedEntitiesRef.Id);

            using (var wrapper = await get(batch, checkExclusiveOnReplication: false))
            {
                await Task.WhenAll(linkedEntitiesRefs.Select(x => ((IEntityExt) ((IEntityRefExt) x).GetEntity()).DispatchDeferredMigratingRpc()));
            }
        }

        private async Task rollbackMigrating(List<IEntityRef> migratedEntitiesRef)
        {
            var batch = EntityBatch.Create();
            foreach (var linkedEntitiesRef in migratedEntitiesRef)
                ((IEntityBatchExt)batch).AddExclusive(linkedEntitiesRef.TypeId, linkedEntitiesRef.Id);

            using (var wrapper = await Get(batch))
            {
                foreach (var linkedEntityRef in migratedEntitiesRef)
                {
                    var entity = wrapper.Get<IEntityExt>(linkedEntityRef.TypeId, linkedEntityRef.Id, ReplicationLevel.Master);
                    entity.SetMigrating(EntityMigrationState.None);
                    entity.SetMigratingId(Guid.Empty);
                }
            }

            foreach (var linkedEntityRef in migratedEntitiesRef)
                await ((IEntityExt)((IEntityRefExt)linkedEntityRef).GetEntity()).DispatchDeferredMigratingRpc();
        }

        private async Task<MigrateEntityResult> setMigrationStateAndWaitExecutingRpc(IEntityRef entityRef, bool setState, EntityMigrationState state, MigrationIncrementCounterType migrationIncrementCounterType, Guid migratingId, List<IEntityRef> migratedEntities)
        {
            var linkedEntitiesRefs = new List<IEntityRef> { entityRef };
            // TODO VOVA Не стал поддерживать
            // getAllLinkedEntitiesRecursive(entityRef, linkedEntitiesRefs, (long)ReplicationLevel.Master);

            migratedEntities.RemoveAll(x => !linkedEntitiesRefs.Contains(x));//энити, которые отвязались от куста и больше не мингрируют

            var batch = EntityBatch.Create();
            foreach (var linkedEntitiesRef in linkedEntitiesRefs)
                ((IEntityBatchExt)batch).AddExclusive(linkedEntitiesRef.TypeId, linkedEntitiesRef.Id);

            var waitTasks = new Dictionary<IEntityRefExt, Task>();
            using (var wrapper = await Get(batch))
            {
                foreach (var linkedEntityRef in linkedEntitiesRefs)
                {
                    var entity = wrapper.Get<IEntityExt>(linkedEntityRef.TypeId, linkedEntityRef.Id, ReplicationLevel.Master);
                    if (entity == null)
                    {
                        Logger.IfError()?.Message("MigrateEntity entity not found typeId {0} entityId {1}", linkedEntityRef.TypeName, linkedEntityRef.Id).Write();
                        return MigrateEntityResult.ErrorEntityNotFound;
                    }

                    if (state == EntityMigrationState.Migrating && ((IEntityRefExt)linkedEntityRef).Migrating == EntityMigrationState.Migrating)
                    {
                        Logger.IfError()?.Message("MigrateEntity entity already migrating {0} entityId {1}", linkedEntityRef.TypeName, linkedEntityRef.Id).Write();
                        return MigrateEntityResult.ErrorAlreadyMigrating;
                    }

                    if (setState)
                    {
                        entity.SetMigrating(state);
                        if (state == EntityMigrationState.GiveRpcMigratingId)
                            entity.SetMigratingId(migratingId);

                        if (!migratedEntities.Contains(linkedEntityRef))
                            migratedEntities.Add(linkedEntityRef);
                    }

                    Task task;
                    if (entity.WaitFinishExecutingRpcMethods(migrationIncrementCounterType, out task))
                        waitTasks.Add((IEntityRefExt)linkedEntityRef, task);
                }
            }

            if (waitTasks.Any())
            {
                var delayTask = Task.Delay(TimeSpan.FromSeconds(ServerCoreRuntimeParameters.WaitFinishExecutingRpcMethodsTimeoutSeconds), StopToken);
                await Task.WhenAny(Task.WhenAll(waitTasks.Values.ToArray()), delayTask);
                if (delayTask.IsCompleted)
                {
                    Logger.Error("Entity typeId {0} Id {1} repo {2}. state {3} MigrateEntity WaitFinishExecutingRpcMethods timeout {4} seconds. Waiting entities: {5}",
                        entityRef.Id, entityRef.TypeId, Id, state, ServerCoreRuntimeParameters.WaitFinishExecutingRpcMethodsTimeoutSeconds,
                        string.Join(",", waitTasks.Where(x => !x.Value.IsCompleted).Select(x => $"<TypeId:{x.Key.GetEntity().TypeName} Id:{x.Key.GetEntity().Id} MigrationState:{x.Key.GetEntity().Migrating} CounterDefault:{((IEntityExt)x.Key.GetEntity()).ExecutedMethodsCounterDefault} CounterMigrationId:{((IEntityExt)x.Key.GetEntity()).ExecutedMethodsCounterMigrationId}> {Environment.NewLine}")));
                }
            }
            return MigrateEntityResult.Success;
        }

        public bool IsMigratingOrNeedRedirecting(IEntity entity, byte[] data, Guid callback)
        {
            if (entity.Migrating == EntityMigrationState.Migrating)
            {
                var typeState = GetTypeState(entity.TypeId);
                MigratingEntityRpcQueue queue = null;
                if (!typeState.MigratingQueues.TryGetValue(entity.Id, out queue))
                    lock (typeState.MigratingQueues)
                        if (!typeState.MigratingQueues.TryGetValue(entity.Id, out queue))
                        {
                            queue = new MigratingEntityRpcQueue();
                            typeState.MigratingQueues[entity.Id] = queue;
                        }
                queue.AddDefferedRpc(generateRedirectingData(data, callback));
                return true;
            }

            //Проверяем что сообщение дошло на мастер копию, если нет, то перенаправляем
            if (!entity.IsMaster())
            {
                var remoteProxy = GetNetworkProxy(((IEntityExt)entity).OwnerNodeId);
                var newData = generateRedirectingData(data, callback);
                remoteProxy?.SendMessage(newData, 0, newData.Length, MessageSendOptions.ReliableOrdered);
                return true;
            }

            return false;
        }
        byte[] generateRedirectingData(byte[] data, Guid callback)
        {
            int doffset = 0;
            var packetHeader = _serializer.Deserialize<NetworkProxy.PacketHeader>(data, ref doffset);
            switch (packetHeader.PacketType)
            {
                case PacketType.Send:
                    packetHeader.PacketType = PacketType.SendRedirect;
                    break;
                case PacketType.Request:
                    packetHeader.PacketType = PacketType.RequestRedirect;
                    break;
                default:
                    Logger.IfError()?.Message("Incorrect packet type: {0}", packetHeader.PacketType).Write();
                    return null;
            }

            var newData = new byte[data.Length + 16];
            int offset = 0;
            _serializer.Serialize(newData, ref offset, packetHeader);
            _serializer.Serialize(newData, ref offset, callback);
            Buffer.BlockCopy(data, doffset, newData, offset, data.Length - doffset);
            return newData;
        }
    }
}
