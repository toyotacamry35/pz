using System.Collections.Generic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.Inventory;
using SharedCode.EntitySystem;

namespace Assets.Src.ContainerApis
{
    public delegate void SlotItemChangedDelegate(int slotIndex, SlotItem slotItem, int stackDelta);

    public delegate void SlotItemOperationDelegate(int slotIndex, SlotItem slotItem);

    public delegate void FrequentStatsChangedDelegate(int slotIndex, List<KeyValuePair<StatResource, float>> stats);

    public abstract class SlotsCollectionApi : EntityApi
    {
        protected SlotListenersCollection SlotListenersCollection;

        public int CollectionId;


        //=== Props ===========================================================

        public PropertyAddress CollectionPropertyAddress { get; protected set; }

        protected virtual bool WatchForSubitems => false;


        //=== Unity ===========================================================

        public SlotsCollectionApi()
        {
            SlotListenersCollection = new SlotListenersCollection(WatchForSubitems);
        }


        //=== Public ==========================================================

        public void SubscribeToSlot(int slotIndex, SlotItemChangedDelegate onItemChangedDelegate)
        {
            if (onItemChangedDelegate.AssertIfNull(nameof(onItemChangedDelegate)))
                return;

            SlotListenersCollection.OnSubscribeRequest(slotIndex, onItemChangedDelegate);
            Debug_ShowCollectionId();
        }

        public void UnsubscribeFromSlot(int slotIndex, SlotItemChangedDelegate onItemChangedDelegate)
        {
            if (onItemChangedDelegate.AssertIfNull(nameof(onItemChangedDelegate)))
                return;

            SlotListenersCollection.OnUnsubscribeRequest(slotIndex, onItemChangedDelegate);
        }

        public void SubscribeToSlotFrequentStats(int slotIndex, FrequentStatsChangedDelegate onFrequentStatsChanged)
        {
            if (onFrequentStatsChanged.AssertIfNull(nameof(onFrequentStatsChanged)))
                return;

            SlotListenersCollection.OnStatsSubscribeRequest(slotIndex, onFrequentStatsChanged);
        }

        public void UnsubscribeFromSlotFrequentStats(int slotIndex, FrequentStatsChangedDelegate onFrequentStatsChanged)
        {
            if (onFrequentStatsChanged.AssertIfNull(nameof(onFrequentStatsChanged)))
                return;

            SlotListenersCollection.OnStatsUnsubscribeRequest(slotIndex, onFrequentStatsChanged);
        }


        //=== Protected =======================================================

        private void Debug_ShowCollectionId()
        {
            if (CollectionId == 0)
                CollectionId = SlotListenersCollection.CollectionId;
        }
    }
}