using System;
using Assets.Src.Inventory;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using ColonyShared.SharedCode.Input;
using Uins.ContainerWindow;
using Uins.Cursor;
using Uins.GuiWindows;
using Uins.Sound;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityWeld.Binding;
using Transform = UnityEngine.Transform;

namespace Uins
{
    [Binding]
    public class InventoryNode : DependencyNodeWithChildren, IGuiWindow
    {
        public enum WindowMode
        {
            Normal,
            Container,
            Machine
        }

        [UsedImplicitly, SerializeField]
        private GameObject[] _mustBeActive;

        [UsedImplicitly, SerializeField, FormerlySerializedAs("WindowId")]
        private WindowId _windowId;

        [UsedImplicitly, SerializeField]
        private TabsContextContr _inventoryTabsContext;

        [UsedImplicitly, SerializeField]
        private TechnoAtlasContr _technoAtlasContr;

        [UsedImplicitly, SerializeField]
        private TechnosSideContr _technosSideContr;

        [UsedImplicitly, SerializeField]
        private OurCharacterSlotsViewModel _ourCharacterSlotsViewModel;

        [UsedImplicitly, SerializeField]
        private CraftSideViewModel _craftSideViewModel;

        [UsedImplicitly, SerializeField]
        private MachineCraftSideViewModel _machineCraftSideViewModel;

        [UsedImplicitly, SerializeField]
        private InventoryContextMenuViewModel _contextMenuViewModel;

        [UsedImplicitly, SerializeField]
        private Transform _rightPart;

        [UsedImplicitly, SerializeField]
        private HotkeyListener _inventoryHotkey;

        [UsedImplicitly, SerializeField]
        private HotkeyListener _escHotkey;

        [UsedImplicitly, SerializeField]
        private ContainerGuiWindow _containerGuiWindow;

        [UsedImplicitly, SerializeField]
        private InventoryUiOpener _inventoryUiOpener;

        [UsedImplicitly, SerializeField]
        private PerksPanelViewModel _perksPanelViewModel;

        [UsedImplicitly, SerializeField]
        private PlayerPointsSource _playerPointsSource;

        [UsedImplicitly, SerializeField]
        private QuestsPanelViewModel _questsPanelViewModel;

        [UsedImplicitly, SerializeField]
        private WindowId _benchMountingGuiWindowId;

        public MapWindowOpener MapWindowOpener;

        [UsedImplicitly, SerializeField]
        private CraftQueueSlots _selfCcraftQueueSlots;

        [UsedImplicitly, SerializeField]
        private CraftQueueSlots _machineCraftQueueSlots;

        [UsedImplicitly, SerializeField]
        private CraftRecipeContextView _craftRecipeContextView;

        [UsedImplicitly, SerializeField]
        private WorldCharacterDefRef _worldCharacterDefRef;

        [UsedImplicitly, SerializeField]
        private CharStatsContextViewModel _charStatsContextViewModel;

        private CraftWindowLogic _craftUiLogic;
        private AK.Wwise.Event _closeWindowSoundEvent;
        private CursorControl.Token _token;


        //=== Props ===============================================================

        [ColonyDI.Dependency]
        private GameState GameState { get; set; }

