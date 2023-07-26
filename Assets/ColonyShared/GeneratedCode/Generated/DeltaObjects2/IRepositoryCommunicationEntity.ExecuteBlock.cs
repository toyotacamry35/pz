// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class RepositoryCommunicationEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IRepositoryCommunicationEntity_SetCloudRequirementsMet__Message")]
            internal static async System.Threading.Tasks.Task SetCloudRequirementsMet_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).SetCloudRequirementsMet();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IRepositoryCommunicationEntity_SetInitializationTasksCompleted__Message")]
            internal static async System.Threading.Tasks.Task SetInitializationTasksCompleted_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).SetInitializationTasksCompleted();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IRepositoryCommunicationEntity_SetExternalCommNodeOpen__Message")]
            internal static async System.Threading.Tasks.Task SetExternalCommNodeOpen_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).SetExternalCommNodeOpen();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IRepositoryCommunicationEntity_FireOnDisconnect__Message")]
            internal static async System.Threading.Tasks.Task FireOnDisconnect_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).FireOnDisconnect();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IRepositoryCommunicationEntity_NotifyOfExistingConnections_List_Message")]
            internal static async System.Threading.Tasks.Task NotifyOfExistingConnections_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Collections.Generic.List<SharedCode.Entities.Cloud.EndpointAddress> endpoints;
                (endpoints, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<SharedCode.Entities.Cloud.EndpointAddress>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).NotifyOfExistingConnections(endpoints);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IRepositoryCommunicationEntity_NewNodeConnected_Guid_Message")]
            internal static async System.Threading.Tasks.Task NewNodeConnected_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid newNodeId;
                (newNodeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).NewNodeConnected(newNodeId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "IRepositoryCommunicationEntity_EntityUpdate_UpdateBatch_Message")]
            internal static async System.Threading.Tasks.Task EntityUpdate_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.UpdateBatch updateBatch;
                (updateBatch, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.UpdateBatch>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).EntityUpdate(updateBatch);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "IRepositoryCommunicationEntity_EntityUpdate_UpdateBatchContainer_Message")]
            internal static async System.Threading.Tasks.Task EntityUpdate_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.UpdateBatchContainer updateBatchContainer;
                (updateBatchContainer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.UpdateBatchContainer>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).EntityUpdate(updateBatchContainer);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "IRepositoryCommunicationEntity_EntityUpload_UploadBatchContainer_Message")]
            internal static async System.Threading.Tasks.Task EntityUpload_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.UploadBatchContainer uploadBatchContainer;
                (uploadBatchContainer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.UploadBatchContainer>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).EntityUpload(uploadBatchContainer);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "IRepositoryCommunicationEntity_EntityDestroyed_DestroyBatchContainer_Message")]
            internal static async System.Threading.Tasks.Task EntityDestroyed_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.DestroyBatchContainer destroyBatchContainer;
                (destroyBatchContainer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.DestroyBatchContainer>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).EntityDestroyed(destroyBatchContainer);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "IRepositoryCommunicationEntity_EntityDowngrade_DowngradeBatchContainer_Message")]
            internal static async System.Threading.Tasks.Task EntityDowngrade_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.DowngradeBatchContainer downgradeBatchContainer;
                (downgradeBatchContainer, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.DowngradeBatchContainer>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).EntityDowngrade(downgradeBatchContainer);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(11, "IRepositoryCommunicationEntity_Dump_String_Message")]
            internal static async System.Threading.Tasks.Task Dump_11(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string fileName;
                (fileName, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).Dump(fileName);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(12, "IRepositoryCommunicationEntity_DumpEntity_Int32_Guid_String_Message")]
            internal static async System.Threading.Tasks.Task DumpEntity_12(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                string fileName;
                (fileName, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).DumpEntity(typeId, entityId, fileName);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(13, "IRepositoryCommunicationEntity_DumpEntitySerializedData_String_Int32_Guid_Int64_Message")]
            internal static async System.Threading.Tasks.Task DumpEntitySerializedData_13(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string fileName;
                (fileName, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                int entityTypeId;
                (entityTypeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 2, chainContext, argumentRefs);
                long replicationMask;
                (replicationMask, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<long>(__data__, __offset__, 3, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).DumpEntitySerializedData(fileName, entityTypeId, entityId, replicationMask);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(14, "IRepositoryCommunicationEntity_SubscribeReplication_Int32_Guid_Guid_ReplicationLevel_Message")]
            internal static async System.Threading.Tasks.Task SubscribeReplication_14(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Guid repositoryId;
                (repositoryId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 2, chainContext, argumentRefs);
                SharedCode.EntitySystem.ReplicationLevel replicationLevel;
                (replicationLevel, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.ReplicationLevel>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).SubscribeReplication(typeId, entityId, repositoryId, replicationLevel);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(15, "IRepositoryCommunicationEntity_UnsubscribeReplication_Int32_Guid_Guid_ReplicationLevel_Message")]
            internal static async System.Threading.Tasks.Task UnsubscribeReplication_15(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Guid repositoryId;
                (repositoryId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 2, chainContext, argumentRefs);
                SharedCode.EntitySystem.ReplicationLevel replicationLevel;
                (replicationLevel, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.ReplicationLevel>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).UnsubscribeReplication(typeId, entityId, repositoryId, replicationLevel);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(16, "IRepositoryCommunicationEntity_GetEntityStatus_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task GetEntityStatus_16(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).GetEntityStatus(typeId, entityId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(17, "IRepositoryCommunicationEntity_GetAllServiceEntityStatus__Message")]
            internal static async System.Threading.Tasks.Task GetAllServiceEntityStatus_17(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).GetAllServiceEntityStatus();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(18, "IRepositoryCommunicationEntity_ForceCloseConnection_Guid_Message")]
            internal static async System.Threading.Tasks.Task ForceCloseConnection_18(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid userId;
                (userId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).ForceCloseConnection(userId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(19, "IRepositoryCommunicationEntity_StartMigrateEntity_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task StartMigrateEntity_19(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int entityTypeId;
                (entityTypeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).StartMigrateEntity(entityTypeId, entityId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(20, "IRepositoryCommunicationEntity_FinishMigrateEntity_Int32_Guid_Dictionary_Message")]
            internal static async System.Threading.Tasks.Task FinishMigrateEntity_20(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int entityTypeId;
                (entityTypeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Collections.Generic.Dictionary<(int, System.Guid), System.Collections.Generic.Dictionary<System.Guid, int>> replicateRefsVersions;
                (replicateRefsVersions, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.Dictionary<(int, System.Guid), System.Collections.Generic.Dictionary<System.Guid, int>>>(__data__, __offset__, 2, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).FinishMigrateEntity(entityTypeId, entityId, replicateRefsVersions);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(21, "IRepositoryCommunicationEntity_DispatchMigratedEntityDeferredRpc_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task DispatchMigratedEntityDeferredRpc_21(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int entityTypeId;
                (entityTypeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.Entities.Cloud.IRepositoryCommunicationEntity)__deltaObj__).DispatchMigratedEntityDeferredRpc(entityTypeId, entityId);
            }
        }
    }
}