// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class DialogEngine
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IDialogEngine_Next_DialogDef_Message")]
            internal static async System.Threading.Tasks.Task Next_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Aspects.Dialog.DialogDef nextDialog;
                (nextDialog, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Aspects.Dialog.DialogDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IDialogEngine)__deltaObj__).Next(nextDialog);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IDialogEngine_NextWithCheck_DialogDef_Message")]
            internal static async System.Threading.Tasks.Task NextWithCheck_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                ResourceSystem.Aspects.Dialog.DialogDef nextDialog;
                (nextDialog, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<ResourceSystem.Aspects.Dialog.DialogDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.IDialogEngine)__deltaObj__).NextWithCheck(nextDialog);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}