using Assets.Src.Lib.ProfileTools;
using UnityEngine;
using UnityEngine.Rendering;

namespace DeepSky.Haze
{
    public enum DS_SamplingQuality { x4, x8, x16, x32 };
    public enum DS_LightFalloff { Unity, Quadratic };

    [ExecuteInEditMode, RequireComponent(typeof(Light)), AddComponentMenu("DeepSky Haze/Light Volume", 2)]
    public class DS_HazeLightVolume : MonoBehaviour
    {
        private static int kConeSubdivisions = 16;
        private static Shader kLightVolumeShader;

        private Light m_Light;

        [SerializeField]
        private Mesh m_MeshProxySphere;
        private Mesh m_ProxyMesh;
        private Matrix4x4 m_LightVolumeTransform;
        private CommandBuffer m_RenderCmd;
        private Material m_VolumeMaterial;
        private Vector3 m_DensityOffset = Vector3.zero;

        [SerializeField]
        private DS_SamplingQuality m_Samples = DS_SamplingQuality.x16;
        [SerializeField]
        private DS_LightFalloff m_Falloff = DS_LightFalloff.Unity;
        [SerializeField]
        private bool m_UseFog = false;
        [SerializeField, Range(0, 100)]
        private float m_Scattering = 1.0f;
        [SerializeField, Range(0, 1)]
        private float m_SecondaryScattering = 0.1f;
        [SerializeField, Range(-1, 1)]
        private float m_ScatteringDirection = 0.75f;
        [SerializeField]
        private Texture3D m_DensityTexture;
        [SerializeField, Range(0.1f, 10.0f)]
        private float m_DensityTextureScale = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float m_DensityTextureContrast = 1.0f;
        [SerializeField]
        private Vector3 m_AnimateDirection = Vector3.zero;
        [SerializeField, Range(0, 10)]
        private float m_AnimateSpeed = 1.0f;
        [SerializeField]
        private float m_StartFade = 25.0f;
        [SerializeField]
        private float m_EndFade = 30.0f;
        [SerializeField, Range(0.01f, 1)]
        private float m_FarClip = 1.0f;

        // For checking when the light proxy mesh needs rebuilding in the editor.
        private LightType m_PreviousLightType = LightType.Point;
        private float m_PreviousAngle = 45.0f;
        private LightShadows m_PreviousShadowMode = LightShadows.None;

        public Light LightSource
        {
            get { return m_Light; }
        }

        public LightType Type
        {
            get
            {
                return m_Light != null ? m_Light.type : LightType.Point;
            }
        }

        public bool CastShadows
        {
            get { return m_Light.shadows != LightShadows.None ? true : false; }
        }

        public CommandBuffer RenderCommandBuffer
        {
            get { return m_RenderCmd; }
        }

        public DS_SamplingQuality Samples
        {
            get { return m_Samples; }
            set { m_Samples = value; }
        }

        public DS_LightFalloff Falloff
        {
            get { return m_Falloff; }
            set { m_Falloff = value; }
        }

        public bool UseFog
        {
            get { return m_UseFog; }
            set { m_UseFog = value; }
        }

        public float Scattering
        {
            get { return m_Scattering; }
            set { m_Scattering = Mathf.Clamp01(value); }
        }

        public float ScatteringDirection
        {
            get { return m_ScatteringDirection; }
            set { m_ScatteringDirection = Mathf.Clamp(value, -1, 1); }
        }

        public Texture3D DensityTexture
        {
            get { return m_DensityTexture; }
            set { m_DensityTexture = value; }
        }

        public float DensityTextureScale
        {
            get { return m_DensityTextureScale; }
            set { m_DensityTextureScale = Mathf.Clamp01(m_DensityTextureScale); }
        }

        public Vector3 AnimateDirection
        {
            get { return m_AnimateDirection; }
            set { m_AnimateDirection = value.normalized; }
        }

        public float AnimateSpeed
        {
            get { return m_AnimateSpeed; }
            set { m_AnimateSpeed = Mathf.Clamp01(value); }
        }

        public float StartFade
        {
            get { return m_StartFade; }
            set { m_StartFade = value > 0 ? value : 1; }
        }

        public float EndFade
        {
            get { return m_EndFade; }
            set { m_EndFade = value > m_StartFade ? value : m_StartFade + 1; }
        }

