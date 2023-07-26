// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class ContentKeyServiceEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IContentKeyServiceEntity_EnableKey_String_Message")]
            internal static async System.Threading.Tasks.Task EnableKey_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string key;
                (key, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObj__).EnableKey(key);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IContentKeyServiceEntity_DisableKey_String_Message")]
            internal static async System.Threading.Tasks.Task DisableKey_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                string key;
                (key, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<string>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((GeneratedCode.ContentKeys.IContentKeyServiceEntity)__deltaObj__).DisableKey(key);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}