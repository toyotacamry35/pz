// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class LegionaryEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ILegionaryEntity_InvokeHitZonesDamageReceivedEvent_Damage_Message")]
            internal static async System.Threading.Tasks.Task InvokeHitZonesDamageReceivedEvent_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ColonyShared.SharedCode.Aspects.Damage.Damage damage;
                (damage, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ColonyShared.SharedCode.Aspects.Damage.Damage>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.AI.ILegionaryEntity)__deltaObj__).InvokeHitZonesDamageReceivedEvent(damage);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ILegionaryEntity_NameSet_String_Message")]
            internal static async System.Threading.Tasks.Task NameSet_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.AI.ILegionaryEntity)__deltaObj__).NameSet(value);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ILegionaryEntity_PrefabSet_String_Message")]
            internal static async System.Threading.Tasks.Task PrefabSet_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.AI.ILegionaryEntity)__deltaObj__).PrefabSet(value);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ILegionaryEntity_Destroy__Message")]
            internal static async System.Threading.Tasks.Task Destroy_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.AI.ILegionaryEntity)__deltaObj__).Destroy();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ILegionaryEntity_GetIncomingDamageMultiplier__Message")]
            internal static async System.Threading.Tasks.Task GetIncomingDamageMultiplier_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.AI.ILegionaryEntity)__deltaObj__).GetIncomingDamageMultiplier();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}