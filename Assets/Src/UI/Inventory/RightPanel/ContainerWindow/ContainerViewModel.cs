using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Src.Aspects;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using Assets.Src.Shared.Impl;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using Uins.Slots;
using JetBrains.Annotations;
using SharedCode.Entities;
using UnityEngine;
using UnityWeld.Binding;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using UnityEngine.Serialization;
using Transform = UnityEngine.Transform;

namespace Uins
{
    [Binding]
    public class ContainerViewModel : BindingViewModel, ISlotAcceptanceResolver
    {
        [SerializeField, UsedImplicitly]
        private OurCharacterSlotsViewModel _ourCharacterSlotsViewModel;

        [SerializeField, UsedImplicitly]
        private Template _playerStatsTemplate;

        [SerializeField, UsedImplicitly]
        private ContainerSlotViewModel _containerSlotViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _slotsTransform;

        [SerializeField, UsedImplicitly]
        private DraggingHandler _draggingHandler;

        [SerializeField, UsedImplicitly]
        private InventoryContextMenuViewModel _contextMenuViewModel;

        [SerializeField, UsedImplicitly, FormerlySerializedAs("_contextView")]
        private ContextViewWithParams _contextViewWithParams;

        private PlayerMainStatsViewModel _playerMainStatsViewModel;
        private ContainerMode _containerMode;

        private readonly string[] _playerMainStatsViewModelControlledProps =
        {
            nameof(PlayerMainStatsViewModel.ItemsTotalWeight),
            nameof(PlayerMainStatsViewModel.InventoryMaxWeight)
        };

        private IHasEntityApi _sizeWatchingApiWrapper;


        //=== Props ===============================================================

        public OuterRef<IEntityObject> TargetOuterRef { get; set; }

        public Guid PlayerGuid => _ourCharacterSlotsViewModel.PlayerGuid;

        public List<ContainerSlotViewModel> ContainerSlotViewModels { get; set; } = new List<ContainerSlotViewModel>();

        public List<ContainerSlotViewModel> MarkedToTakeSlotViewModels { get; set; } = new List<ContainerSlotViewModel>();

        private float _selectedItemsWeight;

