using Assets.Src.ContainerApis;

namespace Uins.Slots
{
    public class MachineBaseSlotViewModel<T> : OuterBaseSlotViewModel where T : MachineBaseFullApi
    {
        public void Subscribe(T machineApi)
        {
            BaseSubscribe(machineApi);
            machineApi.SubscribeToSlot(SlotId, OnItemChanged);
        }

        public void Unsubscribe(T machineApi)
        {
            BaseUnsubscribe();
            machineApi.UnsubscribeFromSlot(SlotId, OnItemChanged);
            Reset();
        }
    }
}