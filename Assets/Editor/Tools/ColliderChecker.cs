using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tools
{
    public class ColliderChecker : EditorWindow
    {
        private CheckerOptions options = new CheckerOptions();
        private List<Object> objectList = new List<Object>();

        [MenuItem("Tools/Tech Art/Check Collider Sizes", false)]
        private static void MenuItem()
        {
            ShowWindow();
        }

        private static void ShowWindow()
        {
            var window = GetWindow<ColliderChecker>(true, "Collider Checker");
            window.position = new Rect(100, 100, 190, 110);
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                options.minSize = EditorGUILayout.FloatField(new GUIContent("Минимальный размер", "Коллайдер попадёт в список, если его bounding box меньше указанного размера по любому измерению"), options.minSize);
            }
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                options.convexOnly = EditorGUILayout.Toggle(new GUIContent("Только конвексные", "Проверять только конвексные коллайдеры"), options.convexOnly);
            }
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                options.logAll = EditorGUILayout.Toggle(new GUIContent("Логировать все", "Записывать в лог инстанцировнаие всех проверенных коллайдеров"), options.logAll);
            }
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Поиск", GUILayout.ExpandWidth(true)))
                        FindColliders();
                    if (GUILayout.Button("Выделить", GUILayout.ExpandWidth(true)))
                        SelectColliders();
                }
            }
        }

        private void SelectColliders()
        {
            if (!objectList.IsNullOrEmptyOrHasNullElements(nameof(objectList)))
                Selection.objects = objectList.ToArray();
        }

        private void FindColliders()
        {
            var objects = AssetDatabase.FindAssets("t:Prefab").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadMainAssetAtPath);
            foreach (var obj in objects)
                if (obj is GameObject gObj)
                {
                    var instantiatedObj = GameObject.Instantiate(gObj);
                    instantiatedObj.SetActive(true);
                    var colliders = instantiatedObj.GetComponentsInChildren<Collider>();
                    foreach (var collider in colliders)
                    {
                        collider.enabled = true;
                        var current = collider.gameObject;
                        while (current != null)
                        {
                            current.SetActive(true);
                            current = current.transform.parent?.gameObject;
                        }
                    }

                    foreach (var collider in colliders)
                    {
                        if (collider is MeshCollider meshCollider)
                        {
                            if (!options.convexOnly || meshCollider.convex)
                            {
                                if (CheckColliderBounds(meshCollider.bounds))
                                    Debug.LogWarning($"'{gObj.name}': too small collider '{meshCollider.name}'", obj);

                                if (meshCollider.convex)
                                {
                                    if (meshCollider.sharedMesh == default)
                                    {
                                        Debug.LogWarning($"'{gObj.name}': no mesh is set for collider '{meshCollider.name}'", obj);
                                    }
                                    else
                                    {
                                        var vertCount = meshCollider.sharedMesh.vertexCount;
                                        if (vertCount <= 3)
                                            Debug.LogWarning($"'{gObj.name}': only {vertCount} vertices in collider '{meshCollider.name}'", obj);
                                    }
                                }
                            }
                        }
                        if (options.logAll)
                            Debug.Log(gObj.name, obj);
                    }
                    GameObject.DestroyImmediate(instantiatedObj);
                }
        }

        private bool CheckColliderBounds(Bounds bounds)
        {
            var minSize = options.minSize;
            return bounds.size.x < minSize || bounds.size.x < minSize || bounds.size.z < minSize;
        }

        private class CheckerOptions
        {
            public float minSize = 0.1f;
            public bool convexOnly = true;
            public bool logAll = false;
        }
    }
}