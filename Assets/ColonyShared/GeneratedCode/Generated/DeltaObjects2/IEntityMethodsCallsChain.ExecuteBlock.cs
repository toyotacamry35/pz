// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class EntityMethodsCallsChain
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IEntityMethodsCallsChain_GetCurrentChainBlock__Message")]
            internal static async System.Threading.Tasks.Task GetCurrentChainBlock_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).GetCurrentChainBlock();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IEntityMethodsCallsChain_TryGetNextEntityToCall__Message")]
            internal static async System.Threading.Tasks.Task TryGetNextEntityToCall_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).TryGetNextEntityToCall();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IEntityMethodsCallsChain_CreateFork_Int32_Message")]
            internal static async System.Threading.Tasks.Task CreateFork_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).CreateFork(index);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IEntityMethodsCallsChain_ForkFinished_Guid_Message")]
            internal static async System.Threading.Tasks.Task ForkFinished_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid forkId;
                (forkId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).ForkFinished(forkId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IEntityMethodsCallsChain_GetDescription__Message")]
            internal static async System.Threading.Tasks.Task GetDescription_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).GetDescription();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IEntityMethodsCallsChain_SetNextTimeToCall_Int64_Message")]
            internal static async System.Threading.Tasks.Task SetNextTimeToCall_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                long nextTimeToCall;
                (nextTimeToCall, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<long>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).SetNextTimeToCall(nextTimeToCall);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "IEntityMethodsCallsChain_IncrementCurrentChainIndex__Message")]
            internal static async System.Threading.Tasks.Task IncrementCurrentChainIndex_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).IncrementCurrentChainIndex();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "IEntityMethodsCallsChain_DecrementCurrentChainIndex__Message")]
            internal static async System.Threading.Tasks.Task DecrementCurrentChainIndex_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObj__).DecrementCurrentChainIndex();
            }
        }
    }
}