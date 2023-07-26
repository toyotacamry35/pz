using SharedCode.Aspects.Item.Templates;

namespace Assets.Src.ContainerApis
{
    public abstract class PerkSlotsBaseFullApi : SizeWatchingSlotsCollectionApi
    {
        public delegate void PerkSlotTypeChangedDelegate(int slotIndex, ItemTypeResource perkSlotType);

        protected PerkSlotListenersCollection PerkSlotListenersCollection { get; } = new PerkSlotListenersCollection();


        //=== Public ==========================================================

        public void SubscribeToPerkSlotType(int slotIndex, PerkSlotTypeChangedDelegate onPerkSlotTypeChangedDelegate)
        {
            if (onPerkSlotTypeChangedDelegate.AssertIfNull(nameof(onPerkSlotTypeChangedDelegate)))
                return;

            PerkSlotListenersCollection.OnSubscribeRequest(slotIndex, onPerkSlotTypeChangedDelegate);
        }

        public void UnsubscribeFromPerkSlotType(int slotIndex, PerkSlotTypeChangedDelegate onPerkSlotTypeChangedDelegate)
        {
            if (
                onPerkSlotTypeChangedDelegate.AssertIfNull(nameof(onPerkSlotTypeChangedDelegate)))
                return;

            PerkSlotListenersCollection.OnUnsubscribeRequest(slotIndex, onPerkSlotTypeChangedDelegate);
        }
    }
}