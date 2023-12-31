// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldSpaced
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IWorldSpaced_AssignToWorldSpace_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task AssignToWorldSpace_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.Entities.Service.IWorldSpaceServiceEntity> ownWorldSpace;
                (ownWorldSpace, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.Entities.Service.IWorldSpaceServiceEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaced)__deltaObj__).AssignToWorldSpace(ownWorldSpace);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldSpaceServiceEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IWorldSpaceServiceEntity_GetWorldNodeId_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task GetWorldNodeId_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Utils.OuterRef entityRef;
                (entityRef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Utils.OuterRef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).GetWorldNodeId(entityRef);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IWorldSpaceServiceEntity_AddWorldObject_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task AddWorldObject_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).AddWorldObject(typeId, entityId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IWorldSpaceServiceEntity_RemoveWorldObject_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task RemoveWorldObject_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).RemoveWorldObject(typeId, entityId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IWorldSpaceServiceEntity_UpdateTransform_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task UpdateTransform_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).UpdateTransform(typeId, entityId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IWorldSpaceServiceEntity_AddClient_Guid_Guid_Message")]
            internal static async System.Threading.Tasks.Task AddClient_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid characterId;
                (characterId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid repositoryId;
                (repositoryId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).AddClient(characterId, repositoryId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IWorldSpaceServiceEntity_RemoveClient_Guid_Boolean_Message")]
            internal static async System.Threading.Tasks.Task RemoveClient_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid repositoryId;
                (repositoryId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                bool immediate;
                (immediate, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).RemoveClient(repositoryId, immediate);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "IWorldSpaceServiceEntity_GetWorldBoxIdToDrop_Vector3_Guid_Message")]
            internal static async System.Threading.Tasks.Task GetWorldBoxIdToDrop_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Utils.Vector3 position;
                (position, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Utils.Vector3>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid characterOwnerId;
                (characterOwnerId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).GetWorldBoxIdToDrop(position, characterOwnerId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "IWorldSpaceServiceEntity_RegisterFencePlace_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task RegisterFencePlace_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.Entities.Building.IFencePlace> fencePlace;
                (fencePlace, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.Entities.Building.IFencePlace>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).RegisterFencePlace(fencePlace);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "IWorldSpaceServiceEntity_UnregisterFencePlace_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task UnregisterFencePlace_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.Entities.Building.IFencePlace> fencePlace;
                (fencePlace, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.Entities.Building.IFencePlace>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).UnregisterFencePlace(fencePlace);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "IWorldSpaceServiceEntity_CreateFencePlaceId_Vector3_Message")]
            internal static async System.Threading.Tasks.Task CreateFencePlaceId_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Utils.Vector3 position;
                (position, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Utils.Vector3>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).CreateFencePlaceId(position);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "IWorldSpaceServiceEntity_GetFencePlaceId_Vector3_Boolean_Message")]
            internal static async System.Threading.Tasks.Task GetFencePlaceId_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Utils.Vector3 position;
                (position, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Utils.Vector3>(__data__, __offset__, 0, chainContext, argumentRefs);
                bool onlyExisted;
                (onlyExisted, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).GetFencePlaceId(position, onlyExisted);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(11, "IWorldSpaceServiceEntity_OnVisibilityChanged_Int32_Guid_List_List_Message")]
            internal static async System.Threading.Tasks.Task OnVisibilityChanged_11(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int subjectTypeId;
                (subjectTypeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid subjectEntityId;
                (subjectEntityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Collections.Generic.List<(int, System.Guid)> addedObjects;
                (addedObjects, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<(int, System.Guid)>>(__data__, __offset__, 2, chainContext, argumentRefs);
                System.Collections.Generic.List<(int, System.Guid)> removedObjects;
                (removedObjects, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<(int, System.Guid)>>(__data__, __offset__, 3, chainContext, argumentRefs);
                await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).OnVisibilityChanged(subjectTypeId, subjectEntityId, addedObjects, removedObjects);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(12, "IWorldSpaceServiceEntity_EnableReplications_Int32_Guid_Boolean_Message")]
            internal static async System.Threading.Tasks.Task EnableReplications_12(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int subjectTypeId;
                (subjectTypeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid subjectEntityId;
                (subjectEntityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                bool enable;
                (enable, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).EnableReplications(subjectTypeId, subjectEntityId, enable);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(13, "IWorldSpaceServiceEntity_GetCCU__Message")]
            internal static async System.Threading.Tasks.Task GetCCU_13(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).GetCCU();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(14, "IWorldSpaceServiceEntity_SpawnNewBot_String_List_Guid_Message")]
            internal static async System.Threading.Tasks.Task SpawnNewBot_14(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string spawnPointTypePath;
                (spawnPointTypePath, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Collections.Generic.List<System.Guid> botIds;
                (botIds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<System.Guid>>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Guid userId;
                (userId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).SpawnNewBot(spawnPointTypePath, botIds, userId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(15, "IWorldSpaceServiceEntity_Respawn_Guid_Boolean_Boolean_Guid_Message")]
            internal static async System.Threading.Tasks.Task Respawn_15(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid charId;
                (charId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                bool checkBakens;
                (checkBakens, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 1, chainContext, argumentRefs);
                bool anyCommonBaken;
                (anyCommonBaken, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 2, chainContext, argumentRefs);
                System.Guid commonBakenId;
                (commonBakenId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).Respawn(charId, checkBakens, anyCommonBaken, commonBakenId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(16, "IWorldSpaceServiceEntity_Login_BotActionDef_String_Guid_MapOwner_Message")]
            internal static async System.Threading.Tasks.Task Login_16(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.Aspects.Doings.BotActionDef botDef;
                (botDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.Aspects.Doings.BotActionDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                string spawnPointPath;
                (spawnPointPath, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Guid userRepository;
                (userRepository, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 2, chainContext, argumentRefs);
                GeneratedCode.MapSystem.MapOwner mapOwner;
                (mapOwner, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<GeneratedCode.MapSystem.MapOwner>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).Login(botDef, spawnPointPath, userRepository, mapOwner);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(17, "IWorldSpaceServiceEntity_Logout_Guid_Boolean_Message")]
            internal static async System.Threading.Tasks.Task Logout_17(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid userId;
                (userId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                bool terminal;
                (terminal, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).Logout(userId, terminal);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(18, "IWorldSpaceServiceEntity_LogoutAll__Message")]
            internal static async System.Threading.Tasks.Task LogoutAll_18(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).LogoutAll();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(19, "IWorldSpaceServiceEntity_ConnectStreamingRepo_Guid_Message")]
            internal static async System.Threading.Tasks.Task ConnectStreamingRepo_19(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid repo;
                (repo, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).ConnectStreamingRepo(repo);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(20, "IWorldSpaceServiceEntity_DisconnectStreamingRepo_Guid_Message")]
            internal static async System.Threading.Tasks.Task DisconnectStreamingRepo_20(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid repo;
                (repo, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).DisconnectStreamingRepo(repo);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(21, "IWorldSpaceServiceEntity_Teleport_Guid_Message")]
            internal static async System.Threading.Tasks.Task Teleport_21(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid oldRepositoryGuid;
                (oldRepositoryGuid, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).Teleport(oldRepositoryGuid);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(22, "IWorldSpaceServiceEntity_PrepareStaticsFor_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task PrepareStaticsFor_22(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> sceneEntity;
                (sceneEntity, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).PrepareStaticsFor(sceneEntity);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(23, "IWorldSpaceServiceEntity_SpawnEntity_Guid_OuterRef_Vector3_Quaternion_MapOwner_Guid_IEntityObjectDef_SpawnPointTypeDef_ScenicEntityDef_ScriptingContext_Message")]
            internal static async System.Threading.Tasks.Task SpawnEntity_23(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid staticIdFromExport;
                (staticIdFromExport, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent;
                (ent, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 1, chainContext, argumentRefs);
                SharedCode.Utils.Vector3 pos;
                (pos, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Utils.Vector3>(__data__, __offset__, 2, chainContext, argumentRefs);
                SharedCode.Utils.Quaternion rot;
                (rot, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Utils.Quaternion>(__data__, __offset__, 3, chainContext, argumentRefs);
                GeneratedCode.MapSystem.MapOwner mapOwner;
                (mapOwner, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<GeneratedCode.MapSystem.MapOwner>(__data__, __offset__, 4, chainContext, argumentRefs);
                System.Guid spawner;
                (spawner, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 5, chainContext, argumentRefs);
                SharedCode.Entities.GameObjectEntities.IEntityObjectDef def;
                (def, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.GameObjectEntities.IEntityObjectDef>(__data__, __offset__, 6, chainContext, argumentRefs);
                SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef point;
                (point, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef>(__data__, __offset__, 7, chainContext, argumentRefs);
                SharedCode.Entities.GameObjectEntities.ScenicEntityDef scenicEntityDef;
                (scenicEntityDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.GameObjectEntities.ScenicEntityDef>(__data__, __offset__, 8, chainContext, argumentRefs);
                Scripting.ScriptingContext ctx;
                (ctx, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Scripting.ScriptingContext>(__data__, __offset__, 9, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).SpawnEntity(staticIdFromExport, ent, pos, rot, mapOwner, spawner, def, point, scenicEntityDef, ctx);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(24, "IWorldSpaceServiceEntity_DespawnEntity_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task DespawnEntity_24(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> ent;
                (ent, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).DespawnEntity(ent);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(25, "IWorldSpaceServiceEntity_GetPositionToSpawnAt_Guid_Boolean_Boolean_Guid_SpawnPointTypeDef_Message")]
            internal static async System.Threading.Tasks.Task GetPositionToSpawnAt_25(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid charId;
                (charId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                bool checkBakens;
                (checkBakens, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 1, chainContext, argumentRefs);
                bool anyCommonBaken;
                (anyCommonBaken, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 2, chainContext, argumentRefs);
                System.Guid commonBakenId;
                (commonBakenId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 3, chainContext, argumentRefs);
                SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef overrideAllowedPointType;
                (overrideAllowedPointType, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef>(__data__, __offset__, 4, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).GetPositionToSpawnAt(charId, checkBakens, anyCommonBaken, commonBakenId, overrideAllowedPointType);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(26, "IWorldSpaceServiceEntity_RegisterWorldObjectsInNewInformationSet_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task RegisterWorldObjectsInNewInformationSet_26(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Utils.OuterRef worldObjectSetRef;
                (worldObjectSetRef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Utils.OuterRef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IWorldSpaceServiceEntity)__deltaObj__).RegisterWorldObjectsInNewInformationSet(worldObjectSetRef);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}