        /// <summary>
        /// Create the cone proxy mesh (for use when this light type is a spot).
        /// When the near clip value is (almost) 0, the cone is created with a single vertex at the apex.
        /// Otherwise, it is capped with circles at both ends.
        /// </summary>
        private void CreateProxyMeshCone(Mesh proxyMesh)
        {
            Vector3[] verts = null;
            int[] tris = null;

            // Treat this spotlight as a proper cone.
            float rad = Mathf.Tan((m_Light.spotAngle / 2) * Mathf.Deg2Rad) * m_FarClip;

            verts = new Vector3[kConeSubdivisions + 2];
            tris = new int[kConeSubdivisions * 6];

            float rd = Mathf.PI * 2.0f / kConeSubdivisions;
            float angle = 0.0f;

            for (int ri = 0; ri < kConeSubdivisions; ri++)
            {
                verts[ri] = new Vector3(Mathf.Sin(angle) * rad, Mathf.Cos(angle) * rad, m_FarClip);
                angle += rd;
            }

            verts[kConeSubdivisions] = new Vector3(0, 0, m_FarClip);
            verts[kConeSubdivisions + 1] = new Vector3(0, 0, -0.1f); //<-- budge the apex back a little to compensate for the lower res mesh.

            // Faces.
            for (int ti = 0; ti < kConeSubdivisions; ti++)
            {
                // End cap
                tris[ti * 3] = kConeSubdivisions;
                tris[ti * 3 + 1] = ti == kConeSubdivisions - 1 ? 0 : ti + 1;
                tris[ti * 3 + 2] = ti;

                // Cone
                tris[kConeSubdivisions * 3 + ti * 3] = ti;
                tris[kConeSubdivisions * 3 + ti * 3 + 1] = ti == kConeSubdivisions - 1 ? 0 : ti + 1;
                tris[kConeSubdivisions * 3 + ti * 3 + 2] = kConeSubdivisions + 1;
            }

            proxyMesh.vertices = verts;
            proxyMesh.triangles = tris;
            proxyMesh.hideFlags = HideFlags.HideAndDontSave;

            m_PreviousAngle = m_Light.spotAngle;
        }

        /// <summary>
        /// Used by the custom Inspector, if the Light component on this gameobject has
        /// also been changed, we may need to rebuild the proxy mesh.
        /// </summary>
        /// <returns></returns>
        public bool ProxyMeshRequiresRebuild()
        {
            if (m_Light == null)
            {
                return false;
            }

            if (m_ProxyMesh == null || (m_Light.type == LightType.Spot && m_Light.spotAngle != m_PreviousAngle))
            {
                return true;
            }
            return false;
        }

        public bool LightTypeChanged()
        {
            if (m_Light == null)
            {
                return false;
            }

            return m_Light.type != m_PreviousLightType;
        }

        public void UpdateLightType()
        {
            // Changing light type always clears the cookie field.
            m_VolumeMaterial.DisableKeyword("POINT_COOKIE");
            m_VolumeMaterial.DisableKeyword("SPOT_COOKIE");

            if (m_Light.type == LightType.Point)
            {
                m_VolumeMaterial.EnableKeyword("POINT");
                m_VolumeMaterial.DisableKeyword("SPOT");
            }
            else if (m_Light.type == LightType.Spot)
            {
                m_VolumeMaterial.EnableKeyword("SPOT");
                m_VolumeMaterial.DisableKeyword("POINT");
            }
            else
            {
                Debug.LogError("DeepSky::DS_HazeLightVolume: Unsupported light type! " + gameObject.name + " will not render volumetrics.");
                enabled = false;
                return;
            }

            // Changing light type always requires a rebuild of the proxy mesh (unless it's an unsupported type).
            RebuildProxyMesh();

            m_PreviousLightType = m_Light.type;
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (m_MeshProxySphere == null)
                m_MeshProxySphere = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>("Assets/ASkyLighting/DeepSky Haze/Resources/DS_HazeMeshProxySphere.fbx");
        }
#endif
        public void RebuildProxyMesh()
        {
            switch (m_Light.type)
            {
                case LightType.Point:
                    if (m_PreviousLightType != LightType.Point)
                    {
                        DestroyImmediate(m_ProxyMesh);
                    }
                    m_ProxyMesh = m_MeshProxySphere;
                    break;
                case LightType.Spot:
                    if (m_PreviousLightType == LightType.Point)
                    {
                        m_ProxyMesh = new Mesh();
                    }
                    else
                    {
                        if (m_ProxyMesh != null)
                        {
                            m_ProxyMesh.Clear();
                        }
                    }
                    // Create the spot light proxy mesh.
                    CreateProxyMeshCone(m_ProxyMesh);
                    break;
                default:
                    Debug.LogError("DeepSky::DS_HazeLightVolume: Unsupported light type! " + gameObject.name + " will not render volumetrics.");
                    enabled = false;
                    break;
            }
            //m_PreviousLightType = m_Light.type;
        }

