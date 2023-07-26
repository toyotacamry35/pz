using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class MachineFuelSlotViewModel : MachineBaseSlotViewModel<MachineFuelFullApi>
    {
        public override CollectionType SlotCollectionType => CollectionType.Fuel;
    }
}