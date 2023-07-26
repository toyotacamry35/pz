// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldBaken
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IWorldBaken_SetCooldown__Message")]
            internal static async System.Threading.Tasks.Task SetCooldown_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Entities.IWorldBaken)__deltaObj__).SetCooldown();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IWorldBaken_GetVerticalSpawnPointDistance__Message")]
            internal static async System.Threading.Tasks.Task GetVerticalSpawnPointDistance_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.IWorldBaken)__deltaObj__).GetVerticalSpawnPointDistance();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IWorldBaken_NameSet_String_Message")]
            internal static async System.Threading.Tasks.Task NameSet_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IWorldBaken)__deltaObj__).NameSet(value);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IWorldBaken_PrefabSet_String_Message")]
            internal static async System.Threading.Tasks.Task PrefabSet_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IWorldBaken)__deltaObj__).PrefabSet(value);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IWorldBaken_GetIncomingDamageMultiplier__Message")]
            internal static async System.Threading.Tasks.Task GetIncomingDamageMultiplier_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.IWorldBaken)__deltaObj__).GetIncomingDamageMultiplier();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}