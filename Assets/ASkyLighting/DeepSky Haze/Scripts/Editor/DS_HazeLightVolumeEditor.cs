using UnityEngine;
using UnityEditor;

namespace DeepSky.Haze
{
    [CustomEditor(typeof(DS_HazeLightVolume))]
    public class DS_HazeLightVolumeEditor : Editor
    {
        #region HELP_STRINGS
        private static string m_HelpTxtPerf =
            "These settings affect the performance of the light volume and can be used to find the right balance between " +
            "rendering speed and quality.\n Choose the number of <b>Samples</b> used to render this light volume (more samples " +
            "improves quality but is <b>slower</b> to render).\nThe light volume will fade out between <b>Start Fade</b> and " +
            "<b>End Fade</b> distances. Beyond the end value it will be culled.\nFor spotlights, control the end range of the " +
            "light volume using <b>Far Clip</b>, up to the range value of the light component.";
        private static string m_HelpTxtScatter =
            "<b>Scattering</b> controls how much light is scattered out by dust/water vapour in this volume and is a multiplier " +
            "of the light's intensity.\n<b>Secondary Scattering</b> controls how dark shadows in the volume appear, approximating light " +
            "bouncing around multiple times. The ratio between forward scattering (in the same direction as the light) and back scattering " +
            "(reflected back towards the light) is controlled with <b>Scattering Direction</b>. This has the most affect on spotlights, but " +
            "also creates a bright halo around point lights.";
        private static string m_HelpTxtDensity =
            "Assign a <b>Density Texture</b> to break up the light volume and create a smoky or foggy atmosphere. The size of the texture " +
            "is controlled with the <b>Density Texture Scale</b> slider and the amount of influence with the <b>Density Texture Contrast</b> " +
            "control.\nThe texture can be animated to add some movement to the atmosphere by specifying a world-space vector in <b>Animate " +
            "Direction</b> and a speed for movement with <b>Animate Speed</b>.";
        #endregion

        private bool m_HelpTxtPerfExpanded = false;
        private bool m_HelpTxtScatterExpanded = false;
        private bool m_HelpTxtDensityExpanded = false;

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

            serializedObject.Update();
            DS_HazeLightVolume lightVolume = target as DS_HazeLightVolume;
            SerializedProperty samples = serializedObject.FindProperty("m_Samples");
            SerializedProperty falloff = serializedObject.FindProperty("m_Falloff");
            SerializedProperty scatter = serializedObject.FindProperty("m_Scattering");
            SerializedProperty secondScatter = serializedObject.FindProperty("m_SecondaryScattering");
            SerializedProperty scatterDir = serializedObject.FindProperty("m_ScatteringDirection");
            SerializedProperty densityTex = serializedObject.FindProperty("m_DensityTexture");
            SerializedProperty densityTexScale = serializedObject.FindProperty("m_DensityTextureScale");
            SerializedProperty densityTexContrast = serializedObject.FindProperty("m_DensityTextureContrast");
            SerializedProperty animateDir = serializedObject.FindProperty("m_AnimateDirection");
            SerializedProperty animateSp = serializedObject.FindProperty("m_AnimateSpeed");
            SerializedProperty startFade = serializedObject.FindProperty("m_StartFade");
            SerializedProperty endFade = serializedObject.FindProperty("m_EndFade");
            SerializedProperty farClip = serializedObject.FindProperty("m_FarClip");

            if (lightVolume.LightTypeChanged())
            {
                lightVolume.UpdateLightType();
            }
            else if (lightVolume.ProxyMeshRequiresRebuild())
            {
                // Changing the light type always updates the proxy, so only need to trigger a specific update if the light shape changed.
                lightVolume.RebuildProxyMesh();
            }

            if (lightVolume.ShadowModeChanged())
            {
                lightVolume.UpdateShadowMode();
            }

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Falloff Type:", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(falloff);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Performance:", EditorStyles.boldLabel);
                m_HelpTxtPerfExpanded = EditorGUILayout.Toggle(m_HelpTxtPerfExpanded, helpIconStyle, GUILayout.Width(helpIconImage.width));
                EditorGUILayout.EndHorizontal();
                if (m_HelpTxtPerfExpanded) EditorGUILayout.TextArea(m_HelpTxtPerf, helpBoxStyle);

                EditorGUILayout.PropertyField(samples);
                EditorGUILayout.PropertyField(startFade);
                EditorGUILayout.PropertyField(endFade);

                GUI.enabled = lightVolume.Type == LightType.Spot;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(farClip);
                if (EditorGUI.EndChangeCheck())
                {
                    lightVolume.RebuildProxyMesh();
                }
                GUI.enabled = true;
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Scattering:", EditorStyles.boldLabel);
                m_HelpTxtScatterExpanded = EditorGUILayout.Toggle(m_HelpTxtScatterExpanded, helpIconStyle, GUILayout.Width(helpIconImage.width));
                EditorGUILayout.EndHorizontal();
                if (m_HelpTxtScatterExpanded) EditorGUILayout.TextArea(m_HelpTxtScatter, helpBoxStyle);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UseFog"));
                EditorGUILayout.PropertyField(scatter);
                EditorGUILayout.PropertyField(secondScatter);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(scatterDir);
                GUI.enabled = true;
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Density:", EditorStyles.boldLabel);
                m_HelpTxtDensityExpanded = EditorGUILayout.Toggle(m_HelpTxtDensityExpanded, helpIconStyle, GUILayout.Width(helpIconImage.width));
                EditorGUILayout.EndHorizontal();
                if (m_HelpTxtDensityExpanded) EditorGUILayout.TextArea(m_HelpTxtDensity, helpBoxStyle);
                EditorGUILayout.PropertyField(densityTex);

                if (densityTex.objectReferenceValue == null)
                {
                    GUI.enabled = false;
                }
                EditorGUILayout.PropertyField(densityTexScale);
                EditorGUILayout.PropertyField(densityTexContrast);
                EditorGUILayout.PropertyField(animateDir);
                EditorGUILayout.PropertyField(animateSp);
                GUI.enabled = true;
            }
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
