// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldBench
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IWorldBench_NameSet_String_Message")]
            internal static async System.Threading.Tasks.Task NameSet_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IWorldBench)__deltaObj__).NameSet(value);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IWorldBench_PrefabSet_String_Message")]
            internal static async System.Threading.Tasks.Task PrefabSet_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string value;
                (value, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IWorldBench)__deltaObj__).PrefabSet(value);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IWorldBench_GetOpenOuterRef_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task GetOpenOuterRef_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Utils.OuterRef oref;
                (oref, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Utils.OuterRef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IWorldBench)__deltaObj__).GetOpenOuterRef(oref);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}