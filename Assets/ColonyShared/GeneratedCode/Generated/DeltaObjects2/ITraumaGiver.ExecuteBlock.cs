// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TraumaGiver
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ITraumaGiver_StartTrauma_IEntity_Message")]
            internal static async System.Threading.Tasks.Task StartTrauma_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.IEntity entity;
                (entity, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.IEntity>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((Src.Aspects.Impl.Stats.ITraumaGiver)__deltaObj__).StartTrauma(entity);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ITraumaGiver_StopTrauma_IEntity_Message")]
            internal static async System.Threading.Tasks.Task StopTrauma_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.IEntity entity;
                (entity, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.IEntity>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((Src.Aspects.Impl.Stats.ITraumaGiver)__deltaObj__).StopTrauma(entity);
            }
        }
    }
}