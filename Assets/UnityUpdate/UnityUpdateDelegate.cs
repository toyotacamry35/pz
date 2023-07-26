using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityUpdate
{
    [AddComponentMenu("")]
    public class UnityUpdateDelegate : MonoBehaviour
    {
        private event Action _onUpdate;
        public static event Action OnUpdate
        {
            add
            {
                if (value == null)
                    return;

                if (!_instance)
                {
                    _instance = new GameObject("UnityUpdateDelegate").AddComponent<UnityUpdateDelegate>();
                    DontDestroyOnLoad(_instance);
                }

                _instance._onUpdate += new DelegateWrapper(value).Invoke;
            }
            remove
            {
                if (!_instance)
                    return;

                var existing = _instance._onUpdate.GetInvocationList().Where(v => ((DelegateWrapper)v.Target).Action == value).SingleOrDefault();
                if (existing == null)
                    return;

                _instance._onUpdate -= (Action)existing;

                if (_instance._onUpdate == null)
                {
                    Destroy(_instance);
                    _instance = null;
                }
            }
        }

        private static UnityUpdateDelegate _instance;

        private void Update()
        {
            _onUpdate?.Invoke();
        }

        private class DelegateWrapper
        {
            private readonly CustomSampler _sampler;
            public  readonly Action Action;

            public DelegateWrapper(Action action)
            {
                Action = action;
                _sampler = CustomSampler.Create($"{action.Method.ReflectedType.Name}.{action.Method.Name}");
            }

            public void Invoke()
            {
                _sampler.Begin();
                Action();
                _sampler.End();
            }
        }
    }
}
