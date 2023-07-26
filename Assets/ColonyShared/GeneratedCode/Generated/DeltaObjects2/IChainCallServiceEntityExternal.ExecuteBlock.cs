// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class ChainCallServiceEntityExternal
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IChainCallServiceEntityExternal_ChainCall_EntityMethodsCallsChainBatch_Message")]
            internal static async System.Threading.Tasks.Task ChainCall_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch batch;
                (batch, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.ChainCalls.EntityMethodsCallsChainBatch>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Service.IChainCallServiceEntityExternal)__deltaObj__).ChainCall(batch);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IChainCallServiceEntityExternal_CancelChain_Int32_Guid_Guid_Message")]
            internal static async System.Threading.Tasks.Task CancelChain_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                System.Guid chainId;
                (chainId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((SharedCode.Entities.Service.IChainCallServiceEntityExternal)__deltaObj__).CancelChain(typeId, entityId, chainId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IChainCallServiceEntityExternal_CancelAllChain_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task CancelAllChain_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Service.IChainCallServiceEntityExternal)__deltaObj__).CancelAllChain(typeId, entityId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}