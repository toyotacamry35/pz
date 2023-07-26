using UnityEditor;
using UnityEngine;

namespace Assets.Editor.Tools
{
    public class AddLodGroup : EditorWindow
    {
        [MenuItem("Tools/Tech Art/Create LOD", false)]
        private static void MenuItem()
        {
            var window = GetWindow<AddLodGroup>(true, "Add LOD Group");
            window.position = new Rect(200, 100, 200, 50);
        }
        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                if (GUILayout.Button("Create LODs", GUILayout.ExpandWidth(true)))
                    CreateLodGroup();
            }
            EditorGUILayout.HelpBox("Use this tool carefully and always check output results.", MessageType.Warning);
        }

        private void CreateLodGroup()
        {
            foreach (var selectedObject in Selection.gameObjects)
            {
                var status = PrefabUtility.GetPrefabInstanceStatus(selectedObject);
                if (status == PrefabInstanceStatus.Connected)
                {
                    var prefabInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(selectedObject);
                    if (prefabInstanceRoot == null)
                    {
                        Debug.LogError($"Can't get prefab instance for {nameof(GameObject)} {selectedObject.name}", selectedObject);
                        continue;
                    }
                    var assetPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(prefabInstanceRoot));
                    if (MoveRenderersToLod(selectedObject, true, assetPath))
                    {
                        AssetDatabase.SaveAssets();
                        Debug.Log($"Successfully created LOD group for {nameof(GameObject)} {selectedObject.name} from prefab instance {prefabInstanceRoot}", selectedObject);
                    }
                    else
                    {
                        Debug.LogError($"Failed to create LOD group for {nameof(GameObject)} {selectedObject.name} from prefab instance {prefabInstanceRoot}", selectedObject);
                    }
                }
                else
                {
                    if (MoveRenderersToLod(selectedObject, false, null))
                    {
                        Debug.Log($"Successfully created LOD group for {nameof(GameObject)} {selectedObject.name} of scene {selectedObject.scene}", selectedObject);
                    }
                    else
                    {
                        Debug.LogError($"Failed to create LOD group for {nameof(GameObject)} {selectedObject.name}  of scene {selectedObject.scene}", selectedObject);
                    }
                }
            }
        }

        private bool MoveRenderersToLod(GameObject gameObject, bool isAsset, string assetPath)
        {
            if (GetSingleComponent<MeshFilter>(gameObject, out var meshFilter) && GetSingleComponent<MeshRenderer>(gameObject, out var meshRenderer))
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    var child = gameObject.transform.GetChild(i);
                    if (child.name.Contains("LOD"))
                    {
                        Debug.LogError($"{nameof(GameObject)} {gameObject.name} already contains LOD group", gameObject);
                        return false;
                    }
                }

                var lodGroup = gameObject.GetComponent<LODGroup>();
                if (lodGroup)
                {
                    DestroyImmediate(lodGroup, true);
                }

                var lodChild = new GameObject("LOD_0");
                lodChild.transform.parent = gameObject.transform;
                var newMeshFilter = lodChild.AddComponent<MeshFilter>();
                var newMeshRenderer = lodChild.AddComponent<MeshRenderer>();
                EditorUtility.CopySerialized(meshFilter, newMeshFilter);
                EditorUtility.CopySerialized(meshRenderer, newMeshRenderer);

                lodGroup = gameObject.AddComponent<LODGroup>();
                LOD[] lods = new LOD[1];
                Renderer[] renderers = new Renderer[1] { newMeshRenderer };
                lods[0] = new LOD(0.05f, renderers);
                lodGroup.SetLODs(lods);

                if (isAsset)
                {
                    PrefabUtility.ApplyAddedGameObject(lodChild, assetPath, InteractionMode.UserAction);
                    var meshRendererComponentOnPrefab = PrefabUtility.GetCorrespondingObjectFromSource(meshRenderer);
                    var meshFilterComponentOnPrefab = PrefabUtility.GetCorrespondingObjectFromSource(meshFilter);
                    DestroyImmediate(meshFilter, true);
                    DestroyImmediate(meshRenderer, true);
                    PrefabUtility.ApplyRemovedComponent(gameObject, meshRendererComponentOnPrefab, InteractionMode.UserAction);
                    PrefabUtility.ApplyRemovedComponent(gameObject, meshFilterComponentOnPrefab, InteractionMode.UserAction);
                    PrefabUtility.ApplyAddedComponent(lodGroup, assetPath, InteractionMode.UserAction);
                }
                else
                {
                    DestroyImmediate(meshFilter, true);
                    DestroyImmediate(meshRenderer, true);
                }
                return true;
            }
            else
                return false;
        }

        public bool GetSingleComponent<T>(GameObject gameObject, out T component) where T : Component
        {
            component = null;
            var components = gameObject.GetComponents<T>();
            if (components.Length > 1)
            {
                Debug.LogError($"More than one {typeof(T)} found on {nameof(GameObject)} {gameObject.name}", gameObject);
                return false;
            }
            if (components.Length == 0)
            {
                Debug.LogError($"No {typeof(T)} found on {nameof(GameObject)} {gameObject.name}", gameObject);
                return false;
            }
            component = components[0];
            return true;
        }
    }
}