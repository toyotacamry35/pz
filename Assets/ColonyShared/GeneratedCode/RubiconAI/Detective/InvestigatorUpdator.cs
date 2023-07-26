using System;
using UnityEngine;

namespace Assets.Src.Detective
{
    public class InvestigatorUpdator : MonoBehaviour
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        void Update()
        {
            OnUpdate?.Invoke();
        }

        void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
    }
}