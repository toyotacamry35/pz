using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor.SceneProcessors
{
    public static class SceneHelpers
    {
        public static IEnumerable<GameObject> CollectGameObjectsWithComponent<T>(this Scene scene, bool includeInactive = true) where T : Component
        {
            foreach (GameObject obj in scene.GetRootGameObjects())
                foreach (T cmp in obj.GetComponentsInChildren<T>(includeInactive))
                    yield return cmp.gameObject;
        }

        public static IEnumerable<T> CollectComponents<T>(this Scene scene, bool includeInactive = true)
        {
            foreach (GameObject obj in scene.GetRootGameObjects())
                foreach (T cmp in obj.GetComponentsInChildren<T>(includeInactive))
                    yield return cmp;
        }
    }
}