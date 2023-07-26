using Assets.Src.Server.Impl;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Tools;
using Uins.Slots;
using GeneratedCode.Repositories;
using SharedCode.Entities.Engine;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Core.Environment.Logging.Extension;
using SharedCode.Serializers;

namespace Assets.Src.Aspects
{
    public static class ClusterCommands
    {
        private const string DollPropertyName = nameof(IWorldCharacter.Doll);
        private const string InventoryPropertyName = nameof(IWorldCharacter.Inventory);

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();


        //=== Props ===========================================================

        public static IEntitiesRepository ClientRepository =>
            GameState.Instance
                .ClientClusterNode; //TODO !!! не использовать здесь NodeAccessor.Repository, так как на дедике в нем серверный репозиторий, и в результате в логике получается каша

        public static IEntitiesRepository Repository => NodeAccessor.Repository;


        //=== Public ==========================================================

        public static void DropItemAsBox_OnClient(SlotViewModel slotViewModel)
        {
            AsyncUtils.RunAsyncTask(() => DropItemAsBox_OnClientAsync(slotViewModel));
        }

        public static async Task DropItemAsBox_OnClientAsync(SlotViewModel slotViewModel)
        {
            if (NodeAccessor.Repository == null || GameState.Instance == null)
                return;

            var characterGuid = GameState.Instance.CharacterRuntimeData.CharacterId;
            using (var wrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterGuid);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return;

                await worldCharacter.ContainerApi.Drop(GetOurCharacterContainerAdress(worldCharacter, slotViewModel.IsInventorySlot),
                    slotViewModel.SlotId, slotViewModel.Stack);
            }
        }

