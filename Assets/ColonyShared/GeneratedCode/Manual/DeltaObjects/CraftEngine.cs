using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.Chain;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using GeneratedCode.Transactions;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnumerableExtensions;
using ResourcesSystem.Loader;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class CraftEngine : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool Active = true;

        private static BaseItemResource GetAnyItem(IEntitiesRepository repo)
        {
            return GameResourcesHolder.Instance.LoadResource<BaseItemResource>("/Inventory/Items/UtilItems/AnyCraftItem");
        }

        public Task OnInit()
        {
            IntermediateCraftContainer.Size = 100;

            //FuelTimeAlreadyInUse = 0;
            //IntermediateFuelContainer.Size = 1;

            if(UseOwnOutputContainer)
            {
                OutputContainer.Items.OnChanged += async (eventArgs) =>
                {
                    if (OutputContainer.Items.Count >= OutputContainer.Size)
                    {
                        using (var wrapper = await EntitiesRepository.Get(TypeId, Id))
                        {
                            var ce = wrapper.Get<ICraftEngineServer>(TypeId, Id, ReplicationLevel.Server);
                            if (ce != null)
                            {
                                await ce.StopCraft();
                            }
                        }
                    }
                    else if (!Active)
                    {
                        using (var wrapper = await EntitiesRepository.Get(TypeId, Id))
                        {
                            var ce = wrapper.Get<ICraftEngineServer>(TypeId, Id, ReplicationLevel.Server);
                            if (ce != null)
                            {
                                await ce.RunCraft();
                            }
                        }
                    }
                };
            }
            return Task.CompletedTask;
        }

        public async Task OnDatabaseLoad()
        {
            foreach (var item in CraftingQueue)
            {
                if (item.Value.MandatorySlotPermutation == null)
                    item.Value.MandatorySlotPermutation = new List<int>();

                if (item.Value.OptionalSlotPermutation == null)
                    item.Value.OptionalSlotPermutation = new List<int>();
            }
            await UpdateCraftingTime();

        }

        public Task SetResultContainerAddressImpl(PropertyAddress resultContainerAddress)
        {
            ResultContainerAddress = resultContainerAddress;
            return Task.CompletedTask;
        }

        public async Task<CraftOperationResult> RunCraftImpl()
        {
            if (!Active)
            {
                Active = true;
                foreach (var item in CraftingQueue)
                    item.Value.IsActive = true;
            }

            if (CraftingQueue.Count > 0)
            {
                StartCraftingTimeUTC0InMilliseconds = SyncTime.Now;
                RescheduleQueue();
            }
            return CraftOperationResult.SuccessCraft;

            var batch = EntityBatch.Create().Add(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid);
            using (var ownerWrapper = await EntitiesRepository.Get(batch))
            {
                //var activator = ownerWrapper.Get<ICanBeActive>(OwnerInformation.Owner);
                //if (activator == null || activator.IsActive)
                //{
                //    Logger.IfError()?.Message($"RunCraftImpl activator == null || activator.IsActive: {activator?.IsActive}").Write();
                //    return CraftOperationResult.Success;
                //}

                CraftOperationResult result = CraftOperationResult.SuccessCraft;
                long currentTimeUTC0InMilliseconds = SyncTime.Now;
                //Logger.IfError()?.Message($"RunCraftImpl BEFORE check CanRun").Write();
                //if (await CanRun())
                //{
                //    Logger.IfError()?.Message($"RunCraftImpl IN CanRun").Write();
                //    using (var fuelContainerWrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId))
                //    {
                //        var fuelContainer = EntityPropertyResolver.Resolve<IItemsContainer>(fuelContainerWrapper.Get<IEntity>(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId), FuelContainerAddress, ReplicationLevel.Master);
                //        if (fuelContainer.Items.Count > 0)
                //        {
                //            StartFuelTimeUTC0InMilliseconds = currentTimeUTC0InMilliseconds;
                //            await RescheduleFuel(fuelContainer, FuelContainerAddress);
                //            result = CraftOperationResult.SuccessCraft;
                //        }
                //        else
                //            result = CraftOperationResult.ErrorNotEnoughFuel;
                //    }
                //}
                //else
                //    result = CraftOperationResult.ErrorCraftQueueIsEmpty;

                //Logger.IfError()?.Message($"RunCraftImpl after check CanRun, current result: {result}").Write();

                if (result.Is(CraftOperationResult.Success))
                {
                    //activator.IsActive = true;
                    if (CraftingQueue.Count == 0)
                        await UpdateCraftingQueue();

                    if (CraftingQueue.Count > 0)
                    {
                        StartCraftingTimeUTC0InMilliseconds = currentTimeUTC0InMilliseconds;
                        RescheduleQueue();
                    }
                }

                return result;
            }
        }

        public async Task<bool> CanRunImpl()
        {
            return true; 

            //Logger.IfInfo()?.Message("CanRun: In craft queue = " + CraftingQueue.Count + "; in fuel container = " + IntermediateFuelContainer.Items.Count).Write();
            if (IntermediateFuelContainer.Items.Any())
                return true;
            else if (FuelContainerAddress?.IsValid() ?? false)
            {
                using (var fuelContainerWrapper = await EntitiesRepository.Get(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId))
                {
                    var fuelContainer = EntityPropertyResolver.Resolve<IItemsContainer>(fuelContainerWrapper.Get<IEntity>(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId), FuelContainerAddress, ReplicationLevel.Master);
                    if (fuelContainer.Items.Any())
                        return true;
                }
            }

            return false;
        }

        public async Task StopCraftImpl()
        {
            if (Active && CraftingQueue.Any())
            {
                Active = false;
                foreach (var item in CraftingQueue)
                    item.Value.IsActive = false;
            }
            else
                return;

            //IntermediateFuelContainer.Items.Clear();
            //var batch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusive(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid);
            //using (var ownerWrapper = await EntitiesRepository.Get(batch))
            //{
            //    var activator = ownerWrapper?.Get<ICanBeActive>(OwnerInformation.Owner);
            //    if (activator == null || !activator.IsActive)
            //        return;
            //
            //    activator.IsActive = false;
            //}
            ////Logger.IfInfo()?.Message("StopCraft").Write();
            //
            //long currentTimeUTC0InMilliseconds = SyncTime.Now;
            //if (FuelContainerAddress?.IsValid() ?? false)
            //{
            //    using (var fuelContainerWrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId))
            //    {
            //        var fuelContainer = EntityPropertyResolver.Resolve<IItemsContainer>(fuelContainerWrapper.Get<IEntity>(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId), FuelContainerAddress, ReplicationLevel.Master);
            //        if (fuelContainer.Items.Count > 0)
            //        {
            //            FuelTimeAlreadyInUse += (currentTimeUTC0InMilliseconds - StartFuelTimeUTC0InMilliseconds);
            //            //Logger.IfInfo()?.Message("ClearSchedule: Fuel").Write();
            //            ClearSchedule(FuelScheduleCancellation);
            //        }
            //    }
            //}

            if (CraftingQueue.Any())
            {
                var currentQueueKey = CraftingQueue.Keys.Min();
                CraftingQueue[currentQueueKey].TimeAlreadyCrafted += (SyncTime.Now - StartCraftingTimeUTC0InMilliseconds);
            }
            //Logger.IfInfo()?.Message("ClearSchedule: Craft").Write();
            ClearSchedule(CraftScheduleCancellation);
        }

        public Task<CraftOperationResult> StopCraftWithWorkbenchImpl(WorkbenchTypeDef workbenchType)
        {
            return Task.FromResult(CraftOperationResult.None);

            //Logger.IfError()?.Message($"StopCraftWithWorkbench -> {workbenchType }\n{ToString()}").Write();
            //CurrentWorkbenchType.Remove(workbenchType);
            //if (!CraftingQueue.Any())
            //    return CraftOperationResult.ErrorCraftQueueIsEmpty;
            //
            //var currentCraftIndex = CraftingQueue.Keys.Min();
            //var workbenchCrafts = CraftingQueue.Where(v => v.Value.CraftRecipe.WorkbenchTypes?.Select(c => c.Target).Contains(workbenchType) ?? false);
            //foreach (var workbenchCraft in workbenchCrafts)
            //{
            //    if (workbenchCraft.Key == currentCraftIndex)
            //    {
            //        long TimePassedInMilliseconds = SyncTime.Now - StartCraftingTimeUTC0InMilliseconds;
            //        workbenchCraft.Value.TimeAlreadyCrafted = TimePassedInMilliseconds;
            //    }
            //    workbenchCraft.Value.IsActive = false;
            //}
            //RescheduleQueue();
            //
            ////Logger.IfInfo()?.Message("CraftingQueue = \n" + ToString()).Write();
            //return CraftOperationResult.SuccessCraft;
        }

        public Task<CraftOperationResult> ContinueCraftWithWorkbenchImpl(WorkbenchTypeDef workbenchType)
        {
            //Logger.IfError()?.Message($"ContinueCraftWithWorkbench -> {workbenchType}\n{ToString()}").Write();
            return Task.FromResult(CraftOperationResult.None);

           //if (!CurrentWorkbenchType.Contains(workbenchType))
           //    CurrentWorkbenchType.Add(workbenchType);
           //
           //if (!CraftingQueue.Any())
           //    return CraftOperationResult.ErrorCraftQueueIsEmpty;
           //
           //var currentCraftIndex = CraftingQueue.Keys.Min();
           ////Logger.IfError()?.Message($"ContinueCraftWithWorkbenchImpl currentCraftIndex: {currentCraftIndex}").Write();
           //var workbenchCrafts = CraftingQueue.Where(v => v.Value.CraftRecipe.WorkbenchTypes?.Select(c => c.Target).Contains(workbenchType) ?? false);
           //foreach (var workbenchCraft in workbenchCrafts)
           //{
           //    workbenchCraft.Value.IsActive = true;
           //    if (workbenchCraft.Key == currentCraftIndex) // мы сейчас должны крафтить этот элемент
           //    {
           //        StartCraftingTimeUTC0InMilliseconds = SyncTime.Now;
           //    }
           //}
           //RescheduleQueue();
           //
           ////Logger.IfInfo()?.Message("CraftingQueue = \n" + ToString()).Write();
           //return CraftOperationResult.SuccessCraft;
        }

        public async Task<CraftOperationResult> CraftImpl(CraftRecipeDef recipe, int variantIdx, int count, int[] mandatorySlotPermutation, int[] optionalSlotPermutation,
            PropertyAddress fromInventoryAddress, PropertyAddress fromInventoryAddress2)
        {
            if (CraftingQueue.Count >= MaxQueueSize)
                return CraftOperationResult.ErrorCraftQueueIsFull;

            UpdateRecipeUsages(recipe);
            //Logger.IfError()?.Message("Craft: \t" + recipe + " ----------------------------").Write();
            var variant = recipe.Variants[variantIdx];
            var variantItems = variant.MandatorySlots;
            bool isImmidiateCraft = variant.CraftingTime == 0;

            var batch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusive(fromInventoryAddress.EntityTypeId, fromInventoryAddress.EntityId);
            if (fromInventoryAddress2 != null && (fromInventoryAddress.EntityId != fromInventoryAddress2.EntityId || fromInventoryAddress.EntityTypeId != fromInventoryAddress2.EntityTypeId))
                ((IEntityBatchExt)batch).AddExclusive(fromInventoryAddress2.EntityTypeId, fromInventoryAddress2.EntityId);

            using (var batchContainer = await EntitiesRepository.Get(batch))
            {
                var fromInventoryEntity = batchContainer.Get<IEntity>(fromInventoryAddress.EntityTypeId, fromInventoryAddress.EntityId);
                var fromInventory = EntityPropertyResolver.Resolve<IItemsContainer>(fromInventoryEntity, fromInventoryAddress, ReplicationLevel.Master);

                IItemsContainer fromInventory2 = null;
                if (fromInventoryAddress2 != null)
                {
                    var fromInventoryEntity2 = batchContainer.Get<IEntity>(fromInventoryAddress2.EntityTypeId, fromInventoryAddress2.EntityId);
                    fromInventory2 = EntityPropertyResolver.Resolve<IItemsContainer>(fromInventoryEntity2, fromInventoryAddress2, ReplicationLevel.Master);
                }

                var intermediateCraftContainerAddress = EntityPropertyResolver.GetPropertyAddress(IntermediateCraftContainer);
                if (await MoveItemsByRecipe(fromInventoryAddress, fromInventoryAddress2, isImmidiateCraft ? null : intermediateCraftContainerAddress, recipe, variantIdx, count, mandatorySlotPermutation, optionalSlotPermutation, EntitiesRepository))
                {
                    if (isImmidiateCraft)
                    {
                        await CraftProduce(recipe, variantIdx, variant.Product.Count * count, mandatorySlotPermutation, optionalSlotPermutation);
                        await UpdateCraftingQueue();
                    }
                    else
                    {
                        if (CraftingQueue.Count == 0)
                            StartCraftingTimeUTC0InMilliseconds = SyncTime.Now;
                        var newIndx = CraftingQueue.Any() ? CraftingQueue.Keys.Max() + 1 : 0;
                        var active = Active && (recipe.WorkbenchTypes == null || recipe.WorkbenchTypes.Where(v => v.Target == CurrentWorkbenchType).Any()) ? true : false;
                        CraftingQueue.Add(newIndx,
                            new CraftingQueueItem
                            {
                                IsActive = active,
                                CraftRecipe = recipe,
                                SelectedVariantIndex = variantIdx,
                                Count = count,
                                MandatorySlotPermutation = new List<int>(mandatorySlotPermutation),
                                OptionalSlotPermutation = new List<int>(optionalSlotPermutation),
                                TimeAlreadyCrafted = 0
                            });
                        RescheduleQueue();

                        if (CraftingQueue.Count == 1)
                            return CraftOperationResult.SuccessCraft;
                        else
                            return CraftOperationResult.SuccessAddedToQueue;
                    }
                }
                else
                    return CraftOperationResult.ErrorNotEnoughItemsForCraft;
            }

            return CraftOperationResult.SuccessCraft;
        }

        private void UpdateRecipeUsages(CraftRecipeDef recipe)
        {
            if (recipe.AssertIfNull(nameof(recipe)))
                return;

            if (CraftRecipesUsageStats.ContainsKey(recipe))
                CraftRecipesUsageStats[recipe] += 1;
            else
                CraftRecipesUsageStats.Add(recipe, 1);

            CraftRecipesLastUsageTimes[recipe] = SyncTime.Now;
        }

        public async Task<CraftOperationResult> RepairImpl(PropertyAddress itemAddress, int itemIndex, int recipeIndex, int variantIdx, int[] mandatorySlotPermutation, int[] optionalSlotPermutation, PropertyAddress fromInventoryAddress, PropertyAddress fromInventoryAddress2)
        {
            var batch = ((IEntityBatchExt)EntityBatch.Create()).AddExclusive(fromInventoryAddress.EntityTypeId, fromInventoryAddress.EntityId);
            if (fromInventoryAddress2 != null && (fromInventoryAddress.EntityId != fromInventoryAddress2.EntityId || fromInventoryAddress.EntityTypeId != fromInventoryAddress2.EntityTypeId))
            {
                ((IEntityBatchExt)batch).AddExclusive(fromInventoryAddress2.EntityTypeId, fromInventoryAddress2.EntityId);
            }
            if (itemAddress != null && (fromInventoryAddress.EntityId != itemAddress.EntityId || fromInventoryAddress.EntityTypeId != itemAddress.EntityTypeId))
            {
                ((IEntityBatchExt)batch).AddExclusive(itemAddress.EntityTypeId, itemAddress.EntityId);
            }

            using (var batchContainer = await EntitiesRepository.Get(batch))
            {
                var itemEntity = batchContainer.Get<IEntity>(itemAddress.EntityTypeId, itemAddress.EntityId);
                var itemContainer = EntityPropertyResolver.Resolve<IItemsContainer>(itemEntity, itemAddress, ReplicationLevel.Master);

                var fromInventoryEntity = batchContainer.Get<IEntity>(fromInventoryAddress.EntityTypeId, fromInventoryAddress.EntityId);
                var fromInventory = EntityPropertyResolver.Resolve<IItemsContainer>(fromInventoryEntity, fromInventoryAddress, ReplicationLevel.Master);

                IItemsContainer fromInventory2 = null;
                if (fromInventoryAddress2 != null)
                {
                    var fromInventoryEntity2 = batchContainer.Get<IEntity>(fromInventoryAddress2.EntityTypeId, fromInventoryAddress2.EntityId);
                    fromInventory2 = EntityPropertyResolver.Resolve<IItemsContainer>(fromInventoryEntity2, fromInventoryAddress2, ReplicationLevel.Master);
                }

                ISlotItem slotItem;
                if (itemContainer.Items.TryGetValue(itemIndex, out slotItem))
                {
                    var entity = slotItem.Item;
                    var durability = (entity.ItemResource as ItemResource)?.Durability.Target;
                    if (durability == null)
                        return CraftOperationResult.ErrorItemIsNotRepairable;

                    RepairRecipeDef recipeDef = durability.RepairRecipe;
                    var variant = recipeDef.Variants[variantIdx];
                    bool isImmidiateCraft = variant.CraftingTime == 0;
                    float maxAbsoluteDurability = await entity.Health.GetMaxHealthAbsolute();
                    float maxCurrentDurability = await entity.Health.GetMaxHealth();
                    float currentDurability = await entity.Health.GetHealthCurrent();
                    float increaseDurability = durability.IncreaseDurabilityOnRepair * (await entity.Health.GetMaxHealthAbsolute());
                    float durabilityAfterRepair = Math.Min(currentDurability + increaseDurability, await entity.Health.GetMaxHealth());
                    if (maxCurrentDurability >= durability.FullBreakDurability * maxAbsoluteDurability)
                    {
                        float repairResourceCount = slotItem.Stack * (durabilityAfterRepair - currentDurability) / increaseDurability;

                        var intermediateCraftContainerAddress = EntityPropertyResolver.GetPropertyAddress(IntermediateCraftContainer);
                        if (await MoveItemsByRecipe(fromInventoryAddress, fromInventoryAddress2, isImmidiateCraft ? null : intermediateCraftContainerAddress, recipeDef, variantIdx, repairResourceCount, new int[variant.MandatorySlots.Length], new int[0] { }, EntitiesRepository))
                        {
                            if (isImmidiateCraft)
                            {
                                await entity.Health.ChangeHealth(increaseDurability);
                                return CraftOperationResult.SuccessCraft;
                            }
                            else
                            {
                                //slotItem.To<ISlotItem>().IsUnderRepair = true;
                                float repairTime = variant.CraftingTime * (durabilityAfterRepair - currentDurability) / increaseDurability;
                                repairTime = Math.Max(repairTime, 1f); 
                                this.Chain().Delay(repairTime, false, false).UpdateRepairTime(itemAddress, itemIndex).Run();
                                return CraftOperationResult.SuccessAddedToQueue;
                            }
                        }
                        else
                        {
                            return CraftOperationResult.ErrorNotEnoughItemsForCraft;
                        }
                    }
                    else
                    {
                        return CraftOperationResult.ErrorUnknown;
                    }
                }
            }

            return CraftOperationResult.SuccessCraft;
        }

        public async Task<CraftOperationResult> RemoveCraftImpl(int recipeIndex)
        {
            if (!CraftingQueue.Any() || recipeIndex < 0 || !CraftingQueue.ContainsKey(recipeIndex))
            {
                return CraftOperationResult.ErrorUnknown;
            }
            else if (!Active)
            {
                return CraftOperationResult.ErrorCraftResultConainerIsFull;
            }
            bool result = false;
            var removingRecipe = CraftingQueue[recipeIndex];
            removingRecipe.IsActive = false;
            using (var toInventoryContainer = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(ResultContainerAddress.EntityTypeId, ResultContainerAddress.EntityId))
            {
                var toInventoryEntity = toInventoryContainer.Get<IEntity>(ResultContainerAddress.EntityTypeId, ResultContainerAddress.EntityId);
                var toInventory = EntityPropertyResolver.Resolve<IItemsContainer>(toInventoryEntity, ResultContainerAddress, ReplicationLevel.Master);

                var intermediateCraftContainerAddress = EntityPropertyResolver.GetPropertyAddress(IntermediateCraftContainer);
                result = await MoveItemsByRecipe(intermediateCraftContainerAddress, null, ResultContainerAddress,
                    removingRecipe.CraftRecipe, removingRecipe.SelectedVariantIndex, removingRecipe.Count,
                    removingRecipe.MandatorySlotPermutation.ToArray(), removingRecipe.OptionalSlotPermutation.ToArray(),
                    EntitiesRepository/*, recipeIndex == CraftingQueue.Keys.Min() ? 0.5f : 1f*/);

                if (result)
                {
                    var currentCraftingIndx = CraftingQueue.Keys.Min();
                    if (recipeIndex != currentCraftingIndx)
                        CraftingQueue[currentCraftingIndx].TimeAlreadyCrafted += (SyncTime.Now - StartCraftingTimeUTC0InMilliseconds);

                    CraftingQueue.Remove(recipeIndex);
                    StartCraftingTimeUTC0InMilliseconds = SyncTime.Now;
                    RescheduleQueue();
                }
            }
            return result ? CraftOperationResult.Success : CraftOperationResult.ErrorUnknown;
        }

        public Task<CraftOperationResult> SwapCraftImpl(int recipeIndex1, int recipeIndex2)
        {
            if (CraftingQueue.Any() && CraftingQueue.ContainsKey(recipeIndex1) && CraftingQueue.ContainsKey(recipeIndex2))
            {
                int currentCraftingIndex = CraftingQueue.Keys.Min();
                if (recipeIndex1 != recipeIndex2 && recipeIndex1 != currentCraftingIndex && recipeIndex2 != currentCraftingIndex)
                {
                    var tmp = CraftingQueue[recipeIndex1];
                    CraftingQueue[recipeIndex1] = CraftingQueue[recipeIndex2];
                    CraftingQueue[recipeIndex2] = tmp;
                    RescheduleQueue();
                    return Task.FromResult(CraftOperationResult.SuccessCraft);
                }
            }

            return Task.FromResult(CraftOperationResult.ErrorUnknown);
        }

        private async Task CraftProduce(CraftRecipeDef recipeDef, int variantIdx, int count, int[] mandatorySlotPermutation, int[] optionalSlotPermutation)
        {
            //Logger.IfError()?.Message("CraftProduce: " + recipeDef.Variants[variantIdx].Product.Item.Target.ItemNameLs).Write();
            ///////////// Stats
            var statsModifiers_Mandatory = mandatorySlotPermutation.Select((v, i) => recipeDef.MandatorySlots[i].Items[v]).Where(v => v.StatsModifiers != null).SelectMany(v => v.StatsModifiers);
            var statsModifiers_Optional = optionalSlotPermutation.Select((v, i) => recipeDef.OptionalSlots[i].Items[v]).Where(v => v.StatsModifiers != null).SelectMany(v => v.StatsModifiers);
            var statsModifiers = statsModifiers_Mandatory.Concat(statsModifiers_Optional).ToList();

            var creaftResult = recipeDef.Variants[variantIdx].Product;
            var itemsPack = new List<ItemResourcePack>() { new ItemResourcePack(creaftResult.Item, (uint)creaftResult.Count, -1, statsModifiers) };

            var itemTransaction = new ItemAddBatchManagementTransaction(itemsPack, ResultContainerAddress, false, EntitiesRepository);
            var itemTransactionResult = await itemTransaction.ExecuteTransaction();

            var statOwner = OwnerInformation.Owner.IsValid ?
                OwnerInformation.Owner : 
                new OuterRef<IEntity>(ResultContainerAddress.EntityId, ResultContainerAddress.EntityTypeId);

            using (var wrapper = await EntitiesRepository.Get(statOwner.TypeId, statOwner.Guid))
            {
                var hasStatistics = wrapper.Get<IHasStatisticsServer>(statOwner.TypeId, statOwner.Guid, ReplicationLevel.Server);
                if (hasStatistics != null)
                {
                    var benchtype = CurrentWorkbenchType != null ? CraftSourceType.Bench : CraftSourceType.Player;
                    await hasStatistics.StatisticEngine.PostStatisticsEvent(new SharedCode.Quests.CraftEventArgs() { Recipe = recipeDef, CraftSource = benchtype });
                }
            }
        }

        public async Task UpdateRepairTimeImpl(PropertyAddress itemAddress, int itemIndex)
        {
            using (var itemWrapper = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(itemAddress.EntityTypeId, itemAddress.EntityId))
            {
                var itemEntity = itemWrapper.Get<IEntity>(itemAddress.EntityTypeId, itemAddress.EntityId);
                var itemContainer = EntityPropertyResolver.Resolve<IItemsContainer>(itemEntity, itemAddress, ReplicationLevel.Master);

                ISlotItem slotItem;
                if (itemContainer.Items.TryGetValue(itemIndex, out slotItem))
                {
                    IItem entity = slotItem.Item;
                    var durability = (entity.ItemResource as ItemResource)?.Durability.Target;
                    if (durability == null)
                        return;

                    RepairRecipeDef recipeDef = durability.RepairRecipe;
                    float maxAbsoluteDurability = await entity.Health.GetMaxHealthAbsolute();
                    float increaseDurability = durability.IncreaseDurabilityOnRepair * maxAbsoluteDurability;
                    float decreaseMaxDurability = durability.DecreaseMaxDurabilityOnRepair * maxAbsoluteDurability;
                    await entity.Stats.ChangeValue(GlobalConstsHolder.StatResources.HealthMaxStat, -decreaseMaxDurability);
                    //await entity.Stats.SetModifiers(new StatModifierData[] { new StatModifierData(GlobalConstsHolder.StatResources.HealthMaxStat, StatModifierType.ClampMax, await entity.Health.GetMaxHealth() - decreaseMaxDurability)}, new ModifierCauser() { Causer = recipeDef });
                    await entity.Health.ChangeHealth(increaseDurability);
                }
            }
        }

        bool _UpdateCraftingTimeImplRunning;

        public async Task UpdateCraftingTimeImpl()
        {
            if (_UpdateCraftingTimeImplRunning)
                return;

            _UpdateCraftingTimeImplRunning = true;
            try
            {
                //Logger.IfInfo()?.Message("UpdateCraftingTime: " + ToString()).Write();

                //Logger.IfInfo()?.Message($"UpdateCraftingTime: CraftingQueue.Count = {CraftingQueue.Count}").Write();
                if (CraftingQueue.Where(v => v.Value.IsActive).Any())
                {
                    int currentCraftingIndex = CraftingQueue.Where(v => v.Value.IsActive).Select(v => v.Key).Min();
                    ICraftingQueueItem currentRecipeInQueue = CraftingQueue[currentCraftingIndex];
                    CraftRecipeDef recipe = currentRecipeInQueue.CraftRecipe;
                    long currentRecipeCraftingTimeInMilliseconds =
                        (long)(recipe.Variants[currentRecipeInQueue.SelectedVariantIndex].CraftingTime * 1000.0f) -
                        currentRecipeInQueue.TimeAlreadyCrafted;

                    //currentRecipeInQueue.TimeAlreadyCrafted = 0;
                    long currentTimeUTC0 = SyncTime.Now;
                    /*
                    //Logger.Info("UpdateCraftingTime: " + recipe.Name + " - " + currentRecipeCraftingTimeInMilliseconds / 1000f + " sec." + "; " +
                        "\ncurrentTimeUTC0 = " + currentTimeUTC0 +
                        "\nStartCraftingTimeUTC0InMilliseconds = " + StartCraftingTimeUTC0InMilliseconds +
                        "\ncurrentRecipeCraftingTimeInMilliseconds = " + currentRecipeCraftingTimeInMilliseconds +
                        "\ntimediff = " + (currentTimeUTC0 - (StartCraftingTimeUTC0InMilliseconds + currentRecipeCraftingTimeInMilliseconds)));*/
                    if (currentTimeUTC0 >= StartCraftingTimeUTC0InMilliseconds + currentRecipeCraftingTimeInMilliseconds)
                    {

                    }
                    else
                    {
                        Logger.IfError()?.Message($"Time for craft hasn't came, but chain is fired: (deltatime = {currentTimeUTC0 - StartCraftingTimeUTC0InMilliseconds - currentRecipeCraftingTimeInMilliseconds}) < 0").Write();
                    }

                    //Logger.IfInfo()?.Message(nameof(currentCraftingIndex) + " = " + currentCraftingIndex).Write();
                    var variant = recipe.Variants[currentRecipeInQueue.SelectedVariantIndex];
                    await CraftProduce(recipe, currentRecipeInQueue.SelectedVariantIndex, variant.Product.Count, currentRecipeInQueue.MandatorySlotPermutation.ToArray(), currentRecipeInQueue.OptionalSlotPermutation.ToArray());
                    var intermediateCraftContainerAddress = EntityPropertyResolver.GetPropertyAddress(IntermediateCraftContainer);
                    await MoveItemsByRecipe(intermediateCraftContainerAddress, null, null,
                        currentRecipeInQueue.CraftRecipe, currentRecipeInQueue.SelectedVariantIndex, 1,
                        currentRecipeInQueue.MandatorySlotPermutation.ToArray(), currentRecipeInQueue.OptionalSlotPermutation.ToArray(),
                        EntitiesRepository);

                    // TODOA Не совсем корректно выставлять время заново, следует сделать инкрементально, но возможны баги из-за этого, перевести всё на chain-ы
                    StartCraftingTimeUTC0InMilliseconds = currentTimeUTC0;

                    currentRecipeInQueue.Count--;
                    if (currentRecipeInQueue.Count == 0)
                        CraftingQueue.Remove(currentCraftingIndex);

                    await UpdateCraftingQueue();
                    RescheduleQueue();
                }
                else
                {
                    await StopCraft();
                }

                //Logger.IfInfo()?.Message("UpdateCraftingTime: \t" + ToString()).Write();
            }
            finally
            {
                _UpdateCraftingTimeImplRunning = false;
            }
        }

        public async Task UpdateFuelTimeImpl()
        {
            //Logger.IfError()?.Message($"UpdateFuelTimeImpl").Write();
            if (FuelContainerAddress?.IsValid() ?? false)
            {
                long currentTimeUTC0 = SyncTime.Now;
                using (var fuelContainer = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId))
                {
                    var fuelEntity = fuelContainer.Get<IEntity>(FuelContainerAddress.EntityTypeId, FuelContainerAddress.EntityId);
                    var FuelContainer = EntityPropertyResolver.Resolve<IItemsContainer>(fuelEntity, FuelContainerAddress, ReplicationLevel.Master);

                    FuelDef fuel = await GetCurrentFuel();
                    if (fuel.Fuel != null)
                    {
                        long currentFuelTimeInMilliseconds = (long)(fuel.BurnTime * 1000.0f);
                        //if (currentTimeUTC0 >= StartFuelTimeUTC0InMilliseconds + currentFuelTimeInMilliseconds)
                        {
                            var item = FuelContainer.Items.FirstOrDefault(x => x.Value.Item.ItemResource == fuel.Fuel.Target);
                            if(item.Value != null)
                            {
                                var itemTransaction = new ItemRemoveBatchManagementTransaction(new List<RemoveItemBatchElement>() { new RemoveItemBatchElement(FuelContainerAddress, item.Key, 1, Guid.Empty) }, false, EntitiesRepository);
                                await itemTransaction.ExecuteTransaction();

                                StartFuelTimeUTC0InMilliseconds += currentFuelTimeInMilliseconds;

                                if (FuelContainer.Items.Any())
                                    await RescheduleFuel(FuelContainer, FuelContainerAddress);
                                else
                                {
                                    await StopCraft();
                                    StartCraftingTimeUTC0InMilliseconds = currentTimeUTC0;
                                }
                            }
                            else
                            {
                                await StopCraft();
                                StartCraftingTimeUTC0InMilliseconds = currentTimeUTC0;
                            }

                        }
                    }
                }
            }
        }

        private async Task<FuelDef> GetCurrentFuel()
        {
            if (IntermediateFuelContainer.Items.Any())
            {
                if (OwnerInformation.Owner.IsValid)
                {
                    using (var wrapper = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                    {
                        var entity = wrapper.Get<IWorldMachineServer>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Server);
                        if (entity != null)
                        {
                            FuelDef[] acceptableFuel = ((WorldMachineDef)entity.Def)?.AcceptableFuel;
                            if (acceptableFuel != null)
                            {
                                var currentBurnItemIndex = IntermediateFuelContainer.Items.Keys.Min();
                                var currentBurnItemResource = IntermediateFuelContainer.Items[currentBurnItemIndex].Item.ItemResource;
                                return acceptableFuel.Where(v => v.Fuel == currentBurnItemResource).FirstOrDefault();
                            }
                        }
                    }
                }
            }

            return default(FuelDef);
        }

        private static System.Random random = new System.Random();

        private static async Task<bool> MoveItemsByRecipe([NotNull] PropertyAddress fromInventory, [CanBeNull] PropertyAddress fromInventory2, [CanBeNull] PropertyAddress toInventory,
            [NotNull] RepairRecipeDef recipe, int variantIdx, float count,
            [NotNull] int[] mandatorySlotPermutation, [NotNull] int[] optionalSlotPermutation, [NotNull] IEntitiesRepository repo, float moveItemMultiplier = 1)
        {
            var craftingItems = await GetCraftingItems(fromInventory, fromInventory2, recipe, variantIdx, count, mandatorySlotPermutation, optionalSlotPermutation, GetAnyItem(repo), repo, moveItemMultiplier);
            if (craftingItems != null)
            {
                foreach (var item in craftingItems)
                {
                    int remaining = item.Required;
                    foreach (var slot in item.Slots)
                    {
                        int toRemove = Math.Min(remaining, slot.Count);
                        var moveResult = toInventory != null ?
                            await (new ItemMoveManagementTransaction(slot.Address, slot.Slot, toInventory, -1, toRemove, Guid.Empty, false, repo)).ExecuteTransaction() :
                            await (new ItemRemoveBatchManagementTransaction(new List<RemoveItemBatchElement>() { new RemoveItemBatchElement(slot.Address, slot.Slot, toRemove, Guid.Empty) }, false, repo)).ExecuteTransaction();


                        if (!moveResult.IsSuccess)
                            Logger.IfError()?.Message($"MoveItemsByRecipe: Can't move {item.Required} items {item.Item} from slot {slot.Slot}").Write();

                        remaining -= moveResult.ItemsCount;
                        if (remaining <= 0)
                            break;
                    }

                    if (remaining > 0)
                    {
                        Logger.IfError()?.Message($"MoveItemsByRecipe: Can't get enought items of {item.Item}, should be {item.Required}, but founded only {item.Required - remaining}").Write();
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private static async Task<CraftingItems[]> GetNeccessaryItems(
            [NotNull] PropertyAddress itemsAddress,
            [CanBeNull] PropertyAddress itemsAddress2,
            IEnumerable<RecipeItemStack> allRecipeItems,
            [NotNull] IEntitiesRepository repo,
            float moveItemMultiplier = 1)
        {
            if (itemsAddress?.IsValid() ?? false)
            {
                var batch = EntityBatch.Create().Add(itemsAddress.EntityTypeId, itemsAddress.EntityId);
                if (itemsAddress2?.IsValid() ?? false)
                    batch.Add(itemsAddress2.EntityTypeId, itemsAddress2.EntityId);

                using (var itemContainers = await repo.Get(batch))
                {
                    var itemEntity = itemContainers.Get<IEntity>(itemsAddress.EntityTypeId, itemsAddress.EntityId, ReplicationLevel.ClientFull);
                    var itemEntity2 = itemsAddress2 != null
                        ? itemContainers.Get<IEntity>(itemsAddress2.EntityTypeId, itemsAddress2.EntityId, ReplicationLevel.ClientFull)
                        : null;

                    var items = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(itemEntity, itemsAddress);
                    var items2 = itemEntity2 != null ? EntityPropertyResolver.Resolve<IItemsContainerClientFull>(itemEntity2, itemsAddress2) : null;

                    var invItems = items.Items
                        .Select(v => new CraftingContainerItems() {Index = v.Key, ItemSlot = v.Value, Address = itemsAddress, Priority = 0})
                        .Concat(
                            items2?.Items?.Select(
                                v => new CraftingContainerItems() {Index = v.Key, ItemSlot = v.Value, Address = itemsAddress2, Priority = 1}) ??
                            Enumerable.Empty<CraftingContainerItems>())
                        .GroupBy(v => v.ItemSlot.Item.ItemResource)
                        .Select(
                            v => new
                            {
                                Item = v.Key,
                                Count = v.Sum(q => q.ItemSlot.Stack),
                                Slots = v.Select(q => new CraftingSlots {Slot = q.Index, Count = q.ItemSlot.Stack, Address = q.Address}).ToArray()
                            });

                    var query = from reqItem in allRecipeItems
                        let reqItemCountFloat = reqItem.Count * moveItemMultiplier // Число предметов, которые надо вернуть (логически оно дробное)
                        let reqItemCountInt = (int) reqItemCountFloat // Целая часть предметов, которые надо вернуть
                        let reqItemCountFraction = reqItemCountFloat - reqItemCountInt // Дробная часть предметов, которые надо вернуть
                        let reqItemCount = reqItemCountInt + // С вероятностью равной дробной части предмета я его получаю, в противном слечае нет
                                           (reqItemCountFraction > 0 ? (random.NextDouble() <= reqItemCountFraction ? 1 : 0) : 0)
                        where reqItemCount >= 1 // Только если есть предметы для перемещения, то выполняем остальное
                        join invItem in invItems on reqItem.Item.Target equals invItem.Item into grp
                        from invItemEx in grp.DefaultIfEmpty()
                        select new CraftingItems
                        {
                            Item = reqItem.Item.Target,
                            Required = reqItemCount,
                            Count = invItemEx != null ? invItemEx.Count : 0,
                            Slots = invItemEx != null ? invItemEx.Slots : Enumerable.Empty<CraftingSlots>(),
                        };

                    return query.ToArray();
                }
            }

            return null;
        }

        private void RescheduleQueue()
        {
            ClearSchedule(CraftScheduleCancellation);
            if (CraftingQueue.Where(v => v.Value.IsActive).Any())
            {
                int currentCraftingIndex = CraftingQueue.Where(v => v.Value.IsActive).Select(v => v.Key).Min();
                ICraftingQueueItem currentRecipeInQueue = CraftingQueue[currentCraftingIndex];
                currentRecipeInQueue.CraftStartTime = StartCraftingTimeUTC0InMilliseconds;
                var craftRecipe = currentRecipeInQueue.CraftRecipe.Variants[currentRecipeInQueue.SelectedVariantIndex];
                var deltatime = (long)(craftRecipe.CraftingTime * 1000.0f) - currentRecipeInQueue.TimeAlreadyCrafted - (SyncTime.Now - StartCraftingTimeUTC0InMilliseconds);
                deltatime = (long)Math.Max(deltatime, 1000f);
                //Logger.IfInfo()?.Message($"RescheduleQueue: {(deltatime / 1000.0f).ToString("F2")} sec.").Write();
                var cancellationToken = this.Chain().Delay(deltatime * 0.001f, false, false).UpdateCraftingTime().Run();
                CraftScheduleCancellation.Add(cancellationToken);
            }
        }

        private async Task RescheduleFuel(IItemsContainer fuelContainer, PropertyAddress fuelContainerAddress)
        {
            //Logger.IfInfo()?.Message(nameof(ClearSchedule) + ": " + nameof(FuelScheduleCancellation)).Write();
            ClearSchedule(FuelScheduleCancellation);
            if (fuelContainer.Items.Any())
            {
                FuelTimeAlreadyInUse = 0;
                if (!IntermediateFuelContainer.Items.Any())
                {
                    var IntermediateFuelContainerAddress = EntityPropertyResolver.GetPropertyAddress(IntermediateFuelContainer);
                    await (new ItemMoveManagementTransaction(fuelContainerAddress, fuelContainer.Items.Keys.Min(), IntermediateFuelContainerAddress, -1, 1, Guid.Empty, false, EntitiesRepository)).ExecuteTransaction();
                }

                FuelDef fuel = await GetCurrentFuel();
                if (fuel.Fuel != null)
                {
                    int deltatime = (int)(StartFuelTimeUTC0InMilliseconds - SyncTime.Now - FuelTimeAlreadyInUse) + (int)(fuel.BurnTime * 1000.0f);

                    //Logger.IfInfo()?.Message($"RescheduleFuel: {(deltatime / 1000f).ToString("F2")} sec.").Write();

                    deltatime = (int)Math.Max(deltatime, 1000f);
                    var cancellationToken = this.Chain().Delay(deltatime / 1000f, false, false).UpdateFuelTime().Run();
                    FuelScheduleCancellation.Add(cancellationToken);

                    //Logger.IfInfo()?.Message(nameof(RescheduleFuel) + ": " + (deltatime / 1000f).ToString("F2")).Write();
                }
            }
        }

        private void ClearSchedule(IDeltaList<ChainCancellationToken> shedularList)
        {
            if (shedularList.Any())
            {
                Logger.IfInfo()?.Message("Clear shedule {0}", shedularList == CraftScheduleCancellation ? "craft" : "fuel").Write();
                foreach (var token in shedularList.ToArray())
                    token.Cancel(EntitiesRepository);
                shedularList.Clear();
            }
        }

        private async Task UpdateCraftingQueue()
        {
            return; // т.к. этот код только для перекладывания из приоритетной очереди WorldMachine в кравтовую очередь
            var canRun = await CanRun();
            //Logger.IfInfo()?.Message($"CraftingQueue.Count = {CraftingQueue.Count} && CanRun() = {canRun} && Owner.IsValid = {Owner.IsValid}").Write();
            if (CraftingQueue.Count == 0 && canRun && OwnerInformation.Owner.IsValid)
            {
                using (var wrapper = await EntitiesRepository.Get(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid))
                {
                    var worldMachine = wrapper.Get<IWorldMachineServer>(OwnerInformation.Owner.TypeId, OwnerInformation.Owner.Guid, ReplicationLevel.Server);
                    if (worldMachine != null)
                    {
                        foreach (var craftPriority in worldMachine.PriorityQueue)
                        {
                            //Logger.IfInfo()?.Message(nameof(UpdateCraftingQueue) + ": try to run " + craftPriority.CraftRecipe.Name).Write();
                            var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(worldMachine.Inventory);
                            int[] slots = new int[craftPriority.CraftRecipe.Variants[0].MandatorySlots.Length];
                            CraftOperationResult result = await Craft(craftPriority.CraftRecipe, 0, 1, slots, new int[0] { }, inventoryAddress, null);
                            if (result.Is(CraftOperationResult.Success))
                            {
                                //Logger.IfInfo()?.Message($"Runned Successfully... {craftPriority.CraftRecipe.Name}").Write();
                                break;
                            }
                            else
                            {
                                //Logger.IfInfo()?.Message("Not Runned ...").Write();
                            }
                        }
                    }
                }
            }
        }

        public static async Task<int> GetCraftingItemsMaxCount(
            [NotNull] PropertyAddress items,
            [CanBeNull] PropertyAddress items2,
            [NotNull] RepairRecipeDef recipe,
            int variantIdx,
            [NotNull] int[] mandatorySlotPermutation,
            [NotNull] int[] optionalSlotPermutation,
            BaseItemResource anyItem,
            IEntitiesRepository repo)
        {
            if (items.AssertIfNull(nameof(items)) ||
                recipe.AssertIfNull(nameof(recipe)) ||
                mandatorySlotPermutation.AssertIfNull(nameof(mandatorySlotPermutation)) ||
                optionalSlotPermutation.AssertIfNull(nameof(optionalSlotPermutation)))
                return 0;

            var allRecipeItems = GetAllRecipeItems(recipe, variantIdx, 1, mandatorySlotPermutation, optionalSlotPermutation, anyItem);
            var craftingItems = await GetNeccessaryItems(items, items2, allRecipeItems, repo); //число доступных айтемов для 1 единицы крафта
            Logger.IfInfo()?.Message(
                $"{nameof(GetCraftingItemsMaxCount)} allRecipeItems: {allRecipeItems.ItemsToStringByLines()}\n" +
                $"craftingItems: {craftingItems.ItemsToStringByLines()}")
                .Write(); //DEBUG

            return craftingItems.Min(item => item.Count / item.Required);
        }

        public static async Task<CraftingItems[]> GetCraftingItems(
            [NotNull] PropertyAddress items,
            [CanBeNull] PropertyAddress items2,
            [NotNull] RepairRecipeDef recipe,
            int variantIdx,
            float count,
            [NotNull] int[] mandatorySlotPermutation,
            [NotNull] int[] optionalSlotPermutation,
            BaseItemResource anyItem,
            IEntitiesRepository repo,
            float moveItemMultiplier = 1)
        {
//            Logger.Info($"{nameof(GetCraftingItems)}({count}x{recipe}, variant={variantIdx}, mand={mandatorySlotPermutation.ItemsToString()}, " +
//                        $"opt={optionalSlotPermutation.ItemsToString()}, addr={items}/{items2}, any={anyItem})"); //DEBUG
            if (items.AssertIfNull(nameof(items)) ||
                recipe.AssertIfNull(nameof(recipe)) ||
                mandatorySlotPermutation.AssertIfNull(nameof(mandatorySlotPermutation)) ||
                optionalSlotPermutation.AssertIfNull(nameof(optionalSlotPermutation)) ||
                count <= 0)
            {
                return null;
            }

            var allRecipeItems = GetAllRecipeItems(recipe, variantIdx, count, mandatorySlotPermutation, optionalSlotPermutation, anyItem);
            var craftingItems = await GetNeccessaryItems(items, items2, allRecipeItems, repo, moveItemMultiplier);
//            Logger.Info($"{nameof(GetCraftingItems)} allRecipeItems: {allRecipeItems.ItemsToStringByLines()}\n" +
//                        $"craftingItems: {craftingItems.ItemsToStringByLines()}"); //DEBUG
            if (!craftingItems.Any(v => v.Required > v.Count))
            {
//                Logger.IfInfo()?.Message($"{nameof(GetCraftingItems)} OK").Write(); //DEBUG
                return craftingItems;
            }

            return null;
        }

        public static IEnumerable<RecipeItemStack> GetAllRecipeItems(
            [NotNull] RepairRecipeDef recipe,
            int variantIdx,
            float count,
            [NotNull] int[] mandatorySlotPermutation,
            [NotNull] int[] optionalSlotPermutation,
            BaseItemResource anyItem)
        {
            var variant = recipe.Variants[variantIdx];
            var craftRecipe = recipe as CraftRecipeDef;
            var variantItems = variant.MandatorySlots
                .Select(
                    (v, i) => v.Item == anyItem
                        ? craftRecipe?.MandatorySlots[i].Items[mandatorySlotPermutation[i]].Item ?? v
                        : v
                );
            var optionalItems = craftRecipe != null
                ? optionalSlotPermutation
                    .Where(v => v >= 0)
                    .Select((v, i) => craftRecipe.OptionalSlots[i].Items[v].Item)
                : Enumerable.Empty<RecipeItemStack>();

            IEnumerable<RecipeItemStack> allRecipeItems =
                variantItems.Concat(optionalItems)
                    .GroupBy(v => v.Item)
                    .Select(
                        v => new RecipeItemStack
                        {
                            Item = v.Key,
                            Count = (int) Math.Ceiling(v.Sum(val => val.Count) * count) // округление до ближайшего верхнего целого
                        }
                    );

            return allRecipeItems;
        }

        public override string ToString()
        {
            try
            {
                string str = "";
                foreach (var queue in CraftingQueue.ToList())
                {
                    str += $"{queue.Key}: {queue.Value.CraftRecipe}: {queue.Value.Count} pcs.| {queue.Value.TimeAlreadyCrafted} sec.; ";
                    //String.Join(", ", queue.Value.MandatorySlotPermutation) + "] ; [" +
                    //String.Join(", ", queue.Value.OptionalSlotPermutation) + "])\n";
                }

                return str;
            }
            catch (Exception e)
            {
                return "CraftEngine ToString exception:" + e.Message;
            }
        }
    }

    public struct CraftingContainerItems
    {
        public int Index;
        public int Priority;
        public ISlotItemClientFull ItemSlot;
        public PropertyAddress Address;
    }
}
