// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class RealmsCollectionEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IRealmsCollectionEntity_AddRealm_Guid_RealmRulesDef_Message")]
            internal static async System.Threading.Tasks.Task AddRealm_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid mapId;
                (mapId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Aspects.Sessions.RealmRulesDef realmDef;
                (realmDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Sessions.RealmRulesDef>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmsCollectionEntity)__deltaObj__).AddRealm(mapId, realmDef);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IRealmsCollectionEntity_RemoveRealm_Guid_Message")]
            internal static async System.Threading.Tasks.Task RemoveRealm_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid mapId;
                (mapId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmsCollectionEntity)__deltaObj__).RemoveRealm(mapId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class RealmEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IRealmEntity_TryAttach_Guid_Message")]
            internal static async System.Threading.Tasks.Task TryAttach_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid account;
                (account, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmEntity)__deltaObj__).TryAttach(account);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IRealmEntity_Enter_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task Enter_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> account;
                (account, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmEntity)__deltaObj__).Enter(account);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IRealmEntity_Leave_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task Leave_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> account;
                (account, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmEntity)__deltaObj__).Leave(account);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IRealmEntity_AddMap_Guid_MapMeta_Message")]
            internal static async System.Threading.Tasks.Task AddMap_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid mapId;
                (mapId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.MapSystem.MapMeta mapMeta;
                (mapMeta, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.MapSystem.MapMeta>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmEntity)__deltaObj__).AddMap(mapId, mapMeta);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IRealmEntity_RemoveMap_Guid_Message")]
            internal static async System.Threading.Tasks.Task RemoveMap_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid mapId;
                (mapId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmEntity)__deltaObj__).RemoveMap(mapId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IRealmEntity_SetActive_Boolean_Message")]
            internal static async System.Threading.Tasks.Task SetActive_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                bool active;
                (active, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmEntity)__deltaObj__).SetActive(active);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "IRealmEntity_SetMapDead_Guid_Message")]
            internal static async System.Threading.Tasks.Task SetMapDead_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid mapId;
                (mapId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IRealmEntity)__deltaObj__).SetMapDead(mapId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}