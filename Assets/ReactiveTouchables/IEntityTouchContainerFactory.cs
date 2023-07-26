using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public interface IEntityTouchContainerFactory<T> where T : IEntity
    {
        EntityTouchContainer<T> GetNewContainer(EntityTouchContainer<T> lastContainer);
        EntityTouchContainer<T> GetNewContainer(IEntitiesRepository repo, int typeId, Guid entityId, ReplicationLevel replicationLevel);
    }
}