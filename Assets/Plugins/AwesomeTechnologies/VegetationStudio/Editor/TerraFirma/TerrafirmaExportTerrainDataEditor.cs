using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.External.Terrafirma
{

    [CustomEditor(typeof(AwesomeTechnologies.External.Terrafirma.TerrafirmaExportTerrainData))]
    public class ExportTerrainDataEditor : VegetationStudioBaseEditor
    {
        private TerrafirmaExportTerrainData _exportTerrainData;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _exportTerrainData = (AwesomeTechnologies.External.Terrafirma.TerrafirmaExportTerrainData)target;
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Export terrain data to Terrafirma.", LabelStyle);
            EditorGUI.BeginChangeCheck();
            _exportTerrainData.terrain = EditorGUILayout.ObjectField("Selected terrain", _exportTerrainData.terrain, typeof(Terrain), true) as Terrain;
            if (EditorGUI.EndChangeCheck())
            {
                _exportTerrainData.RefreshImage();
            }

            if (GUILayout.Button("Export data"))
            {
                _exportTerrainData.Export();
            }

            EditorGUILayout.HelpBox("A png file with the terrain data will be saved in the root Asset folder. Same name as terrain gameobject.", MessageType.Info);

            _exportTerrainData.outputTexture = (Texture2D)EditorGUILayout.ObjectField(_exportTerrainData.outputTexture, typeof(Texture2D), false, GUILayout.Height(256), GUILayout.Width(256));

            EditorGUILayout.EndVertical();
        }
    }
}
