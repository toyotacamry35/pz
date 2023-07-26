using Assets.ColonyShared.SharedCode.Aspects.Craft;
using GeneratorAnnotations;
using SharedCode.DeltaObjects;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.MapSystem;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    public interface IWorldMachine : IEntity, IMountable, IHasOpenMechanics, ICanBeActive, IHasCraftEngine, IHasOwner, IHasCraftProgressInfo, IHasContainerApi, IDatabasedMapedEntity, IHasStatsEngine, IHasHealth, IHasWizardEntity, IOpenable
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IBuildingContainer Inventory { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IMachineFuelContainer FuelContainer { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IMachineOutputContainer OutputContainer { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IDeltaList<ICraftingPriorityQueueItem> PriorityQueue { get; }

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<RecipeOperationResult> SetRecipeActivity(CraftRecipeDef recipeDef, bool activate);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<RecipeOperationResult> SetRecipePriority(CraftRecipeDef recipeDef, int priority);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task UpdateCraftProgressInfo();
    }

    [GenerateDeltaObjectCode]
    public interface ICraftingPriorityQueueItem : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        CraftRecipeDef CraftRecipe { get; set; }
    }

    [Flags]
    public enum RecipeOperationResult
    {
        None = 0x00,
        SuccessAdded = 0x01,
        ErrorUnknown = 0x02,
        ErrorNotExist = 0x04,
        ErrorNotOwner = 0x08,

        Success = SuccessAdded,
        Error = ErrorUnknown | ErrorNotExist | ErrorNotOwner
    }

    public static class RecipeOperationResultExtensions
    {
        public static bool Is(this RecipeOperationResult check, RecipeOperationResult check2)
        {
            return (check & check2) != 0;
        }
    }
}