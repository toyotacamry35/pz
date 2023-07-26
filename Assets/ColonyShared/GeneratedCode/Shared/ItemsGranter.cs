using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.EntitySystem;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using GeneratedCode.Transactions;

namespace Assets.ColonyShared.GeneratedCode.Shared
{
    public static class ItemsGranter
    {
        // ReSharper disable once UnusedMember.Local
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // @Returns: granted items
        [CanBeNull]
        public static async Task<ItemResourcePack[]> GrantItems([NotNull] ItemResourcePack[] items, 
                                                                         Guid receiverId, 
                                                                         [NotNull] IEntitiesRepository repo, 
                                                                         bool grantRandom, 
                                                                         float miningLootMultiplier, 
                                                                         Guid? itemsContainerGuid = null)
        {
            //Logger.IfDebug()?.Message($"TC-3466   ##_4.2 `{nameof(ItemsGranter)}.{nameof(GrantItems)}`({nameof(receiverId)}: {receiverId};  {nameof(repo.CloudNodeType)}: {repo?.CloudNodeType};  {nameof(grantRandom)}: {grantRandom};  {nameof(grantItemsContext)}: {grantItemsContext};  {nameof(items)}[N:{items?.Count}]: {items}).").Write();

            if (miningLootMultiplier < 0)
                Logger.IfError()?.Message($"Invalid `miningLootMultiplier` < 0: ({miningLootMultiplier}).").Write();

            //DbgLog.Log(13334, $"7.3. GrantItems: items:{items.Length}. [{(items.Length > 0 ? items[0].Count.ToString() : "items is empty")}]");

            var batchContainer = ((IEntityBatchExt)EntityBatch.Create()).AddExclusive<IWorldCharacterServer>(receiverId);
            if (itemsContainerGuid.HasValue)
                ((IEntityBatchExt)batchContainer).AddExclusive<IWorldBoxServer>(itemsContainerGuid.Value);

            // Apply miningLootMultiplier:
            for (int i = 0;  i < items.Length;  ++i)
                items[i].Count = LootTableCalcer.StochasticAmoutOfResources(miningLootMultiplier, items[i].Count);

            //DbgLog.Log(13334, $"7.3(b). GrantItems: items:{items.Length}. mlm:{miningLootMultiplier} [{(items.Length > 0 ? items[0].Count.ToString() : "items is empty")}]");

            using (var container = await repo.Get(batchContainer))
            {
                var addResult = ContainerItemOperation.Error;
                ItemResourcePack[] itemsToGrant = null;

                var character = container?.Get<IWorldCharacterServer>(receiverId);
                if (character != null)
                {
                    if (grantRandom)
                    {
                        itemsToGrant = new ItemResourcePack[] {items[SharedHelpers.Random.Next(items.Length)]};
                        addResult = await AddItem(character, itemsToGrant.ToArray());
                    }
                    else
                    {
                        itemsToGrant = items;
                        if (itemsContainerGuid.HasValue)
                        {
                            var containerBox = container.Get<IWorldBoxServer>(itemsContainerGuid.Value);
                            addResult = await AddItem(character, containerBox, repo);
                        }
                        else
                        {
                            addResult = await AddItem(character, itemsToGrant.ToArray());
                        }
                    }
                }

                return (addResult.IsSuccess)
                    ? itemsToGrant
                    : null;
            }
        }

        public static async Task<ContainerItemOperation> AddItem(IWorldCharacterServer character, params ItemResourcePack[] itemsPack)
        {
            var containerPropertyAddress = EntityPropertyResolver.GetPropertyAddress(character.Inventory);
            var result = await character.AddItems(itemsPack.ToList(), containerPropertyAddress);
            return result;
        }

        public static async Task<ContainerItemOperation> AddItem(IWorldCharacterServer character, IWorldBoxServer containerBox, IEntitiesRepository repo)
        {
            if (containerBox == null)
                return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcNotFound };

            var sourceInventoryPropertyAddress = EntityPropertyResolver.GetPropertyAddress(containerBox.Inventory);
            var destPropertyAddress = EntityPropertyResolver.GetPropertyAddress(character.Inventory);

            var itemTransaction = new ItemMoveAllManagementTransaction(sourceInventoryPropertyAddress, destPropertyAddress, false, false, repo);
            return await itemTransaction.ExecuteTransaction();
        }

    }

    // --- Util types: ----------------------------------------

    public enum GrantItemsContext
    {
        Gathering,
        Mining,
    }

}
