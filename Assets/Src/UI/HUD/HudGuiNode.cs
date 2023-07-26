using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Aspects;
using Assets.Src.ContainerApis;
using Assets.Src.Lib.DOTweenAdds;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.SpawnSystem;
using UnityEngine;
using ColonyDI;
using ColonyShared.SharedCode.Input;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using Uins.Slots;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Utils;
using Uins.Knowledge;
using UnityWeld.Binding;
using SharedCode.EntitySystem;

using Core.Cheats;
using SharedCode.Serializers;
using ShareCode.Threading;

namespace Uins
{
    [Binding]
    public class HudGuiNode : DependencyNodeWithChildren, IGuiWindow, IWeaponSelector
    {
        public event SelectedSlotChangesDelegate SelectedSlotChanged;

        [SerializeField, UsedImplicitly]
        private WindowId _windowId;

        [SerializeField, UsedImplicitly]
        private WindowId _inventoryWindowId;

        [SerializeField, UsedImplicitly]
        private WindowId _containerWindowId;

        [SerializeField, UsedImplicitly]
        private WindowId _mapWindowId;

        [SerializeField, UsedImplicitly]
        public PlayerMainStatsViewModel PlayerMainStatsViewModel;

        [SerializeField, UsedImplicitly]
        public PlayerInteractionViewModel PlayerInteractionViewModel;

        [SerializeField, UsedImplicitly]
        private CanvasGroup _bottomContentCanvasGroup;

        [SerializeField, UsedImplicitly]
        private DebugAimSight _debugAimSight;

        [SerializeField, UsedImplicitly]
        private InteractionIndicators _interactionIndicators;

        [SerializeField, UsedImplicitly]
        private NavigationProvider _navigationProvider;

        [SerializeField, UsedImplicitly]
        private PointMarkers _pointMarkers;

        [SerializeField, UsedImplicitly]
        private NavigationPanel _navigationPanel;

        [SerializeField, UsedImplicitly]
        private HudHpBlockHider _hudHpBlockHider;

        [SerializeField, UsedImplicitly]
        private KnowledgeMessage _knowledgeMessage;

        [SerializeField, UsedImplicitly]
        private PerksNotifier _perksNotifier;

        [SerializeField, UsedImplicitly]
        private TweenComponentBase _spellFailTweenComponent;

        [SerializeField, UsedImplicitly]
        private FactionIndicator _factionIndicator;

        [SerializeField, UsedImplicitly]
        private float _afterOurPawnChangedSilenceTime = 2;

        [SerializeField, UsedImplicitly]
        private AchievedResourcesViewModel _achievedResourcesViewModel;

        [SerializeField, UsedImplicitly]
        private QuestPhaseIndicator _questPhaseIndicator;

        [SerializeField, UsedImplicitly]
        private ChatWindow _chatWindow;

        public UnseenYetPanel UnseenYetPanel;

        [SerializeField, UsedImplicitly]
        private StatDebugViewModel _statDebugVmPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _statDebugRoot;

        [SerializeField, UsedImplicitly]
        private CenterNotificationQueue _centerNotificationQueue;

        [SerializeField, UsedImplicitly]
        private HudSidePanelSwitcher[] _hudSidePanelSwitchers;

        [SerializeField, UsedImplicitly]
        private NegativeIndicatorsVisibility _negativeIndicatorsVisibility;

        [SerializeField, UsedImplicitly]
        private KnockdownContr _knockdownContr;

        private float _lastFailedPlayingTime;
        private float _lastOurPawnChangedTime;

        private List<CharDollInputSlotViewModel> _bottomSlotViewModels = new List<CharDollInputSlotViewModel>();

        private IGuiWindow _inventoryGuiWindow;
        private IGuiWindow _containerGuiWindow;
        private IGuiWindow _mapGuiWindow;

        private IList<ResourceIDFull> _replicatedUsedSlots = new ThreadSafeList<ResourceIDFull>();

        private EntityApiWrapper<HasDollBroadcastApi> _hasDollBroadcastApiWrapper;
        private EntityApiWrapper<HasDollFullApi> _hasDollFullApiWrapper;
        private readonly ListStream<IStatusEffectVM> _statusEffectVMs = new ListStream<IStatusEffectVM>();

        private static MemorySpender _memorySpender = new MemorySpender();


