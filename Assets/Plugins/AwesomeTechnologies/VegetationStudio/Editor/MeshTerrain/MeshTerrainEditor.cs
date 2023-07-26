using System.Collections;
using System.Collections.Generic;
using AwesomeTechnologies.Utility;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.MeshTerrains
{
    [CustomEditor(typeof(MeshTerrain))]
    public class MeshTerrainEditor : VegetationStudioBaseEditor
    {
        private MeshTerrain _meshTerrain;

        private static readonly string[] TabNames =
        {
            "Settings", "Mesh terrain sources", "Debug"
        };


        public override void OnInspectorGUI()
        {
            OverrideLogoTextureName = "SectionBanner_MeshTerrain";
            LargeLogo = false;
            //HelpTopic = "home/vegetation-studio/components/vegetation-system";
            _meshTerrain = (MeshTerrain) target;

            base.OnInspectorGUI();

            _meshTerrain.CurrentTabIndex = GUILayout.SelectionGrid(_meshTerrain.CurrentTabIndex, TabNames, 3, EditorStyles.toolbarButton);
            switch (_meshTerrain.CurrentTabIndex)
            {
                case 0:
                    DrawSettingsInspector();
                    break;
                case 1:
                    DrawSourceInspector();
                    break;
                case 2:
                    DrawDebugInspector();
                    break;                                  
            }
        }

        void DrawSettingsInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Settings", LabelStyle);

            EditorGUI.BeginChangeCheck();
            _meshTerrain.MeshTerrainData = EditorGUILayout.ObjectField("Mesh terrain data", _meshTerrain.MeshTerrainData, typeof(MeshTerrainData), true) as MeshTerrainData;
            if (EditorGUI.EndChangeCheck())
            {
               
            }
           
            EditorGUILayout.HelpBox(
                "Create and drop a MeshTerrainData object here. This will store the generated mesh terrain data.",
                MessageType.Info);

            GUILayout.EndVertical();


            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Info", LabelStyle);         

            GUILayout.EndVertical();
        }

        void DrawSourceInspector()
        {

            //EditorGUILayout.Space();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Add terrain sources", LabelStyle);

            bool addMesh = false;
            GUILayout.BeginHorizontal();
            DropZoneTools.DrawMeshTerrainDropZone(DropZoneType.MeshRenderer,_meshTerrain, ref addMesh);
            DropZoneTools.DrawMeshTerrainDropZone(DropZoneType.Terrain, _meshTerrain, ref addMesh);           

            GUILayout.EndHorizontal();
            EditorGUILayout.HelpBox(
                "Drop a mesh renderers or Unity terrains to add them to the terrain source data.",
                MessageType.Info);

            GUILayout.EndVertical();

            if (addMesh)
            {
                EditorUtility.SetDirty(target);
                return;
            }

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Included sources", LabelStyle);
            EditorGUILayout.Space();

            int removeIndex = -1;

            if (_meshTerrain.MeshTerrainMeshSourceList.Count > 0)
            {
                EditorGUILayout.LabelField("Meshes", LabelStyle);

                for (int i = 0; i <= _meshTerrain.MeshTerrainMeshSourceList.Count - 1; i++)
                {
                    GUILayout.BeginHorizontal();

                    MeshTerrainMeshSource meshTerrainMeshSource =_meshTerrain.MeshTerrainMeshSourceList[i];

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField("Mesh: ", LabelStyle, GUILayout.Width(50));
                    meshTerrainMeshSource.MeshRenderer = EditorGUILayout.ObjectField("", meshTerrainMeshSource.MeshRenderer, typeof(MeshRenderer), true, GUILayout.Width(150)) as MeshRenderer;
                    meshTerrainMeshSource.MeshTerrainSourceType = (MeshTerrainSourceType) EditorGUILayout.EnumPopup("", meshTerrainMeshSource.MeshTerrainSourceType, GUILayout.Width(150));
                    if (GUILayout.Button("Remove"))
                    {
                        removeIndex = i;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        _meshTerrain.MeshTerrainMeshSourceList[i] = meshTerrainMeshSource;
                        EditorUtility.SetDirty(target);
                    }

                    GUILayout.EndHorizontal();
                }
            }

            if (removeIndex > -1)
            {
                _meshTerrain.MeshTerrainMeshSourceList.RemoveAt(removeIndex);
                _meshTerrain.NeedGeneration = true;
                EditorUtility.SetDirty(target);

                //HandleUtility.Repaint();
            }

            if (_meshTerrain.MeshTerrainTerrainSourceList.Count > 0)
            {
                EditorGUILayout.LabelField("Terrains", LabelStyle);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Generate terrain data", LabelStyle);

            if (_meshTerrain.NeedGeneration)
            {
                EditorGUILayout.HelpBox(
                    "The terrain sources have changed. Mesh terrain data needs to be regenerated.",
                    MessageType.Warning);
            }


            if (GUILayout.Button("Generate terrain data"))
            {
                _meshTerrain.NeedGeneration = false;
            }
            EditorGUILayout.HelpBox(
                "The generated data will be stored in the assigned MeshTerrainData object.",
                MessageType.Info);
            GUILayout.EndVertical();

        }

        void DrawDebugInspector()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Debug", LabelStyle);
            
            EditorGUI.BeginChangeCheck();
            _meshTerrain.ShowDebugInfo = EditorGUILayout.Toggle("Show debug info", _meshTerrain.ShowDebugInfo);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
           
            GUILayout.EndVertical();
        }
    }
}
