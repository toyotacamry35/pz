// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SessionHolderEntity
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ISessionHolderEntity_Register_Guid_Guid_Message")]
            internal static async System.Threading.Tasks.Task Register_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid guid;
                (guid, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Guid session;
                (session, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObj__).Register(guid, session);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ISessionHolderEntity_Unregister_Guid_Message")]
            internal static async System.Threading.Tasks.Task Unregister_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid guid;
                (guid, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObj__).Unregister(guid);
            }
        }
    }
}