        //=== Props ===============================================================

        [Dependency]
        private GameState GameState { get; set; }

        [Dependency]
        private SurvivalGuiNode SurvivalGui { get; set; }

        [Dependency]
        private InventoryNode InventoryNode { get; set; }

        [Dependency]
        private RespawnGuiNode RespawnGui { get; set; }

        [Dependency]
        private ConstructionGuiNode ConstructionGuiNode { get; set; }

        public static HudGuiNode Instance { get; private set; }

        public bool IsUnclosable => true;

        public WindowId Id => _windowId;

        public ReactiveProperty<GuiWindowState> State { get; set; } = new ReactiveProperty<GuiWindowState>();

        public IListStream<IStatusEffectVM> StatusEffectVMs => _statusEffectVMs;

        public WindowStackId CurrentWindowStack { get; set; }

        public InputBindingsDef InputBindings => null;

        [Binding]
        public bool IsHiCanvasVisible { get; private set; }

        [Binding]
        public bool IsLoCanvasVisible { get; private set; }

        [Binding]
        public bool IsKnockdownButtonVisible { get; private set; }

        public int SelectedWeaponSlotIndex { get; private set; } = -1;

        public LocalizedString DropItemMsgLs { get; set; }

        public LocalizedString PluralItemLs { get; set; }

        public TmpKnockdownInterface KnockdownInterface { get; private set; }


        //=== Unity ===============================================================
        [Cheat]
        public static void SpendMemory()
        {
            _memorySpender.StartSpending(Time.time);
        }
        [Cheat]
        public static void StopSpendignMemory()
        {
            _memorySpender.StopSpending();
        }

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
            _windowId.AssertIfNull(nameof(_windowId));
            _inventoryWindowId.AssertIfNull(nameof(_inventoryWindowId));
            _containerWindowId.AssertIfNull(nameof(_containerWindowId));
            _bottomContentCanvasGroup.AssertIfNull(nameof(_bottomContentCanvasGroup));
            PlayerMainStatsViewModel.AssertIfNull(nameof(PlayerMainStatsViewModel));
            PlayerInteractionViewModel.AssertIfNull(nameof(PlayerInteractionViewModel));
            _debugAimSight.AssertIfNull(nameof(_debugAimSight));
            _interactionIndicators.AssertIfNull(nameof(_interactionIndicators));
            _navigationProvider.AssertIfNull(nameof(_navigationProvider));
            _pointMarkers.AssertIfNull(nameof(_pointMarkers));
            _navigationPanel.AssertIfNull(nameof(_navigationPanel));
            _hudHpBlockHider.AssertIfNull(nameof(_hudHpBlockHider));
            _knowledgeMessage.AssertIfNull(nameof(_knowledgeMessage));
            _perksNotifier.AssertIfNull(nameof(_perksNotifier));
            _spellFailTweenComponent.AssertIfNull(nameof(_spellFailTweenComponent));
            _achievedResourcesViewModel.AssertIfNull(nameof(_achievedResourcesViewModel));
            _questPhaseIndicator.AssertIfNull(nameof(_questPhaseIndicator));
            _chatWindow.AssertIfNull(nameof(_chatWindow));
            UnseenYetPanel.AssertIfNull(nameof(UnseenYetPanel));
            _statDebugVmPrefab.AssertIfNull(nameof(_statDebugVmPrefab));
            _statDebugRoot.AssertIfNull(nameof(_statDebugRoot));
//            _premiumHudNotifier.AssertIfNull(nameof(_premiumHudNotifier));
//            _premiumWindow.AssertIfNull(nameof(_premiumWindow));
            _centerNotificationQueue.AssertIfNull(nameof(_centerNotificationQueue));
            _negativeIndicatorsVisibility.AssertIfNull(nameof(_negativeIndicatorsVisibility));
            _hudSidePanelSwitchers.IsNullOrEmptyOrHasNullElements(nameof(_hudSidePanelSwitchers));
            _knockdownContr.AssertIfNull(nameof(_knockdownContr));
            State.Value = GuiWindowState.Closed;

            PlayerInteractionViewModel.enabled = false;
            _debugAimSight.IsVisible = false;

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            State.Dispose();

            if (Instance == this)
                Instance = null;
        }


        //=== Public ==============================================================

