// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class LoadDataEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ILoadDataEntity_LoadEntity_Int32_Guid_Message")]
            internal static async System.Threading.Tasks.Task LoadEntity_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int typeId;
                (typeId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid entityId;
                (entityId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Cloud.ILoadDataEntity)__deltaObj__).LoadEntity(typeId, entityId);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}