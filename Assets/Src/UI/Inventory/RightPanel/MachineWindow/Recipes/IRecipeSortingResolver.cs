using Assets.ColonyShared.SharedCode.Aspects.Craft;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;

namespace Uins.Inventory
{
    public interface IRecipeSortingResolver
    {
        //Sorting
        int GetSortingIndex(RepairRecipeDef recipeDef, bool isAvailableRecipe, WorkbenchTypeDef currentWorkbenchType);
        void SetAvailabilityChanged(RecipeBaseViewModel recipeBaseViewModel);

        //Filtering
        RecipesFilterViewModel RecipesFilterViewModel { get; }
    }

    public interface IHasCraftEngineParams
    {
        bool IsMachineTab { get; }
        OuterRef CraftEngineOuterRef { get; }
    }
}