        public void OnOpen(object arg)
        {
            var hudOrInventoryOpened = _inventoryGuiWindow.State
                .Zip(D, State)
                .Zip(D, GameState.IsInGameRp)
                .Func(D, (inventory, hud, inGame) => (inventory == GuiWindowState.Opened || hud == GuiWindowState.Opened) && inGame);
            foreach (var charDollInputSlotViewModel in _bottomSlotViewModels)
                charDollInputSlotViewModel.OnOpen(hudOrInventoryOpened);
        }

        public void OnClose()
        {
            foreach (var charDollInputSlotViewModel in _bottomSlotViewModels)
                charDollInputSlotViewModel.OnClose();
        }

        public void OnFade()
        {
        }

        public void OnUnfade()
        {
        }

        public void OpenUpdate()
        {
            if (!GameState.IsInGameRp.Value)
                return;

            RespawnGui.ChildrenWindowsUpdate();
            ConstructionGuiNode?.MainWindow.ClosedHotkeyUpdate();
            _inventoryGuiWindow.ClosedHotkeyUpdate();
            _mapGuiWindow?.ClosedHotkeyUpdate();
            _chatWindow.ClosedHotkeyUpdate();

            _memorySpender.Spending(Time.time);
            //_premiumWindow.ClosedHotkeyUpdate();
        }

        public void NoClosedUpdate() // Перевёл на стримы, инициализирующиеся по OnOpen PZ-14910
        {
        }

        public void ClosedHotkeyUpdate(Action additionalAction = null)
        {
        }

        public void SlotSelectionRequest(int newSelectedWeaponSlotIndex = -1)
        {
            //UI.CallerLog($"newSelectedWeaponSlotIndex={newSelectedWeaponSlotIndex} SelectedWeaponSlotIndex={SelectedWeaponSlotIndex}"); //DEBUG
            if (newSelectedWeaponSlotIndex == SelectedWeaponSlotIndex)
                return;

            SelectedWeaponSlotIndex = newSelectedWeaponSlotIndex;
            SelectedSlotChanged?.Invoke(SelectedWeaponSlotIndex);
        }

        public async Task SlotUsageRequest(SlotDef slotDef, bool isInUse)
        {
            //UI.CallerLog($"slot={slotDef.Id} isInUse{isInUse.AsSign()}"); //DEBUG
            await Awaiters.ThreadPool;

            if (slotDef.AssertIfNull(nameof(slotDef)) ||
                GameState.Instance.AssertIfNull("GameState.Instance"))
                return;

            var slotResourceIDFull = GameResourcesHolder.Instance.GetID(slotDef);

            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            using (var wrapper = await NodeAccessor.Repository.Get<IWorldCharacterClientFull>(characterId))
            {
                IList<ResourceIDFull> indicesToAdd = new ThreadSafeList<ResourceIDFull>();
                IList<ResourceIDFull> indicesToRemove = new ThreadSafeList<ResourceIDFull>();
                if (isInUse)
                {
                    //добавить slotIndex, остальных убрать
                    indicesToAdd.Add(slotResourceIDFull);
                    if (_replicatedUsedSlots.Count > 0)
                        indicesToRemove = _replicatedUsedSlots;
                }
                else
                {
                    //просто убрать slotIndex
                    indicesToRemove.Add(slotResourceIDFull);
                }

                if (indicesToRemove.Count > 0)
                    foreach (var idFull in indicesToRemove)
                    {
                        await wrapper.Get<IWorldCharacterClientFull>(characterId).RemoveUsedSlot(idFull);
                    }

                if (indicesToAdd.Count > 0)
                    foreach (var idFull in indicesToAdd)
                    {
                        await wrapper.Get<IWorldCharacterClientFull>(characterId).AddUsedSlot(idFull);
                    }
            }
        }

        public static void StaminaFlash()
        {
            Instance?.StaminaFlashWork();
        }

        public override string ToString()
        {
            return GuiWindowHelper.GuiWindowToString(this);
        }

        public override void AfterDependenciesInjected()
        {
            WindowsManager.RegisterWindow(this);
        }

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            _inventoryGuiWindow = WindowsManager.GetWindow(_inventoryWindowId);
            _containerGuiWindow = WindowsManager.GetWindow(_containerWindowId);
            _mapGuiWindow = WindowsManager.GetWindow(_mapWindowId);

