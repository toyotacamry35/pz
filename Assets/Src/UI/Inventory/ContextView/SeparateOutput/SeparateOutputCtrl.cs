using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using Assets.Src.ResourceSystem.L10n;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using Uins.Slots;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class SeparateOutputCtrl : BindingController<SeparateOutputVM>, ISlotAcceptanceResolver
    {
        private const int OutputContainerMaxDisplayedCount = 16;

        [SerializeField, UsedImplicitly]
        protected DraggingHandler DraggingHandler;

        [SerializeField, UsedImplicitly]
        protected InventoryContextMenuViewModel ContextMenuViewModel;

        [SerializeField, UsedImplicitly]
        protected OurCharacterSlotsViewModel OurCharacterSlotsViewModel;

        [SerializeField, UsedImplicitly]
        private Transform OutSlotsTransform;

        [SerializeField, UsedImplicitly]
        private SeparateOutputSlotViewModel SeparateOutputSvmPrefab;

        [SerializeField, UsedImplicitly]
        private Transform BlockedSlotPrefab;

        [SerializeField, UsedImplicitly]
        private Transform DragContainer;

        [SerializeField, UsedImplicitly]
        private ContextViewWithParams _contextViewWithParams;

        public List<SeparateOutputSlotViewModel> OutSlotViewModels { get; set; } = new List<SeparateOutputSlotViewModel>();

        private readonly ListStream<SlotItem> _outputSlots = new ListStream<SlotItem>();

        private readonly List<Transform> _blockedSlots = new List<Transform>();

        private EntityApiWrapper<SeparateOutputFullApi> _outputFullApiWrapper;


        //=== Props ===========================================================

        [Binding, UsedImplicitly]
        public bool IsTakeAllItemsEnabled { get; private set; }

        [Binding, UsedImplicitly]
        public bool Active { get; private set; }

        public OuterRef<IEntityObject> TargetOuterRef { get; private set; }

        private Guid PlayerGuid => OurCharacterSlotsViewModel.PlayerGuid;


        //=== Unity ===========================================================

        private void Awake()
        {
            foreach (Transform child in OutSlotsTransform)
                Destroy(child.gameObject);

            Vmodel.SubStream(D, vm => vm.InventoryNode.CraftSourceRp)
                .Action(D, craftSourceVM => { SubscribeSlots(craftSourceVM?.TargetOuterRef ?? default); });

            var hasOutItemsStream = _outputSlots.ListChanges(D).Func(D, lst => lst.Any(si => si != null && !si.IsEmpty));
            Bind(hasOutItemsStream, () => IsTakeAllItemsEnabled);

            if (!_contextViewWithParams.AssertIfNull(nameof(_contextViewWithParams)))
                Bind(_contextViewWithParams.IsMachineTabOpenedRp, () => Active);

            Vmodel.Func(D, vm => vm.SurvivalGui)
                .SubStream(D, node => node.PawnChangesStream)
                .Action(D, OnOurPawnChanged);
        }


        //=== Public ==========================================================

        [UsedImplicitly]
        public void OnTakeAllItemsButton()
        {
            Assets.Src.Aspects.ClusterCommands.TakeAllItemsFromContainer_OnClient(
                PlayerGuid,
                OutSlotViewModels[0].SlotsCollectionApi.CollectionPropertyAddress
            );
        }

        public int TryMoveTo(SlotViewModel fromSvm, SlotViewModel toSvm, bool doMove = false, bool isCounterSwapCheck = false)
        {
            var toMachineSvm = toSvm as OuterBaseSlotViewModel;
            if (toMachineSvm == null)
            {
                UI.Logger.Error(
                    $"{nameof(TryMoveTo)}(from={fromSvm}, to={toSvm}, do{doMove.AsSign()}) " +
                    $"{nameof(toSvm)} isn't {nameof(OuterBaseSlotViewModel)}");
                return 0;
            }

            var acceptedStack = 0;
            if (fromSvm is OuterBaseSlotViewModel && fromSvm.SlotCollectionType == toMachineSvm.SlotCollectionType)
            {
                //в пределах того же контейнера
                acceptedStack = fromSvm.SlotCollectionType == SlotViewModel.CollectionType.Output ? 0 : fromSvm.Stack;
            }
            else
            {
                switch (toMachineSvm.SlotCollectionType)
                {
                    case SlotViewModel.CollectionType.Output:
                        acceptedStack = 0;
                        break;
                    default:
                        //в инвентарь автостанка пока все можно
                        acceptedStack = fromSvm.Stack;
                        break;
                }
            }

            //проверка возможности переместить содержимое toSvm во fromSvm
            if (acceptedStack > 0 &&
                !toSvm.IsEmpty &&
                !isCounterSwapCheck && //против зацикливания
                fromSvm.SlotAcceptanceResolver.TryMoveTo(toSvm, fromSvm, false, true) <= 0)
                acceptedStack = 0;

            if (acceptedStack > 0 && doMove && !isCounterSwapCheck)
            {
                Assets.Src.Aspects.ClusterCommands.MoveItem_OnClient(PlayerGuid, fromSvm, toMachineSvm, acceptedStack);
                if (toSvm.IsEmpty && fromSvm is CharDollSlotViewModel)
                    InventoryContextMenuViewModel.DoEquipmentSound(false, fromSvm, gameObject);
            }

            return acceptedStack;
        }

        public bool TryToDropFrom(SlotViewModel fromSvm)
        {
            return false;
        }


        //=== Private =========================================================

        private void SubscribeSlots(OuterRef<IEntityObject> targetOuterRef)
        {
            UnsubscribeSlots();

            if (!targetOuterRef.IsValid)
                return;

            TargetOuterRef = targetOuterRef;

            _outputFullApiWrapper = EntityApi.GetWrapper<SeparateOutputFullApi>(TargetOuterRef);
            _outputFullApiWrapper.EntityApi.SubscribeToCollectionSizeChanged(OnOutputCollectionSize);
        }

        private void UnsubscribeSlots()
        {
            _outputFullApiWrapper?.EntityApi.UnsubscribeFromCollectionSizeChanged(OnOutputCollectionSize);
            OnOutputCollectionSize(0);

            _outputFullApiWrapper?.Dispose();
            _outputFullApiWrapper = null;
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
                SubscribeSlots(default);
        }

        private void OnOutputCollectionSize(int collectionSize)
        {
            var size = Mathf.Min(collectionSize, OutputContainerMaxDisplayedCount);
            var blockedCount = Math.Max(0, OutputContainerMaxDisplayedCount - size);

            SetSlotViewModels(
                OutSlotsTransform,
                SeparateOutputSvmPrefab,
                OutSlotViewModels,
                size,
                _outputFullApiWrapper?.EntityApi);
            AddBlockedSlotsInstances(blockedCount);

            OnSlotItemListCollectionSize(collectionSize, _outputSlots, _outputFullApiWrapper?.EntityApi);
        }

        private void AddBlockedSlotsInstances(int count)
        {
            if (count < 0)
                return;

            var slotsCount = _blockedSlots.Count;
            for (var i = 0; i < slotsCount; i++)
                _blockedSlots[i].SetAsLastSibling();

            for (var i = slotsCount; i < count; i++)
            {
                var newBlocked = Instantiate(BlockedSlotPrefab, OutSlotsTransform);
                newBlocked.name = $"BlockedSlot_{i + 1}";
                _blockedSlots.Add(newBlocked);
            }

            for (var i = slotsCount - 1; i >= count; --i)
            {
                var blockedSlot = _blockedSlots[i];
                Destroy(blockedSlot.gameObject);
            }

            if (slotsCount > count)
                _blockedSlots.RemoveRange(count, slotsCount - count);
        }

        private void OnSlotItemListCollectionSize<Api>(int collectionSize, IList<SlotItem> list, Api api, Action slotsChanged = null)
            where Api : SizeWatchingSlotsCollectionApi
        {
            void OnAnyItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
            {
                if (slotItem.AssertIfNull(nameof(slotItem)))
                    return;

                if (slotIndex < 0 || slotIndex >= list.Count)
                {
                    UI.Logger.Warn(
                        $"{nameof(OnAnyItemChanged)}() {nameof(slotIndex)}={slotIndex} is out of range " +
                        $"({nameof(list)}.Count={list.Count})");
                    return;
                }

                //Просто строим ListStream за которым следят подписчики
                var newSlotItem = new SlotItem();
                newSlotItem.Clone(slotItem);
                list[slotIndex] = newSlotItem;

                slotsChanged?.Invoke();
            }

            while (list.Count > collectionSize)
            {
                //отписки
                var index = list.Count - 1;
                api.UnsubscribeFromSlot(index, OnAnyItemChanged);
                list.RemoveAt(index);
            }

            while (list.Count < collectionSize)
            {
                //подписки
                var index = list.Count;
                list.Add(new SlotItem());
                api.SubscribeToSlot(index, OnAnyItemChanged);
            }
        }

        private void SetSlotViewModels<T, Api>(
            Transform slotsTransform,
            T prefab,
            IList<T> slotViewModels,
            int collectionSize,
            Api api)
            where T : BaseSeparateOutputSlotViewModel<Api> where Api : SizeWatchingSlotsCollectionApi
        {
            while (slotViewModels.Count > collectionSize)
            {
                var index = slotViewModels.Count - 1;
                var removedSvm = slotViewModels[index];
                removedSvm.Unsubscribe(api);
                slotViewModels.Remove(removedSvm);
                Destroy(removedSvm.gameObject);
            }

            while (slotViewModels.Count < collectionSize)
            {
                var i = slotViewModels.Count;
                var newSvm = Instantiate(prefab, slotsTransform);
                newSvm.SetAcceptanceResolver(this);

                var draggableItem = newSvm.transform.GetComponentInChildren<DraggableItem>();
                if (!draggableItem.AssertIfNull(nameof(draggableItem)))
                    draggableItem.Init(DraggingHandler, ContextMenuViewModel, DragContainer);

                newSvm.name = $"{prefab.name}{i}";
                newSvm.SetSlotId(i);
                slotViewModels.Add(newSvm);
                newSvm.Subscribe(api, _contextViewWithParams.Vmodel.Value);
            }
        }
    }
}