        [ColonyDI.Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        [ColonyDI.Dependency]
        private HudGuiNode HudGui { get; set; }

        [Binding]
        public bool IsOpenOrFaded { get; private set; }

        [Binding]
        public Sprite GenderSprite { get; private set; }

        public RecipesSource RecipesSource { get; private set; }

        /// <summary>
        /// Стейт смены GuiWindowState и WindowMode без лишних (неконсистентных) состояний
        /// </summary>
        public StreamProxy<(GuiWindowState, WindowMode)> InventoryStateAndMode { get; } = new StreamProxy<(GuiWindowState, WindowMode)>();

        /// <summary>
        /// Стейт для подписок на функционал крафта
        /// </summary>
        public ReactiveProperty<CraftSourceVmodel> CraftSourceRp { get; } = new ReactiveProperty<CraftSourceVmodel>() {Value = null};

        public ICharacterItemsNotifier CharacterItemsNotifier => _ourCharacterSlotsViewModel;

        public PerksPanelViewModel PerksPanelViewModel => _perksPanelViewModel;

        public WindowId Id => _windowId;

        public WindowStackId CurrentWindowStack { get; set; }

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public bool IsUnclosable => false;

        public WindowMode Mode { get; private set; }

        public InputBindingsDef InputBindings => CurrentWindowStack.InputBindings ?? UI.BlockedActionsAndCamera;

        public InventoryContextMenuViewModel ContextMenuViewModel => _contextMenuViewModel;

        public IQuestTrackingContext QuestTrackingContext => _questsPanelViewModel.QuestTrackingContext;

        public HotkeyListener DefaultListener => _inventoryHotkey;

        public IStream<ContextViewWithParamsVmodel> ContextViewWithParamsVmodelStream => _inventoryTabsContext.Vmodel;


        //=== Unity ===============================================================

        private void Awake()
        {
            _windowId.AssertIfNull(nameof(_windowId));
            _ourCharacterSlotsViewModel.AssertIfNull(nameof(_ourCharacterSlotsViewModel));
            _craftSideViewModel.AssertIfNull(nameof(_craftSideViewModel));
            _machineCraftSideViewModel.AssertIfNull(nameof(_machineCraftSideViewModel));
            _inventoryHotkey.AssertIfNull(nameof(_inventoryHotkey));
            _escHotkey.AssertIfNull(nameof(_escHotkey));
            _rightPart.AssertIfNull(nameof(_rightPart));
            _contextMenuViewModel.AssertIfNull(nameof(_contextMenuViewModel));
            _containerGuiWindow.AssertIfNull(nameof(_containerGuiWindow));
            _inventoryUiOpener.AssertIfNull(nameof(_inventoryUiOpener));
            _perksPanelViewModel.AssertIfNull(nameof(_perksPanelViewModel));
            _playerPointsSource.AssertIfNull(nameof(_playerPointsSource));
            _benchMountingGuiWindowId.AssertIfNull(nameof(_benchMountingGuiWindowId));
            _questsPanelViewModel.AssertIfNull(nameof(_questsPanelViewModel));
            _inventoryTabsContext.AssertIfNull(nameof(_inventoryTabsContext));
            MapWindowOpener.AssertIfNull(nameof(MapWindowOpener));
            _technoAtlasContr.AssertIfNull(nameof(_technoAtlasContr));
            _selfCcraftQueueSlots.AssertIfNull(nameof(_selfCcraftQueueSlots));
            _craftRecipeContextView.AssertIfNull(nameof(_craftRecipeContextView));
            _charStatsContextViewModel.AssertIfNull(nameof(_charStatsContextViewModel));

            State.Value = GuiWindowState.Closed;
            _contextMenuViewModel.enabled = false;

            if (!_mustBeActive.IsNullOrEmptyOrHasNullElements(nameof(_mustBeActive)))
                for (int i = 0; i < _mustBeActive.Length; i++)
                    _mustBeActive[i]?.SetActive(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            InventoryStateAndMode.Dispose();
            State.Dispose();
        }


        //=== Public ==============================================================

        public void OnOpen(object arg)
        {
            var openParams = (OpenParams) arg ?? new OpenParams();
            Mode = openParams.Mode;
            PlayOpenInventorySound(Mode, _containerGuiWindow.State.Value != GuiWindowState.Closed);
            BlurredBackground.Instance.SwitchCameraFullBlur(this, true);
            _token = CursorControl.AddCursorFreeRequest(this);

            InventoryStateAndMode.OnNext((State.Value, Mode));

            if (openParams.Listener != null)
            {
                _inventoryTabsContext.OpenTabByHotkey(openParams.Listener);
            }
            else
            {
                if (Mode == WindowMode.Container)
                    _inventoryTabsContext.CloseOpenedTab();
                else
                    _inventoryTabsContext.OpenTabByInventoryTabType(openParams.InventoryTabType);
            }

            CraftSourceRp.Value = Mode == WindowMode.Machine ? openParams.CraftSourceVmodel : null;

            CheckRightPartActivity();
        }

        public void OnClose()
        {
            PlayCloseInventorySound();
            BlurredBackground.Instance.SwitchCameraFullBlur(this, false);
            _token.Dispose();
            _token = null;
            if (_containerGuiWindow.State.Value != GuiWindowState.Closed)
                WindowsManager.Close(_containerGuiWindow);
            _contextMenuViewModel.CloseContextMenuRequest();
            CheckRightPartActivity();
            InventoryStateAndMode.OnNext((State.Value, Mode));
            Mode = WindowMode.Normal;
            CraftSourceRp.Value = null;
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            if (_escHotkey.IsFired())
            {
                WindowsManager.Close(this);
                return;
            }

            if (_inventoryTabsContext.IsSomeListenerFired(false, out var firedHotkeyListener, out var isAlreadyOpened))
            {
                if (isAlreadyOpened)
                {
                    WindowsManager.Close(this);
                }
                else
                {
                    _inventoryTabsContext.OpenTabByHotkey(firedHotkeyListener);
                }

                return;
            }

            MapWindowOpener.OpenInventoryUpdate();
            _questsPanelViewModel.OnUpdate();
        }

        public void NoClosedUpdate()
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
            if (State.Value == GuiWindowState.Closed &&
                _inventoryTabsContext.IsSomeListenerFired(true, out var firedHotkeyListener, out var isAlreadyOpened))
            {
                additionalAction?.Invoke();
                WindowsManager.Open(this, null, new OpenParams(WindowMode.Normal, firedHotkeyListener));
            }
        }

        public override string ToString()
        {
            return GuiWindowHelper.GuiWindowToString(this);
        }

        public void SetCustomMode(WindowMode mode)
        {
            Mode = mode;
            _rightPart.gameObject.SetActive(false);
        }

//        public void SetTechnologiesField(Transform technologiesFieldTransform) //2del После переноса UnseenYetPanel
//        {
//            _technologiesPanel.Init(SurvivalGui, WindowsManager, technologiesFieldTransform, HudGui.UnseenYetPanel, ContextViewWithParamsVmodelStream);
//        }


        //=== Protected ===========================================================

        public override void AfterDependenciesInjected()
        {
            _contextMenuViewModel.enabled = true;
            WindowsManager.RegisterWindow(this);
            RecipesSource = new RecipesSource(SurvivalGui);
            GameState.IsInGameRp.Action(D, OnIsInGameChanged);
            SurvivalGui.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            Bind(State.Func(D, state => state != GuiWindowState.Closed), () => IsOpenOrFaded);
            Bind(SurvivalGui.GenderRp.Func(D, genderDef => genderDef?.InventoryImage?.Target), () => GenderSprite);

            _worldCharacterDefRef.Target.AssertIfNull(nameof(_worldCharacterDefRef));
            _craftUiLogic = new CraftWindowLogic(
                _selfCcraftQueueSlots,
                _machineCraftQueueSlots,
                _craftRecipeContextView,
                _worldCharacterDefRef.Target.MaxCraftQueueSize,
                _craftSideViewModel,
                _machineCraftSideViewModel,
                CraftSourceRp);

            var benchMountingGuiWindow = WindowsManager.GetWindow(_benchMountingGuiWindowId);
            _craftUiLogic.Init(D, SurvivalGui);
            _contextMenuViewModel.Init(_ourCharacterSlotsViewModel, this, benchMountingGuiWindow, WindowsManager);
            _ourCharacterSlotsViewModel.Init(SurvivalGui, HudGui);
            _craftSideViewModel.Init(SurvivalGui, RecipesSource);
            _machineCraftSideViewModel.Init(SurvivalGui, RecipesSource);
            _playerPointsSource.Init(SurvivalGui, this);
            _inventoryUiOpener.Init(this, WindowsManager);
            _perksPanelViewModel.Init(SurvivalGui, WindowsManager);
            _questsPanelViewModel.Init(SurvivalGui, HudGui.UnseenYetPanel, WindowsManager, ContextViewWithParamsVmodelStream);
            _technoAtlasContr.SetCharacterStreams(SurvivalGui, _playerPointsSource, this, ContextViewWithParamsVmodelStream);
            _technosSideContr.Init(SurvivalGui, _playerPointsSource, WindowsManager);
            _inventoryTabsContext.InitModeStream(InventoryStateAndMode.Func(D, (state, mode) => mode));
            _charStatsContextViewModel.Init(SurvivalGui, HudGui);
        }


        //=== Private =============================================================

        private void OnIsInGameChanged(bool isWorking)
        {
            if (!isWorking && State.Value != GuiWindowState.Closed)
                WindowsManager.Close(this);
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                if (State.Value != GuiWindowState.Closed)
                    WindowsManager.Close(this);
            }
        }

