using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.TerrainBaker.Editor
{

    [CustomEditor(typeof(TerrainBaker)), CanEditMultipleObjects]
    public class TerrainBakerInspector : UnityEditor.Editor
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

            //For fast start no custom editor here. TODO it later
            DrawDefaultInspector();

            TerrainBaker terrainBaker = target as TerrainBaker;


            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            if(!string.IsNullOrEmpty(terrainBaker.errorString))
            {
                EditorGUILayout.HelpBox(terrainBaker.errorString, MessageType.Error);
            }
            else
            {
                /*
                if(terrainBaker.numAdaptiveTriangles > 0)
                {
                    string message = string.Format("Triangles {0}\nVertices {1}",
                        terrainBaker.numAdaptiveTriangles,
                        terrainBaker.numAdaptiveVertices);
                    EditorGUILayout.HelpBox(message, MessageType.Info);
                }*/
            }
            GUILayout.EndVertical();

        }

    }

}
