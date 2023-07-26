using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class PerkSlotsTemporaryFullApi : PerkSlotsBaseFullApi
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

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacterClientFull.TemporaryPerks);

            await PerkSlotListenersCollection.SubscribeOnCollection(worldCharacterClientFull.TemporaryPerks.PerkSlots);
            //SubscribeToEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<ICharacterPerksClientBroadcast>()); //TODOM
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldCharacterClientFull = (IWorldCharacterClientFull) wrapper;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return;

            await PerkSlotListenersCollection.UnsubscribeFromCollection(worldCharacterClientFull.TemporaryPerks.PerkSlots);
            //UnsubscribeFromEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<ICharacterPerksClientBroadcast>()); //TODOM
        }
    }
}