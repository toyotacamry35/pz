using UnityEngine;
using System.Collections;

namespace Utilities
{
    [ExecuteInEditMode]
    public class SceneClosingMarker : MonoBehaviour
    {
        public static bool SceneUnloaded = false;
        public void OnEnable()
        {
            SceneUnloaded = false;
        }

        public void OnDisable()
        {
            SceneUnloaded = true;
        }
    }

}