        [Binding]
        public float SelectedItemsWeight
        {
            get => _selectedItemsWeight;
            set
            {
                if (!Mathf.Approximately(_selectedItemsWeight, value))
                {
                    _selectedItemsWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _itemsWeight;

        [Binding]
        public float ItemsWeight
        {
            get => _itemsWeight;
            set
            {
                if (!Mathf.Approximately(_itemsWeight, value))
                {
                    _itemsWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _selectedAndTotalItemsWeight;

        [Binding]
        public float SelectedAndTotalItemsWeight
        {
            get => _selectedAndTotalItemsWeight;
            set
            {
                if (!Mathf.Approximately(_selectedAndTotalItemsWeight, value))
                {
                    _selectedAndTotalItemsWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [Binding]
        public bool IsSelectedOverWeight => SelectedOverWeight > 0;

        private float _selectedOverWeight;

        [Binding]
        public float SelectedOverWeight
        {
            get => _selectedOverWeight;
            set
            {
                if (!Mathf.Approximately(_selectedOverWeight, value))
                {
                    _selectedOverWeight = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsSelectedOverWeight));
                }
            }
        }

        [Binding]
        public bool IsSelectedItems => SelectedItemsCount > 0;

        private int _selectedAndInventoryItemsCount;

        [Binding]
        public int SelectedAndInventoryItemsCount
        {
            get => _selectedAndInventoryItemsCount;
            set
            {
                if (_selectedAndInventoryItemsCount != value)
                {
                    _selectedAndInventoryItemsCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _selectedItemsCount;

        [Binding]
        public int SelectedItemsCount
        {
            get => _selectedItemsCount;
            set
            {
                if (_selectedItemsCount != value)
                {
                    _selectedItemsCount = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsSelectedItems));
                }
            }
        }

        [Binding]
        public bool IsAnyItems => ItemsCount > 0;

        private int _itemsCount;

        [Binding]
        public int ItemsCount
        {
            get => _itemsCount;
            set
            {
                if (_itemsCount != value)
                {
                    _itemsCount = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsAnyItems));
                }
            }
        }

        [Binding]
        public bool IsSelectedOverCount => SelectedOverCount > 0;

        private int _selectedOverCount;

        [Binding]
        public int SelectedOverCount
        {
            get => _selectedOverCount;
            set
            {
                if (_selectedOverCount != value)
                {
                    _selectedOverCount = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsSelectedOverCount));
                }
            }
        }


        //=== Unity ==============================================================

        private void Awake()
        {
            _ourCharacterSlotsViewModel.AssertIfNull(nameof(_ourCharacterSlotsViewModel));
            _playerStatsTemplate.AssertIfNull(nameof(_playerStatsTemplate));
            _containerSlotViewModelPrefab.AssertIfNull(nameof(_containerSlotViewModelPrefab));
            _slotsTransform.AssertIfNull(nameof(_slotsTransform));
            _draggingHandler.AssertIfNull(nameof(_draggingHandler));
            _contextMenuViewModel.AssertIfNull(nameof(_contextMenuViewModel));
            _contextViewWithParams.AssertIfNull(nameof(_contextViewWithParams));

            _ourCharacterSlotsViewModel.CharacterItemsChanged += OnCharacterAnyItemsChanged;
        }


        //=== Public ==============================================================

        public void Init(PlayerMainStatsViewModel playerMainStatsViewModel)
        {
            _playerStatsTemplate.InitChildBindings(playerMainStatsViewModel);
            _playerMainStatsViewModel = playerMainStatsViewModel;
            if (!_playerMainStatsViewModel.AssertIfNull(nameof(_playerMainStatsViewModel)))
                _playerMainStatsViewModel.PropertyChanged += OnPlayerMainStatsViewModelPropertyChanged;
        }

        public void SubscribeSlots(OuterRef<IEntityObject> targetOuterRef, ContainerMode containerMode)
        {
            if (!targetOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message("targetOuterRef is invalid").Write();
                return;
            }

            _containerMode = containerMode;
            TargetOuterRef = targetOuterRef;
            switch (containerMode)
            {
                case ContainerMode.Bank:
                {
                    _sizeWatchingApiWrapper = EntityApi.GetWrapper<HasBankBroadcastApi>(TargetOuterRef, true);
                    break;
                }

                case ContainerMode.Inventory:
                {
                    _sizeWatchingApiWrapper = EntityApi.GetWrapper<HasInventoryFullApi>(TargetOuterRef, true);
                    break;
                }

                default:
                {
                    throw new Exception($"Unexpected {nameof(_containerMode)}: {_containerMode}");
                }
            }

            (_sizeWatchingApiWrapper.Api as SizeWatchingSlotsCollectionApi)?.SubscribeToCollectionSizeChanged(OnInventorySizeChanged);

            if (EntitiesRepository.IsImplements<IWorldCorpse>(targetOuterRef.TypeId))
            {
                var targetGameObject = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(targetOuterRef);
                if (targetGameObject != null)
                {
                    var selfCorpseNotifier = targetGameObject.transform.GetComponent<SelfCorpseNotifier>();
                    if (selfCorpseNotifier != null)
                        selfCorpseNotifier.RemoveNavigationPointMarker();

                    // AsyncUtilsUnity.RunAsyncTaskFromUnity(
                    //     async () => { await ClusterCommands.MoveAllDollItemsToInventory(_ourCharacterSlotsViewModel.PlayerGuid, targetOuterRef.Guid); },
                    //     NodeAccessor.Repository).WrapErrors();
                }
            }
        }

        public void UnsubscribeSlots()
        {
            if (!TargetOuterRef.IsValid)
            {
                UI.Logger.IfError()?.Message($"{nameof(TargetOuterRef)} is invalid").Write();
                return;
            }

            var sizeWatchingSlotsCollectionApi = _sizeWatchingApiWrapper.Api as SizeWatchingSlotsCollectionApi;
            if (!sizeWatchingSlotsCollectionApi.AssertIfNull(nameof(sizeWatchingSlotsCollectionApi)))
                sizeWatchingSlotsCollectionApi.UnsubscribeFromCollectionSizeChanged(OnInventorySizeChanged);
            while (ContainerSlotViewModels.Count > 0)
            {
                var containerSvm = ContainerSlotViewModels.First();

                var draggableItem = containerSvm.GetComponentInChildren<DraggableItem>();
                if (!draggableItem.AssertIfNull(nameof(draggableItem)))
                    draggableItem.Release();

                containerSvm.Unsubscribe(sizeWatchingSlotsCollectionApi);
                containerSvm.PropertyChanged -= OnAnySlotPropertyChanged;
                containerSvm.SlotChanged -= OnAnySlotChanged;

                ContainerSlotViewModels.Remove(containerSvm);
                Destroy(containerSvm.gameObject);
            }

            _containerMode = ContainerMode.None;
            TargetOuterRef = default;
            _sizeWatchingApiWrapper.Dispose();
            _sizeWatchingApiWrapper = null;
        }

        [UsedImplicitly]
        public void TakeSelectedItems()
        {
            ClusterCommands.TakeItemsFromContainer_OnClient(
                _ourCharacterSlotsViewModel.PlayerGuid,
                MarkedToTakeSlotViewModels.Select(csvm => (OuterBaseSlotViewModel) csvm).ToList());
        }

        [UsedImplicitly]
        public void TakeAllItems()
        {
            ClusterCommands.TakeAllItemsFromContainer_OnClient(
                _ourCharacterSlotsViewModel.PlayerGuid,
                ContainerSlotViewModels[0].SlotsCollectionApi.CollectionPropertyAddress);
        }

        public int TryMoveTo(SlotViewModel fromSvm, SlotViewModel toSvm, bool doMove = false, bool isCounterSwapCheck = false)
        {
            var toContainerSlotViewModel = toSvm as ContainerSlotViewModel;

            if (toContainerSlotViewModel == null)
            {
                UI.Logger.Error(
                    $"{nameof(TryMoveTo)}(from={fromSvm}, to={toSvm}, do{doMove.AsSign()}) " +
                    $"{nameof(toSvm)} isn't {nameof(ContainerSlotViewModel)}");
                return 0;
            }

            var acceptedStack = fromSvm.Stack;

            //проверка возможности переместить содержимое toSvm во fromSvm
            if (acceptedStack > 0 &&
                !toSvm.IsEmpty &&
                !isCounterSwapCheck && //против зацикливания
                fromSvm.SlotAcceptanceResolver.TryMoveTo(toSvm, fromSvm, false, true) <= 0)
                acceptedStack = 0;

            if (acceptedStack > 0 && doMove && !isCounterSwapCheck)
            {
                ClusterCommands.MoveItem_OnClient(PlayerGuid, fromSvm, toContainerSlotViewModel, acceptedStack);
                if (toSvm.IsEmpty && fromSvm is CharDollSlotViewModel)
                    InventoryContextMenuViewModel.DoEquipmentSound(false, fromSvm, gameObject);
            }

            return acceptedStack;
        }

        public bool TryToDropFrom(SlotViewModel fromSvm)
        {
            return false;
        }


        //=== Private =============================================================

        private void OnInventorySizeChanged(int inventorySize)
        {
            if (ContainerSlotViewModels.Count > 0)
                EmergencyRemoveContainerSlotViewModels();

            if (inventorySize <= 0)
            {
                UI.Logger.IfError()?.Message($"Unexpected container size: {inventorySize}").Write();
                return;
            }

            var sizeWatchingSlotsCollectionApi = _sizeWatchingApiWrapper.Api as SizeWatchingSlotsCollectionApi;
            if (sizeWatchingSlotsCollectionApi.AssertIfNull(nameof(sizeWatchingSlotsCollectionApi)))
                return;

            for (int i = 0; i < inventorySize; i++)
            {
                var containerSvm = Instantiate(_containerSlotViewModelPrefab, _slotsTransform);
                var index = ContainerSlotViewModels.Count;
                containerSvm.SetAcceptanceResolver(this);
                containerSvm.name = $"{nameof(ContainerSlotViewModel)}{index}";
                containerSvm.SetSlotId(index);
                containerSvm.SetListOfMarkedToTake(MarkedToTakeSlotViewModels);
                ContainerSlotViewModels.Add(containerSvm);

                var draggableItem = containerSvm.transform.GetComponentInChildren<DraggableItem>();
                if (!draggableItem.AssertIfNull(nameof(draggableItem)))
                    draggableItem.Init(_draggingHandler, _contextMenuViewModel);

                containerSvm.ContainerMode = _containerMode;
                containerSvm.Subscribe(sizeWatchingSlotsCollectionApi, _contextViewWithParams.Vmodel.Value);
                containerSvm.PropertyChanged += OnAnySlotPropertyChanged;
                //Подписываемся на изменения слота не через _hasInventoryFullApiWrapper, а непосредственно из слота,
                //чтобы гарантировать, что в момент события слот всегда находится в актуальном состоянии
                containerSvm.SlotChanged += OnAnySlotChanged;
            }

            ItemsCount = GetItemsCount();
        }

        private void EmergencyRemoveContainerSlotViewModels()
        {
            UI.Logger.IfError()?.Message($"{nameof(EmergencyRemoveContainerSlotViewModels)} count={ContainerSlotViewModels.Count}").Write();
            while (ContainerSlotViewModels.Count > 0)
            {
                var containerSvm = ContainerSlotViewModels.First();
                ContainerSlotViewModels.Remove(containerSvm);
                Destroy(containerSvm.gameObject);
            }
        }

        private void OnAnySlotPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ContainerSlotViewModel.IsMarkedToTake))
                return;
            OnMarkedToTakeItemsChanged();
        }

        private void OnAnySlotChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            if (slotViewModel.AssertIfNull(nameof(slotViewModel)))
                return;

            if (slotViewModel.Stack == 0)
            {
                var selected = MarkedToTakeSlotViewModels.FirstOrDefault(svm => svm == slotViewModel);
                if (selected != null)
                {
                    MarkedToTakeSlotViewModels.Remove(selected);
                    OnMarkedToTakeItemsChanged();
                }
            }

            ItemsWeight = GetItemsWeight();
            ItemsCount = GetItemsCount();
        }

