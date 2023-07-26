using UnityEngine;
using Utilities;

namespace Assets.Src.Effects.FX
{
    public class FXPoolElement : IGameObjectPoolElement
    {
        public GameObject GameObject { get; set; }
        public GameObject Prefab { get; set; }

        private IFXElementController[] _controllers;
        private bool[] _completedControllers;
        private bool _canLoop;
        private int _index;
        private Transform _transform;

        public void Init(GameObject gameObject, GameObject prefab)
        {
            GameObject = gameObject;
            Prefab = prefab;
            
            if (_controllers == null)
                _controllers = GameObject.GetComponentsInChildren<IFXElementController>(true);

            _completedControllers = new bool[_controllers.Length];
            for (int i = 0; i < _controllers.Length; i++)
            {
                _controllers[i].Completed += Complete;
                _controllers[i].Init();
                _completedControllers[i] = false;
            }

            _transform = GameObject.transform;
        }

        public void Show(Vector3 position, Quaternion rotation,IGameObjectPoolParams poolParams)
        {
            SetPositionAndRotation(position, rotation);
            Show(poolParams);
        }

        public void Show(IGameObjectPoolParams poolParams)
        {
            GameObject.SetActive(true);

            var fxParams = poolParams as FXElementParams;
            
            _index = 0;
            for (int i = 0; i < _controllers.Length; i++)
            {
                _completedControllers[i] = false;
                _controllers[i].Show(fxParams);
            }
        }

        public void Tick()
        {
            if (_index == _controllers.Length)
                Hide();

            for (int i = 0; i < _controllers.Length; i++)
            {
                if(!_completedControllers[i])
                    _controllers[i].Tick();
            }
        }

        private void Complete(IFXElementController fxElementController)
        {
            for (var i = 0; i < _controllers.Length; i++)
            {
                if (_controllers[i] == fxElementController)
                    _completedControllers[i] = true;
            }
            _index++;
        }

        public void Hide()
        {
            for (int i = 0; i < _controllers.Length; i++)
            {
                _controllers[i].Hide();
            }
            GameObject.SetActive(false);
            GameObjectPool.Instance.Return<FXPoolElement>(Prefab, this);
        }

        public void HideImmediately()
        {
            for (int i = 0; i < _controllers.Length; i++)
            {
                _controllers[i].HideImmediately();
            }

            GameObject.SetActive(false);
            GameObjectPool.Instance.Return<FXPoolElement>(Prefab, this);
        }

        private void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            _transform.position = position;
            _transform.rotation = rotation;
        }
    }
}