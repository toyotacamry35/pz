using UnityEngine;

namespace Assets.Src.Character.Events
{
    public class ObjectDelayedRemover : MonoBehaviour
    {
        public float DestroyDelay = 0.0f;

        public void Start()
        {
            if (DestroyDelay != 0.0f)
                Destroy(gameObject, DestroyDelay);
        }

        public void DestroyWithDelay(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}
