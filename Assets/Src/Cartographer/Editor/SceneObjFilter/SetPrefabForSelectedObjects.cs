using UnityEditor;
using UnityEngine;

namespace Assets.Src.Cartographer.Editor
{
    class SetPrefabForSelectedObject : ScriptableWizard
    {
        public bool DisconnectedOnly = false;
        public GameObject Prefab;

        [MenuItem("Level Design/Set prefab")]

        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard("Set prefab for selected objects", typeof(SetPrefabForSelectedObject), "Set prefab");
        }

        void OnWizardCreate()
        {
            var prefab = PrefabUtility.GetPrefabObject(Prefab);
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Selected object is not a prefab or doesn't have parent prefab", "OK");
                return;
            }
            var selected = Selection.objects;
            foreach (GameObject go in selected)
            {
                var position = go.transform.position;
                var rotation = go.transform.rotation;
                var localScale = go.transform.localScale;
                if (DisconnectedOnly && PrefabUtility.GetPrefabType(go) != PrefabType.DisconnectedPrefabInstance)
                    continue;

                var newGo = PrefabUtility.ConnectGameObjectToPrefab(go, Prefab);
                newGo.transform.position = position;
                newGo.transform.rotation = rotation;
                newGo.transform.localScale = localScale;
            }
        }
    }

    public class ReplaceWithPrefab : EditorWindow
    {
        [SerializeField] private GameObject prefab;

        [MenuItem("Level Design/Replace With Prefab")]
        static void CreateReplaceWithPrefab()
        {
            EditorWindow.GetWindow<ReplaceWithPrefab>();
        }

        private void OnGUI()
        {
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

            if (GUILayout.Button("Replace"))
            {
                var selection = Selection.gameObjects;

                for (var i = selection.Length - 1; i >= 0; --i)
                {
                    var selected = selection[i];
                    var prefabType = PrefabUtility.GetPrefabType(prefab);
                    GameObject newObject;

                    if (prefabType == PrefabType.Prefab)
                    {
                        newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    }
                    else
                    {
                        newObject = Instantiate(prefab);
                        newObject.name = prefab.name;
                    }

                    if (newObject == null)
                    {
                        Debug.LogError("Error instantiating prefab");
                        break;
                    }

                    Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                    newObject.transform.parent = selected.transform.parent;
                    newObject.transform.localPosition = selected.transform.localPosition;
                    newObject.transform.localRotation = selected.transform.localRotation;
                    newObject.transform.localScale = selected.transform.localScale;
                    newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                    Undo.DestroyObjectImmediate(selected);
                }
            }

            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
        }
    }
}
