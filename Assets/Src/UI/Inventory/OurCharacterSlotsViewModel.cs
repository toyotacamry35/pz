using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using Assets.Tools;
using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;
using Assets.Src.Aspects;
using Uins.Slots;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using Uins.Sound;
using UnityEngine.Serialization;
using SlotItem = Assets.Src.Inventory.SlotItem;
using Transform = UnityEngine.Transform;
using SharedCode.Repositories;
using SharedCode.Serializers;

namespace Uins
{
    [Binding]
    public class OurCharacterSlotsViewModel : BindingViewModel, ICharacterItemsNotifier, ISlotAcceptanceResolver
    {
        private const int InventorySlotsMaxCount = 36;

        public event SlotChangedDelegate CharacterItemsChanged;

        [SerializeField, UsedImplicitly]
        private CharacterStatsMapping _statsMapping;

        [SerializeField, UsedImplicitly]
        private Transform _dollSlotsTransform;

        [SerializeField, UsedImplicitly]
        private Transform _inventorySlotsTransform;

        [SerializeField, UsedImplicitly]
        private DraggingHandler _draggingHandler;

        [SerializeField, UsedImplicitly]
        private InventoryContextMenuViewModel _contextMenuViewModel;

        [SerializeField, UsedImplicitly, FormerlySerializedAs("_contextView")]
        private ContextViewWithParams _contextViewWithParams;

        [SerializeField, UsedImplicitly]
        private CharInvSlotViewModel _inventorySlotViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _blockedSlotPrefab;

        [SerializeField, UsedImplicitly]
        private FiltrableTypeFilterViewModel _inventoryFilterViewModel;

        private List<Transform> _blockedSlots = new List<Transform>();
        private IWeaponSelector _weaponSelector;

        private ItemSpecificStats _defaultSpecificStats;
        private int _usedSlotIndex = -1;

        private bool _needForBlockedSlots;

        private List<BaseItemResource> _lastMovedFromSelfResources = new List<BaseItemResource>();
        private EntityApiWrapper<EntityGuidApi> _entityGuidApiWrapper;
        private EntityApiWrapper<HasDollBroadcastApi> _hasDollBroadcastApiWrapper;
        private EntityApiWrapper<HasDollFullApi> _hasDollFullApiWrapper;
        private EntityApiWrapper<HasInventoryFullApi> _hasInventoryFullApiWrapper;


        //=== Props ===============================================================

        public InventoryFiltrableTypeDef CurrentInventoryFiltrableTypeDef { get; private set; }

        public Guid PlayerGuid { get; private set; }

        public List<CharInvSlotViewModel> InventoryViewModels { get; } = new List<CharInvSlotViewModel>();

        public List<CharDollSlotViewModel> DollViewModels { get; } = new List<CharDollSlotViewModel>();

        //--- Binding

        private int _inventorySlotsCount;

