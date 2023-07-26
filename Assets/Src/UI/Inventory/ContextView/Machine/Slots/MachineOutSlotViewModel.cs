using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class MachineOutSlotViewModel : MachineBaseSlotViewModel<MachineOutputFullApi>
    {
        public override CollectionType SlotCollectionType => CollectionType.Output;
    }
}