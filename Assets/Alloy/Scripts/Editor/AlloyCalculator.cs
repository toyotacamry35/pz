using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Alloy;
using Object = UnityEngine.Object;

class AlloyCalculator : EditorWindow
{

    [MenuItem(AlloyUtils.MenuItem + "Alloy Calculator", false, 15)]
    static void Init()
    {
        GetWindow(typeof(AlloyCalculator), false, "Alloy calculator");
    }


    float linear = 0;
    float sRGB = 0;
    float to255 = 0;
    int to01 = 0;
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        GUILayout.Label("");
        GUILayout.Label(" Linear value: ");
        GUILayout.Label("   sRGB value: ");
        GUILayout.Label("[0..1] to 255: ");
        GUILayout.Label("255 to [0..1]: ");
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        GUILayout.Label("Input");
        linear = EditorGUILayout.FloatField(linear);
        sRGB = EditorGUILayout.FloatField(sRGB);
        to255 = EditorGUILayout.FloatField(to255);
        to01 = EditorGUILayout.IntField(to01);
        EditorGUILayout.EndVertical();        

        EditorGUILayout.BeginVertical();
        GUILayout.Label("[0..1]");
        GUILayout.Label(" " + LinearToSRGB(linear));
        GUILayout.Label(" " + SRGBtoLinear(sRGB));
        GUILayout.Label(" ");
        GUILayout.Label(" " + Mathf.Clamp(to01, 0, 255) / 255.0f);

        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical();
        GUILayout.Label("[0..255]");
        GUILayout.Label(" " + (int)(LinearToSRGB(linear) * 255.0f));
        GUILayout.Label(" " + (int)(SRGBtoLinear(sRGB) * 255.0f));
        GUILayout.Label(" " + (int)(Mathf.Clamp01(to255) * 255.0f));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);
        EditorGUILayout.EndVertical();
    }

    float LinearToSRGB(float value)
    {
        if (value > 1.0f)
        {
            value /= 255.0f;
        }
        value = Mathf.Clamp01(value);

        if (value <= 0.0031308f)
        {
            return 12.92f * value;
        }
        else
        {
            return 1.055f * Mathf.Pow(value, 0.4166667f) - 0.055f;
        }
    }



    float SRGBtoLinear(float value)
    {
        if (value > 1.0f)
        {
            value /= 255.0f;
        }
        value = Mathf.Clamp01(value);

        if (value <= 0.04045F)
            return value / 12.92F;
        else if (value < 1.0F)
            return Mathf.Pow((value + 0.055F) / 1.055F, 2.4F);
        else
            return Mathf.Pow(value, 2.2F);
    }


}

