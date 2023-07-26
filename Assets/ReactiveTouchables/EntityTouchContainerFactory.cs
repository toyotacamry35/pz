using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public static class EntityTouchContainerFactory<T> where T : IEntity
    {
        public static IEntityTouchContainerFactory<T> Instance = new BaseEntityTouchContainerFactory<T>();

        private class BaseEntityTouchContainerFactory<T> : IEntityTouchContainerFactory<T> where T : IEntity
        {
            public EntityTouchContainer<T> GetNewContainer(EntityTouchContainer<T> lastContainer)
            {
                return new EntityTouchContainer<T>(lastContainer);
            }

            public EntityTouchContainer<T> GetNewContainer(IEntitiesRepository repo, int typeId, Guid entityId, ReplicationLevel replicationLevel)
            {
                return new EntityTouchContainer<T>(repo, typeId, entityId, replicationLevel);
            }
        }
    }
}