            if (_inventoryGuiWindow.AssertIfNull(nameof(_inventoryGuiWindow)) ||
                _containerGuiWindow.AssertIfNull(nameof(_containerGuiWindow)) || //TODOM Убрать, если не понадобится при переключении Lo-канваса
                _mapGuiWindow.AssertIfNull(nameof(_mapGuiWindow))) //TODOM Аналогично
                return;

            var pawnStream = SurvivalGui.PawnChangesStream;
            var isInGameStream = GameState.IsInGameRp;

            pawnStream.Action(D, OnOurPawnChanged);

            var hasPawnStream = pawnStream
                .Zip(D, isInGameStream)
                .Func(D, (prevEgo, newEgo, isInGame) => isInGame && newEgo != null);

            KnockdownInterface = new TmpKnockdownInterface(hasPawnStream);
            _knockdownContr.SetVmodel(KnockdownInterface);

            TimeTicker.Instance.GetLocalTimer(0.01f)
                .Zip(D, hasPawnStream)
                .Where(D, (dt, hasPawn) => hasPawn)
                .Action(D, t => PlayerMainStatsViewModel.OnUpdate());

            Bind(hasPawnStream, () => IsHiCanvasVisible);

            var isLoCanvasVisibleStream = hasPawnStream
                .Zip(D, _inventoryGuiWindow.State)
                .Func(D, (hasPawn, inventoryWndState) => hasPawn && inventoryWndState == GuiWindowState.Closed);

            isLoCanvasVisibleStream.Action(D, b => _debugAimSight.IsVisible = b);
            Bind(isLoCanvasVisibleStream, () => IsLoCanvasVisible);

            pawnStream
                .Transform(
                    D,
                    tuple =>
                    {
                        var (_, newEgo1) = tuple;
                        return newEgo1 != null ? new TraumasVM(newEgo1.OuterRef) : null;
                    }
                )
                .SubListStream(D, vm => vm.StatusEffectVMs)
                .CopyTo(D, _statusEffectVMs);

            _interactionIndicators.Init(SurvivalGui);
            _navigationProvider.Init(SurvivalGui, InventoryNode.QuestTrackingContext);
            _pointMarkers.Init(SurvivalGui);
            //_premiumHudNotifier.Init(SurvivalGui);

            _navigationPanel.Init(SurvivalGui, NavigationProvider.Instance);

            PlayerMainStatsViewModel.Init(SurvivalGui, _statusEffectVMs);
            _negativeIndicatorsVisibility.Init();

