using System.Collections.Concurrent;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System.Linq;
using Core.Environment.Logging.Extension;
using Uins;

namespace Assets.Src.ContainerApis
{
    public class CraftQueueListeners
    {
        private const string CountPropertyName = nameof(ICraftingQueueItemClientFull.Count);
        private const string IsActivePropertyName = nameof(ICraftingQueueItemClientFull.IsActive);
        private const string CraftStartTimePropertyName = nameof(ICraftingQueueItemClientFull.CraftStartTime);
        private ConcurrentDictionary<int, QueueSlotListener> _listeners = new ConcurrentDictionary<int, QueueSlotListener>();
        private CraftVirtualQueue _virtualQueue;


        //=== Ctor ============================================================

        public CraftQueueListeners(CraftVirtualQueue virtualQueue)
        {
            virtualQueue.AssertIfNull(nameof(virtualQueue));
            _virtualQueue = virtualQueue;
        }


        //=== Public ==========================================================

        public void Subscribe(IDeltaDictionary<int, ICraftingQueueItemClientFull> queue)
        {
            if (queue.AssertIfNull(nameof(queue)))
                return;

            queue.OnItemAddedOrUpdated += OnCraftingQueueAddedOrUpdated;
            queue.OnItemRemoved += OnCraftingQueueRemoved;

            foreach (var kvp in queue)
                OnItemAddedOrUpdatedInternal(kvp.Key, kvp.Value, true);
        }

        public void Unsubscribe(IDeltaDictionary<int, ICraftingQueueItemClientFull> queue)
        {
            queue.OnItemAddedOrUpdated -= OnCraftingQueueAddedOrUpdated;
            queue.OnItemRemoved -= OnCraftingQueueRemoved;

            foreach (var kvp in queue)
                OnItemRemovedInternal(kvp.Key, kvp.Value);
        }

        public override string ToString()
        {
            return $"[{nameof(CraftQueueListeners)} {nameof(_listeners)}={_listeners.Count}]";
        }


        //=== Private ==============================================================

        private Task OnCraftingQueueAddedOrUpdated(DeltaDictionaryChangedEventArgs<int, ICraftingQueueItemClientFull> eventArgs)
        {
            var itemKey = eventArgs.Key;
            var slotItemClientBroadcast = eventArgs.Value;

            OnItemAddedOrUpdatedInternal(itemKey, slotItemClientBroadcast);
            return Task.CompletedTask;
        }

        private Task OnCraftingQueueRemoved(DeltaDictionaryChangedEventArgs<int, ICraftingQueueItemClientFull> eventArgs)
        {
            var itemIndex = eventArgs.Key;
            var slotItemClientBroadcast = eventArgs.OldValue;

            OnItemRemovedInternal(itemIndex, slotItemClientBroadcast);
            return Task.CompletedTask;
        }

        private void OnItemAddedOrUpdatedInternal(int itemIndex, ICraftingQueueItemClientFull craftingQueueItem, bool isFirstTime = false)
        {
            QueueSlotListener queueSlotListener;
            if (_listeners.TryGetValue(itemIndex, out queueSlotListener))
            {
                UI.Logger.IfError()?.Message($"Unexpected {nameof(QueueSlotListener)} at index={itemIndex}").Write();
            }
            else
            {
                queueSlotListener = new QueueSlotListener(itemIndex, this);
                lock (_listeners)
                {
                    _listeners.TryAdd(itemIndex, queueSlotListener);
                }
            }

            queueSlotListener.SubscribeAndSetSelfSlotItem(craftingQueueItem);
        }

        private void OnItemRemovedInternal(int itemIndex, ICraftingQueueItemClientFull craftingQueueItem)
        {
            QueueSlotListener slotListener;
            lock (_listeners)
            {
                if (!_listeners.TryRemove(itemIndex, out slotListener))
                    return;
            }

            slotListener.UnsubscribeAndResetSelfSlotItem(craftingQueueItem);
        }

        private void OnSlotChanged()
        {
            _virtualQueue.ChangeCraftQueueSlots();
        }

