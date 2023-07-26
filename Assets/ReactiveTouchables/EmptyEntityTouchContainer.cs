using System;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public class EmptyEntityTouchContainer<T> : EntityTouchContainer<T> where T : IEntity
    {
        public EmptyEntityTouchContainer(EntityTouchContainer<T> source) : base(source.Repo, source.TypeId, source.EntityId, source.ReplicationLevel)
        {
        }

        public override Task Connect() {
            return EmptyEntity();
        }

        private async Task EmptyEntity()
        {
            lock (_callbacksLock)
            {
                //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] EmptyEntityTouchContainer.EmptyEntity()");
                WasSucessful = true;
                try {
                    onEntity?.Invoke(default);
                } catch (Exception e) {
                    ReactiveLogs.Logger.Error(e);
                }
                onEntity = null;
                onNoEntity = null;
            }
        }

    }
}
