using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.Colliders
{
    [CustomEditor(typeof(ColliderSystem))]

    public class ColliderSystemEditor : VegetationStudioBaseEditor
    {
        private ColliderSystem _colliderSystem;

        private int _currentTabIndex;
        private static readonly string[] TabNames =
        {
            "Settings", "Editor","Navmesh","Debug"
        };

        public override void OnInspectorGUI()
        {
            OverrideLogoTextureName = "SectionBanner_ColliderSystem";
            HelpTopic = "home/vegetation-studio/components/collider-system";
            LargeLogo = false;
            _colliderSystem = (ColliderSystem)target;

            ShowLogo = !_colliderSystem.VegetationSystem.GetSleepMode();

            base.OnInspectorGUI();          

            if (_colliderSystem.VegetationSystem.GetSleepMode())
            {
                EditorGUILayout.HelpBox("Wake up the Vegetation System from sleepmode to edit settings", MessageType.Info);
                return;
            }

            if (!_colliderSystem.VegetationSystem.InitDone)
            {
                GUILayout.BeginVertical("box");
                EditorGUILayout.HelpBox("Vegetation system component has configuration errors. Fix to enable component.", MessageType.Error);
                GUILayout.EndVertical();
                return;
            }

            _currentTabIndex = GUILayout.SelectionGrid(_currentTabIndex, TabNames, 4, EditorStyles.toolbarButton);

            switch (_currentTabIndex)
            {
                case 0:
                    DrawSettingsInspector();
                    break;
                case 1:
                    DrawEditorInspector();
                    break;
                case 2:
                    DrawNavMeshInspector();
                    break;
                case 3:
                    DrawDebugInspector();
                    break;
            }
        }


        private void DrawNavMeshInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Navmesh baker", LabelStyle);

            EditorGUILayout.HelpBox("Baking colliders for navmesh will generate a root gameobject in the scene that includes children with colliders. Use this to bake the navmesh and then delete from the scene.", MessageType.Info);

            if (GUILayout.Button("Bake all colliders to scene mesh"))
            {
                _colliderSystem.BakeNavmeshColliders(true);
            }

            if (GUILayout.Button("Bake all colliders to scene"))
            {
                _colliderSystem.BakeNavmeshColliders(false);
            }
            GUILayout.EndVertical();
        }

        private void DrawSettingsInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Collider info", LabelStyle);

            EditorGUILayout.HelpBox("Creates colliders for objects in front of and close to the camera.", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            _colliderSystem.ColliderRange = (ColliderRange)EditorGUILayout.EnumPopup("Range", _colliderSystem.ColliderRange);
            if (EditorGUI.EndChangeCheck())
            {
                _colliderSystem.RefreshColliders();
            }

            EditorGUILayout.HelpBox("Range is relative to vegetation system vegetation distance. Restrict range to what you need. Many colliders can reduce FPS.", MessageType.Info);

            if (_colliderSystem.ColliderSystemType != ColliderSystemType.Disabled)
            {
                EditorGUILayout.HelpBox("Vegetation items of type Object, Large Object and trees can use colliders. Colliders needs to be enabled and configured on the vegetation item setting.", MessageType.Info);
            }
            GUILayout.EndVertical();



            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Layers", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _colliderSystem.TreeLayer = EditorGUILayout.LayerField("Tree collider layer", _colliderSystem.TreeLayer);
            _colliderSystem.ObjectLayer = EditorGUILayout.LayerField("Object collider layer", _colliderSystem.ObjectLayer);
            _colliderSystem.LargeObjectLayer = EditorGUILayout.LayerField("Large Object collider layer", _colliderSystem.LargeObjectLayer);
            if (EditorGUI.EndChangeCheck())
            {
                _colliderSystem.RefreshColliders();
            }
            GUILayout.EndVertical();
        }

        private void DrawEditorInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Editor", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _colliderSystem.HideColliders = EditorGUILayout.Toggle("Hide colliders in hierarchy", _colliderSystem.HideColliders);
            if (EditorGUI.EndChangeCheck())
            {
                _colliderSystem.RefreshColliders();
            }
            GUILayout.EndVertical();
        }

        private void DrawDebugInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Debug info", LabelStyle);
            _colliderSystem.ShowCellGrid = EditorGUILayout.Toggle("Show included cells", _colliderSystem.ShowCellGrid);
            EditorGUILayout.LabelField("Included cells: " + _colliderSystem.CellCount.ToString(), LabelStyle);
            EditorGUILayout.LabelField("Colliders: " + _colliderSystem.ColliderCount.ToString(), LabelStyle);
            GUILayout.EndVertical();
        }
    }
}
