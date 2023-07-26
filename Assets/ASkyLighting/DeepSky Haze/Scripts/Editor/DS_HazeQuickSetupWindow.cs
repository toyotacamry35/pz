using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace DeepSky.Haze
{
    public class DS_HazeQuickSetupWindow : EditorWindow
    {
        private const string kCameraTxt = "Add the <b>main camera</b> where you want to see atmospheric effects.";
        private const string kLightTxt = "Add the <b>directional light</b> you want to use as the sunlight.";
        private const string kObjTxt = "<b>(Optional)</b> Add a <b>Terrain</b> or GameObject with a <b>Mesh Renderer</b>, the " +
            "atmospherics zone will automatically be fitted around it.";

        private Light m_DirectionalLight;
        private Camera m_Camera;
        private bool m_Fit;
        private GameObject m_Object;
        private int m_PresetInd = 0;
        private string[] m_PresetPaths;
        private string[] m_PresetDisplayNames;

        [MenuItem("GameObject/DeepSky Haze/Create Quick Setup", false, 14)]
        public static void ShowWindow()
        {
            DS_HazeQuickSetupWindow window = EditorWindow.GetWindow<DS_HazeQuickSetupWindow>("DeepSky Haze Quick Setup") as DS_HazeQuickSetupWindow;
            window.GetPresetPathsAndNames();
            window.maxSize = new Vector2(400, 300);
            window.Show();
        }

        void OnGUI()
        {
            string message = string.Empty;
            MessageType messageType = MessageType.Info;
            GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox);
            helpBoxStyle.richText = true;

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Quick Setup", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            {
                m_Camera = (Camera)EditorGUILayout.ObjectField("Camera", m_Camera, typeof(Camera), true);
                GUILayout.Space(15);

                m_DirectionalLight = (Light)EditorGUILayout.ObjectField("Directional Light", m_DirectionalLight, typeof(Light), true);
                GUILayout.Space(15);

                m_Fit = EditorGUILayout.BeginToggleGroup("Fit to object bounds (Optional)", m_Fit);
                m_Object = (GameObject)EditorGUILayout.ObjectField("Object", m_Object, typeof(GameObject), true);
                EditorGUILayout.EndToggleGroup();
                GUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Preset", EditorStyles.boldLabel);
                    m_PresetInd = EditorGUILayout.Popup(m_PresetInd, m_PresetDisplayNames);
                }
                EditorGUILayout.EndHorizontal();

                if (m_Camera == null && m_DirectionalLight == null)
                {
                    message = "Add the main camera where you want to render atmospheric effects and the directional light you want to use as the sunlight.";
                }
                else
                {
                    if (m_DirectionalLight == null)
                    {
                        message = "Add the directional light you want to use as the sunlight.";
                    }
                    if (m_Camera == null)
                    {
                        message = "Add the main camera where you want to render atmospheric effects.";
                    }
                }

                if (m_Camera != null && m_Camera.actualRenderingPath == RenderingPath.Forward && QualitySettings.antiAliasing != 0)
                {
                    message = "Float render targets are not supported in Forward when MSAA is enabled. Please disable MSAA in the project Quality Settings.";
                    messageType = MessageType.Error;
                }

                EditorGUILayout.Separator();
                if (!m_Camera || !m_DirectionalLight || messageType == MessageType.Error) GUI.enabled = false;
                if (GUILayout.Button("Go", GUILayout.Height(50)))
                {
                    // Do quick setup.
                    AddQuickSetupToScene(m_Object, m_PresetPaths[m_PresetInd]);
                }
                GUI.enabled = true;

                EditorGUILayout.HelpBox(message, messageType);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Get the paths and names of presets from the DeepSky Haze/Contexts directory.
        /// These are displayed in a dropdown for quickly applying settings when the
        /// first zone is created.
        /// </summary>
        private void GetPresetPathsAndNames()
        {
            string[] presetPaths = Directory.GetFiles(Application.dataPath + "/ASkyLighting/LightingPresets/Haze", "*.asset");

            // Need to subtract Application.dataPath".
            int subInd = Application.dataPath.Length + 1;

            m_PresetPaths = new string[presetPaths.Length];
            m_PresetDisplayNames = new string[presetPaths.Length];
            for (int pi = 0; pi < m_PresetDisplayNames.Length; pi++)
            {
                string substr = presetPaths[pi].Substring(subInd);
                m_PresetPaths[pi] = Path.Combine("Assets", substr);
                m_PresetDisplayNames[pi] = Path.GetFileNameWithoutExtension(m_PresetPaths[pi]);
            }
        }

        /// <summary>
        /// Create the Controller, Zone and View needed to see DeepSky effects, fitting
        /// the zone around the passed terrain or object (if supplied) and applying the
        /// selected preset.
        /// </summary>
        /// <param name="boundsObject"> The Zone will be scaled to surround this object/terrain. </param>
        /// <param name="presetPath"> Asset path to preset Context to load settings from. </param>
        private void AddQuickSetupToScene(GameObject boundsObject, string presetPath)
        {
            DS_HazeCore core = DS_HazeCore.Instance;
            GameObject cont;
            if (core == null)
            {
                cont = new GameObject("DS_HazeController", typeof(DS_HazeCore));
                cont.transform.position = new Vector3(0, 0, 0);
            }
            else
            {
                cont = core.gameObject;
                Debug.Log("DeepSky::AddQuickSetupToScene - There is already a Haze Controller in this scene! Skipping adding a new one.");
            }

            GameObject zone = new GameObject("DS_HazeZone", typeof(DS_HazeZone));
            DS_HazeContextAsset ctxAsset = AssetDatabase.LoadAssetAtPath<DS_HazeContextAsset>(presetPath);
            if (ctxAsset == null)
            {
                Debug.LogWarning("DeepSky::AddQuickSetupToScene - Unable to load preset " + presetPath);
            }
            else
            {
                DS_HazeZone zoneComp = zone.GetComponent<DS_HazeZone>();
                DS_HazeZoneEditor.SetZoneFromContextPreset(zoneComp, ctxAsset);
            }

            Vector3 zoneSize = new Vector3(250, 100, 250);
            Vector3 zoneLocation = Vector3.zero;

            if (boundsObject)
            {
                zoneLocation = boundsObject.transform.position;

                bool useDefault = false;
                Vector3 objSize = zoneSize;

                Terrain ter = boundsObject.GetComponent<Terrain>();
                if (ter)
                {
                    // This is a terrain, get the size from the terrain data and scale zone to default inner blend zone.
                    objSize = ter.terrainData.size;
                    zoneLocation += ter.terrainData.size * 0.5f;
                }
                else
                {
                    // Normal object, fit to mesh renderer bounds (if there is a mesh renderer).
                    MeshRenderer mr = boundsObject.GetComponent<MeshRenderer>();
                    if (mr)
                    {
                        objSize = mr.bounds.size;
                        zoneLocation = mr.bounds.center;
                    }
                    else
                    {
                        Debug.Log("DeepSky::AddQuickSetupToScene - Supplied object has no Mesh Renderer to get bounds from! Using default size.");
                        useDefault = true;
                    }
                }

                if (!useDefault)
                {
                    // Make sure there is at least some size.
                    if (objSize.x < 10) objSize.x = 10;
                    if (objSize.y < 10) objSize.y = 10;
                    if (objSize.z < 10) objSize.z = 10;

                    zoneSize = objSize;
                    float dT = Mathf.Min(zoneSize.x, zoneSize.y, zoneSize.z) * 0.1f;
                    Vector3 scale = new Vector3(zoneSize.x / (zoneSize.x - dT), zoneSize.y / (zoneSize.y - dT), zoneSize.z / (zoneSize.z - dT));
                    zoneSize.Scale(scale);
                    zoneSize.y += objSize.y * 0.1f; // Scale up by 10% in Y to give a little extra room at the top/bottom of the terrain/mesh.
                }
            }
            zone.transform.localScale = zoneSize;
            zone.transform.position = zoneLocation;
            zone.transform.SetParent(cont.transform);

            DS_HazeView view = m_Camera.GetComponent<DS_HazeView>();
            if (view)
            {
                Debug.Log("DeepSky::AddQuickSetupToScene - The camera already has a Haze View! Skipping adding a new one.");
                return;
            }
            else
            {
                view = m_Camera.gameObject.AddComponent<DS_HazeView>();
                view.DirectLight = m_DirectionalLight;
            }
        }
    }
}

