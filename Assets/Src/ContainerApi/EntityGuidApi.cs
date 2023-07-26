using System;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace Assets.Src.ContainerApis
{
    public delegate void EntityGuidDelegate(Guid entityGuid);

    public class EntityGuidApi : EntityApi
    {
        private event EntityGuidDelegate EntityGuidEvent;


        //=== Public ==========================================================

        public void SubscribeOnEntityGuid(EntityGuidDelegate onEntityGuidReceived)
        {
            if (onEntityGuidReceived.AssertIfNull(nameof(onEntityGuidReceived)))
                return;

            EntityGuidEvent += onEntityGuidReceived;
            if (EntityGuid != Guid.Empty)
                onEntityGuidReceived.Invoke(EntityGuid);
        }

        public void UnsubscribeFromEntityGuid(EntityGuidDelegate onEntityGuidReceived)
        {
            if (onEntityGuidReceived.AssertIfNull(nameof(onEntityGuidReceived)))
                return;

            EntityGuidEvent -= onEntityGuidReceived;
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            await base.OnWrapperReceivedAtStart(wrapper);

            if (EntityGuidEvent != null)
                UnityQueueHelper.RunInUnityThreadNoWait(() => { EntityGuidEvent.Invoke(EntityGuid); });
        }
    }
}