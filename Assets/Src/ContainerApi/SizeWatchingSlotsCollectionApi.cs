using System;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Entities;
using SharedCode.EntitySystem;

namespace Assets.Src.ContainerApis
{
    public abstract class SizeWatchingSlotsCollectionApi : SlotsCollectionApi
    {
        public static readonly string[] InventoryPropertyNames = new string[] {nameof(IHasInventory.Inventory)};

        public event Action<int> CollectionSizeChanged;

        protected int LastCollectionSize;


        //=== Public ==========================================================

        public void SubscribeToCollectionSizeChanged(Action<int> onCollectionSizeChanged)
        {
            if (onCollectionSizeChanged.AssertIfNull(nameof(onCollectionSizeChanged)))
                return;

            CollectionSizeChanged += onCollectionSizeChanged;
            if (LastCollectionSize > 0)
                onCollectionSizeChanged(LastCollectionSize);
        }

        public void UnsubscribeFromCollectionSizeChanged(Action<int> onCollectionSizeChanged)
        {
            if (onCollectionSizeChanged.AssertIfNull(nameof(onCollectionSizeChanged)))
                return;

            CollectionSizeChanged -= onCollectionSizeChanged;
        }


        //=== Protected =======================================================

        protected void SubscribeToEntityCollectionSize(IItemsContainerClientBroadcast itemsContainerClientBroadcast)
        {
            itemsContainerClientBroadcast.SubscribePropertyChanged(nameof(itemsContainerClientBroadcast.Size), OnCollectionSizeChanged);
            LastCollectionSize = itemsContainerClientBroadcast.Size;

            if (LastCollectionSize > 0)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { CollectionSizeChanged?.Invoke(LastCollectionSize); });
        }

        protected void UnsubscribeFromEntityCollectionSize(IItemsContainerClientBroadcast itemsContainerClientBroadcast)
        {
            itemsContainerClientBroadcast.UnsubscribePropertyChanged(nameof(itemsContainerClientBroadcast.Size), OnCollectionSizeChanged);
            CollectionSizeChanged = null;
        }


        //=== Private =========================================================

        private Task OnCollectionSizeChanged(EntityEventArgs args)
        {
            LastCollectionSize = (int) args.NewValue;
            UnityQueueHelper.RunInUnityThreadNoWait(() => CollectionSizeChanged?.Invoke(LastCollectionSize));
            return Task.CompletedTask;
        }
    }
}