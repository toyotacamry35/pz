using Assets.Src.ContainerApis;
using UnityWeld.Binding;

namespace Uins.Slots
{
    [Binding]
    public class PermanentPerkSlotViewModel : MayBeFactionPerkSlotViewModel<PerksPermanentFullApi, PerkSlotsPermanentFullApi>
    {
        protected override void UpdateContextViewIsSelected()
        {
            if (!IsSelected || HasContextStream == null)
                return;

            TakeContext(true);
            TakeContext();
            IsSelected = true;
        }
    }
}