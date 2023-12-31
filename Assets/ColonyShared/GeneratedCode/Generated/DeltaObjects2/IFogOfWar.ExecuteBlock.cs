// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class FogOfWar
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IFogOfWar_DiscoverRegion_IndexedRegionDef_Message")]
            internal static async System.Threading.Tasks.Task DiscoverRegion_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.SpatialSystem.IndexedRegionDef region;
                (region, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.SpatialSystem.IndexedRegionDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.FogOfWar.IFogOfWar)__deltaObj__).DiscoverRegion(region);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IFogOfWar_SetRegionVisited_IndexedRegionDef_Message")]
            internal static async System.Threading.Tasks.Task SetRegionVisited_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.SpatialSystem.IndexedRegionDef region;
                (region, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.SpatialSystem.IndexedRegionDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.FogOfWar.IFogOfWar)__deltaObj__).SetRegionVisited(region);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IFogOfWar_DiscoverGroup_IndexedRegionGroupDef_Message")]
            internal static async System.Threading.Tasks.Task DiscoverGroup_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.SpatialSystem.IndexedRegionGroupDef group;
                (group, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.SpatialSystem.IndexedRegionGroupDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.FogOfWar.IFogOfWar)__deltaObj__).DiscoverGroup(group);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IFogOfWar_SetGroupVisited_IndexedRegionGroupDef_Message")]
            internal static async System.Threading.Tasks.Task SetGroupVisited_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.SpatialSystem.IndexedRegionGroupDef group;
                (group, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.SpatialSystem.IndexedRegionGroupDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.FogOfWar.IFogOfWar)__deltaObj__).SetGroupVisited(group);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IFogOfWar_GetGroupPercent_IndexedRegionGroupDef_Message")]
            internal static async System.Threading.Tasks.Task GetGroupPercent_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.SpatialSystem.IndexedRegionGroupDef group;
                (group, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.SpatialSystem.IndexedRegionGroupDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.FogOfWar.IFogOfWar)__deltaObj__).GetGroupPercent(group);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IFogOfWar_ClearRegions__Message")]
            internal static async System.Threading.Tasks.Task ClearRegions_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.FogOfWar.IFogOfWar)__deltaObj__).ClearRegions();
            }
        }
    }
}