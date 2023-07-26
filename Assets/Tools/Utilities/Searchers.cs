#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Utilities
{
    public class Searchers : EditorWindow
    {
        [MenuItem("Tools/Scenes View Counters", false, 102)]
        public static void RequestSearchers()
        {
            var editor = EditorWindow.GetWindow<Searchers>();
            editor.titleContent = new GUIContent("Scenes View Counters");
        }

        private int _allObjectsCount = -1;
        private int _objectsWithComponentCount = -1;
        private int _objectsWithLayerCount = -1;
        private int _combinedCount = -1;
        private string _selectedComponent;
        private string _layerName;
        void OnGUI()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Count all objects in a scene"))
            {
                var allGos = GameObject.FindObjectsOfType<GameObject>();
                _allObjectsCount = allGos.Length;
            }
            if (_allObjectsCount != -1)
                GUILayout.Label("All objects count: " + _allObjectsCount);
            _selectedComponent = EditorGUILayout.TextField("searched component: ", _selectedComponent);
            if (GUILayout.Button("Count all with the component"))
            {
                var allGos = GameObject.FindObjectsOfType<GameObject>();
                _allObjectsCount = allGos.Length;
                _objectsWithComponentCount = 0;
                for (int i = 0; i < allGos.Length; i++)
                    if (allGos[i].GetComponent(_selectedComponent) != null)
                        _objectsWithComponentCount++;
            }

            if (_objectsWithComponentCount != -1)
                GUILayout.Label("All objects with a component count: " + _objectsWithComponentCount);
            _layerName = EditorGUILayout.TextField("searched layer: ", _layerName);
            if (GUILayout.Button("Search all with a layer"))
            {
                int layerMask = LayerMask.GetMask(_layerName);
                _objectsWithLayerCount = 0;
                var allGos = GameObject.FindObjectsOfType<GameObject>();
                _allObjectsCount = allGos.Length;
                for (int i = 0; i < allGos.Length; i++)
                    if (allGos[i].layer == layerMask)
                        _objectsWithLayerCount++;
            }

            if (_objectsWithLayerCount != -1)
                GUILayout.Label("All objects on a layer: " + _objectsWithLayerCount);
            if (GUILayout.Button("Combined search"))
            {

                var allGos = GameObject.FindObjectsOfType<GameObject>();
                _allObjectsCount = allGos.Length;
                _combinedCount = 0;
                int layerMask = LayerMask.GetMask(_layerName);
                for (int i = 0; i < allGos.Length; i++)
                    if (allGos[i].layer == layerMask)
                        if (allGos[i].GetComponent(_selectedComponent) != null)
                            _combinedCount++;
            }
            if (_combinedCount != -1)
                GUILayout.Label("Combined search objects count: " + _combinedCount);
            GUILayout.EndVertical();
        }


        //=== Prefab instances count on scene

        [MenuItem("Tools/Prefab instances count on scene &p", true)]
        public static bool IsPrefabSelected()
        {
            var prefab = Selection.activeObject;

            if (prefab == null)
                return false;

            return PrefabUtility.GetPrefabType(prefab) == PrefabType.Prefab;
        }

        private static Texture2D GenerationTreeIcon;
        [MenuItem("Tools/Prefab instances count on scene &p", false, 100)]
        public static void ProjectWindowItemOnGui()
        {
            var prefab = Selection.activeObject;
            int count = 0;
            var allGos = GameObject.FindObjectsOfType<GameObject>();
            foreach (var go in allGos)
            {
                if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance)
                {
                    UnityEngine.Object goPrefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                    if (prefab == goPrefab)
                        count++;
                }
            }
            Debug.LogFormat("Prefab '{0}' count on scene: {1}", prefab.name, count);
        }
        
    }
}
#endif