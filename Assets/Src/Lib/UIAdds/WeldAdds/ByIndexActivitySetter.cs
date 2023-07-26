using System;
using UnityEngine;

namespace WeldAdds
{
    /// <summary>
    /// Включает только объект, индекс которого соответствует текущему Index
    /// </summary>
    [Obsolete]
    public class ByIndexActivitySetter : MonoBehaviour
    {
        public GameObject[] GameObjects;

        private int _lastIndex = -1;


        //=== Props ===========================================================

        public int Index { get; set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            Sync();
        }

        private void Update()
        {
            Sync();
        }


        //=== Private =========================================================

        private void Sync()
        {
            if (_lastIndex == Index)
                return;

            _lastIndex = Index;
            if (GameObjects == null || GameObjects.Length == 0)
                return;

            for (int i = 0; i < GameObjects.Length; i++)
            {
                var go = GameObjects[i];
                if (go != null)
                    go.SetActive(_lastIndex == i);
            }
        }
    }
}