        private void PlayOpenInventorySound(WindowMode mode, bool isContainerWindowOpened)
        {
            var openSoundEvent = SoundControl.Instance.InventoryOpenEvent;
            _closeWindowSoundEvent = SoundControl.Instance.InventoryCloseEvent;

            if (isContainerWindowOpened)
            {
                openSoundEvent = SoundControl.Instance.OpenedContainerInventoryOpenEvent;
                _closeWindowSoundEvent = null; //Играется в самом котейнере
            }
            else
            {
                switch (mode)
                {
                    case WindowMode.Machine:
                        openSoundEvent = SoundControl.Instance.BenchInventoryOpenEvent;
                        _closeWindowSoundEvent = SoundControl.Instance.BenchInventoryCloseEvent;
                        break;
                }
            }

            openSoundEvent?.Post(gameObject);
        }

        private void PlayCloseInventorySound()
        {
            _closeWindowSoundEvent?.Post(gameObject);
        }

        private void CheckRightPartActivity()
        {
            _rightPart.gameObject.SetActive(_containerGuiWindow.State.Value == GuiWindowState.Closed);
        }

        public class OpenParams
        {
            public WindowMode Mode;
            public HotkeyListener Listener;
            public InventoryTabType InventoryTabType;
            public CraftSourceVmodel CraftSourceVmodel;

            public OpenParams(WindowMode mode = WindowMode.Normal, HotkeyListener listener = null) : this(mode, listener, InventoryTabType.Crafting, null)
            {
            }

            public OpenParams(CraftSourceVmodel craftSourceVmodel) : this(WindowMode.Machine, null, InventoryTabType.Machine, craftSourceVmodel)
            {
            }

            private OpenParams(WindowMode mode, HotkeyListener listener, InventoryTabType inventoryTabType, CraftSourceVmodel craftSourceVmodel)
            {
                Mode = mode;
                CraftSourceVmodel = craftSourceVmodel;
                InventoryTabType = inventoryTabType;
                Listener = listener;
            }
        }
    }
}