// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class VisibilityEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IVisibilityEntity_Update__Message")]
            internal static async System.Threading.Tasks.Task Update_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.MovementSync.IVisibilityEntity)__deltaObj__).Update();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IVisibilityEntity_ForceUnsubscribeAll_Guid_Message")]
            internal static async System.Threading.Tasks.Task ForceUnsubscribeAll_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid user;
                (user, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.MovementSync.IVisibilityEntity)__deltaObj__).ForceUnsubscribeAll(user);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}