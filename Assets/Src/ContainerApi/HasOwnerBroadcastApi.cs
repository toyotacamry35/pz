using System;
using UnityEngine;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;

namespace Assets.Src.ContainerApis
{
    public class HasOwnerBroadcastApi : EntityApi
    {
        private const string HasOwnerPropertyName = nameof(IHasOwnerClientBroadcast.OwnerInformation.Owner);

        public event EntityGuidDelegate OwnerGuidEvent;

        public Guid OwnerGuid { get; private set; }


        //=== Public ==========================================================

        public void SubscribeToOwnerGuid(EntityGuidDelegate onOwnerGuidReceived)
        {
            if (onOwnerGuidReceived.AssertIfNull(nameof(onOwnerGuidReceived)))
                return;

            OnSubscribeToOwnerGuidRequest(onOwnerGuidReceived).WrapErrors();
        }

        public void UnsubscribeFromOwnerGuid(EntityGuidDelegate onOwnerGuidReceived)
        {
            if (onOwnerGuidReceived.AssertIfNull(nameof(onOwnerGuidReceived)))
                return;

            OwnerGuidEvent -= onOwnerGuidReceived;
        }


        //=== Protected =======================================================

        protected override Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            var hasOwnerClientBroadcast = (IHasOwnerClientBroadcast) wrapper;
            if (hasOwnerClientBroadcast.AssertIfNull(nameof(hasOwnerClientBroadcast)))
                return Task.CompletedTask;

            var ownerInformation = hasOwnerClientBroadcast.OwnerInformation;
            if (ownerInformation.AssertIfNull(nameof(ownerInformation)))
                return Task.CompletedTask;

            ownerInformation.SubscribePropertyChanged(HasOwnerPropertyName, OnHasOwnerChanged);

            OwnerGuid = hasOwnerClientBroadcast.OwnerInformation.Owner.Guid;
            if (OwnerGuid != Guid.Empty)
            {
                UnityQueueHelper.RunInUnityThreadNoWait(() => OwnerGuidEvent?.Invoke(OwnerGuid));
            }

            return Task.CompletedTask;
        }

        protected override Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            OwnerGuid = Guid.Empty;
            var hasOwnerClientBroadcast = (IHasOwnerClientBroadcast) wrapper;
            if (hasOwnerClientBroadcast == null)
                return Task.CompletedTask;

            var ownerInformation = hasOwnerClientBroadcast.OwnerInformation;
            if (ownerInformation.AssertIfNull(nameof(ownerInformation)))
                return Task.CompletedTask;

            ownerInformation.UnsubscribePropertyChanged(HasOwnerPropertyName, OnHasOwnerChanged);
            return Task.CompletedTask;
        }


        //=== Private =========================================================

        private Task OnHasOwnerChanged(EntityEventArgs args)
        {
            var ownerRef = args.NewValue != null ? (OuterRef<IEntity>) args.NewValue : default;
            OwnerGuid = ownerRef.Guid;
            UnityQueueHelper.RunInUnityThreadNoWait(() => OwnerGuidEvent?.Invoke(OwnerGuid));
            return Task.CompletedTask;
        }

        private Task OnSubscribeToOwnerGuidRequest(EntityGuidDelegate onOwnerGuid)
        {
            OwnerGuidEvent += onOwnerGuid;
            if (OwnerGuid != Guid.Empty)
                UnityQueueHelper.RunInUnityThreadNoWait(() => onOwnerGuid.Invoke(OwnerGuid));
            return Task.CompletedTask;
        }
    }
}