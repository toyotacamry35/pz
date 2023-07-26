using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TOD;

namespace DeepSky.Haze
{
    [ExecuteInEditMode, AddComponentMenu("DeepSky Haze/Controller", 51)]
    public class DS_HazeCore : MonoBehaviour
    {
		public static bool isGizmos = false;
        public static string kVersionStr = "DeepSky Haze v1.4.0";
        private static int kGUIHeight = 180;
        public enum NoiseTextureSize { x8 = 8, x16 = 16, x32 = 32, x64 = 64 };
        public enum DebugGUIPosition { TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight };

        public HazeItemEclipse current;// = new HazeItemEclipse();

        public Color hazeColor;
        public float hazeIntensity;
        public float hazeDirectIntensity;

        private static DS_HazeCore instance;
        public static DS_HazeCore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DS_HazeCore>();
                }

                return instance;
            }
        }

        #region FIELDS
        [SerializeField, Range(0, 1), Tooltip("The time at which Zones will evaluate their settings. Animate this or set in code to create time-of-day transitions.")]
        private float m_Time = 0.0f;
        [SerializeField]
        public DS_HazeZone mainPreset;
        [SerializeField]
        private DebugGUIPosition m_DebugGUIPosition = DebugGUIPosition.TopLeft;

        // Volumetric lights set.
        private HashSet<DS_HazeLightVolume> m_LightVolumes = new HashSet<DS_HazeLightVolume>();
        public HashSet<DS_HazeZoneSphere> m_ZoneSpheres = new HashSet<DS_HazeZoneSphere>();

        [SerializeField] private Texture3D m_NoiseLUT;

        // Editor and GUI.
        [SerializeField]
        private bool m_ShowDebugGUI = false;
        private Vector2 m_GUIScrollPosition;
        private int m_GUISelectedView = -1;
        private bool m_GUISelectionPopup = false;
        private DS_HazeView m_GUIDisplayedView = null;

        public Texture3D NoiseLUT
        {
            get { return m_NoiseLUT; }
        }
        #endregion

        /*
        private void Update()
        {
            ForceUpdate();
        }
        */
        public void ForceUpdate()
        {
            m_Time = ASkyLighting.CGTime;
            mainPreset.SetupCurves();

            
        }
        private void SetGlobalHeightFalloff()
        {
            Shader.DisableKeyword("DS_HAZE_HEIGHT_FALLOFF_NONE");
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogError("DeepSky::DS_HazeCore:Awake - There is more than one Haze Controller in this scene! Disabling " + name);
                enabled = false;
            }

            current = new HazeItemEclipse();
        }

        void OnEnable()
        {
            SetGlobalHeightFalloff();

            Shader.SetGlobalTexture("_SamplingOffsets", m_NoiseLUT);
        }


        public void SetGlobalNoiseLUT()
        {
            Shader.SetGlobalTexture("_SamplingOffsets", m_NoiseLUT);
        }

        public void AddLightVolume(DS_HazeLightVolume lightVolume)
        {
            RemoveLightVolume(lightVolume);
            m_LightVolumes.Add(lightVolume);
        }
        public void RemoveLightVolume(DS_HazeLightVolume lightVolume)
        {
            m_LightVolumes.Remove(lightVolume);
        }

        public void AddZoneSphere(DS_HazeZoneSphere zoneSphere)
        {
            RemoveZoneSphere(zoneSphere);
            m_ZoneSpheres.Add(zoneSphere);
        }

        public void RemoveZoneSphere(DS_HazeZoneSphere zoneSphere)
        {
            m_ZoneSpheres.Remove(zoneSphere);
        }

        public void GetRenderLightVolumes(Vector3 cameraPosition, List<DS_HazeLightVolume> lightVolumes, List<DS_HazeLightVolume> shadowVolumes)
        {
            foreach (DS_HazeLightVolume lv in m_LightVolumes)
            {
                if (lv.WillRender(cameraPosition))
                {
                    if (lv.CastShadows)
                    {
                        shadowVolumes.Add(lv);
                    }
                    else
                    {
                        lightVolumes.Add(lv);
                    }
                }
            }
        }

		[ContextMenu("Switch Gizmos")]
		public void SwitchGizmo()
		{
			isGizmos = !isGizmos;
		}

        private List<DS_HazeZoneSphere> blendZones = new List<DS_HazeZoneSphere>();
        public DS_HazeContextItem GetRenderContextAtPositionSphere(Vector3 position)
        {
            blendZones.Clear();
            foreach (DS_HazeZoneSphere sp in m_ZoneSpheres)
            {
                if (sp == null)
                {
                    m_ZoneSpheres.Remove(sp);
                }
                if ((sp.m_BlendRangeOutSphere + sp.m_BlendRangeSphere >= Vector3.Distance(position, sp.transform.position)) && sp.enabled)
                {
                    blendZones.Add(sp);
                }
            }

            DS_HazeContextItem renderContext = mainPreset.Context.m_ComplexItem;
            renderContext.caveAmbient = ASkyLighting._instance.context.ambientIntensity;

            if (blendZones.Count == 0)
                return renderContext;
            
            blendZones.Sort(delegate (DS_HazeZoneSphere z1, DS_HazeZoneSphere z2)
            {
                if (z1 < z2) return -1;
                else return 1;
            });
            
            
            for (int ci = 0; ci < blendZones.Count; ci++)
                renderContext.Lerp(blendZones[ci].Context.m_ComplexItem, blendZones[ci].GetBlendWeight(position));

            return renderContext;
        }
       
