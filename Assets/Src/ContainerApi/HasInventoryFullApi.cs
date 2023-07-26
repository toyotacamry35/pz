using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class HasInventoryFullApi : SizeWatchingSlotsCollectionApi
    {
        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;

        protected override bool WatchForSubitems => true;


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            await base.OnWrapperReceivedAtStart(wrapper);
            var hasInventory = (IHasInventoryClientFull) wrapper;
            if (hasInventory.AssertIfNull(nameof(hasInventory)))
                return;

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(hasInventory.Inventory);

            await SlotListenersCollection.SubscribeOnItems(hasInventory.Inventory.Items);
            SubscribeToEntityCollectionSize(hasInventory.Inventory.To<IContainerClientBroadcast>());
        }

        protected override Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var hasInventory = (IHasInventoryClientFull)wrapper;
            if (hasInventory.AssertIfNull(nameof(hasInventory)))
                return Task.CompletedTask;

            SlotListenersCollection.UnsubscribeFromItems(hasInventory.Inventory.Items);
            UnsubscribeFromEntityCollectionSize(hasInventory.Inventory.To<IContainerClientBroadcast>());
            return Task.CompletedTask;
        }
    }
}