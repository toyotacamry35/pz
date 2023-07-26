using Assets.Src.ContainerApis;

namespace Uins.Slots
{
    public class BaseSeparateOutputSlotViewModel<T> : OuterBaseSlotViewModel where T : SizeWatchingSlotsCollectionApi
    {
        public void Subscribe(T machineApi, IHasContextStream hasContextStream = null)
        {
            BaseSubscribe(machineApi, hasContextStream);
            machineApi.SubscribeToSlot(SlotId, OnItemChanged);
        }

        public void Unsubscribe(T machineApi)
        {
            machineApi.UnsubscribeFromSlot(SlotId, OnItemChanged);
            BaseUnsubscribe();
            Reset();
        }
    }
}