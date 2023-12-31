// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class BotCoordinator
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IBotCoordinator_Register__Message")]
            internal static async System.Threading.Tasks.Task Register_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((GeneratedCode.EntityModel.Bots.IBotCoordinator)__deltaObj__).Register();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IBotCoordinator_Initialize_MapDef_List_LegionaryEntityDef_Message")]
            internal static async System.Threading.Tasks.Task Initialize_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                GeneratedCode.Custom.Config.MapDef mapDef;
                (mapDef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<GeneratedCode.Custom.Config.MapDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> botsRefs;
                (botsRefs, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>>(__data__, __offset__, 1, chainContext, argumentRefs);
                SharedCode.AI.LegionaryEntityDef botConfig;
                (botConfig, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.AI.LegionaryEntityDef>(__data__, __offset__, 2, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Bots.IBotCoordinator)__deltaObj__).Initialize(mapDef, botsRefs, botConfig);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IBotCoordinator_ActivateBots_Guid_List_Message")]
            internal static async System.Threading.Tasks.Task ActivateBots_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid account;
                (account, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                System.Collections.Generic.List<System.Guid> botsIds;
                (botsIds, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.List<System.Guid>>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Bots.IBotCoordinator)__deltaObj__).ActivateBots(account, botsIds);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IBotCoordinator_DeactivateBots_Guid_Message")]
            internal static async System.Threading.Tasks.Task DeactivateBots_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Guid account;
                (account, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Guid>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((GeneratedCode.EntityModel.Bots.IBotCoordinator)__deltaObj__).DeactivateBots(account);
            }
        }
    }
}