        public static async Task DisassemblyPerkTask_OnClient(SlotViewModel svm, int count = -1)
        {
            string svmToString = await UnityQueueHelper.RunInUnityThread(() => { return svm.ToString(); });
            if (NodeAccessor.Repository == null || GameState.Instance == null)
                return;

            var characterGuid = GameState.Instance.CharacterRuntimeData.CharacterId;
            using (var wrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterGuid);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return;

                var taskResult = await worldCharacter.DisassemblyPerk(
                    svm.SlotsCollectionApi.CollectionPropertyAddress,
                    svm.SlotId,
                    svm.SelfSlotItem.ItemGuid);

                Logger.IfDebug()?.Message($"{nameof(RemoveItemTask_OnClient)}({svmToString}) {taskResult}").Write();
            }
        }

        public static async Task RemoveItemTask_OnClient(SlotViewModel svm, int count = -1)
        {
            string svmToString = await UnityQueueHelper.RunInUnityThread(() => { return svm.ToString(); });
            if (NodeAccessor.Repository == null || GameState.Instance == null)
                return;

            var characterGuid = GameState.Instance.CharacterRuntimeData.CharacterId;
            using (var wrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(characterGuid);
                if (worldCharacter.AssertIfNull(nameof(worldCharacter)))
                    return;

                var taskResult = await worldCharacter.RemoveItem(
                    svm.SlotsCollectionApi.CollectionPropertyAddress,
                    svm.SlotId,
                    count < 0 ? svm.Stack : count,
                    svm.SelfSlotItem.ItemGuid);

                Logger.IfDebug()?.Message($"{nameof(RemoveItemTask_OnClient)}({svmToString}) {taskResult}").Write();
            }
        }

        public static void TakeItemsFromContainer_OnClient(Guid playerGuid, List<OuterBaseSlotViewModel> outerSlotViewModels)
        {
            AsyncUtils.RunAsyncTask(() => TakeItemsFromContainer_OnClientAsync(playerGuid, outerSlotViewModels));
        }

        public static async Task TakeItemsFromContainer_OnClientAsync(Guid playerGuid, List<OuterBaseSlotViewModel> outerSlotViewModels)
        {
            if (outerSlotViewModels == null || outerSlotViewModels.Count == 0)
            {
                LogError(playerGuid, $"{nameof(outerSlotViewModels)} is null or empty");
                return;
            }

            var fromContainerAddr = outerSlotViewModels[0].SlotsCollectionApi.CollectionPropertyAddress;

            var batch = EntityBatch.Create().Add<IWorldCharacterClientFull>(playerGuid);

            if (!batch.HasItem(fromContainerAddr.EntityTypeId, fromContainerAddr.EntityId))
                batch.Add(fromContainerAddr.EntityTypeId, fromContainerAddr.EntityId);

            using (var wrapper = await NodeAccessor.Repository.Get(batch))
            {
                var characterEntity = wrapper.Get<IWorldCharacterClientFull>(playerGuid);
                if (characterEntity == null)
                {
                    LogError(playerGuid, $"{nameof(characterEntity)} is null");
                    return;
                }

                var toContainerAddr = EntityPropertyResolver.GetPropertyAddress(characterEntity.Inventory);

                var fromEntityClientFullTypeId =
                    EntitiesRepository.GetReplicationTypeId(fromContainerAddr.EntityTypeId, ReplicationLevel.ClientFull);
                var fromEntity = wrapper.Get<IEntity>(fromEntityClientFullTypeId, fromContainerAddr.EntityId);
                if (fromEntity == null)
                {
                    LogError(playerGuid, $"{nameof(fromEntity)} is null" +
                                         $"\n{nameof(fromContainerAddr)}={fromContainerAddr}");
                    return;
                }

                var fromContainer = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(fromEntity, fromContainerAddr);
                foreach (var fromSvm in outerSlotViewModels)
                {
                    Guid fromItemGuid = fromContainer.Items[fromSvm.SlotId].Item.Id;

                    var moveItemResult = await characterEntity.MoveItem(fromContainerAddr, fromSvm.SlotId, toContainerAddr,
                        -1, fromSvm.Stack, fromItemGuid);

                    if (!moveItemResult.IsSuccess)
                    {
                        LogError(playerGuid, $"Move is failed: {moveItemResult}\n" +
                                             $"from: {fromSvm}");
                    }
                }
            }
        }

        public static void TakeAllItemsFromContainer_OnClient(Guid playerGuid, PropertyAddress fromContainerAddr)
        {
            AsyncUtils.RunAsyncTask(() => TakeAllItemsFromContainer_OnClientAsync(playerGuid, fromContainerAddr));
        }

        public static async Task TakeAllItemsFromContainer_OnClientAsync(Guid playerGuid, PropertyAddress fromContainerAddr)
        {
            if (fromContainerAddr == null)
            {
                LogError(playerGuid, $"{nameof(fromContainerAddr)} is null");
                return;
            }

            var batch = EntityBatch.Create().Add<IWorldCharacterClientFull>(playerGuid);

            if (!batch.HasItem(fromContainerAddr.EntityTypeId, fromContainerAddr.EntityId))
                batch.Add(fromContainerAddr.EntityTypeId, fromContainerAddr.EntityId);

            using (var wrapper = await NodeAccessor.Repository.Get(batch))
            {
                var characterEntity = wrapper.Get<IWorldCharacterClientFull>(playerGuid);
                if (characterEntity == null)
                {
                    LogError(playerGuid, $"{nameof(characterEntity)} is null");
                    return;
                }

                var toContainerAddr = EntityPropertyResolver.GetPropertyAddress(characterEntity.Inventory);
                if (toContainerAddr == null)
                {
                    LogError(playerGuid, $"{nameof(toContainerAddr)} is null");
                    return;
                }

                var moveItemResult = await characterEntity.MoveAllItems(fromContainerAddr, toContainerAddr);
                if (!moveItemResult.IsSuccess)
                {
                    LogError(playerGuid, $"{nameof(characterEntity.MoveAllItems)}() is failed: result={moveItemResult}\n" +
                                         $"  {nameof(fromContainerAddr)}={fromContainerAddr}\n" +
                                         $"  {nameof(toContainerAddr)}={toContainerAddr}");
                }
            }
        }

        public static void MoveItem_OnClient(Guid playerGuid, SlotViewModel fromSvm, SlotViewModel toSvm, int stack = -1)
        {
            Logger.IfDebug()?.Message($"{nameof(MoveItem_OnClient)}()  {nameof(fromSvm)}={fromSvm}, " +
                                         $"{nameof(toSvm)}={toSvm}, {nameof(stack)}={stack}")
                .Write();
            AsyncUtils.RunAsyncTask(() => MoveItem_OnClientAsync(playerGuid, fromSvm, toSvm, stack));
        }

        public static async Task<ContainerItemOperation> MoveItem_OnClientAsync(Guid playerGuid, SlotViewModel fromSvm, SlotViewModel toSvm,
            int stack = -1)
        {
            var fromAddress = fromSvm.SlotsCollectionApi.CollectionPropertyAddress;
            var toAddress = toSvm.SlotsCollectionApi.CollectionPropertyAddress;

            var batch = EntityBatch.Create()
                .Add<IWorldCharacterClientFull>(playerGuid);
            if (!batch.HasItem(fromAddress.EntityTypeId, fromAddress.EntityId))
                batch.Add(fromAddress.EntityTypeId, fromAddress.EntityId);
            if (!batch.HasItem(toAddress.EntityTypeId, toAddress.EntityId))
                batch.Add(toAddress.EntityTypeId, toAddress.EntityId);
            using (var wrapper = await NodeAccessor.Repository.Get(batch))
            {
                var characterEntity = wrapper.Get<IWorldCharacterClientFull>(playerGuid);
                if (characterEntity == null)
                {
                    LogError(playerGuid, $"{nameof(characterEntity)} is null");
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcNotFound };
                }

                var fromEntityClientFullTypeId =
                    EntitiesRepository.GetReplicationTypeId(fromAddress.EntityTypeId, ReplicationLevel.ClientFull);
                var fromEntity = wrapper.Get<IEntity>(fromEntityClientFullTypeId, fromAddress.EntityId);
                if (fromEntity == null)
                {
                    LogError(playerGuid, $"{nameof(fromEntity)} is null\n" +
                                         $"{nameof(fromAddress)}={fromAddress}");
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcNotFound };
                }

                var toEntityClientFullTypeId =
                    EntitiesRepository.GetReplicationTypeId(toAddress.EntityTypeId, ReplicationLevel.ClientFull);
                var toEntity = wrapper.Get<IEntity>(toEntityClientFullTypeId, toAddress.EntityId);
                if (toEntity == null)
                {
                    LogError(playerGuid, $"{nameof(toEntity)} is null\n" +
                                         $"{nameof(toAddress)}={toAddress}");
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstNotFound };
                }

                var fromContainer = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(fromEntity, fromAddress);
                Guid fromItemGuid = fromContainer.Items[fromSvm.SlotId].Item.Id;
                var moveItemResult = await characterEntity.MoveItem(fromAddress, fromSvm.SlotId, toAddress, toSvm.SlotId,
                    stack <= 0 ? fromSvm.Stack : stack, fromItemGuid);

                if (!moveItemResult.IsSuccess)
                {
                    //LogError(playerGuid, $"{nameof(characterEntity.MoveItem)} is failed: result={moveItemResult}");
                }

                return moveItemResult;
            }
        }

        public static void SavePerk_OnClient(Guid playerGuid, SlotViewModel fromSvm, SlotViewModel toSvm, int stack = -1)
        {
            Logger.IfDebug()?.Message($"{nameof(SavePerk_OnClient)}()  {nameof(fromSvm)}={fromSvm}, " +
                                         $"{nameof(toSvm)}={toSvm}, {nameof(stack)}={stack}")
                .Write();
            AsyncUtils.RunAsyncTask(() => SavePerk_OnClientAsync(playerGuid, fromSvm, toSvm, stack));
        }

        public static async Task<ContainerItemOperationResult> SavePerk_OnClientAsync(Guid playerGuid, SlotViewModel fromSvm,
            SlotViewModel toSvm, int stack = -1)
        {
            var fromAddress = fromSvm.SlotsCollectionApi.CollectionPropertyAddress;

            var batch = EntityBatch.Create()
                .Add<IWorldCharacterClientFull>(playerGuid);
            if (!batch.HasItem(fromAddress.EntityTypeId, fromAddress.EntityId))
                batch.Add(fromAddress.EntityTypeId, fromAddress.EntityId);
            using (var wrapper = await NodeAccessor.Repository.Get(batch))
            {
                var characterEntity = wrapper.Get<IWorldCharacterClientFull>(playerGuid);
                if (characterEntity == null)
                {
                    LogError(playerGuid, $"{nameof(characterEntity)} is null");
                    return ContainerItemOperationResult.ErrorSrcNotFound;
                }

                var fromEntityClientFullTypeId =
                    EntitiesRepository.GetReplicationTypeId(fromAddress.EntityTypeId, ReplicationLevel.ClientFull);
                var fromEntity = wrapper.Get<IEntity>(fromEntityClientFullTypeId, fromAddress.EntityId);
                if (fromEntity == null)
                {
                    LogError(playerGuid, $"{nameof(fromEntity)} is null\n" +
                                         $"{nameof(fromAddress)}={fromAddress}");
                    return ContainerItemOperationResult.ErrorSrcNotFound;
                }

                var fromContainer = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(fromEntity, fromAddress);
                Guid fromItemGuid = fromContainer.Items[fromSvm.SlotId].Item.Id;
                var moveItemResult = await characterEntity.SavePerk(fromAddress, fromSvm.SlotId, fromItemGuid);

                if (moveItemResult != ContainerItemOperationResult.Success)
                {
                    //LogError(playerGuid, $"{nameof(characterEntity.MoveItem)} is failed: result={moveItemResult}");
                }

                return moveItemResult;
            }
        }

        /// <summary>
        /// Костыль перемещения в трупе всех предметов из куклы в инвентарь при первом облутывании
        /// </summary>
        public static async Task MoveAllDollItemsToInventory(Guid playerGuid, Guid corpseGuid)
        {
            using (var wrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(playerGuid))
            {
                PropertyAddress dollAddress;
                PropertyAddress inventoryAddress;

                var playerEntity = wrapper.Get<IWorldCharacterClientFull>(playerGuid);
                if (playerEntity.AssertIfNull(nameof(playerEntity)))
                    return;

                using (var wrapper2 = await NodeAccessor.Repository.Get<IWorldCorpseClientFull>(corpseGuid))
                {
                    var containerEntity = wrapper2.Get<IWorldCorpseClientFull>(corpseGuid);
                    if (containerEntity.AssertIfNull(nameof(containerEntity)))
                        return;

                    if (containerEntity.Doll.Items.Count == 0)
                        return;


                    dollAddress = EntityPropertyResolver.GetPropertyAddress(containerEntity.Doll);
                    inventoryAddress = EntityPropertyResolver.GetPropertyAddress(containerEntity.Inventory);
                }

                await playerEntity.MoveAllItems(dollAddress, inventoryAddress);
            }
        }

        public static void Repair_OnClient(SlotViewModel slotViewModel)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            AsyncUtils.RunAsyncTask(() => Repair_OnClientAsync(characterId, slotViewModel));
        }

        public static void Break_OnClient(SlotViewModel slotViewModel)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            AsyncUtils.RunAsyncTask(() => Break_OnClientAsync(characterId, slotViewModel));
        }

        public static async Task Break_OnClientAsync(Guid playerGuid, SlotViewModel slotViewModel)
        {
            var characterGuid = GameState.Instance.CharacterRuntimeData.CharacterId;
            var repository = NodeAccessor.Repository;
            using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(characterGuid);

                PropertyAddress inventoryAddress =
                    EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);
                PropertyAddress itemAddress = null;
                ISlotItemClientFull usedItem = null;
                if (slotViewModel.IsInventorySlot)
                {
                    worldCharacter.Inventory.Items.TryGetValue(slotViewModel.SlotId, out usedItem);
                    itemAddress = inventoryAddress;
                }
                else
                {
                    worldCharacter.Doll.Items.TryGetValue(slotViewModel.SlotId, out usedItem);
                    itemAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Doll);
                }

                await worldCharacter.Break(itemAddress, slotViewModel.SlotId, usedItem.Item.Id);
            }
        }

        public static async Task Repair_OnClientAsync(Guid playerGuid, SlotViewModel slotViewModel)
        {
            var characterGuid = GameState.Instance.CharacterRuntimeData.CharacterId;
            var repository = NodeAccessor.Repository;
            using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientFull>(characterGuid))
            {
                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientFull>(characterGuid);

                ISlotItemClientFull usedItem = null;
                if (slotViewModel.IsInventorySlot)
                    worldCharacter.Inventory.Items.TryGetValue(slotViewModel.SlotId, out usedItem);
                else
                    worldCharacter.Doll.Items.TryGetValue(slotViewModel.SlotId, out usedItem);

                if (usedItem != null)
                {
                    var itemAddress = GetOurCharacterContainerAdress(worldCharacter, slotViewModel.IsInventorySlot);
                    var inventoryAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Inventory);
                    var inventoryAddress2 = EntityPropertyResolver.GetPropertyAddress(worldCharacter.Doll);

                    using (var wrapper2 = await repository.Get<ICraftEngineClientFull>(worldCharacter.CraftEngine.Id))
                    {
                        var craftEngine = wrapper2.Get<ICraftEngineClientFull>(worldCharacter.CraftEngine.Id);
                        var result = await craftEngine.Repair(itemAddress, slotViewModel.SlotId, 0, 0, null, null, inventoryAddress,
                            inventoryAddress2);
                        if (result.Is(CraftOperationResult.SuccessAddedToQueue))
                        {
                        }
                    }
                }
            }
        }

        //=== Private =========================================================

        private static PropertyAddress GetOurCharacterContainerAdress(
            IWorldCharacterClientFull worldCharacterClientFull, bool isInventorySlot)
        {
            return EntityPropertyResolver.GetPropertyAddress(isInventorySlot ? worldCharacterClientFull.Inventory as IDeltaObject: worldCharacterClientFull.Doll as IDeltaObject);
        }

        private static void LogError(Guid playerGuid, string message, [CallerMemberName] string callerMethodName = "")
        {
            Logger.IfError()?.Message($"<{nameof(ClusterCommands)}> {callerMethodName}() {message} ({nameof(playerGuid)}={playerGuid})").Write();
        }
    }
}