        private void OnPlayerMainStatsViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_playerMainStatsViewModelControlledProps.Contains(e.PropertyName))
                return;
            SelectedAndTotalItemsWeight = GetSelectedAndTotalItemsWeight();
            SelectedOverWeight = GetSelectedOverWeight();
        }

        private void OnMarkedToTakeItemsChanged()
        {
            SelectedItemsCount = MarkedToTakeSlotViewModels.Count;
            SelectedAndInventoryItemsCount = GetSelectedAndInventoryItemsCount();
            SelectedOverCount = GetSelectedOverCount();
            SelectedItemsWeight = GetSelectedItemsWeight();
            SelectedAndTotalItemsWeight = GetSelectedAndTotalItemsWeight();
            SelectedOverWeight = GetSelectedOverWeight();
        }

        private void OnCharacterAnyItemsChanged(SlotViewModel slotViewModel, int stackDelta)
        {
            SelectedAndInventoryItemsCount = GetSelectedAndInventoryItemsCount();
            SelectedOverCount = GetSelectedOverCount();
            SelectedAndTotalItemsWeight = GetSelectedAndTotalItemsWeight();
            SelectedOverWeight = GetSelectedOverWeight();
        }

        private int GetSelectedAndInventoryItemsCount()
        {
            return SelectedItemsCount + _ourCharacterSlotsViewModel.InventoryItemsCount;
        }

        private int GetSelectedOverCount()
        {
            return SelectedAndInventoryItemsCount - _ourCharacterSlotsViewModel.InventorySlotsCount;
        }

        private float GetSelectedItemsWeight()
        {
            return MarkedToTakeSlotViewModels.Sum(svm => svm.Stack * (svm.ItemResource != null ? svm.ItemResource.Weight : 0));
        }

        private int GetItemsCount()
        {
            return ContainerSlotViewModels.Count(svm => svm.Stack > 0);
        }

        private float GetItemsWeight()
        {
            return ContainerSlotViewModels.Sum(svm => svm.Stack * (svm.ItemResource != null ? svm.ItemResource.Weight : 0));
        }

        private float GetSelectedAndTotalItemsWeight()
        {
            return SelectedItemsWeight + _playerMainStatsViewModel.ItemsTotalWeight;
        }

        private float GetSelectedOverWeight()
        {
            if (_playerMainStatsViewModel == null)
                return 0;
            return SelectedAndTotalItemsWeight - _playerMainStatsViewModel.InventoryMaxWeight;
        }
    }
}