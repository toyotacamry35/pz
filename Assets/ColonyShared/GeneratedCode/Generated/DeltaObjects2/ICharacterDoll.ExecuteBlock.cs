// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterDoll
    {
        internal static class ExecuteMethods
        {
            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(0, "ICharacterDoll_CanAddUsedSlot_ResourceIDFull_Message")]
            internal static async System.Threading.Tasks.Task CanAddUsedSlot_0(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.ResourcesSystem.Base.ResourceIDFull dollSlotRes;
                (dollSlotRes, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.ResourcesSystem.Base.ResourceIDFull>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).CanAddUsedSlot(dollSlotRes);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(1, "ICharacterDoll_AddUsedSlot_ResourceIDFull_Message")]
            internal static async System.Threading.Tasks.Task AddUsedSlot_1(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.ResourcesSystem.Base.ResourceIDFull dollSlotRes;
                (dollSlotRes, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.ResourcesSystem.Base.ResourceIDFull>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).AddUsedSlot(dollSlotRes);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(2, "ICharacterDoll_RemoveUsedSlot_ResourceIDFull_Message")]
            internal static async System.Threading.Tasks.Task RemoveUsedSlot_2(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                Assets.Src.ResourcesSystem.Base.ResourceIDFull dollSlotRes;
                (dollSlotRes, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<Assets.Src.ResourcesSystem.Base.ResourceIDFull>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).RemoveUsedSlot(dollSlotRes);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(3, "ICharacterDoll_AddBlockedForUsageSlots_SlotDef_Message")]
            internal static async System.Threading.Tasks.Task AddBlockedForUsageSlots_3(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Aspects.Item.Templates.SlotDef[] slots;
                (slots, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Item.Templates.SlotDef[]>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).AddBlockedForUsageSlots(slots);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(4, "ICharacterDoll_RemoveBlockedForUsageSlots_SlotDef_Message")]
            internal static async System.Threading.Tasks.Task RemoveBlockedForUsageSlots_4(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                SharedCode.Aspects.Item.Templates.SlotDef[] slots;
                (slots, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<SharedCode.Aspects.Item.Templates.SlotDef[]>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).RemoveBlockedForUsageSlots(slots);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(5, "ICharacterDoll_GetMaxWeigth__Message")]
            internal static async System.Threading.Tasks.Task GetMaxWeigth_5(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).GetMaxWeigth();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(6, "ICharacterDoll_GetTotalWeight__Message")]
            internal static async System.Threading.Tasks.Task GetTotalWeight_6(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).GetTotalWeight();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(7, "ICharacterDoll_GetMaxStackForSlot_Int32_Message")]
            internal static async System.Threading.Tasks.Task GetMaxStackForSlot_7(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                int __offset__ = 0;
                int destinationSlot;
                (destinationSlot, __offset__) = await SharedCode.EntitySystem.ChainCalls.ChainCallHelpers.GetArg<int>(__data__, __offset__, 0, chainContext, argumentRefs);
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).GetMaxStackForSlot(destinationSlot);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(8, "ICharacterDoll_IgnoreMaxStack__Message")]
            internal static async System.Threading.Tasks.Task IgnoreMaxStack_8(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).IgnoreMaxStack();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(9, "ICharacterDoll_CanAutoselectEmptySlotsForAddStacks__Message")]
            internal static async System.Threading.Tasks.Task CanAutoselectEmptySlotsForAddStacks_9(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
            {
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).CanAutoselectEmptySlotsForAddStacks();
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(10, "ICharacterDoll_CanAdd_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanAdd_10(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
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
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).CanAdd(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(11, "ICharacterDoll_CanRemove_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task CanRemove_11(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
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
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).CanRemove(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(12, "ICharacterDoll_OnItemAdded_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnItemAdded_12(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
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
                await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).OnItemAdded(item, index, count, manual);
            }

            [SharedCode.EntitySystem.RpcMethodExecuteFuncAttribute(13, "ICharacterDoll_OnBeforeItemRemoved_IItem_Int32_Int32_Boolean_Message")]
            internal static async System.Threading.Tasks.Task OnBeforeItemRemoved_13(SharedCode.EntitySystem.IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, System.Collections.Generic.Dictionary<int, string> argumentRefs)
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
                var __result__ = await ((SharedCode.DeltaObjects.ICharacterDoll)__deltaObj__).OnBeforeItemRemoved(item, index, count, manual);
                if (!string.IsNullOrEmpty(saveResultKey))
                    await chainContext.SetContextValue(saveResultKey, __result__);
            }
        }
    }
}