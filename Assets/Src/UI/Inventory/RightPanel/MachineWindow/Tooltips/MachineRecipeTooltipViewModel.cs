using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using JetBrains.Annotations;
using L10n;
using Uins.Inventory;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class MachineRecipeTooltipViewModel : ItemPropsBaseViewModel
    {
        private const int RecipeItemsCount = 5;

        [UsedImplicitly]
        [SerializeField]
        private SidePanelRecipeSlotViewModel _sidePanelRecipeSlotVmPrefab;

        [UsedImplicitly]
        [SerializeField]
        private Transform _recipeItemsTransform;

        private List<SidePanelRecipeSlotViewModel> _recipeItems = new List<SidePanelRecipeSlotViewModel>();


        //=== Props ===========================================================

        private ICraftRecipeSource _craftRecipeSource;

        public override object TargetDescription
        {
            get => _craftRecipeSource;
            set
            {
                _craftRecipeSource = value as ICraftRecipeSource;
                var craftRecipeDef = _craftRecipeSource?.CraftRecipeDef;
                if (_craftRecipeSource.AssertIfNull(nameof(_craftRecipeSource)) || 
                    craftRecipeDef.AssertIfNull(nameof(craftRecipeDef)))
                    return;

                var productItemResource = craftRecipeDef.GetProductItemResource(0);
                if (productItemResource.AssertIfNull(nameof(productItemResource)))
                    return;

                BigIcon = productItemResource.BigIcon?.Target;
                ItemName = craftRecipeDef.GetRecipeOrProductNameLs();
                Description = craftRecipeDef.GetRecipeOrProductDescriptionLs();
                ProductCraftTime = craftRecipeDef.GetProductCraftTime(0);
                InventoryFiltrableType = craftRecipeDef.InventoryFiltrableType;
                ItemTier = craftRecipeDef.Tier;
                ItemWeight = productItemResource.Weight;

                var regularStats = CraftRecipeContextView.GetRecipeRegularStats(
                    productItemResource, craftRecipeDef.GetSimpleRecipeModifiers());
                var mainStats = SlotPropsBaseViewModel.GetMainStats(productItemResource, regularStats);
                StatsViewModel.SetItemStats(mainStats, regularStats);
                FillRecipeItems(craftRecipeDef, _recipeItems);
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
            _sidePanelRecipeSlotVmPrefab.AssertIfNull(nameof(_sidePanelRecipeSlotVmPrefab));
            _recipeItemsTransform.AssertIfNull(nameof(_recipeItemsTransform));
            CreateRecipeItems();
        }


        //=== Private =========================================================

        public void FillRecipeItems(CraftRecipeDef craftRecipeDef, List<SidePanelRecipeSlotViewModel> recipeItems)
        {
            var mandatorySlots = craftRecipeDef?.MandatorySlots;
            for (int i = 0; i < RecipeItemsCount; i++)
            {
                if (mandatorySlots != null &&
                    i < mandatorySlots.Length &&
                    mandatorySlots[i].Items != null &&
                    mandatorySlots[i].Items.Length > 0)
                {
                    var recipeItemStack = mandatorySlots[i].Items[0].Item;
                    recipeItems[i].SetItemStack(recipeItemStack.Item.Target, recipeItemStack.Count);
                }
                else
                {
                    recipeItems[i].SetItemStack(null);
                }
            }
        }


        private void CreateRecipeItems()
        {
            for (int i = 0; i < RecipeItemsCount; i++)
            {
                var recipeItemViewModel = Instantiate(_sidePanelRecipeSlotVmPrefab, _recipeItemsTransform);
                recipeItemViewModel.name = $"{_sidePanelRecipeSlotVmPrefab.name}{i}";
                _recipeItems.Add(recipeItemViewModel);
            }
        }
    }
}