// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class LinksEngine
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ILinksEngine_SetLinksFromScene_Dictionary_Message")]
            internal static async System.Threading.Tasks.Task SetLinksFromScene_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                System.Collections.Generic.Dictionary<Assets.ResourceSystem.Aspects.Links.LinkTypeDef, System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>> links;
                (links, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<System.Collections.Generic.Dictionary<Assets.ResourceSystem.Aspects.Links.LinkTypeDef, System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>>>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((Assets.ColonyShared.SharedCode.Entities.Engine.ILinksEngine)__deltaObj__).SetLinksFromScene(links);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ILinksEngine_GetLinked_LinkTypeDef_Message")]
            internal static async System.Threading.Tasks.Task GetLinked_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef link;
                (link, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((Assets.ColonyShared.SharedCode.Entities.Engine.ILinksEngine)__deltaObj__).GetLinked(link);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ILinksEngine_GetLinkeds_LinkTypeDef_Message")]
            internal static async System.Threading.Tasks.Task GetLinkeds_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef link;
                (link, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((Assets.ColonyShared.SharedCode.Entities.Engine.ILinksEngine)__deltaObj__).GetLinkeds(link);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ILinksEngine_AddLinkRef_LinkTypeDef_OuterRef_Boolean_Boolean_Message")]
            internal static async System.Threading.Tasks.Task AddLinkRef_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef key;
                (key, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> outerRef;
                (outerRef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 1, chainContext, argumentRefs);
                bool watched;
                (watched, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool saved;
                (saved, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                await ((Assets.ColonyShared.SharedCode.Entities.Engine.ILinksEngine)__deltaObj__).AddLinkRef(key, outerRef, watched, saved);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ILinksEngine_RemoveLinkKey_LinkTypeDef_Message")]
            internal static async System.Threading.Tasks.Task RemoveLinkKey_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef key;
                (key, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((Assets.ColonyShared.SharedCode.Entities.Engine.ILinksEngine)__deltaObj__).RemoveLinkKey(key);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ILinksEngine_RemoveLinkRef_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task RemoveLinkRef_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> outerRef;
                (outerRef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((Assets.ColonyShared.SharedCode.Entities.Engine.ILinksEngine)__deltaObj__).RemoveLinkRef(outerRef);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "ILinksEngine_RemoveLinkRefByKey_LinkTypeDef_OuterRef_Message")]
            internal static async System.Threading.Tasks.Task RemoveLinkRefByKey_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ResourceSystem.Aspects.Links.LinkTypeDef key;
                (key, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ResourceSystem.Aspects.Links.LinkTypeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> outerRef;
                (outerRef, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((Assets.ColonyShared.SharedCode.Entities.Engine.ILinksEngine)__deltaObj__).RemoveLinkRefByKey(key, outerRef);
            }
        }
    }
}