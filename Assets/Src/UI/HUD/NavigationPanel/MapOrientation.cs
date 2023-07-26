using UnityEngine;

namespace Uins
{
    public class MapOrientation : MonoBehaviour
    {
        //Z-ось данного объекта смотрит на север, относительно нее считаем угол на навигационной панели 

        public static MapOrientation Instance { get; private set; }

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}