using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class PerksTemporaryFullApi : PerksBaseFullApi
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

            await SlotListenersCollection.SubscribeOnItems(worldCharacterClientFull.TemporaryPerks.Items);
            SubscribeToEntityCollectionSize(worldCharacterClientFull.TemporaryPerks.To<ITemporaryPerksClientBroadcast>());
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldCharacterClientFull = (IWorldCharacterClientFull) wrapper;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(worldCharacterClientFull.TemporaryPerks.Items);
            UnsubscribeFromEntityCollectionSize(worldCharacterClientFull.TemporaryPerks.To<ITemporaryPerksClientBroadcast>());
        }
    }
}