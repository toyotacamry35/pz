using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Shared;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class HasCraftEngineIntermediateFullApi : SizeWatchingSlotsCollectionApi
    {
        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(wrapper);

            //await base.OnWrapperReceivedAtStart(wrapper); //специально откл.
            var hasCraftEngineClientFull = (IHasCraftEngineClientFull) wrapper;
            if (hasCraftEngineClientFull.AssertIfNull(nameof(hasCraftEngineClientFull)))
                return;

            var craftEngineRef = hasCraftEngineClientFull.CraftEngine;
            if (craftEngineRef.AssertIfNull(nameof(craftEngineRef)))
                return;

            using (var craftEngineWrapper = await NodeAccessor.Repository.Get(craftEngineRef.TypeId, craftEngineRef.Id))
            {
                if (craftEngineWrapper.AssertIfNull(nameof(craftEngineWrapper)))
                    return;

                var craftEngineClientFull = craftEngineWrapper.Get<ICraftEngineClientFull>(craftEngineRef.TypeId, craftEngineRef.Id,
                    ReplicationLevel.ClientFull);
                if (craftEngineClientFull.AssertIfNull(nameof(craftEngineClientFull)))
                    return;

                await SlotListenersCollection.SubscribeOnItems(craftEngineClientFull.IntermediateFuelContainer.Items);
                SubscribeToEntityCollectionSize(craftEngineClientFull.IntermediateFuelContainer.To<IContainerClientBroadcast>());
            }
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var hasCraftEngineClientFull = (IHasCraftEngineClientFull) wrapper;
            if (hasCraftEngineClientFull.AssertIfNull(nameof(hasCraftEngineClientFull)))
                return;

            var craftEngineRef = hasCraftEngineClientFull.CraftEngine;
            if (craftEngineRef.AssertIfNull(nameof(craftEngineRef)))
                return;

            using (var craftEngineWrapper = await NodeAccessor.Repository.Get(craftEngineRef.TypeId, craftEngineRef.Id))
            {
                if (craftEngineWrapper.AssertIfNull(nameof(craftEngineWrapper)))
                    return;

                var craftEngineClientFull = craftEngineWrapper.Get<ICraftEngineClientFull>(craftEngineRef.TypeId, craftEngineRef.Id,
                    ReplicationLevel.ClientFull);
                if (craftEngineClientFull.AssertIfNull(nameof(craftEngineClientFull)))
                    return;

                SlotListenersCollection.UnsubscribeFromItems(craftEngineClientFull.IntermediateFuelContainer.Items);
                UnsubscribeFromEntityCollectionSize(craftEngineClientFull.IntermediateFuelContainer.To<IContainerClientBroadcast>());
            }
        }
    }
}