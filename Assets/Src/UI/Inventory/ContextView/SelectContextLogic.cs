using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Core.Environment.Logging.Extension;
using Uins.Inventory;

namespace Uins
{
    public class SelectContextLogic
    {
        private ContextViewWithParams _contextViewWithParams;
        private CraftSideViewModel _craftSideViewModel;
        private MachineCraftSideViewModel _machineCraftSideViewModel;


        //=== Ctor ============================================================

        public SelectContextLogic(ContextViewWithParams contextViewWithParams, CraftSideViewModel craftSideViewModel,
            MachineCraftSideViewModel machineCraftSideViewModel)
        {
            _contextViewWithParams = contextViewWithParams;
            _craftSideViewModel = craftSideViewModel;
            _machineCraftSideViewModel = machineCraftSideViewModel;
            _contextViewWithParams.AssertIfNull(nameof(_contextViewWithParams));
            _craftSideViewModel.AssertIfNull(nameof(_craftSideViewModel));
            _machineCraftSideViewModel.AssertIfNull(nameof(_machineCraftSideViewModel));
        }


        //=== Public ==========================================================

        public void SelectRecipe(CraftRecipeDef craftRecipeDef)
        {
            if (craftRecipeDef.AssertIfNull(nameof(craftRecipeDef)))
                return;

            if (!TryToSelectRecipe(craftRecipeDef, _machineCraftSideViewModel, InventoryTabType.Machine))
            {
                if (!TryToSelectRecipe(craftRecipeDef, _craftSideViewModel, InventoryTabType.Crafting))
                    UI.Logger.IfWarn()?.Message($"Unable to find viewModel for {craftRecipeDef}").Write();
            }
        }


        //=== Private =========================================================

        private bool TryToSelectRecipe(CraftRecipeDef craftRecipeDef, CraftSideViewModel someSideViewModel, InventoryTabType tabType)
        {
            if (craftRecipeDef.HasWorkbenchTypes) //станочный рецепт
            {
                if (tabType == InventoryTabType.Machine)
                {
                    if (_machineCraftSideViewModel.CurrentWorkbenchTypeDef == null ||
                        !craftRecipeDef.WorkbenchTypes
                            .Select(defRef => defRef.Target)
                            .Where(def => def != null)
                            .Contains(_machineCraftSideViewModel.CurrentWorkbenchTypeDef))
                        return false;
                }
            }
            else //ручной рецепт
            {
                if (tabType == InventoryTabType.Machine)
                    return false;
            }

            var recipeViewModel = GetRecipeViewModel(craftRecipeDef, someSideViewModel);
            if (recipeViewModel != null)
            {
                recipeViewModel.IsSelected = true;
                someSideViewModel.SelectedRecipeScrollCentering();
            }
            return recipeViewModel != null;
        }

        private AvailableRecipeViewModel GetRecipeViewModel(CraftRecipeDef craftRecipeDef, CraftSideViewModel someSideViewModel)
        {
            return someSideViewModel.KnownRecipeViewModels.FirstOrDefault(vm => vm.CraftRecipeDef == craftRecipeDef);
        }
    }
}