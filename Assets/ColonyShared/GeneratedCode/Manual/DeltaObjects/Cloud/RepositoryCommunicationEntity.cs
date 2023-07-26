using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Repositories;
using SharedCode.Entities.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;
using SharedCode.Refs;
using SharedCode.Utils;
using SharedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class RepositoryCommunicationEntity : IHookOnInit
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Guid, HashSet<Guid>> _notifiedNodes = new Dictionary<Guid, HashSet<Guid>>();

        public async Task OnInit()
        {
            try
            {
                WantsToDisconnect = new WantsToDisconnectEventProxy();
                var dumpDirectory = await PathUtils.GetDumpDirectory();
                Log.Logger.IfInfo()?.Message("Dump directory is {0}", dumpDirectory).Write();
            }
            catch (Exception e)
            {
                Log.Logger.IfError()?.Message(e, "Dump directory exception").Write();;
            }
        }

        public async Task<bool> FireOnDisconnectImpl()
        {
            await WantsToDisconnect.Fire(CallbackRepositoryHolder.CurrentCallbackRepositoryId);
            return true;
        }

        public Task<bool> NotifyOfExistingConnectionsImpl(List<EndpointAddress> addresses)
        {
            ((EntitiesRepository)EntitiesRepository).NewInternalNodeAddedToCluster(addresses);
            return Task.FromResult<bool>(true);
        }

        public async Task NewNodeConnectedImpl(Guid newNodeId)
        {
            var newNotifications = new Dictionary<Guid, List<EndpointAddress>>();

            var newNodeCommunicationEntity = (IRepositoryCommunicationEntity) ((IEntityRefExt) ((EntitiesRepository) EntitiesRepository)
                .GetEntitiesCollection(typeof(IRepositoryCommunicationEntity))
                .First(e => e.Key == newNodeId).Value).GetEntity();
            
            foreach (var repositoryCommunicationRef in ((EntitiesRepository) EntitiesRepository).GetEntitiesCollection(typeof(IRepositoryCommunicationEntity))
                .Values)
            {
                var repositoryCommunication = (IRepositoryCommunicationEntity) ((IEntityRefExt) repositoryCommunicationRef).GetEntity();
                if (repositoryCommunication.CloudNodeType == SharedCode.Cloud.CloudNodeType.Server
                    && repositoryCommunication.Id != Id
                    && repositoryCommunication.Id != newNodeId)
                {
                    // node with bigger nodeId should connect to repositories with smaller ids
                    if (repositoryCommunication.Id.CompareTo(newNodeId) < 0)
                    {
                        TryAddNotification(newNodeId, repositoryCommunication, newNotifications);
                    }
                    else
                    {
                        TryAddNotification(repositoryCommunication.Id, newNodeCommunicationEntity, newNotifications);
                    }
                }
            }

            foreach (var newNotification in newNotifications)
            {
                using (var wrapper = await EntitiesRepository.Get<IRepositoryCommunicationEntityServer>(newNotification.Key))
                {
                    var entity = wrapper.Get<IRepositoryCommunicationEntityServer>(newNotification.Key);
                    
                    Logger.IfInfo()?.Message("Node {repo_id} notifying new repo {new_repo_id}, port {remote_port} of existing connections {connections}", Id,
                        newNotification.Key, entity.InternalAddress.Port, string.Join(",", newNotification.Value.Select(v => v.ToString()))).Write();
                    await entity.NotifyOfExistingConnections(newNotification.Value);
                }
            }
        }

        private void TryAddNotification(Guid newNodeId,
            IRepositoryCommunicationEntity target,
            Dictionary<Guid, List<EndpointAddress>> newNotifications)
        {
            if (!_notifiedNodes.TryGetValue(newNodeId, out var notifiedNodes))
            {
                notifiedNodes = new HashSet<Guid>();
                _notifiedNodes.Add(newNodeId, notifiedNodes);
            }
            
            if (notifiedNodes.Add(target.Id))
            {
                if (!newNotifications.TryGetValue(newNodeId, out var newNotifiedNodes))
                {
                    newNotifiedNodes = new List<EndpointAddress>();
                    newNotifications[newNodeId] = newNotifiedNodes;
                }

                newNotifiedNodes.Add(target.InternalAddress);
            }
        }

        public async Task EntityUpdateImpl(UpdateBatch updateBatch)
        {
            await ((IEntitiesRepositoryDataExtension)EntitiesRepository).EntityUpdate(updateBatch);
        }

        public async Task EntityUpdateImpl(UpdateBatchContainer updateBatchContainer)
        {
            await ((IEntitiesRepositoryDataExtension)EntitiesRepository).EntityUpdate(updateBatchContainer);
        }

        public async Task EntityUploadImpl(UploadBatchContainer uploadBatchContainer, INetworkProxy networkProxy)
        {
            await ((IEntitiesRepositoryDataExtension)EntitiesRepository).EntityUpload(uploadBatchContainer, networkProxy);
        }

        public async Task EntityDestroyedImpl(DestroyBatchContainer destroyBatchContainer)
        {
            await ((IEntitiesRepositoryDataExtension)EntitiesRepository).EntityDestroyed(destroyBatchContainer);
        }

        public async Task EntityDowngradeImpl(DowngradeBatchContainer downgradeBatchContainer)
        {
            await ((IEntitiesRepositoryDataExtension)EntitiesRepository).EntityDowngrade(downgradeBatchContainer);
        }

        public async Task DumpImpl(string fileName)
        {
            await RepositoryCommunicationEntity.Dump(fileName, EntitiesRepository, this.ToString());
        }

        public static async Task Dump(string fileName, IEntitiesRepository entitiesRepository, string tag)
        {
            Log.Logger.IfInfo()?.Message("Try dump {0}", fileName).Write();
            string fullFileName = string.Empty;
            try
            {
                fullFileName = System.IO.Path.Combine(await PathUtils.GetDumpDirectory(), fileName + ".repdmp");
                if (File.Exists(fullFileName))
                    File.Delete(fullFileName);
                using (var fileStream = File.Create(fullFileName))
                    ((IEntitiesRepositoryDebugExtension)entitiesRepository).Dump(fileStream);
                Log.Logger.IfInfo()?.Message("Dump saved to file \"{0}\" {1}", fullFileName, tag).Write();
            }
            catch (Exception e)
            {
                Log.Logger.IfError()?.Message("Exception dump to file \"{0}\" {1} exception: {2}", fullFileName, tag, e.ToString()).Write();
            }
        }

        public async Task DumpEntityImpl(int typeId, Guid entityId, string fileName)
        {
            Log.Logger.IfInfo()?.Message("Try dump entity").Write();
            string fullFileName = string.Empty;
            try
            {
                fullFileName = System.IO.Path.Combine(await PathUtils.GetDumpDirectory(), fileName + ".entdmp");
                if (File.Exists(fullFileName))
                    File.Delete(fullFileName);
                using (var fileStream = File.Create(fullFileName))
                    await ((IEntitiesRepositoryDebugExtension)EntitiesRepository).DumpEntity(typeId, entityId, fileStream);
                Log.Logger.IfInfo()?.Message("Entity {0} {1} dump saved to file \"{2}\" {3}", ReplicaTypeRegistry.GetTypeById(typeId)?.Name ?? typeId.ToString(), entityId, fullFileName, this.ToString()).Write();
            }
            catch (Exception e)
            {
                Log.Logger.IfError()?.Message("Exception dump to file \"{0}\" {1} exception: {2}", fullFileName, this.ToString(), e.ToString()).Write();
            }
        }

        public async Task DumpEntitySerializedDataImpl(string fileName, int entityTypeId, Guid entityId, long replicationMask)
        {
            Log.Logger.IfInfo()?.Message("Try dump entity serialized data").Write();
            string fullFileName = string.Empty;
            try
            {
                fullFileName = System.IO.Path.Combine(await PathUtils.GetDumpDirectory(), fileName + ".entitydmp");
                if (File.Exists(fullFileName))
                    File.Delete(fullFileName);
                using (var fileStream = File.Create(fullFileName))
                    await ((IEntitiesRepositoryDebugExtension) EntitiesRepository).DumpEntitySerializedData(fileStream,
                        entityTypeId, entityId, (ReplicationLevel) replicationMask);
                Log.Logger.IfInfo()?.Message("Dump entityId {0} saved to file \"{1}\" {2}", entityId, fullFileName, this.ToString()).Write();
            }
            catch (Exception e)
            {
                Log.Logger.IfError()?.Message("Exception dump serialized entity to file \"{0}\" {1} typeId:{2} entityId:{3} mask:{4} exception: {5}", fullFileName, entityTypeId, entityId, replicationMask, this.ToString(), e.ToString()).Write();
            }
        }

        public async Task<bool> SubscribeReplicationImpl(int typeId, Guid entityId, Guid repositoryId, ReplicationLevel replicationLvl)
        {
            await EntitiesRepository.SubscribeReplication(typeId, entityId, repositoryId, replicationLvl);
            return true;
        }

        public async Task<bool> UnsubscribeReplicationImpl(int typeId, Guid entityId, Guid repositoryId, ReplicationLevel replicationLvl)
        {
            Logger.IfDebug()?.Message($"#Dbg: RepositoryCommunicationEntity.UnsubscribeReplicationImpl {typeId} / {entityId},  repoId:{repositoryId},  lvl:{replicationLvl}.").Write();

            await EntitiesRepository.UnsubscribeReplication(typeId, entityId, repositoryId, replicationLvl);
            return true;
        }

        public Task<string> GetEntityStatusImpl(int typeId, Guid entityId)
        {
            return Task.FromResult(((IEntitiesRepositoryExtension) EntitiesRepository).GetEntityStatus(typeId, entityId));
        }

        public Task<string> GetAllServiceEntityStatusImpl()
        {
            return Task.FromResult(((IEntitiesRepositoryExtension)EntitiesRepository).GetAllServiceEntitiesOperationLog());
        }

        public ValueTask<bool> SetCloudRequirementsMetImpl()
        {
            CloudRequirementsMet = true;
            return new ValueTask<bool>(true);
        }

        public ValueTask<bool> SetInitializationTasksCompletedImpl()
        {
            InitializationTasksCompleted = true;
            return new ValueTask<bool>(true);
        }

        public ValueTask<bool> SetExternalCommNodeOpenImpl()
        {
            ExternalCommunicationNodeOpen = true;
            return new ValueTask<bool>(true);
        }


        public override string ToString()
        {
            return $"<Repository Id:{Config.ConfigId ?? "null"}, Internal {InternalAddress}, External:{ExternalAddress}, CloudNodeType:{CloudNodeType}, Req.Met:{CloudRequirementsMet}, Init.TasksDone:{InitializationTasksCompleted}, Ext.Comm.Node:{ExternalCommunicationNodeOpen} ProcessId:{ProcessId}, Guid {Id}>";
        }

        public async Task<bool> ForceCloseConnectionImpl(Guid userId)
        {
            Logger.IfInfo()?.Message("{0}: Try force close connection user {1}", this.ToString(), userId).Write();
            using (var wrapper = await EntitiesRepository.Get<IRepositoryCommunicationEntityServer>(userId))
            {
                var entity = wrapper?.Get<IRepositoryCommunicationEntityServer>(userId);
                if (entity == null)
                {
                    Logger.IfError()?.Message("{0}: ForceCloseConnectionImpl not found user repository {1}", this.ToString(), userId).Write();
                    return false;
                }

                var networkProxy = ((IRemoteEntity)entity.GetBaseDeltaObject()).GetNetworkProxy();
                if (networkProxy != null)
                {
                    Logger.IfInfo()?.Message("{0}: Force close connection user {1}", this.ToString(), userId).Write();
                    networkProxy.Close();
                }
                else
                    return false; 
            }

            return true;
        }

        public Task<StartMigrateEntityResult> StartMigrateEntityImpl(int entityTypeId, Guid entityId)
        {
            return ((IEntitiesRepositoryExtension)EntitiesRepository).StartMigrateEntity(entityTypeId, entityId, CallbackRepositoryHolder.CurrentCallbackRepositoryId);
        }
        public Task<FinishMigrateEntityResult> FinishMigrateEntityImpl(int entityTypeId, Guid entityId, Dictionary<ValueTuple<int, Guid>, Dictionary<Guid, int>> replicateRefsVersions)
        {
            return ((IEntitiesRepositoryExtension)EntitiesRepository).FinishMigrateEntity(entityTypeId, entityId, replicateRefsVersions);
        }

        public Task DispatchMigratedEntityDeferredRpcImpl(int entityTypeId, Guid entityId)
        {
            return ((IEntitiesRepositoryExtension)EntitiesRepository).DispatchMigratedEntityDeferredRpc(entityTypeId, entityId);
        }
    }
}
