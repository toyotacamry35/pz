using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.Inventory;
using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine.UI;
using UnityWeld.Binding;
using SharedCode.Aspects.Item.Templates;
using Uins.Slots;
using TMPro;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ContainerApis;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using ResourceSystem.Utils;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using Src.Input;
using SharedCode.Serializers;

namespace Uins.Inventory
{
    /// <summary>
    /// UI рецепта в ContextView
    /// </summary>
    [Binding]
    public class CraftRecipeContextView : DependencyEndNode, IRecipeAvailabilityResolver, IContextViewTargetResolver
    {
        private const int MaxRecipeChangeElements = 8;
        private const int MaxRecipeBaseElements = 5;
        private const int MaxRecipeExtraElements = 3;
        private const float ButtonAfterPressLockTime = 1;
        private const float ButtonTimeTickerInterval = 0.2f;

        public event Action SlotsChanged;

        /// <summary>
        /// Количество создаваемых предметов
        /// </summary>
        public Action<int> ChangeCountEvent;

        [SerializeField, UsedImplicitly]
        private InventoryNode _inventoryNode;

        [SerializeField, UsedImplicitly]
        private OurCharacterSlotsViewModel _ourCharacterSlotsViewModel;

        [SerializeField, UsedImplicitly]
        private ContextViewWithParams _contextViewWithParams;

        [SerializeField, UsedImplicitly]
        private StatResource _weaponStatResource;

        [SerializeField, UsedImplicitly]
        private WindowStackId _stack;

        [SerializeField, UsedImplicitly]
        private ContextViewTitlesDefRef _contextViewTitlesDefRef;

        [SerializeField, UsedImplicitly]
        private StatsViewModel _statsViewModel;

        [SerializeField, UsedImplicitly]
        private GameObject _visualGameObject;

        [SerializeField, UsedImplicitly]
        private TMP_InputField _countField;

        /// <summary>
        /// Блокировки и/или переопределения привязок клавиш к командам активируемые при открытии данного окна
        /// </summary>    
        [SerializeField, UsedImplicitly]
        private InputBindingsRef _inputBindingsWhenCountFieldSelected;

        [SerializeField, UsedImplicitly]
        private GameObject _craftRecipeBaseElementPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _parentCraftRecipeBaseElements;

        // //экстра компоненты
        // [SerializeField, UsedImplicitly]
        // private GameObject _craftRecipeExtraElementPrefab;
        //
        // [SerializeField, UsedImplicitly]
        // private Transform _parentCraftRecipeExtraElements;

        //альтернативные
        [SerializeField, UsedImplicitly]
        private GameObject _craftRecipeChangePanel;

        [SerializeField, UsedImplicitly]
        private GameObject _craftRecipeChangeElementPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _parentCraftRecipeChangeElements;

        [SerializeField, UsedImplicitly]
        private RectTransform _recipeChangeLeftOutline;

        [SerializeField, UsedImplicitly]
        private RectTransform _recipeChangeRightOutline;

        [SerializeField, UsedImplicitly]
        private Transform _recipeChangeCentrObj;

        [SerializeField, UsedImplicitly]
        private ItemResourceRef _missingItemDefRef;

        private CraftRecipeElementUI[] _recipeBaseElements;

        //базовые компоненты
        private bool _wasAddCraftRecipeElement;

        // private CraftRecipeElementUI[] _recipeExtraElements;
        private CraftRecipeElementUI[] _recipeChangeElements;
        private CraftRecipeElementUI _openedCraftRecipeElementUI;
        private OuterRef _craftEngineOuterRef;
        private PropertyAddress _inventoryPropertyAddress;
        private PropertyAddress _dollPropertyAddress;
        private DateTime _maxCountButtonLastPressTime;


        //=== Props ===========================================================

        public ReactiveProperty<WorkbenchTypeDef> WorkbenchTypeRp { get; } = new ReactiveProperty<WorkbenchTypeDef>() {Value = null};

        [Binding]
        public bool IsCraftAvailable => IsCraftAvailableByIngred && IsCraftAvailableByExtraCond;

        private bool _isCraftAvailableByIngred;

        [Binding]
        public bool IsCraftAvailableByIngred
        {
            get => _isCraftAvailableByIngred;
            private set
            {
                if (_isCraftAvailableByIngred != value)
                {
                    var oldIsCraftAvailable = IsCraftAvailable;
                    _isCraftAvailableByIngred = value;
                    NotifyPropertyChanged();
                    if (oldIsCraftAvailable != IsCraftAvailable)
                        NotifyPropertyChanged(nameof(IsCraftAvailable));
                }
            }
        }

        private bool _isCraftAvailableByExtraCond;