        public bool ShadowModeChanged()
        {
            if (m_Light == null)
            {
                return false;
            }

            return m_Light.shadows != m_PreviousShadowMode;
        }

        public void UpdateShadowMode()
        {
            if (m_Light.shadows == LightShadows.None)
            {
                m_VolumeMaterial.DisableKeyword("SHADOWS_DEPTH");
                m_VolumeMaterial.DisableKeyword("SHADOWS_CUBE");
            }
            else
            {
                if (m_Light.type == LightType.Point)
                {
                    m_VolumeMaterial.EnableKeyword("SHADOWS_CUBE");
                    m_VolumeMaterial.DisableKeyword("SHADOWS_DEPTH");
                }
                else if (m_Light.type == LightType.Spot)
                {
                    m_VolumeMaterial.EnableKeyword("SHADOWS_DEPTH");
                    m_VolumeMaterial.DisableKeyword("SHADOWS_CUBE");
                }
            }
            m_PreviousShadowMode = m_Light.shadows;
        }

        public void Register()
        {
            DS_HazeCore core = DS_HazeCore.Instance;

            if (core == null)
            {
                Debug.LogError("DeepSky::DS_HazeLightVolume: Attempting to add a light volume but no HS_HazeCore found in scene! Please make sure there is a DS_HazeCore object.");
            }
            else
            {
                core.AddLightVolume(this);
            }
        }

        public void Deregister()
        {
            DS_HazeCore core = DS_HazeCore.Instance;

            if (core != null)
            {
                core.RemoveLightVolume(this);
            }
        }

        public bool WillRender(Vector3 cameraPos)
        {
            return isActiveAndEnabled & Vector3.Distance(cameraPos, transform.position) < m_EndFade;
        }

        /// <summary>
        /// Update the density texture position (if using one). This needs to happen in Update so multiple cameras get the same
        /// values.
        /// </summary>
        void Update()
        {
            m_DensityOffset -= m_AnimateDirection * m_AnimateSpeed * Time.deltaTime * 0.1f;
        }

        void OnEnable()
        {
            m_Light = GetComponent<Light>();
            if (m_Light == null)
            {
                Debug.LogError("DeepSky::DS_HazeLightVolume: No Light component found on " + gameObject.name);
                enabled = false;
            }

            if (kLightVolumeShader == null)
            {
                kLightVolumeShader = Profile.Load<Shader>("DS_HazeLightVolume");
            }

            if (m_VolumeMaterial == null)
            {
                m_VolumeMaterial = new Material(kLightVolumeShader);
                m_VolumeMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            if (m_RenderCmd == null)
            {
                m_RenderCmd = new CommandBuffer();
                m_RenderCmd.name = gameObject.name + "_DS_Haze_RenderLightVolume";
                m_Light.AddCommandBuffer(LightEvent.AfterShadowMap, m_RenderCmd);
            }

            if (LightTypeChanged())
            {
                UpdateLightType();
            }
            else if (ProxyMeshRequiresRebuild())
            {
                RebuildProxyMesh();
            }

            if (ShadowModeChanged())
            {
                UpdateShadowMode();
            }

            Register();
        }

        void OnDisable()
        {
            Deregister();
        }

        void OnDestroy()
        {
            if (m_RenderCmd != null)
            {
                m_RenderCmd.Dispose();
            }

            Deregister();

            if (m_ProxyMesh != null && m_Light.type != LightType.Point)
            {
                DestroyImmediate(m_ProxyMesh);
            }

            if (m_VolumeMaterial != null)
            {
                DestroyImmediate(m_VolumeMaterial);
            }
        }

        private int SetShaderPassAndMatrix(Transform cameraTransform, int downSampleFactor, out Matrix4x4 worldMtx)
        {
            worldMtx = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(m_Light.range, m_Light.range, m_Light.range));

            int shaderPass = 0; // <-- Default == LightType.Point
            if (m_Light.type == LightType.Spot)
            {
                float cosTheta = Mathf.Cos((m_Light.spotAngle / 2.0f) * Mathf.Deg2Rad);
                Vector3 lvec = (cameraTransform.position - transform.position).normalized;
                float LdotC = Vector3.Dot(lvec, transform.forward);
                if (LdotC > cosTheta)
                {
                    // Inside the spot cone.
                    shaderPass = 1;
                }
                else
                {
                    // Outside.
                    shaderPass = 2;
                }
            }

            if (downSampleFactor == 4) shaderPass += 3;
            if (m_Falloff == DS_LightFalloff.Quadratic) shaderPass += 6;
            if (m_UseFog) shaderPass += 12;

            return shaderPass;
        }

