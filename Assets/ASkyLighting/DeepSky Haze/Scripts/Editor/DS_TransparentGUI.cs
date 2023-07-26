using UnityEngine;
using UnityEditor;

using System;

namespace DeepSky.Haze
{
    public class DS_TransparentGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material mat = materialEditor.target as Material;

            bool usePerFragment = Array.IndexOf(mat.shaderKeywords, "DS_HAZE_APPLY_PER_FRAGMENT") != -1;
            MaterialProperty colour = FindProperty("_Color", properties, false);
            MaterialProperty albedo = FindProperty("_MainTex", properties, false);
            MaterialProperty intensity = FindProperty("_Intensity", properties, false);

            // These properties will not be present if there is an _Intensity property (eg. on the skybox shaders).
            MaterialProperty metalGlossTex = FindProperty("_MetallicGloss", properties, false); //<--- will not exist on Specular model shaders.
            MaterialProperty specGlossTex = FindProperty("_SpecularGloss", properties, false); //<--- will not exist on Standard model shaders.
            MaterialProperty metallic = FindProperty("_Metallic", properties, false);
            MaterialProperty specular = FindProperty("_Specular", properties, false);
            MaterialProperty gloss = FindProperty("_Gloss", properties, false);
            MaterialProperty normalTex = FindProperty("_Normal", properties, false);

            EditorGUI.BeginChangeCheck();
            usePerFragment = EditorGUILayout.Toggle("Compute per-fragment", usePerFragment);
            materialEditor.TexturePropertySingleLine(new GUIContent("Albedo"), albedo, colour);

            if (intensity != null)
            {
                materialEditor.RangeProperty(intensity, "Intensity");
            }
            else
            {
                // There will either be a Metal/Gloss texture OR a Specular/Gloss texture, never both.
                if (metalGlossTex != null)
                { 
                    if (metalGlossTex.textureValue != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Metallic (R) Smoothness (A)"), metalGlossTex);
                    else
                        materialEditor.TexturePropertyTwoLines(new GUIContent("Metallic"), metalGlossTex, metallic, new GUIContent("Smoothness"), gloss);
                }
                if (specGlossTex != null)
                {
                    if (specGlossTex.textureValue != null)
                        materialEditor.TexturePropertySingleLine(new GUIContent("Specular (R) Smoothness (A)"), specGlossTex);
                    else
                        materialEditor.TexturePropertyTwoLines(new GUIContent("Specular"), specGlossTex, specular, new GUIContent("Smoothness"), gloss);
                }
                materialEditor.TexturePropertySingleLine(new GUIContent("Normal"), normalTex);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (usePerFragment)
                    mat.EnableKeyword("DS_HAZE_APPLY_PER_FRAGMENT");
                else
                    mat.DisableKeyword("DS_HAZE_APPLY_PER_FRAGMENT");

                if (intensity == null)
                {
                    if (metalGlossTex != null)
                    {
                        if (metalGlossTex.textureValue != null)
                            mat.EnableKeyword("_DS_HAZE_METAL_GLOSS_TEX");
                        else
                            mat.DisableKeyword("_DS_HAZE_METAL_GLOSS_TEX");
                    }
                    if (specGlossTex != null)
                    {
                        if (specGlossTex.textureValue != null)
                            mat.EnableKeyword("_DS_HAZE_METAL_GLOSS_TEX");
                        else
                            mat.DisableKeyword("_DS_HAZE_METAL_GLOSS_TEX");
                    }

                    if (normalTex.textureValue != null)
                        mat.EnableKeyword("_DS_HAZE_NORMAL_TEX");
                    else
                        mat.DisableKeyword("_DS_HAZE_NORMAL_TEX");
                }

                materialEditor.PropertiesChanged();
            }
        }
    }
}