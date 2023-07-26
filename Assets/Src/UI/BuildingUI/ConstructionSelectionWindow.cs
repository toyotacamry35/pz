using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Src.BuildingSystem;
using Assets.Src.Inventory;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Building;
using Uins.Slots;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ConstructionSelectionWindow : DependencyEndNode, IGuiWindow
    {
        public event Action<SelectionConstructionButton> SelectedButtonChanged;

        public const string ColorTagEnd = "</color>";

        private const string TitlePropName = nameof(Title);
        private const string BigIconPropName = nameof(BigIcon);
        private const string DescriptionPropName = nameof(Description);
        private const string ProductMaxCountPropName = nameof(ProductMaxCount);

        private const int MinTier = 1;
        private const int MaxTier = 3;

        public static string EnoughColorTagBegin;
        public static string NotEnoughColorTagBegin;
        public static string NeutralColorTagBegin;

        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _closeWindowHotkey;

        [SerializeField, UsedImplicitly]
        private SelectionConstructionButton _buttonPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _buttonsRoot;

        [SerializeField, UsedImplicitly]
        private Color _enoughColor;

        [SerializeField, UsedImplicitly]
        private Color _notEnoughColor;

        [SerializeField, UsedImplicitly]
        private Color _neutralColor;

        [SerializeField, UsedImplicitly]
        private HotkeyListener _changeTierHotkey;

        private ConstructionMainWindow _mainWindow;
        private List<SelectionConstructionButton> _buttons = new List<SelectionConstructionButton>();
        private KeyValuePair<HotkeyListener, Action>[] _openUpdateHotkeysWithActions;

        private IGrouping<BuildRecipeGroupDef, BuildRecipeDef> _lastRecipesGroup;

        private bool _isInited;


        //=== Props ===========================================================

        [ColonyDI.Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public InputBindingsDef InputBindings => InputBindingsRef.Target;
        private ResourceRef<InputBindingsDef> InputBindingsRef => new ResourceRef<InputBindingsDef>("/UtilPrefabs/Input/Bindings/UIConstructionSelectionWindow");
 
        public ICharacterItemsNotifier CharacterItemsNotifier { get; set; }

        public ICharacterBuildInterface CharacterBuilderInterface => _mainWindow.CharacterBuilderInterface;

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

        private SelectionConstructionButton _selectedButton;

        public SelectionConstructionButton SelectedButton
        {
            get => _selectedButton;
            set
            {
                if (_selectedButton != value)
                {
                    var prevButton = _selectedButton;
                    _selectedButton = value;
                    OnSelectedButtonChanged(prevButton, _selectedButton);
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
                    var oldHasTitle = HasTitle;
                    _title = value;
                    NotifyPropertyChanged();
                    if (oldHasTitle != HasTitle)
                        NotifyPropertyChanged(nameof(HasTitle));
                }
            }
        }

        [Binding]
        public bool HasTitle => !Title.IsEmpty();

        private Sprite _bigIcon;

        [Binding]
        public Sprite BigIcon
        {
            get => _bigIcon;
            set
            {
                if (_bigIcon != value)
                {
                    _bigIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _description = "";

        [Binding]
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public bool IsEnoughIngreds => ProductMaxCount > 0;

        private int _productMaxCount;

        [Binding]
        public int ProductMaxCount
        {
            get => _productMaxCount;
            set
            {
                if (_productMaxCount != value)
                {
                    var oldIsEnoughIngreds = IsEnoughIngreds;
                    _productMaxCount = value;
                    NotifyPropertyChanged();
                    if (IsEnoughIngreds != oldIsEnoughIngreds)
                        NotifyPropertyChanged(nameof(IsEnoughIngreds));
                }
            }
        }

        private const string DisplayedConstructionsTierKey = "DisplayedConstructionsTier";

        [Binding]
        public int DisplayedConstructionsTier
        {
            get => UniquePlayerPrefs.GetInt(DisplayedConstructionsTierKey, 1);
            set
            {
                if (value != UniquePlayerPrefs.GetInt(DisplayedConstructionsTierKey, 1))
                {
                    UniquePlayerPrefs.SetInt(DisplayedConstructionsTierKey, value);
                    NotifyPropertyChanged();
                    if (_lastRecipesGroup != null)
                        SetupButtons(_lastRecipesGroup, SelectedButton?.ButtonIndex ?? -1);
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_windowId.AssertIfNull(nameof(_windowId)) ||
                _closeWindowHotkey.AssertIfNull(nameof(_closeWindowHotkey)) ||
                _buttonPrefab.AssertIfNull(nameof(_buttonPrefab)) ||
                _buttonsRoot.AssertIfNull(nameof(_buttonsRoot)) ||
                _changeTierHotkey.AssertIfNull(nameof(_changeTierHotkey)))
                return;

            State.Value = GuiWindowState.Closed;
            EnoughColorTagBegin = GetColorTagBegin(_enoughColor);
            NotEnoughColorTagBegin = GetColorTagBegin(_notEnoughColor);
            NeutralColorTagBegin = GetColorTagBegin(_neutralColor);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();
        }

        //=== Public ==========================================================

        public void Init(ConstructionMainWindow constructionMainWindow)
        {
            if (constructionMainWindow.AssertIfNull(nameof(constructionMainWindow)))
                return;

            _mainWindow = constructionMainWindow;
            _openUpdateHotkeysWithActions = new[]
            {
                new KeyValuePair<HotkeyListener, Action>(_changeTierHotkey, OnTierChange),
                new KeyValuePair<HotkeyListener, Action>(_mainWindow.PlaceObjectHotkey, OnActionPlace),
                new KeyValuePair<HotkeyListener, Action>(_mainWindow.MoveIncrHotkey, () => OnActionMove(true)),
                new KeyValuePair<HotkeyListener, Action>(_mainWindow.MoveDecrHotkey, () => OnActionMove(false)),
                new KeyValuePair<HotkeyListener, Action>(_mainWindow.ZoomIncrHotkey, () => OnActionZoom(true)),
                new KeyValuePair<HotkeyListener, Action>(_mainWindow.ZoomDecrHotkey, () => OnActionZoom(false)),
                new KeyValuePair<HotkeyListener, Action>(_mainWindow.RotateIncrHotkey, () => OnActionRotate(true)),
                new KeyValuePair<HotkeyListener, Action>(_mainWindow.RotateDecrHotkey, () => OnActionRotate(false)),
                new KeyValuePair<HotkeyListener, Action>(_closeWindowHotkey, () => WindowsManager.Close(this))
            };
            _isInited = true;
        }

        public void OnOpen(object args)
        {
            if (args.AssertIfNull(nameof(args)))
                return;

            var group = (IGrouping<BuildRecipeGroupDef, BuildRecipeDef>) args;
            if (group.AssertIfNull(nameof(group)))
                return;

            SetupButtons(group, 0);
            IsVisible = true;
        }

        public void OnClose()
        {
            IsVisible = false;
            TakeSelection(null);
        }

        public void OnFade()
        {
            IsVisible = false;
        }

        public void OnUnfade()
        {
            IsVisible = true;
        }

        public void OpenUpdate()
        {
            if (!_mainWindow.CanBuildHere)
            {
                WindowsManager.Close(this);
                return;
            }

            for (int i = 0, len = _buttons.Count; i < len; i++)
            {
                var button = _buttons[i];
                if (button.IsInteractable && button.IsHotkeyFired())
                    return;
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

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
        }

        public void TakeSelection(SelectionConstructionButton selectionConstructionButton)
        {
            if (selectionConstructionButton == SelectedButton)
                return;

            if (selectionConstructionButton != null && !selectionConstructionButton.IsInteractable)
            {
                UI.Logger.IfWarn()?.Message($"Unable to take selection for !IsInteractable button: {selectionConstructionButton}").Write();
                return;
            }

            SelectedButton = selectionConstructionButton;
            SelectedButtonChanged?.Invoke(SelectedButton);
            if (SelectedButton == null)
                CharacterBuilderInterface?.Activate(false);
        }


        //=== Protected ==============================================================

        public override void AfterDependenciesInjected()
        {
            if (!_isInited)
                return;

            WindowsManager.RegisterWindow(this);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            if (!_isInited)
                return;

            CreateButtons();
            SurvivalGui.PawnChangesStream.Action(D, OnOurPawnChanged);

            if (!CharacterItemsNotifier.AssertIfNull(nameof(CharacterItemsNotifier)))
                CharacterItemsNotifier.CharacterItemsChanged += OnCharacterItemsChanged;
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                if (State.Value != GuiWindowState.Closed)
                    WindowsManager.Close(this);
            }
        }

        private void OnCharacterItemsChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            if (State.Value != GuiWindowState.Opened || SelectedButton == null)
                return;

            SelectedButton.UpdateDescription();
        }

        private void CreateButtons()
        {
            for (int i = 0, len = _mainWindow.ButtonsHotkeys.Length; i < len; i++)
            {
                var button = Instantiate(_buttonPrefab, _buttonsRoot);
                button.name = $"{_buttonPrefab.name}{i + 1}";
                button.Hotkey = _mainWindow.ButtonsHotkeys[i];
                button.Init(i, CharacterItemsNotifier, this);
                _buttons.Add(button);
            }

            var remainder = _buttons.Count % ConstructionMainWindow.ButtonsRowCount;
            if (remainder == 0)
                return;

            for (int i = 0, len = ConstructionMainWindow.ButtonsRowCount - remainder; i < len; i++)
            {
                var index = _buttons.Count + 1;
                var button = Instantiate(_buttonPrefab, _buttonsRoot);
                button.name = $"{_buttonPrefab.name}{index + 1}_empty";
                _buttons.Add(button);
            }
        }

        private void SetupButtons(IGrouping<BuildRecipeGroupDef, BuildRecipeDef> recipesGroup, int lastSelectedButtonIndex)
        {
            if (recipesGroup.AssertIfNull(nameof(recipesGroup)) || !recipesGroup.Any())
            {
                UI.Logger.IfError()?.Message($"{nameof(recipesGroup)} is null or empty").Write();
                return;
            }

            if (recipesGroup.Key.AssertIfNull("buildRecipeGroupDef", gameObject))
                return;

            _lastRecipesGroup = recipesGroup;
            var suitableRecipes = recipesGroup.Where(r => r.Tier == DisplayedConstructionsTier).OrderBy(g => g.OrderIndex).ToArray();

            for (int i = 0, len = _buttons.Count; i < len; i++)
            {
                _buttons[i].BuildRecipeDef = i < suitableRecipes.Length ? suitableRecipes[i] : null;
                var rowIndex = i / ConstructionMainWindow.ButtonsRowCount;
                //видимость ряда определяется интерактивностью первой кнопки в этом ряду
                _buttons[i].IsVisible = _buttons[rowIndex * ConstructionMainWindow.ButtonsRowCount].IsInteractable;
            }

            TakeSelection(null);
            if (lastSelectedButtonIndex < 0 || !_buttons[lastSelectedButtonIndex].IsInteractable)
                lastSelectedButtonIndex = _buttons[0].IsInteractable ? 0 : -1; //сброс для пустой

            if (lastSelectedButtonIndex >= 0)
                TakeSelection(_buttons[lastSelectedButtonIndex]);
        }

        private LocalizedString GetTitle()
        {
            return SelectedButton?.Title ?? LsExtensions.Empty;
        }

        private Sprite GetBigIcon()
        {
            return SelectedButton?.BigIcon;
        }

        private string GetDescription()
        {
            return SelectedButton?.Description ?? "";
        }

        private int GetProductMaxCount()
        {
            return SelectedButton?.ProductMaxCount ?? 0;
        }

        private void OnTierChange()
        {
            var nextTier = DisplayedConstructionsTier + 1;
            if (nextTier > MaxTier)
                nextTier = MinTier;

            DisplayedConstructionsTier = nextTier;
        }

        private void OnActionPlace()
        {
            if (SelectedButton == null)
                return;

            bool isEnoughIngreds = SelectedButton.IsEnoughIngreds;
            if (BuildSystem.Builder.ClaimResourcesEnableCheat)
            {
                if (!BuildSystem.Builder.ClaimResourcesValueCheat)
                {
                    isEnoughIngreds = true;
                }
            }

            if (!SelectedButton.IsInteractable ||
                !isEnoughIngreds ||
                CharacterBuilderInterface.AssertIfNull(nameof(CharacterBuilderInterface)) ||
                !CharacterBuilderInterface.IsBuildingPlaceActive)
            {
                UI.Logger.Warn($"Unable to place: {nameof(SelectedButton)}={SelectedButton}, " +
                               $"{nameof(CharacterBuilderInterface.IsBuildingPlaceActive)}" +
                               $"{(CharacterBuilderInterface?.IsBuildingPlaceActive ?? false).AsSign()}");
                return;
            }

            var targetBuildRecipeDef = SelectedButton.BuildRecipeDef;

            if (targetBuildRecipeDef.Type == BuildRecipeDef.BuildType.Building ||
                targetBuildRecipeDef.Type == BuildRecipeDef.BuildType.Fence)
            {
                var result = CharacterBuilderInterface?.CreateElement() ?? false;
                if (result)
                {
                    SoundControl.Instance?.CraftBuildingPlacementEvent?.Post(transform.root.gameObject);
                }
                else
                {
                    SoundControl.Instance?.CraftBuildingPlacementDeniedEvent?.Post(transform.root.gameObject);
                }
            }
            else
            {
                UI.Logger.IfError()?.Message($"Unexpected construction type: {targetBuildRecipeDef.Type}. Nothing to place").Write();
            }
        }

        private void OnActionRotate(bool isPositive)
        {
            if (SelectedButton == null)
                return;

            if (CharacterBuilderInterface.AssertIfNull(nameof(CharacterBuilderInterface)))
            {
                UI.Logger.Warn($"Unable to rotate: {nameof(SelectedButton)}={SelectedButton}, " +
                               $"{nameof(CharacterBuilderInterface)}={CharacterBuilderInterface}");
                return;
            }

            CharacterBuilderInterface.CyclePlaceholderRotation(isPositive);
        }

        private void OnActionMove(bool isPositive)
        {
            if (SelectedButton == null)
                return;

            if (CharacterBuilderInterface.AssertIfNull(nameof(CharacterBuilderInterface)))
            {
                UI.Logger.Warn($"Unable to move: {nameof(SelectedButton)}={SelectedButton}, " +
                               $"{nameof(CharacterBuilderInterface)}={CharacterBuilderInterface}");
                return;
            }

            CharacterBuilderInterface.CyclePlaceholderShift(isPositive, PlaceholderShiftType.Vertical);
        }

        private void OnActionZoom(bool isPositive)
        {
            if (SelectedButton == null)
                return;

            if (CharacterBuilderInterface.AssertIfNull(nameof(CharacterBuilderInterface)))
            {
                UI.Logger.Warn($"Unable to zoom: {nameof(SelectedButton)}={SelectedButton}, " +
                               $"{nameof(CharacterBuilderInterface)}={CharacterBuilderInterface}");
                return;
            }

            CharacterBuilderInterface.CyclePlaceholderShift(isPositive, PlaceholderShiftType.Horizontal);
        }

        private void OnSelectedButtonChanged(SelectionConstructionButton prevButton, SelectionConstructionButton newButton)
        {
            if (prevButton != null)
            {
                prevButton.PropertyChanged -= OnSelectedButtonPropertyChanged;
            }

            if (newButton != null)
            {
                newButton.PropertyChanged += OnSelectedButtonPropertyChanged;
            }

            Title = GetTitle();
            BigIcon = GetBigIcon();
            Description = GetDescription();
            ProductMaxCount = GetProductMaxCount();
        }

        private void OnSelectedButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            switch (propertyName)
            {
                case TitlePropName:
                    Title = GetTitle();
                    break;

                case BigIconPropName:
                    BigIcon = GetBigIcon();
                    break;

                case DescriptionPropName:
                    Description = GetDescription();
                    break;

                case ProductMaxCountPropName:
                    ProductMaxCount = GetProductMaxCount();
                    break;
            }
        }

        private string GetColorTagBegin(Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        }
    }
}