        public void FillLightCommandBuffer(RenderTexture radianceTarget, Transform cameraTransform, int downSampleFactor)
        {
            m_RenderCmd.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);

            Matrix4x4 worldMtx;
            int shaderPass = SetShaderPassAndMatrix(cameraTransform, downSampleFactor, out worldMtx);

            m_RenderCmd.SetRenderTarget(radianceTarget);
            m_RenderCmd.DrawMesh(m_ProxyMesh, worldMtx, m_VolumeMaterial, 0, shaderPass);
        }

        public void AddLightRenderCommand(Transform cameraTransform, CommandBuffer cmd, int downSampleFactor)
        {
            Matrix4x4 worldMtx;
            int shaderPass = SetShaderPassAndMatrix(cameraTransform, downSampleFactor, out worldMtx);
            cmd.DrawMesh(m_ProxyMesh, worldMtx, m_VolumeMaterial, 0, shaderPass);
        }

        public void SetupMaterialPerFrame(Matrix4x4 viewProjMtx, Matrix4x4 viewMtx, Transform cameraTransform, float offsetIndex)
        {
            m_VolumeMaterial.DisableKeyword("SAMPLES_4");
            m_VolumeMaterial.DisableKeyword("SAMPLES_8");
            m_VolumeMaterial.DisableKeyword("SAMPLES_16");
            m_VolumeMaterial.DisableKeyword("SAMPLES_32");

            // Common parameters.
            switch (m_Samples)
            {
                case DS_SamplingQuality.x4:
                    m_VolumeMaterial.EnableKeyword("SAMPLES_4");
                    break;
                case DS_SamplingQuality.x8:
                    m_VolumeMaterial.EnableKeyword("SAMPLES_8");
                    break;
                case DS_SamplingQuality.x16:
                    m_VolumeMaterial.EnableKeyword("SAMPLES_16");
                    break;
                case DS_SamplingQuality.x32:
                    m_VolumeMaterial.EnableKeyword("SAMPLES_32");
                    break;
                default:
                    m_VolumeMaterial.EnableKeyword("SAMPLES_16");
                    break;
            }

            float fadeFactor = 1.0f - Mathf.Clamp01((Vector3.Distance(cameraTransform.position, transform.position) - m_StartFade) / (m_EndFade - m_StartFade));

            m_VolumeMaterial.SetVector("_DS_HazeSamplingParams", new Vector4(offsetIndex, 0, m_DensityTextureContrast, 0));
            m_VolumeMaterial.SetVector("_DS_HazeCameraDirection", new Vector4(cameraTransform.forward.x, cameraTransform.forward.y, cameraTransform.forward.z, 1.0f));
            m_VolumeMaterial.SetColor("_DS_HazeLightVolumeColour", m_Light.color.linear * m_Light.intensity * fadeFactor);
            m_VolumeMaterial.SetVector("_DS_HazeLightVolumeScattering", new Vector4(m_Scattering, m_SecondaryScattering, m_ScatteringDirection, Mathf.Clamp01(1.0f - m_SecondaryScattering)));
            m_VolumeMaterial.SetVector("_DS_HazeLightVolumeParams0", new Vector4(transform.position.x, transform.position.y, transform.position.z, m_Light.range));

            Matrix4x4 worldMtx = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(m_Light.range, m_Light.range, m_Light.range));
            m_VolumeMaterial.SetMatrix("_WorldViewProj", viewProjMtx * worldMtx);
            m_VolumeMaterial.SetMatrix("_WorldView", viewMtx * worldMtx);

