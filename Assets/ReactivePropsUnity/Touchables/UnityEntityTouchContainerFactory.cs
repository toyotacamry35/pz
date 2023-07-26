using System;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public static class UnityEntityTouchContainerFactory<T> where T : IEntity
    {
        public static IEntityTouchContainerFactory<T> Instance = new EntityTouchContainerFactoryForUnity<T>();

        private class EntityTouchContainerFactoryForUnity<T> : IEntityTouchContainerFactory<T> where T : IEntity
        {
            public EntityTouchContainer<T> GetNewContainer(EntityTouchContainer<T> lastContainer)
            {
                return new UnityEntityTouchContainer<T>(lastContainer);
            }

            public EntityTouchContainer<T> GetNewContainer(IEntitiesRepository repo, int typeId, Guid entityId, ReplicationLevel replicationLevel)
            {
                return new UnityEntityTouchContainer<T>(repo, typeId, entityId, replicationLevel);
            }
        }
    }
}