#if UNITY_EDITOR
        void OnGUI()
        {
            
            if (!m_ShowDebugGUI) return;

            Rect guiPosition = new Rect(5, 5, 256, kGUIHeight);

            switch (m_DebugGUIPosition)
            {
                case DebugGUIPosition.TopCenter:
                    guiPosition.x = Screen.width / 2 - 128;
                    break;
                case DebugGUIPosition.TopRight:
                    guiPosition.x = Screen.width - 261;
                    break;
                case DebugGUIPosition.CenterLeft:
                    guiPosition.y = Screen.height / 2 - 64;
                    break;
                case DebugGUIPosition.Center:
                    guiPosition.x = Screen.width / 2 - 128;
                    guiPosition.y = Screen.height / 2 - 64;
                    break;
                case DebugGUIPosition.CenterRight:
                    guiPosition.x = Screen.width - 261;
                    guiPosition.y = Screen.height / 2 - 64;
                    break;
                case DebugGUIPosition.BottomLeft:
                    guiPosition.y = Screen.height - (kGUIHeight + 5);
                    break;
                case DebugGUIPosition.BottomCenter:
                    guiPosition.x = Screen.width / 2 - 128;
                    guiPosition.y = Screen.height - (kGUIHeight + 5);
                    break;
                case DebugGUIPosition.BottomRight:
                    guiPosition.x = Screen.width - 261;
                    guiPosition.y = Screen.height - (kGUIHeight + 5);
                    break;
                default: // DebugGUIPosition.TopLeft
                    break;
            }

            Rect dropDownPosition = guiPosition;
            dropDownPosition.y += 50;
            dropDownPosition.height -= 50;

            GUIStyle headerStyle = new GUIStyle();
            headerStyle.normal.textColor = Color.white;
            headerStyle.alignment = TextAnchor.UpperCenter;
            headerStyle.padding = new RectOffset(5, 5, 5, 5);
            headerStyle.fontStyle = FontStyle.Bold;

            GUIStyle currentViewStyle = new GUIStyle(GUI.skin.GetStyle("box"));
            currentViewStyle.fontSize = 12;
            currentViewStyle.alignment = TextAnchor.MiddleLeft;
            currentViewStyle.margin.left = currentViewStyle.margin.right = 0;

            GUIStyle dropDownButtonStyle = new GUIStyle(GUI.skin.GetStyle("button"));
            dropDownButtonStyle.fontSize = 14;
            dropDownButtonStyle.alignment = TextAnchor.MiddleCenter;
            dropDownButtonStyle.normal.textColor = Color.grey;
            dropDownButtonStyle.hover.textColor = Color.white;
            dropDownButtonStyle.margin.left = dropDownButtonStyle.margin.right = 0;

            GUIStyle infoTextStyle = new GUIStyle(GUI.skin.GetStyle("label"));
            infoTextStyle.fontSize = 12;
            infoTextStyle.margin = new RectOffset(0, 0, 0, 0);
            infoTextStyle.padding = new RectOffset(2, 2, 2, 2);

            GUIStyle popupWindowStyle = new GUIStyle(GUI.skin.GetStyle("window"));
            popupWindowStyle.fontSize = 12;
            popupWindowStyle.alignment = TextAnchor.UpperLeft;

            GUI.Box(guiPosition, GUIContent.none);
            GUILayout.BeginArea(guiPosition, kVersionStr, headerStyle);
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Box(m_GUIDisplayedView != null ? m_GUIDisplayedView.gameObject.name : "(None)", currentViewStyle, GUILayout.Width(220));
            if (GUILayout.Button(m_GUISelectionPopup ? '\u25B2'.ToString() : '\u25BC'.ToString(), dropDownButtonStyle))
            {
                m_GUISelectionPopup = !m_GUISelectionPopup;
            }
            GUILayout.EndHorizontal();
            
            if (m_GUISelectionPopup)
            {
                GUILayout.Window(0, dropDownPosition, ViewSelectionPopup, "Select A View", popupWindowStyle);
            }

            if (!m_GUISelectionPopup)
            {
                if (m_GUIDisplayedView == null) GUI.enabled = false;

                GUILayout.BeginHorizontal();
                GUILayout.Label("Volume samples", infoTextStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_GUIDisplayedView != null ? m_GUIDisplayedView.SampleCount.ToString() : "-", infoTextStyle);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Atmosphere volumetrics", infoTextStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_GUIDisplayedView != null ? m_GUIDisplayedView.RenderAtmosphereVolumetrics.ToString() : "-", infoTextStyle);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Local volumetrics", infoTextStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_GUIDisplayedView != null ? m_GUIDisplayedView.RenderLocalVolumetrics.ToString() : "-", infoTextStyle);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Scaling factor", infoTextStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_GUIDisplayedView != null ? "1/" + m_GUIDisplayedView.DownSampleFactor.ToString() : "-", infoTextStyle);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Uses temporal reprojection", infoTextStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_GUIDisplayedView != null ? m_GUIDisplayedView.WillRenderWithTemporalReprojection.ToString() : "-", infoTextStyle);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Render target size", infoTextStyle);
                GUILayout.FlexibleSpace();
                string rtSize = "-";
                if (m_GUIDisplayedView != null)
                {
                    Vector2 rts = m_GUIDisplayedView.RadianceTargetSize;
                    rtSize = rts.x.ToString() + "x" + rts.y.ToString();
                }
                GUILayout.Label(rtSize, infoTextStyle);
                GUILayout.EndHorizontal();

                GUI.enabled = true;
            }
            GUILayout.EndArea();
        }

        void ViewSelectionPopup(int id)
        {
            GUIStyle elementStyle = new GUIStyle(GUI.skin.GetStyle("button"));
            elementStyle.fontSize = 12;
            elementStyle.normal.textColor = Color.grey;
            elementStyle.hover.textColor = Color.white;
            elementStyle.alignment = TextAnchor.MiddleLeft;
            elementStyle.margin = new RectOffset(0, 0, 0, 0);

            DS_HazeView[] views = FindObjectsOfType<DS_HazeView>();

            m_GUIScrollPosition = GUILayout.BeginScrollView(m_GUIScrollPosition);
            string[] viewNames = views.Select(v => v.gameObject.name).ToArray();

            int select = GUILayout.SelectionGrid(m_GUISelectedView < viewNames.Length ? m_GUISelectedView : 0, viewNames, 1, elementStyle);
            if (select != m_GUISelectedView)
            {
                m_GUISelectedView = select;
                m_GUIDisplayedView = views[m_GUISelectedView];
                m_GUISelectionPopup = false;
            }
            GUILayout.EndScrollView();
        }
#endif
    }
}
