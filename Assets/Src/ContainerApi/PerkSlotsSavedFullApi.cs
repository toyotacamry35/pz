using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class PerkSlotsSavedFullApi : PerkSlotsBaseFullApi
    {
        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            await base.OnWrapperReceivedAtStart(wrapper);
            var worldCharacterClientFull = (IWorldCharacterClientFull) wrapper;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return;

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacterClientFull.SavedPerks);

            await PerkSlotListenersCollection.SubscribeOnCollection(worldCharacterClientFull.SavedPerks.PerkSlots);
            //SubscribeToEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<ICharacterPerksClientBroadcast>()); //TODOM
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldCharacterClientFull = (IWorldCharacterClientFull) wrapper;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return;

            await PerkSlotListenersCollection.UnsubscribeFromCollection(worldCharacterClientFull.SavedPerks.PerkSlots);
            //UnsubscribeFromEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<ICharacterPerksClientBroadcast>()); //TODOM
        }
    }
}