using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class PerksSavedFullApi : PerksBaseFullApi
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

            await SlotListenersCollection.SubscribeOnItems(worldCharacterClientFull.SavedPerks.Items);
            SubscribeToEntityCollectionSize(worldCharacterClientFull.SavedPerks.To<ISavedPerksClientBroadcast>());
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldCharacterClientFull = (IWorldCharacterClientFull) wrapper;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(worldCharacterClientFull.SavedPerks.Items);
            UnsubscribeFromEntityCollectionSize(worldCharacterClientFull.SavedPerks.To<ISavedPerksClientBroadcast>());
        }
    }
}