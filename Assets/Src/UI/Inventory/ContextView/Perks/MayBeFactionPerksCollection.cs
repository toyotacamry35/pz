using Assets.Src.ContainerApis;
using Uins.Slots;

namespace Uins
{
    public class MayBeFactionPerksCollection<VM, T, U> : PerksCollection<VM, T, U>
        where VM : MayBeFactionPerkSlotViewModel<T, U>
        where T : PerksBaseFullApi, new()
        where U : PerkSlotsBaseFullApi, new()
    {
        private IFactionStagePerksResolver _factionStagePerksResolver;


        //=== Public ==========================================================

        public void SetFactionStagePerksResolver(IFactionStagePerksResolver factionStagePerksResolver)
        {
            _factionStagePerksResolver = factionStagePerksResolver;
            _factionStagePerksResolver.AssertIfNull(nameof(_factionStagePerksResolver));
        }

        protected override void OnAddPerkSvm(VM newPerkSvm)
        {
            newPerkSvm.SetFactionStagePerksResolver(_factionStagePerksResolver);
            base.OnAddPerkSvm(newPerkSvm);
        }

        protected override void OnRemovePerkSvm(VM removedPerkSvm)
        {
            base.OnRemovePerkSvm(removedPerkSvm);
            removedPerkSvm.SetFactionStagePerksResolver(null);
        }
    }
}