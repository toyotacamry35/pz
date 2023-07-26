using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class MachineInventoryFullApi : MachineBaseFullApi
    {
        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;

        protected override bool WatchForSubitems => true;


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            await base.OnWrapperReceivedAtStart(wrapper);
            var worldMachineClientFull = (IWorldMachineClientFull) wrapper;
            if (worldMachineClientFull.AssertIfNull(nameof(worldMachineClientFull)))
                return;

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldMachineClientFull.Inventory);

            await SlotListenersCollection.SubscribeOnItems(worldMachineClientFull.Inventory.Items);
            SubscribeToEntityCollectionSize(worldMachineClientFull.Inventory.To<IBuildingContainerClientBroadcast>());
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldMachineClientFull = (IWorldMachineClientFull) wrapper;
            if (worldMachineClientFull.AssertIfNull(nameof(worldMachineClientFull)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(worldMachineClientFull.Inventory.Items);
            UnsubscribeFromEntityCollectionSize(worldMachineClientFull.Inventory.To<IBuildingContainerClientBroadcast>());
        }
    }
}