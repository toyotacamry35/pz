using SharedCode.Aspects.Building;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class TechnoContextBuildRecipeViewModel : BindingViewModel, IBuildRecipeSource
    {
        private BuildRecipeDef _buildRecipeDef;

        public BuildRecipeDef BuildRecipeDef
        {
            get => _buildRecipeDef;
            set
            {
                if (_buildRecipeDef != value)
                {
                    var oldHasRecipe = HasRecipe;
                    _buildRecipeDef = value;
                    OnCraftRecipeDefChanged();
                    if (oldHasRecipe != HasRecipe)
                        NotifyPropertyChanged(nameof(HasRecipe));
                }
            }
        }

        private void OnCraftRecipeDefChanged()
        {
            RecipeBlueprintIcon = BuildRecipeDef?.BlueprintIcon?.Target;
        }

        [Binding]
        public bool HasRecipe => _buildRecipeDef != null;

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