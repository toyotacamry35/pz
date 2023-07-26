using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class MachineOutputFullApi : MachineBaseFullApi
    {
        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            await base.OnWrapperReceivedAtStart(wrapper);
            var worldMachineClientFull = (IWorldMachineClientFull) wrapper;
            if (worldMachineClientFull.AssertIfNull(nameof(worldMachineClientFull)))
                return;

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(worldMachineClientFull.OutputContainer);

            await SlotListenersCollection.SubscribeOnItems(worldMachineClientFull.OutputContainer.Items);
            SubscribeToEntityCollectionSize(worldMachineClientFull.OutputContainer.To<IMachineOutputContainerClientBroadcast>());
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var worldMachineClientFull = (IWorldMachineClientFull) wrapper;
            if (worldMachineClientFull.AssertIfNull(nameof(worldMachineClientFull)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(worldMachineClientFull.OutputContainer.Items);
            UnsubscribeFromEntityCollectionSize(worldMachineClientFull.OutputContainer.To<IMachineOutputContainerClientBroadcast>());
        }
    }
}