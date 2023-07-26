using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Tools.Editor
{
    class MissingScriptsFinder
    {
        [MenuItem("Window/Utilities/Find Missing Scripts")]
        public static void FindMissingScripts()
        {
            if(EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", "Enumerating Asset DB", 0.0f))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            var assetGuids = AssetDatabase.FindAssets("t: GameObject");
            var assetPaths = assetGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            for (int idx = 0; idx < assetPaths.Length; ++idx)
            {
                var path = assetPaths[idx];
                if(EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", $"Checking prefab {path}", (float)idx / (float)assetPaths.Length))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                FindInGO(go, path);
            }
            EditorUtility.UnloadUnusedAssetsImmediate(true);
            EditorUtility.ClearProgressBar();
        }

        private static void FindInGO(GameObject go, string prefabName = "")
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Transform t = go.transform;

                    string componentPath = go.name;
                    while (t.parent != null)
                    {
                        componentPath = t.parent.name + "/" + componentPath;
                        t = t.parent;
                    }
                    Debug.LogWarning("Prefab " + prefabName + " has an empty script attached:\n" + componentPath, go);
                }
            }

            foreach (Transform child in go.transform)
            {
                FindInGO(child.gameObject, prefabName);
            }
        }
    }
}
