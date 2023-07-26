using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class SeparateOutputFullApi : SizeWatchingSlotsCollectionApi
    {
        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;
        
        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            await base.OnWrapperReceivedAtStart(wrapper);
            var craftEngineClientFull = (ICraftEngineClientFull) wrapper;
            if (craftEngineClientFull.AssertIfNull(nameof(craftEngineClientFull)))
                return;

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(craftEngineClientFull.OutputContainer);

            await SlotListenersCollection.SubscribeOnItems(craftEngineClientFull.OutputContainer.Items);
            SubscribeToEntityCollectionSize(craftEngineClientFull.OutputContainer.To<IContainerClientBroadcast>());
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var craftEngineClientFull = (ICraftEngineClientFull) wrapper;
            if (craftEngineClientFull.AssertIfNull(nameof(craftEngineClientFull)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(craftEngineClientFull.OutputContainer.Items);
            UnsubscribeFromEntityCollectionSize(craftEngineClientFull.OutputContainer.To<IContainerClientBroadcast>());
        }
    }
}