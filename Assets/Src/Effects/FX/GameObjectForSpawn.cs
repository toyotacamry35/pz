using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public class GameObjectForSpawn
    {
        public float startedTime;
        public GameObjectPoolSettings settings;
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
        public IGameObjectPoolParams poolParams;

        public override string ToString()
        {
            return $"{nameof(settings)}: {settings}, {nameof(prefab)}: {prefab.name}";
        }
    }
}