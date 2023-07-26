using System;
using System.Threading.Tasks;
using Assets.Tools;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    public class UnityEntityTouchContainer<T> : EntityTouchContainer<T> where T : IEntity
    {
        public UnityEntityTouchContainer(IEntitiesRepository repo, int typeId, Guid entityId, ReplicationLevel replicationLevel) 
            : base(repo, typeId, entityId, replicationLevel)
        {
        }

        public UnityEntityTouchContainer(EntityTouchContainer<T> source) : this(source.Repo, source.TypeId, source.EntityId,
            source.ReplicationLevel)
        {
        }

        public override async Task Connect()
        {
            await AsyncUtils.RunAsyncTask(() => InternalConnect());
        }
    }
}