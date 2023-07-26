using UnityEngine;
using UnityEditor;

namespace DeepSky.Haze
{
    [CustomEditor(typeof(DS_HazeView))]
    public class DS_HazeViewEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            // Styling and icons.
            GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox);
            helpBoxStyle.richText = true;
            Texture2D helpIconImage = EditorGUIUtility.FindTexture("console.infoicon.sml");
            GUIStyle helpIconStyle = new GUIStyle();
            helpIconStyle.normal.background = helpIconImage;
            helpIconStyle.onNormal.background = helpIconImage;
            helpIconStyle.active.background = helpIconImage;
            helpIconStyle.onActive.background = helpIconImage;
            helpIconStyle.focused.background = helpIconImage;
            helpIconStyle.onFocused.background = helpIconImage;

            // Get the serialized properties upfront.
            serializedObject.Update();
            DS_HazeView view = target as DS_HazeView;
            SerializedProperty renderAtmos = serializedObject.FindProperty("m_RenderAtmosphereVolumetrics");
            SerializedProperty renderLocal = serializedObject.FindProperty("m_RenderLocalVolumetrics");
            SerializedProperty useTemporal = serializedObject.FindProperty("m_TemporalReprojection");
            SerializedProperty sizeFactor = serializedObject.FindProperty("m_DownsampleFactor");
            SerializedProperty samples = serializedObject.FindProperty("m_VolumeSamples");
            SerializedProperty lightProp = serializedObject.FindProperty("m_DirectLight");
            SerializedProperty ctxOverProp = serializedObject.FindProperty("m_OverrideContextAsset");
            SerializedProperty ctxTimeProp = serializedObject.FindProperty("m_OverrideTime");
            SerializedProperty ctxVariantOverProp = serializedObject.FindProperty("m_OverrideContextVariant");
            SerializedProperty ctxGaussDepthProp = serializedObject.FindProperty("m_GaussianDepthFalloff");
            SerializedProperty ctxUpDepthProp = serializedObject.FindProperty("m_UpsampleDepthThreshold");
            SerializedProperty ctxTemporalProp = serializedObject.FindProperty("m_TemporalRejectionScale");
            SerializedProperty ctxTemporalBlendProp = serializedObject.FindProperty("m_TemporalBlendFactor");
            SerializedProperty applyAirProp = serializedObject.FindProperty("m_ApplyAirToSkybox");
            SerializedProperty applyHazeProp = serializedObject.FindProperty("m_ApplyHazeToSkybox");
            SerializedProperty applyFogEProp = serializedObject.FindProperty("m_ApplyFogExtinctionToSkybox");
            SerializedProperty applyFogRProp = serializedObject.FindProperty("m_ApplyFogLightingToSkybox");
            SerializedProperty ctxShowTemporalProp = serializedObject.FindProperty("m_ShowTemporalRejection");
            SerializedProperty ctxShowUpsampleThresholdProp = serializedObject.FindProperty("m_ShowUpsampleThreshold");

            bool updateTemporalKeywords = false;
            bool updateSkyboxKeywords = false;
            bool updateDebugKeywords = false;

            // Main GUI layout and drawing.
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General:", EditorStyles.boldLabel);

                GUI.enabled = renderAtmos.boolValue | renderLocal.boolValue;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(useTemporal);
                GUI.enabled = true;
                EditorGUILayout.PropertyField(renderAtmos);
                EditorGUILayout.PropertyField(renderLocal);
                if (EditorGUI.EndChangeCheck())
                {
                    updateTemporalKeywords = true;
                }

                EditorGUILayout.PropertyField(sizeFactor);
                EditorGUILayout.PropertyField(samples);

                EditorGUI.BeginChangeCheck();
                Object tmp = lightProp.objectReferenceValue;
                
                EditorGUILayout.PropertyField(lightProp);
                if (EditorGUI.EndChangeCheck())
                {
                    // Remove the command buffer if no longer referencing the directional light.
                    if (tmp != null && tmp != lightProp.objectReferenceValue)
                    {
                        view.RemoveCommandBufferFromLight((Light)tmp);
                    }
                }

                               
                EditorGUILayout.BeginHorizontal();
                {
                    if (!ctxOverProp.boolValue) GUI.enabled = false;
                    EditorGUI.BeginChangeCheck();
                    ctxTimeProp.boolValue = EditorGUILayout.Toggle(ctxTimeProp.boolValue, EditorStyles.radioButton, GUILayout.Width(16));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (ctxTimeProp.boolValue)
                        {
                            ctxVariantOverProp.boolValue = false;
                        }
                    }
                    if (!ctxTimeProp.boolValue) GUI.enabled = false;
                    EditorGUILayout.LabelField("Time:", GUILayout.Width(100));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Time"), GUIContent.none);
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Shader Parameters:", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();
               
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Apply To Skybox:");
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(applyAirProp, new GUIContent("Air"));
                EditorGUILayout.PropertyField(applyHazeProp, new GUIContent("Haze"));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(applyFogEProp, new GUIContent("Fog"));
                EditorGUILayout.PropertyField(applyFogRProp, new GUIContent("Fog Lighting"));
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    updateSkyboxKeywords = true;
                }
                EditorGUILayout.PropertyField(ctxGaussDepthProp);
                EditorGUILayout.PropertyField(ctxUpDepthProp);
                if (useTemporal.boolValue == false)
                {
                    GUI.enabled = false;
                }
                EditorGUILayout.PropertyField(ctxTemporalProp);
                EditorGUILayout.PropertyField(ctxTemporalBlendProp);
                GUI.enabled = true;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debug Options:", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    if (useTemporal.boolValue == false) GUI.enabled = false;
                    ctxShowTemporalProp.boolValue = EditorGUILayout.Toggle(ctxShowTemporalProp.boolValue, EditorStyles.radioButton, GUILayout.Width(16));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (ctxShowTemporalProp.boolValue)
                        {
                            ctxShowUpsampleThresholdProp.boolValue = false;
                        }
                        updateDebugKeywords = true;
                    }
                    if (!ctxShowTemporalProp.boolValue) GUI.enabled = false;
                    EditorGUILayout.LabelField("Show Temporal Rejection");
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    ctxShowUpsampleThresholdProp.boolValue = EditorGUILayout.Toggle(ctxShowUpsampleThresholdProp.boolValue, EditorStyles.radioButton, GUILayout.Width(16));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (ctxShowUpsampleThresholdProp.boolValue)
                        {
                            ctxShowTemporalProp.boolValue = false;
                        }
                        updateDebugKeywords = true;
                    }
                    if (!ctxShowUpsampleThresholdProp.boolValue) GUI.enabled = false;
                    EditorGUILayout.LabelField("Show Upsample Threshold");
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (updateTemporalKeywords) view.SetTemporalKeywords();
            if (updateSkyboxKeywords) view.SetSkyboxKeywords();
            if (updateDebugKeywords) view.SetDebugKeywords();
        }

    }
}