        [Binding]
        public bool IsCraftAvailableByExtraCond
        {
            get => _isCraftAvailableByExtraCond;
            private set
            {
                if (_isCraftAvailableByExtraCond != value)
                {
                    _isCraftAvailableByExtraCond = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsCraftAvailable));
                }
            }
        }

        private RecipeBaseViewModel _currentRecipeVm;

        /// <summary>
        /// Просматриваемый рецепт крафта из скролла рецептов
        /// </summary>
        public RecipeBaseViewModel CurrentRecipeVm
        {
            get => _currentRecipeVm;
            set
            {
                if (value != _currentRecipeVm)
                {
                    _currentRecipeVm = value;
                    OnCurrentRecipeVmChanged(_currentRecipeVm);
                }
            }
        }

        private Sprite _filtrableTypeIcon;

        [Binding]
        public Sprite FiltrableTypeIcon
        {
            get => _filtrableTypeIcon;
            private set
            {
                if (_filtrableTypeIcon != value)
                {
                    _filtrableTypeIcon = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(HasFiltrableTypeIcon));
                }
            }
        }

        [Binding]
        public bool HasFiltrableTypeIcon => _filtrableTypeIcon != null;

        private Sprite _bigIcon;

        [Binding]
        public Sprite BigIcon
        {
            get => _bigIcon;
            private set
            {
                if (_bigIcon != value)
                {
                    _bigIcon = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(HasBigIcon));
                }
            }
        }

        [Binding]
        public bool HasBigIcon => _bigIcon != null;

        private Sprite _blueprintIcon;

        [Binding]
        public Sprite BlueprintIcon
        {
            get => _blueprintIcon;
            private set
            {
                if (_blueprintIcon != value)
                {
                    _blueprintIcon = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(HasBlueprintIcon));
                }
            }
        }

        [Binding]
        public bool HasBlueprintIcon => _blueprintIcon != null;

        private LocalizedString _productName;

        [Binding]
        public LocalizedString ProductName
        {
            get => _productName;
            private set
            {
                if (!_productName.Equals(value))
                {
                    _productName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _productWeight;

        [Binding]
        public float ProductWeight
        {
            get => _productWeight;
            private set
            {
                if (!Mathf.Approximately(_productWeight, value))
                {
                    _productWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _productCraftTime;

        [Binding]
        public float ProductCraftTime
        {
            get => _productCraftTime;
            private set
            {
                if (!Mathf.Approximately(_productCraftTime, value))
                {
                    _productCraftTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isMinButtonActive;

        [Binding]
        public bool IsMinButtonActive
        {
            get => _isMinButtonActive;
            private set
            {
                if (_isMinButtonActive != value)
                {
                    _isMinButtonActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isMinusButtonActive;

        [Binding]
        public bool IsMinusButtonActive
        {
            get => _isMinusButtonActive;
            private set
            {
                if (_isMinusButtonActive != value)
                {
                    _isMinusButtonActive = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public bool IsMaxCountButtonActive { get; private set; }

        private int _productsCount;

        [Binding]
        public int ProductsCount
        {
            get => _productsCount;
            private set
            {
                if (_productsCount != value)
                {
                    var oldIsVisibleProductsCount = IsVisibleProductsCount;
                    _productsCount = value;
                    //UI.CallerLog($"ProductsCount={ProductsCount}"); //2del
                    NotifyPropertyChanged();
                    if (oldIsVisibleProductsCount != IsVisibleProductsCount)
                        NotifyPropertyChanged(nameof(IsVisibleProductsCount));
                }
            }
        }

        [Binding]
        public bool IsVisibleProductsCount => _productsCount > 1;

        /// <summary>
        /// путь из обязательных предметов, создаваемых изначально для проверки достаточности ресурсов в инвентаре
        /// </summary>
        public CraftRecipeModifier[] RecipeModifiers { get; private set; }

        /// <summary>
        /// путь из индексов альтернативных предметов для обязательных слотов
        /// </summary>
        public int[] RecipeModifiersIndices { get; private set; }

        /// <summary>
        /// путь из индексов альтернативных предметов для дополнительных слотов
        /// </summary>
        public int[] RecipeModifiersIndicesOptional { get; private set; }

        /// <summary>
        /// выбранный вариант конечного предмета. зависит от набора pathItem
        /// </summary>
        public int VariantIndex { get; private set; }

        private ViewVariant _productVariant;

        public ViewVariant ProductVariant
        {
            get => _productVariant;
            private set
            {
                _productVariant = value;
                OnChangeProductVariant();
            }
        }

        private int _count;

        public int Count
        {
            get => _count;
            private set
            {
                _count = value;
                ChangeCountEvent?.Invoke(value);
            }
        }

        private ICharacterItemsNotifier CharacterItemsNotifier => _ourCharacterSlotsViewModel;


        //=== Unity ===========================================================

        private void Start()
        {
            if (_visualGameObject.AssertIfNull(nameof(_visualGameObject)) ||
                _ourCharacterSlotsViewModel.AssertIfNull(nameof(_ourCharacterSlotsViewModel)) ||
                _contextViewWithParams.AssertIfNull(nameof(_contextViewWithParams)) ||
                _statsViewModel.AssertIfNull(nameof(_statsViewModel)) ||
                _contextViewTitlesDefRef.Target.AssertIfNull(nameof(_contextViewTitlesDefRef)))
                return;

            if (!_inventoryNode.AssertIfNull(nameof(_inventoryNode)))
                _inventoryNode.InventoryStateAndMode.Action(D, OnInventoryWindowStateChanged);

            _countField.onEndEdit.AddListener(ChangeCount);
            CharacterItemsNotifier.CharacterItemsChanged += OnCharacterItemsChanged;
            _contextViewWithParams.Vmodel.SubStream(D, vm => vm.CurrentContext).Action(D, OnContextViewTargetChanged);
            SwitchVisibility(false);

            var maxCountButtonLockStream = TimeTicker.Instance.GetLocalTimer(ButtonTimeTickerInterval)
                .Func(D, dt => (dt - _maxCountButtonLastPressTime).TotalSeconds > ButtonAfterPressLockTime); //пауза между нажатиями CraftMaxCount-кнопки 
            Bind(maxCountButtonLockStream, () => IsMaxCountButtonActive);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _countField.onEndEdit.RemoveListener(ChangeCount);
            if (_wasAddCraftRecipeElement)
            {
                for (int i = 0; i < _recipeBaseElements.Length; i++)
                {
                    _recipeBaseElements[i].ClickEvent -= ShowAlternativeItems;
                }

                // for (int i = 0; i < _recipeExtraElements.Length; i++)
                // {
                //     _recipeExtraElements[i].ClickEvent -= ShowAlternativeItems;
                // }
            }

            CharacterItemsNotifier.CharacterItemsChanged -= OnCharacterItemsChanged;
        }


        //=== Public ==========================================================

        public (LocalizedString, Sprite, bool) GetContextParams(IContextViewTargetWithParams target)
        {
            var recipeBaseViewModel = target as RecipeBaseViewModel;
            if (recipeBaseViewModel.AssertIfNull(nameof(recipeBaseViewModel)))
                return (LsExtensions.Empty, null, false);

            return (GetContextTitle(recipeBaseViewModel), GetContextIcon(recipeBaseViewModel), !GetIsAvailableByWorkbenchType(recipeBaseViewModel));
        }

        public bool GetIsAvailableByWorkbenchType(RecipeBaseViewModel recipeBaseViewModel)
        {
            return !recipeBaseViewModel.HasWorkbenchTypeDefs ||
                   (recipeBaseViewModel.WorkbenchTypeDefs.Contains(WorkbenchTypeRp.Value) && recipeBaseViewModel.IsInMachineTab);
        }

        public void SetAvailableWorkbenchType(WorkbenchTypeDef availableWorkbenchType)
        {
            IsCraftAvailableByExtraCond = GetIsCraftAvailableByExtraCond();
            WorkbenchTypeRp.Value = availableWorkbenchType;
        }

        public void SetCharacterContainersAddresses(PropertyAddress inventoryPropertyAddress = null, PropertyAddress dollPropertyAddress = null)
        {
            _inventoryPropertyAddress = inventoryPropertyAddress;
            _dollPropertyAddress = dollPropertyAddress;
        }

        public bool FindFirstAvailableCraftVariant()
        {
            var recipe = CurrentRecipeVm.CraftRecipeDef;

            RecipeModifiers = new CraftRecipeModifier[recipe.MandatorySlots.Length];
            RecipeModifiersIndices = new int[recipe.MandatorySlots.Length];
            RecipeModifiersIndicesOptional = new int[recipe.OptionalSlots.Length];
            for (int i = 0; i < RecipeModifiersIndicesOptional.Length; i++)
                RecipeModifiersIndicesOptional[i] = -1;

            var isAvailable = true;
            for (int mandatorySlotIdx = 0; mandatorySlotIdx < recipe.MandatorySlots.Length; mandatorySlotIdx++)
            {
                var mandatorySlotItems = recipe.MandatorySlots[mandatorySlotIdx].Items;
                for (int i = 0; i < mandatorySlotItems.Length; i++)
                {
                    if (_ourCharacterSlotsViewModel.GetItemResourceCount(mandatorySlotItems[i].Item.Item.Target) >=
                        mandatorySlotItems[i].Item.Count)
                    {
                        RecipeModifiers[mandatorySlotIdx] = mandatorySlotItems[i];
                        RecipeModifiersIndices[mandatorySlotIdx] = i;
                        break;
                    }

                    if (i == mandatorySlotItems.Length - 1)
                    {
                        //если ни на один из вариантов не хватает ингридиентов, ставим первый в списке
                        RecipeModifiers[mandatorySlotIdx] = mandatorySlotItems[0];
                        RecipeModifiersIndices[mandatorySlotIdx] = 0;
                        isAvailable = false;
                    }
                }
            }

            return isAvailable;
        }

        public int GetMaxCraftedProducts(RecipeBaseViewModel recipeVm)
        {
            if (recipeVm.AssertIfNull(nameof(recipeVm)))
                return 0;

            var recipe = recipeVm.CraftRecipeDef;
            for (int mandatorySlotIdx = 0; mandatorySlotIdx < recipe.MandatorySlots.Length; mandatorySlotIdx++)
            {
                var mandatorySlot = recipe.MandatorySlots[mandatorySlotIdx];
                var isAnyItemEnough = false;
                for (int i = 0; i < mandatorySlot.Items.Length; i++)
                {
                    var item = mandatorySlot.Items[i].Item;
                    if (_ourCharacterSlotsViewModel.GetItemResourceCount(item.Item.Target) >= item.Count)
                    {
                        isAnyItemEnough = true;
                        break;
                    }
                }

                if (!isAnyItemEnough)
                    return 0;
            }

            return 1; //fake, но для интерфейса доступности крафта рецепта достаточно
        }

        [UsedImplicitly]
        public void OnMoreButton()
        {
            ChangeCount(Count + 1, false);
            CraftAvailabilityRequest();
        }

        [UsedImplicitly]
        public void OnLessButton()
        {
            if (Count < 1)
                return;

            ChangeCount(Count - 1, false);
            CraftAvailabilityRequest();
        }

        [UsedImplicitly]
        public void OnMinButton()
        {
            ChangeCount(1, false);
            CraftAvailabilityRequest();
        }

        [UsedImplicitly]
        public void OnMaxButton()
        {
            CraftAvailableMaxCountRequest();
        }

        /// <summary>
        /// По клику по кнопке крафта
        /// </summary>
        public void Craft()
        {
            var recipe = CurrentRecipeVm.CraftRecipeDef;
            var variantIndex = VariantIndex;
            var modifiersIndices = RecipeModifiersIndices;
            var modifiersIndicesOptional = RecipeModifiersIndicesOptional;
            var count = Count;

            AsyncUtils.RunAsyncTask(
                () =>
                    CraftEngineCommands.IsCraftAvailableAsync(
                        _craftEngineOuterRef,
                        _inventoryPropertyAddress,
                        _dollPropertyAddress,
                        recipe,
                        variantIndex,
                        modifiersIndices,
                        modifiersIndicesOptional,
                        count,
                        _missingItemDefRef.Target,
                        isSuccess =>
                        {
                            if (!isSuccess)
                            {
                                UI. Logger.IfInfo()?.Message(" Craft unabled: not enough items").Write();;
                                return;
                            }

                            AsyncUtils.RunAsyncTask(
                                () => CraftEngineCommands.DoCraftAsync(
                                    _craftEngineOuterRef,
                                    _inventoryPropertyAddress,
                                    _dollPropertyAddress,
                                    recipe,
                                    variantIndex,
                                    modifiersIndices,
                                    modifiersIndicesOptional,
                                    count,
                                    result => { UI.Logger.IfDebug()?.Message($"Craft result: {result}").Write(); }));
                        }));
        }

        public void HideAlternativeItems(bool isFirstTime = false)
        {
            if (_openedCraftRecipeElementUI == null && !isFirstTime)
                return;

            _openedCraftRecipeElementUI = null;
            if (_recipeBaseElements != null)
            {
                for (int i = 0; i < _recipeBaseElements.Length; i++)
                {
                    _recipeBaseElements[i].SetActive(null);
                }
            }

            // if (_recipeExtraElements != null)
            // {
            //     for (int i = 0; i < _recipeExtraElements.Length; i++)
            //     {
            //         _recipeExtraElements[i].SetActive(null);
            //     }
            // }

            _craftRecipeChangePanel.SetActive(false);
        }

        public static List<KeyValuePair<StatResource, float>> GetRecipeRegularStats(BaseItemResource itemResource, CraftRecipeModifier[] recipeModifiers)
        {
            var summaryGeneralStats = new Dictionary<StatResource, float>();
            var summarySpecificStats = new Dictionary<StatResource, float>();

            if (!itemResource.AssertIfNull(nameof(itemResource)))
            {
                var hasStatResource = itemResource as IHasStatsResource;
                if (hasStatResource != null)
                {
                    if (hasStatResource.GeneralStats.Target != null &&
                        hasStatResource.GeneralStats.Target.Stats != null)
                        ItemPropsBaseViewModel.GatherShownStats(
                            ref summaryGeneralStats,
                            hasStatResource.GeneralStats.Target.Stats);

                    if (hasStatResource.SpecificStats.Target != null &&
                        hasStatResource.SpecificStats.Target.Stats != null)
                        ItemPropsBaseViewModel.GatherShownStats(
                            ref summarySpecificStats,
                            hasStatResource.SpecificStats.Target.Stats);

                    if (recipeModifiers != null && recipeModifiers.Length > 0)
                        for (int i = 0; i < recipeModifiers.Length; i++)
                        {
                            if (recipeModifiers[i].StatsModifiers != null)
                            {
                                ItemPropsBaseViewModel.GatherShownStats(
                                    ref summaryGeneralStats,
                                    recipeModifiers[i]
                                        .StatsModifiers
                                        .Where(v => v.StatType == StatType.General)
                                        .Select(v => new StatModifier(v.StatResource, v.InitialValue))
                                        .ToArray());
                                ItemPropsBaseViewModel.GatherShownStats(
                                    ref summarySpecificStats,
                                    recipeModifiers[i]
                                        .StatsModifiers
                                        .Where(v => v.StatType == StatType.Specific)
                                        .Select(v => new StatModifier(v.StatResource, v.InitialValue))
                                        .ToArray());
                            }
                        }
                }
            }

            var lst = new List<KeyValuePair<StatResource, float>>();
            foreach (var kvp in summaryGeneralStats)
                lst.Add(kvp);
            foreach (var kvp in summarySpecificStats)
                lst.Add(kvp);

            return lst.Where(kvp => !Mathf.Approximately(kvp.Value, 0)).ToList();
        }


        //=== Private =========================================================

        private LocalizedString GetContextTitle(RecipeBaseViewModel recipeBaseViewModel)
        {
            if (!recipeBaseViewModel.HasWorkbenchTypeDefs)
                return _contextViewTitlesDefRef.Target.Handcraft.Ls;

            return recipeBaseViewModel.RecipeWorkbenchTypeName;
        }

        private Sprite GetContextIcon(RecipeBaseViewModel recipeBaseViewModel)
        {
            if (!recipeBaseViewModel.HasWorkbenchTypeDefs)
                return _contextViewTitlesDefRef.Target.Handcraft.Sprite?.Target;

            return recipeBaseViewModel.RecipeWorkbenchTypeIcon;
        }

        private void OnInventoryWindowStateChanged(GuiWindowState guiWindowState, InventoryNode.WindowMode mode)
        {
            if (guiWindowState == GuiWindowState.Closed)
                ChangeCount(1, false);
        }

        private bool GetIsCraftAvailableByExtraCond()
        {
            if (CurrentRecipeVm == null || CurrentRecipeVm.CraftRecipeDef == null)
                return false;

            return GetIsAvailableByWorkbenchType(CurrentRecipeVm);
        }

        private void OnCurrentRecipeVmChanged(RecipeBaseViewModel recipeVm)
        {
            if (recipeVm == null)
            {
                RecipeModifiers = null;
                RecipeModifiersIndices = null;
                RecipeModifiersIndicesOptional = null;
                _craftEngineOuterRef = OuterRef<IEntityObject>.Invalid;
                return;
            }

            //параметры, не меняющиеся от выбора ингридиентов
            BlueprintIcon = recipeVm.RecipeBlueprintIcon;
            FiltrableTypeIcon = recipeVm.FiltrableTypeIcon;
            _craftEngineOuterRef = recipeVm.HasCraftEngineParams.CraftEngineOuterRef;

            //параметры, меняющиеся от выбора ингридиентов (влияет номер варианта крафта), там же вызывается изменение количества
            Count = 1;
            IsCraftAvailableByIngred = FindFirstAvailableCraftVariant();
            ChangeVariantIndex(true);
            //блок ингридиентов
            ShowPathItem();
            HideAlternativeItems(true);
        }

        private void SwitchVisibility(bool isVisible)
        {
            _visualGameObject.SetActive(isVisible);
        }

        private void OnContextViewTargetChanged(IContextViewTarget target)
        {
            CurrentRecipeVm = target as RecipeBaseViewModel;
            SwitchVisibility(CurrentRecipeVm != null);
            if (CurrentRecipeVm != null)
            {
                IsCraftAvailableByExtraCond = GetIsCraftAvailableByExtraCond();
            }
        }

        private void ChangeVariantIndex(bool isFirstTime) //при смене ингридиентов может поменяться result item, меняем информацию
        {
            int newVariantIndex = GetVariantIndex(CurrentRecipeVm.CraftRecipeDef, RecipeModifiers);
            if (isFirstTime || newVariantIndex != VariantIndex)
            {
                VariantIndex = newVariantIndex;
                ProductVariant = GetProductVariant(VariantIndex);
            }

            var regularStats = GetRecipeRegularStats(ProductVariant.Product.Item.Target, RecipeModifiers);
            var mainStats = SlotPropsBaseViewModel.GetMainStats(ProductVariant.Product.Item.Target, regularStats);
            _statsViewModel.SetItemStats(mainStats, regularStats);
            ChangeCount(Count, isFirstTime);
            CraftAvailabilityRequest();
        }

        private int GetVariantIndex(CraftRecipeDef recipe, CraftRecipeModifier[] recipeModifiers)
        {
            for (int variantIndex = 0; variantIndex < recipe.Variants.Length; variantIndex++)
            {
                var variant = recipe.Variants[variantIndex];
                if (variant.MandatorySlots.Length == recipeModifiers.Length)
                {
                    for (int i = 0; i < recipeModifiers.Length; i++)
                    {
                        if (variant.MandatorySlots[i].Item != recipeModifiers[i].Item.Item &&
                            variant.MandatorySlots[i].Item.Target != _missingItemDefRef.Target)
                            break;

                        if (i == recipeModifiers.Length - 1) //полное сходство - это тот самый вариант
                            return variantIndex;
                    }
                }
                else
                {
                    UI.Logger.Warn(
                        $"ERROR IN JSON CRAFT: {variant.Product.Item}, " +
                        $"{variant.MandatorySlots.Length} != {recipeModifiers.Length}");
                }
            }

            UI.Logger.Warn(
                $"ERROR FIND CRAFT VARIANTS IN: {recipe}. " +
                $"Items: {string.Join(" ", recipeModifiers.Select(v => v.Item.Item.Target))}");
            return 0;
        }

        private ViewVariant GetProductVariant(int variantIndex)
        {
            return CurrentRecipeVm == null ||
                   variantIndex < 0 ||
                   variantIndex >= CurrentRecipeVm.CraftRecipeDef.Variants.Length
                ? default(ViewVariant)
                : CurrentRecipeVm.CraftRecipeDef.Variants[variantIndex];
        }

        private void OnChangeProductVariant()
        {
            var itemResource = ProductVariant.Product.Item.Target;
            ProductName = itemResource?.ItemNameLs ?? LsExtensions.Empty;
            ProductWeight = itemResource?.Weight ?? 0;
            ProductCraftTime = GetProductCraftTime();
            ProductsCount = ProductVariant.Product.Count;

            //_timeText.text = LocalizationHolder.GetTime(Count * CurrentRecipeVm.CraftRecipeDef.Variants[VariantIndex].CraftingTime);
            BigIcon = itemResource?.BigIcon.Target;
        }

        private float GetProductCraftTime()
        {
            return Count * ProductVariant.CraftingTime;
        }

        //изменение количества: ввод через input field
        private void ChangeCount(string value)
        {
            int parseValue;
            if (int.TryParse(value, out parseValue))
            {
                ChangeCount(parseValue, false);
                CraftAvailabilityRequest();
            }
            else
            {
                _countField.text = Count.ToString();
            }
        }

        [UsedImplicitly]
        public void OnCountFieldSelected()
        {
            if (_inputBindingsWhenCountFieldSelected != null && _inputBindingsWhenCountFieldSelected.Target != null)
                InputManager.Instance.PushBindings(_countField, _inputBindingsWhenCountFieldSelected.Target);
        }

        [UsedImplicitly]
        public void OnCountFieldDeselected()
        {
            if (_inputBindingsWhenCountFieldSelected != null && _inputBindingsWhenCountFieldSelected.Target != null)
                InputManager.Instance.PopBindings(_countField);
        }

        //изменение количества, отображение связанных параметров
        private void ChangeCount(int value, bool isFirstTime)
        {
            if (!isFirstTime && value == Count)
                return;

            Count = Mathf.Max(value, 1);
            _countField.text = Count.ToString();
            IsMinusButtonActive = Count > 1;
            IsMinButtonActive = Count != 1;
            ProductCraftTime = GetProductCraftTime();
        }

        //изменилась доступность рецепта при изменении ингридиентов
        private void OnCharacterItemsChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            SlotsChanged?.Invoke();
            CraftAvailabilityRequest();
        }

        private void CraftAvailabilityRequest()
        {
            if (CurrentRecipeVm == null)
            {
                IsCraftAvailableByIngred = false;
                return;
            }

            AsyncUtils.RunAsyncTask(
                () =>
                    CraftEngineCommands.IsCraftAvailableAsync(
                        _craftEngineOuterRef,
                        _inventoryPropertyAddress,
                        _dollPropertyAddress,
                        CurrentRecipeVm.CraftRecipeDef,
                        VariantIndex,
                        RecipeModifiersIndices,
                        RecipeModifiersIndicesOptional,
                        Count,
                        _missingItemDefRef.Target,
                        isSuccess => { IsCraftAvailableByIngred = isSuccess; })
            );
        }

        private void CraftAvailableMaxCountRequest()
        {
            _maxCountButtonLastPressTime = DateTime.Now;
            if (CurrentRecipeVm == null)
            {
                IsCraftAvailableByIngred = false;
                return;
            }

            AsyncUtils.RunAsyncTask(
                () =>
                    CraftEngineCommands.CraftAvailableMaxCountAsync(
                        _craftEngineOuterRef,
                        _inventoryPropertyAddress,
                        _dollPropertyAddress,
                        CurrentRecipeVm.CraftRecipeDef,
                        VariantIndex,
                        RecipeModifiersIndices,
                        RecipeModifiersIndicesOptional,
                        _missingItemDefRef.Target,
                        maxCount =>
                        {
                            IsCraftAvailableByIngred = maxCount > 0;
                            ChangeCount(Mathf.Max(maxCount, 1), false);
                        })
            );
        }

        private void ShowPathItem()
        {
            //добавляем из префаба ингридиенты
            if (!_wasAddCraftRecipeElement)
            {
                _wasAddCraftRecipeElement = true;

                _recipeBaseElements = new CraftRecipeElementUI[MaxRecipeBaseElements];
                for (int i = 0; i < MaxRecipeBaseElements; i++)
                {
                    GameObject go = Instantiate(_craftRecipeBaseElementPrefab, _parentCraftRecipeBaseElements);
                    go.transform.localScale = Vector3.one;
                    _recipeBaseElements[i] = go.GetComponent<CraftRecipeElementUI>();
                    _recipeBaseElements[i].index = i;
                    _recipeBaseElements[i].Init(CharacterItemsNotifier, this);
                    _recipeBaseElements[i].ClickEvent += SwitchAlternativeItems;
                }

                // _recipeExtraElements = new CraftRecipeElementUI[MaxRecipeExtraElements];
                // for (int i = 0; i < MaxRecipeExtraElements; i++)
                // {
                //     GameObject go = Instantiate(_craftRecipeExtraElementPrefab, _parentCraftRecipeExtraElements);
                //     go.transform.localScale = Vector3.one;
                //     _recipeExtraElements[i] = go.GetComponent<CraftRecipeElementUI>();
                //     _recipeExtraElements[i].index = i;
                //     _recipeExtraElements[i].Init(CharacterItemsNotifier, this);
                //     _recipeExtraElements[i].ClickEvent += SwitchAlternativeItems;
                // }

                _recipeChangeElements = new CraftRecipeElementUI[MaxRecipeChangeElements];
                for (int i = 0; i < MaxRecipeChangeElements; i++)
                {
                    GameObject go = Instantiate(_craftRecipeChangeElementPrefab, _parentCraftRecipeChangeElements);
                    go.transform.localScale = Vector3.one;
                    _recipeChangeElements[i] = go.GetComponent<CraftRecipeElementUI>();
                    _recipeChangeElements[i].Init(CharacterItemsNotifier, this);
                    _recipeChangeElements[i].ClickEvent += ClickChangeElement;
                }
            }

            //заполняем базовые слоты
            for (int i = 0; i < Mathf.Min(RecipeModifiers.Length, _recipeBaseElements.Length); i++)
            {
                _recipeBaseElements[i]
                    .Show(
                        RecipeModifiers[i],
                        RecipeModifiersIndices[i],
                        CurrentRecipeVm.CraftRecipeDef.MandatorySlots[i].Items.Length > 1);
            }

            if (RecipeModifiers.Length < _recipeBaseElements.Length) //в рецепте используются не все базовые слоты
            {
                for (int i = RecipeModifiers.Length; i < _recipeBaseElements.Length; i++)
                {
                    _recipeBaseElements[i].Hide();
                }
            }

            // //заполняем дополнительные слоты
            // for (int i = 0; i < Mathf.Min(CurrentRecipeVm.CraftRecipeDef.OptionalSlots.Length, _recipeExtraElements.Length); i++)
            // {
            //     _recipeExtraElements[i].ShowEmpty();
            // }
            //
            // if (CurrentRecipeVm.CraftRecipeDef.OptionalSlots.Length < _recipeExtraElements.Length)
            // {
            //     for (int i = CurrentRecipeVm.CraftRecipeDef.OptionalSlots.Length; i < _recipeExtraElements.Length; i++)
            //     {
            //         _recipeExtraElements[i].Hide();
            //     }
            // }
        }

        private void SwitchAlternativeItems(CraftRecipeElementUI element)
        {
            if (_openedCraftRecipeElementUI == element)
                HideAlternativeItems();
            else
                ShowAlternativeItems(element);
        }

        private void ShowAlternativeItems(CraftRecipeElementUI element)
        {
            if (_openedCraftRecipeElementUI == element)
                return;

            _openedCraftRecipeElementUI = element;
            for (int i = 0; i < _recipeBaseElements.Length; i++)
            {
                _recipeBaseElements[i].SetActive(element);
            }

            // for (int i = 0; i < _recipeExtraElements.Length; i++)
            // {
            //     _recipeExtraElements[i].SetActive(element);
            // }

            //показываем варианты замены для этого слота
            _craftRecipeChangePanel.SetActive(true);
            for (int i = 0; i < _recipeChangeElements.Length; i++)
            {
                _recipeChangeElements[i].Hide();
            }

            int countAlternative = 0;
            if (element.type == CraftRecipeElementUI.Type.BaseType)
            {
                for (int i = 0; i < CurrentRecipeVm.CraftRecipeDef.MandatorySlots[element.index].Items.Length; i++)
                {
                    if (RecipeModifiersIndices[element.index] != i)
                    {
                        _recipeChangeElements[countAlternative].type = CraftRecipeElementUI.Type.BaseType;
                        _recipeChangeElements[countAlternative].index = element.index;
                        _recipeChangeElements[countAlternative]
                            .Show(CurrentRecipeVm.CraftRecipeDef.MandatorySlots[element.index].Items[i], i, true);
                        countAlternative++;
                    }
                }
            }
            else if (element.type == CraftRecipeElementUI.Type.Extra)
            {
                if (element.indexItem >= 0)
                {
                    _recipeChangeElements[countAlternative].type = CraftRecipeElementUI.Type.Extra;
                    _recipeChangeElements[countAlternative].index = element.index;
                    _recipeChangeElements[countAlternative].ShowEmpty();
                    countAlternative++;
                }

                for (int i = 0; i < CurrentRecipeVm.CraftRecipeDef.OptionalSlots[element.index].Items.Length; i++)
                {
                    if (element.indexItem >= 0 && element.indexItem != i || element.indexItem < 0)
                    {
                        _recipeChangeElements[countAlternative].type = CraftRecipeElementUI.Type.Extra;
                        _recipeChangeElements[countAlternative].index = element.index;
                        _recipeChangeElements[countAlternative]
                            .Show(CurrentRecipeVm.CraftRecipeDef.OptionalSlots[element.index].Items[i], i, true);
                        countAlternative++;
                    }
                }
            }

            //ставим положение _parentCraftRecipeChangeElements
            var parentRect = _parentCraftRecipeChangeElements.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
            var targetPositionX = element.gameObject.transform.position.x;
            var currentPos = parentRect.position;
            parentRect.position = new Vector3(targetPositionX, currentPos.y, currentPos.z);
            var currentPivot = parentRect.pivot;
            var pivotX = Mathf.Ceil(countAlternative / 2f) > element.index ? (element.index + 0.5f) / countAlternative : 0.5f;
            currentPivot.x = pivotX;
            parentRect.pivot = currentPivot;

            var temp = _recipeChangeRightOutline.anchorMin;
            temp.x = pivotX;
            _recipeChangeRightOutline.anchorMin = temp;

            temp = _recipeChangeLeftOutline.anchorMax;
            temp.x = pivotX;
            _recipeChangeLeftOutline.anchorMax = temp;

            var centerTransform = _recipeChangeCentrObj.transform;
            var position = centerTransform.position;
            position.x = targetPositionX;
            centerTransform.position = position;
        }

        private void ClickChangeElement(CraftRecipeElementUI element) //выбран альтернативный элемент
        {
            if (element.type == CraftRecipeElementUI.Type.BaseType)
            {
                RecipeModifiers[element.index] = element.CraftRecipeModifier;
                RecipeModifiersIndices[element.index] = element.indexItem;
                _recipeBaseElements[element.index].Show(element.CraftRecipeModifier, element.indexItem, true);
            }
            else if (element.type == CraftRecipeElementUI.Type.Extra)
            {
                RecipeModifiersIndicesOptional[element.index] = element.indexItem;

                // if (element.indexItem < 0)
                // {
                //     _recipeExtraElements[element.index].ShowEmpty();
                // }
                // else
                // {
                //     _recipeExtraElements[element.index].Show(element.CraftRecipeModifier, element.indexItem, true);
                // }
            }

            ChangeVariantIndex(false); //возможно поменялся результат крафта: название, иконка и тд
            HideAlternativeItems();
        }
    }
}