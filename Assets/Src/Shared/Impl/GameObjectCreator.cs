using Assets.Src.GameObjectAssembler;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Src.Shared.Impl
{
    public static class GameObjectCreator
    {
        private static CustomSampler _sampler;

        public static UnitySpawnService ClusterSpawnInstance;
        [NotNull]
        public static GameObject CreateGameObject([NotNull] GameObject prototype, Vector3 position = default, Quaternion rotation = default)
        {
            if (_sampler == null)
                _sampler = CustomSampler.Create("CreateGameObject");

            _sampler.Begin();
            var goInstance = JsonToGO.Instance.InstantiateAndMerge(prototype, position, rotation, false);
            goInstance.SetActive(true);

            _sampler.End();
            return goInstance;
        }
    }
}
