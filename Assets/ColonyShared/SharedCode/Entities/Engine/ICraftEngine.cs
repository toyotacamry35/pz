using Assets.ColonyShared.SharedCode.Aspects.Craft;
using GeneratorAnnotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.EntitySystem.Delta;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedCode.Entities.Engine
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    public interface ICraftEngine : IEntity, IHasInventory, IHasOwner, IHasContainerApi
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] bool UseOwnOutputContainer { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IContainer IntermediateFuelContainer { get; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IContainer IntermediateCraftContainer { get; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IContainer OutputContainer { get; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] PropertyAddress FuelContainerAddress { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] PropertyAddress ResultContainerAddress { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] Task SetResultContainerAddress(PropertyAddress resultContainerAddress);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<int, ICraftingQueueItem> CraftingQueue { get; }
        [ReplicationLevel(ReplicationLevel.Server)] int MaxQueueSize { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<CraftRecipeDef, int> CraftRecipesUsageStats { get; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<CraftRecipeDef, long> CraftRecipesLastUsageTimes { get; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task UpdateFuelTime();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] long FuelTimeAlreadyInUse { get; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] long StartFuelTimeUTC0InMilliseconds { get; } // Info: время исчисляется !только! по UTC+0

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task UpdateCraftingTime();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] long StartCraftingTimeUTC0InMilliseconds { get; } // Info: время исчисляется !только! по UTC+0

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task UpdateRepairTime(PropertyAddress itemAddress, int itemIndex);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<bool> CanRun();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<CraftOperationResult> RunCraft();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task StopCraft();
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<CraftOperationResult> Craft(CraftRecipeDef recipe, int variantIdx, int count, int[] mandatorySlotPermutation, int[] optionalSlotPermutation, PropertyAddress inventoryAddress, PropertyAddress inventoryAddress2);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<CraftOperationResult> Repair(PropertyAddress itemAddress, int itemIndex, int recipeIndex, int variantIdx, int[] mandatorySlotPermutation, int[] optionalSlotPermutation, PropertyAddress fromInventoryAddress, PropertyAddress fromInventoryAddress2);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<CraftOperationResult> RemoveCraft(int recipeIndex);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<CraftOperationResult> SwapCraft(int index1, int index2);

        [ReplicationLevel(ReplicationLevel.Server)] WorkbenchTypeDef CurrentWorkbenchType { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<CraftOperationResult> StopCraftWithWorkbench(WorkbenchTypeDef workbenchType);
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] Task<CraftOperationResult> ContinueCraftWithWorkbench(WorkbenchTypeDef workbenchType);

        [ReplicationLevel(ReplicationLevel.Server)] IDeltaList<ChainCancellationToken> FuelScheduleCancellation { get; }
        [ReplicationLevel(ReplicationLevel.Server)] IDeltaList<ChainCancellationToken> CraftScheduleCancellation { get; }
    }
}