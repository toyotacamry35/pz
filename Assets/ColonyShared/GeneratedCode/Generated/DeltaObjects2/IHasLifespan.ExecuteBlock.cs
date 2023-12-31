// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class Lifespan
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ILifespan_InvokeLifespanExpiredEvent_OnLifespanExpired_Message")]
            internal static async System.Threading.Tasks.Task InvokeLifespanExpiredEvent_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired whatToDo;
                (whatToDo, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObj__).InvokeLifespanExpiredEvent(whatToDo);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ILifespan_CancelLifespanCountdown__Message")]
            internal static async System.Threading.Tasks.Task CancelLifespanCountdown_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObj__).CancelLifespanCountdown();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ILifespan_StartLifespanCountdown__Message")]
            internal static async System.Threading.Tasks.Task StartLifespanCountdown_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObj__).StartLifespanCountdown();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ILifespan_LifespanExpired__Message")]
            internal static async System.Threading.Tasks.Task LifespanExpired_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObj__).LifespanExpired();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ILifespan_GetExpiredLifespanPercent__Message")]
            internal static async System.Threading.Tasks.Task GetExpiredLifespanPercent_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObj__).GetExpiredLifespanPercent();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ILifespan_IsExpiredLifespanPercentInRange_Single_Single_Message")]
            internal static async System.Threading.Tasks.Task IsExpiredLifespanPercentInRange_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                float fromIncluding;
                (fromIncluding, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 0, chainContext, argumentRefs);
                float tillExcluding;
                (tillExcluding, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<float>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((Assets.ColonyShared.SharedCode.Entities.ILifespan)__deltaObj__).IsExpiredLifespanPercentInRange(fromIncluding, tillExcluding);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}