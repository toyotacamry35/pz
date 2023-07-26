using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class CharInvSlotViewModel : SlotViewModel
    {
        //=== Props ===========================================================

        public override CollectionType SlotCollectionType => CollectionType.Inventory;

        public override bool IsOurPlayerSlot => true;

        private bool _isVisible = true;

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


        //=== Public ==========================================================

        public void Subscribe(HasInventoryFullApi hasInventoryFullApi, IHasContextStream hasContextStream = null)
        {
            BaseSubscribe(hasInventoryFullApi, hasContextStream);
            hasInventoryFullApi.SubscribeToSlot(SlotId, OnItemChanged);
            hasInventoryFullApi.SubscribeToSlotFrequentStats(SlotId, OnFrequentStatsChanged);
        }

        public void Unsubscribe(HasInventoryFullApi hasInventoryFullApi)
        {
            BaseUnsubscribe();
            hasInventoryFullApi.UnsubscribeFromSlot(SlotId, OnItemChanged);
            hasInventoryFullApi.UnsubscribeFromSlotFrequentStats(SlotId, OnFrequentStatsChanged);
        }
    }
}