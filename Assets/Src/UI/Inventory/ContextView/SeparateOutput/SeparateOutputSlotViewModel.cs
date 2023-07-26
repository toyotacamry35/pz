using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class SeparateOutputSlotViewModel : BaseSeparateOutputSlotViewModel<SeparateOutputFullApi>
    {
        public override CollectionType SlotCollectionType => CollectionType.Output;
    }
}