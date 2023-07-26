using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class PerksPermanentFullApi : PerksBaseFullApi
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

            await SlotListenersCollection.SubscribeOnItems(worldCharacterClientFull.PermanentPerks.Items);
            SubscribeToEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<IPermanentPerksClientBroadcast>());
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldCharacterClientFull = (IWorldCharacterClientFull) wrapper;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(worldCharacterClientFull.PermanentPerks.Items);
            UnsubscribeFromEntityCollectionSize(worldCharacterClientFull.PermanentPerks.To<IPermanentPerksClientBroadcast>());
        }
    }
}