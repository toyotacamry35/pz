using Assets.ColonyShared.SharedCode.Aspects.Craft;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class TechnoContextCraftRecipeViewModel : BindingViewModel, ICraftRecipeSource
    {
        private CraftRecipeDef _craftRecipeDef;

        public CraftRecipeDef CraftRecipeDef
        {
            get => _craftRecipeDef;
            set
            {
                if (_craftRecipeDef != value)
                {
                    var oldHasRecipe = HasRecipe;
                    _craftRecipeDef = value;
                    OnCraftRecipeDefChanged();
                    if (oldHasRecipe != HasRecipe)
                        NotifyPropertyChanged(nameof(HasRecipe));
                }
            }
        }

        private void OnCraftRecipeDefChanged()
        {
            RecipeBlueprintIcon = CraftRecipeDef?.BlueprintIcon?.Target;
        }

        [Binding]
        public bool HasRecipe => _craftRecipeDef != null;

        private Sprite _recipeBlueprintIcon;

        [Binding]
        public Sprite RecipeBlueprintIcon
        {
            get => _recipeBlueprintIcon;
            set
            {
                if (_recipeBlueprintIcon != value)
                {
                    _recipeBlueprintIcon = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(HasRecipeBlueprintIcon));
                }
            }
        }

        [Binding]
        public bool HasRecipeBlueprintIcon => RecipeBlueprintIcon != null;

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}