// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class AccountEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IAccountEntity_SetCurrentUserId_Guid_Message")]
            internal static async System.Threading.Tasks.Task SetCurrentUserId_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid userId;
                (userId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.IAccountEntity)__deltaObj__).SetCurrentUserId(userId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IAccountEntity_GetCurrentUserId__Message")]
            internal static async System.Threading.Tasks.Task GetCurrentUserId_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.IAccountEntity)__deltaObj__).GetCurrentUserId();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IAccountEntity_ClearAndConsumeOldRealmRewards__Message")]
            internal static async System.Threading.Tasks.Task ClearAndConsumeOldRealmRewards_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.IAccountEntity)__deltaObj__).ClearAndConsumeOldRealmRewards();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IAccountEntity_ConsumeRewards__Message")]
            internal static async System.Threading.Tasks.Task ConsumeRewards_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.IAccountEntity)__deltaObj__).ConsumeRewards();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IAccountEntity_DeleteAccountCharacter_Guid_Message")]
            internal static async System.Threading.Tasks.Task DeleteAccountCharacter_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid characterId;
                (characterId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IAccountEntity)__deltaObj__).DeleteAccountCharacter(characterId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IAccountEntity_CreateNewCharacter_String_Guid_Message")]
            internal static async System.Threading.Tasks.Task CreateNewCharacter_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string name;
                (name, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid accountId;
                (accountId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IAccountEntity)__deltaObj__).CreateNewCharacter(name, accountId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "IAccountEntity_SetGender_GenderDef_Message")]
            internal static async System.Threading.Tasks.Task SetGender_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Aspects.Misc.GenderDef val;
                (val, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Aspects.Misc.GenderDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.IAccountEntity)__deltaObj__).SetGender(val);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "IAccountEntity_TryConsumeUnconsumedExp_Int32_Message")]
            internal static async System.Threading.Tasks.Task TryConsumeUnconsumedExp_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int val;
                (val, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.IAccountEntity)__deltaObj__).TryConsumeUnconsumedExp(val);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "IAccountEntity_CalcAccLevel__Message")]
            internal static async System.Threading.Tasks.Task CalcAccLevel_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.IAccountEntity)__deltaObj__).CalcAccLevel();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "IAccountEntity_AddExperience_Int32_Message")]
            internal static async System.Threading.Tasks.Task AddExperience_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int deltaVal;
                (deltaVal, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IAccountEntity)__deltaObj__).AddExperience(deltaVal);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}