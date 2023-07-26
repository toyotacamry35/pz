using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class TemporaryPerkSlotViewModel : MayBeFactionPerkSlotViewModel<PerksTemporaryFullApi, PerkSlotsTemporaryFullApi>
    {
        protected override bool GetIsLocked()
        {
            return ItemTypeIndex < 0;
        }
    }
}