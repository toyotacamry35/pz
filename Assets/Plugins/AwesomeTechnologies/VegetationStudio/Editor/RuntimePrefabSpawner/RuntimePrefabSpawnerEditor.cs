using AwesomeTechnologies.Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.RuntimePrefabSpawner
{
    [CustomEditor(typeof(RuntimePrefabSpawner))]
    public class RuntimePrefabSpawnerEditor : VegetationStudioBaseEditor
    {
        private RuntimePrefabSpawner _runtimePrefabSpawner;

        private int _currentTabIndex;
        private static readonly string[] TabNames =
        {
            "Settings", "Editor","Debug"
        };

        public override void OnInspectorGUI()
        {
            HelpTopic = "runtime-prefab-spawner";
            _runtimePrefabSpawner = (RuntimePrefabSpawner)target;
            if (_runtimePrefabSpawner.VegetationSystem == null) _runtimePrefabSpawner.FindVegetationSystem();

            base.OnInspectorGUI();

            _currentTabIndex = GUILayout.SelectionGrid(_currentTabIndex, TabNames, 3, EditorStyles.toolbarButton);

            switch (_currentTabIndex)
            {
                case 0:
                    DrawSettingsInspector();
                    break;
                case 1:
                    DrawEditorInspector();
                    break;
                case 2:
                    DrawDebugInspector();
                    break;
            }
        }

        private void DrawEditorInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Editor", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _runtimePrefabSpawner.HideRuntimePrefabs = EditorGUILayout.Toggle("Hide instanced prefabs in hierarchy", _runtimePrefabSpawner.HideRuntimePrefabs);
           // _runtimePrefabSpawner.DisableInEditor = EditorGUILayout.Toggle("Disable in editor mode", _runtimePrefabSpawner.DisableInEditor);

            if (EditorGUI.EndChangeCheck())
            {
                _runtimePrefabSpawner.RefreshRuntimePrefabs();
            }
            GUILayout.EndVertical();
        }

        private void DrawDebugInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Debug info", LabelStyle);
            _runtimePrefabSpawner.ShowCellGrid = EditorGUILayout.Toggle("Show included cells", _runtimePrefabSpawner.ShowCellGrid);
            GUILayout.EndVertical();
        }

        void DrawSettingsInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select vegetation item", LabelStyle);

            EditorGUILayout.HelpBox("Select the VegetationItem you want to use to spawn run-time prefabs. If you want multiple prefabs or VegetationItems add multiple components. Prefabs will be spawned with same position, scale and rotation as the vegetation item.", MessageType.Info);
            EditorGUI.BeginChangeCheck();

            VegetationPackageEditorTools.DrawVegetationItemSelector(
                _runtimePrefabSpawner.VegetationSystem.currentVegetationPackage,
                VegetationPackageEditorTools.CreateVegetationInfoIdList(
                    _runtimePrefabSpawner.VegetationSystem.currentVegetationPackage,
                    new[] { VegetationType.Objects, VegetationType.Tree, VegetationType.LargeObjects ,VegetationType.Grass,VegetationType.Plant}), 60,
                ref _runtimePrefabSpawner.SelectedVegetationItemID);

            _runtimePrefabSpawner.RuntimePrefabRange = (RuntimePrefabRange)EditorGUILayout.EnumPopup("Range", _runtimePrefabSpawner.RuntimePrefabRange);
            EditorGUILayout.HelpBox("Range is relative to vegetation system vegetation distance. Normal (1/3), Long (2/3) and Very long (3/3). Restrict range to what you need.", MessageType.Info);
            if (EditorGUI.EndChangeCheck())
            {
                _runtimePrefabSpawner.RefreshRuntimePrefabs();

            }

            if (GUILayout.Button("Add run-time prefab rule"))
            {
                RuntimePrefabRule newRuntimePrefabRule =
                    new RuntimePrefabRule {VegetationItemID = _runtimePrefabSpawner.SelectedVegetationItemID};
                newRuntimePrefabRule.SetSeed();
                _runtimePrefabSpawner.RuntimePrefabRuleList.Add(newRuntimePrefabRule);
                _runtimePrefabSpawner.RefreshRuntimePrefabs();
            }               

            GUILayout.EndVertical();

            for (int i = 0; i <= _runtimePrefabSpawner.RuntimePrefabRuleList.Count - 1; i++)
            {
                RuntimePrefabRule runtimePrefabRule = _runtimePrefabSpawner.RuntimePrefabRuleList[i];

                if (runtimePrefabRule.VegetationItemID != _runtimePrefabSpawner.SelectedVegetationItemID) continue;

                EditorGUI.BeginChangeCheck();

                GUILayout.BeginVertical("box");
               

                runtimePrefabRule.RuntimePrefab = EditorGUILayout.ObjectField("Runtime prefab", runtimePrefabRule.RuntimePrefab, typeof(GameObject), true) as GameObject;

                var prefabTexture = AssetPreview.GetAssetPreview(runtimePrefabRule.RuntimePrefab);
                Texture2D convertedPrefabTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true, true);
                if (Application.isPlaying)
                {
                    convertedPrefabTexture = prefabTexture;
                }
                else
                {
                    if (prefabTexture)
                    {
                        convertedPrefabTexture.LoadImage(prefabTexture.EncodeToPNG());
                    }
                }

                if (convertedPrefabTexture)
                {
                    Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(convertedPrefabTexture.height));
                    float width = space.width;

                    space.xMin = (width - convertedPrefabTexture.width);
                    if (space.xMin < 0) space.xMin = 0;

                    space.width = convertedPrefabTexture.width;
                    space.height = convertedPrefabTexture.height;
                    EditorGUI.DrawPreviewTexture(space, convertedPrefabTexture);
                }
                runtimePrefabRule.SpawnFrequency = EditorGUILayout.Slider("Spawn frequency", runtimePrefabRule.SpawnFrequency, 0, 1f);
                runtimePrefabRule.PrefabScale = EditorGUILayout.Vector3Field("Scale", runtimePrefabRule.PrefabScale);
                runtimePrefabRule.UseVegetationItemScale = EditorGUILayout.Toggle("Add vegetation item scale", runtimePrefabRule.UseVegetationItemScale);
                runtimePrefabRule.PrefabRotation = EditorGUILayout.Vector3Field("Rotation", runtimePrefabRule.PrefabRotation);
                runtimePrefabRule.PrefabOffset = EditorGUILayout.Vector3Field("Offset", runtimePrefabRule.PrefabOffset);
                runtimePrefabRule.PrefabLayer = EditorGUILayout.LayerField("Prefab layer", runtimePrefabRule.PrefabLayer);
                runtimePrefabRule.Seed = EditorGUILayout.IntSlider("Seed", runtimePrefabRule.Seed,0,99);

                if (EditorGUI.EndChangeCheck())
                {
                    _runtimePrefabSpawner.RefreshRuntimePrefabs();
                }

                if (GUILayout.Button("Remove run-time prefab rule"))
                {
                    _runtimePrefabSpawner.RuntimePrefabRuleList.Remove(runtimePrefabRule);
                    _runtimePrefabSpawner.RefreshRuntimePrefabs();
                    return;
                }

                GUILayout.EndVertical();
            }

            return;

            //EditorGUI.BeginChangeCheck();

            //GUILayout.BeginVertical("box");
            //_runtimePrefabSpawner.RuntimePrefab = EditorGUILayout.ObjectField("Runtime prefab", _runtimePrefabSpawner.RuntimePrefab, typeof(GameObject), true) as GameObject;

            //var prefabTexture = AssetPreview.GetAssetPreview(_runtimePrefabSpawner.RuntimePrefab);
            //Texture2D convertedPrefabTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true, true);
            //if (Application.isPlaying)
            //{
            //    convertedPrefabTexture = prefabTexture;
            //}
            //else
            //{
            //    if (prefabTexture)
            //    {
            //        convertedPrefabTexture.LoadImage(prefabTexture.EncodeToPNG());
            //    }
            //}

            //if (convertedPrefabTexture)
            //{
            //    Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(convertedPrefabTexture.height));
            //    float width = space.width;

            //    space.xMin = (width - convertedPrefabTexture.width);
            //    if (space.xMin < 0) space.xMin = 0;

            //    space.width = convertedPrefabTexture.width;
            //    space.height = convertedPrefabTexture.height;
            //    EditorGUI.DrawPreviewTexture(space, convertedPrefabTexture);
            //}
            //_runtimePrefabSpawner.SpawnFrequency = EditorGUILayout.Slider("Spawn frequency", _runtimePrefabSpawner.SpawnFrequency, 0, 1f);
            //_runtimePrefabSpawner.PrefabScale = EditorGUILayout.Vector3Field("Scale", _runtimePrefabSpawner.PrefabScale);
            //_runtimePrefabSpawner.UseVegetationItemScale = EditorGUILayout.Toggle("Add vegetation item scale", _runtimePrefabSpawner.UseVegetationItemScale);
            //_runtimePrefabSpawner.PrefabRotation = EditorGUILayout.Vector3Field("Rotation", _runtimePrefabSpawner.PrefabRotation);
            //_runtimePrefabSpawner.PrefabOffset = EditorGUILayout.Vector3Field("Offset", _runtimePrefabSpawner.PrefabOffset);
            //_runtimePrefabSpawner.RuntimePrefabRange = (RuntimePrefabRange)EditorGUILayout.EnumPopup("Range", _runtimePrefabSpawner.RuntimePrefabRange);
            //_runtimePrefabSpawner.PrefabLayer = EditorGUILayout.LayerField("Prefab layer", _runtimePrefabSpawner.PrefabLayer);

            //EditorGUILayout.HelpBox("Range is relative to vegetation system vegetation distance. Normal (1/3), Long (2/3) and Very long (3/3). Restrict range to what you need.", MessageType.Info);

            //if (EditorGUI.EndChangeCheck())
            //{
            //    _runtimePrefabSpawner.RefreshRuntimePrefabs();
            //}

            //GUILayout.EndVertical();
        }
    }
}

