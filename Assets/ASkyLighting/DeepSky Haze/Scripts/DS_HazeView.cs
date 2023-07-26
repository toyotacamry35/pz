using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using TOD;
using Assets.Src.Lib.ProfileTools;
using System.Reflection;
using System;

namespace DeepSky.Haze
{
    [System.Serializable]
    public class HazeItemEclipse
    {
        public float airScattering; //0, 8.0f)
        public float airHeightFalloff; //(0.0001f, 0.1f)]

        public float hazeScattering; //(0.0001f, 0.1f)]
        public float hazeHeightFalloff; //(0.0001f, 0.1f)]
        public float hazeScatteringDir; //-0.99f, 0.99f)]
        public float hazeScatteringRatio; //(0, 1)]

        public float fogOpacity02; //(0, 1)]
        public float fogScattering;// (0, 8.0f)]
        public float fogExtinction; //Range(0, 8.0f

        public float fogHeightFalloff; //Range(0.0001f, 1.0f)]
        public float fogDistance; //0, 1)]
        public float fogHeight;//-200,200
        public float fogScatteringDir;//-90,90


        public Color fogAmbient;
        public Color fogLight;
        public Color fogHorizon;

        [ColorUsage(true, true)]
        public Color fogColor;
        public float fogDensity; //Range(0.01f, 0.0001f)]
        public float fogAlpha; //0, 1f

        public bool isCave = false;
        public float caveAmbient;

