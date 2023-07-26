using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public interface IGameObjectPoolElement
    {
        GameObject GameObject { get; set; }
        GameObject Prefab { get; set; }
        
        void Init(GameObject gameObject, GameObject prefab);
        void Show(Vector3 position, Quaternion rotation, IGameObjectPoolParams poolParams);
        void Show(IGameObjectPoolParams poolParams);
        void Tick();
        void Hide();
        void HideImmediately();
    }
}