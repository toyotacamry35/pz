using Assets.Src.ContainerApis;
using Uins.Slots;

namespace Uins
{
    public class PermanentPerksCollection : MayBeFactionPerksCollection<PermanentPerkSlotViewModel, PerksPermanentFullApi, PerkSlotsPermanentFullApi>
    {
        //=== Props ===========================================================

        protected override bool NeedForPerkSlotTypeSubscribe => true;
    }
}