using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Utils;

namespace Assets.Src.ContainerApis
{
    public class HasDollBroadcastApi : SlotsCollectionApi
    {
        private event UsedSlotsChangedDelegate UsedSlotsChanged;

        public delegate void UsedSlotsChangedDelegate(IList<ResourceIDFull> usedSlotsIndices);

        private ThreadSafeList<ResourceIDFull> _usedSlotsIndices = new ThreadSafeList<ResourceIDFull>();


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientBroadcast;

        protected override bool WatchForSubitems => true;


        //=== Public ==========================================================

        public void SubscribeToUsedSlotsChanged(UsedSlotsChangedDelegate onUsedSlotsChanged)
        {
            if (onUsedSlotsChanged.AssertIfNull(nameof(onUsedSlotsChanged)))
                return;

            UsedSlotsChanged += onUsedSlotsChanged;
            if (_usedSlotsIndices.Count > 0)
                onUsedSlotsChanged(_usedSlotsIndices);
        }

        public void UnsubscribeFromUsedSlotsChanged(UsedSlotsChangedDelegate onUsedSlotsChanged)
        {
            if (onUsedSlotsChanged.AssertIfNull(nameof(onUsedSlotsChanged)))
                return;

            UsedSlotsChanged -= onUsedSlotsChanged;
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var characterDollClientBroadcast = ((IHasDollClientBroadcast) wrapper)?.Doll;
            if (characterDollClientBroadcast.AssertIfNull(nameof(characterDollClientBroadcast)))
                return;

            CollectionPropertyAddress = EntityPropertyResolver.GetPropertyAddress(characterDollClientBroadcast);

            await SlotListenersCollection.SubscribeOnItems(characterDollClientBroadcast.Items);
            await SubscribeToDollEntityUsedSlots(characterDollClientBroadcast.To<ICharacterDollClientBroadcast>());
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            var characterDollClientBroadcast = ((IHasDollClientBroadcast) wrapper)?.Doll;
            if (characterDollClientBroadcast.AssertIfNull(nameof(characterDollClientBroadcast)))
                return;

            SlotListenersCollection.UnsubscribeFromItems(characterDollClientBroadcast.Items);
            UnsubscribeFromDollEntityUsedSlots(characterDollClientBroadcast.To<ICharacterDollClientBroadcast>());
        }


        //=== Private =========================================================

        private Task OnUsedSlotsItemAdded(DeltaListChangedEventArgs<ResourceIDFull> eventArgs)
        {
            OnUsedSlotsItemAddedInternal(eventArgs.Value);
            return Task.CompletedTask;
        }

        private void OnUsedSlotsItemAddedInternal(ResourceIDFull addedSlotId, bool doEvent = true)
        {
            if (!_usedSlotsIndices.Contains(addedSlotId))
            {
                _usedSlotsIndices.Add(addedSlotId);
                if (doEvent)
                    UnityQueueHelper.RunInUnityThreadNoWait(() => UsedSlotsChanged?.Invoke(_usedSlotsIndices));
            }
            else
            {
                LogError($"{nameof(_usedSlotsIndices)} already contains {nameof(addedSlotId)}={addedSlotId}");
            }
        }

        private async Task OnUsedSlotsItemRemoved(DeltaListChangedEventArgs<ResourceIDFull> eventArgs)
        {
            await OnUsedSlotsItemRemovedInternal(eventArgs.Value);
        }

        private async Task OnUsedSlotsItemRemovedInternal(ResourceIDFull removedSlotId)
        {
            if (_usedSlotsIndices.Contains(removedSlotId))
            {
                _usedSlotsIndices.Remove(removedSlotId);
                UnityQueueHelper.RunInUnityThreadNoWait(() => UsedSlotsChanged?.Invoke(_usedSlotsIndices));
            }
            else
            {
                LogError($"{nameof(removedSlotId)}={removedSlotId} not found in {nameof(_usedSlotsIndices)}");
            }
        }

        private async Task SubscribeToDollEntityUsedSlots(ICharacterDollClientBroadcast characterDollClientBroadcast)
        {
            characterDollClientBroadcast.UsedSlots.OnItemAdded += OnUsedSlotsItemAdded;
            characterDollClientBroadcast.UsedSlots.OnItemRemoved += OnUsedSlotsItemRemoved;
            if (characterDollClientBroadcast.UsedSlots.Count > 0)
            {
                foreach (var slotResourceId in characterDollClientBroadcast.UsedSlots)
                {
                    if (!_usedSlotsIndices.Contains(slotResourceId))
                        OnUsedSlotsItemAddedInternal(slotResourceId, false);
                }

                if (_usedSlotsIndices.Count > 0)
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(() => { UsedSlotsChanged?.Invoke(_usedSlotsIndices); });
                }
            }
        }

        private void UnsubscribeFromDollEntityUsedSlots(ICharacterDollClientBroadcast characterDollClientBroadcast)
        {
            characterDollClientBroadcast.UsedSlots.OnItemAdded -= OnUsedSlotsItemAdded;
            characterDollClientBroadcast.UsedSlots.OnItemRemoved -= OnUsedSlotsItemRemoved;
            while (_usedSlotsIndices.Count > 0)
                _usedSlotsIndices.RemoveAt(_usedSlotsIndices.Count - 1);
            UsedSlotsChanged = null;
        }
    }
}