using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class MachineInvSlotViewModel : MachineBaseSlotViewModel<MachineInventoryFullApi>
    {
        public override CollectionType SlotCollectionType => CollectionType.Inventory;
    }
}