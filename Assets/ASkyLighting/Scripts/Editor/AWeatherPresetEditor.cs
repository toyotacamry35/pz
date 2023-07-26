using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(AWeatherPreset))]
public class AWeatherPresetEditor : Editor
{

    GUIStyle boxStyle;
    GUIStyle boxStyle2;
    GUIStyle wrapStyle;
    GUIStyle clearStyle;

    AWeatherPreset myTarget;

    public bool showAudio = false;
    public bool showFog = false;
    public bool showSeason = false;
    public bool showClouds = false;
    public bool showGeneral = false;

    SerializedObject serializedObj;

    void OnEnable()
    {
        myTarget = (AWeatherPreset)target;

        serializedObj = new SerializedObject(myTarget);

    }

    public override void OnInspectorGUI()
    {

        myTarget = (AWeatherPreset)target;
#if UNITY_5_6_OR_NEWER
        serializedObj.UpdateIfRequiredOrScript();
#else
		serializedObj.UpdateIfDirtyOrScript ();
#endif
        //Set up the box style
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;
        }

        if (boxStyle2 == null)
        {
            boxStyle2 = new GUIStyle(GUI.skin.label);
            boxStyle2.normal.textColor = GUI.skin.label.normal.textColor;
            boxStyle2.fontStyle = FontStyle.Bold;
            boxStyle2.alignment = TextAnchor.UpperLeft;
        }

        //Setup the wrap style
        if (wrapStyle == null)
        {
            wrapStyle = new GUIStyle(GUI.skin.label);
            wrapStyle.fontStyle = FontStyle.Bold;
            wrapStyle.wordWrap = true;
        }

        if (clearStyle == null)
        {
            clearStyle = new GUIStyle(GUI.skin.label);
            clearStyle.normal.textColor = GUI.skin.label.normal.textColor;
            clearStyle.fontStyle = FontStyle.Bold;
            clearStyle.alignment = TextAnchor.UpperRight;
        }



        // Begin
        GUILayout.BeginVertical("", boxStyle);
        GUILayout.Space(10);
        myTarget.Name = EditorGUILayout.TextField("Name", myTarget.Name);
        GUILayout.Space(10);

        // General Setup
        GUILayout.BeginVertical("", boxStyle);
        showGeneral = EditorGUILayout.BeginToggleGroup("General Configs", showGeneral);
        if (showGeneral)
        {
            myTarget.WindStrenght = EditorGUILayout.Slider("Wind Intensity", myTarget.WindStrenght, 0f, 1f);

        }
        EditorGUILayout.EndToggleGroup();
        GUILayout.EndVertical();


        // Season Setup




        // Clouds Setup
        GUILayout.BeginVertical("", boxStyle);
        showClouds = EditorGUILayout.BeginToggleGroup("Clouds Configs", showClouds);
        if (showClouds)
        {
            if (GUILayout.Button("Add"))
            {
                myTarget.cloudConfig.Add(new AWeatherCloudsConfig());
            }
            for (int i = 0; i < myTarget.cloudConfig.Count; i++)
            {
                GUILayout.BeginVertical("Layer " + i, boxStyle);
                GUILayout.Space(15);
                myTarget.cloudConfig[i].BaseColor = EditorGUILayout.ColorField("Base Color", myTarget.cloudConfig[i].BaseColor);
                myTarget.cloudConfig[i].DirectLightInfluence = EditorGUILayout.Slider("Light Influence", myTarget.cloudConfig[i].DirectLightInfluence, 0f, 100f);
                myTarget.cloudConfig[i].Density = EditorGUILayout.Slider("Density", myTarget.cloudConfig[i].Density, -0.25f, 0.25f);
                myTarget.cloudConfig[i].Coverage = EditorGUILayout.Slider("Coverage", myTarget.cloudConfig[i].Coverage, -1f, 2f);
                myTarget.cloudConfig[i].Alpha = EditorGUILayout.Slider("Alpha", myTarget.cloudConfig[i].Alpha, 0f, 10f);
                if (GUILayout.Button("Remove"))
                {
                    myTarget.cloudConfig.Remove(myTarget.cloudConfig[i]);
                }
                GUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndToggleGroup();
        GUILayout.EndVertical();

        // Fog Setup



        // Audio Setup



        // END
        EditorGUILayout.EndVertical();
        EditorUtility.SetDirty(target);
    }
}
