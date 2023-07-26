// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TemporaryPerks
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ITemporaryPerks_AddPerkSlot_Int32_ItemTypeResource_Message")]
            internal static async System.Threading.Tasks.Task AddPerkSlot_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int slotId;
                (slotId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Aspects.Item.Templates.ItemTypeResource perkSlotType;
                (perkSlotType, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Item.Templates.ItemTypeResource>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).AddPerkSlot(slotId, perkSlotType);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ITemporaryPerks_RemovePerkSlot_Int32_Message")]
            internal static async System.Threading.Tasks.Task RemovePerkSlot_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int slotId;
                (slotId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).RemovePerkSlot(slotId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ITemporaryPerks_GetMaxWeigth__Message")]
            internal static async System.Threading.Tasks.Task GetMaxWeigth_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).GetMaxWeigth();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ITemporaryPerks_GetTotalWeight__Message")]
            internal static async System.Threading.Tasks.Task GetTotalWeight_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).GetTotalWeight();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ITemporaryPerks_GetMaxStackForSlot_Int32_Message")]
            internal static async System.Threading.Tasks.Task GetMaxStackForSlot_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int destinationSlot;
                (destinationSlot, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).GetMaxStackForSlot(destinationSlot);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ITemporaryPerks_IgnoreMaxStack__Message")]
            internal static async System.Threading.Tasks.Task IgnoreMaxStack_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).IgnoreMaxStack();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "ITemporaryPerks_CanAutoselectEmptySlotsForAddStacks__Message")]
            internal static async System.Threading.Tasks.Task CanAutoselectEmptySlotsForAddStacks_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).CanAutoselectEmptySlotsForAddStacks();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "ITemporaryPerks_CanAdd_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanAdd_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).CanAdd(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "ITemporaryPerks_CanRemove_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanRemove_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).CanRemove(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "ITemporaryPerks_OnItemAdded_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnItemAdded_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).OnItemAdded(item, index, count, manual);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "ITemporaryPerks_OnBeforeItemRemoved_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnBeforeItemRemoved_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ITemporaryPerks)__deltaObj__).OnBeforeItemRemoved(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class PermanentPerks
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "IPermanentPerks_AddPerkSlot_Int32_ItemTypeResource_Message")]
            internal static async System.Threading.Tasks.Task AddPerkSlot_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int slotId;
                (slotId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Aspects.Item.Templates.ItemTypeResource perkSlotType;
                (perkSlotType, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Item.Templates.ItemTypeResource>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).AddPerkSlot(slotId, perkSlotType);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "IPermanentPerks_RemovePerkSlot_Int32_Message")]
            internal static async System.Threading.Tasks.Task RemovePerkSlot_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int slotId;
                (slotId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).RemovePerkSlot(slotId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "IPermanentPerks_GetMaxWeigth__Message")]
            internal static async System.Threading.Tasks.Task GetMaxWeigth_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).GetMaxWeigth();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "IPermanentPerks_GetTotalWeight__Message")]
            internal static async System.Threading.Tasks.Task GetTotalWeight_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).GetTotalWeight();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "IPermanentPerks_GetMaxStackForSlot_Int32_Message")]
            internal static async System.Threading.Tasks.Task GetMaxStackForSlot_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int destinationSlot;
                (destinationSlot, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).GetMaxStackForSlot(destinationSlot);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "IPermanentPerks_IgnoreMaxStack__Message")]
            internal static async System.Threading.Tasks.Task IgnoreMaxStack_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).IgnoreMaxStack();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "IPermanentPerks_CanAutoselectEmptySlotsForAddStacks__Message")]
            internal static async System.Threading.Tasks.Task CanAutoselectEmptySlotsForAddStacks_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).CanAutoselectEmptySlotsForAddStacks();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "IPermanentPerks_CanAdd_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanAdd_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).CanAdd(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "IPermanentPerks_CanRemove_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanRemove_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).CanRemove(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "IPermanentPerks_OnItemAdded_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnItemAdded_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).OnItemAdded(item, index, count, manual);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "IPermanentPerks_OnBeforeItemRemoved_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnBeforeItemRemoved_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.IPermanentPerks)__deltaObj__).OnBeforeItemRemoved(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class SavedPerks
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ISavedPerks_AddPerkSlot_Int32_ItemTypeResource_Message")]
            internal static async System.Threading.Tasks.Task AddPerkSlot_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int slotId;
                (slotId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                SharedCode.Aspects.Item.Templates.ItemTypeResource perkSlotType;
                (perkSlotType, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Item.Templates.ItemTypeResource>(__data__, __offset__, 1, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).AddPerkSlot(slotId, perkSlotType);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ISavedPerks_RemovePerkSlot_Int32_Message")]
            internal static async System.Threading.Tasks.Task RemovePerkSlot_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int slotId;
                (slotId, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).RemovePerkSlot(slotId);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ISavedPerks_GetMaxWeigth__Message")]
            internal static async System.Threading.Tasks.Task GetMaxWeigth_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).GetMaxWeigth();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ISavedPerks_GetTotalWeight__Message")]
            internal static async System.Threading.Tasks.Task GetTotalWeight_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).GetTotalWeight();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ISavedPerks_GetMaxStackForSlot_Int32_Message")]
            internal static async System.Threading.Tasks.Task GetMaxStackForSlot_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int destinationSlot;
                (destinationSlot, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).GetMaxStackForSlot(destinationSlot);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ISavedPerks_IgnoreMaxStack__Message")]
            internal static async System.Threading.Tasks.Task IgnoreMaxStack_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).IgnoreMaxStack();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "ISavedPerks_CanAutoselectEmptySlotsForAddStacks__Message")]
            internal static async System.Threading.Tasks.Task CanAutoselectEmptySlotsForAddStacks_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).CanAutoselectEmptySlotsForAddStacks();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "ISavedPerks_CanAdd_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanAdd_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).CanAdd(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "ISavedPerks_CanRemove_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanRemove_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).CanRemove(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "ISavedPerks_OnItemAdded_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnItemAdded_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).OnItemAdded(item, index, count, manual);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "ISavedPerks_OnBeforeItemRemoved_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnBeforeItemRemoved_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Entities.IItem item;
                (item, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Entities.IItem>(__data__, __offset__, 0, chainContext, argumentRefs);
                int index;
                (index, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 1, chainContext, argumentRefs);
                int count;
                (count, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 2, chainContext, argumentRefs);
                bool manual;
                (manual, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<bool>(__data__, __offset__, 3, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ISavedPerks)__deltaObj__).OnBeforeItemRemoved(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}