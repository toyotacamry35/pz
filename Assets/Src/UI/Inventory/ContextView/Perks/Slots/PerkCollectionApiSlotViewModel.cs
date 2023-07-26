using Assets.Src.ContainerApis;

namespace Uins.Slots
{
    public abstract class PerkCollectionApiSlotViewModel<T, U> : PerkBaseSlotViewModel
        where T : PerksBaseFullApi
        where U : PerkSlotsBaseFullApi
    {
        public void Subscribe(T perksBaseFullApi, U perkSlotsBaseFullApi, IHasContextStream hasContextStream = null)
        {
            BaseSubscribe(perksBaseFullApi, hasContextStream);
            perksBaseFullApi.SubscribeToSlot(SlotId, OnItemChanged);
            if (NeedForPerkSlotTypeSubscribe)
            {
                PerkSlotsCollectionApi = perkSlotsBaseFullApi;
                perkSlotsBaseFullApi.SubscribeToPerkSlotType(SlotId, OnPerkSlotTypeChanged);
            }
        }

        public void Unsubscribe(T perksBaseFullApi, U perkSlotsBaseFullApi)
        {
            BaseUnsubscribe();
            perksBaseFullApi.UnsubscribeFromSlot(SlotId, OnItemChanged);
            if (NeedForPerkSlotTypeSubscribe)
            {
                perkSlotsBaseFullApi.UnsubscribeFromPerkSlotType(SlotId, OnPerkSlotTypeChanged);
                PerkSlotsCollectionApi = null;
            }
        }

        public virtual void SubscribeOnSomePerkDrag(DraggingHandler draggingHandler, bool isSubscribeNorUnsubscribe)
        {
        }
    }
}