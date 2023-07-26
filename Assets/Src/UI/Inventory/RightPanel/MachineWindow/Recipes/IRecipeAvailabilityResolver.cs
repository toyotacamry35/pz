using System;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;

namespace Uins.Inventory
{
    public interface IRecipeAvailabilityResolver
    {
        int GetMaxCraftedProducts(RecipeBaseViewModel recipeVm);
        bool GetIsAvailableByWorkbenchType(RecipeBaseViewModel recipeVm);
        ReactiveProperty<WorkbenchTypeDef> WorkbenchTypeRp { get; }
        event Action SlotsChanged;
    }
}