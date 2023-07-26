using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Assets.WorldFiller.Editor
{

    //[CustomEditor(typeof(LayerFillingData))]
    public class LayerFillingInspector : UnityEditor.Editor
    {

        private Vector2 scrollPos;

        private void OnEnable()
        {


        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            //LayerFillingData data = target as LayerFillingData;
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            FillingGUI();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private void FillingGUI()
        {

        }



    }
}
