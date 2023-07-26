using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects;
using Assets.Src.SpawnSystem;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;

namespace Assets.Src.ContainerApis
{
    public class EntityApiWrapper<T> : IHasEntityApi where T : EntityApi, new()
    {
        public T EntityApi { get; private set; }

        public EntityApi Api => EntityApi;

        /// <summary>
        /// Имеет собственный EntityApi, не принадлежащий какой-либо коллекции, который удаляет при собственном удалении
        /// </summary>
        private readonly bool _hasSelfEntityApy;

        private readonly bool _removeOnDispose;
        private EntityGameObject _ego;

        public EntityApiWrapper(OuterRef<IEntityObject> outerRef, EntityGameObject ego, bool removeOnDispose = false)
        {
            if (ego.AssertIfNull(nameof(ego)))
                return;

            if (ego.EntityApis == null)
                ego.EntityApis = new List<EntityApi>();

            _ego = ego;
            _removeOnDispose = removeOnDispose;
            var suitable = ego.EntityApis.FirstOrDefault(api => api.GetType() == typeof(T));
            if (suitable != null)
            {
                EntityApi = (T) suitable;
            }
            else
            {
                EntityApi = new T();
                EntityApi.Init(outerRef.Guid, outerRef.TypeId, ego.name);
                ego.EntityApis.Add(EntityApi);
                var retransator = ego.gameObject.GetComponentInChildren<TmpPlugEntityApiEgoComponentRetransator>();
                if (!retransator.AssertIfNull(nameof(retransator), ego.gameObject))
                    retransator.Subscribe_TmpPlug_GotLostClient(EntityApi, SubscribeUnsubscribe.Subscribe);
            }
        }

        /// <summary>
        /// Создание экземпляра для IInteractiveEntity без GameObject на сцене
        /// </summary>
        public EntityApiWrapper(OuterRef<IEntityObject> outerRef)
        {
            _hasSelfEntityApy = true;
            EntityApi = new T();
            EntityApi.Init(outerRef.Guid, outerRef.TypeId, "none");
            EntityApi.OnGotClient(); //вручную
        }

        public void Dispose()
        {
            if (_removeOnDispose)
            {
                if (_ego != null && _ego.EntityApis != null && _ego.EntityApis.Contains(EntityApi))
                {
                    _ego.EntityApis.Remove(EntityApi);
                }

                EntityApi.Dispose();
            }
            else
            {
                if (_hasSelfEntityApy)
                    EntityApi.Dispose();
            }

            EntityApi = null;
        }
    }
}