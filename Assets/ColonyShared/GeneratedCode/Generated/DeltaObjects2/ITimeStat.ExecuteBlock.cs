// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeStat
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ITimeStat_ChangeValue_Single_Message")]
            internal static async System.Threading.Tasks.Task ChangeValue_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float delta;
                (delta, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((Src.Aspects.Impl.Stats.ITimeStat)__deltaObj__).ChangeValue(delta);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ITimeStat_Initialize_StatDef_Boolean_Message")]
            internal static async System.Threading.Tasks.Task Initialize_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.Aspects.Impl.Stats.StatDef statDef;
                (statDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.Aspects.Impl.Stats.StatDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                bool resetState;
                (resetState, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((Src.Aspects.Impl.Stats.ITimeStat)__deltaObj__).Initialize(statDef, resetState);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ITimeStat_RecalculateCaches_Boolean_Message")]
            internal static async System.Threading.Tasks.Task RecalculateCaches_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                bool calcersOnly;
                (calcersOnly, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((Src.Aspects.Impl.Stats.ITimeStat)__deltaObj__).RecalculateCaches(calcersOnly);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ITimeStat_GetValue__Message")]
            internal static async System.Threading.Tasks.Task GetValue_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((Src.Aspects.Impl.Stats.ITimeStat)__deltaObj__).GetValue();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}