        public void CopyFrom(HazeItemEclipse other)
        {
            Type thisType = GetType();
            Type otherType = other.GetType();

            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }
        }
    }

    [ExecuteInEditMode, AddComponentMenu("DeepSky Haze/DS_HazeView", 1)]
    public class DS_HazeView : MonoBehaviour
    {
        private enum SizeFactor { Half = 2, Quarter = 4 };
        private enum VolumeSamples { x24, x32, x64 };
        public float t = -1;
        private DS_HazeContextItem ctxToRenderSecond = null;

        private static LightEvent ShadowCascadesCmdBufferEvent = LightEvent.AfterShadowMap;
        private static LightEvent DirectionalLightCmdBufferEvent = LightEvent.AfterScreenspaceMask;
        private static CameraEvent RenderNonShadowVolumesEvent = CameraEvent.BeforeImageEffectsOpaque;
        
        private static string kPreviousDepthTargetName = "DS_Haze_PreviousDepthTarget";
        private static string kRadianceTarget01Name = "DS_Haze_RadianceTarget_01";
        private static string kRadianceTarget02Name = "DS_Haze_RadianceTarget_02";
        private static Shader kShader;

        [SerializeField]
        private bool m_OverrideTime = false;
        [SerializeField, Range(0, 1)]
        private float m_Time = 0.5f;
        [SerializeField]
        private bool m_OverrideContextAsset = false;
        [SerializeField]
        private DS_HazeContextAsset m_Context;
        [SerializeField]
        private bool m_OverrideContextVariant = false;
        [SerializeField]
        private int m_ContextItemIndex = 0;
        [SerializeField]
        private Light m_DirectLight;
        [SerializeField]
        private bool m_RenderAtmosphereVolumetrics = true;
        [SerializeField]
        private bool m_RenderLocalVolumetrics = true;
        [SerializeField]
        private bool m_TemporalReprojection = true;
        [SerializeField]
        private SizeFactor m_DownsampleFactor = SizeFactor.Half;
        [SerializeField]
        private VolumeSamples m_VolumeSamples = VolumeSamples.x24;

        // Shader params.
        [SerializeField, Range(100, 5000)]
        private int m_GaussianDepthFalloff = 500;
        [SerializeField, Range(0, 0.5f)]
        private float m_UpsampleDepthThreshold = 0.06f;
        [SerializeField, Range(0.001f, 1.0f)]
        private float m_TemporalRejectionScale = 0.1f;
        [SerializeField, Range(0.1f, 0.9f)]
        private float m_TemporalBlendFactor = 0.25f;

        // Shader keywords.
        private ShadowProjection m_ShadowProjectionType = ShadowProjection.StableFit;
        [SerializeField]
        private bool m_ApplyAirToSkybox = false;
        [SerializeField]
        private bool m_ApplyHazeToSkybox = true;
        [SerializeField]
        private bool m_ApplyFogExtinctionToSkybox = true;
        [SerializeField]
        private bool m_ApplyFogLightingToSkybox = true;
        [SerializeField]
        private bool m_ShowTemporalRejection = false;
        [SerializeField]
        private bool m_ShowUpsampleThreshold = false;

        // Non-serialized fields.
        private Camera m_Camera;
        private RenderTexture m_PerFrameRadianceTarget;
        private RenderTexture m_RadianceTarget_01;
        private RenderTexture m_RadianceTarget_02;
        private RenderTexture m_CurrentRadianceTarget;
        private RenderTexture m_PreviousRadianceTarget;
        private RenderTexture m_PreviousDepthTarget;
        private (Light Light, CommandBuffer Buffer) m_ShadowCascadesCmdBuffer;
        private (Light Light, CommandBuffer Buffer) m_DirectionalLightCmdBuffer;
        private (Camera Camera, CommandBuffer Buffer, CameraEvent Event) m_ClearRadianceCmdBuffer;
        private (Camera Camera, CommandBuffer Buffer) m_RenderNonShadowVolumes;
        private Material m_Material;
        private Matrix4x4 m_PreviousViewProjMatrix = Matrix4x4.identity;
        private Matrix4x4 m_PreviousInvViewProjMatrix = Matrix4x4.identity;
        private float m_InterleavedOffsetIndex = 0f;
        private int m_X;
        private int m_Y;
        private RenderingPath m_PreviousRenderPath;
        private ColorSpace m_ColourSpace;


        private List<DS_HazeLightVolume> m_PerFrameLightVolumes = new List<DS_HazeLightVolume>();
        private List<DS_HazeLightVolume> m_PerFrameShadowLightVolumes = new List<DS_HazeLightVolume>();
        private Dictionary<Light, CommandBuffer> m_LightVolumeCmdBuffers = new Dictionary<Light, CommandBuffer>();

        // Public Accessors.
        public bool OverrideTime
        {
            get { return m_OverrideTime; }
            set
            {
                m_OverrideTime = value;
                if (value && m_OverrideContextVariant)
                {
                    // Time and Variant overrides can't both be true.
                    m_OverrideContextVariant = false;
                }
            }
        }

        public float Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        public bool OverrideContextAsset
        {
            get { return m_OverrideContextAsset; }
            set { m_OverrideContextAsset = value; }
        }

        public DS_HazeContextAsset ContextAsset
        {
            get { return m_Context; }
            set { m_Context = value; }
        }

        public bool OverrideContextVariant
        {
            get { return m_OverrideContextVariant; }
            set
            {
                m_OverrideContextVariant = value;
                if (value && m_OverrideTime)
                {
                    // Time and Variant overrides can't both be true.
                    m_OverrideTime = false;
                }
            }
        }

        public int ContextItemIndex
        {
            get { return m_ContextItemIndex; }
            set { m_ContextItemIndex = value > 0 ? value : 0; }
        }

        public Light DirectLight
        {
            get { return m_DirectLight; }
            set { m_DirectLight = value; }
        }

        public Vector2 RadianceTargetSize
        {
            get { return new Vector2(m_X, m_Y); }
        }

        public int SampleCount
        {
            get
            {
                switch (m_VolumeSamples)
                {
                    case VolumeSamples.x24:
                        return 24;
                    case VolumeSamples.x32:
                        return 32;
                    case VolumeSamples.x64:
                        return 64;
                    default:
                        return 24;
                }
            }
        }

        public int DownSampleFactor
        {
            get
            {
                return m_DownsampleFactor == SizeFactor.Half ? (int)SizeFactor.Half : (int)SizeFactor.Quarter;
            }
        }

        public bool RenderAtmosphereVolumetrics
        {
            get { return m_RenderAtmosphereVolumetrics; }
            set
            {
                m_RenderAtmosphereVolumetrics = value;
                SetTemporalKeywords();
            }
        }

        public bool RenderLocalVolumetrics
        {
            get { return m_RenderLocalVolumetrics; }
            set
            {
                m_RenderLocalVolumetrics = value;
                SetTemporalKeywords();
            }
        }

        public bool TemporalReprojection
        {
            get { return m_TemporalReprojection; }
            set
            {
                m_TemporalReprojection = value;
                SetTemporalKeywords();
            }
        }

        public bool WillRenderWithTemporalReprojection
        {
            get { return m_TemporalReprojection & (m_RenderAtmosphereVolumetrics | m_RenderLocalVolumetrics); }
        }

        public int AntiAliasingLevel()
        {
            int aa = 1;
#if UNITY_5_6_OR_NEWER // MSAA and float targets allowed, Camera.allowMSAA exists.
            if (m_Camera.actualRenderingPath == RenderingPath.Forward && m_Camera.allowMSAA && QualitySettings.antiAliasing > 0)
#elif UNITY_5_5 // MSAA and float targets allowed.
            if (m_Camera.actualRenderingPath == RenderingPath.Forward && QualitySettings.antiAliasing > 0)
#else // MSAA and float targets NOT allowed.
            if (m_Camera.actualRenderingPath != RenderingPath.Forward && QualitySettings.antiAliasing > 0)
#endif
            {
                aa = QualitySettings.antiAliasing;
            }
            return aa;
        }

        /// <summary>
        /// Does this system support the shader model and render texture formats required?
        /// </summary>
        /// <returns></returns>
        private bool hasSystemSupportShecked = false;
        private bool hasSystemSupport = false;
        private bool GetHasSystemSupport()
        {
            if (SystemInfo.graphicsShaderLevel < 30)
            {
                Debug.LogError("DeepSky::DS_HazeView: Minimum required shader model (3.0) is not supported on this platform.");
                return false;
            }

            if (m_Camera.allowHDR)
            {
                if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                {
                    Debug.LogError("DeepSky::DS_HazeView: ARGBHalf render texture format is not supported on this platform.");
                    return false;
                }
            }

            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat))
            {
                Debug.LogError("DeepSky::DS_HazeView: RFloat render texture format is not supported on this platform.");
                return false;
            }

            return true;
        }

        private bool CheckHasSystemSupport()
        {
            if (!hasSystemSupportShecked)
            {
                hasSystemSupport = GetHasSystemSupport();
                enabled = hasSystemSupport;
                hasSystemSupportShecked = true;
            }
            return hasSystemSupport;
        }

        /// <summary>
        /// Set all material parameters from a context item, ready for rendering.
        /// The Rayleigh and Mie scattering coefficients are derived from 'Precomputed 
        /// Atmospheric Scattering (Bruneton, Neyret 2008)' - https://hal.inria.fr/inria-00288758/file/article.pdf
        /// The original Rayleigh values have been scaled up to make it easier to work with
        /// the typically shorter view distances in a game environment.
        /// </summary>
        /// <param name="ctx"> The context item to get settings from. </param>
        public void SetMaterialFromContext(DS_HazeContextItem ctx)
        {
            HazeItemEclipse nightItem = DS_HazeCore.Instance.mainPreset.Context.m_NightItem;

            if (WillRenderWithTemporalReprojection)
            {
                m_InterleavedOffsetIndex += 0.0625f;
                if (Mathf.Approximately(m_InterleavedOffsetIndex, 1))
                {
                    m_InterleavedOffsetIndex = 0;
                }
            }

            
            
            float atmosViewpointDensity = 1.0f;
            float hazeViewpointDensity = 1.0f;
            float fogViewpointDensity = 1.0f;

            float absY = Mathf.Abs(transform.position.y);

            HazeItemEclipse eclipseItem = ctx.MergeWithEclipse(nightItem);

            if (ASkyLighting._instance.context.isEclipse)
            {
                atmosViewpointDensity = Mathf.Exp(-eclipseItem.airHeightFalloff * absY);
                hazeViewpointDensity = Mathf.Exp(-eclipseItem.hazeHeightFalloff * absY);
                fogViewpointDensity = Mathf.Exp(-eclipseItem.fogHeightFalloff * (absY - eclipseItem.fogHeight)); 
            }
            else
            {
                atmosViewpointDensity = Mathf.Exp(-ctx.airHeightFalloff.value * absY);
                hazeViewpointDensity = Mathf.Exp(-ctx.hazeHeightFalloff.value * absY);
                fogViewpointDensity = Mathf.Exp(-ctx.fogHeightFalloff.value * (absY - ctx.fogHeight.value));
            }
              

            Vector3 kRBetaS;
            Vector4 airHazeParams;
            Vector4 betaParams;
            Vector4 initialDensityParams;
            Vector4 fogParams;
            Vector4 samplingParams;
            float kMBetaS;
            float kFBetaS;
            float kFBetaE;

            if (ASkyLighting._instance.context.isEclipse)
            {
               
                kRBetaS = eclipseItem.airScattering * new Vector3(0.00116f, 0.0027f, 0.00662f); //<--- (.0000058, .0000135, .0000331) * 200
                kMBetaS = eclipseItem.hazeScattering * 0.0021f;
                kFBetaS = eclipseItem.fogScattering;
                kFBetaE = eclipseItem.fogExtinction * 0.01f;

                airHazeParams = new Vector4(eclipseItem.airHeightFalloff, eclipseItem.hazeHeightFalloff, 0, Mathf.Lerp(0, eclipseItem.hazeScatteringDir, DS_HazeCore.Instance.hazeDirectIntensity));
                betaParams = new Vector4(kMBetaS, m_RenderAtmosphereVolumetrics ? Mathf.Lerp(0, eclipseItem.hazeScatteringRatio, DS_HazeCore.Instance.hazeDirectIntensity) : 0, kFBetaS, kFBetaE);
                initialDensityParams = new Vector4(atmosViewpointDensity, hazeViewpointDensity, fogViewpointDensity, 0);
                fogParams = new Vector4(eclipseItem.fogDistance, eclipseItem.fogHeightFalloff, eclipseItem.fogOpacity02, Mathf.Lerp(0, eclipseItem.fogScatteringDir, DS_HazeCore.Instance.hazeDirectIntensity));
                samplingParams = new Vector4(m_GaussianDepthFalloff, m_UpsampleDepthThreshold * 0.01f, m_TemporalRejectionScale, m_TemporalBlendFactor);

                RenderSettings.fogColor = new Color(eclipseItem.fogColor.r, eclipseItem.fogColor.g, eclipseItem.fogColor.b, eclipseItem.fogAlpha);
                RenderSettings.fogDensity = eclipseItem.fogDensity;
                Shader.SetGlobalFloat("fogColor_alpha", eclipseItem.fogAlpha);
            }
            else
            {
                kRBetaS = ctx.airScattering.value * new Vector3(0.00116f, 0.0027f, 0.00662f); //<--- (.0000058, .0000135, .0000331) * 200
                kMBetaS = ctx.hazeScattering.value * 0.0021f;
                kFBetaS = ctx.fogScattering.value;
                kFBetaE = ctx.fogExtinction.value * 0.01f;

                airHazeParams = new Vector4(ctx.airHeightFalloff.value, ctx.hazeHeightFalloff.value, 0, Mathf.Lerp(0, ctx.hazeScatteringDir.value, DS_HazeCore.Instance.hazeDirectIntensity));
                betaParams = new Vector4(kMBetaS, m_RenderAtmosphereVolumetrics ? Mathf.Lerp(0, ctx.hazeScatteringRatio.value, DS_HazeCore.Instance.hazeDirectIntensity) : 0, kFBetaS, kFBetaE);
                initialDensityParams = new Vector4(atmosViewpointDensity, hazeViewpointDensity, fogViewpointDensity, 0);
                fogParams = new Vector4(ctx.fogDistance.value, ctx.fogHeightFalloff.value, ctx.fogOpacity02.value, Mathf.Lerp(0, ctx.fogScatteringDir.value, DS_HazeCore.Instance.hazeDirectIntensity));
                samplingParams = new Vector4(m_GaussianDepthFalloff, m_UpsampleDepthThreshold * 0.01f, m_TemporalRejectionScale, m_TemporalBlendFactor);

                RenderSettings.fogColor = new Color(ctx.fogColor.color.r, ctx.fogColor.color.g, ctx.fogColor.color.b, ctx.fogAlpha.value);
                RenderSettings.fogDensity = ctx.fogDensity.value;
                Shader.SetGlobalFloat("fogColor_alpha", ctx.fogAlpha.value);
            }


            m_Material.SetVector("_SamplingParams", samplingParams);
            m_Material.SetVector("_InterleavedOffset", new Vector4(m_InterleavedOffsetIndex, 0, 0, 0));
            m_Material.SetMatrix("_PreviousViewProjMatrix", m_PreviousViewProjMatrix);
            m_Material.SetMatrix("_PreviousInvViewProjMatrix", m_PreviousInvViewProjMatrix);

            Shader.SetGlobalVector("_DS_BetaParams", betaParams);
            Shader.SetGlobalVector("_DS_RBetaS", kRBetaS);
            Shader.SetGlobalVector("_DS_AirHazeParams", airHazeParams);
            Shader.SetGlobalVector("_DS_FogParams", fogParams);
            Shader.SetGlobalVector("_DS_InitialDensityParams", initialDensityParams);

            float currentIntensity = 1;
            Vector3 direction = Vector3.up;
            Color lightColour = Color.magenta;

            currentIntensity = DS_HazeCore.Instance.hazeIntensity;
            direction = -m_DirectLight.transform.forward;
            lightColour = DS_HazeCore.Instance.hazeColor.linear * currentIntensity;


            if (ASkyLighting._instance.context.isEclipse)
            {
                Shader.SetGlobalColor("_DS_FogAmbientLight", eclipseItem.fogAmbient.linear * currentIntensity);
                Shader.SetGlobalColor("_DS_FogDirectLight", eclipseItem.fogLight.linear * currentIntensity);
                Shader.SetGlobalVector("_DS_HorizonColour", eclipseItem.fogHorizon.linear);
            }
            else
            {
                Shader.SetGlobalColor("_DS_FogAmbientLight", ctx.fogAmbient.color.linear * currentIntensity);
                Shader.SetGlobalColor("_DS_FogDirectLight", ctx.fogLight.color.linear * currentIntensity);
                Shader.SetGlobalVector("_DS_HorizonColour", ctx.fogHorizon.color.linear);
            }

            Shader.SetGlobalVector("_DS_LightDirection", direction);
            Shader.SetGlobalVector("_DS_LightColour", lightColour);
        }

        /*
        private void OnGUI()
        {
            if (ASkyLighting._instance.isDebug)
            {
                DS_HazeContextItem ctxToRender;
                ctxToRender = DS_HazeCore.Instance.m_Zones[0].Context.m_ComplexItem;

                GUI.Label(new Rect(10, 250, 250, 20), "Air Scattering " + ctxToRender.airScattering.value.ToString());
                GUI.Label(new Rect(10, 270, 250, 20), "Air Height Falloff " + ctxToRender.airHeightFalloff.value.ToString());
                GUI.color = ctxToRender.fogHorizon.color;
                GUI.Button(new Rect(10, 290, 250, 20), "Air Horizon Color " + GUI.color.ToString(), UnityEditor.EditorStyles.helpBox);
                GUI.color = GUI.contentColor;

                GUI.Label(new Rect(10, 320, 250, 20), "Haze Scattering " + ctxToRender.hazeScattering.value.ToString());
                GUI.Label(new Rect(10, 340, 250, 20), "Haze Height Falloff " + ctxToRender.hazeHeightFalloff.value.ToString());
                GUI.Label(new Rect(10, 360, 250, 20), "Haze Direction " + ctxToRender.hazeScatteringDir.value.ToString());
                GUI.Label(new Rect(10, 380, 250, 20), "Haze Ratio " + ctxToRender.hazeScatteringRatio.value.ToString());

                GUI.Label(new Rect(10, 410, 250, 20), "Mist Opacity " + ctxToRender.fogOpacity02.value.ToString());
                GUI.Label(new Rect(10, 430, 250, 20), "Mist Height Falloff " + ctxToRender.fogScattering.value.ToString());
                GUI.Label(new Rect(10, 450, 250, 20), "Mist Extinction " + ctxToRender.fogExtinction.value.ToString());

                GUI.Label(new Rect(10, 475, 250, 20), "Mist Height Falloff " + ctxToRender.fogHeightFalloff.value.ToString());
                GUI.Label(new Rect(10, 495, 250, 20), "Mist Distance " + ctxToRender.fogDistance.value.ToString());
                GUI.Label(new Rect(10, 515, 250, 20), "Mist Height " + ctxToRender.fogHeight.value.ToString());
                GUI.Label(new Rect(10, 535, 250, 20), "Mist Direction " + ctxToRender.fogScatteringDir.value.ToString());

                GUI.color = ctxToRender.fogAmbient.color;
                GUI.Button(new Rect(10, 555, 250, 20), "Mist Ambient Color " + GUI.color.ToString(), UnityEditor.EditorStyles.helpBox);
                GUI.color = GUI.contentColor;

                GUI.color = ctxToRender.fogLight.color;
                GUI.Button(new Rect(10, 575, 250, 20), "Mist Light Color " + GUI.color.ToString(), UnityEditor.EditorStyles.helpBox);
                GUI.color = GUI.contentColor;

            }
        }
        */

        private void SetGlobalParamsToNull()
        {
            Shader.SetGlobalVector("_DS_BetaParams", Vector4.zero);
            Shader.SetGlobalVector("_DS_RBetaS", Vector4.zero);
        }

        /// <summary>
        /// Enable the keywords for debugging the upsample and reprojection thresholds.
        /// </summary>
        public void SetDebugKeywords()
        {
            if (m_ShowTemporalRejection)
            {
                m_Material.EnableKeyword("SHOW_TEMPORAL_REJECTION");
            }
            else
            {
                m_Material.DisableKeyword("SHOW_TEMPORAL_REJECTION");
            }

            if (m_ShowUpsampleThreshold)
            {
                m_Material.EnableKeyword("SHOW_UPSAMPLE_THRESHOLD");
            }
            else
            {
                m_Material.DisableKeyword("SHOW_UPSAMPLE_THRESHOLD");
            }
        }

        public void SetSkyboxKeywords()
        {
            if (m_ApplyAirToSkybox)
            {
                m_Material.EnableKeyword("DS_HAZE_APPLY_RAYLEIGH");
            }
            else
            {
                m_Material.DisableKeyword("DS_HAZE_APPLY_RAYLEIGH");
            }

            if (m_ApplyHazeToSkybox)
            {
                m_Material.EnableKeyword("DS_HAZE_APPLY_MIE");
            }
            else
            {
                m_Material.DisableKeyword("DS_HAZE_APPLY_MIE");
            }

            if (m_ApplyFogExtinctionToSkybox)
            {
                m_Material.EnableKeyword("DS_HAZE_APPLY_FOG_EXTINCTION");
            }
            else
            {
                m_Material.DisableKeyword("DS_HAZE_APPLY_FOG_EXTINCTION");
            }

            if (m_ApplyFogLightingToSkybox)
            {
                m_Material.EnableKeyword("DS_HAZE_APPLY_FOG_RADIANCE");
            }
            else
            {
                m_Material.DisableKeyword("DS_HAZE_APPLY_FOG_RADIANCE");
            }
        }

        public void SetTemporalKeywords()
        {
            // Always disable temporal reprojection if no volumetrics are actually being rendered.
            if (WillRenderWithTemporalReprojection)
            {
                // UpdateResources will take care of creating the render targets, just need to enable the shader keyword.
                m_Material.EnableKeyword("DS_HAZE_TEMPORAL");
            }
            else
            {
                // If disabling temporal reprojection, need to clean up the unused render targets as well.
                m_Material.DisableKeyword("DS_HAZE_TEMPORAL");

                if (m_ShowTemporalRejection)
                {
                    m_ShowTemporalRejection = false;
                    m_Material.DisableKeyword("SHOW_TEMPORAL_REJECTION");
                }

                if (m_RadianceTarget_01)
                {
                    m_RadianceTarget_01.Release();
                    DestroyImmediate(m_RadianceTarget_01);
                    m_RadianceTarget_01 = null;
                }
                if (m_RadianceTarget_02)
                {
                    m_RadianceTarget_02.Release();
                    DestroyImmediate(m_RadianceTarget_02);
                    m_RadianceTarget_02 = null;
                }
                if (m_PreviousDepthTarget)
                {
                    m_PreviousDepthTarget.Release();
                    DestroyImmediate(m_PreviousDepthTarget);
                    m_PreviousDepthTarget = null;
                }
            }
        }

        /// <summary>
        /// Set the various keywords on the material to control shader variant being used.
        /// </summary>
        private void SetShaderKeyWords()
        {
            if (m_ShadowProjectionType == ShadowProjection.CloseFit)
            {
                m_Material.EnableKeyword("SHADOW_PROJ_CLOSE");
            }
            else if (m_ShadowProjectionType == ShadowProjection.StableFit)
            {
                m_Material.DisableKeyword("SHADOW_PROJ_CLOSE");
            }

            if (DS_HazeCore.Instance != null)
            {
                m_Material.DisableKeyword("DS_HAZE_HEIGHT_FALLOFF_NONE");
            }
        }

        void OnEnable()
        {
            SetGlobalParamsToNull();

            m_Camera = GetComponent<Camera>();

            if (!m_Camera)
            {
                Debug.LogError("DeepSky::DS_HazeView: GameObject '" + gameObject.name + "' does not have a camera component!");
                enabled = false;
                return;
            }

            if (!CheckHasSystemSupport())
            {
                enabled = false;
                return;
            }

            if (kShader == null)
            {
                kShader = Profile.Load<Shader>("DS_Haze");
            }

            if (m_Material == null)
            {
                m_Material = new Material(kShader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }

            if (m_Camera.actualRenderingPath == RenderingPath.Forward && (m_Camera.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.Depth)
            {
                m_Camera.depthTextureMode = m_Camera.depthTextureMode | DepthTextureMode.Depth;
            }

            if (m_RenderNonShadowVolumes.Camera != m_Camera || (m_RenderNonShadowVolumes.Buffer == null && m_Camera))
            {
                if (m_RenderNonShadowVolumes.Camera && m_RenderNonShadowVolumes.Buffer != null)
                    m_RenderNonShadowVolumes.Camera.RemoveCommandBuffer(RenderNonShadowVolumesEvent,  m_RenderNonShadowVolumes.Buffer);

                if (m_Camera)
                {
                    if (m_RenderNonShadowVolumes.Buffer == null)
                    {
                        m_RenderNonShadowVolumes.Buffer = new CommandBuffer();
                        m_RenderNonShadowVolumes.Buffer.name = "DS_Haze_RenderLightVolume";
                    }
                    m_Camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, m_RenderNonShadowVolumes.Buffer);
                }

                m_RenderNonShadowVolumes.Camera = m_Camera;
            }
            
            m_CurrentRadianceTarget = m_RadianceTarget_01;
            m_PreviousRadianceTarget = m_RadianceTarget_02;

            SetSkyboxKeywords();
            SetDebugKeywords();

            m_ColourSpace = QualitySettings.activeColorSpace;
            m_PreviousRenderPath = m_Camera.actualRenderingPath;
        }

        private void CreateRadianceTarget(string name, out RenderTexture radianceTarget)
        {
#if UNITY_5_6_OR_NEWER
            if (m_Camera.allowHDR)
#else
            if (m_Camera.hdr)
#endif
            {
                radianceTarget = new RenderTexture(m_Camera.pixelWidth, m_Camera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);
            }
            else
            {
                radianceTarget = new RenderTexture(m_Camera.pixelWidth, m_Camera.pixelHeight, 0, RenderTextureFormat.ARGB32);
            }

            radianceTarget.name = name;
#if UNITY_5_5_OR_NEWER
            radianceTarget.antiAliasing = AntiAliasingLevel();
#else
            radianceTarget.antiAliasing = 1;
#endif
            radianceTarget.useMipMap = false;
            radianceTarget.hideFlags = HideFlags.HideAndDontSave;
            radianceTarget.filterMode = FilterMode.Point;
        }


        private void CreateDepthTarget(string name, out RenderTexture depthTarget, bool downsample = false)
        {
            depthTarget = new RenderTexture(downsample ? m_X : m_Camera.pixelWidth, downsample ? m_Y : m_Camera.pixelHeight, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            depthTarget.name = name;
            depthTarget.antiAliasing = 1;
            depthTarget.useMipMap = false;
            depthTarget.hideFlags = HideFlags.HideAndDontSave;
            depthTarget.filterMode = FilterMode.Point;
        }

        public void RemoveCommandBufferFromLight(Light light)
        {
            if (m_ShadowCascadesCmdBuffer.Light == light)
            {
                if (m_ShadowCascadesCmdBuffer.Buffer != null)
                    m_ShadowCascadesCmdBuffer.Light.RemoveCommandBuffer(ShadowCascadesCmdBufferEvent, m_ShadowCascadesCmdBuffer.Buffer);
                m_ShadowCascadesCmdBuffer.Light = null;
            }
            
            if (m_DirectionalLightCmdBuffer.Light == light)
            {
                if (m_DirectionalLightCmdBuffer.Buffer != null)
                    m_DirectionalLightCmdBuffer.Light.RemoveCommandBuffer(DirectionalLightCmdBufferEvent, m_DirectionalLightCmdBuffer.Buffer);
                m_DirectionalLightCmdBuffer.Light = null;
            }
        }

        private void RenderPathChanged()
        {
            if (m_Camera.actualRenderingPath == RenderingPath.Forward && (m_Camera.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.Depth)
            {
                m_Camera.depthTextureMode = m_Camera.depthTextureMode | DepthTextureMode.Depth;
            }

            if (m_ClearRadianceCmdBuffer.Buffer != null && m_ClearRadianceCmdBuffer.Camera)
            {
                m_ClearRadianceCmdBuffer.Camera.RemoveCommandBuffer(m_ClearRadianceCmdBuffer.Event, m_ClearRadianceCmdBuffer.Buffer);
                m_ClearRadianceCmdBuffer.Camera = null;
            }
            m_PreviousRenderPath = m_Camera.actualRenderingPath;
        }

        void UpdateResources()
        {
            if (m_DirectLight == null)
            {
                if (ASkyLighting._instance != null)
                {
                    m_DirectLight = ASkyLighting._instance.GetDirectionalLight();
                }
            }

            m_X = m_Camera.pixelWidth / (int)m_DownsampleFactor;
            m_Y = m_Camera.pixelHeight / (int)m_DownsampleFactor;

            // If the rendering path has changed, need to clean up the existing command buffers first.
            if (m_Camera.actualRenderingPath != m_PreviousRenderPath)
            {
                RenderPathChanged();
            }

            // Expected radiance target format and colorspace.
#if UNITY_5_6_OR_NEWER
            RenderTextureFormat rtFormat = m_Camera.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
#else
            RenderTextureFormat rtFormat = m_Camera.hdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
#endif
            bool colorSpaceChanged = m_ColourSpace != QualitySettings.activeColorSpace;
            m_ColourSpace = QualitySettings.activeColorSpace;

            if (WillRenderWithTemporalReprojection)
            {
                if (m_RadianceTarget_01 == null)
                {
                    CreateRadianceTarget(kRadianceTarget01Name, out m_RadianceTarget_01);
                    m_CurrentRadianceTarget = m_RadianceTarget_01;
                }
                else
                {
                    if (colorSpaceChanged || m_RadianceTarget_01.width != m_Camera.pixelWidth || m_RadianceTarget_01.height != m_Camera.pixelHeight || m_RadianceTarget_01.format != rtFormat)
                    {
                        DestroyImmediate(m_RadianceTarget_01);
                        CreateRadianceTarget(kRadianceTarget01Name, out m_RadianceTarget_01);
                        m_CurrentRadianceTarget = m_RadianceTarget_01;
                    }
                }

                if (m_RadianceTarget_02 == null)
                {
                    CreateRadianceTarget(kRadianceTarget02Name, out m_RadianceTarget_02);
                    m_PreviousRadianceTarget = m_RadianceTarget_02;
                }
                else
                {
                    if (colorSpaceChanged || m_RadianceTarget_02.width != m_Camera.pixelWidth || m_RadianceTarget_02.height != m_Camera.pixelHeight || m_RadianceTarget_02.format != rtFormat)
                    {
                        DestroyImmediate(m_RadianceTarget_02);
                        CreateRadianceTarget(kRadianceTarget02Name, out m_RadianceTarget_02);
                        m_PreviousRadianceTarget = m_RadianceTarget_02;
                    }
                }

                if (m_PreviousDepthTarget == null)
                {
                    CreateDepthTarget(kPreviousDepthTargetName, out m_PreviousDepthTarget);
                }
                else
                {
                    if (m_PreviousDepthTarget.width != m_Camera.pixelWidth || m_PreviousDepthTarget.height != m_Camera.pixelHeight)
                    {
                        DestroyImmediate(m_PreviousDepthTarget);
                        CreateDepthTarget(kPreviousDepthTargetName, out m_PreviousDepthTarget);
                    }
                }
            }

            if (m_ClearRadianceCmdBuffer.Buffer == null)
            {
                m_ClearRadianceCmdBuffer.Buffer = new CommandBuffer();
                m_ClearRadianceCmdBuffer.Buffer.name = "DS_Haze_ClearRadiance";
            }

            CameraEvent cv;
            if (m_Camera.actualRenderingPath == RenderingPath.DeferredShading)
            {
                cv = CameraEvent.BeforeGBuffer;
            }
            else
            {
                cv = (m_Camera.depthTextureMode & DepthTextureMode.DepthNormals) == DepthTextureMode.DepthNormals ? CameraEvent.BeforeDepthNormalsTexture : CameraEvent.BeforeDepthTexture;
            }

            if (m_ClearRadianceCmdBuffer.Camera != m_Camera || m_ClearRadianceCmdBuffer.Event != cv)
            {
                if (m_ClearRadianceCmdBuffer.Camera)
                    m_ClearRadianceCmdBuffer.Camera.RemoveCommandBuffer(m_ClearRadianceCmdBuffer.Event, m_ClearRadianceCmdBuffer.Buffer);

                m_Camera.AddCommandBuffer(cv, m_ClearRadianceCmdBuffer.Buffer);

                m_ClearRadianceCmdBuffer.Camera = m_Camera;
                m_ClearRadianceCmdBuffer.Event = cv;
            }

            if (m_ShadowCascadesCmdBuffer.Light != m_DirectLight || (m_ShadowCascadesCmdBuffer.Buffer == null && m_DirectLight))
            {
                if (m_ShadowCascadesCmdBuffer.Light && m_ShadowCascadesCmdBuffer.Buffer != null)
                    m_ShadowCascadesCmdBuffer.Light.RemoveCommandBuffer(ShadowCascadesCmdBufferEvent, m_ShadowCascadesCmdBuffer.Buffer);

                if (m_DirectLight)
                {
                    if (m_ShadowCascadesCmdBuffer.Buffer == null)
                    {
                        m_ShadowCascadesCmdBuffer.Buffer = new CommandBuffer();
                        m_ShadowCascadesCmdBuffer.Buffer.name = "DS_Haze_ShadowCascadesCopy";
                        m_ShadowCascadesCmdBuffer.Buffer.SetGlobalTexture("_ShadowCascades", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
                    }
                    m_DirectLight.AddCommandBuffer(ShadowCascadesCmdBufferEvent, m_ShadowCascadesCmdBuffer.Buffer);
                }
                
                m_ShadowCascadesCmdBuffer.Light = m_DirectLight;
            }

            if (m_DirectionalLightCmdBuffer.Light != m_DirectLight || (m_DirectionalLightCmdBuffer.Buffer == null && m_DirectLight))
            {
                if (m_DirectionalLightCmdBuffer.Light && m_DirectionalLightCmdBuffer.Buffer != null)
                    m_DirectionalLightCmdBuffer.Light.RemoveCommandBuffer(DirectionalLightCmdBufferEvent, m_DirectionalLightCmdBuffer.Buffer);

                if (m_DirectLight)
                {
                    if (m_DirectionalLightCmdBuffer.Buffer == null)
                    {
                        m_DirectionalLightCmdBuffer.Buffer = new CommandBuffer();
                        m_DirectionalLightCmdBuffer.Buffer.name = "DS_Haze_DirectLight";
                    }
                    m_DirectLight.AddCommandBuffer(DirectionalLightCmdBufferEvent, m_DirectionalLightCmdBuffer.Buffer);
                }

                m_DirectionalLightCmdBuffer.Light = m_DirectLight;
            }
            
            
            if (m_DirectLight)
            {
                m_ShadowProjectionType = QualitySettings.shadowProjection;
            }
        }

        void OnDisable()
        {
            SetGlobalParamsToNull();

            if (m_ClearRadianceCmdBuffer.Buffer != null && m_ClearRadianceCmdBuffer.Camera)
            {
                m_ClearRadianceCmdBuffer.Camera.RemoveCommandBuffer(m_ClearRadianceCmdBuffer.Event, m_ClearRadianceCmdBuffer.Buffer);
                m_ClearRadianceCmdBuffer.Camera = null;
            }

            if (m_ShadowCascadesCmdBuffer.Light)
            {
                if (m_ShadowCascadesCmdBuffer.Buffer != null)
                    m_ShadowCascadesCmdBuffer.Light.RemoveCommandBuffer(ShadowCascadesCmdBufferEvent, m_ShadowCascadesCmdBuffer.Buffer);
                m_ShadowCascadesCmdBuffer.Light = null;
            }

            if (m_DirectionalLightCmdBuffer.Light)
            {
                if (m_DirectionalLightCmdBuffer.Buffer != null)
                    m_DirectionalLightCmdBuffer.Light.RemoveCommandBuffer(DirectionalLightCmdBufferEvent, m_DirectionalLightCmdBuffer.Buffer);
                m_DirectionalLightCmdBuffer.Light = null;
            }
            
            if (m_LightVolumeCmdBuffers.Count > 0)
            {
                foreach (KeyValuePair<Light, CommandBuffer> entry in m_LightVolumeCmdBuffers)
                {
                    entry.Key.RemoveCommandBuffer(LightEvent.AfterShadowMap, entry.Value);
                    entry.Value.Dispose();
                }
                m_LightVolumeCmdBuffers.Clear();
            }

            m_RenderNonShadowVolumes.Buffer?.Clear();
        }

        /// <summary>
        /// Free up render targets, command buffers and any per-frame data.
        /// </summary>
        void OnDestroy()
        {
            if (m_RadianceTarget_01)
            {
                m_RadianceTarget_01.Release();
                DestroyImmediate(m_RadianceTarget_01);
                m_RadianceTarget_01 = null;
            }
            if (m_RadianceTarget_02)
            {
                m_RadianceTarget_02.Release();
                DestroyImmediate(m_RadianceTarget_02);
                m_RadianceTarget_02 = null;
            }
            if (m_PreviousDepthTarget)
            {
                m_PreviousDepthTarget.Release();
                DestroyImmediate(m_PreviousDepthTarget);
                m_PreviousDepthTarget = null;
            }

            if (m_ClearRadianceCmdBuffer.Buffer != null)
            {
                if (m_ClearRadianceCmdBuffer.Camera)
                    m_ClearRadianceCmdBuffer.Camera.RemoveCommandBuffer(m_ClearRadianceCmdBuffer.Event, m_ClearRadianceCmdBuffer.Buffer);
                m_ClearRadianceCmdBuffer.Buffer.Dispose();
                m_ClearRadianceCmdBuffer = default;
            }

            if (m_ShadowCascadesCmdBuffer.Buffer != null)
            {
                if (m_ShadowCascadesCmdBuffer.Light)
                    m_ShadowCascadesCmdBuffer.Light.RemoveCommandBuffer(ShadowCascadesCmdBufferEvent, m_ShadowCascadesCmdBuffer.Buffer);
                m_ShadowCascadesCmdBuffer.Buffer.Dispose();
                m_ShadowCascadesCmdBuffer = default;
            }

            if (m_DirectionalLightCmdBuffer.Buffer != null)
            {
                if (m_DirectionalLightCmdBuffer.Light)
                    m_DirectionalLightCmdBuffer.Light.RemoveCommandBuffer(DirectionalLightCmdBufferEvent, m_DirectionalLightCmdBuffer.Buffer);
                m_DirectionalLightCmdBuffer.Buffer.Dispose();
                m_DirectionalLightCmdBuffer = default;
            }

            if (m_LightVolumeCmdBuffers.Count > 0)
            {
                foreach (KeyValuePair<Light, CommandBuffer> entry in m_LightVolumeCmdBuffers)
                {
                    entry.Key.RemoveCommandBuffer(LightEvent.AfterShadowMap, entry.Value);
                    entry.Value.Dispose();
                }

                m_LightVolumeCmdBuffers.Clear();
            }

            if (m_RenderNonShadowVolumes.Buffer != null)
            {
                if (m_RenderNonShadowVolumes.Camera)
                    m_RenderNonShadowVolumes.Camera.RemoveCommandBuffer(RenderNonShadowVolumesEvent, m_RenderNonShadowVolumes.Buffer);
                m_RenderNonShadowVolumes.Buffer.Dispose();
                m_RenderNonShadowVolumes = default;
            }
        }

        private void LateUpdate()
        {
            if (ctxToRenderSecond == null || !ASkyLighting.isActiveOnClient)
                return;
            
            if (ctxToRenderSecond.isCave == true)
            {
                Debug.Log("Pidor");
                RenderSettings.ambientIntensity = ctxToRenderSecond.caveAmbient.value;
            }
            
        }
        void OnPreRender()
        {
            if (!CheckHasSystemSupport())
            {
                enabled = false;
            }

            UpdateResources();
            SetShaderKeyWords();

            // Get the temporary radiance target for use this frame. This is released in OnRenderImage after the final upscale and compose.
#if UNITY_5_6_OR_NEWER
            RenderTextureFormat rtFormat = m_Camera.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
#else
            RenderTextureFormat rtFormat = m_Camera.hdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
#endif
#if UNITY_5_5_OR_NEWER
            m_PerFrameRadianceTarget = RenderTexture.GetTemporary(m_X, m_Y, 0, rtFormat, RenderTextureReadWrite.Linear, AntiAliasingLevel());
#else
            m_PerFrameRadianceTarget = RenderTexture.GetTemporary(m_X, m_Y, 0, rtFormat, RenderTextureReadWrite.Linear, 1);
#endif
            m_PerFrameRadianceTarget.name = "_DS_Haze_PerFrameRadiance";
            m_PerFrameRadianceTarget.filterMode = FilterMode.Point;

            // Use a command buffer to clear the per-frame radiance target at the start of the frame, as it most likely has the contents of the
            // previous frame.
            if (m_ClearRadianceCmdBuffer.Buffer != null)
            {
                m_ClearRadianceCmdBuffer.Buffer.Clear();
                m_ClearRadianceCmdBuffer.Buffer.SetRenderTarget(m_PerFrameRadianceTarget);
                m_ClearRadianceCmdBuffer.Buffer.ClearRenderTarget(false, true, Color.clear);
            }

            DS_HazeCore core = DS_HazeCore.Instance;
            if (core == null)
                return;

            DS_HazeContextItem ctxToRender = core.GetRenderContextAtPositionSphere(transform.position);
            ctxToRenderSecond = ctxToRender;

            if (ctxToRender == null)
            {
                SetGlobalParamsToNull();
            }
            else
            {
                SetMaterialFromContext(ctxToRenderSecond);

                float farClip = m_Camera.farClipPlane;
                float fovWHalf = m_Camera.fieldOfView * 0.5f;
                float dY = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);
                float dX = dY * m_Camera.aspect;
                Vector3 vpC = transform.forward * farClip;
                Vector3 vpR = transform.right * dX * farClip;
                Vector3 vpU = transform.up * dY * farClip;
                m_Material.SetVector("_ViewportCorner", vpC - vpR - vpU);
                m_Material.SetVector("_ViewportRight", vpR * 2.0f);
                m_Material.SetVector("_ViewportUp", vpU * 2.0f);

                if (m_DirectLight && m_RenderAtmosphereVolumetrics)
                {
                    m_DirectionalLightCmdBuffer.Buffer?.Blit(BuiltinRenderTextureType.None, m_PerFrameRadianceTarget, m_Material, (int)m_VolumeSamples + (m_DownsampleFactor == SizeFactor.Half ? 0 : 3));
                }
            }

            if (m_RenderLocalVolumetrics == false)
            {
                return;
            }

            Matrix4x4 gpuProjMtx = GL.GetGPUProjectionMatrix(m_Camera.projectionMatrix, true);
            Matrix4x4 viewProjMtx = gpuProjMtx * m_Camera.worldToCameraMatrix;

            core.GetRenderLightVolumes(transform.position, m_PerFrameLightVolumes, m_PerFrameShadowLightVolumes);

            // If this camera will render light volumes, set the render target.
            if (m_PerFrameLightVolumes.Count > 0)
            {
                m_RenderNonShadowVolumes.Buffer?.SetRenderTarget(m_PerFrameRadianceTarget);
            }

            foreach (DS_HazeLightVolume lv in m_PerFrameLightVolumes)
            {
                lv.SetupMaterialPerFrame(viewProjMtx, m_Camera.worldToCameraMatrix, transform, WillRenderWithTemporalReprojection ? m_InterleavedOffsetIndex : 0);
                // Add this light to this camera's command buffer.
                lv.AddLightRenderCommand(transform, m_RenderNonShadowVolumes.Buffer, (int)m_DownsampleFactor);
            }
            foreach (DS_HazeLightVolume lv in m_PerFrameShadowLightVolumes)
            {
                lv.SetupMaterialPerFrame(viewProjMtx, m_Camera.worldToCameraMatrix, transform, WillRenderWithTemporalReprojection ? m_InterleavedOffsetIndex : 0);

                // This light will render using it's own command buffer.
                lv.FillLightCommandBuffer(m_PerFrameRadianceTarget, transform, (int)m_DownsampleFactor);
                m_LightVolumeCmdBuffers.Add(lv.LightSource, lv.RenderCommandBuffer);
            }
        }

        private RenderBuffer[] blitToMRTColourBuffers = new RenderBuffer[2];
        private void BlitToMRT(RenderTexture source, RenderTexture destination0, RenderTexture destination1, Material mat, int pass)
        {
            blitToMRTColourBuffers[0] = destination0.colorBuffer;
            blitToMRTColourBuffers[1] = destination1.colorBuffer;
            Graphics.SetRenderTarget(blitToMRTColourBuffers, destination0.depthBuffer);

            mat.SetTexture("_MainTex", source);
            mat.SetPass(pass);

            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 0.1f);
            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 0.1f);
            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 0.1f);
            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.1f);
            GL.End();
            GL.PopMatrix();
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            RenderTexture tmpRad = null;
            RenderTexture tmpHalfDepth = null;

            if (m_RenderAtmosphereVolumetrics || m_RenderLocalVolumetrics)
            {
#if UNITY_5_6_OR_NEWER
                tmpRad = RenderTexture.GetTemporary(m_X, m_Y, 0, m_Camera.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
#else
                tmpRad = RenderTexture.GetTemporary(m_X, m_Y, 0, m_Camera.hdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32);
#endif
                tmpHalfDepth = RenderTexture.GetTemporary(m_X, m_Y, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear, 1);

                // Down-sample the depth buffer.
                Graphics.Blit(null, tmpHalfDepth, m_Material, m_DownsampleFactor == SizeFactor.Half ? 10 : 11);
                m_Material.SetTexture("_HalfResDepth", tmpHalfDepth);

                // Bi-lateral Gaussian gather pass.
                Graphics.Blit(m_PerFrameRadianceTarget, tmpRad, m_Material, 6);
                Graphics.Blit(tmpRad, m_PerFrameRadianceTarget, m_Material, 7);

                // Set the previous radiance, depth and current radiance buffers to use during temporal reprojection.
                if (m_TemporalReprojection)
                {
                    m_Material.SetTexture("_PrevAccumBuffer", m_PreviousRadianceTarget);
                    m_Material.SetTexture("_PrevDepthBuffer", m_PreviousDepthTarget);
                }
            }

            m_PerFrameRadianceTarget.filterMode = FilterMode.Bilinear;
            m_Material.SetTexture("_RadianceBuffer", m_PerFrameRadianceTarget);

            // If this DS_HazeView is the last in the blit chain, dest will be null. This should never be the case in an
            // actual game (tonemapping/colour grading/bloom etc. will come after), but to prevent errors while editing,
            // compose into a temporary target and then perform a final blit to the backbuffer.
            RenderTexture tmpDest;
            if (dest == null)
            {
                tmpDest = RenderTexture.GetTemporary(src.width, src.height, src.depth, src.format);
                if (WillRenderWithTemporalReprojection)
                {
                    BlitToMRT(src, tmpDest, m_CurrentRadianceTarget, m_Material, 8);
                }
                else
                {
                    Graphics.Blit(src, tmpDest, m_Material, 8);
                }
                Graphics.Blit(tmpDest, (RenderTexture)null);
                RenderTexture.ReleaseTemporary(tmpDest);
            }
            else
            {
                if (WillRenderWithTemporalReprojection)
                {
                    BlitToMRT(src, dest, m_CurrentRadianceTarget, m_Material, 8);
                }
                else
                {
                    Graphics.Blit(src, dest, m_Material, 8);

                }
            }

            // Grab the depth buffer for reprojection next frame. We need to manually set the render target back to 'dest' afterwards
            // so further rendering (transparencies, image effects) goes into the correct buffer.
            if (WillRenderWithTemporalReprojection)
            {
                Graphics.Blit(src, m_PreviousDepthTarget, m_Material, 9);
                Graphics.SetRenderTarget(dest);

                // Make the radiance buffer available to everyone so skybox transparent shaders can sample volumetrics.
                Shader.SetGlobalTexture("_DS_RadianceBuffer", m_CurrentRadianceTarget);
                RenderTexture.ReleaseTemporary(m_PerFrameRadianceTarget);
            }
            else
            {
                Shader.SetGlobalTexture("_DS_RadianceBuffer", m_PerFrameRadianceTarget);
            }

            if (tmpRad != null)
            {
                RenderTexture.ReleaseTemporary(tmpRad);
            }
            if (tmpHalfDepth != null)
            {
                RenderTexture.ReleaseTemporary(tmpHalfDepth);
            }
        }

        /// <summary>
        /// Update the values used for temporal reprojection and swap the radiance buffer being rendered to next frame.
        /// Also cleanup the per-frame lists and command buffers used to render light volumes.
        /// </summary>
        void OnPostRender()
        {
            if (WillRenderWithTemporalReprojection)
            {
                // Swap the radiance buffers ready for the next frame.
                RenderTexture tmp = m_CurrentRadianceTarget;
                m_CurrentRadianceTarget = m_PreviousRadianceTarget;
                m_PreviousRadianceTarget = tmp;

                // Update the view/projection matrix used to transform from world-space to previous frame clip-space.
                Matrix4x4 thisViewMatrix = m_Camera.worldToCameraMatrix;
                Matrix4x4 thisProjMatrix = GL.GetGPUProjectionMatrix(m_Camera.projectionMatrix, true);
                m_PreviousViewProjMatrix = thisProjMatrix * thisViewMatrix;
                m_PreviousInvViewProjMatrix = m_PreviousViewProjMatrix.inverse;
            }
            else
            {
                RenderTexture.ReleaseTemporary(m_PerFrameRadianceTarget);
            }

            if (m_LightVolumeCmdBuffers.Count > 0)
            {
                foreach (KeyValuePair<Light, CommandBuffer> entry in m_LightVolumeCmdBuffers)
                {
                    entry.Value.Clear();
                }
                m_LightVolumeCmdBuffers.Clear();
            }

            m_DirectionalLightCmdBuffer.Buffer?.Clear();
            m_RenderNonShadowVolumes.Buffer?.Clear();
            m_PerFrameLightVolumes.Clear();
            m_PerFrameShadowLightVolumes.Clear();
        }
    }
}
