using Assets.ColonyShared.GeneratedCode.Shared;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using ResourcesSystem.Loader;
using GeneratedCode.Custom.Containers;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Transactions;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.MapSystem;
using SharedCode.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class ContainerApi : IContainerApiImplementRemoteMethods
    {
        private Dictionary<Guid, BaseItemTransactionInfo> _currentTransactionsAddBack =
            new Dictionary<Guid, BaseItemTransactionInfo>();
        private Dictionary<Guid, BaseItemTransactionInfo> _currentTransactionsAdd
        {
            get => _currentTransactionsAddBack == null ?
                _currentTransactionsAddBack = new Dictionary<Guid, BaseItemTransactionInfo>()
                : _currentTransactionsAddBack;
        }

        private Dictionary<Guid, BaseItemTransactionInfo> _currentTransactionsRemoveBack =
            new Dictionary<Guid, BaseItemTransactionInfo>();
        private Dictionary<Guid, BaseItemTransactionInfo> _currentTransactionsRemove
        {
            get => _currentTransactionsRemoveBack == null ?
                _currentTransactionsRemoveBack = new Dictionary<Guid, BaseItemTransactionInfo>()
                : _currentTransactionsRemoveBack;
        }

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        abstract class BaseItemTransactionInfo
        {
            public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        }

        class ItemMoveTransactionInfo : BaseItemTransactionInfo
        {
            public Guid Id { get; set; }

            public PropertyAddress Address { get; set; }

            public int SlotId { get; set; }

            public IItem Item { get; set; }

            public int Count { get; set; }

            public bool Manual { get; set; }

            public bool RemoveOld { get; set; }

            public Guid ClientEntityId { get; set; }
        }

        class ItemMoveAllTransactionInfo : BaseItemTransactionInfo
        {
            public Guid Id { get; set; }

            public PropertyAddress Address { get; set; }

            public Dictionary<int, IItemWrapper> Items { get; set; }

            public bool Manual { get; set; }

            public List<int> ReservedSlots { get; set; }

            public bool SameSlots { get; set; }
        }

        public async Task<ContainerOperationAddPrepareResult> ContainerOperationMoveAddPrepareImpl(Guid transactionId,
            PropertyAddress address, int slotId, IItemWrapper itemWrapper, bool manual)
        {
            var itemsContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, address, ReplicationLevel.Master);
            var res = await ContainerUtils.CanAddItem(itemsContainer, itemWrapper.Item, slotId, itemWrapper.Count,
                manual, Guid.Empty);
            int countToAdd = res.Item1.ItemsCount;
            ISlotItem itemToSwitch = res.Item2;
            var canAdd = res.Item1.IsSuccess;
            if (!canAdd)
                return new ContainerOperationAddPrepareResult
                { ContainerItemOperationResult = ContainerItemOperationResult.ErrorDstCantAdd };

            if (slotId >= 0)
            {
                if (itemsContainer.TransactionReservedSlots.ContainsKey(slotId))
                {
                    Logger.Error("{1}: ContainerOperationMoveAddPrepare already reserved slot {0}", slotId,
                        itemsContainer.GetType().Name);
                    return new ContainerOperationAddPrepareResult
                    {
                        ContainerItemOperationResult = ContainerItemOperationResult.ErrorDstSlotLockedByOtherTransaction
                    };
                }

                itemsContainer.TransactionReservedSlots[slotId] = transactionId;
            }

            var newTransaction = new ItemMoveTransactionInfo
            {
                Id = transactionId,
                Address = address,
                SlotId = slotId,
                Count = countToAdd,
                Item = await ContainerUtils.CloneItem(itemWrapper.Item),
                Manual = manual,
                RemoveOld = itemToSwitch != null
            };

            _currentTransactionsAdd[newTransaction.Id] = newTransaction;
            return new ContainerOperationAddPrepareResult
            {
                ContainerItemOperationResult = ContainerItemOperationResult.Success,
                Count = itemToSwitch?.Stack ?? countToAdd,
                Item = itemToSwitch?.Item
            };
        }

        public async Task<bool> ContainerOperationMoveAddCommitImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsAdd.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationAddCommit Item transaction not found", transactionId).Write();
                return false;
            }
            _currentTransactionsAdd.Remove(transactionId);

            var moveAddTransaction = (ItemMoveTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAddTransaction.Address, ReplicationLevel.Master);
            if (moveAddTransaction.RemoveOld)
            {
                await ContainerUtils.RemoveItem(itemsContainer, moveAddTransaction.SlotId, 0, Guid.Empty, false,
                    transactionId);
            }

            var item = moveAddTransaction.Item;
            var itemsToAdd = moveAddTransaction.Count;
            var result = await ContainerUtils.AddItem(itemsContainer, item, moveAddTransaction.SlotId,
                moveAddTransaction.Count, moveAddTransaction.Manual, moveAddTransaction.Id);
            itemsContainer.TransactionReservedSlots.TryRemove(moveAddTransaction.SlotId, out _);

            int missedItemsCount = itemsToAdd - result.ItemsCount;
            if (missedItemsCount > 0)
            {
                if (!(await Drop(item, missedItemsCount)).IsSuccess)
                {
                    return false;
                }
            }

            return result.IsSuccess;
        }

        public Task<bool> ContainerOperationMoveAddRollbackImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsAdd.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationAddRollback Item transaction not found", transactionId).Write();
                return Task.FromResult(false);
            }
            _currentTransactionsAdd.Remove(transactionId);
            var moveAddTransaction = (ItemMoveTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAddTransaction.Address, ReplicationLevel.Master);
            itemsContainer.TransactionReservedSlots.TryRemove(moveAddTransaction.SlotId, out _);
            return Task.FromResult(true);
        }

        public async Task<ContainerOperationRemovePrepareResult> ContainerOperationMoveRemovePrepareImpl(
            Guid transactionId, PropertyAddress address, int slotId, int count,
            Guid clientSrcEntityId, bool manual)
        {
            var itemsContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, address, ReplicationLevel.Master);
            ISlotItem itemToRemove =
                await ContainerUtils.CanRemoveItem(itemsContainer, slotId, count, manual, clientSrcEntityId);
            var canRemove = itemToRemove != null;
            if (!canRemove)
                return new ContainerOperationRemovePrepareResult
                { ContainerItemOperationResult = ContainerItemOperationResult.ErrorSrcCantRemove };

            if (slotId >= 0)
            {
                if (itemsContainer.TransactionReservedSlots.ContainsKey(slotId))
                {
                    Logger.Error("{1}: ContainerOperationMoveRemovePrepare already reserved slot {0}", slotId,
                        itemsContainer.GetType().Name);
                    return new ContainerOperationRemovePrepareResult
                    {
                        ContainerItemOperationResult = ContainerItemOperationResult.ErrorSrcSlotLockedByOtherTransaction
                    };
                }

                itemsContainer.TransactionReservedSlots[slotId] = transactionId;
            }

            var newTransaction = new ItemMoveTransactionInfo
            {
                Id = transactionId,
                Address = address,
                SlotId = slotId,
                Manual = manual,
                Count = count,
                ClientEntityId = clientSrcEntityId,
            };

            _currentTransactionsRemove[newTransaction.Id] = newTransaction;
            return new ContainerOperationRemovePrepareResult
            { ContainerItemOperationResult = ContainerItemOperationResult.Success, Item = itemToRemove.Item };
        }

        public async Task<ContainerItemOperation> ContainerOperationMoveChangePrepareImpl(Guid transactionId,
            IItemWrapper itemWrapper)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsRemove.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationMoveChangePrepare Item transaction not found", transactionId).Write();
                return new ContainerItemOperation()
                { Result = ContainerItemOperationResult.ErrorChangeTransactionNotFound };
            }

            var moveAddTransaction = (ItemMoveTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAddTransaction.Address, ReplicationLevel.Master);
            var res = await ContainerUtils.CanAddItem(itemsContainer, itemWrapper.Item, moveAddTransaction.SlotId,
                itemWrapper.Count, moveAddTransaction.Manual, transactionId);
            int countToAdd = res.Item1.ItemsCount;
            ISlotItem itemToRemove = res.Item2;
            if (!res.Item1.IsSuccess || countToAdd != itemWrapper.Count ||
                itemToRemove.Stack > moveAddTransaction.Count)
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcCantAdd };

            moveAddTransaction.Item = await ContainerUtils.CloneItem(itemWrapper.Item);
            moveAddTransaction.Count = itemWrapper.Count;

            return res.Item1;
        }

        public async Task<ContainerItemOperation> ContainerOperationMoveRemoveCommitImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsRemove.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationRemoveCommit Item transaction not found", transactionId).Write();
                return ContainerItemOperation.Error;
            }
            _currentTransactionsRemove.Remove(transactionId);

            var moveAddTransaction = (ItemMoveTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAddTransaction.Address, ReplicationLevel.Master);

            ContainerItemOperation removeResult = ContainerItemOperation.Error;
            if (moveAddTransaction.Item != null)
            {
                removeResult = await ContainerUtils.RemoveItem(itemsContainer, moveAddTransaction.SlotId,
                    0, Guid.Empty, false, transactionId);

                var itemsToAdd = moveAddTransaction.Count;
                await ContainerUtils.AddItem(itemsContainer, moveAddTransaction.Item, moveAddTransaction.SlotId,
                    itemsToAdd, moveAddTransaction.Manual, moveAddTransaction.Id);
            }
            else
            {
                removeResult = await ContainerUtils.RemoveItem(itemsContainer, moveAddTransaction.SlotId,
                    moveAddTransaction.Count, moveAddTransaction.ClientEntityId, moveAddTransaction.Manual,
                    transactionId);
            }

            itemsContainer.TransactionReservedSlots.TryRemove(moveAddTransaction.SlotId, out _);
            return removeResult;
        }

        public Task<bool> ContainerOperationMoveRemoveRollbackImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsRemove.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationRemoveRollback Item transaction not found", transactionId).Write();
                return Task.FromResult(false);
            }
            _currentTransactionsRemove.Remove(transactionId);

           var moveAddTransaction = (ItemMoveTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAddTransaction.Address, ReplicationLevel.Master);
            itemsContainer.TransactionReservedSlots.TryRemove(moveAddTransaction.SlotId, out _);
            return Task.FromResult(true);
        }

        public async Task<ContainerItemOperation> ContainerOperationAddNewItemImpl(IItemWrapper itemWrapper,
            PropertyAddress destination, int destinationSlotId, bool manual)
        {
            var itemsContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, destination, ReplicationLevel.Master);
            return await ContainerUtils.AddItem(itemsContainer, itemWrapper.Item, destinationSlotId, itemWrapper.Count,
                manual, Guid.Empty);
        }

        public async Task<ContainerItemOperation> ContainerOperationAddItemsImpl(
            List<ItemResourcePack> itemResourcesToAdd, PropertyAddress destination, bool manual)
        {
            var destinationContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, destination, ReplicationLevel.Master);
            var result = new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorUnknown };

            var missedItems = new List<ItemResourcePack>();
            foreach (var itemResourcePack in itemResourcesToAdd)
            {
                var itemsToAdd = (int)itemResourcePack.Count;
                var subresult =
                    await ContainerUtils.AddItem(destinationContainer, itemResourcePack, manual, Guid.Empty);

                int missedItemsCount = itemsToAdd - subresult.ItemsCount;
                if (missedItemsCount > 0)
                    missedItems.Add(new ItemResourcePack(itemResourcePack.ItemResource, (uint)missedItemsCount,
                        itemResourcePack.Index));

                result.Result = result.IsSuccess ? ContainerItemOperationResult.Success : subresult.Result;
                result.ItemsCount += subresult.ItemsCount;
            }

            if (missedItems.Any())
            {
                var worldCharacter = parentEntity as IWorldCharacter;
                if (worldCharacter != null)
                {
                    await Drop(missedItems);
                }
            }

            return result;
        }

        public async Task<ContainerItemOperation> ContainerOperationRemoveBatchItemImpl(
            List<RemoveItemBatchElement> items, bool manual)
        {
            var result = new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorUnknown };
            foreach (var item in items)
            {
                var itemsContainer =
                    EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, item.Source, ReplicationLevel.Master);
                var opResult = await ContainerUtils.RemoveItem(itemsContainer, item.SourceSlotId, item.Count,
                    item.ClientEntityId, manual, Guid.Empty);
                if (opResult.IsSuccess)
                    result.Result = opResult.Result;
                result.ItemsCount += opResult.ItemsCount;
            }

            return result;
        }

        public async Task<ContainerOperationMoveAllAddPrepareResult> ContainerOperationMoveAllAddPrepareImpl(
            Guid transactionId, PropertyAddress address, Dictionary<int, IItemWrapper> itemWrappers, bool manual,
            bool sameSlots)
        {
            var itemsContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, address, ReplicationLevel.Master);
            if (itemsContainer == null)
            {
                Logger.IfError()?.Message("ContainerOperationMoveAllAddPrepare src {0} not found", address).Write();
                return new ContainerOperationMoveAllAddPrepareResult
                { ContainerItemOperationResult = ContainerItemOperationResult.ErrorDstNotFound };
            }

            var dictItems = new Dictionary<int, IItemWrapper>();
            foreach (var pair in itemWrappers)
            {
                var item = await ContainerUtils.CloneItem(pair.Value.Item);
                var slotId = sameSlots ? pair.Key : -1;
                var itemsToAdd = pair.Value.Count;
                var result =
                    await ContainerUtils.AddItem(itemsContainer, item, slotId, itemsToAdd, manual, transactionId);

                if (result.IsSuccess)
                    dictItems.Add(pair.Key, new IItemWrapper { Item = item, Count = pair.Value.Count });
            }

            var newTransaction = new ItemMoveAllTransactionInfo
            {
                Id = transactionId,
                Address = address,
                Items = dictItems,
                Manual = manual,
                SameSlots = sameSlots
            };

            _currentTransactionsAdd[newTransaction.Id] = newTransaction;
            return new ContainerOperationMoveAllAddPrepareResult
            {
                ContainerItemOperationResult = ContainerItemOperationResult.Success,
                Items = dictItems
            };
        }

        public Task<ContainerOperationMoveAllRemovePrepareResult> ContainerOperationMoveAllRemovePrepareImpl(
            Guid transactionId, PropertyAddress address, bool manual)
        {
            var itemsContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, address, ReplicationLevel.Master);
            if (itemsContainer == null)
            {
                Logger.IfError()?.Message("ContainerOperationMoveAllRemovePrepare src {0} not found", address).Write();
                return Task.FromResult(new ContainerOperationMoveAllRemovePrepareResult
                { ContainerItemOperationResult = ContainerItemOperationResult.ErrorSrcNotFound });
            }

            var dictItems = new Dictionary<int, IItemWrapper>();
            var transactionReservedSlots = new List<int>();
            foreach (var pair in itemsContainer.Items)
            {
                if (itemsContainer.TransactionReservedSlots.ContainsKey(pair.Key))
                    continue;

                itemsContainer.TransactionReservedSlots[pair.Key] = transactionId;
                transactionReservedSlots.Add(pair.Key);

                dictItems.Add(pair.Key, new IItemWrapper { Item = pair.Value.Item, Count = pair.Value.Stack });
            }

            var newTransaction = new ItemMoveAllTransactionInfo
            {
                Id = transactionId,
                Address = address,
                Items = dictItems,
                Manual = manual,
                ReservedSlots = transactionReservedSlots,
            };
            _currentTransactionsRemove[newTransaction.Id] = newTransaction;

            return Task.FromResult(new ContainerOperationMoveAllRemovePrepareResult
            {
                ContainerItemOperationResult = ContainerItemOperationResult.Success,
                Items = dictItems
            });
        }

        public Task<ContainerItemOperation> ContainerOperationMoveAllChangePrepareImpl(Guid transactionId,
            Dictionary<int, IItemWrapper> itemWrappers)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsRemove.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationMoveAllChangePrepare Item transaction not found", transactionId).Write();
                return Task.FromResult(new ContainerItemOperation()
                { Result = ContainerItemOperationResult.ErrorChangeTransactionNotFound });
            }

            var moveAddAllTransaction = (ItemMoveAllTransactionInfo)transaction;
            moveAddAllTransaction.Items = itemWrappers;
            return Task.FromResult(new ContainerItemOperation() { Result = ContainerItemOperationResult.Success });
        }

        public Task<bool> ContainerOperationMoveAllAddCommitImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsAdd.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationMoveAllAddCommit Item transaction not found", transactionId).Write();
                return Task.FromResult(false);
            }
            _currentTransactionsAdd.Remove(transactionId);

            var moveAllAddTransaction = (ItemMoveAllTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAllAddTransaction.Address, ReplicationLevel.Master);
            if (moveAllAddTransaction.ReservedSlots != null
            ) //TODO сейчас их нет, потому что вместо резервации слотов айтема добавляются сразу на prepare. По идее так неправильно, потом нужно переделать
                foreach (var slot in moveAllAddTransaction.ReservedSlots)
                    itemsContainer.TransactionReservedSlots.TryRemove(slot, out _);

            return Task.FromResult(true);
        }

        public async Task<bool> ContainerOperationMoveAllRemoveCommitImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsRemove.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationMoveAllRemoveCommit Item transaction not found", transactionId).Write();
                return false;
            }
            _currentTransactionsRemove.Remove(transactionId);

            var moveAllRemoveTransaction = (ItemMoveAllTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAllRemoveTransaction.Address, ReplicationLevel.Master);
            foreach (var pair in moveAllRemoveTransaction.Items)
            {
                await ContainerUtils.RemoveItem(itemsContainer, pair.Key, pair.Value.Count, pair.Value.Item.Id, false,
                    transactionId);
            }

            if (moveAllRemoveTransaction.ReservedSlots != null)
                foreach (var slot in moveAllRemoveTransaction.ReservedSlots)
                    itemsContainer.TransactionReservedSlots.TryRemove(slot, out _);
            return true;
        }

        public async Task<bool> ContainerOperationMoveAllAddRollbackImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsAdd.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationMoveAllAddRollback Item transaction not found", transactionId).Write();
                return false;
            }
            _currentTransactionsAdd.Remove(transactionId);

            var moveAllAddTransaction = (ItemMoveAllTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAllAddTransaction.Address, ReplicationLevel.Master);
            foreach (var pair in moveAllAddTransaction.Items)
            {
                var slotId = moveAllAddTransaction.SameSlots ? pair.Key : -1;
                await ContainerUtils.RemoveItem(itemsContainer, slotId, pair.Value.Count, pair.Value.Item.Id, false,
                    transactionId);
            }

            if (moveAllAddTransaction.ReservedSlots != null)
                foreach (var slot in moveAllAddTransaction.ReservedSlots)
                    itemsContainer.TransactionReservedSlots.TryRemove(slot, out _);
            return true;
        }

        public Task<bool> ContainerOperationMoveAllRemoveRollbackImpl(Guid transactionId)
        {
            BaseItemTransactionInfo transaction;
            if (!_currentTransactionsRemove.TryGetValue(transactionId, out transaction))
            {
                Logger.IfError()?.Message("ContainerOperationMoveAllRemoveRollback Item transaction not found", transactionId).Write();
                return Task.FromResult(false);
            }
            _currentTransactionsRemove.Remove(transactionId);

            var moveAllRemoveTransaction = (ItemMoveAllTransactionInfo)transaction;
            var itemsContainer = EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity,
                moveAllRemoveTransaction.Address, ReplicationLevel.Master);
            foreach (var slot in moveAllRemoveTransaction.ReservedSlots)
                itemsContainer.TransactionReservedSlots.TryRemove(slot, out _);
            return Task.FromResult(true);
        }

        public async Task<PropertyAddress> ContainerOperationSetSizeImpl(PropertyAddress address, int size)
        {
            var itemsContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, address, ReplicationLevel.Master);
            KeyValuePair<int, ISlotItem>[] itemsToDrop = itemsContainer.Items.Where(v => v.Key >= size).ToArray();

            PropertyAddress worldBoxAddress = null;
            if (itemsToDrop.Any())
                worldBoxAddress = await Drop(address, itemsToDrop);

            itemsContainer.Size = size;
            return worldBoxAddress;
        }

        public async Task<ContainerItemOperation> DropImpl(PropertyAddress address, int index, int count)
        {
            IItemsContainer itemsContainer =
                EntityPropertyResolver.Resolve<IItemsContainer>(parentEntity, address, ReplicationLevel.Master);
            if (itemsContainer.Items.TryGetValue(index, out ISlotItem slotItem))
            {
                ISlotItem itemToRemove =
                    await ContainerUtils.CanRemoveItem(itemsContainer, index, count, true, slotItem.Item.Id);
                var canRemove = itemToRemove != null;
                if (!canRemove)
                    return ContainerItemOperation.Error;

                ContainerItemOperation result = await DropImplementation(
                    async (PropertyAddress destAddress, int typeId, Guid id) =>
                    {
                        var itemTransaction = new ItemMoveManagementTransaction(address, index, destAddress, -1, count,
                            Guid.Empty, false, parentEntity.EntitiesRepository);
                        return await itemTransaction.ExecuteTransaction();
                    });
                return result;
            }

            return ContainerItemOperation.Error;
        }

        private async Task<PropertyAddress> Drop(PropertyAddress address, KeyValuePair<int, ISlotItem>[] items)
        {
            var repository = parentEntity.EntitiesRepository;
            PropertyAddress dropBoxProperty = null;
            ContainerItemOperation result = await DropImplementation(
                async (PropertyAddress destAddress, int typeId, Guid id) =>
                {
                    dropBoxProperty = destAddress;

                    ContainerItemOperation moveresult = ContainerItemOperation.Error;
                    foreach (var item in items)
                    {
                        ItemMoveManagementTransaction itemTransaction = new ItemMoveManagementTransaction(address,
                            item.Key, dropBoxProperty, -1, item.Value.Stack, Guid.Empty, false, EntitiesRepository);
                        var subresult = await itemTransaction.ExecuteTransaction();
                        if (subresult.IsSuccess)
                        {
                            moveresult = subresult;
                            using (var wrapper = await repository.Get(typeId, id))
                            {
                                var worldCharacter =
                                    wrapper.Get<IWorldCharacterServerApi>(typeId, id, ReplicationLevel.ServerApi);
                                if (worldCharacter != null)
                                    await worldCharacter.InvokeItemDropped(item.Value.Item.ItemResource,
                                        item.Value.Stack);
                            }
                        }
                    }

                    return moveresult;
                });

            return dropBoxProperty;
        }

        private async Task<ContainerItemOperation> Drop(List<ItemResourcePack> itemsPack)
        {
            var repository = EntitiesRepository;
            var result = await DropImplementation(async (PropertyAddress destAddress, int typeId, Guid id) =>
            {
                var itemTransaction =
                    new ItemAddBatchManagementTransaction(itemsPack, destAddress, false, EntitiesRepository);
                var transactionResult = await itemTransaction.ExecuteTransaction();
                if (transactionResult.IsSuccess)
                {
                    using (var wrapper = await repository.Get(typeId, id))
                    {
                        var worldCharacter =
                            wrapper.Get<IWorldCharacterServerApi>(typeId, id, ReplicationLevel.ServerApi);
                        if (worldCharacter != null)
                        {
                            foreach (var item in itemsPack)
                                await worldCharacter.InvokeItemDropped(item.ItemResource, (int)item.Count);
                        }
                    }
                }

                return transactionResult;
            });
            return result;
        }

        private async Task<ContainerItemOperation> Drop(IItem item, int count)
        {
            var repository = EntitiesRepository;
            var result = await DropImplementation(async (PropertyAddress destAddress, int typeId, Guid id) =>
            {
                var itemTransaction =
                    new ItemAddManagementTransaction(item, count, destAddress, -1, false, EntitiesRepository);
                var transactionResult = await itemTransaction.ExecuteTransaction();
                if (transactionResult.IsSuccess)
                {
                    using (var wrapper = await repository.Get(typeId, id))
                    {
                        var worldCharacter =
                            wrapper.Get<IWorldCharacterServerApi>(typeId, id, ReplicationLevel.ServerApi);
                        if (worldCharacter != null)
                        {
                            await worldCharacter.InvokeItemDropped(item.ItemResource, count);
                        }
                    }
                }

                return transactionResult;
            });
            return result;
        }

        private async Task<ContainerItemOperation> DropImplementation(
            Func<PropertyAddress, int, Guid, Task<ContainerItemOperation>> action)
        {
            var parentEntityOutRef = new OuterRef(parentEntity.Id, parentEntity.TypeId);
            var worldBoxResource =
                GameResourcesHolder.Instance.LoadResource<WorldBoxDef>(
                    "/UtilPrefabs/Res/Prototypes/Mounting/WorldBoxDef");

            Transform? transform = null;
            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var positionObjectEntity =
                    PositionedObjectHelper.GetPositioned(wrapper, parentEntity.TypeId, parentEntity.Id);
                if (positionObjectEntity != null)
                {
                    transform = positionObjectEntity.Transform;
                }
            }

            Guid boxId = default;
            Vector3? boxPosition = null;
            if (transform.HasValue)
            {
                var ownWorldSpaceRef = ((IHasWorldSpaced)parentEntity).WorldSpaced.OwnWorldSpace;
                boxId = await ClusterCommands.GetWorldBoxIdToDrop(transform.Value.Position, parentEntity.Id,
                    ownWorldSpaceRef.Guid, EntitiesRepository);
                if (boxId == Guid.Empty)
                {
                    boxPosition = await WorldSpaceServiceEntity.GetDropPosition(parentEntity, EntitiesRepository, transform.Value);
                }
            }

            if (boxId != default)
            {
                using (var boxWrapper = await EntitiesRepository.Get<IWorldBox>(boxId))
                {
                    var createdItem = boxWrapper?.Get<IWorldBox>(boxId);
                    if (createdItem != null)
                    {
                        var destAddress = EntityPropertyResolver.GetPropertyAddress(createdItem.Inventory);
                        return await action(destAddress, parentEntity.TypeId, parentEntity.Id);
                    }
                }

                Logger.IfError()?.Message("Cant get box with ID {0}", boxId).Write();

                boxPosition = await WorldSpaceServiceEntity.GetDropPosition(parentEntity, EntitiesRepository, transform.Value);
            }

            if (parentEntity is IHasWorldSpaced && parentEntity is IHasMapped)
            {
                var entRef = await EntitiesRepository.Create<IWorldBox>(Guid.NewGuid(), createdItem =>
                {
                    createdItem.WorldSpaced.OwnWorldSpace = ((IHasWorldSpaced)parentEntity).WorldSpaced.OwnWorldSpace;
                    createdItem.MapOwner = ((IHasMapped)parentEntity).MapOwner;
                    createdItem.Inventory.Size = worldBoxResource.Size;
                    createdItem.Def = worldBoxResource;
                    createdItem.MovementSync.SetPosition = boxPosition.Value;
                    createdItem.OwnerInformation.Owner = new OuterRef<IEntity>(parentEntity);
                    return Task.CompletedTask;
                });

                PropertyAddress destAddress = null;
                using (var wrapper = await EntitiesRepository.Get(entRef.TypeId, entRef.Id))
                {
                    var entity = wrapper.Get<IHasInventoryServer>(entRef.TypeId, entRef.Id, ReplicationLevel.Server);
                    if (entity == null)
                    {
                        Logger.IfError()?.Message("Failed to create box with id {0}", boxId).Write();
                        return default;
                    }

                    destAddress = EntityPropertyResolver.GetPropertyAddress(entity.Inventory);
                }

                if (destAddress == null)
                {
                    Logger.IfError()?.Message("Failed to get address of box {0}", boxId).Write();
                    return default;
                }

                var result = await action(destAddress, parentEntity.TypeId, parentEntity.Id);

                if (!result.IsSuccess)
                {
                    var worldBoxIsEmpty = false;
                    using (var wrapper = await EntitiesRepository.Get(destAddress.EntityTypeId, destAddress.EntityId))
                    {
                        var dropBoxEntity = wrapper?.Get<IHasInventoryClientFull>(destAddress.EntityTypeId,
                            destAddress.EntityId, ReplicationLevel.ClientFull);
                        worldBoxIsEmpty = !dropBoxEntity.Inventory.Items.Any();
                    }

                    if (worldBoxIsEmpty)
                    {
                        await EntitiesRepository.Destroy(destAddress.EntityTypeId, destAddress.EntityId);
                    }
                }

                return result;
            }

            return ContainerItemOperation.Error;
        }
    }
}
