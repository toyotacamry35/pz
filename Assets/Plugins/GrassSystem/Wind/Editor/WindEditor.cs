using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AdvancedTerrainGrass
{

    [CustomEditor(typeof(Wind))]
    public class WindEditor : Editor
    {
        private bool isOpenSettings = false;

        public override void OnInspectorGUI()
        {
            Wind current = (Wind)target;
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Wind Settings", EditorStyles.boldLabel);
            

            GUILayout.BeginHorizontal();

            for (int i=0; i<current.windVariants.Count; i++)
            {
                GUIStyle style = (i == 0) ? EditorStyles.miniButtonLeft : EditorStyles.miniButtonMid;
                style = (i == current.windVariants.Count-1) ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid;
                GUI.color = (current.currentWind == i) ? Color.gray : Color.white; 
                if (GUILayout.Button(current.windVariants[i].name, style))
                {
                    current.currentWind = i;
                    current.SelectWindVariant();
                }
                
            }
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
            if (GUILayout.Button((!isOpenSettings) ? "▼  Settings" : "▲  Settings", GUILayout.Width(200)))
            {
                isOpenSettings = !isOpenSettings;
            }

            if (isOpenSettings)
            {

                EditorGUI.BeginChangeCheck();

                GUI.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                EditorGUILayout.BeginVertical("box");
                GUI.color = Color.white;
                current.windVariants[current.currentWind].name = EditorGUILayout.TextField("Name", current.windVariants[current.currentWind].name);
                current.windVariants[current.currentWind].windFrequency = EditorGUILayout.Slider("Wind Frequency", current.windVariants[current.currentWind].windFrequency, 0.01f, 2f);
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Foliage", EditorStyles.boldLabel);
                current.windVariants[current.currentWind].foliagePrimaryStrenght = EditorGUILayout.Slider("Primary Strenght", current.windVariants[current.currentWind].foliagePrimaryStrenght, 0.1f, 10f);
                current.windVariants[current.currentWind].foliageSecondaryStrenght = EditorGUILayout.Slider("Secondary Strenght", current.windVariants[current.currentWind].foliageSecondaryStrenght, 0.1f, 10f);
                current.windVariants[current.currentWind].foliageLeafTurbulence = EditorGUILayout.Slider("Leaf Turbulence", current.windVariants[current.currentWind].foliageLeafTurbulence, 0.0f, 10f);
                current.windVariants[current.currentWind].foliageWaveSize = EditorGUILayout.Slider("Wave Size", current.windVariants[current.currentWind].foliageWaveSize, 0.1f, 25f);
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Grass", EditorStyles.boldLabel);

                current.windVariants[current.currentWind].grassStrenght = EditorGUILayout.Slider("Strenght", current.windVariants[current.currentWind].grassStrenght, 0.1f, 5f);
                current.windVariants[current.currentWind].grassSize = EditorGUILayout.Slider("Size", current.windVariants[current.currentWind].grassSize, 0.001f, 0.1f);
                current.windVariants[current.currentWind].grassSpeed = EditorGUILayout.Slider("Speed", current.windVariants[current.currentWind].grassSpeed, 0.001f, 0.2f);
                EditorGUILayout.EndVertical();

                if (EditorGUI.EndChangeCheck())
                    current.SelectWindVariant();
            }
            EditorGUILayout.BeginVertical("box");
            base.OnInspectorGUI();
            EditorGUILayout.EndVertical();
            /*
            GUILayout.BeginHorizontal();
            current.currentWind = EditorGUILayout.IntSlider("", current.currentWind, 0, current.windVariants.Count);
            GUILayout.EndHorizontal();
            */



        }
    }
}
