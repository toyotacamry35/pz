using UnityEngine;

namespace Assets.Src.Target
{
    public class TriggerEvent : MonoBehaviour
    {
        public event System.Action<Collider> TriggerStayEvent;
        public event System.Action<Collider> TriggerEnterEvent;
        public event System.Action<Collider> TriggerExitEvent;

        void OnTriggerStay(Collider other)
        {
            TriggerStayEvent?.Invoke(other);
        }

        void OnTriggerEnter(Collider other)
        {
            TriggerEnterEvent?.Invoke(other);
        }

        void OnTriggerExit(Collider other)
        {
            TriggerExitEvent?.Invoke(other);
        }
    }
}
