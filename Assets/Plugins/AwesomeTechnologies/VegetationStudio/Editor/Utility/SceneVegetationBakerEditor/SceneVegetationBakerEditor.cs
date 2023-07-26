using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.Utility.Baking
{
    [CustomEditor(typeof(SceneVegetationBaker))]
    public class SceneVegetationBakerEditor : VegetationStudioBaseEditor
    {
        private SceneVegetationBaker _sceneVegetationBaker;
        public override void OnInspectorGUI()
        {
            HelpTopic = "scene-vegetation-baker";
            _sceneVegetationBaker = (SceneVegetationBaker)target;
            ShowLogo = !_sceneVegetationBaker.VegetationSystem.GetSleepMode();

            base.OnInspectorGUI();

            if (_sceneVegetationBaker.VegetationSystem.GetSleepMode())
            {
                EditorGUILayout.HelpBox("Wake up the Vegetation System from sleepmode to edit settings", MessageType.Info);
                return;
            }

            if (!_sceneVegetationBaker.VegetationSystem)
            {
                EditorGUILayout.HelpBox(
                    "The SceneVegetationBaker Component needs to be added to a GameObject with a VegetationSystem Component.",
                    MessageType.Error);
                return;
            }

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Select Vegetation Item", LabelStyle);
            EditorGUI.BeginChangeCheck();

            VegetationPackageEditorTools.DrawVegetationItemSelector(
                _sceneVegetationBaker.VegetationSystem.currentVegetationPackage,
                VegetationPackageEditorTools.CreateVegetationInfoIdList(
                    _sceneVegetationBaker.VegetationSystem.currentVegetationPackage,
                    new[] { VegetationType.Objects, VegetationType.Tree, VegetationType.LargeObjects,VegetationType.Grass,VegetationType.Plant }), 60,
                ref _sceneVegetationBaker.SelectedVegetationID);

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            _sceneVegetationBaker.ExportStatic = EditorGUILayout.Toggle("Export as static objects", _sceneVegetationBaker.ExportStatic);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            if (GUILayout.Button("Bake VegetationItem to scene"))
            {
                BakeVegetationToScene(_sceneVegetationBaker.SelectedVegetationID);
            }

            GUILayout.EndVertical();
        }

        void BakeVegetationToScene(string vegetationItemID)
        {
            GameObject root = new GameObject
            {
                name = "BakedVegetationItem_" + vegetationItemID,
                isStatic = true
            };

            root.transform.position = _sceneVegetationBaker.VegetationSystem.UnityTerrainData.terrainPosition;

            for (int i = 0; i <= _sceneVegetationBaker.VegetationSystem.VegetationCellList.Count - 1; i++)
            {
                if (i % 100 == 0)
                {
                    float progress = (float)i / _sceneVegetationBaker.VegetationSystem.VegetationCellList.Count;
                    EditorUtility.DisplayProgressBar("Bake to scene", "Spawn all vegetation items", progress);
                }
                AddVegetationItemsToScene(_sceneVegetationBaker.VegetationSystem.VegetationCellList[i], root, vegetationItemID);
            }

            EditorUtility.ClearProgressBar();
        }

        void AddVegetationItemsToScene(VegetationCell vegetationCell, GameObject parent, string vegetationItemID)
        {
            VegetationItemInfo vegetationItemInfo = _sceneVegetationBaker.VegetationSystem.currentVegetationPackage
                .GetVegetationInfo(vegetationItemID);

            if (vegetationItemInfo.VegetationPrefab == null) return;

            List<Matrix4x4> vegetationItemInstanceList = vegetationCell.DirectSpawnVegetation(vegetationItemID, true);
            for (int i = 0; i <= vegetationItemInstanceList.Count - 1; i++)
            {
                Vector3 position = MatrixTools.ExtractTranslationFromMatrix(vegetationItemInstanceList[i]);
                Vector3 scale = MatrixTools.ExtractScaleFromMatrix(vegetationItemInstanceList[i]);
                Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(vegetationItemInstanceList[i]);

                GameObject vegetationItem = Instantiate(vegetationItemInfo.VegetationPrefab, parent.transform);
                vegetationItem.transform.position = position;
                vegetationItem.transform.localScale = scale;
                vegetationItem.transform.rotation = rotation;
                vegetationItem.isStatic = _sceneVegetationBaker.ExportStatic;
            }
        }
    }
}