            if (m_DensityTexture)
            {
                m_VolumeMaterial.EnableKeyword("DENSITY_TEXTURE");
                m_VolumeMaterial.SetTexture("_DensityTexture", m_DensityTexture);
                m_VolumeMaterial.SetVector("_DS_HazeDensityParams", new Vector4(m_DensityOffset.x, m_DensityOffset.y, m_DensityOffset.z, m_DensityTextureScale * 0.01f));
            }
            else
            {
                m_VolumeMaterial.DisableKeyword("DENSITY_TEXTURE");
            }

            bool shadows = m_Light.shadows != LightShadows.None;

            // Setup keywords.
            if (m_Light.type == LightType.Point)
            {
                m_VolumeMaterial.DisableKeyword("SPOT_COOKIE");
                m_VolumeMaterial.DisableKeyword("SHADOWS_DEPTH");

                if (shadows)
                {
                    m_VolumeMaterial.EnableKeyword("SHADOWS_CUBE");
                }
                else
                {
                    m_VolumeMaterial.DisableKeyword("SHADOWS_CUBE");
                }

                if (m_Light.cookie)
                {
                    m_VolumeMaterial.EnableKeyword("POINT_COOKIE");

                    m_VolumeMaterial.SetMatrix("_DS_Haze_WorldToCookie", transform.worldToLocalMatrix);
                    m_VolumeMaterial.SetTexture("_LightTexture0", m_Light.cookie);
                }
                else
                {
                    m_VolumeMaterial.DisableKeyword("POINT_COOKIE");
                }
            }
            else if (m_Light.type == LightType.Spot)
            {
                m_VolumeMaterial.DisableKeyword("POINT_COOKIE");
                m_VolumeMaterial.DisableKeyword("SHADOWS_CUBE");

                if (shadows)
                {
                    m_VolumeMaterial.EnableKeyword("SHADOWS_DEPTH");

                    Matrix4x4 lightView = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
                    Matrix4x4 shadowClip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
#if UNITY_5_3_OR_NEWER
#if UNITY_5_5_OR_NEWER
                    Matrix4x4 shadowProj = Matrix4x4.Perspective(m_Light.spotAngle, 1, m_Light.range, m_Light.shadowNearPlane);
#else
                    Matrix4x4 shadowProj = Matrix4x4.Perspective(m_Light.spotAngle, 1, m_Light.shadowNearPlane, m_Light.range);
#endif
#else
                    Matrix4x4 shadowProj = Matrix4x4.Perspective(m_Light.spotAngle, 1, m_Light.range * 0.04f, m_Light.range);
#endif

                    Matrix4x4 mtx = shadowClip * shadowProj;
                    mtx[0, 2] *= -1.0f;
                    mtx[1, 2] *= -1.0f;
                    mtx[2, 2] *= -1.0f;
                    mtx[3, 2] *= -1.0f;
                    mtx *= lightView;

                    m_VolumeMaterial.SetMatrix("_DS_Haze_WorldToShadow", mtx);
                }
                else
                {
                    m_VolumeMaterial.DisableKeyword("SHADOWS_DEPTH");
                }

                float cosTheta = Mathf.Cos((m_Light.spotAngle / 2.0f) * Mathf.Deg2Rad);
                Vector3 planeC = transform.position + transform.forward * m_Light.range;
                float planeD = -Vector3.Dot(planeC, transform.forward);

                m_VolumeMaterial.SetVector("_DS_HazeLightVolumeParams1", new Vector4(transform.forward.x, transform.forward.y, transform.forward.z, 1.0f));
                m_VolumeMaterial.SetVector("_DS_HazeLightVolumeParams2", new Vector4(cosTheta, 1.0f / cosTheta, planeD, 0.0f));

                if (m_Light.cookie)
                {
                    m_VolumeMaterial.EnableKeyword("SPOT_COOKIE");

                    // In forward mode unity_WorldToLight isn't set during shadow rendering, so we have to manually create the cookie
                    // projection matrix.
                    Matrix4x4 lightView = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
                    Matrix4x4 cookieClip = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1.0f));
                    Matrix4x4 cookieProj = Matrix4x4.Perspective(m_Light.spotAngle, 1, 0, 1);

                    m_VolumeMaterial.SetMatrix("_DS_Haze_WorldToCookie", cookieClip * cookieProj * lightView);

                    m_VolumeMaterial.SetTexture("_LightTexture0", m_Light.cookie);
                }
                else
                {
                    m_VolumeMaterial.DisableKeyword("SPOT_COOKIE");
                }
            }
        }
    }
}
