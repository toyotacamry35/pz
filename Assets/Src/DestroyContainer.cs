using Assets.Src.Server.Impl;
using Assets.Src.Shared;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using UnityEngine;

namespace Assets.Src
{
    public class DestroyContainer : ColonyBehaviour
    {
        [SerializeField] float _destroyTime;        

        float _currentTime;
        bool _wasDestroy;

        private void Update()
        {
            _currentTime += Time.deltaTime;
            if (! _wasDestroy && _currentTime > _destroyTime)
            {
                _wasDestroy = true;
                DestroyEntity();
            }
        }

        void DestroyEntity()
        {
            // if (IsServer)
            // {
            //     var repo = ServerProvider.Server.EntitiesRepository;
            //     var ego = gameObject.GetComponent<EntityGameObject>();
            //     if (repo != null && ego != null)
            //     {
            //         var typeId = ego.TypeId;
            //         var characterId = ego.EntityId;
            //         AsyncUtilsUnity.RunAsyncTaskFromUnity(async () => 
            //         {
            //             await repo.Destroy(typeId, characterId);
            //         }).WrapErrors();
            //     }
            // }
            Destroy(gameObject);
        }
    }
}
