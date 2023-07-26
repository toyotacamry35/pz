using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class BuildRecipeTooltipViewModel : ItemPropsBaseViewModel
    {
        //=== Props ===========================================================

        private IBuildRecipeSource _buildRecipeSource;

        public override object TargetDescription
        {
            get => _buildRecipeSource;
            set
            {
                _buildRecipeSource = value as IBuildRecipeSource;
                var buildRecipeDef = _buildRecipeSource?.BuildRecipeDef;
                if (_buildRecipeSource.AssertIfNull(nameof(_buildRecipeSource)) ||
                    buildRecipeDef.AssertIfNull(nameof(buildRecipeDef)))
                    return;

                BigIcon = buildRecipeDef.Icon?.Target;
                ItemName = buildRecipeDef.NameLs;
                Description = buildRecipeDef.DescriptionLs;
                InventoryFiltrableType = buildRecipeDef.InventoryFiltrableType;
                ItemTier = buildRecipeDef.Tier;
            }
        }

        private float _productCraftTime;

        [Binding]
        public float ProductCraftTime
        {
            get => _productCraftTime;
            set
            {
                if (!Mathf.Approximately(_productCraftTime, value))
                {
                    _productCraftTime = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Protected =======================================================

        protected override void OnAwake()
        {
        }
    }
}