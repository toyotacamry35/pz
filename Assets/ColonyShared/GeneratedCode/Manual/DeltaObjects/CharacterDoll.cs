using System;
using System.Collections.Concurrent;
using SharedCode.Entities;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.EntitySystem.Delta;
using SharedCode.Aspects.Item.Templates;
using NLog;
using System.Threading.Tasks;
using SharedCode.DeltaObjects;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.EntitySystem;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using GeneratedCode.Transactions;
using GeneratedCode.Custom.Containers;
using ResourcesSystem.Loader;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Serializers;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterDoll : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private List<SlotDef> _blockedForUsageSlots; // не сохраняем это в базе потому, что, на момент написания, блокировка осуществляется спеллами, а спеллы НЕ сохраняются в базе
        
        protected override void constructor()
        {
            base.constructor();
            TransactionReservedSlots = new ConcurrentDictionary<int, Guid>();
        }

        public Task<float> GetMaxWeigthImpl()
        {
            return Task.FromResult(float.MaxValue);
        }

        public Task<float> GetTotalWeightImpl()
        {
            return Task.FromResult(Items.Values.Where(v => v?.Item?.ItemResource != null).Select(v => v.Stack * v.Item.ItemResource.Weight).Sum());
        }

        public Task<int> GetMaxStackForSlotImpl(int destinationSlot)
        {
            return Task.FromResult(int.MaxValue);
        }

        public Task<bool> IgnoreMaxStackImpl()
        {
            return Task.FromResult(false);
        }

        public Task<bool> CanAutoselectEmptySlotsForAddStacksImpl()
        {
            return Task.FromResult(false);
        }

        public async Task<bool> CanAddImpl(IItem item, int index, int count, bool manual)
        {
            if (!(item.ItemResource is ItemResource))
                return false;

            var itemMaxDurability = await item.Health.GetMaxHealthAbsolute();
            if (itemMaxDurability > 0)
            {
                var currentDurability = await item.Health.GetHealthCurrent();
                if (currentDurability <= 0)
                {
                    Logger.IfInfo()?.Message($"Item '{item.ItemResource}' can't not be added to '{GetType().Name}'. Reason: currentDurability = {currentDurability} < 0").Write();
                    return false;
                }

                ISlotItem slotItem;
                if (Items.TryGetValue(index, out slotItem))
                {
                    if (item.ItemResource == slotItem.Item.ItemResource)  // Если ресурсы различны, то это операция свитча, если нет, то проверяем дальше
                    {
                        var DurabilityLevels = GlobalConstsHolder.GlobalConstsDef.DurabilityLevels.Select((v, i) => new { Index = i, Level = v })
                            .OrderBy(v => v.Level);

                        var stackMaxDurability = await slotItem.Item.Health.GetMaxHealthAbsolute();
                        if (stackMaxDurability > 0)
                        {
                            var stackDurabilityPercent =
                                await slotItem.Item.Health.GetHealthCurrent() / stackMaxDurability;

                            //Logger.IfInfo()?.Message($"stackDurabilityPercent = '{stackDurabilityPercent}'").Write();
                            var stackDurabilityLevel = DurabilityLevels.Where(v => v.Level >= stackDurabilityPercent).First().Index;
                            var itemDurabilityPercent = currentDurability / itemMaxDurability;
                            //Logger.IfInfo()?.Message($"itemDurabilityPercent = '{itemDurabilityPercent}'").Write();

                            var itemDurabilityLevel = DurabilityLevels.Where(v => v.Level >= itemDurabilityPercent).First().Index;

                            if (stackDurabilityLevel != itemDurabilityLevel)
                            {
                                Logger.IfInfo()?.Message($"Item '{item.ItemResource}' can't not be added to '{GetType().Name}'. " +
                                    $"Reason: stackDurabilityLevel '{stackDurabilityLevel}' != itemDurabilityLevel '{itemDurabilityLevel}'. " +
                                    $"Variables: stackDurabilityPercent = {stackDurabilityPercent}; itemDurabilityPercent = {itemDurabilityPercent}")
                                    .Write();
                                return false;
                            }
                        }
                    }
                }
            }

            if (parentEntity is IHasMutationMechanics)
            {
                IHasMutationMechanics mutationMechanicsEntity = (IHasMutationMechanics)parentEntity;
                if (!(mutationMechanicsEntity.MutationMechanics.Stage?.Items ?? Enumerable.Empty<ResourceRef<ItemsListDef>>())
                    .SelectMany(v => v.Target.Items)
                    .Where(v => v.Target != null)
                    .Select(v => v.Target)
                    .Contains(item.ItemResource))
                {
                    Logger.IfInfo()?.Message($"Item '{item.ItemResource}' can't not be added to '{GetType().Name}'. " +
                                                $"Reason: Faction '{mutationMechanicsEntity.Faction?.____GetDebugShortName() ?? "null"}' and Stage '{mutationMechanicsEntity.MutationMechanics.Stage?.____GetDebugShortName() ?? "null"}' does't not allow this item.")
                        .Write();
                    return false;
                }

            }

            return !BlockedSlotsId.Contains(index);
        }

        public Task<bool> CanRemoveImpl(IItem item, int index, int count, bool manual)
        {
            var quickSlotsUsed =
                (item.ItemResource as IHasPackDef)?.PackDef.UnblockSlots?
                    .Select(v => v.Target.SlotId)
                    .Intersect(Items.Keys)
                    .Any()
                ?? false;

            if (quickSlotsUsed)
                Logger.IfInfo()?.Message($"Item '{item.ItemResource}' can't not be removed from '{GetType().Name}'. " +
                                            $"Reason: {(quickSlotsUsed ? "Quick Slots in use" : "")}")
                    .Write();

            return Task.FromResult(!quickSlotsUsed);
        }

        public async Task OnItemAddedImpl(IItem item, int index, int count, bool manual)
        {
            //Logger.IfInfo()?.Message($"{GetType().Name}.OnItemAdded: itemToAddSource = {item.ItemResource.____GetDebugShortName()}, index = {index}, countToAdd = {count}, manual = {manual}").Write();
            await OnItemAddedToContainer(item?.ItemResource, index, count, manual);

            var newBlockedIds = SetBlockedSlots();
            await MoveItems(newBlockedIds);
            await ChangeInventorySize(item, (int changeInventorySize) => changeInventorySize);
        }

        public async Task<PropertyAddress> OnBeforeItemRemovedImpl(IItem item, int index, int count, bool manual)
        {
            //Logger.IfInfo()?.Message($"{GetType().Name}.OnItemRemoved: itemToAddSource = {item.ItemResource.____GetDebugShortName()}, index = {index}, countToAdd = {count}, manual = {manual}").Write();
            await OnItemRemovedToContainer(item?.ItemResource, index, count, manual);

            IEnumerable<int> newBlockedIds = SetBlockedSlots(index);
            await MoveItems(newBlockedIds);
            return await ChangeInventorySize(item, (int changeInventorySize) => -changeInventorySize);
        }

        private async Task MoveItems(IEnumerable<int> idsToMove)
        {
            var hasInventory = parentEntity as IHasInventory;
            var hasContainerApi = parentEntity as IHasContainerApi;            
            if (hasContainerApi != null && hasInventory != null)
            {
                var thisAddress = EntityPropertyResolver.GetPropertyAddress(this);
                var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(hasInventory.Inventory);

                var moveTransactions = new List<ItemMoveManagementTransaction>();
                foreach (var idToMove in idsToMove)
                {
                    if (Items.TryGetValue(idToMove, out ISlotItem slotItem))
                    {
                        var moveInventoryItemTransaction = new ItemMoveManagementTransaction(thisAddress, idToMove, inventoryAddress, -1, slotItem.Stack, slotItem.Item.Id, false, EntitiesRepository);
                        moveTransactions.Add(moveInventoryItemTransaction);
                    }
                }

                foreach (var transaction in moveTransactions)
                {
                    var result = await transaction.ExecuteTransaction();
                    if (!result.IsSuccess)
                    {
                        Logger.IfError()?.Message($"Can't move item on transaction = {transaction.ToString()}").Write();
                    }
                }
            }
        }

        private async Task<PropertyAddress> ChangeInventorySize(IItem item, Func<int, int> changeInventorySize)
        {
            var packDef = (item.ItemResource as IHasPackDef)?.PackDef;
            if (packDef.HasValue)
            {
                var inventoryExtraSlots = packDef.Value.ExtraInventorySlots;
                if (inventoryExtraSlots > 0)
                {
                    var hasContainerApi = parentEntity as IHasContainerApi;
                    var hasInventory = parentEntity as IHasInventory;
                    if (hasContainerApi != null && hasInventory != null)
                    {
                        var inventorySize = hasInventory.Inventory.Size;
                        var inventoryNewSize = inventorySize + changeInventorySize(inventoryExtraSlots);
                        var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(hasInventory.Inventory);

                        return await hasContainerApi.ContainerApi.ContainerOperationSetSize(inventoryAddress, inventoryNewSize);
                    }
                }
            }

            return null;
        }

        public async Task<bool> CanAddUsedSlotImpl(ResourceIDFull dollSlotRes)
        {
            var slot = ItemHelper.GetSlotResourceId(dollSlotRes);
            if (slot == null)
            {
                Logger.IfError()?.Message($"Slot with resource id {dollSlotRes} is not exist").Write();
                return false;
            }
            
            if (!Items.TryGetValue(slot.SlotId, out var itemSlot) ||
                (await itemSlot.Item.Health.GetMaxHealth() > 0 &&
                 await itemSlot.Item.Health.GetHealthCurrent() <= 0))
            {
                return false;
            }

            if (_blockedForUsageSlots != null && _blockedForUsageSlots.Contains(slot))
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Slot {slot.____GetDebugAddress()} is blocked for usage").Write();
                return false;
            }
            return !UsedSlots.Any();
        }
        public async Task<bool> AddUsedSlotImpl(ResourceIDFull dollSlotRes)
        {
            if (!await CanAddUsedSlotImpl(dollSlotRes))
                return false;

            if (!UsedSlots.Any())
            {
                StopSpellsOnChangeUsage(dollSlotRes);
                UsedSlots.Add(dollSlotRes);
                return true;
            }

            return false;
        }

        public Task<bool> RemoveUsedSlotImpl(ResourceIDFull dollSlotRes)
        {
            var result = Task.FromResult(UsedSlots.Remove(dollSlotRes));
            StopSpellsOnChangeUsage(dollSlotRes);
            return result;
        }

        private void StopSpellsOnChangeUsage(ResourceIDFull dollSlotRes)
        {
            var slotDef = GameResourcesHolder.Instance.LoadResource<SlotDef>(dollSlotRes);
            if (slotDef?.StopSpellGroupsOnUnuse != null && slotDef.StopSpellGroupsOnUnuse.Length > 0)
                if (parentEntity is IHasWizardEntity hasWizard)
                {
                    var wizardRef = hasWizard.Wizard.OuterRef;
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        using (var cnt = await EntitiesRepository.Get(wizardRef))
                        {
                            var wizard = cnt.Get<IWizardEntity>(wizardRef);
                            foreach (var group in slotDef.StopSpellGroupsOnUnuse)
                                await wizard.StopAllSpellsOfGroup(group, SpellId.Invalid, SpellFinishReason.FailOnDemand);
                        }
                    });
                }
        }
        
        public Task OnInit()
        {
            var newBlockedIds = SetBlockedSlots();
            return MoveItems(newBlockedIds);
        }

        public Task OnDatabaseLoad()
        {
            var newBlockedIds = SetBlockedSlots();
            return MoveItems(newBlockedIds);
        }

        private IEnumerable<int> SetBlockedSlots(int exceptItemIndex = -1)
        {
            var character = (parentEntity as IWorldCharacter);
            
            if (character == null)
                return Enumerable.Empty<int>();

            if (BlockedSlotsId == null)
                BlockedSlotsId = new DeltaList<int>();

            var defaultCharacterDef = character.Gender.DefaultCharacter.Target;

            var faction = character as IHasMutationMechanics;
            if(faction?.MutationMechanics?.Stage?.IsHostStage ?? false)
            {
                var allslotIds = Enumerable.Range(0, defaultCharacterDef.DefaultDoll.Size);
                foreach (var indx in allslotIds)
                    BlockedSlotsId.Add(indx);

                return allslotIds;
            }


            IEnumerable<int> blockedSlotsByDefault = defaultCharacterDef.DefaultBlockedSlots?.Select(v => v.Target.SlotId) ?? Enumerable.Empty<int>();
            IEnumerable<int> blockedSlotsByItems = Items
                .Where(v => v.Key != exceptItemIndex)
                .SelectMany(v =>
                    ((v.Value.Item.ItemResource as IHasPackDef)?.PackDef.BlockSlots ?? Enumerable.Empty<ResourceRef<SlotDef>>())
                    .Select(t => t.Target.SlotId)
                );
            IEnumerable<int> unblockedSlotsByItems = Items
                .Where(v => v.Key != exceptItemIndex)
                .SelectMany(v =>
                    ((v.Value.Item.ItemResource as IHasPackDef)?.PackDef.UnblockSlots ?? Enumerable.Empty<ResourceRef<SlotDef>>())
                    .Select(t => t.Target.SlotId)
                );

            var blockedSlots = blockedSlotsByDefault.Union(blockedSlotsByItems).Except(unblockedSlotsByItems);

            var idsToRemove = BlockedSlotsId.Except(blockedSlots).ToList();
            foreach (var idToRemove in idsToRemove)
                BlockedSlotsId.Remove(idToRemove);

            var idsToAdd = blockedSlots.Except(BlockedSlotsId).ToList();
            foreach (var idToAdd in idsToAdd)
                BlockedSlotsId.Add(idToAdd);

            return idsToAdd;
        }

        public ValueTask<bool> AddBlockedForUsageSlotsImpl(SlotDef[] slots)
        {
            if (slots != null)
                foreach (var slot in slots)
                    if (_blockedForUsageSlots == null || !_blockedForUsageSlots.Contains(slot))
                        (_blockedForUsageSlots = _blockedForUsageSlots ?? new List<SlotDef>()).Add(slot);
            return new ValueTask<bool>(true);
        }
        
        
        public ValueTask<bool> RemoveBlockedForUsageSlotsImpl(SlotDef[] slots)
        {
            if (slots != null)
                foreach (var slot in slots)
                    if (_blockedForUsageSlots != null)
                        _blockedForUsageSlots.Remove(slot);
            return new ValueTask<bool>(true);
        }

    }
} 