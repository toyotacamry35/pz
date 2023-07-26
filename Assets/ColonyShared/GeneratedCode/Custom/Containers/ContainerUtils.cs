using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.Custom.Containers
{
    public static class ContainerUtils
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<PropertyAddress> GetPropertyAddress(OuterRef<IEntity> entityRef, ContainerType containerType, IEntitiesRepository repo)
        {
            using (var fromWrapper = await repo.Get(entityRef))
            {
                return GetPropertyAddress(fromWrapper, entityRef, containerType, repo);
            }
        }

        public static PropertyAddress GetPropertyAddress(IEntitiesContainer wrapper, OuterRef<IEntity> entityRef, ContainerType containerType, IEntitiesRepository repo)
        {
            switch (containerType)
            {
                case ContainerType.Doll:
                    {
                        var entity = wrapper.Get<IHasDollClientFull>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientFull);
                        return EntityPropertyResolver.GetPropertyAddress(entity.Doll);
                    }
                case ContainerType.Inventory:
                    {
                        var entity = wrapper.Get<IHasInventoryClientFull>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientFull);
                        return EntityPropertyResolver.GetPropertyAddress(entity.Inventory);
                    }
                default:
                    break;
            }

            return default;
        }

        private static bool CanStack(IItem item1, IItem item2)
        {
            if (item1 == null || item2 == null)
                return false;

            if (item1.ItemResource.MaxStack <= 1 || item2.ItemResource.MaxStack <= 1)
                return false;

            if (item1.ItemResource != item2.ItemResource)
                return false;

            return true;
        }

        private static async Task MovetoStack(ISlotItem fromSlot, ISlotItem toSlot, int count)
        {
            var toSlotCount = toSlot.Stack;
            var fromSlotCount = count;
            var total = toSlotCount + fromSlotCount;
            var toSlotDurability = await toSlot.Item.Health.GetHealthCurrent();
            var fromSlotDurability = await fromSlot.Item.Health.GetHealthCurrent();
            var newDurability = (toSlotCount * toSlotDurability + fromSlotCount * fromSlotDurability) / total;
            await toSlot.Item.Health.ChangeHealth(newDurability - toSlotDurability);

            //Logger.IfInfo()?.Message($"from = ({fromSlotCount}: {fromSlotDurability}) + to = ({toSlotCount}: {toSlotDurability}) -> total = ({total}: {newDurability})").Write();

            toSlot.Stack += count;
            fromSlot.Stack -= count;
        }

        private static async Task<int> CanAddCount(IItemsContainer container, IItem itemToAdd, int index, int count, Guid checkTransactionId, bool ignoreItemInSlot = false)
        {
            var itemResourceMaxStack = itemToAdd.ItemResource?.MaxStack;
            var slotMaxStack = Math.Min(itemResourceMaxStack ?? int.MaxValue, await container.GetMaxStackForSlot(index));
            var maxStack = slotMaxStack > 1 ? (await container.IgnoreMaxStack() ? int.MaxValue : slotMaxStack) : slotMaxStack;

            if (await container.CanAutoselectEmptySlotsForAddStacks())
            {
                var currentCount = count;

                if (maxStack > 1)
                    foreach (var pair in container.Items)
                        if (CanStack(pair.Value.Item, itemToAdd) && 
                            (!container.TransactionReservedSlots.TryGetValue(pair.Key, out Guid currentTransactionId) || checkTransactionId == currentTransactionId))
                        {
                            currentCount -= maxStack - pair.Value.Stack;
                            if (currentCount <= 0)
                                return count;
                        }

                var emptySlots = container.Size - container.Items.Count;
                if (ignoreItemInSlot && container.Items.ContainsKey(index))
                    emptySlots++;
                var maxEmptyCount = maxStack == int.MaxValue && emptySlots > 0 ? int.MaxValue : maxStack * emptySlots;

                return maxEmptyCount >= currentCount ? count : count - currentCount + maxEmptyCount;
            }

            ISlotItem slot;
            if (!ignoreItemInSlot && container.Items.TryGetValue(index, out slot))
                if (CanStack(itemToAdd, slot.Item))
                    return Math.Min(count, maxStack - slot.Stack);
                else
                    return 0;

            return Math.Min(count, maxStack);
        }

        private static async Task AddToEmptySlot(IItemsContainer container, ISlotItem slotToAdd, int index, int partCountToAdd, IItem clonedItem, bool manual)
        {
            ISlotItem slotToInsert = new SlotItem
            {
                Item = clonedItem ?? slotToAdd.Item,
                Stack = 0
            };

            container.Items[index] = slotToInsert;
            await MovetoStack(slotToAdd, slotToInsert, partCountToAdd);
            await container.OnItemAdded(slotToAdd.Item, index, partCountToAdd, manual);
        }

        private static async Task<ContainerItemOperation> AddStackableItemToContainer(IItemsContainer container, ISlotItem slotItemToAdd, int count, int index, bool manual)
        {
            var capacity = container.Size;
            var items = container.Items;
            var itemToAdd = slotItemToAdd.Item;
            var maxStack = await container.IgnoreMaxStack() ? int.MaxValue : itemToAdd.ItemResource?.MaxStack ?? int.MaxValue;

            var addCount = count;
            var result = new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorUnknown };
            bool alwaysClone = slotItemToAdd.Stack > count;

            if (index >= 0)
            {
                var partCountToAdd = Math.Min(addCount, maxStack);
                ISlotItem dstSlotItem;
                if (!items.TryGetValue(index, out dstSlotItem))
                {
                    IItem clonedItem = null;
                    bool clone = alwaysClone || partCountToAdd < slotItemToAdd.Stack;
                    if (clone)
                        clonedItem = await CloneItem(slotItemToAdd.Item);

                    if (!items.TryGetValue(index, out dstSlotItem))
                    {
                        await AddToEmptySlot(container, slotItemToAdd, index, partCountToAdd, clonedItem, manual);

                        addCount -= partCountToAdd;
                        result.Result = ContainerItemOperationResult.Success;
                        result.ItemsCount += partCountToAdd;
                        alwaysClone = true;
                    }
                }
                else
                {
                    var dstItem = dstSlotItem.Item;
                    if (CanStack(itemToAdd, dstItem))
                    {
                        partCountToAdd = Math.Min(partCountToAdd, maxStack - dstSlotItem.Stack);
                        await MovetoStack(slotItemToAdd, dstSlotItem, partCountToAdd);
                        addCount -= partCountToAdd;

                        result.Result = ContainerItemOperationResult.Success;
                        result.ItemsCount += partCountToAdd;

                        await container.OnItemAdded(dstItem, index, partCountToAdd, manual);
                    }
                }
            }

            if (await container.CanAutoselectEmptySlotsForAddStacks())
            {
                var resourceMaxItem = itemToAdd.ItemResource?.MaxStack;
                if (addCount > 0 && (resourceMaxItem ?? int.MaxValue) > 1)
                    foreach (var pair in items)
                    {
                        if (container.TransactionReservedSlots.ContainsKey(pair.Key))
                            continue;

                        var item = pair.Value.Item;
                        if (CanStack(item, itemToAdd))
                        {
                            var moveCount = addCount;
                            if (moveCount > maxStack - pair.Value.Stack)
                                moveCount = maxStack - pair.Value.Stack;

                            if (moveCount <= 0)
                                continue;

                            await MovetoStack(slotItemToAdd, pair.Value, moveCount);
                            await container.OnItemAdded(item, pair.Key, moveCount, manual);

                            addCount -= moveCount;
                            result.Result = ContainerItemOperationResult.Success;
                            result.ItemsCount += moveCount;

                            if (pair.Value.Stack <= 0)
                                break;
                        }
                    }

                bool tryAdd = true;
                IItem clonedItem = null;
                while (addCount > 0 && tryAdd)
                {
                    tryAdd = false;
                    var partCountToAdd = Math.Min(addCount, maxStack);
                    for (var addIndex = 0; addIndex < capacity; addIndex++)
                    {
                        if (!items.ContainsKey(addIndex) && !container.TransactionReservedSlots.ContainsKey(addIndex))
                        {
                            if (!await container.CanAdd(itemToAdd, addIndex, partCountToAdd, manual))
                                continue;

                            if (clonedItem == null)
                            {
                                bool clone = alwaysClone || partCountToAdd < slotItemToAdd.Stack;
                                if (clone)
                                    clonedItem = await CloneItem(slotItemToAdd.Item);
                            }

                            if (items.ContainsKey(addIndex) || container.TransactionReservedSlots.ContainsKey(addIndex))
                                continue;

                            await AddToEmptySlot(container, slotItemToAdd, addIndex, partCountToAdd, clonedItem, manual);
                            clonedItem = null;

                            addCount -= partCountToAdd;
                            tryAdd = true;
                            result.Result = ContainerItemOperationResult.Success;
                            result.ItemsCount += partCountToAdd;
                            alwaysClone = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        internal static async Task<ISlotItem> CanRemoveItem(IItemsContainer container, int destinationSlotId, int count, bool manual, Guid clientSrcEntityId)
        {
            ISlotItem itemToRemove;
            if (destinationSlotId < 0)
                return null;

            if (container.Items.TryGetValue(destinationSlotId, out itemToRemove) && container.TransactionReservedSlots.ContainsKey(destinationSlotId))
                return null;

            if (itemToRemove == null)
                return null;

            if (destinationSlotId >= container.Size)
                return null;

            if (itemToRemove.Stack < count)
                return null;

            if (!await container.CanRemove(itemToRemove.Item, destinationSlotId, count, manual))
                return null;

            if (manual && clientSrcEntityId != itemToRemove.Item.Id)
                return null;

            return itemToRemove;
        }

        internal static async Task<ValueTuple<ContainerItemOperation, ISlotItem>> CanAddItem(IItemsContainer container, IItem item, int index, int count, bool manual, Guid checkTransactionId)
        {
            ISlotItem currentItem = null;
            ISlotItem itemToSwitch = null;
            Guid currentTransactionId = Guid.Empty;
            if (container.Items.TryGetValue(index, out currentItem))
            {
                container.TransactionReservedSlots.TryGetValue(index, out currentTransactionId);
                if (!CanStack(currentItem.Item, item))
                {
                    var canRemoveSwitchItem = await container.CanRemove(currentItem.Item, index, currentItem.Stack, manual);
                    if (canRemoveSwitchItem)
                        itemToSwitch = currentItem;
                    else
                        return ValueTuple.Create(new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcCantAdd }, itemToSwitch);
                }
            }

            if (currentTransactionId != checkTransactionId)
                return ValueTuple.Create(new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorChangeTransactionNotFound }, itemToSwitch);

            if (index >= container.Size)
                return ValueTuple.Create(new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstCantAdd }, itemToSwitch);

            if (!await container.CanAdd(item, index, count, manual))
                return ValueTuple.Create(new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstCantAdd }, itemToSwitch);

            int countToAdd = await CanAddCount(container, item, index, count, checkTransactionId, itemToSwitch != null);
            if (countToAdd <= 0)
                return ValueTuple.Create(new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstCantAdd }, itemToSwitch);

            return ValueTuple.Create(new ContainerItemOperation() { ItemsCount = countToAdd, Result = ContainerItemOperationResult.Success }, itemToSwitch);
        }

        internal static async Task<ContainerItemOperation> AddItem(IItemsContainer container, IItem item, int index, int count, bool manual, Guid checkTransactionId)
        {
            if (!await container.CanAdd(item, index, count, manual))
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstCantAdd };

            var slotItemToAdd = new SlotItem
            {
                Stack = count,
                Item = item
            };

            var itemToAddSource = slotItemToAdd.Item;
            var countToAdd = await CanAddCount(container, itemToAddSource, index, count, checkTransactionId);
            if (countToAdd <= 0)
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstIsFull };

            var result = await AddStackableItemToContainer(container, slotItemToAdd, countToAdd, index, manual);
            return result;
        }

        internal static async Task<ContainerItemOperation> AddItem(IItemsContainer container, ItemResourcePack itemPack, bool manual, Guid checkTransactionId)
        {
            var newItem = await CreateItem(itemPack.ItemResource);
            if (newItem == null)
            {
                Logger.IfError()?.Message("Item prototype {0} not found", itemPack.ItemResource).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorUnknown };
            }
            foreach (var stat in itemPack.StatsModifiers ?? Enumerable.Empty<ValueStatDef>())
                await newItem.Stats.ChangeValue(stat.StatResource, stat.InitialValue);

            var res = await CanAddItem(container, newItem, itemPack.Index, (int)itemPack.Count, manual, Guid.Empty);

            var result = res.Item1;
            int countToAdd = result.ItemsCount;
            ISlotItem itemToSwitch = res.Item2;

            if (!result.IsSuccess || itemToSwitch != null)
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcCantAdd };

            var addedResult = await AddItem(container, newItem, itemPack.Index, countToAdd, manual, checkTransactionId);
            return addedResult;
        }

        internal static async Task<ContainerItemOperation> RemoveItem(IItemsContainer container, int index, int count, Guid clientEntityId, bool manual, Guid checkTransactionId)
        {
            if (index < 0)
            {
                Logger.IfError()?.Message("{1}: slotId: {0} < 0", index, container.GetType().Name).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcIncorrectSlotId };
            }

            Guid currentTransactionId;
            if (container.TransactionReservedSlots.TryGetValue(index, out currentTransactionId) && currentTransactionId != checkTransactionId)
            {
                Logger.IfError()?.Message("{1}: cant remove item: slotId {0} blocked by another transaction", index, container.GetType().Name).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorChangeTransactionNotFound };
            }

            var capacity = container.Size;
            if (index >= capacity)
            {
                Logger.IfError()?.Message("{2}: slotId: {0}  >= capacity: {1}", index, capacity, container.GetType().Name).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcIncorrectSlotId };
            }

            ISlotItem slot;
            if (!container.Items.TryGetValue(index, out slot))
            {
                Logger.IfError()?.Message("{1}: Item not found at containerIndex: {0}", index, container.GetType().Name).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcNotFound };
            }

            var itemToRemove = slot.Item;
            if (manual && itemToRemove.Id != clientEntityId)
            {
                Logger.IfError()?.Message("{2}: Server and client itemId mismatch. serverItemId: {0}, clientItemId {1}", itemToRemove.Id, clientEntityId, container.GetType().Name).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcClientMismatch };
            }

            var removeCount = count <= 0 ? slot.Stack : count;
            if (slot.Stack < count)
            {
                Logger.IfError()?.Message("{2}: currentStack: {0}  <  count {1}", slot.Stack, count, container.GetType().Name).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorUnknown };
            }

            if (!await container.CanRemove(itemToRemove, index, removeCount, manual))
            {
                Logger.IfError()?.Message("{2}: cant remove entityId: {0},  slotId {1}", itemToRemove.Id, index, container.GetType().Name).Write();
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcCantRemove };
            }

            PropertyAddress dropBoxAddress = await container.OnBeforeItemRemoved(slot.Item, index, removeCount, manual);
            slot.Stack -= removeCount;
            if (slot.Stack == 0)
            {
                container.Items.Remove(index);
            }

            return new ContainerItemOperation() { ItemsCount = removeCount, Result = ContainerItemOperationResult.Success };
        }

        public static int FindResourceSlotIndex(IItemsContainerClientFull src, BaseItemResource itemResource)
        {
            var slots = src.Items.Where(v => v.Value.Item.ItemResource == itemResource);
            if (slots.Any())
                return slots.First().Key;
            else
                return -1;
        }

        internal static async Task<IItem> CloneItem(IItem item)
        {
            var newItem = await CreateItem(item.ItemResource);
            await newItem.Stats.Copy(item.Stats);

            foreach (var pair in item.AmmoContainer.Items)
                newItem.AmmoContainer.Items.Add(pair.Key, new SlotItem
                {
                    Stack = pair.Value.Stack,
                    Item = await CloneItem(pair.Value.Item)
                });

            return newItem;
        }

        internal static async Task<IItem> CreateItem(BaseItemResource itemResource)
        {
            var newItem = new Item()
            {
                Id = Guid.NewGuid(),
                ItemResource = itemResource
            };

            var itemResourceDef = newItem.ItemResource as ItemResource;
            if (itemResourceDef != null)
            {
                await newItem.Stats.SetToDefault(true);
                newItem.AmmoContainer.Size = itemResourceDef.InnerContainerSize;
            }
            await newItem.OnInit();

            return newItem;
        }
    }
}