// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class CraftEngine
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ICraftEngine_SetResultContainerAddress_PropertyAddress_Message")]
            internal static async System.Threading.Tasks.Task SetResultContainerAddress_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.PropertyAddress resultContainerAddress;
                (resultContainerAddress, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.PropertyAddress>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).SetResultContainerAddress(resultContainerAddress);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ICraftEngine_UpdateFuelTime__Message")]
            internal static async System.Threading.Tasks.Task UpdateFuelTime_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).UpdateFuelTime();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ICraftEngine_UpdateCraftingTime__Message")]
            internal static async System.Threading.Tasks.Task UpdateCraftingTime_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).UpdateCraftingTime();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ICraftEngine_UpdateRepairTime_PropertyAddress_Int32_Message")]
            internal static async System.Threading.Tasks.Task UpdateRepairTime_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.PropertyAddress itemAddress;
                (itemAddress, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.PropertyAddress>(__data__, __offset__, 0, chainContext, argumentRefs);
                int itemIndex;
                (itemIndex, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).UpdateRepairTime(itemAddress, itemIndex);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ICraftEngine_CanRun__Message")]
            internal static async System.Threading.Tasks.Task CanRun_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).CanRun();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ICraftEngine_RunCraft__Message")]
            internal static async System.Threading.Tasks.Task RunCraft_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).RunCraft();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "ICraftEngine_StopCraft__Message")]
            internal static async System.Threading.Tasks.Task StopCraft_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).StopCraft();
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "ICraftEngine_Craft_CraftRecipeDef_Int32_Int32_Int32_Int32_PropertyAddress_PropertyAddress_Message")]
            internal static async System.Threading.Tasks.Task Craft_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef recipe;
                (recipe, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                int variantIdx;
                (variantIdx, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                int[] mandatorySlotPermutation;
                (mandatorySlotPermutation, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int[]>(__data__, __offset__, 3, chainContext, argumentRefs);
                int[] optionalSlotPermutation;
                (optionalSlotPermutation, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int[]>(__data__, __offset__, 4, chainContext, argumentRefs);
                SharedCode.EntitySystem.PropertyAddress inventoryAddress;
                (inventoryAddress, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.PropertyAddress>(__data__, __offset__, 5, chainContext, argumentRefs);
                SharedCode.EntitySystem.PropertyAddress inventoryAddress2;
                (inventoryAddress2, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.PropertyAddress>(__data__, __offset__, 6, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).Craft(recipe, variantIdx, count, mandatorySlotPermutation, optionalSlotPermutation, inventoryAddress, inventoryAddress2);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "ICraftEngine_Repair_PropertyAddress_Int32_Int32_Int32_Int32_Int32_PropertyAddress_PropertyAddress_Message")]
            internal static async System.Threading.Tasks.Task Repair_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.EntitySystem.PropertyAddress itemAddress;
                (itemAddress, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.PropertyAddress>(__data__, __offset__, 0, chainContext, argumentRefs);
                int itemIndex;
                (itemIndex, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int recipeIndex;
                (recipeIndex, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                int variantIdx;
                (variantIdx, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 3, chainContext, argumentRefs);
                int[] mandatorySlotPermutation;
                (mandatorySlotPermutation, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int[]>(__data__, __offset__, 4, chainContext, argumentRefs);
                int[] optionalSlotPermutation;
                (optionalSlotPermutation, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int[]>(__data__, __offset__, 5, chainContext, argumentRefs);
                SharedCode.EntitySystem.PropertyAddress fromInventoryAddress;
                (fromInventoryAddress, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.PropertyAddress>(__data__, __offset__, 6, chainContext, argumentRefs);
                SharedCode.EntitySystem.PropertyAddress fromInventoryAddress2;
                (fromInventoryAddress2, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.EntitySystem.PropertyAddress>(__data__, __offset__, 7, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).Repair(itemAddress, itemIndex, recipeIndex, variantIdx, mandatorySlotPermutation, optionalSlotPermutation, fromInventoryAddress, fromInventoryAddress2);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "ICraftEngine_RemoveCraft_Int32_Message")]
            internal static async System.Threading.Tasks.Task RemoveCraft_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int recipeIndex;
                (recipeIndex, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).RemoveCraft(recipeIndex);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "ICraftEngine_SwapCraft_Int32_Int32_Message")]
            internal static async System.Threading.Tasks.Task SwapCraft_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int index1;
                (index1, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index2;
                (index2, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).SwapCraft(index1, index2);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(11, "ICraftEngine_StopCraftWithWorkbench_WorkbenchTypeDef_Message")]
            internal static async System.Threading.Tasks.Task StopCraftWithWorkbench_11(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Aspects.Item.Templates.WorkbenchTypeDef workbenchType;
                (workbenchType, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Item.Templates.WorkbenchTypeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).StopCraftWithWorkbench(workbenchType);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(12, "ICraftEngine_ContinueCraftWithWorkbench_WorkbenchTypeDef_Message")]
            internal static async System.Threading.Tasks.Task ContinueCraftWithWorkbench_12(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Aspects.Item.Templates.WorkbenchTypeDef workbenchType;
                (workbenchType, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Item.Templates.WorkbenchTypeDef>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.Entities.Engine.ICraftEngine)__deltaObj__).ContinueCraftWithWorkbench(workbenchType);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}