            PlayerInteractionViewModel.enabled = true;
            PlayerInteractionViewModel.Init(SurvivalGui, InventoryNode.CharacterItemsNotifier);
            _knowledgeMessage.Init(SurvivalGui);
            _hudHpBlockHider.Init(InventoryNode);
            _perksNotifier.Init(SurvivalGui, InventoryNode.PerksPanelViewModel, InventoryNode.ContextViewWithParamsVmodelStream, WindowsManager);
            _factionIndicator.Init(SurvivalGui);
            FindBottomSlotViewModels();
            _achievedResourcesViewModel.Init(SurvivalGui, InventoryNode.CharacterItemsNotifier, WindowsManager);
            _questPhaseIndicator.Init(InventoryNode.QuestTrackingContext);
            _centerNotificationQueue.Init(SurvivalGui, WindowsManager);
            for (int i = 0; i < _hudSidePanelSwitchers.Length; i++)
                _hudSidePanelSwitchers[i].Init(WindowsManager);
        }


        //=== Private =============================================================

        private void StaminaFlashWork()
        {
            if (Time.time - _lastOurPawnChangedTime < _afterOurPawnChangedSilenceTime ||
                Time.time - _lastFailedPlayingTime < _spellFailTweenComponent.Duration + _spellFailTweenComponent.Delay)
                return;

            _lastFailedPlayingTime = Time.time;
            _spellFailTweenComponent.Play(true);
        }

        private void FindBottomSlotViewModels()
        {
            if (_bottomSlotViewModels.Count == 0)
                _bottomSlotViewModels = _bottomContentCanvasGroup.GetComponentsInChildren<CharDollInputSlotViewModel>().ToList();
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            _lastOurPawnChangedTime = Time.time;
            if (prevEgo != null && _hasDollBroadcastApiWrapper != null)
            {
                _hasDollBroadcastApiWrapper.EntityApi.UnsubscribeFromUsedSlotsChanged(OnUsedSlotsChanged);
                SubscribeToItemDroppedEvent(prevEgo.EntityId, false);
                UnsubscribeBottomSlots(_hasDollBroadcastApiWrapper.EntityApi, _hasDollFullApiWrapper.EntityApi);

                _hasDollBroadcastApiWrapper.Dispose();
                _hasDollBroadcastApiWrapper = null;

                _hasDollFullApiWrapper.Dispose();
                _hasDollFullApiWrapper = null;
            }

            if (newEgo != null)
            {
                _hasDollBroadcastApiWrapper = EntityApi.GetWrapper<HasDollBroadcastApi>(newEgo.OuterRef);
                _hasDollFullApiWrapper = EntityApi.GetWrapper<HasDollFullApi>(newEgo.OuterRef);

                _hasDollBroadcastApiWrapper.EntityApi.SubscribeToUsedSlotsChanged(OnUsedSlotsChanged);
                SubscribeToItemDroppedEvent(newEgo.EntityId, true);
                SubscribeBottomSlots(_hasDollBroadcastApiWrapper.EntityApi, _hasDollFullApiWrapper.EntityApi);
            }
        }

        private void SubscribeToItemDroppedEvent(Guid playerGuid, bool subscribe)
        {
            var repo = NodeAccessor.Repository;
            if (repo != null)
                AsyncUtils.RunAsyncTask(async () =>
                    {
                        using (var wrapper = await repo.Get<IWorldCharacterClientFull>(playerGuid))
                        {
                            var playerEntity = wrapper.Get<IWorldCharacterClientFull>(playerGuid);
                            if (playerEntity.AssertIfNull(nameof(playerEntity)))
                                return;

                            if (subscribe)
                                playerEntity.ItemDroppedEvent += PlayerEntity_ItemDroppedEvent;
                            else
                                playerEntity.ItemDroppedEvent -= PlayerEntity_ItemDroppedEvent;
                        }
                    });
        }

        private async Task PlayerEntity_ItemDroppedEvent(BaseItemResource item, int count)
        {
            await UnityQueueHelper.RunInUnityThread(() =>
            {
                WarningMessager.Instance?.ShowWarningMessage(
                    DropItemMsgLs.GetText(0, PluralItemLs.GetText(count), item.ItemNameLs.GetText()),
                    UnityEngine.Color.yellow);
            });
        }

        private void SubscribeBottomSlots(HasDollBroadcastApi hasDollBroadcastApi, HasDollFullApi hasDollFullApi)
        {
            if (hasDollBroadcastApi.AssertIfNull(nameof(hasDollBroadcastApi)) ||
                hasDollFullApi.AssertIfNull(nameof(hasDollFullApi)))
                return;

            for (int i = 0, len = _bottomSlotViewModels.Count; i < len; i++)
            {
                var dollInputSvm = _bottomSlotViewModels[i];
                dollInputSvm.Subscribe(hasDollBroadcastApi, hasDollFullApi);
                dollInputSvm.SetWeaponSelector(this);
                dollInputSvm.SetContextActionsSource(InventoryNode.ContextMenuViewModel);
            }
        }

        private void UnsubscribeBottomSlots(HasDollBroadcastApi hasDollBroadcastApi, HasDollFullApi hasDollFullApi)
        {
            if (hasDollBroadcastApi.AssertIfNull(nameof(hasDollBroadcastApi)) ||
                hasDollFullApi.AssertIfNull(nameof(hasDollFullApi)))
                return;

            for (int i = 0, len = _bottomSlotViewModels.Count; i < len; i++)
            {
                var dollInputSvm = _bottomSlotViewModels[i];
                dollInputSvm.Unsubscribe(hasDollBroadcastApi, hasDollFullApi);
                dollInputSvm.Reset();
            }
        }

        private void OnUsedSlotsChanged(IList<ResourceIDFull> usedSlotsIndices)
        {
            if (usedSlotsIndices.AssertIfNull(nameof(usedSlotsIndices)))
                return;

            _replicatedUsedSlots = usedSlotsIndices;
        }
    }
}