        [Binding]
        public int InventorySlotsCount
        {
            get => _inventorySlotsCount;
            set
            {
                if (value != _inventorySlotsCount)
                {
                    _inventorySlotsCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _inventoryItemsCount;

        [Binding]
        public int InventoryItemsCount
        {
            get => _inventoryItemsCount;
            set
            {
                if (value != _inventoryItemsCount)
                {
                    _inventoryItemsCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _inventoryEmptySlotsCount;

        [Binding]
        public int InventoryEmptySlotsCount
        {
            get => _inventoryEmptySlotsCount;
            set
            {
                if (_inventoryEmptySlotsCount != value)
                {
                    _inventoryEmptySlotsCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ItemSpecificStats _selectedItemSpecificStats;

        public ItemSpecificStats SelectedItemSpecificStats
        {
            get => _selectedItemSpecificStats;
            set
            {
                if (_selectedItemSpecificStats != value)
                {
                    _selectedItemSpecificStats = value;
//                    if (SelectedItemSpecificStats != null)
//                    {
//                        UsedDamageType = SelectedItemSpecificStats.DamageType;
//                    }
//                    else
//                    {
//                        UsedDamageType = null;
//                    }
                }
            }
        }

//        private float GetSpecificStatValue(StatModifier[] specificStatModifiers, StatResource statResource)
//        {
//            if (specificStatModifiers == null || specificStatModifiers.Length == 0 || statResource == null)
//                return 0;
//
//            return specificStatModifiers.FirstOrDefault(sm => sm.Stat.Target == statResource).Value; //даже если не найдется, вернет 0
//        }

//        private DamageTypeDef _usedDamageType;
//
//        public DamageTypeDef UsedDamageType
//        {
//            get => _usedDamageType;
//            set
//            {
//                if (value != _usedDamageType)
//                {
//                    _usedDamageType = value;
//                    NotifyPropertyChanged(nameof(UsedDamageTypeIcon));
//                    NotifyPropertyChanged(nameof(UsedDamageTypeName));
//                    NotifyPropertyChanged(nameof(IsUsedDamageType));
//                }
//            }
//        }
//
//        [Binding]
//        public Sprite UsedDamageTypeIcon => UsedDamageType?.Sprite?.Target;
//
//        [Binding]
//        public bool IsUsedDamageType => UsedDamageType != null;
//
//        [Binding]
//        public string UsedDamageTypeName => UsedDamageType == null ? "none" : UsedDamageType.DisplayNameLs.GetText();


        //=== Unity ===============================================================

        private void Awake()
        {
            _dollSlotsTransform.AssertIfNull(nameof(_dollSlotsTransform));
            _inventorySlotsTransform.AssertIfNull(nameof(_inventorySlotsTransform));
            _draggingHandler.AssertIfNull(nameof(_draggingHandler));
            _contextViewWithParams.AssertIfNull(nameof(_contextViewWithParams));
            _contextMenuViewModel.AssertIfNull(nameof(_contextMenuViewModel));
            _inventorySlotViewModelPrefab.AssertIfNull(nameof(_inventorySlotViewModelPrefab));
            _blockedSlotPrefab.AssertIfNull(nameof(_blockedSlotPrefab));
            _inventoryFilterViewModel.AssertIfNull(nameof(_inventoryFilterViewModel));
            _statsMapping.AssertIfNull(nameof(_statsMapping));

            _inventoryFilterViewModel.FiltrableTypeFilterChanged += OnFiltrableTypeFilterChanged;
        }


        //=== Public ==============================================================

        public void Init(IPawnSource pawnSource, IWeaponSelector weaponSelector)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);

            _weaponSelector = weaponSelector;
            FindDollSlotViewModels();
        }

        public CharInvSlotViewModel GetInventoryFirstEmptySlot()
        {
            return InventoryViewModels.FirstOrDefault(slotViewModel => slotViewModel.IsEmpty);
        }

        public List<CharDollSlotViewModel> GetDollSuitableSlots(SlotViewModel fromSlotViewModel)
        {
            return DollViewModels
                .Where(svm => svm.CanMoveToThis(fromSlotViewModel))
                .OrderBy(svm => 10000 - svm.SlotDef.SelectionPriority * 100 + svm.SlotDef.SlotId)
                .ToList();
        }

        public List<CharInvSlotViewModel> GetInventorySuitableSlots(SlotViewModel fromSlotViewModel)
        {
            return InventoryViewModels
                .Where(svm => svm.CanMoveToThis(fromSlotViewModel))
                .OrderBy(svm => (svm.IsEmpty ? 0 : 1) * -100 + svm.SlotId)
                .ToList();
        }

        public int GetItemResourceCount([NotNull] BaseItemResource itemResource)
        {
            int itemsCount = 0;
            if (itemResource.AssertIfNull(nameof(itemResource)))
                return itemsCount;
            var inventoryStack = InventoryViewModels.Sum(svm => svm.ItemResource == itemResource ? svm.Stack : 0);
            itemsCount = inventoryStack;
            var dollStack = DollViewModels.Sum(svm => svm.ItemResource == itemResource ? svm.Stack : 0);
            itemsCount += dollStack;
            //UI.CallerLog($"{itemResource}: {inventoryStack} + {dollStack} = {itemsCount}"); //2del
            return itemsCount;
        }

        public int TryMoveTo(SlotViewModel fromSvm, SlotViewModel toSvm, bool doMove = false, bool isCounterSwapCheck = false)
        {
            if (!toSvm.IsOurPlayerSlot)
            {
                UI.Logger.Error(
                    $"{nameof(TryMoveTo)}(from={fromSvm}, to={toSvm}, do{doMove.AsSign()}) " +
                    $"{nameof(toSvm)} isn't {nameof(toSvm.IsOurPlayerSlot)}");
                return 0;
            }

            var acceptedStack = 0;

            if (toSvm.IsInventorySlot)
            {
                if (fromSvm.ItemResource is DollItemResource)
                    return 0;

                acceptedStack = fromSvm.Stack;
            }
            else
            {
                var charDollSlotViewModel = toSvm as CharDollSlotViewModel;
                if (charDollSlotViewModel.AssertIfNull(nameof(charDollSlotViewModel)))
                    return 0;

                acceptedStack = !charDollSlotViewModel.IsInaccessible &&
                                charDollSlotViewModel.SlotDef.Accepts(fromSvm.ItemType)
                    ? fromSvm.Stack
                    : 0;
            }

            //проверка возможности переместить содержимое toSvm во fromSvm
            if (acceptedStack > 0 &&
                !toSvm.IsEmpty &&
                !isCounterSwapCheck && //против зацикливания
                fromSvm.SlotAcceptanceResolver.TryMoveTo(toSvm, fromSvm, false, true) <= 0)
                acceptedStack = 0;

            if (acceptedStack > 0 && doMove)
            {
                ClusterCommands.MoveItem_OnClient(PlayerGuid, fromSvm, toSvm, acceptedStack);
                SaveLastMovedFromDollItemResource(fromSvm, toSvm);
                if (toSvm is CharDollSlotViewModel)
                    InventoryContextMenuViewModel.DoEquipmentSound(true, toSvm, gameObject);
                if (toSvm.IsEmpty && fromSvm is CharDollSlotViewModel)
                    InventoryContextMenuViewModel.DoEquipmentSound(false, fromSvm, gameObject);
            }

            return acceptedStack;
        }

        public bool TryToDropFrom(SlotViewModel fromSvm)
        {
            SoundControl.Instance.ItemDrop.Post(gameObject);
            ClusterCommands.DropItemAsBox_OnClient(fromSvm);
            return true;
        }


        //=== Private =============================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                PlayerGuid = Guid.Empty;
                _entityGuidApiWrapper?.EntityApi.UnsubscribeFromEntityGuid(GetPlayerGuid);
                _hasDollBroadcastApiWrapper?.EntityApi.UnsubscribeFromUsedSlotsChanged(OnUsedSlotsChanged);
                if (_hasDollBroadcastApiWrapper != null)
                {
                    UnsubscribeDollSlots(_hasDollBroadcastApiWrapper?.EntityApi, _hasDollFullApiWrapper?.EntityApi);
                    UnsubscribeInventorySlots(_hasInventoryFullApiWrapper?.EntityApi);
                }

                if (_entityGuidApiWrapper != null)
                {
                    _entityGuidApiWrapper.Dispose();
                    _entityGuidApiWrapper = null;
                }

                if (_hasDollBroadcastApiWrapper != null)
                {
                    _hasDollBroadcastApiWrapper.Dispose();
                    _hasDollBroadcastApiWrapper = null;
                }

                if (_hasDollFullApiWrapper != null)
                {
                    _hasDollFullApiWrapper.Dispose();
                    _hasDollFullApiWrapper = null;
                }

                if (_hasInventoryFullApiWrapper != null)
                {
                    _hasInventoryFullApiWrapper.Dispose();
                    _hasInventoryFullApiWrapper = null;
                }
            }

            if (newEgo != null)
            {
                _entityGuidApiWrapper = EntityApi.GetWrapper<EntityGuidApi>(newEgo.OuterRef);
                _hasDollBroadcastApiWrapper = EntityApi.GetWrapper<HasDollBroadcastApi>(newEgo.OuterRef);
                _hasDollFullApiWrapper = EntityApi.GetWrapper<HasDollFullApi>(newEgo.OuterRef);
                _hasInventoryFullApiWrapper = EntityApi.GetWrapper<HasInventoryFullApi>(newEgo.OuterRef);

                _hasDollBroadcastApiWrapper.EntityApi.SubscribeToUsedSlotsChanged(OnUsedSlotsChanged);
                _entityGuidApiWrapper.EntityApi.SubscribeOnEntityGuid(GetPlayerGuid);

                SubscribeDollSlots(_hasDollBroadcastApiWrapper.EntityApi, _hasDollFullApiWrapper.EntityApi, _contextViewWithParams.Vmodel.Value);
                SubscribeInventorySlots(_hasInventoryFullApiWrapper.EntityApi);
            }

            var repo = NodeAccessor.Repository;
            if (repo != null)
                AsyncUtils.RunAsyncTask(SetDefaultSpecificStats);
        }

        private void GetPlayerGuid(Guid entityGuid)
        {
            PlayerGuid = entityGuid;
        }

        private void OnUsedSlotsChanged(IList<ResourceIDFull> usedSlotsIndices)
        {
            // Logger.IfDebug()?.Message(" -1- OnUsedSlotsChanged").Write();;

            if (usedSlotsIndices != null && usedSlotsIndices.Count > 0)
            {
                _usedSlotIndex = ItemHelper.TmpPlug_GetIndexBySlotResourceIdFull(usedSlotsIndices[0]);
                var selectedWeaponSlot = DollViewModels.FirstOrDefault(vm => vm.SlotId == _usedSlotIndex);
                SelectedItemSpecificStats = (selectedWeaponSlot != null && !selectedWeaponSlot.IsEmpty)
                    ? (selectedWeaponSlot.SelfSlotItem?.ItemResource as ItemResource)?.SpecificStats.Target
                    : _defaultSpecificStats;
            }
            else
            {
                SelectedItemSpecificStats = _defaultSpecificStats;
                _usedSlotIndex = -1;
            }
        }

        private void OnUsedSlotItemChanged(SlotItem slotItem)
        {
            // Logger.IfDebug()?.Message(" -2- OnUsedSlot-ITEM-Changed").Write();;

            if (slotItem.IsEmpty)
                SelectedItemSpecificStats = _defaultSpecificStats;

            var specificStats = (slotItem.ItemResource as IHasStatsResource)?.SpecificStats.Target;
            SelectedItemSpecificStats = specificStats ?? _defaultSpecificStats;
        }

        private async Task SetDefaultSpecificStats()
        {
            // Logger.IfDebug()?.Message(" -0- SetDefaultSpecificStats").Write();;

            if (GameState.Instance?.CharacterRuntimeData == null)
                return;

            var id = GameState.Instance.CharacterRuntimeData.CharacterId;
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));
            var repo = NodeAccessor.Repository;
            using (var wrapper = await repo.Get(typeId, id))
            {
                var entity = wrapper?.Get<IHasSpecificStatsClientFull>(typeId, id, ReplicationLevel.ClientFull);
                if (entity != null)
                    _defaultSpecificStats = entity.SpecificStats;
            }

            await UnityQueueHelper.RunInUnityThread(
                () =>
                {
                    if (!_defaultSpecificStats.AssertIfNull(nameof(_defaultSpecificStats)))
                    {
                        SelectedItemSpecificStats = _defaultSpecificStats;
                    }
                });
        }

        private void FindDollSlotViewModels()
        {
            var dollSlotViewModels = _dollSlotsTransform.GetComponentsInChildren<CharDollSlotViewModel>();

            if (dollSlotViewModels == null || dollSlotViewModels.Length == 0)
            {
                UI.Logger.Error(
                    $"{nameof(FindDollSlotViewModels)}() Not found {nameof(dollSlotViewModels)}",
                    gameObject);
            }
            else
            {
                foreach (var slotViewModel in dollSlotViewModels)
                {
                    slotViewModel.SetAcceptanceResolver(this);

                    var draggableItem = slotViewModel.transform.GetComponentInChildren<DraggableItem>();
                    if (!draggableItem.AssertIfNull(nameof(draggableItem)))
                        draggableItem.Init(_draggingHandler, _contextMenuViewModel);

                    slotViewModel.SetContextActionsSource(_contextMenuViewModel);
                    DollViewModels.Add(slotViewModel);
                }
            }
        }

        private void SubscribeDollSlots(HasDollBroadcastApi hasDollBroadcastApi, HasDollFullApi hasDollFullApi, IHasContextStream hasContextStream)
        {
            if (hasDollBroadcastApi.AssertIfNull(nameof(hasDollBroadcastApi)) ||
                hasDollFullApi.AssertIfNull(nameof(hasDollFullApi)))
                return;

            foreach (var dollSlotViewModel in DollViewModels)
            {
                dollSlotViewModel.Subscribe(hasDollBroadcastApi, hasDollFullApi, hasContextStream);
                dollSlotViewModel.SetWeaponSelector(_weaponSelector);
                dollSlotViewModel.SlotChanged += OnAnyDollSlotChanged;
            }
        }

        private void UnsubscribeDollSlots(HasDollBroadcastApi hasDollBroadcastApi, HasDollFullApi hasDollFullApi)
        {
            if (hasDollBroadcastApi.AssertIfNull(nameof(hasDollBroadcastApi)) ||
                hasDollFullApi.AssertIfNull(nameof(hasDollFullApi)))
                return;

            foreach (var dollSlotViewModel in DollViewModels)
            {
                dollSlotViewModel.Unsubscribe(hasDollBroadcastApi, hasDollFullApi);
                dollSlotViewModel.SlotChanged -= OnAnyDollSlotChanged;
            }
        }

        private void SubscribeInventorySlots(HasInventoryFullApi hasInventoryFullApi)
        {
            if (hasInventoryFullApi.AssertIfNull(nameof(hasInventoryFullApi)))
                return;

            _needForBlockedSlots = true;
            hasInventoryFullApi.SubscribeToCollectionSizeChanged(OnInventorySizeChanged);

            if (InventoryViewModels.Count == 0)
                AddBlockedSlotsInstances(InventorySlotsMaxCount);
        }

        private void UnsubscribeInventorySlots(HasInventoryFullApi hasInventoryFullApi)
        {
            if (hasInventoryFullApi.AssertIfNull(nameof(hasInventoryFullApi)))
                return;

            hasInventoryFullApi.UnsubscribeFromCollectionSizeChanged(OnInventorySizeChanged);
            _needForBlockedSlots = false;
            OnInventorySizeChanged(0);

            if (InventoryViewModels.Count == 0)
                AddBlockedSlotsInstances(InventorySlotsMaxCount);
        }

        private void OnInventorySizeChanged(int inventorySize)
        {
            if (inventorySize == InventorySlotsCount)
                return;

            //UI.CallerLog($"inventorySize={inventorySize}");
            InventorySlotsCount = inventorySize;
            DeleteAllBlockedSlotsInstances();
            if (inventorySize > InventoryViewModels.Count)
            {
                while (inventorySize > InventoryViewModels.Count)
                {
                    var newInventorySvm = Instantiate(_inventorySlotViewModelPrefab, _inventorySlotsTransform);
                    var index = InventoryViewModels.Count;
                    newInventorySvm.SetAcceptanceResolver(this);

                    var draggableItem = newInventorySvm.transform.GetComponentInChildren<DraggableItem>();
                    if (!draggableItem.AssertIfNull(nameof(draggableItem)))
                        draggableItem.Init(_draggingHandler, _contextMenuViewModel);

                    newInventorySvm.name = $"InventorySlotViewModel{index}";
                    newInventorySvm.SetSlotId(index);
                    newInventorySvm.SetContextActionsSource(_contextMenuViewModel);
                    InventoryViewModels.Add(newInventorySvm);
                    newInventorySvm.Subscribe(_hasInventoryFullApiWrapper.EntityApi, _contextViewWithParams.Vmodel.Value);
                    //Подписываемся на изменения слота не через _hasInventoryFullApiWrapper, а непосредственно из слота,
                    //чтобы гарантировать, что в момент события слот всегда находится в актуальном состоянии
                    newInventorySvm.SlotChanged += OnAnyInventorySlotChanged;
                }
            }
            else
            {
                while (inventorySize < InventoryViewModels.Count)
                {
                    var index = InventoryViewModels.Count - 1;
                    var removedSlotViewModel = InventoryViewModels[index];
                    removedSlotViewModel.Unsubscribe(_hasInventoryFullApiWrapper.EntityApi);
                    removedSlotViewModel.SlotChanged -= OnAnyInventorySlotChanged;
                    InventoryViewModels.Remove(removedSlotViewModel);
                    Destroy(removedSlotViewModel.gameObject);
                }
            }

            if (_needForBlockedSlots)
                AddBlockedSlotsInstances(InventorySlotsMaxCount - inventorySize);

            InventoryEmptySlotsCount = GetInventoryEmptySlotsCount();
        }

        private void OnAnyInventorySlotChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            if (slotViewModel.AssertIfNull(nameof(slotViewModel)))
                return;

            InventoryEmptySlotsCount = GetInventoryEmptySlotsCount();
            InventoryItemsCount = InventorySlotsCount - InventoryEmptySlotsCount;

            ApplyFilterToInventoryViewModel(slotViewModel as CharInvSlotViewModel);
            CharacterItemsChanged?.Invoke(slotViewModel, stackDelta);
            if (stackDelta > 0 && !IsItMoveFromDoll(slotViewModel.ItemResource))
            {
                //UI.CallerLog($"--- ItemPickupEvent for {slotItem} delta={stackDelta}"); //DEBUG
                SoundControl.Instance.ItemPickupEvent?.Post(transform.root.gameObject);
            }
        }

        private int GetInventoryEmptySlotsCount()
        {
            return InventoryViewModels.Sum(vm => vm.Stack == 0 ? 1 : 0);
        }

        private void OnAnyDollSlotChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            if (slotViewModel.AssertIfNull(nameof(slotViewModel)))
                return;

            CharacterItemsChanged?.Invoke(slotViewModel, stackDelta);
            if (slotViewModel.SlotId == _usedSlotIndex)
                OnUsedSlotItemChanged(slotViewModel.SelfSlotItem);
        }

        private void DeleteAllBlockedSlotsInstances()
        {
            while (_blockedSlots.Count > 0)
            {
                var removed = _blockedSlots[_blockedSlots.Count - 1];
                _blockedSlots.RemoveAt(_blockedSlots.Count - 1);
                if (removed != null)
                    Destroy(removed.gameObject);
            }
        }

        private void AddBlockedSlotsInstances(int count)
        {
            if (count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                var newBlocked = Instantiate(_blockedSlotPrefab, _inventorySlotsTransform);
                newBlocked.name = $"BlockedSlot{i + 1}";
                _blockedSlots.Add(newBlocked);
            }
        }

        private void OnFiltrableTypeFilterChanged(InventoryFiltrableTypeDef inventoryFiltrableTypeDef)
        {
            CurrentInventoryFiltrableTypeDef = _inventoryFilterViewModel.CurrentFiltrableTypeFilter;
            DoFiltering();
        }

        private void ApplyFilterToInventoryViewModel(CharInvSlotViewModel charInvSlotViewModel)
        {
            if (charInvSlotViewModel.AssertIfNull(nameof(charInvSlotViewModel)))
                return;

            charInvSlotViewModel.IsVisible = CurrentInventoryFiltrableTypeDef == null ||
                                             charInvSlotViewModel.IsEmpty ||
                                             (charInvSlotViewModel.ItemResource as ItemResource)?.InventoryFiltrableType == CurrentInventoryFiltrableTypeDef;
        }

        private void DoFiltering()
        {
            foreach (var vm in InventoryViewModels)
                ApplyFilterToInventoryViewModel(vm);
        }

        private void SaveLastMovedFromDollItemResource(SlotViewModel fromSvm, SlotViewModel toSvm)
        {
            _lastMovedFromSelfResources.Clear();
            var isFromDoll = fromSvm is CharDollSlotViewModel;
            var isToDoll = toSvm is CharDollSlotViewModel;
            if (isFromDoll == isToDoll)
            {
                if (isToDoll) //действия в только кукле
                    return;

                if (!fromSvm.IsEmpty)
                    _lastMovedFromSelfResources.Add(fromSvm.ItemResource);
            }

            if (isToDoll && !toSvm.IsEmpty) //перетаскиваем что-то в куклу на непустой слот
            {
                _lastMovedFromSelfResources.Add(toSvm.ItemResource);
                return;
            }

            _lastMovedFromSelfResources.Add(fromSvm.ItemResource);

            if (!isFromDoll && !toSvm.IsEmpty)
                _lastMovedFromSelfResources.Add(toSvm.ItemResource);
        }

        private bool IsItMoveFromDoll(BaseItemResource itemResource)
        {
            if (!_lastMovedFromSelfResources.Contains(itemResource))
                return false;

            _lastMovedFromSelfResources.Remove(itemResource);
            return true;
        }
    }
}