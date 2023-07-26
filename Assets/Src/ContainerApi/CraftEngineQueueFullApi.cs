using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Shared;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using SharedCode.EntitySystem;
using Uins;
using UnityEngine;

namespace Assets.Src.ContainerApis
{
    public delegate void QueueSlotChangedDelegate(int slotIndex, CraftQueueSlot craftQueueSlot);

    public delegate void QueueEventDelegate(CraftQueueEvent craftQueueEvent);

    public enum CraftQueueEvent
    {
        TaskAdded,
        TaskCompleted,
        HandcraftQueueStarted,
        QueueCompleted,
        QueuePaused,
        BenchSequenceStarted,
        BenchSequenceEnded,
    }

    public class CraftEngineQueueFullApi : EntityApi
    {
        [HideInInspector]
        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;

        private CraftVirtualQueue _virtualQueue;
        private CraftQueueListeners _listeners;
        private event QueueEventDelegate QueueEvent;


        //=== Props ===========================================================

        public bool IsBenchSequenceStarted { get; private set; }


        //=== Unity ===========================================================

        public override void Init(Guid entityGuid, int entityTypeId, string debugName)
        {
            base.Init(entityGuid, entityTypeId, debugName);
            _virtualQueue = new CraftVirtualQueue(this);
            TimeTicker.Instance.GetLocalTimer(0.2f).Action(D, dt => _virtualQueue.OnUpdate());
        }


        //=== Public ==========================================================

        public void SubscribeToCraftQueueSlot(int slotIndex, QueueSlotChangedDelegate onItemChanged)
        {
            if (onItemChanged.AssertIfNull(nameof(onItemChanged)))
                return;

            _virtualQueue.OnSubscribeRequest(slotIndex, onItemChanged);
        }

        public void UnsubscribeFromCraftQueueSlot(int slotIndex, QueueSlotChangedDelegate onItemChanged)
        {
            if (onItemChanged.AssertIfNull(nameof(onItemChanged)))
                return;

            _virtualQueue.OnUnsubscribeRequest(slotIndex, onItemChanged);
        }

        public void SubscribeToCraftQueueEvents(QueueEventDelegate onQueueEvent)
        {
            if (onQueueEvent.AssertIfNull(nameof(onQueueEvent)))
                return;

            QueueEvent += onQueueEvent;
        }

        public void UnsubscribeFromCraftQueueEvents(QueueEventDelegate onQueueEvent)
        {
            if (onQueueEvent.AssertIfNull(nameof(onQueueEvent)))
                return;

            QueueEvent -= onQueueEvent;
        }

        public void SendQueueEvent(CraftQueueEvent craftQueueEvent)
        {
            if (craftQueueEvent == CraftQueueEvent.BenchSequenceStarted)
                IsBenchSequenceStarted = true;
            else if (craftQueueEvent == CraftQueueEvent.BenchSequenceEnded)
                IsBenchSequenceStarted = false;

            QueueEvent?.Invoke(craftQueueEvent);
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            _listeners = new CraftQueueListeners(_virtualQueue);
            if (!await TryGetCraftEngineClientFull(wrapper, OnGetCraftEngineClientFullAtStart))
                UI. Logger.IfError()?.Message("Unable to get CraftEngineClientFull at start").Write();;
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            if (!await TryGetCraftEngineClientFull(wrapper, OnGetCraftEngineClientFullAtEnd))
                UI. Logger.IfError()?.Message("Unable to get CraftEngineClientFull at end").Write();;
        }


        //=== Private =========================================================

        private async Task<bool> TryGetCraftEngineClientFull(IEntity entityWrapper, Func<ICraftEngineClientFull, Task> onSuccess)
        {
            if (entityWrapper.AssertIfNull(nameof(entityWrapper)) ||
                onSuccess.AssertIfNull(nameof(onSuccess)))
                return false;

            using (var craftEngineWrapper = await NodeAccessor.Repository.Get<ICraftEngineClientFull>(entityWrapper.Id, entityWrapper.TypeId))
            {
                var craftEngineClientFull = craftEngineWrapper.Get<ICraftEngineClientFull>(entityWrapper.Id);
                if (craftEngineClientFull.AssertIfNull(nameof(craftEngineClientFull)))
                    return false;
                await onSuccess.Invoke(craftEngineClientFull);
            }

            return true;
        }

        private Task OnGetCraftEngineClientFullAtStart(ICraftEngineClientFull craftEngineClientFull)
        {
            _listeners.Subscribe(craftEngineClientFull.CraftingQueue);
            return Task.CompletedTask;
        }

        private Task OnGetCraftEngineClientFullAtEnd(ICraftEngineClientFull craftEngineClientFull)
        {
            _listeners.Unsubscribe(craftEngineClientFull.CraftingQueue);
            return Task.CompletedTask;
        }
    }
}