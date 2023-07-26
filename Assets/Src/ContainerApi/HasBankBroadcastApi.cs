using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;

namespace Assets.Src.ContainerApis
{
    public class HasBankBroadcastApi : SizeWatchingSlotsCollectionApi
    {
        //=== Props ===========================================================

        protected override bool WatchForSubitems => true;
        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientBroadcast;


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity entity)
        {
            await base.OnWrapperReceivedAtStart(entity);
            var bank = (IBankClientBroadcast) entity;
            if (bank.AssertIfNull(nameof(bank)))
                return;

            var cellRef = await bank.Bank.OpenBankCell();
            using (var containerHolderWrapper = await entity.EntitiesRepository.Get(cellRef.TypeId, cellRef.Guid))
            {
                IHasInventoryClientFull cellEntity =
                    containerHolderWrapper.Get<IHasInventoryClientFull>(cellRef.TypeId, cellRef.Guid, ReplicationLevel.ClientFull);

                CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(cellEntity.Inventory);

                await SlotListenersCollection.SubscribeOnItems(cellEntity.Inventory.Items);
                SubscribeToEntityCollectionSize(cellEntity.Inventory.To<IContainerClientBroadcast>());
            }
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var bank = (IBankClientBroadcast) wrapper;
            if (bank.AssertIfNull(nameof(bank)))
                return;

            var cellRef = await bank.Bank.CloseBankCell();
            using (var containerHolderWrapper = await wrapper.EntitiesRepository.Get(cellRef.TypeId, cellRef.Guid))
            {
                IHasInventoryClientFull cellEntity =
                    containerHolderWrapper.Get<IHasInventoryClientFull>(cellRef.TypeId, cellRef.Guid, ReplicationLevel.ClientFull);

                SlotListenersCollection.UnsubscribeFromItems(cellEntity.Inventory.Items);
                UnsubscribeFromEntityCollectionSize(cellEntity.Inventory.To<IContainerClientBroadcast>());
            }
        }
    }
}