using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public abstract class RecipeBaseViewModel : BindingViewModel, IContextViewTargetWithParams, ICraftRecipeSource
    {
        private IHasContextStream _hasContextStream;
        private IRecipeAvailabilityResolver _recipeAvailabilityResolver;
        private IContextViewTargetResolver _contextViewTargetResolver;
        private IRecipeSortingResolver _recipeSortingResolver;

        private DisposableComposite _tempoD = new DisposableComposite();


        //=== Props ==============================================================

        public IHasCraftEngineParams HasCraftEngineParams { get; set; }

        public bool IsInMachineTab => HasCraftEngineParams?.IsMachineTab ?? false;

        public CraftRecipeDef Recipe => CraftRecipeDef;

        private InventoryFiltrableTypeDef _inventoryFiltrableType;

        public InventoryFiltrableTypeDef InventoryFiltrableType
        {
            get => _inventoryFiltrableType;
            set
            {
                if (_inventoryFiltrableType != value)
                {
                    var oldHasFiltrableTypeIcon = HasFiltrableTypeIcon;
                    _inventoryFiltrableType = value;
                    NotifyPropertyChanged(nameof(FiltrableTypeIcon));
                    if (oldHasFiltrableTypeIcon != HasFiltrableTypeIcon)
                        NotifyPropertyChanged(nameof(HasFiltrableTypeIcon));
                }
            }
        }

        public Dictionary<BaseItemResource, int> RequiredItems { get; } = new Dictionary<BaseItemResource, int>();

        public virtual bool IsActive => false;

        private CraftRecipeDef _craftRecipeDef;

        public CraftRecipeDef CraftRecipeDef
        {
            get => _craftRecipeDef;
            private set
            {
                if (_craftRecipeDef == value ||
                    value.AssertIfNull($"{nameof(CraftRecipeDef)} {nameof(value)}"))
                    return;

                _craftRecipeDef = value;
                Tier = _craftRecipeDef.Tier;
                Title = _craftRecipeDef.GetRecipeOrProductNameLs();
                Description = _craftRecipeDef.GetRecipeOrProductDescriptionLs();
                RecipeBlueprintIcon = _craftRecipeDef.BlueprintIcon?.Target;
                WorkbenchTypeDefs = (!_craftRecipeDef?.HasWorkbenchTypes ?? false)
                    ? new List<WorkbenchTypeDef>()
                    : _craftRecipeDef.WorkbenchTypes.Select(v => v.Target).ToList();
                ProductVariant = 0;
                RecipeModifiers = _craftRecipeDef?.GetSimpleRecipeModifiers();
                ProductItemResource = _craftRecipeDef.GetProductItemResource(ProductVariant);
                ProductCraftTime = CraftRecipeDef?.GetProductCraftTime(ProductVariant) ?? 0;
                InventoryFiltrableType = GetInventoryFiltrableType();
                GatheringRequiredItems(_craftRecipeDef);
            }
        }

        private bool _isVisible = true;

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
                    IsVisibleFinally = GetIsVisibleFinally();
                }
            }
        }

        private bool _isFilteredOut;

        [Binding]
        public bool IsFilteredOut
        {
            get => _isFilteredOut;
            private set
            {
                if (_isFilteredOut != value)
                {
                    _isFilteredOut = value;
                    NotifyPropertyChanged();
                    IsVisibleFinally = GetIsVisibleFinally();
                }
            }
        }

        private bool _isVisibleFinally = true;

        [Binding]
        public bool IsVisibleFinally
        {
            get => _isVisibleFinally;
            set
            {
                if (_isVisibleFinally != value)
                {
                    _isVisibleFinally = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _productVariant;

        public int ProductVariant
        {
            get => _productVariant;
            set
            {
                if (_productVariant != value)
                {
                    _productVariant = value;
                    ProductItemResource = CraftRecipeDef?.GetProductItemResource(_productVariant) ?? null;
                    ProductCraftTime = CraftRecipeDef?.GetProductCraftTime(_productVariant) ?? 0;
                }
            }
        }

        private CraftRecipeModifier[] _recipeModifiers;

        public CraftRecipeModifier[] RecipeModifiers
        {
            get => _recipeModifiers;
            set
            {
                if (_recipeModifiers != value)
                {
                    _recipeModifiers = value;
                }
            }
        }

        public BaseItemResource _productItemResource;

        public BaseItemResource ProductItemResource
        {
            get => _productItemResource;
            set
            {
                if (_productItemResource != value)
                {
                    var oldProductWeight = ProductWeight;
                    var oldProductIcon = ProductIcon;
                    var oldHasProductIcon = HasProductIcon;
                    _productItemResource = value;
                    if (oldProductIcon != ProductIcon)
                        NotifyPropertyChanged(nameof(ProductIcon));
                    if (oldHasProductIcon != HasProductIcon)
                        NotifyPropertyChanged(nameof(HasProductIcon));
                    if (!Mathf.Approximately(oldProductWeight, ProductWeight))
                        NotifyPropertyChanged(nameof(ProductWeight));
                    InventoryFiltrableType = GetInventoryFiltrableType();
                }
            }
        }

        [Binding]
        public Sprite ProductIcon => ProductItemResource?.Icon.Target;

        [Binding]
        public bool HasProductIcon => ProductIcon != null;

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

        private List<WorkbenchTypeDef> _workbenchTypeDefs;

        [Binding]
        public List<WorkbenchTypeDef> WorkbenchTypeDefs
        {
            get => _workbenchTypeDefs;
            set
            {
                if (_workbenchTypeDefs != value)
                {
                    var oldHasWorkbenchTypeDefs = HasWorkbenchTypeDefs;
                    var oldWorkbenchTypeSprite = WorkbenchTypeSprite;
                    var oldLackOfWorkbenchTypeSprite = LackOfWorkbenchTypeSprite;
                    _workbenchTypeDefs = value;
                    NotifyPropertyChanged();
                    if (oldHasWorkbenchTypeDefs != HasWorkbenchTypeDefs)
                        NotifyPropertyChanged(nameof(HasWorkbenchTypeDefs));

                    if (oldWorkbenchTypeSprite != WorkbenchTypeSprite)
                        NotifyPropertyChanged(nameof(WorkbenchTypeSprite));

                    if (oldLackOfWorkbenchTypeSprite != LackOfWorkbenchTypeSprite)
                        NotifyPropertyChanged(nameof(LackOfWorkbenchTypeSprite));

                    RecipeWorkbenchTypeIcon = GetRecipeWorkbenchTypeIcon(_workbenchTypeDefs.FirstOrDefault());
                    RecipeWorkbenchTypeName = _workbenchTypeDefs?.FirstOrDefault()?.DisplayNameLs ?? LsExtensions.Empty;
                }
            }
        }

        [Binding]
        public bool HasWorkbenchTypeDefs => WorkbenchTypeDefs != null && WorkbenchTypeDefs.Any(t => t != null);

        [Binding]
        public Sprite WorkbenchTypeSprite => GetWorkbenchTypeSprite();

        [Binding]
        public bool LackOfWorkbenchTypeSprite => HasWorkbenchTypeDefs && WorkbenchTypeSprite == null;

        private LocalizedString _recipeWorkbenchTypeName;

        [Binding]
        public LocalizedString RecipeWorkbenchTypeName
        {
            get => _recipeWorkbenchTypeName;
            set
            {
                if (!_recipeWorkbenchTypeName.Equals(value))
                {
                    _recipeWorkbenchTypeName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Sprite _recipeWorkbenchTypeIcon;

        [Binding]
        public Sprite RecipeWorkbenchTypeIcon
        {
            get => _recipeWorkbenchTypeIcon;
            set
            {
                if (_recipeWorkbenchTypeIcon != value)
                {
                    _recipeWorkbenchTypeIcon = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(HasRecipeWorkbenchTypeIcon));
                }
            }
        }

        [Binding]
        public bool HasRecipeWorkbenchTypeIcon => RecipeWorkbenchTypeIcon != null;

        [Binding]
        public float ProductWeight => ProductItemResource?.Weight ?? 0;

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

        private int _tier;

        [Binding]
        public int Tier
        {
            get => _tier;
            set
            {
                if (_tier != value)
                {
                    _tier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _title;

        [Binding]
        public LocalizedString Title
        {
            get => _title;
            set
            {
                if (!_title.Equals(value))
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private LocalizedString _description;

        [Binding]
        public LocalizedString Description
        {
            get => _description;
            set
            {
                if (!_description.Equals(value))
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSelected;

        [Binding]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (_isSelected)
                        TakeContext();

                    NotifyPropertyChanged();
                }
            }
        }

        private int _maxCraftedProducts;

        [Binding]
        public int MaxCraftedProducts
        {
            get => _maxCraftedProducts;
            set
            {
                if (_maxCraftedProducts != value)
                {
                    _maxCraftedProducts = value;
                    HasItemsForCraft = _maxCraftedProducts > 0;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasItemsForCraft;

        [Binding]
        public bool HasItemsForCraft
        {
            get => _hasItemsForCraft;
            set
            {
                if (_hasItemsForCraft != value)
                {
                    var oldIsAvailableFinally = IsAvailableFinally;
                    var oldIsAvailableFinallyColorIndex = IsAvailableFinallyColorIndex;
                    _hasItemsForCraft = value;
                    NotifyPropertyChanged();
                    if (oldIsAvailableFinally != IsAvailableFinally)
                        NotifyPropertyChanged(nameof(IsAvailableFinally));
                    if (oldIsAvailableFinallyColorIndex != IsAvailableFinallyColorIndex)
                    {
                        NotifyPropertyChanged(nameof(IsAvailableFinallyColorIndex));
                    }

                    _recipeSortingResolver?.SetAvailabilityChanged(this);
                }
            }
        }

        private bool _isAvailableByWorkbenchType;

        [Binding]
        public bool IsAvailableByWorkbenchType
        {
            get => _isAvailableByWorkbenchType;
            set
            {
                if (_isAvailableByWorkbenchType != value)
                {
                    var oldIsAvailableFinally = IsAvailableFinally;
                    var oldIsAvailableFinallyColorIndex = IsAvailableFinallyColorIndex;
                    _isAvailableByWorkbenchType = value;
                    NotifyPropertyChanged();
                    if (oldIsAvailableFinally != IsAvailableFinally)
                        NotifyPropertyChanged(nameof(IsAvailableFinally));
                    if (oldIsAvailableFinallyColorIndex != IsAvailableFinallyColorIndex)
                        NotifyPropertyChanged(nameof(IsAvailableFinallyColorIndex));
                }
            }
        }

        [Binding]
        public bool IsAvailableFinally => IsAvailableByWorkbenchType && HasItemsForCraft;

        [Binding]
        public int IsAvailableFinallyColorIndex => GetIsAvailableFinallyColorIndex();

        [Binding]
        public Sprite FiltrableTypeIcon => InventoryFiltrableType?.Icon?.Target;

        [Binding]
        public bool HasFiltrableTypeIcon => FiltrableTypeIcon != null;


        //=== Public ==========================================================

        public void Subscribes(
            IRecipeAvailabilityResolver recipeAvailabilityResolver,
            IHasContextStream hasContextStream,
            InventoryTabType tabType,
            CraftRecipeDef craftRecipeDef,
            IContextViewTargetResolver contextViewTargetResolver,
            IHasCraftEngineParams hasCraftEngineParams)
        {
            if (recipeAvailabilityResolver.AssertIfNull(nameof(recipeAvailabilityResolver)) ||
                hasContextStream.AssertIfNull(nameof(hasContextStream)) ||
                hasCraftEngineParams.AssertIfNull(nameof(hasCraftEngineParams)))
                return;

            _recipeAvailabilityResolver = recipeAvailabilityResolver;
            _contextViewTargetResolver = contextViewTargetResolver;
            _hasContextStream = hasContextStream;
            CraftRecipeDef = craftRecipeDef;
            HasCraftEngineParams = hasCraftEngineParams;
            TabType = tabType;

            hasContextStream.CurrentContext.Action(
                _tempoD,
                target =>
                {
                    if (IsSelected && target != this)
                        IsSelected = false;
                }
            );

            if (_recipeAvailabilityResolver != null)
            {
                _recipeAvailabilityResolver.WorkbenchTypeRp.Action(D, OnWorkbenchTypeChanged);
                _recipeAvailabilityResolver.SlotsChanged += OnSlotsChanged;
                OnSlotsChanged();
                OnWorkbenchTypeChanged(null);
            }
        }

        public void Unsubscribes()
        {
            if (_hasContextStream != null)
            {
                if (_hasContextStream.ContextValue == this)
                    TakeContext(true);

                _tempoD.Clear();
                _hasContextStream = null;
            }

            if (_recipeAvailabilityResolver != null)
            {
                _recipeAvailabilityResolver.SlotsChanged -= OnSlotsChanged;
                _recipeAvailabilityResolver = null;
            }
        }

        public override string ToString()
        {
            return $"[{GetType()}: def={CraftRecipeDef}, {nameof(IsVisible)}{IsVisible.AsSign()}, {nameof(IsSelected)}{IsSelected.AsSign()}]";
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetSortingResolver(IRecipeSortingResolver recipeSortingResolver)
        {
            _recipeSortingResolver = recipeSortingResolver;
        }


        public int GetSortingIndex(WorkbenchTypeDef workbenchTypeDef)
        {
            return _recipeSortingResolver?.GetSortingIndex(CraftRecipeDef, HasItemsForCraft, workbenchTypeDef) ?? 0;
        }

        public void ApplyFiltering(WorkbenchTypeDef workbenchTypeDef)
        {
            IsFilteredOut = IsFilteredOut_FiltrableType() ||
                            IsFilteredOut_Avaliable() ||
                            IsFilteredOut_ByWorkbenchType(workbenchTypeDef);
        }

        public bool IsBelongsToWorkbenchType(WorkbenchTypeDef workbenchTypeDef)
        {
            if (workbenchTypeDef == null || !HasWorkbenchTypeDefs)
                return false;

            return WorkbenchTypeDefs.Contains(workbenchTypeDef);
        }


        public ContextViewParams GetContextViewParamsForOpening()
        {
            if (_contextViewTargetResolver == null)
                return new ContextViewParams();

            var contextParams = _contextViewTargetResolver.GetContextParams(this);
            return new ContextViewParams()
            {
                Layout = ContextViewParams.LayoutType.TitleAndNormalBackground,
                ContextTitleLs = contextParams.Item1,
                ContextIcon = contextParams.Item2,
                HasWarningFlag = contextParams.Item3,
            };
        }

        public InventoryTabType? TabType { get; private set; }


        //=== Protected =======================================================

        private void TakeContext(bool withNull = false)
        {
            var target = withNull ? null : this;
            if (_hasContextStream is IContextViewWithParams contextViewWithParams)
            {
                contextViewWithParams.SetContext(target);
            }
            else if (_hasContextStream is IContextView contextView)
            {
                contextView.TakeContext(target);
            }
        }


        //=== Private =========================================================

        private Sprite GetWorkbenchTypeSprite()
        {
            return WorkbenchTypeDefs?.FirstOrDefault(typeDef => typeDef != null)?.Icon?.Target;
        }

        private bool IsFilteredOut_FiltrableType()
        {
            if (_recipeSortingResolver == null)
                return false;

            return _recipeSortingResolver.RecipesFilterViewModel.CurrentFiltrableTypeFilter != null &&
                   _recipeSortingResolver.RecipesFilterViewModel.CurrentFiltrableTypeFilter != InventoryFiltrableType;
        }

        private bool IsFilteredOut_Avaliable()
        {
            if (_recipeSortingResolver == null)
                return false;

            return _recipeSortingResolver.RecipesFilterViewModel.CurrentPlaFilter == PlaFilter.Avaliable && !HasItemsForCraft;
        }

        private bool IsFilteredOut_ByWorkbenchType(WorkbenchTypeDef workbenchTypeDef)
        {
            if (workbenchTypeDef == null || !IsInMachineTab)
                return false;

            return !HasWorkbenchTypeDefs || !WorkbenchTypeDefs.Contains(workbenchTypeDef);
        }

        private int GetIsAvailableFinallyColorIndex()
        {
            if (IsAvailableByWorkbenchType)
                return HasItemsForCraft ? 0 : 1; // cyan/grey

            return -1; //red
        }

        private Sprite GetRecipeWorkbenchTypeIcon(WorkbenchTypeDef workbenchTypeDef)
        {
            return workbenchTypeDef?.Icon.Target;
        }

        private InventoryFiltrableTypeDef GetInventoryFiltrableType()
        {
            if (CraftRecipeDef == null)
                return null;

            return CraftRecipeDef.InventoryFiltrableType != null
                ? CraftRecipeDef.InventoryFiltrableType
                : (ProductItemResource as ItemResource)?.InventoryFiltrableType;
        }

        private void OnWorkbenchTypeChanged(WorkbenchTypeDef workbenchTypeDef)
        {
            IsAvailableByWorkbenchType = _recipeAvailabilityResolver?.GetIsAvailableByWorkbenchType(this) ?? true;
        }

        private void OnSlotsChanged()
        {
            MaxCraftedProducts = _recipeAvailabilityResolver?.GetMaxCraftedProducts(this) ?? 0;
        }

        private void GatheringRequiredItems(CraftRecipeDef craftRecipeDef) //справедливо для авторецептов
        {
            RequiredItems.Clear();
            if (craftRecipeDef.AssertIfNull(nameof(craftRecipeDef)))
                return;

            foreach (var recipeSlot in craftRecipeDef.MandatorySlots)
            {
                if (recipeSlot.Items == null || recipeSlot.Items.Length == 0)
                    continue;

                var recipeItemStack = recipeSlot.Items[0].Item;
                var itemResource = recipeItemStack.Item.Target;

                if (itemResource == null)
                    continue;

                if (RequiredItems.ContainsKey(itemResource))
                    RequiredItems[itemResource] += recipeItemStack.Count;
                else
                    RequiredItems[itemResource] = recipeItemStack.Count;
            }
        }

        private bool GetIsVisibleFinally()
        {
            return IsVisible && !IsFilteredOut;
        }
    }
}