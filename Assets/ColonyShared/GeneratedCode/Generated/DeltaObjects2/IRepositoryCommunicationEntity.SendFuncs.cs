// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using SharedCode.Network;
using System.Threading.Tasks;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem;
using System.Collections.Generic;

namespace GeneratedCode.DeltaObjects
{
    public partial class RepositoryCommunicationEntity
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> SetCloudRequirementsMet(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> SetInitializationTasksCompleted(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> SetExternalCommNodeOpen(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> FireOnDisconnect(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 3, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 3, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> NotifyOfExistingConnections(System.Collections.Generic.List<SharedCode.Entities.Cloud.EndpointAddress> endpoints, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 4, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<SharedCode.Entities.Cloud.EndpointAddress>)endpoints);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 4, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, 60, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask NewNodeConnected(System.Guid newNodeId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 5, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)newNodeId);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 5, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask EntityUpdate(SharedCode.EntitySystem.UpdateBatch updateBatch, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 6, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.UpdateBatch)updateBatch);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 6, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask EntityUpdate(SharedCode.EntitySystem.UpdateBatchContainer updateBatchContainer, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 7, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.UpdateBatchContainer)updateBatchContainer);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 7, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask EntityUpload(SharedCode.EntitySystem.UploadBatchContainer uploadBatchContainer, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 8, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.UploadBatchContainer)uploadBatchContainer);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 8, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask EntityDestroyed(SharedCode.EntitySystem.DestroyBatchContainer destroyBatchContainer, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 9, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.DestroyBatchContainer)destroyBatchContainer);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 9, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask EntityDowngrade(SharedCode.EntitySystem.DowngradeBatchContainer downgradeBatchContainer, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 10, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.DowngradeBatchContainer)downgradeBatchContainer);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 10, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask Dump(string fileName, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 11, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)fileName);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 11, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask DumpEntity(int typeId, System.Guid entityId, string fileName, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 12, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)typeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)fileName);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 12, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask DumpEntitySerializedData(string fileName, int entityTypeId, System.Guid entityId, long replicationMask, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 13, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)fileName);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)entityTypeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (long)replicationMask);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 13, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> SubscribeReplication(int typeId, System.Guid entityId, System.Guid repositoryId, SharedCode.EntitySystem.ReplicationLevel replicationLevel, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 14, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)typeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)repositoryId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.ReplicationLevel)replicationLevel);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 14, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> UnsubscribeReplication(int typeId, System.Guid entityId, System.Guid repositoryId, SharedCode.EntitySystem.ReplicationLevel replicationLevel, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 15, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)typeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)repositoryId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.ReplicationLevel)replicationLevel);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 15, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, true);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<string> GetEntityStatus(int typeId, System.Guid entityId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 16, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)typeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    return EntitySystem.RpcHelper.SendRequest<string>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 16, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<string> GetAllServiceEntityStatus(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 17, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<string>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 17, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> ForceCloseConnection(System.Guid userId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 18, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)userId);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 18, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<SharedCode.Entities.Cloud.StartMigrateEntityResult> StartMigrateEntity(int entityTypeId, System.Guid entityId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 19, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)entityTypeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    return EntitySystem.RpcHelper.SendRequest<SharedCode.Entities.Cloud.StartMigrateEntityResult>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 19, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<SharedCode.EntitySystem.FinishMigrateEntityResult> FinishMigrateEntity(int entityTypeId, System.Guid entityId, System.Collections.Generic.Dictionary<(int, System.Guid), System.Collections.Generic.Dictionary<System.Guid, int>> replicateRefsVersions, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 20, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)entityTypeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.Dictionary<(int, System.Guid), System.Collections.Generic.Dictionary<System.Guid, int>>)replicateRefsVersions);
                    return EntitySystem.RpcHelper.SendRequest<SharedCode.EntitySystem.FinishMigrateEntityResult>(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 20, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask DispatchMigratedEntityDeferredRpc(int entityTypeId, System.Guid entityId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 21, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)entityTypeId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.Entities.Cloud.IRepositoryCommunicationEntity), 21, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}