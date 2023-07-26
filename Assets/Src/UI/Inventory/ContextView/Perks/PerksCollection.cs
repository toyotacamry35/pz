using System.Collections.Generic;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using Assets.Src.SpawnSystem;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using Uins.Slots;
using UnityEngine;

namespace Uins
{
    public delegate void ItemsCountsDelegate(int[] perksCounts, int[] totalSlotsCounts, int[] emptySlotsCounts);

    public class PerksCollection<VM, T, U> : BindingVmodel, IItemsCountsSource
        where VM : PerkCollectionApiSlotViewModel<T, U>
        where T : PerksBaseFullApi, new()
        where U : PerkSlotsBaseFullApi, new()
    {
        public event ItemsCountsDelegate ItemsCountsChanged;

        public event SlotItemOperationDelegate PerkAdded;

        protected Transform ParentTransform;
        protected List<VM> Collection = new List<VM>();

        private VM _itemPrefab;

        private PerksContextView _perksContextView;
        private DraggingHandler _draggingHandler;
        private ISlotAcceptanceResolver _slotAcceptanceResolver;
        private Transform _dragTopTransform;
        private ISortByItemTypeResolver _sortByItemTypeResolver;
        private PerksCollectionSortViewModel _perksCollectionSortViewModel;
        private PerkSlotTypes _perkSlotTypes;

        private int _sortingOrder;
        private static int _baseSortIndexCounter; //для каждого варианта generic-класса будет свой счетчик, так и задумано

        private int[] _perksCounts = new int[3];
        private int[] _totalSlotsCounts = new int[3];
        private int[] _emptySlotsCounts = new int[3];

        private AK.Wwise.Event _perkAddedSoundEvent;
        private AK.Wwise.Event _perkSelectionSoundEvent;
        private float _lastPawnChangeTime;
        private EntityApiWrapper<T> _perksWrapper;
        private EntityApiWrapper<U> _perkSlotsWrapper;


        //=== Props ===========================================================

        public int[] TotalSlotsCounts => _totalSlotsCounts;

        protected virtual bool DoSortEmptyPerks => true;

        protected virtual bool NeedForPerkSlotTypeSubscribe => false;


        //=== Public ==========================================================

        public void Init(ISlotAcceptanceResolver slotAcceptanceResolver, IPawnSource pawnSource, VM itemPrefab, Transform parentTransform,
            PerksContextView perksContextView, DraggingHandler draggingHandler, Transform dragTopTransform,
            ISortByItemTypeResolver sortByItemTypeResolver, PerksCollectionSortViewModel perksCollectionSortViewModel,
            PerkSlotTypes perkSlotTypes, AK.Wwise.Event perkAddedSoundEvent, AK.Wwise.Event perkSelectionSoundEvent)
        {
            if (pawnSource.AssertIfNull(nameof(pawnSource)) ||
                itemPrefab.AssertIfNull(nameof(itemPrefab)) ||
                parentTransform.AssertIfNull(nameof(parentTransform)) ||
                perksContextView.AssertIfNull(nameof(perksContextView)) ||
                draggingHandler.AssertIfNull(nameof(draggingHandler)) ||
                slotAcceptanceResolver.AssertIfNull(nameof(slotAcceptanceResolver)) ||
                dragTopTransform.AssertIfNull(nameof(dragTopTransform)) ||
                sortByItemTypeResolver.AssertIfNull(nameof(sortByItemTypeResolver)) ||
                perksCollectionSortViewModel.AssertIfNull(nameof(perksCollectionSortViewModel)) ||
                perkSlotTypes.AssertIfNull(nameof(perkSlotTypes)) ||
                perkAddedSoundEvent.AssertIfNull(nameof(perkAddedSoundEvent)) ||
                perkSelectionSoundEvent.AssertIfNull(nameof(perkSelectionSoundEvent)))
                return;

            _slotAcceptanceResolver = slotAcceptanceResolver;
            _draggingHandler = draggingHandler;
            _itemPrefab = itemPrefab;
            ParentTransform = parentTransform;
            _perksContextView = perksContextView;
            _dragTopTransform = dragTopTransform;
            _sortByItemTypeResolver = sortByItemTypeResolver;
            _perksCollectionSortViewModel = perksCollectionSortViewModel;
            _sortingOrder = _perksCollectionSortViewModel.ToggleGroupWithIndex.SelectedIndex;
            _perksCollectionSortViewModel.ToggleGroupWithIndex.OnIndexChanged += OnSortingOnIndexChanged;
            _perksCollectionSortViewModel.Init(this);
            _perkSlotTypes = perkSlotTypes;
            _perkAddedSoundEvent = perkAddedSoundEvent;
            _perkSelectionSoundEvent = perkSelectionSoundEvent;
            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        public void OnSortingOnIndexChanged(int sortingOrder)
        {
            if (_sortingOrder == sortingOrder)
                return;

            _sortingOrder = sortingOrder;
            SortCollection();
        }


        //=== Protected =======================================================

        protected virtual void OnPerksSizeChanged(int collectionSize)
        {
            while (collectionSize < Collection.Count)
            {
                var removeIndex = Collection.Count - 1;
                var removedPerkSvm = Collection[removeIndex];
                OnRemovePerkSvm(removedPerkSvm);
                Collection.RemoveAt(removeIndex);
                Object.Destroy(removedPerkSvm.gameObject);
            }

            while (collectionSize > Collection.Count)
            {
                var newPerkSvm = Object.Instantiate(_itemPrefab, ParentTransform);
                OnAddPerkSvm(newPerkSvm);
                Collection.Add(newPerkSvm);
            }

            SortCollection();
        }

        protected virtual void OnAddPerkSvm(VM newPerkSvm)
        {
            var index = Collection.Count;
            newPerkSvm.name = $"{_itemPrefab.name}{index}";
            newPerkSvm.SetSlotId(index);
            newPerkSvm.PerkSlotTypes = _perkSlotTypes;
            newPerkSvm.NeedForPerkSlotTypeSubscribe = NeedForPerkSlotTypeSubscribe;
            newPerkSvm.Subscribe(_perksWrapper.EntityApi, _perkSlotsWrapper.EntityApi, _perksContextView);
            newPerkSvm.SetAcceptanceResolver(_slotAcceptanceResolver);
            newPerkSvm.SetBaseSortIndex(_baseSortIndexCounter++);
            newPerkSvm.DraggableItem.Init(_draggingHandler, null, _dragTopTransform);
            newPerkSvm.ItemChanged += OnAnyItemChanged;
            newPerkSvm.ItemBecomeSelected += OnAnyItemBecomeSelected;
            newPerkSvm.SubscribeOnSomePerkDrag(_draggingHandler, true);
            if (!newPerkSvm.IsEmpty)
                OnAnyItemChanged(index, newPerkSvm.SelfSlotItem, newPerkSvm.Stack);
        }

        protected virtual void OnRemovePerkSvm(VM removedPerkSvm)
        {
            removedPerkSvm.Unsubscribe(_perksWrapper.EntityApi, _perkSlotsWrapper.EntityApi);
            removedPerkSvm.ItemChanged -= OnAnyItemChanged;
            removedPerkSvm.ItemBecomeSelected -= OnAnyItemBecomeSelected;
            removedPerkSvm.SubscribeOnSomePerkDrag(_draggingHandler, false);
            removedPerkSvm.DraggableItem.Release();
        }

        protected virtual void OnAnyItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
        {
            UpdateSlotsCounts();
            ItemsCountsChanged?.Invoke(_perksCounts, _totalSlotsCounts, _emptySlotsCounts);
            SortCollection();
            if (stackDelta > 0)
                OnAnyPerkAdded(slotIndex, slotItem);
        }


        //=== Private =========================================================

        private void SortCollection()
        {
            var sortedList = new SortedList<int, VM>();
            foreach (var vm in Collection)
            {
                if (!DoSortEmptyPerks && vm.IsEmpty)
                    continue;

                var sortedIndex = vm.GetSortIndex(_sortingOrder, _sortByItemTypeResolver);
                while (sortedList.ContainsKey(sortedIndex))
                {
                    sortedIndex++;
                }

                sortedList.Add(sortedIndex, vm);
            }

            int i = 0;
            foreach (var vm in sortedList.Values)
                vm.transform.SetSiblingIndex(i++);
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            _lastPawnChangeTime = Time.time;
            if (prevEgo != null)
            {
                _perksWrapper.EntityApi.UnsubscribeFromCollectionSizeChanged(OnPerksSizeChanged);
                OnPerksSizeChanged(0);

                _perksWrapper.Dispose();
                _perksWrapper = null;
                _perkSlotsWrapper.Dispose();
                _perkSlotsWrapper = null;
            }

            if (newEgo != null)
            {
                _perksWrapper = EntityApi.GetWrapper<T>(newEgo.OuterRef);
                _perkSlotsWrapper = EntityApi.GetWrapper<U>(newEgo.OuterRef);

                _perksWrapper.EntityApi.SubscribeToCollectionSizeChanged(OnPerksSizeChanged);
            }
        }

        private void OnAnyItemBecomeSelected()
        {
            _perkSelectionSoundEvent.Post(_perksContextView.gameObject);
        }

        private void UpdateSlotsCounts()
        {
            ClearCounts(_perksCounts);
            ClearCounts(_totalSlotsCounts);
            ClearCounts(_emptySlotsCounts);
            foreach (var vm in Collection)
            {
                if (vm.ItemType != null && !vm.IsEmpty)
                    IncreaseCounts(_perksCounts, vm.ItemType);

                if (vm.PerkSlotType != null)
                {
                    if (vm.IsEmpty)
                        IncreaseCounts(_emptySlotsCounts, vm.PerkSlotType);

                    IncreaseCounts(_totalSlotsCounts, vm.PerkSlotType);
                }
            }
        }

        private void ClearCounts(int[] counts)
        {
            for (int i = 0; i < counts.Length; i++)
                counts[i] = 0;
        }

        private void IncreaseCounts(int[] counts, ItemTypeResource itemType)
        {
            var types = _perkSlotTypes.ItemTypes;
            for (int i = 0; i < types.Length; i++)
            {
                if (itemType == types[i])
                {
                    counts[i] += 1;
                    break;
                }
            }
        }

        private void OnAnyPerkAdded(int slotIndex, SlotItem slotItem)
        {
            PerkAdded?.Invoke(slotIndex, slotItem);
            _perkAddedSoundEvent.Post(_perksContextView.gameObject);
        }
    }
}