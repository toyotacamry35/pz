using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class CraftSideViewModel : BindingViewModel, IRecipeSortingResolver, IHasCraftEngineParams
    {
        [SerializeField, UsedImplicitly]
        private AvailableRecipeViewModel _recipePrefab;

        [SerializeField, UsedImplicitly]
        private Transform _recipesTransform;

        [SerializeField, UsedImplicitly, FormerlySerializedAs("_contextView")]
        private ContextViewWithParams _contextViewWithParams;

        [SerializeField, UsedImplicitly]
        private CraftRecipeContextView _craftRecipeContextView;

        [SerializeField, UsedImplicitly]
        private ScrollRect _knownRecipesScrollRect;

        [SerializeField, UsedImplicitly]
        private RecipesFilterViewModel _recipesFilterViewModel;

        private RecipesSource _recipesSource;
        private CraftRecipeDef _lastSelectedCraftRecipeDef;
        private List<RecipeBaseViewModel> _recipesWithChangedAvailability = new List<RecipeBaseViewModel>();


        //=== Props ===========================================================

        public virtual bool IsMachineTab => false;

        public OuterRef CraftEngineOuterRef { get; set; }

        public List<AvailableRecipeViewModel> KnownRecipeViewModels { get; } = new List<AvailableRecipeViewModel>();

        public WorkbenchTypeDef CurrentWorkbenchTypeDef { get; private set; }

        public RecipesFilterViewModel RecipesFilterViewModel => _recipesFilterViewModel;

        private bool _hasKnownRecipes;

        [Binding]
        public bool HasKnownRecipes
        {
            get => _hasKnownRecipes;
            set
            {
                if (_hasKnownRecipes != value)
                {
                    _hasKnownRecipes = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _recipePrefab.AssertIfNull(nameof(_recipePrefab));
            _recipesTransform.AssertIfNull(nameof(_recipesTransform));
            _contextViewWithParams.AssertIfNull(nameof(_contextViewWithParams));
            _knownRecipesScrollRect.AssertIfNull(nameof(_knownRecipesScrollRect));
            _recipesFilterViewModel.AssertIfNull(nameof(_recipesFilterViewModel));
            _craftRecipeContextView.AssertIfNull(nameof(_craftRecipeContextView));
        }

        private void Update()
        {
            if (_recipesWithChangedAvailability.Count == 0)
                return;

            foreach (var recipeBaseViewModel in _recipesWithChangedAvailability)
                recipeBaseViewModel.ApplyFiltering(CurrentWorkbenchTypeDef);
            _recipesWithChangedAvailability.Clear();
            if (KnownRecipeViewModels.Count < 2)
                return;

            SortRecipes(KnownRecipeViewModels, CurrentWorkbenchTypeDef);
            SelectedRecipeScrollCentering();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_recipesSource != null)
            {
                _recipesSource.KnownRecipeAdd -= OnKnownRecipeAdd;
                _recipesSource.KnownRecipeStateChanged -= OnKnownRecipeStateChanged;
                _recipesSource.KnownRecipeRemove -= OnKnownRecipeRemove;
            }

            if (_recipesFilterViewModel != null)
            {
                _recipesFilterViewModel.FiltrableTypeFilterChanged -= OnFiltrableTypeFilterChanged;
                _recipesFilterViewModel.PlaFilterChanged -= OnPlaFilterChanged;
            }
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource, RecipesSource recipesSource)
        {
            if (pawnSource.AssertIfNull(nameof(pawnSource)) ||
                recipesSource.AssertIfNull(nameof(recipesSource)))
                return;

            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);

            _recipesSource = recipesSource;
            _recipesSource.KnownRecipeAdd += OnKnownRecipeAdd;
            _recipesSource.KnownRecipeStateChanged += OnKnownRecipeStateChanged;
            _recipesSource.KnownRecipeRemove += OnKnownRecipeRemove;

            _recipesFilterViewModel.FiltrableTypeFilterChanged += OnFiltrableTypeFilterChanged;
            _recipesFilterViewModel.PlaFilterChanged += OnPlaFilterChanged;

            if (IsMachineTab)
                _craftRecipeContextView.WorkbenchTypeRp.Action(D, OnWorkbenchTypeChanged);
        }

        public void OnUnselectRecipe()
        {
            _contextViewWithParams.Vmodel.Value.SetContext(null);
        }

        //Всего в int 4*8=32 разряда
        //Первые 12 разрядов (0...4095) под сортировку самих рецептов (больший index - выше)
        private const int RecipeIndexDigitCount = 12;
        private const int MaxRecipeSubIndex = (1 << RecipeIndexDigitCount) - 1;

        //важней 10 разрядов под тип станка
        private const int WorkbenchIndexDigitCount = 10;
        private const int MaxWorkbenchSubIndex = (1 << WorkbenchIndexDigitCount) - 1;

        //важней 1 разряд под признак доступности рецепта (доступные - выше)
        private const int AvailabilityDigitCount = 1;
        private const int AvailabilityIndex = 1 << (RecipeIndexDigitCount + WorkbenchIndexDigitCount);

        //важней 1 разряд наличия станка
        private const int HasWorkbenchTypeDigitCount = 1;
        private const int HasWorkbenchTypeIndex = 1 << (RecipeIndexDigitCount + WorkbenchIndexDigitCount + AvailabilityDigitCount);

        public int GetSortingIndex(RepairRecipeDef recipeDef, bool isAvailableRecipe, WorkbenchTypeDef currentWorkbenchType)
        {
            //Это не ошибка, что сейчас currentWorkbenchType  не используется. Но из параметров выбрасывать не буду, 
            if (recipeDef.AssertIfNull(nameof(recipeDef)))
                return 0;

            var sortingIndex = MaxRecipeSubIndex - recipeDef.SortingIndex;

            var workbenchType = recipeDef.HasWorkbenchTypes ? recipeDef.WorkbenchTypes.First(wbt => wbt.Target != null).Target : null;
            var workbenchTypeIndex = workbenchType == null
                ? 0
                : (MaxWorkbenchSubIndex - workbenchType.SortingIndex) << RecipeIndexDigitCount;
            sortingIndex += workbenchTypeIndex;

            if (!isAvailableRecipe)
                sortingIndex += AvailabilityIndex;

            if (workbenchType != null)
                sortingIndex += HasWorkbenchTypeIndex;

            return sortingIndex;
        }

        public void SetAvailabilityChanged(RecipeBaseViewModel recipeBaseViewModel)
        {
            _recipesWithChangedAvailability.Add(recipeBaseViewModel);
        }

        public static void SortRecipes(List<AvailableRecipeViewModel> viewModels, WorkbenchTypeDef workbenchTypeDef = null)
        {
            ItemsSorting.SortSiblings(viewModels, arvm => arvm.GetSortingIndex(workbenchTypeDef));
        }

        public static T InsertRecipeViewModel<T>(
            int index,
            CraftRecipeDef craftRecipeDef,
            T prefab,
            Transform parentTransform,
            IList<T> viewModels,
            bool isMachineTab,
            IRecipeAvailabilityResolver recipeAvailabilityResolver,
            IHasContextStream hasContextStream,
            IHasCraftEngineParams hasCraftEngineParams,
            IRecipeSortingResolver recipeSortingResolver = null,
            IContextViewTargetResolver contextViewTargetResolver = null)
            where T : RecipeBaseViewModel
        {
            if (craftRecipeDef.AssertIfNull(nameof(craftRecipeDef)))
                return null;

            var newViewModel = Instantiate(prefab, parentTransform);
            newViewModel.name = $"{prefab.name}{viewModels.Count}";
            newViewModel.SetSortingResolver(recipeSortingResolver);
            newViewModel.transform.SetSiblingIndex(index);
            //DebugShowTransformIndex(newViewModel.transform, $"Insert: after SetSiblingIndex({index})"); //DEBUG
            newViewModel.Subscribes(
                recipeAvailabilityResolver,
                hasContextStream,
                isMachineTab ? InventoryTabType.Machine : InventoryTabType.Crafting,
                craftRecipeDef,
                contextViewTargetResolver,
                hasCraftEngineParams);
            viewModels.Insert(index, newViewModel);
            return newViewModel;
        }

        public static void RemoveItem<T>(IList<T> viewModels, T removingVm, ref CraftRecipeDef lastSelectedCraftRecipeDef)
            where T : RecipeBaseViewModel
        {
            if (removingVm.IsSelected)
                lastSelectedCraftRecipeDef = removingVm.CraftRecipeDef;
            removingVm.Unsubscribes();
            viewModels.Remove(removingVm);
            Destroy(removingVm.gameObject);
        }

        public static void RestoreRecipeSelection<T>(IList<T> recipes, ref CraftRecipeDef lastSelectedCraftRecipeDef)
            where T : RecipeBaseViewModel
        {
            if (lastSelectedCraftRecipeDef == null)
                return;

            foreach (var recipe in recipes)
            {
                if (recipe.IsVisible && recipe.CraftRecipeDef == lastSelectedCraftRecipeDef)
                {
                    recipe.IsSelected = true;
                    lastSelectedCraftRecipeDef = null;
                    return;
                }
            }
        }

        public static void ClearItems<T>(IList<T> viewModels, ref CraftRecipeDef lastSelectedCraftRecipeDef)
            where T : RecipeBaseViewModel
        {
            while (viewModels.Count > 0)
                RemoveItem(viewModels, viewModels[viewModels.Count - 1], ref lastSelectedCraftRecipeDef);
        }

        public void SelectedRecipeScrollCentering()
        {
            var selectedVm = KnownRecipeViewModels.FirstOrDefault(arvm => arvm.IsSelected);
            if (selectedVm == null || !selectedVm.gameObject.activeSelf)
                return;

            int selectedViewModelVisibleIndex = 0;
            int visibleViewModelsCount = 0;
            int i = 0;
            foreach (var tr in _recipesTransform)
            {
                var vmGo = (tr as Transform)?.gameObject;
                if (!vmGo.AssertIfNull(nameof(vmGo)) && vmGo.activeSelf)
                {
                    visibleViewModelsCount++;
                    if (vmGo.GetComponent<AvailableRecipeViewModel>() == selectedVm)
                        selectedViewModelVisibleIndex = i;
                }

                i++;
            }

            _knownRecipesScrollRect.verticalNormalizedPosition = 1 - ((float) selectedViewModelVisibleIndex / visibleViewModelsCount);
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
                ClearItems(KnownRecipeViewModels, ref _lastSelectedCraftRecipeDef);
        }

        private void OnWorkbenchTypeChanged(WorkbenchTypeDef workbenchTypeDef)
        {
            CurrentWorkbenchTypeDef = workbenchTypeDef;
            FilterRecipes();
            SortRecipes(KnownRecipeViewModels, CurrentWorkbenchTypeDef);
            SelectedRecipeScrollCentering();
            HasKnownRecipes = GetHasKnownRecipes();
        }

        /// <summary>
        /// Рецепт, подходящий для отображения панелью (т.е. заводящийся в ней)
        /// </summary>
        private bool IsSuitableRecipe(CraftRecipeDef craftRecipeDef)
        {
            return craftRecipeDef != null && (!IsMachineTab || craftRecipeDef.HasWorkbenchTypes);
        }

        private void OnKnownRecipeAdd(BaseRecipeDef baseRecipeDef, RecipeState recipeState, bool isFirstTime)
        {
            var craftRecipeDef = baseRecipeDef as CraftRecipeDef;
            if (!IsSuitableRecipe(craftRecipeDef))
                return;

            if (KnownRecipeViewModels.Any(vm => vm.CraftRecipeDef == craftRecipeDef))
            {
                UI.Logger.IfError()?.Message($"{nameof(craftRecipeDef)} already exists in {nameof(KnownRecipeViewModels)}: {craftRecipeDef}").Write();
                return;
            }

            var recipeViewModel = InsertRecipeViewModel(
                KnownRecipeViewModels.Count,
                craftRecipeDef,
                _recipePrefab,
                _recipesTransform,
                KnownRecipeViewModels,
                IsMachineTab,
                _craftRecipeContextView,
                _contextViewWithParams.Vmodel.Value,
                this,
                this,
                _craftRecipeContextView);
            recipeViewModel.ApplyFiltering(CurrentWorkbenchTypeDef);
            recipeViewModel.IsVisible = recipeState.IsAvailable;

            RestoreRecipeSelection(KnownRecipeViewModels, ref _lastSelectedCraftRecipeDef);
            SortRecipes(KnownRecipeViewModels, CurrentWorkbenchTypeDef);

            HasKnownRecipes = GetHasKnownRecipes();
        }

        private void OnKnownRecipeRemove(BaseRecipeDef baseRecipeDef)
        {
            var craftRecipeDef = baseRecipeDef as CraftRecipeDef;
            if (!IsSuitableRecipe(craftRecipeDef))
                return;

            var removedVm = KnownRecipeViewModels.FirstOrDefault(vm => vm.CraftRecipeDef == craftRecipeDef);
            if (removedVm == null)
            {
                UI.Logger.IfError()?.Message($"VM with {nameof(craftRecipeDef)} don't exists in {nameof(KnownRecipeViewModels)}: {craftRecipeDef}").Write();
                return;
            }

            RemoveItem(KnownRecipeViewModels, removedVm, ref _lastSelectedCraftRecipeDef);
            HasKnownRecipes = GetHasKnownRecipes();
        }

        private void OnKnownRecipeStateChanged(BaseRecipeDef baseRecipeDef, RecipeState recipeState)
        {
            var craftRecipeDef = baseRecipeDef as CraftRecipeDef;
            if (!IsSuitableRecipe(craftRecipeDef))
                return;

            var availableRecipeViewModel = KnownRecipeViewModels.FirstOrDefault(vm => vm.CraftRecipeDef == craftRecipeDef);
            if (availableRecipeViewModel == null)
            {
                UI.Logger.IfError()?.Message($"Not found vm for {baseRecipeDef}").Write();
                return;
            }

            availableRecipeViewModel.IsVisible = recipeState.IsAvailable;
            if (!availableRecipeViewModel.IsVisible)
                availableRecipeViewModel.IsSelected = false;
            HasKnownRecipes = GetHasKnownRecipes();
        }

        private void FilterRecipes()
        {
            foreach (var recipeVm in KnownRecipeViewModels)
                recipeVm.ApplyFiltering(CurrentWorkbenchTypeDef);
        }

        private void OnFiltrableTypeFilterChanged(InventoryFiltrableTypeDef inventoryFiltrableTypeDef)
        {
            FilterRecipes();
        }

        private void OnPlaFilterChanged(PlaFilter plaFilter)
        {
            FilterRecipes();
        }

        private bool GetHasKnownRecipes()
        {
            return CurrentWorkbenchTypeDef != null
                ? KnownRecipeViewModels.Any(vm => vm.IsBelongsToWorkbenchType(CurrentWorkbenchTypeDef) && vm.IsVisible)
                : KnownRecipeViewModels.Count > 0;
        }
    }
}