        private void OnItemsCountChanged()
        {
            _virtualQueue.ChangeCraftQueueSlots(_listeners
                .OrderByDescending(kvp => kvp.Value.SelfSlot.IsActive)
                .ThenBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.SelfSlot)
                .ToList());
        }


        //=== Class ===================================================================================================

        private class QueueSlotListener
        {
            public CraftQueueSlot SelfSlot { get; }

            public bool IsEmpty => SelfSlot.CraftRecipe == null;

            private CraftQueueListeners _parent;


            //=== Ctor ========================================================

            public QueueSlotListener(int slotIndex, CraftQueueListeners parent)
            {
                _parent = parent;
                SelfSlot = new CraftQueueSlot() {KeyIndex = slotIndex};
            }


            //=== Public ======================================================

            public override string ToString()
            {
                return $"{nameof(QueueSlotListener)}: {(IsEmpty ? " EMPTY\n" : " ")}{SelfSlot}";
            }

            public void SubscribeAndSetSelfSlotItem(ICraftingQueueItemClientFull queueItem)
            {
                lock (SelfSlot)
                {
                    if (!queueItem.AssertIfNull(nameof(queueItem)))
                    {
                        SelfSlot.CraftRecipe = queueItem.CraftRecipe;
                        SelfSlot.Count = queueItem.Count;
                        SelfSlot.SelectedVariantIndex = queueItem.SelectedVariantIndex;
                        SelfSlot.IsActive = queueItem.IsActive;
                        SelfSlot.CraftStartTime = queueItem.CraftStartTime;
                    }
                }

                queueItem.SubscribePropertyChanged(CraftStartTimePropertyName, OnChangeCraftStartTimeTask);
                queueItem.SubscribePropertyChanged(CountPropertyName, OnChangeCountTask);
                queueItem.SubscribePropertyChanged(IsActivePropertyName, OnChangeIsActiveTask);
                OnItemsCountChanged();
            }

            public void UnsubscribeAndResetSelfSlotItem(ICraftingQueueItemClientFull queueItem)
            {
                queueItem.UnsubscribePropertyChanged(CraftStartTimePropertyName, OnChangeCraftStartTimeTask);
                queueItem.UnsubscribePropertyChanged(CountPropertyName, OnChangeCountTask);
                queueItem.UnsubscribePropertyChanged(IsActivePropertyName, OnChangeIsActiveTask);

                var wasEmpty = SelfSlot.IsEmpty;
                lock (SelfSlot)
                {
                    SelfSlot.Reset();
                }

                if (!wasEmpty)
                    OnItemsCountChanged();
            }


            //=== Private =========================================================

            private Task OnChangeCraftStartTimeTask(EntityEventArgs args)
            {
                var newCraftStartTime = (long) args.NewValue;
                if (SelfSlot.CraftStartTime == newCraftStartTime)
                    return Task.CompletedTask;

                lock (SelfSlot)
                {
                    SelfSlot.CraftStartTime = newCraftStartTime;
                }

                OnItemChanged();
                return Task.CompletedTask;
                
            }

            private Task OnChangeCountTask(EntityEventArgs args)
            {
                var newCount = (int) args.NewValue;
                if (SelfSlot.Count == newCount)
                    return Task.CompletedTask;

                lock (SelfSlot)
                {
                    SelfSlot.Count = newCount;
                }

                OnItemChanged();
                return Task.CompletedTask;
            }

            private Task OnChangeIsActiveTask(EntityEventArgs args)
            {
                var newIsActive = (bool) args.NewValue;
                if (newIsActive == SelfSlot.IsActive)
                    return Task.CompletedTask;

                lock (SelfSlot)
                {
                    SelfSlot.IsActive = newIsActive;
                }

                OnItemChanged();
                return Task.CompletedTask;
            }

            private void OnItemChanged()
            {
                _parent.OnSlotChanged();
            }

            private void OnItemsCountChanged()
            {
                _parent.OnItemsCountChanged();
            }
        }
    }
}