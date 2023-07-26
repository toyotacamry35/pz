using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class PerkSlotsPermanentFullApi : PerkSlotsBaseFullApi
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

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldCharacterClientFull.PermanentPerks);

            await PerkSlotListenersCollection.SubscribeOnCollection(worldCharacterClientFull.PermanentPerks.PerkSlots);
            //SubscribeToEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<ICharacterPerksClientBroadcast>()); //TODOM
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldCharacterClientFull = (IWorldCharacterClientFull) wrapper;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return;

            await PerkSlotListenersCollection.UnsubscribeFromCollection(worldCharacterClientFull.PermanentPerks.PerkSlots);
            //UnsubscribeFromEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<ICharacterPerksClientBroadcast>()); //TODOM
        }
    }
}