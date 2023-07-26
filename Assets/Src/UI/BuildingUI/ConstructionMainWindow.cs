using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Src.BuildingSystem;
using Assets.Src.ContainerApis;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using SharedCode.Aspects.Building;
using Src.Aspects.Impl;
using Uins.Inventory;
using UnityWeld.Binding;
using Uins.Sound;

namespace Uins
{
    [Binding]
    public class ConstructionMainWindow : DependencyEndNode, IGuiWindow
    {
        public const int ButtonsRowCount = 3;
        public const float DelayBetweenAttempts = 1;

        [SerializeField, UsedImplicitly]
        private BuildingPlaceDefRef _buildingPlaceDefRef;

        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private MainConstructionButton _buttonPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _buttonsRoot;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _openWindowHotkey;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _closeConstructionInterfaceHotkey;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _removeObjectHotkey;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _repositionObjectHotkey;

        [SerializeField, UsedImplicitly]
        private HotkeyListener[] _buttonsHotkeys;

        [Header("Actions hotkeys")]
        public ExtentedHotkeyListener PlaceAnchorHotkey;

        public HotkeyListener PlaceObjectHotkey;

        public HotkeyListener RotateIncrHotkey;

        public HotkeyListener RotateDecrHotkey;

        public HotkeyListener ZoomIncrHotkey;

        public HotkeyListener ZoomDecrHotkey;

        public HotkeyListener MoveIncrHotkey;

        public HotkeyListener MoveDecrHotkey;

        [SerializeField, UsedImplicitly]
        private ConstructionSelectionWindow _selectionWindow;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _checkRangeUpdateInterval;

        private List<MainConstructionButton> _buttons = new List<MainConstructionButton>();

        private KeyValuePair<HotkeyListener, Action>[] _openUpdateHotkeysWithActions;
        private bool _isInited;
        private float _lastPlaceAnchorAttemptTime;
        private Dictionary<BuildRecipeDef, bool> _constructionRecipes = new Dictionary<BuildRecipeDef, bool>();

        private HasFactionFullApi _hasFactionFullApi;
        private RecipesSource _recipesSource;


        //=== Props ===========================================================

