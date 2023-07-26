using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.TerrainBaker.Editor
{

    [CustomEditor(typeof(TerrainAtlas)), CanEditMultipleObjects]
    public class TerrainAtlasInspector : UnityEditor.Editor
    {

        private void OnEnable()
        {


        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            TerrainAtlas atlas = target as TerrainAtlas;

            GUILayout.BeginVertical();
            EditorGUILayout.Space();
            string info = null;
            if (atlas.layers != null && atlas.layers.Length > 0)
            {
                if (atlas.albedo != null && atlas.normals != null && atlas.parameters != null && atlas.layers.Length > 0)
                {
                    info = "";
                    info += "Albedo texture atlas: " + atlas.albedo.name;
                    info += "\nNormals texture atlas: " + atlas.normals.name;
                    info += "\nParameters texture atlas: " + atlas.parameters.name;
                    info += (atlas.macro != null) ? "\nParameters texture atlas: " + atlas.parameters.name : "";
                    info += "\nLayers in atlas: " + atlas.layers.Length;
                    for(int i = 0; i < atlas.layers.Length; i++)
                    {
                        info += "\n    [" + i + "] " + atlas.layers[i].albedoName;
                    }
                }
                else info = "Need rebuild atlas";

            }
            else info = "Now atlas is empty and need building.\nFind owner TerrainAtlasBuilder and press button \"Build\"";
            GUILayout.Label("Select owner TerrainAtlasBuilder for edit this terrain atlas");
            EditorGUILayout.HelpBox(info, MessageType.Info);
            GUILayout.EndVertical();

        }

    }
}
