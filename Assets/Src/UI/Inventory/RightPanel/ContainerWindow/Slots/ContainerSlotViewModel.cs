using System.Collections.Generic;
using Assets.Src.ContainerApis;
using Assets.Src.Inventory;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class ContainerSlotViewModel : OuterBaseSlotViewModel
    {
        private List<ContainerSlotViewModel> _markedToTakeSlotViewModels;


        //=== Props ===========================================================

        public ContainerMode ContainerMode { private get; set; }
        
        public override CollectionType SlotCollectionType => CollectionType.Inventory;

        public override bool CanBeEquippedOrUnequipped => false;

        private bool _isMarkedToTake;

        [Binding]
        public bool IsMarkedToTake
        {
            get => _isMarkedToTake;
            set
            {
                if (_isMarkedToTake != value)
                {
                    _isMarkedToTake = value;
                    CheckMarkedToTakeList();
                    NotifyPropertyChanged();
                }
            }
        }

        private void CheckMarkedToTakeList()
        {
            if (_markedToTakeSlotViewModels == null)
                return;

            if (IsMarkedToTake)
            {
                if (!_markedToTakeSlotViewModels.Contains(this))
                    _markedToTakeSlotViewModels.Add(this);
            }
            else
            {
                if (_markedToTakeSlotViewModels.Contains(this))
                    _markedToTakeSlotViewModels.Remove(this);
            }
        }


        //=== Public ==========================================================

        public void Subscribe(SizeWatchingSlotsCollectionApi sizeWatchingSlotsCollectionApi, IHasContextStream hasContextStream = null)
        {
            BaseSubscribe(sizeWatchingSlotsCollectionApi, hasContextStream);
            sizeWatchingSlotsCollectionApi.SubscribeToSlot(SlotId, OnItemChanged);
        }

        public void Unsubscribe(SizeWatchingSlotsCollectionApi sizeWatchingSlotsCollectionApi)
        {
            BaseUnsubscribe();
            sizeWatchingSlotsCollectionApi.UnsubscribeFromSlot(SlotId, OnItemChanged);
            Reset();
        }

        public override void OnClick()
        {
            base.OnClick();
            IsMarkedToTake = !IsMarkedToTake;
        }

        public void SetListOfMarkedToTake(List<ContainerSlotViewModel> markedToTakeSlotViewModels)
        {
            _markedToTakeSlotViewModels = markedToTakeSlotViewModels;
        }


        //=== Protected =======================================================

        protected override void OnItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
        {
            base.OnItemChanged(slotIndex, slotItem, stackDelta);
            if (slotItem.Count == 0)
                IsMarkedToTake = false;
        }
    }
}