        [ColonyDI.Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        [ColonyDI.Dependency]
        private InventoryNode InventoryNode { get; set; }

        public ICharacterBuildInterface CharacterBuilderInterface { get; private set; }

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => InputBindingsRef.Target;

        private ResourceRef<InputBindingsDef> InputBindingsRef = new ResourceRef<InputBindingsDef>("/UtilPrefabs/Input/Bindings/UIConstructionMainWindow");

        public HotkeyListener[] ButtonsHotkeys => _buttonsHotkeys;

        private bool _isVisible;

        [Binding]
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasSelectedConstructionElement;

        [Binding]
        public bool HasSelectedConstructionElement
        {
            get { return _hasSelectedConstructionElement; }
            set
            {
                if (_hasSelectedConstructionElement != value)
                {
                    _hasSelectedConstructionElement = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isBuildingPlaceActive;

        [Binding]
        public bool IsBuildingPlaceActive
        {
            get { return _isBuildingPlaceActive; }
            set
            {
                if (_isBuildingPlaceActive != value)
                {
                    _isBuildingPlaceActive = value;
                    NotifyPropertyChanged();
                    CanBuildHere = GetCanBuildHere();
                }
            }
        }

        private bool _isBuildingPlaceInRange;

        [Binding]
        public bool IsBuildingPlaceInRange
        {
            get { return _isBuildingPlaceInRange; }
            set
            {
                if (_isBuildingPlaceInRange != value)
                {
                    _isBuildingPlaceInRange = value;
                    NotifyPropertyChanged();
                    CanBuildHere = GetCanBuildHere();
                }
            }
        }

        private bool _canBuildHere;

        [Binding]
        public bool CanBuildHere
        {
            get { return _canBuildHere; }
            set
            {
                if (_canBuildHere != value)
                {
                    _canBuildHere = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _hasBuildRecipes;

        [Binding]
        public bool HasBuildRecipes
        {
            get { return _hasBuildRecipes; }
            set
            {
                if (_hasBuildRecipes != value)
                {
                    _hasBuildRecipes = value;
                    NotifyPropertyChanged();
                    CanBuildHere = GetCanBuildHere();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_windowId.AssertIfNull(nameof(_windowId)) ||
                _buildingPlaceDefRef.Target.AssertIfNull(nameof(_buildingPlaceDefRef)) ||
                _openWindowHotkey.AssertIfNull(nameof(_openWindowHotkey)) ||
                _removeObjectHotkey.AssertIfNull(nameof(_removeObjectHotkey)) ||
                _repositionObjectHotkey.AssertIfNull(nameof(_repositionObjectHotkey)) ||
                _closeConstructionInterfaceHotkey.AssertIfNull(nameof(_closeConstructionInterfaceHotkey)) ||
                _selectionWindow.AssertIfNull(nameof(_selectionWindow)) ||
                _buttonPrefab.AssertIfNull(nameof(_buttonPrefab)) ||
                _buttonsRoot.AssertIfNull(nameof(_buttonsRoot)) ||
                _buttonsHotkeys.IsNullOrEmptyOrHasNullElements(nameof(_buttonsHotkeys)) ||
                PlaceAnchorHotkey.AssertIfNull(nameof(PlaceAnchorHotkey)) ||
                PlaceObjectHotkey.AssertIfNull(nameof(PlaceObjectHotkey)) ||
                RotateIncrHotkey.AssertIfNull(nameof(RotateIncrHotkey)) ||
                RotateDecrHotkey.AssertIfNull(nameof(RotateDecrHotkey)) ||
                ZoomIncrHotkey.AssertIfNull(nameof(ZoomIncrHotkey)) ||
                ZoomDecrHotkey.AssertIfNull(nameof(ZoomDecrHotkey)) ||
                MoveIncrHotkey.AssertIfNull(nameof(MoveIncrHotkey)) ||
                MoveDecrHotkey.AssertIfNull(nameof(MoveDecrHotkey)))
                return;
            State.Value = GuiWindowState.Closed;

            _openUpdateHotkeysWithActions = new[]
            {
                //new KeyValuePair<HotkeyListener, Action>(_repositionObjectHotkey, OnRepositionObjectHotkey),
                new KeyValuePair<HotkeyListener, Action>(_removeObjectHotkey, OnRemoveObjectHotkey),
            };

            _selectionWindow.Init(this);
            _isInited = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
            if (_recipesSource != null)
            {
                _recipesSource.KnownRecipeAdd -= OnKnownRecipeAdd;
                _recipesSource.KnownRecipeRemove -= OnKnownRecipeRemove;
                _recipesSource.KnownRecipeStateChanged -= OnKnownRecipeStateChanged;
            }
        }


        //=== Public ==========================================================

        public void OnOpen(object arg)
        {
            if (CharacterBuilderInterface.AssertIfNull(nameof(CharacterBuilderInterface)))
                WindowsManager.Close(this);

            IsVisible = true;
            CharacterBuilderInterface?.Activate(true);
            IsBuildingPlaceActive = GetIsBuildingPlaceActive();
            IsBuildingPlaceInRange = GetIsBuildingPlaceInRange();
        }

        public void OnClose()
        {
            IsVisible = false;
            CharacterBuilderInterface?.Activate(false);
        }

        public void OnFade()
        {
            IsVisible = false;
            CharacterBuilderInterface?.Activate(false);
        }

        public void OnUnfade()
        {
            IsVisible = true;
            CharacterBuilderInterface?.Activate(true);
        }

        public void OpenUpdate()
        {
            if (!HasBuildRecipes)
                return;

            if (IsBuildingPlaceActive)
            {
                if (IsBuildingPlaceInRange)
                {
                    //анкор в зоне доступа, можно строить
                    HasSelectedConstructionElement = CharacterBuilderInterface?.HasSelectedElement ?? false;

                    for (int i = 0, len = _buttons.Count; i < len; i++)
                    {
                        var button = _buttons[i];
                        if (button.IsInteractable && button.Hotkey.IsFired())
                        {
                            SelectGroup(button.Group);
                            break;
                        }
                    }

                    for (int i = 0, len = _openUpdateHotkeysWithActions.Length; i < len; i++)
                    {
                        if (_openUpdateHotkeysWithActions[i].Key.IsFired())
                        {
                            _openUpdateHotkeysWithActions[i].Value.Invoke();
                            return;
                        }
                    }
                }

                //анкор стоит, но далеко - нельзя строить
            }
            else
            {
                //анкор еще не поставлен
                if (Time.time - _lastPlaceAnchorAttemptTime > DelayBetweenAttempts)
                {
                    PlaceAnchorHotkey.Update();
                    if (PlaceAnchorHotkey.Hold)
                    {
                        _lastPlaceAnchorAttemptTime = Time.time;
                    }
                }
            }
        }

        public void NoClosedUpdate()
        {
            if (_closeConstructionInterfaceHotkey.IsFired())
            {
                WindowsManager.Close(this);
                return;
            }

            if (_checkRangeUpdateInterval.IsItTime())
            {
                IsBuildingPlaceActive = GetIsBuildingPlaceActive();
                IsBuildingPlaceInRange = GetIsBuildingPlaceInRange(); //не вышли ли из зоны строительства или наоборот?
            }
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
            if (State.Value == GuiWindowState.Closed && _openWindowHotkey.IsFired())
                WindowsManager.Open(this);
        }

        public override void AfterDependenciesInjected()
        {
            if (!_isInited)
                return;

            if (CreateButtons())
                WindowsManager.RegisterWindow(this);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            if (!_isInited)
                return;

            SurvivalGui.PawnChangesStream.Action(D, OnOurPawnChanged);
            _recipesSource = InventoryNode.RecipesSource;
            if (!_recipesSource.AssertIfNull(nameof(_recipesSource)))
            {
                _recipesSource.KnownRecipeAdd += OnKnownRecipeAdd;
                _recipesSource.KnownRecipeRemove += OnKnownRecipeRemove;
                _recipesSource.KnownRecipeStateChanged += OnKnownRecipeStateChanged;
            }
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _constructionRecipes.Clear();
                HasBuildRecipes = false;

                if (State.Value != GuiWindowState.Closed)
                    WindowsManager.Close(this);
                {
                    CharacterBuilderInterface?.Activate(false);
                    CharacterBuilderInterface = null;
                }
            }

            if (newEgo != null)
            {
                var characterBuilderBehaviour = newEgo.GetComponent<ICharacterPawn>()?.BuildInterface;
                if (!characterBuilderBehaviour.AssertIfNull(nameof(characterBuilderBehaviour)))
                    CharacterBuilderInterface = characterBuilderBehaviour;
            }
        }

        private void OnKnownRecipeAdd(BaseRecipeDef baseRecipeDef, RecipeState recipeState, bool isFirstTime)
        {
            var buildRecipeDef = baseRecipeDef as BuildRecipeDef;
            if (buildRecipeDef == null)
                return;

            if (_constructionRecipes.ContainsKey(buildRecipeDef))
                UI.Logger.IfError()?.Message($"{nameof(_constructionRecipes)} already contains {buildRecipeDef}").Write();

            _constructionRecipes[buildRecipeDef] = recipeState.IsAvailable;
            UpdateGroups();
        }

        private void OnKnownRecipeRemove(BaseRecipeDef baseRecipeDef)
        {
            var buildRecipeDef = baseRecipeDef as BuildRecipeDef;
            if (buildRecipeDef == null)
                return;

            if (!_constructionRecipes.ContainsKey(buildRecipeDef))
            {
                UI.Logger.IfError()?.Message($"{nameof(_constructionRecipes)} don't contains {buildRecipeDef}").Write();
                return;
            }

            _constructionRecipes.Remove(buildRecipeDef);
            UpdateGroups();
        }

        private void OnKnownRecipeStateChanged(BaseRecipeDef baseRecipeDef, RecipeState recipeState)
        {
            var buildRecipeDef = baseRecipeDef as BuildRecipeDef;
            if (buildRecipeDef == null)
                return;

            if (!_constructionRecipes.ContainsKey(buildRecipeDef))
            {
                UI.Logger.IfError()?.Message($"{nameof(_constructionRecipes)} don't contains {buildRecipeDef}").Write();
                return;
            }

            _constructionRecipes[buildRecipeDef] = recipeState.IsAvailable;
            UpdateGroups();
        }

        private void UpdateGroups()
        {
            var groups = GetBuildRecipesGroups(_constructionRecipes);
            HasBuildRecipes = GetHasBuildRecipes(groups);
            InitButtons(groups);
        }

        private bool GetHasBuildRecipes(IOrderedEnumerable<IGrouping<BuildRecipeGroupDef, BuildRecipeDef>> groups)
        {
            return groups != null && groups.Any();
        }

        private bool CreateButtons()
        {
            for (int i = 0; i < _buttonsHotkeys.Length; i++)
            {
                var button = Instantiate(_buttonPrefab, _buttonsRoot);
                button.name = $"{_buttonPrefab.name}{i + 1}";
                button.Hotkey = _buttonsHotkeys[i];
                _buttons.Add(button);
            }

            var remainder = _buttons.Count % ButtonsRowCount;
            if (remainder == 0)
                return true;

            for (int i = 0, len = ButtonsRowCount - remainder; i < len; i++)
            {
                var index = _buttons.Count + 1;
                var button = Instantiate(_buttonPrefab, _buttonsRoot);
                button.name = $"{_buttonPrefab.name}{index + 1}_empty";
                _buttons.Add(button);
            }

            return true;
        }

        private IOrderedEnumerable<IGrouping<BuildRecipeGroupDef, BuildRecipeDef>> GetBuildRecipesGroups(Dictionary<BuildRecipeDef, bool> constructionRecipes)
        {
            return constructionRecipes.Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .Where(r => r.BuildRecipeGroupDef.Target != null)
                .GroupBy(r => r.BuildRecipeGroupDef.Target).OrderBy(g => g.Key.OrderIndex);
        }

        private void InitButtons(IOrderedEnumerable<IGrouping<BuildRecipeGroupDef, BuildRecipeDef>> groups)
        {
            int i = 0;
            foreach (var group in groups)
            {
                var idx = i++;
                if (idx >= _buttons.Count)
                {
                    UI.Logger.IfError()?.Message($"BuildRecipe groups.Count ({idx}) >= {nameof(_buttons)}.Count ({_buttons.Count})").Write();
                    continue;
                }

                var button = _buttons[idx];
                button.Init(group);
            }

            if (i >= _buttons.Count)
                return;

            for (int idx = i; i < _buttons.Count; i++)
            {
                _buttons[idx].Init(null);
            }
        }

        private void SelectGroup(IGrouping<BuildRecipeGroupDef, BuildRecipeDef> group)
        {
            WindowsManager.Open(_selectionWindow, null, group);
        }

        private void OnRemoveObjectHotkey()
        {
            if (!HasSelectedConstructionElement)
                return;

            var result = CharacterBuilderInterface?.RemoveElement() ?? false;
            if (result)
            {
                SoundControl.Instance?.CraftBuildingBreakdownEvent?.Post(transform.root.gameObject);
            }
            else
            {
                SoundControl.Instance?.CraftBuildingPlacementDeniedEvent?.Post(transform.root.gameObject);
            }
        }

        private bool GetIsBuildingPlaceActive()
        {
            return CharacterBuilderInterface?.IsBuildingPlaceActive ?? false;
        }

        private bool GetIsBuildingPlaceInRange()
        {
            return CharacterBuilderInterface?.IsBuildingPlaceInRange ?? false;
        }

        private bool GetCanBuildHere()
        {
            return IsBuildingPlaceActive && IsBuildingPlaceInRange && HasBuildRecipes;
        }
    }
}