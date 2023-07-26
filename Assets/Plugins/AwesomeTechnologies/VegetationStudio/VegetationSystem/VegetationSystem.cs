using System;
using System.Collections.Generic;
using System.Threading;
using AwesomeTechnologies.Billboards;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Profiling;
using AwesomeTechnologies.Utility;
#if TOUCH_REACT
using AwesomeTechnologies.TouchReact;
#endif 

using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine.Rendering;
using AwesomeTechnologies.Utility.Quadtree;
using AwesomeTechnologies.VegetationStudio;
using UnityEngine.AI;
using RandomNumberGenerator = AwesomeTechnologies.Utility.RandomNumberGenerator;

namespace AwesomeTechnologies
{
    public enum VegetationSystemWaterSourceType
    {
        Manual,
        FollowTransform,
        VegetationSystem
    }

    [HelpURL("http://www.awesometech.no/index.php/home/vegetation-studio/components/vegetation-system")]
    [AwesomeTechnologiesScriptOrder(100)]
    [ExecuteInEditMode]
    public partial class VegetationSystem : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (WindWavesTexture == null)
            {
                WindWavesTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/PerlinSeamless.png");
            }
        }
#endif
        public static string str = "";

        [Header("Camera/Terrain")]
        public bool AutoselectCamera = true;
        public bool AutoselectTerrain = true;
        public Camera SelectedCamera;

        [Header("Grid settings")]
        public float CellSize = 10f;
        private float _potentialVisibleCellBufferRange = 30f;
        public bool OverrideCellSize;
        public float CustomCellSize = 40f;
        

        [Header("Render Settings")]
        public bool RecieveShadows = true;
        public bool RenderVegetation = true;
        public bool RenderTrees = true;
        public bool DisableUnityDetails = true;
        public bool DisableUnityTrees = true;
        public bool SetUnityTerrainPixelError = true;
        public bool RenderSingleCamera;
        public bool RenderInstanced = true;
        public bool UseComputeShaders;
        public bool UseGPUCulling = true;
        public bool UseCPUCulling = true;
        public bool UseObjectCPUCulling = false;
        public bool UseLargeObjectCPUCulling = false;
        public bool LoadUnityTerrainDetails = false;
        public Light SunDirectionalLight;

        public bool DirectFromCell = true;
        public bool UseIndirectLoDs;
        public bool isRenderWait = false;
        public bool DisableWhenSaving;

        [Header("Vegetation Packages")]
        public int VegetationPackageIndex;
        public List<VegetationPackage> VegetationPackageList = new List<VegetationPackage>();

        [Header("Editor Settings")]
        public bool ShowCellGrid;
        public bool ShowCellLoadState;

        public GameObject WindSampler;

        public VegetationPackage currentVegetationPackage;
        public bool IsDirty;
        public bool InitDone;
        public bool isCamera = false;

        [Header("Resources")]
        public GameObject TextureGrassPrefab;
        public GameObject TexturePatchGrassPrefab;
        public VegetationCellsStorage vegCellStorage;

        public bool UseMultithreading { get; set; } = false;
        public bool UseListMultithreading { get; set; } = false;
        public bool DrawVegetationFromCells;
        public bool ExcludeSeaLevelCells;
        public bool EnableVegetationItemIDEdit = false;

        [Header("Info")]
        public int TreeCount;
        public int GrassCount;

        public float CullFarStart = 100f;
        public float CullFarDistance = 10f;
        private bool _floatingOriginChanged;

        public delegate void MultiVegetationCellRefreshDelegate(VegetationSystem vegetationSystem);
        public MultiVegetationCellRefreshDelegate OnVegetationCellRefreshDelegate;

        public delegate void MultiVegetationCellVisibleDelegate(VegetationCell vegetationCell, int distanceBand);
        public MultiVegetationCellVisibleDelegate OnVegetationCellVisibleDelegate;

        public delegate void MultiVegetationCellInvisibleDelegate(VegetationCell vegetationCell, int distanceBand);
        public MultiVegetationCellInvisibleDelegate OnVegetationCellInvisibleDelegate;

        public delegate void MultiVegetationCellChangeDistanceBandDelegate(VegetationCell vegetationCell, int distanceBand, int previousDistanceBand);
        public MultiVegetationCellChangeDistanceBandDelegate OnVegetationCellChangeDistanceBandDelegate;

        public delegate void MultiResetVisibleCellListDelegate();
        public MultiResetVisibleCellListDelegate OnResetVisibleCellListDelegate;

        public delegate void MultiLoadVegetationPackageDelegate(VegetationPackage vegetationPackage);
        public MultiLoadVegetationPackageDelegate OnSetVegetationPackageDelegate;

        public delegate void MultiTerrainOnSettingsChangeDelegate();
        public MultiTerrainOnSettingsChangeDelegate OnTerrainSettingsChangeDelegate;

        public delegate void MultiRefreshVegetationBillboardsDelegate(Bounds bounds);
        public MultiRefreshVegetationBillboardsDelegate OnRefreshVegetationBillboardsDelegate;

        [SerializeField]
        public Terrain currentTerrain;

        private TerrainCollider _currentTerrainCollider;
        public VegetationStudioManager vsm;

        [SerializeField]
        public UnityTerrainData UnityTerrainData;
        public VegetationSettings vegetationSettings = new VegetationSettings();
        public int CurrentTabIndex;
        public int LastTabIndex;
        private bool _sleepMode;
        public bool AutomaticWakeup = true;

        private readonly ObjectPool<LODVegetationInstanceInfo> _LODVegetationInstanceInfoListPool = new ObjectPool<LODVegetationInstanceInfo>();
        private Material _windSamplerMaterial;

#if UNITY_2018_1_OR_NEWER
        private List<Vector3> _giPositionList = new List<Vector3>();
        private List<SphericalHarmonicsL2> _giProbeList = new List<SphericalHarmonicsL2>();
        private List<Vector4> _giOcclusionList = new List<Vector4>();  
#endif
        public void SetSleepMode(bool value)
        {
            _sleepMode = value;

            if (!_sleepMode)
            {
                Enable();

#if UNITY_EDITOR
                SceneView.RepaintAll();
#endif
            }
        }

        public bool GetSleepMode()
        {
            if (!enabled) return true;
            return _sleepMode;
        }


        void Start()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
                throw new InvalidOperationException("This should not be active in all sorts of play mode.");
            }
        }







        public Terrain GetTerrain()
        {
            return currentTerrain;
        }

        public void TerrainSettingsChanged()
        {
            if (OnTerrainSettingsChangeDelegate != null) OnTerrainSettingsChangeDelegate();
        }

        private Vector3 _level1CullingCameraPosition;
        [NonSerialized]
        //[SerializeField]
        public List<VegetationCell> VisibleVegetationCellList = new List<VegetationCell>();
        [NonSerialized]
        //[SerializeField]
        public List<VegetationCell> ShadowVegetationCellList = new List<VegetationCell>();
        [NonSerialized]
        //[SerializeField]
        public List<VegetationCell> ProcessVegetationCellList = new List<VegetationCell>();
        [NonSerialized]
        //[SerializeField]
        private readonly List<VegetationCell> _needLoadVegetationList = new List<VegetationCell>();
        [NonSerialized]
        //[SerializeField]
        public List<VegetationItemModelInfo> VegetationModelInfoList = new List<VegetationItemModelInfo>();

        //private MaterialPropertyBlock _vegetationPropertyBlock;
        private CullingGroup _cullingGroup;
        private RandomNumberGenerator _randomNumberGenerator;

        public int ThreadCount = 4;

        private bool _refreshCulling;

        [NonSerialized]
        public QuadTree<VegetationCell> VegetationCellQuadTree;

        private Vector3 _currentTerrainPosition = Vector3.zero;

        public VegetationItemModelInfo GetVegetationModelInfo(int index)
        {
            return VegetationModelInfoList[index];
        }

        public int GetVegetationModelInfoCount()
        {
            return VegetationModelInfoList.Count;
        }




#region Info
        public int GetCacheTreeCount()
        {
            int treeCount = 0;
            for (int i = 0; i <= VegetationCellList.Count - 1; i++)
            {
                treeCount += VegetationCellList[i].TreeCount;
            }

            return treeCount;

        }

        public int GetCacheGrassCount()
        {
            int grassCount = 0;
            for (int i = 0; i <= VegetationCellList.Count - 1; i++)
            {
                grassCount += VegetationCellList[i].GrassCount;
            }

            return grassCount;
        }

        public void RefreshVegetationBillboards(Bounds bounds)
        {
            if (OnRefreshVegetationBillboardsDelegate != null) OnRefreshVegetationBillboardsDelegate(bounds);
        }

        //public void SpawnAllCells(int distanceBand)
        //{
        //    if (Application.isPlaying && UseMultithreading)
        //    {
        //        List<List<VegetationCell>> splitList = SplitCellList(VegetationCellList, ThreadCount);

        //        ManualResetEvent[] resetEvents = new ManualResetEvent[splitList.Count];
        //        int listSize = (VegetationCellList.Count / ThreadCount) + 1;

        //        for (int j = 0; j <= splitList.Count - 1; j++)
        //        {
        //            //int CellCount = splitList[j].Count;
        //            resetEvents[j] = new ManualResetEvent(false);
        //            try
        //            {

        //                int jj = j;
        //                int overrideDistanceBand = distanceBand;
        //                ThreadPool.QueueUserWorkItem(
        //                        (obj) =>
        //                        {
        //                            for (int i = 0; i <= splitList[jj].Count - 1; i++)
        //                            {
        //                                VegetationCellList[listSize * jj + i].LoadVegetation(overrideDistanceBand);
        //                            }
        //                            resetEvents[jj].Set();
        //                        }
        //                        );
        //            }
        //            catch (Exception)
        //            {
        //                resetEvents[j].Set();
        //            }
        //        }

        //        WaitHandle.WaitAll(resetEvents);
        //    }
        //    else
        //    {
        //        for (int i = 0; i <= VegetationCellList.Count - 1; i++)
        //        {
        //            VegetationCellList[i].LoadVegetation(distanceBand);
        //        }
        //    }
        //}

        //public void SpawnCells(int distanceBand, List<VegetationCell> spawnList)
        //{
        //    if (Application.isPlaying && UseMultithreading)
        //    {
        //        List<List<VegetationCell>> splitList = SplitCellList(spawnList, ThreadCount);

        //        ManualResetEvent[] resetEvents = new ManualResetEvent[splitList.Count];
        //        int listSize = (spawnList.Count / ThreadCount) + 1;

        //        for (int j = 0; j <= splitList.Count - 1; j++)
        //        {
        //            //int CellCount = splitList[j].Count;
        //            resetEvents[j] = new ManualResetEvent(false);
        //            try
        //            {

        //                int jj = j;
        //                int overrideDistanceBand = distanceBand;
        //                ThreadPool.QueueUserWorkItem(
        //                    (obj) =>
        //                    {
        //                        for (int i = 0; i <= splitList[jj].Count - 1; i++)
        //                        {
        //                            spawnList[listSize * jj + i].LoadVegetation(overrideDistanceBand);
        //                        }
        //                        resetEvents[jj].Set();
        //                    }
        //                );
        //            }
        //            catch (Exception)
        //            {
        //                resetEvents[j].Set();
        //            }
        //        }

        //        WaitHandle.WaitAll(resetEvents);
        //    }
        //    else
        //    {
        //        for (int i = 0; i <= spawnList.Count - 1; i++)
        //        {
        //            spawnList[i].LoadVegetation(distanceBand);
        //        }
        //    }
        //}

#endregion

#region Set/Get properties 

        /// <summary>
        /// Sets a new terrain to the vegetationsystem and reinitializes the system.
        /// </summary>
        /// <param name="terrain"></param>
        public void SetTerrain(Terrain terrain)
        {
            currentTerrain = terrain;
            TerrainSettingsChanged();
            Enable();
        }

        /// <summary>
        /// Hotswap terrain will change the terrain of a running VegetationSystem component. It will reuse the existing cells and quadtree structure but move it to the new terrain location. Terrain dimentions x,z must be the same. Used for pooling VegetationSystem Components.Exclude sea level must be turned off.
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="unityTerrainData"></param>
        public void HotswapTerrain(Terrain terrain, UnityTerrainData unityTerrainData)
        {
            if (!terrain)
            {
                Debug.LogWarning("missing terrain");
                return;
            }
            if (!InitDone)
            {
                Debug.LogWarning("Terrain needs to be initialized to be hotswapped");
                return;
            }
            if (ExcludeSeaLevelCells)
            {
                Debug.LogWarning("You can not hotswap a terrain with ExcludeSeaLevelCells enabled");
                return;
            }

            if (Math.Abs(terrain.terrainData.size.x - currentTerrain.terrainData.size.x) > 0.1f ||
                Math.Abs(terrain.terrainData.size.z - currentTerrain.terrainData.size.z) > 0.1f) return;

            Vector3 offset = terrain.transform.position - currentTerrain.transform.position;
            currentTerrain = terrain;
            currentTerrain.transform.hasChanged = false;

            UnityTerrainData.OnDestroy();
            UnityTerrainData = unityTerrainData ?? new UnityTerrainData(terrain, true, LoadUnityTerrainDetails);

            VegetationCellQuadTree.Move(new Vector2(offset.x, offset.z));

            for (int i = 0; i <= VegetationCellList.Count - 1; i++)
            {
                VegetationCellList[i].UnityTerrainData = UnityTerrainData;
                VegetationCellList[i].Move(offset);
                VegetationCellList[i].CalculateMinMaxHeight(4f);
            }

            MoveBillboardSystem(offset);
            SetupCullingGroup();
            SetDirty();
        }

        [ContextMenu("SetDirty")]
        public void SetThisDirty()
        {
            SetDirty();
           // Debug.Log(VisibleVegetationCellList.Count);
        }

        public void EnableTouchReact(bool value)
        {
            vegetationSettings.UseTouchReact = value;
            RefreshVegetationModelInfoMaterials();
        }

        public void SetVegetationDistance(float distance)
        {
            vegetationSettings.VegetationDistance = distance;
            if (!InitDone) return;
            SetupCullingGroup(false);
            RefreshVegetationModelInfoMaterials();
            UpdateVegetationDistance();
        }

        public void SetCamera(Camera aCamera)
        {
            SelectedCamera = aCamera;
            _refreshCulling = true;
        }

        public Camera GetCamera()
        {
            return SelectedCamera;
        }

        public float GetVegetationDistance()
        {
            return vegetationSettings.VegetationDistance;
        }

        public float GetTotalDistance()
        {
            return GetVegetationDistance() + GetTreeDistance() + GetBillboardDistance();
        }

        public void SetTreeDistance(float treeDistance)
        {
            vegetationSettings.TreeDistance = treeDistance;
            if (!InitDone) return;

            SetupCullingGroup(false);
            UpdateVegetationDistance();
        }

        void UpdateVegetationDistance()
        {
            //BillboardSystem billboardSystem = GetComponent<BillboardSystem>();
           // if (billboardSystem) billboardSystem.OnVegetationDistanceChange();
        }

        public float GetTreeDistance()
        {
            return vegetationSettings.TreeDistance;
        }

        public void SetBillboardDistance(float billboardDistance)
        {
            vegetationSettings.BillboardDistance = billboardDistance;
            if (!InitDone) return;
            _refreshCulling = true;
            UpdateVegetationDistance();
        }

        public float GetBillboardDistance()
        {
            return vegetationSettings.BillboardDistance;
        }

        public void SetRandomSeed(int seed)
        {
            vegetationSettings.RandomSeed = seed;
            SetVegetationPackage(VegetationPackageIndex, true);
        }

        public void SetWaterLevel(float waterLevel)
        {
            vegetationSettings.WaterLevel = waterLevel;
            if (ExcludeSeaLevelCells)
            {
                SetupVegetationSystem();
            }
            else
            {
                ClearVegetationCellCache();
            }

            TerrainSettingsChanged();
        }

        public float GetWaterLevel()
        {
            return vegetationSettings.WaterLevel;
        }
#endregion

#region Init 
        private bool ExecuteChecklist(bool calledFromSetupVegetationSystem)
        {
            bool hasError = Application.isPlaying && !SelectedCamera;

            if (!currentTerrain) hasError = true;

            if (VegetationPackageList.Count == 0) hasError = true;

            if (VegetationPackageList.Count > VegetationPackageIndex)
            {
                if (VegetationPackageList[VegetationPackageIndex] == null) hasError = true;
            }

            if (!calledFromSetupVegetationSystem && !InitDone) SetupVegetationSystem();

            if (hasError)
            {
                InitDone = false;
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetupVegetationSystem(bool buffers = false, bool details = false)
        {
            
           // Debug.Log("Setup Vegetation System " + transform.name);

            InitDone = false;

            TerrainMaterialOverridden = false;

            Init();

            ConfigureTerrainSettings();

            
            if (!ExecuteChecklist(true)) return;
            
            _randomNumberGenerator = new RandomNumberGenerator(vegetationSettings.RandomSeed);

            if (!Application.isPlaying)
            SetupTerrainData(buffers, details);

            SetVegetationPackage(VegetationPackageIndex, true);
            
            //_vegetationPropertyBlock = new MaterialPropertyBlock();
            RenderVegetation = true;
            InitDone = true;

            //BillboardSystem billboardSystem = GetComponent<BillboardSystem>();
            //if (billboardSystem) billboardSystem.OnVegetationSystemInitComplete();
            
            //GC.Collect();
        }

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            /*
            InitGlobalShaderProperties();
            _windSamplerMaterial = Profile.Load("WindSampler", typeof(Material)) as Material;
            */
            RegisterVegetationSystem();


#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneSaved += OnSceneSaved;

            EditorSceneManager.sceneClosing += CloseScene;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
#endif
#endif
            if (!Application.isPlaying) _sleepMode = true;
            if (AutomaticWakeup)
            {
                _sleepMode = false;
            }

            if (_sleepMode) return;
            Enable();
        }

        [ContextMenu("Enable")]
        public void Enable()
        {
#if UNITY_EDITOR
            SceneViewDetector.OnChangedSceneViewCameraDelegate += OnSceneViewChanged;
#endif
            SetupFrustumComputeShaders();
            EnableEditorApi();
            SetupVegetationSystem();
            RefreshPreset();
        }

#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
        void OnSceneSaving(Scene scene, string path)
        {
            RestoreTerrainMaterial();

            if (!DisableWhenSaving) return;
            //Debug.Log("Before Scene Save");
            Disable();
            GC.Collect();
        }

        void OnSceneSaved(Scene scene)
        {
            if (!DisableWhenSaving) return;
            //Debug.Log("After Scene Saved");
            Enable();
        }

        void CloseScene(Scene scene, bool removeScene)
        {
            Disable();
            //GC.Collect();
        }

#if UNITY_2017_2_OR_NEWER
        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            RestoreTerrainMaterial();
        }
#else
        private void OnPlayModeStateChanged()
        {
            RestoreTerrainMaterial();
        }
#endif
#endif
        void InitGlobalShaderProperties()
        {
            float minVegetationDistance = Mathf.Clamp(vegetationSettings.VegetationDistance, 20,
                vegetationSettings.VegetationDistance - 20);
            Shader.SetGlobalVector("_VSGrassFade", new Vector4(minVegetationDistance, 20, 0, 0));
            Shader.SetGlobalVector("_VSShadowMapFadeScale", new Vector4(QualitySettings.shadowDistance - 30, 20, 1, 1));
        }

        private void ConfigureTerrainSettings()
        {
            if (!currentTerrain) return;

            if (DisableUnityDetails)
            {
                currentTerrain.detailObjectDistance = 0;
                currentTerrain.detailObjectDensity = 0;
            }

            if (DisableUnityTrees)
            {
                currentTerrain.treeDistance = 0;
                currentTerrain.treeBillboardDistance = 0;
            }

            if (SetUnityTerrainPixelError)
            {
                if (currentTerrain.heightmapPixelError < 5) currentTerrain.heightmapPixelError = 5;
            }
            if (DisableUnityDetails && DisableUnityTrees) currentTerrain.drawTreesAndFoliage = false;

            _currentTerrainPosition = currentTerrain.transform.position;

            _currentTerrainCollider = currentTerrain.gameObject.GetComponent<TerrainCollider>();
        }

        public void Init()
        {
            
            if (VegetationPackageList == null) VegetationPackageList = new List<VegetationPackage>();
            ThreadCount = Mathf.Clamp(SystemInfo.processorCount, 1, SystemInfo.processorCount);

            AutoSelectCamera();
            if (AutoselectTerrain) currentTerrain = transform.GetComponent<Terrain>();
            /*
            if (!WindSampler) CreateWindSampler();
            SetupWind();
            SetupHeatMap();
            */
        }

        void CreateWindSampler()
        {
            var windSamplerTransform = transform.Find("WindSampler");

            if (!windSamplerTransform)
            {
                GameObject windSamplerObject = new GameObject("WindSampler");
                windSamplerObject.transform.SetParent(transform, false);
                windSamplerObject.transform.position = Vector3.zero;
                WindSampler = windSamplerObject;
            }
            else
            {
                WindSampler = windSamplerTransform.gameObject;
            }
        }

        void AutoSelectCamera()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (AutoselectCamera) SelectedCamera = GetCurrentCamera();
            }

            else
            {
                if (SelectedCamera == null)
                {
                    Camera[] cameras = FindObjectsOfType<Camera>();
                    for (int i = 0; i <= cameras.Length - 1; i++)
                    {
                        if (cameras[i].tag == "MainCamera")
                        {
                            SelectedCamera = cameras[i];
                            break;
                        }
                    }
                }
            }
#else
            if (SelectedCamera == null)
            {
                Camera[] cameras = FindObjectsOfType<Camera>();
                for (int i = 0; i <= cameras.Length - 1; i++)
                {
                    if (cameras[i].tag == "MainCamera")
                    {
                        SelectedCamera = cameras[i];
                        break;
                    }
                }
            }
#endif
        }

        private void RegisterVegetationSystem()
        {
            if (VegetationCamera.instance != null)
                isCamera = true;

            vsm = transform.parent.GetComponent<VegetationStudioManager>();
            if (vsm != null)
                vsm.Instance_RegisterVegetationSystem(this);
            else
                Debug.LogError("Not find VSManagaer in parent");
        }


        // ReSharper disable once UnusedMember.Local
        void Reset()
        {
            SelectedCamera = Camera.main;
            SetTerrain(Terrain.activeTerrain);
            SelectDirectionalLight();

            //#if UNITY_EDITOR_WIN
            //            UseComputeShaders = false;
            //#else
            //            UseComputeShaders = false;
            //#endif
        }

#endregion

#region VegetationCells


        void SelectDirectionalLight()
        {
            Light[] lights = FindObjectsOfType<Light>();

            for (int i = 0; i <= lights.Length - 1; i++)
            {
                if (lights[i].type == LightType.Directional)
                {
                    SunDirectionalLight = lights[i];
                    break;
                }
            }
        }

        public void RefreshSplatMap(Bounds bounds, bool useBounds)
        {
            if (UnityTerrainData == null) return;
            UnityTerrainData.RefreshSplatMap(bounds, useBounds);
            RefreshVegetationBillboards(useBounds ? bounds : UnityTerrainData.TerrainBounds);
        }

        public void RefreshHeightMap(Bounds bounds, bool onlyVisisble, bool refreshHeightData)
        {
            //Debug.Log("RefreshHeightMap");
            if (GetSleepMode()) return;
            if (!enabled || !InitDone) return;

            RefreshVegetationBillboards(bounds);

            if (refreshHeightData)
            {
                UnityTerrainData.RefreshHeightData(bounds, true);
            }

            UnityTerrainData.RefreshSplatMap(bounds, true);

            if (onlyVisisble)
            {
                for (int i = 0; i <= VisibleVegetationCellList.Count - 1; i++)
                {
                    if (VisibleVegetationCellList[i].CellBounds.Intersects(bounds))
                    {
                        VisibleVegetationCellList[i].ClearCache();
                    }
                }
            }
            else
            {
                for (int i = 0; i <= VegetationCellList.Count - 1; i++)
                {
                    if (VegetationCellList[i].CellBounds.Intersects(bounds))
                    {
                        VegetationCellList[i].ClearCache();
                    }
                }
            }
            SetDirty();
        }
        void ClearVegetationCells()
        {
            for (int i = 0; i <= VegetationCellList.Count - 1; i++)
            {
                VegetationCellList[i].OnDisable();
            }


            VegetationCellList.Clear();
        }
#if UNITY_EDITOR
        [ContextMenu("Create Cells")]

        public void CreateVegetationCells()
        {
            VegetationCellsStorage asset = ScriptableObject.CreateInstance<VegetationCellsStorage>();
            string terPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(UnityTerrainData.terrain.terrainData));
            string terName = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(UnityTerrainData.terrain.terrainData));
            
            AssetDatabase.CreateAsset(asset, terPath + "/" + terName + "_VegetationCells.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            vegCellStorage = asset;
        }

        [ContextMenu("Save Cells")]

        public void SaveVegetationCells()
        {
            vegCellStorage.vegetationCellBaked = new List<VegetationCellBaked>();
            for (int i=0; i< VegetationCellList.Count; i++)
            {
                VegetationCellBaked vegetationCellBake = new VegetationCellBaked();
                vegetationCellBake.CellBounds = VegetationCellList[i].CellBounds;
                vegetationCellBake.CellCorner = VegetationCellList[i].CellCorner;
                vegetationCellBake.CellSize = VegetationCellList[i].CellSize;
                vegetationCellBake.EdgeCell = VegetationCellList[i].EdgeCell;
                vegetationCellBake.RandomIndexOffset = VegetationCellList[i].RandomIndexOffset;
                vegetationCellBake.Rectangle = VegetationCellList[i].Rectangle;
                vegetationCellBake.SeaCell = VegetationCellList[i].SeaCell;
                vegCellStorage.vegetationCellBaked.Add(vegetationCellBake);
            }

            EditorUtility.SetDirty(vegCellStorage);
        }

#endif

        public void LoadVegetationCells(ref VegetationCell vegCell, int index)
        {
            vegCell.CellBounds = vegCellStorage.vegetationCellBaked[index].CellBounds;
            vegCell.CellCorner = vegCellStorage.vegetationCellBaked[index].CellCorner;
            vegCell.CellSize = vegCellStorage.vegetationCellBaked[index].CellSize;
            vegCell.EdgeCell = vegCellStorage.vegetationCellBaked[index].EdgeCell;
            vegCell.RandomIndexOffset = vegCellStorage.vegetationCellBaked[index].RandomIndexOffset;
            vegCell.Rectangle = vegCellStorage.vegetationCellBaked[index].Rectangle;
            vegCell.SeaCell = vegCellStorage.vegetationCellBaked[index].SeaCell;
        }

        [ContextMenu("Setup Cells")]
        public void SetupVegetationCells()
        {
            UnityEngine.Random.InitState(vegetationSettings.RandomSeed);
            CellSize = OverrideCellSize ? CustomCellSize : Mathf.Clamp(currentTerrain.terrainData.size.x / 100f, 20, 30);
            CellSize = Mathf.RoundToInt(CellSize);

            _potentialVisibleCellBufferRange = (CellSize * 3) + UnityEngine.Random.Range(0f, 9f);
            if (_potentialVisibleCellBufferRange < 100) _potentialVisibleCellBufferRange = 100;

            ClearVegetationCells();

            VegetationCellQuadTree = new QuadTree<VegetationCell>(UnityTerrainData.GetTerrainRect(CellSize));
           //Debug.Log(UnityTerrainData.terrainPosition + " " + UnityTerrainData.size);

            int cellXCount = Mathf.CeilToInt(UnityTerrainData.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(UnityTerrainData.size.z / CellSize);
 #if PERSISTENT_VEGETATION
            PersistentVegetationStorage persistentVegetationStorage = GetComponent<PersistentVegetationStorage>();
            bool hasPersistentStorage = false;
            int totalCellCount = cellXCount * cellZCount;

            if (persistentVegetationStorage.AutoInitPersistentVegetationStoragePackage)
            {
                persistentVegetationStorage.InitializePersistentStorage(totalCellCount);
                persistentVegetationStorage.AutoInitPersistentVegetationStoragePackage = false;
            }

            if (persistentVegetationStorage)
            {
                hasPersistentStorage = persistentVegetationStorage.HasValidPersistentStorage(totalCellCount);

            }
#endif
            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    VegetationCell vegetationCell =
                        new VegetationCell
                        {
                            RandomIndexOffset = UnityEngine.Random.Range(0, 200),
                            CellCorner = new Vector3(UnityTerrainData.terrainPosition.x + (CellSize * x),
                                UnityTerrainData.terrainPosition.y,
                                UnityTerrainData.terrainPosition.z + (CellSize * z)),
                            CellSize = new Vector3(CellSize, 0, CellSize),
                            //FrustumKernelHandle = FrustumKernelHandle,
                            //FrusumMatrixShader = FrusumMatrixShader 
                        };
                    vegetationCell.Setup(vegetationSettings, currentVegetationPackage, UnityTerrainData, currentVegetationPlacingData);
                    vegetationCell.RuntimeTextureMaskList = RuntimeTextureMaskList;
#if PERSISTENT_VEGETATION
                    if (hasPersistentStorage && !persistentVegetationStorage.DisablePersistentStorage)
                    {
                        vegetationCell.PersistentVegetationCell =
                            persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList[
                                VegetationCellList.Count];
                    }
#endif

                    vegetationCell.CellIndex = VegetationCellList.Count;
                    vegetationCell.RandomNumberGenerator = _randomNumberGenerator;
                    //vegetationCell.OnClearCellCacheDelegate += OnvegetationCellClearCache;

                    vegetationCell.IsPlaying = Application.isPlaying;

                    if (x == 0 || z == 0 || x == (cellXCount - 1) || z == (cellZCount - 1))
                    {
                        vegetationCell.EdgeCell = true;
                    }


                    bool seaCell = false;
                    vegetationCell.Init();

                    if (ExcludeSeaLevelCells)
                    {

                        if (vegetationCell.CellBounds.center.y + vegetationCell.CellBounds.extents.y <=
                            GetWaterLevel() + UnityTerrainData.terrainPosition.y)
                        {
                            seaCell = true;
                            vegetationCell.SeaCell = true;
                        }
                    }

                    
                    VegetationCellList.Add(vegetationCell);

                    if (!seaCell)
                    {
                        VegetationCellQuadTree.Insert(vegetationCell);
                    }
                }
            }
#if UNITY_EDITOR
            if (vegCellStorage == null)
                CreateVegetationCells();

            SaveVegetationCells();
#endif
            if (OnVegetationCellRefreshDelegate != null) OnVegetationCellRefreshDelegate(this);
        }

        public void SetupVegetationCellsBaked()
        {

            VegetationCellQuadTree = new QuadTree<VegetationCell>(UnityTerrainData.GetTerrainRect(CellSize));
            //Debug.Log(UnityTerrainData.terrainPosition + " " + UnityTerrainData.size);

            int cellXCount = Mathf.CeilToInt(UnityTerrainData.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(UnityTerrainData.size.z / CellSize);
            _potentialVisibleCellBufferRange = (CellSize * 3) + UnityEngine.Random.Range(0f, 9f);
            if (_potentialVisibleCellBufferRange < 100) _potentialVisibleCellBufferRange = 100;

            ClearVegetationCells();
#if PERSISTENT_VEGETATION
            PersistentVegetationStorage persistentVegetationStorage = GetComponent<PersistentVegetationStorage>();
            bool hasPersistentStorage = false;
            int totalCellCount = cellXCount * cellZCount;
            
            if (persistentVegetationStorage.AutoInitPersistentVegetationStoragePackage)
            {
                persistentVegetationStorage.InitializePersistentStorage(totalCellCount);
                persistentVegetationStorage.AutoInitPersistentVegetationStoragePackage = false;
            }
            
            if (persistentVegetationStorage)
            {
                hasPersistentStorage = persistentVegetationStorage.HasValidPersistentStorage(totalCellCount);

            }
#endif
            int ch = 0;
            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    VegetationCell vegetationCell =
                        new VegetationCell
                        {
                            RandomIndexOffset = UnityEngine.Random.Range(0, 200),
                            CellCorner = new Vector3(UnityTerrainData.terrainPosition.x + (CellSize * x),
                                UnityTerrainData.terrainPosition.y,
                                UnityTerrainData.terrainPosition.z + (CellSize * z)),
                            CellSize = new Vector3(CellSize, 0, CellSize),
                            //FrustumKernelHandle = FrustumKernelHandle,
                            //FrusumMatrixShader = FrusumMatrixShader 
                        };

                    LoadVegetationCells(ref vegetationCell, ch);
                    ch++;

                    vegetationCell.SetupBaked(vegetationSettings, currentVegetationPackage, UnityTerrainData, currentVegetationPlacingData);
                    vegetationCell.RuntimeTextureMaskList = RuntimeTextureMaskList;
#if PERSISTENT_VEGETATION
                    if (hasPersistentStorage && !persistentVegetationStorage.DisablePersistentStorage)
                    {
                        vegetationCell.PersistentVegetationCell =
                            persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList[
                                VegetationCellList.Count];
                    }
#endif

                    vegetationCell.CellIndex = VegetationCellList.Count;
                    vegetationCell.RandomNumberGenerator = _randomNumberGenerator;
                    vegetationCell.IsPlaying = Application.isPlaying;

                    VegetationCellList.Add(vegetationCell);

                    if (!vegetationCell.SeaCell)
                    {
                        VegetationCellQuadTree.Insert(vegetationCell);
                    }
                }
            }

            if (OnVegetationCellRefreshDelegate != null) OnVegetationCellRefreshDelegate(this);
        }

        public void RefreshCulling()
        {
            _refreshCulling = true;
        }


        public void SetDirty()
        {
            IsDirty = true;
        }

        public void RefreshVegetationModelInfoMaterials()
        {
            for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
            {
                VegetationModelInfoList[i].RefreshMaterials();
            }
        }

              public void GetModelData(int index)
        {
            //Debug.Log("GetModelData");
            if (currentVegetationPackage.VegetationInfoList[index].VegetationPrefab)
            {
                Bounds vegetationBounds = MeshUtils.CalculateBoundsInstantiate(currentVegetationPackage
                    .VegetationInfoList[index]
                    .VegetationPrefab);

                currentVegetationPackage.VegetationInfoList[index].Bounds = vegetationBounds;
                currentVegetationPackage.VegetationInfoList[index].Volume =
                    vegetationBounds.size.x * vegetationBounds.size.y * vegetationBounds.size.z;

                GameObject tempVegetationModel =
                    Instantiate(currentVegetationPackage.VegetationInfoList[index].VegetationPrefab);
                tempVegetationModel.hideFlags = HideFlags.DontSave; // | HideFlags.HideInHierarchy;
                tempVegetationModel.name = "VegetationSystemRenderer_" + index;
                //tempVegetationModel.transform.SetParent(WindSampler.transform);
                tempVegetationModel.transform.localPosition = new Vector3(0, 0, 3);
                tempVegetationModel.transform.localRotation = Quaternion.identity;

                //GameObject selectedVegetationModelLOD0 = MeshUtils.SelectMeshObject(tempVegetationModel, MeshType.Normal, 0);
                currentVegetationPackage.VegetationInfoList[index].sourceMesh = VegetationItemModelInfo.GetVegetationMesh(tempVegetationModel, 0);

                MeshRenderer VegetationRendererLOD0 = tempVegetationModel.GetComponentInChildren<MeshRenderer>();
                currentVegetationPackage.VegetationInfoList[index].sourceMaterials = VegetationRendererLOD0.sharedMaterials;

               // currentVegetationPackage.VegetationInfoList[index].materialPropertyBlock = new MaterialPropertyBlock();
                //VegetationRendererLOD0.GetPropertyBlock(currentVegetationPackage.VegetationInfoList[index].materialPropertyBlock);
               // if (currentVegetationPackage.VegetationInfoList[index].materialPropertyBlock == null) currentVegetationPackage.VegetationInfoList[index].materialPropertyBlock = new MaterialPropertyBlock();

                DestroyImmediate(tempVegetationModel);
            }
        }

        [ContextMenu("SetupVegetationPrefabs")]
        public void SetupVegetationPrefabs()
        {
           // Debug.Log("SetupVegetationPrefabs");
            ClearVegetationModelInfoList();

            for (int i = 0; i <= currentVegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                if (currentVegetationPackage.VegetationInfoList[i].IncludeDetailLayer < -1) continue;

                VegetationItemModelInfo newVegetationItemModelInfo =
                        new VegetationItemModelInfo
                        {
                            MatrixListPool = new MatrixListPool(1, 1000),
                            FloatListPool = new ListPool<float>(1, 1000),
                            VegetationItemInfo = currentVegetationPackage.VegetationInfoList[i],
                            VegetationRenderType = currentVegetationPackage.VegetationInfoList[i].VegetationRenderType,
                            VegetationSettings = vegetationSettings,
                            VegetationSystem = this,
                        };

                newVegetationItemModelInfo.AddVegetationModel(0);

                VegetationModelInfoList.Add(newVegetationItemModelInfo);


                
                
                
            }

            float maxVegetationItemHeight = 0;

            for (int i = 0; i <= currentVegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                if (currentVegetationPackage.VegetationInfoList[i].IncludeDetailLayer < -1) continue;
                float maxHeight = currentVegetationPackage.VegetationInfoList[i].Bounds.size.y *
                                  currentVegetationPackage.VegetationInfoList[i].MaxScale;
                if (maxHeight > maxVegetationItemHeight) maxVegetationItemHeight = maxHeight;
            }

            currentVegetationPackage.MaxVegetationItemHeight = maxVegetationItemHeight;
        }

        void CleanVegetationObject(GameObject go)
        {
            Mesh emptyMesh = new Mesh { bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(2, 2, 2)) };

            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i <= meshFilters.Length - 1; i++)
            {
                meshFilters[i].sharedMesh = emptyMesh;
            }

            Rigidbody[] rigidbodies = go.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i <= rigidbodies.Length - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(rigidbodies[i]);
                }
                else
                {
                    DestroyImmediate(rigidbodies[i]);
                }
            }

            Collider[] colliders = go.GetComponentsInChildren<Collider>();
            for (int i = 0; i <= colliders.Length - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(colliders[i]);
                }
                else
                {
                    DestroyImmediate(colliders[i]);
                }
            }

            BillboardAtlasRenderer[] billboardAtlasRenderers = go.GetComponentsInChildren<BillboardAtlasRenderer>();
            for (int i = 0; i <= billboardAtlasRenderers.Length - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(billboardAtlasRenderers[i]);
                }
                else
                {
                    DestroyImmediate(billboardAtlasRenderers[i]);
                }
            }

            NavMeshObstacle[] navMeshObstacles = go.GetComponentsInChildren<NavMeshObstacle>();
            for (int i = 0; i <= navMeshObstacles.Length - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(navMeshObstacles[i]);
                }
                else
                {
                    DestroyImmediate(navMeshObstacles[i]);
                }
            }

            Transform[] transforms = go.GetComponentsInChildren<Transform>();
            for (int i = 0; i <= transforms.Length - 1; i++)
            {
                if (transforms[i].name.Contains("Billboard"))
                {
                    if (Application.isPlaying)
                    {
                        Destroy(transforms[i].gameObject);
                    }
                    else
                    {
                        DestroyImmediate(transforms[i].gameObject);
                    }
                }
            }

            transforms = go.GetComponentsInChildren<Transform>();
            for (int i = 0; i <= transforms.Length - 1; i++)
            {
                if (transforms[i].name.Contains("CollisionObject"))
                {
                    if (Application.isPlaying)
                    {
                        Destroy(transforms[i].gameObject);
                    }
                    else
                    {
                        DestroyImmediate(transforms[i].gameObject);
                    }
                }
            }

            LODGroup[] lodgroups = go.GetComponentsInChildren<LODGroup>();
            for (int i = 0; i <= lodgroups.Length - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(lodgroups[i]);
                }
                else
                {
                    DestroyImmediate(lodgroups[i]);
                }
            }

            MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i <= meshRenderers.Length - 1; i++)
            {
                meshRenderers[i].shadowCastingMode = ShadowCastingMode.Off;
                meshRenderers[i].receiveShadows = false;
                meshRenderers[i].lightProbeUsage = LightProbeUsage.Off;

                if (meshRenderers[i].sharedMaterials.Length > 1)
                {
                    Material[] materials = new Material[1];
                    materials[0] = _windSamplerMaterial;//meshRenderers[i].sharedMaterials[0]; //_windSamplerMaterial;//
                    meshRenderers[i].sharedMaterials = materials;
                }
            }
        }




        static void PrepareVegetationSplitList(List<List<Matrix4x4>> outputList, MatrixListPool matrixListPool)
        {
            for (int i = 0; i <= outputList.Count - 1; i++)
            {
                matrixListPool.ReturnList(outputList[i]);
            }
            outputList.Clear();
        }

        void InitLODVegetationInstanceInfo(LODVegetationInstanceInfo lodVegetationInstanceInfo,
            MatrixListPool matrixListPool, ListPool<float> fadeListPool)
        {
            lodVegetationInstanceInfo.LOD0InstanceList = matrixListPool.GetList();
            //lodVegetationInstanceInfo.LOD1InstanceList = matrixListPool.GetList();
            //lodVegetationInstanceInfo.LOD2InstanceList = matrixListPool.GetList();
            lodVegetationInstanceInfo.LOD0ShadowInstanceList = matrixListPool.GetList();
            //lodVegetationInstanceInfo.LOD1ShadowInstanceList = matrixListPool.GetList();
            //lodVegetationInstanceInfo.LOD2ShadowInstanceList = matrixListPool.GetList();

            lodVegetationInstanceInfo.LOD0FadeList = fadeListPool.GetList();
            //lodVegetationInstanceInfo.LOD1FadeList = fadeListPool.GetList();
            //lodVegetationInstanceInfo.LOD2FadeList = fadeListPool.GetList();
            lodVegetationInstanceInfo.LOD0ShadowFadeList = fadeListPool.GetList();
           // lodVegetationInstanceInfo.LOD1ShadowFadeList = fadeListPool.GetList();
           // lodVegetationInstanceInfo.LOD2ShadowFadeList = fadeListPool.GetList();
        }

        void ClearVegetationLODSplitList(List<LODVegetationInstanceInfo> outputList, MatrixListPool matrixListPool, ListPool<float> fadeListPool)
        {
            for (int i = 0; i <= outputList.Count - 1; i++)
            {
                matrixListPool.ReturnList(outputList[i].LOD0InstanceList);
               // matrixListPool.ReturnList(outputList[i].LOD1InstanceList);
               // matrixListPool.ReturnList(outputList[i].LOD2InstanceList);
                matrixListPool.ReturnList(outputList[i].LOD0ShadowInstanceList);
                //matrixListPool.ReturnList(outputList[i].LOD1ShadowInstanceList);
               // matrixListPool.ReturnList(outputList[i].LOD2ShadowInstanceList);

                fadeListPool.ReturnList(outputList[i].LOD0FadeList);
               // fadeListPool.ReturnList(outputList[i].LOD1FadeList);
               // fadeListPool.ReturnList(outputList[i].LOD2FadeList);
                fadeListPool.ReturnList(outputList[i].LOD0ShadowFadeList);
               // fadeListPool.ReturnList(outputList[i].LOD1ShadowFadeList);
               // fadeListPool.ReturnList(outputList[i].LOD2ShadowFadeList);

                _LODVegetationInstanceInfoListPool.Release(outputList[i]);
            }
            outputList.Clear();
        }

        static List<List<VegetationCell>> SplitCellList(List<VegetationCell> cellList, int threadCount)
        {
            int nSize = (cellList.Count / threadCount) + 1;
            var list = new List<List<VegetationCell>>();

            for (int i = 0; i < cellList.Count; i += nSize)
            {
                list.Add(cellList.GetRange(i, Mathf.Min(nSize, cellList.Count - i)));
            }
            return list;
        }
#endregion

#region VegetationPackages

        public void RefreshVegetationPackage(bool isSyncCallback = false)
        {
            //Debug.Log("Refresh");
            if (!InitDone) return;

            SetVegetationPackage(VegetationPackageIndex, true);

            if (!isSyncCallback)
            {
                VegetationStudioManager.VegetationPackageSync_RefreshVegetationPackage(this);
            }

            RefreshPreset();

        }
#if UNITY_EDITOR
        [MenuItem("Level Design/Save All Vegetation Cells")]
        public static void SaveAllVegetationCells()
        {
            VegetationSystem[] vs = FindObjectsOfType<VegetationSystem>();
            for (int i=0; i<vs.Length; i++)
            {
                vs[i].Enable();
                vs[i].CreateVegetationCells();
                vs[i].SaveVegetationCells(); 
            }
        }
#endif
        public void AddVegetationPackage(VegetationPackage vegetationPackage, bool enableVegetationPackage)
        {
            VegetationPackageList.Add(vegetationPackage);
            if (enableVegetationPackage)
            {
                VegetationPackageIndex = VegetationPackageList.Count - 1;
            }
        }

        public void CheckData(string name, float dist)
        {
            double one = Profiler.GetTotalAllocatedMemoryLong() * 0.001 * 0.001 * 0.001;
            double two = Profiler.GetTotalReservedMemoryLong() * 0.001 * 0.001 * 0.001;
            double three = Profiler.GetTotalUnusedReservedMemoryLong() * 0.001 * 0.001 * 0.001;
            double four = Profiler.GetAllocatedMemoryForGraphicsDriver() * 0.001 * 0.001 * 0.001;
            double five = Profiler.GetMonoUsedSizeLong() * 0.001 * 0.001 * 0.001;
            double six = Profiler.GetMonoHeapSizeLong() * 0.001 * 0.001 * 0.001;
            double seven = Profiler.GetRuntimeMemorySizeLong(this.gameObject) * 0.001 * 0.001 * 0.001;
            double full = one + two + three;
            string data = transform.name + "	" + name + "	Full: " + full.ToString() + "	Alloc: " + one.ToString() + "	Reserved: " + two.ToString() + "	Unused: " + three.ToString() + "	Runtime: " + seven.ToString() + "	GFX: " + four.ToString() + "	MonoUsed " + five.ToString() + "	MonoHeap " + six.ToString() + "\n";
            str += data;
            //EditorUtility.DisplayProgressBar(data, name, dist);
        }
        public void SetVegetationPackage(int index, bool resetCullingGroup)
        {
            //CheckData("Start InitPackage", 0);

            VegetationPackageIndex = index;
            currentVegetationPackage = VegetationPackageList[index];
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                currentVegetationPackage.InitPackage();
            }
#endif

            //if (VegetationModelInfoList == null || VegetationModelInfoList.Count == 0)
            SetupVegetationPrefabs();
            //else
            //    UpdateVegetationPrefabs();


            //CheckData("Setup Terrain Textures", 0.2f);
            //_vegetationCellCacheController = new VegetationCellCacheController(CurrentVegetationPackage);

            if (!Application.isPlaying)
            {
                SetupTerrainTextures();
                RefreshTextureMasks();
            }
            else
            {
                RuntimeTextureMaskList.Clear();
            }
            //CheckData("Refresh Masks", 0.3f);

            /*
            */
            if (resetCullingGroup)
            {
                //CheckData("Culling Cells", 0.5f);
                if (!Application.isPlaying)
                    SetupVegetationCells();
                else
                    SetupVegetationCellsBaked();
                SelectedCamera = GetCurrentCamera();
                SetupCullingGroup();

            }
            else
            {
                //CheckData("Culling Cells", 0.5f);
                UpdateVegetationCells();
               // CheckData("Culling Group", 0.7f);
                ClearVegetationCellCache();
            }
            
            //CheckData("Delegate", 0.9f);
            //SetDirty();

            /*
            if (InitDone)
            {
                if (OnSetVegetationPackageDelegate != null) OnSetVegetationPackageDelegate(currentVegetationPackage);
            }
            */
            //CheckData("Final", 1.0f);

            //Resources.UnloadUnusedAssets();
            //EditorUtility.ClearProgressBar();

        }
#endregion
#region Terrain
        [ContextMenu("SetTerrainData")]
        void SetupTerrains()
        {
            SetupTerrainData(false, false);
        }

        [ContextMenu("SetTerrainData02")]
        void SetupTerrains02()
        {
            SetupTerrainData(true, true);
        }

        public void SetupTerrainData(bool buffers, bool details)
        {
            if (!currentTerrain) return;

            if (UnityTerrainData != null)
            {
                UnityTerrainData.OnDestroy();
                UnityTerrainData = null;
            }

            UnityTerrainData = new UnityTerrainData(currentTerrain, buffers, details);

            if (currentTerrain)
            {
                currentTerrain.transform.hasChanged = false;
            }
        }
#endregion
#region TerrainTextures
        public void SetupTerrainTextures(bool forceUpdate = false)
        {
//            if (!gameObject.activeSelf) return;

//            if (currentVegetationPackage.UseTerrainTextures || forceUpdate)
//            {
//                bool needChange = false;
//                SplatPrototype[] textureList = new SplatPrototype[currentVegetationPackage.TerrainTextureList.Count];
//                for (int i = 0; i <= currentVegetationPackage.TerrainTextureList.Count - 1; i++)
//                {
//                    if (currentTerrain.terrainData.splatPrototypes.Length > i)
//                    {
//                        if (currentTerrain.terrainData.splatPrototypes[i].texture != currentVegetationPackage.TerrainTextureList[i].Texture ||
//                        currentTerrain.terrainData.splatPrototypes[i].tileOffset != currentVegetationPackage.TerrainTextureList[i].offset ||
//                        currentTerrain.terrainData.splatPrototypes[i].tileSize != currentVegetationPackage.TerrainTextureList[i].TileSize ||
//                        currentTerrain.terrainData.splatPrototypes[i].normalMap != currentVegetationPackage.TerrainTextureList[i].TextureNormals) needChange = true;
//                    }
//                    else
//                    {
//                        needChange = true;
//                    }
                    
//                    textureList[i] = new SplatPrototype
//                    {
//                        texture = currentVegetationPackage.TerrainTextureList[i].Texture
//                    };
//                    if (currentVegetationPackage.TerrainTextureList[i].TextureNormals)
//                    {
//                        textureList[i].normalMap = currentVegetationPackage.TerrainTextureList[i].TextureNormals;
//                    }

//#if UNITY_EDITOR
//                    AssetUtility.SetTextureReadable(textureList[i].texture);
//#endif
//                    textureList[i].tileOffset = currentVegetationPackage.TerrainTextureList[i].offset;
//                    textureList[i].tileSize = currentVegetationPackage.TerrainTextureList[i].TileSize;
//                    if (textureList[i].texture)
//                    {
//                        textureList[i].texture.Apply(true);
//                    }
//                }
//                if (needChange) currentTerrain.terrainData.splatPrototypes = textureList;
//            }
        }
#endregion
#region CullingSystem

        

        public void SetupCullingGroup(bool clearCellCache = true, bool clearVisibleList = true)
        {
           // Debug.Log("Setup");
            /*
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (SceneViewDetector.GetCurrentSceneViewCamera() == null) return;
            }
#endif
*/
            Camera camera = GetCurrentCamera();

            SetupCullingGroupCam(camera, clearCellCache, clearVisibleList);
        }

        public void SetupCullingGroupCam(Camera camera, bool clearCellCache = true, bool clearVisibleList = true)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (SceneViewDetector.GetCurrentSceneViewCamera() == null) return;
            }
#endif

            if (_cullingGroup != null)
            {
                _cullingGroup.Dispose();
                _cullingGroup = null;
            }

            if (clearVisibleList)
            {
                for (int i = 0; i <= VegetationCellList.Count - 1; i++)
                {
                    VegetationCellList[i].SetVisible(false);

                    if (clearCellCache)
                    {
                        VegetationCellList[i].DataLoadLevel = 5;
                    }
                }
                VisibleVegetationCellList.Clear();
            }

            ShadowVegetationCellList.Clear();

            if (OnResetVisibleCellListDelegate != null) OnResetVisibleCellListDelegate();

            _cullingGroup = new CullingGroup();


            _cullingGroup.targetCamera = camera;
            Vector3 pos = (_cullingGroup.targetCamera != null) ? _cullingGroup.targetCamera.transform.position : ((Application.isPlaying) ? Camera.main.transform.position : Camera.current.transform.position);

            if (Application.isPlaying)
            {
                _cullingGroup.targetCamera = SelectedCamera;
                _cullingGroup.SetDistanceReferencePoint(SelectedCamera.transform);
            }
            else
            {
#if UNITY_EDITOR
                _cullingGroup.targetCamera = SceneViewDetector.GetCurrentSceneViewCamera();
                _cullingGroup.SetDistanceReferencePoint(_cullingGroup.targetCamera.transform);
#endif
            }
            //_cullingGroup.SetDistanceReferencePoint(pos);

            Rect cullingAreaRect = GetCullingAreaRect((vegetationSettings.VegetationDistance + vegetationSettings.TreeDistance));
            _level1CullingCameraPosition = GetCameraPosition();

            //Debug.Log(camera.name + " " + _level1CullingCameraPosition + " " + transform.name);
            PotentialVisibleVegetationCellList = VegetationCellQuadTree.Query(cullingAreaRect);

            BoundingSphere[] spheres = new BoundingSphere[PotentialVisibleVegetationCellList.Count];
            for (int i = 0; i <= PotentialVisibleVegetationCellList.Count - 1; i++)
            {
                //PotentialVisibleVegetationCellList[i].CellBounds = VegetationCellList[i].CellBounds;
                spheres[i] = PotentialVisibleVegetationCellList[i].GetBoundingSphere(currentVegetationPackage.MaxVegetationItemHeight + 5f);
            }
            _cullingGroup.SetBoundingSpheres(spheres);
            _cullingGroup.SetBoundingSphereCount(PotentialVisibleVegetationCellList.Count);
            _cullingGroup.onStateChanged = VisibilityStateChangedMethod;

            float[] distanceBands = new float[4];
            float cellDistance = +Mathf.Sqrt(CellSize * CellSize * 2f) / 2f + CellSize;

            distanceBands[0] = (vegetationSettings.VegetationDistance * 0.0f) + cellDistance;
            distanceBands[1] = (vegetationSettings.VegetationDistance * 0.2f) + cellDistance;
            distanceBands[2] = vegetationSettings.VegetationDistance + cellDistance;
            distanceBands[3] = (vegetationSettings.VegetationDistance + vegetationSettings.TreeDistance) + cellDistance;

            vegetationSettings.DistanceLod0 = distanceBands[0];
            vegetationSettings.DistanceLod1 = distanceBands[1];
            vegetationSettings.DistanceLod2 = distanceBands[2];
            vegetationSettings.DistanceLod3 = distanceBands[3];
            vegetationSettings.DistanceBillboards = (vegetationSettings.VegetationDistance +
                                                     vegetationSettings.TreeDistance +
                                                     vegetationSettings.BillboardDistance);

            _cullingGroup.SetBoundingDistances(distanceBands);

        }

#if UNITY_EDITOR
        void OnSceneViewChanged(Camera sceneViewCamera)
        {
            if (_sleepMode) return;
            //Debug.Log(transform.name);

            if (!Application.isPlaying)
            {
                isRenderWait = true;
                SetDirty();
            }
            //isRenderWait = true;
        }
#endif


        public Rect GetCullingAreaRect(float treeDistance)
        {
            float distanceBuffer = CellSize * 4;
            if (distanceBuffer < 110) distanceBuffer = 110;
            float rectExtent = treeDistance + distanceBuffer;
            Vector3 currentCameraPosition = GetCameraPosition();
            return new Rect(currentCameraPosition.x - rectExtent, currentCameraPosition.z - rectExtent, rectExtent * 2, rectExtent * 2);
        }


        public Camera GetCurrentCamera()
        {
            if (Application.isPlaying)
            {
                if (SelectedCamera == null)
					SelectedCamera = Camera.main;

				return SelectedCamera;
            }
#if UNITY_EDITOR
            else
            {               
                if (SelectedCamera)
                    return SelectedCamera;

                SelectedCamera = SceneViewDetector.GetCurrentSceneViewCamera();
                if (SelectedCamera)
                    return SelectedCamera;
                SelectedCamera = Camera.current;
                  return SelectedCamera;

            }
#else
            return SelectedCamera;
#endif
        }

        public Vector3 GetCameraPosition()
        {
            if (GetCurrentCamera() != null)
            {
                return GetCurrentCamera().transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }


        private void VisibilityStateChangedMethod(CullingGroupEvent evt)
        {
            if (evt.currentDistance != evt.previousDistance)
            {
                PotentialVisibleVegetationCellList[evt.index].DistanceBand = evt.currentDistance;
                SetDirty();
                if (OnVegetationCellChangeDistanceBandDelegate != null) OnVegetationCellChangeDistanceBandDelegate(PotentialVisibleVegetationCellList[evt.index], evt.currentDistance, evt.previousDistance);
            }

            if (evt.hasBecomeVisible)
            {
                PotentialVisibleVegetationCellList[evt.index].SetVisible(true);
                PotentialVisibleVegetationCellList[evt.index].DistanceBand = evt.currentDistance;
                VisibleVegetationCellList.Add(PotentialVisibleVegetationCellList[evt.index]);
                SetDirty();
                if (OnVegetationCellVisibleDelegate != null) OnVegetationCellVisibleDelegate(PotentialVisibleVegetationCellList[evt.index], evt.currentDistance);
            }

            if (evt.hasBecomeInvisible)
            {
                PotentialVisibleVegetationCellList[evt.index].SetVisible(false);
                PotentialVisibleVegetationCellList[evt.index].DistanceBand = evt.currentDistance;
                VisibleVegetationCellList.Remove(PotentialVisibleVegetationCellList[evt.index]);
                SetDirty();
                if (OnVegetationCellInvisibleDelegate != null) OnVegetationCellInvisibleDelegate(PotentialVisibleVegetationCellList[evt.index], evt.currentDistance);
            }

            if (evt.hasBecomeVisible)
            {
                ShadowVegetationCellList.Remove(PotentialVisibleVegetationCellList[evt.index]);
            }

            if (evt.hasBecomeInvisible)
            {
                if (evt.currentDistance < (int)vegetationSettings.ShadowCullinMode)
                {
                    ShadowVegetationCellList.Add(PotentialVisibleVegetationCellList[evt.index]);
                }
                else
                {
                    ShadowVegetationCellList.Remove(PotentialVisibleVegetationCellList[evt.index]);
                }
            }

            if (evt.currentDistance != evt.previousDistance && !evt.isVisible && evt.currentDistance >= (int)vegetationSettings.ShadowCullinMode)
            {
                ShadowVegetationCellList.Remove(PotentialVisibleVegetationCellList[evt.index]);
            }
        }
#endregion
#region Disable
        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {

#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
            EditorSceneManager.sceneClosing -= CloseScene;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
#endif
#endif
            Disable();

        }

        void Disable()
        {
#if UNITY_EDITOR
            SceneViewDetector.OnChangedSceneViewCameraDelegate -= OnSceneViewChanged;
#endif
            DisableEditorApi();
            //Debug.Log("Disable");
            ClearVegetationCells();
            VisibleVegetationCellList.Clear();
            ShadowVegetationCellList.Clear();
            PotentialVisibleVegetationCellList.Clear();

            /*
            if (UnityTerrainData != null)
            {
                UnityTerrainData.OnDestroy();
                UnityTerrainData = null;
            }
            */

            ClearVegetationModelInfoList();

            if (_cullingGroup != null)
            {
                _cullingGroup.Dispose();
                _cullingGroup = null;

            }

        }

        void ClearVegetationModelInfoList()
        {
            for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
            {
                VegetationModelInfoList[i].MatrixListPool.Clear();
                VegetationModelInfoList[i].FloatListPool.Clear();
                VegetationModelInfoList[i].DestroyComputeBuffers();
                VegetationModelInfoList[i].SplitVegetationInstanceList.Clear();
                VegetationModelInfoList[i].SplitVegetationGrowthList.Clear();
                VegetationModelInfoList[i].CulledVegetationDistanceList.Clear();
                VegetationModelInfoList[i].CulledVegetationInstanceList.Clear();
                VegetationModelInfoList[i].LODVegetationInstanceList.Clear();
            }
            VegetationModelInfoList.Clear();
        }

        void OnDestroy()
        {
            if (_cullingGroup != null)
            {
                _cullingGroup.Dispose();
                _cullingGroup = null;
            }

            ClearVegetationModelInfoList();

        }

#endregion
#region RenderLoop

#if PERSISTENT_VEGETATION

        // ReSharper disable once UnusedMember.Local
        void LateUpdate()
        {
            if (_sleepMode) return;
            if (!InitDone) return;
            /*
            if (!Application.isPlaying)
            {
                
                //if (isRenderWait)
                //{
                //    SetupCullingGroupCam(SelectedCamera, false);
                //
                //    isRenderWait = false;
                //}
                
            }
            else
            {
                */
                if (Application.isPlaying)
                    if (!isCamera || !VegetationCamera.isDrawGrass) return;

                if (Vector3.Distance(_level1CullingCameraPosition, GetCameraPosition()) > _potentialVisibleCellBufferRange)
                    SetupCullingGroup(false);

                if (_refreshCulling)
                {
                    _refreshCulling = false;
                    SetupCullingGroup();
                }
            //}
        }
        /*
#if !UNITY_2018_1_OR_NEWER
        // ReSharper disable once UnusedMember.Local
        void OnRenderObject()
        {
            
            if (_sleepMode) return;

            if (!Application.isPlaying)
            {
                if (!ExecuteChecklist(false)) return;

                //if (isRenderWait)
                //{
                    //SetBillboardShaderVariables();
                    ExecuteRenderVegetation(false);
                    //isRenderWait = false;
               // }
            }
            
        }
#endif
        */
        private void OnPreRender()
        {
            if (!Application.isPlaying)
                SelectedCamera = Camera.current;

        }
        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            if (_sleepMode) return;
            if (Application.isPlaying)
            {
                if (!isCamera || !VegetationCamera.isDrawGrass) return;


                if (!ExecuteChecklist(false)) return;

                //SetBillboardShaderVariables();
                /*
#if UNITY_EDITOR
                SetupCullingGroup();
#endif
    */
                //if (SceneViewDetector.Instance == null)
                //    SceneViewDetector.Instance = new SceneViewDetector();
                //
				/*
                if (currentTerrain)
                {
                    if (currentTerrain.transform.hasChanged)
                    {
                        currentTerrain.transform.hasChanged = false;
                        MoveVegetationSystem(currentTerrain.transform.position - _currentTerrainPosition);
                        //MoveBillboardSystem(currentTerrain.transform.position - _currentTerrainPosition);
                        _currentTerrainPosition = currentTerrain.transform.position;
                    }
                }
				*/
                // if (Application.isPlaying)
                {
                    Profiler.BeginSample("ExecuteRenderVegetation");
                    ExecuteRenderVegetation(true);
                    Profiler.EndSample();
                }
            }
            else
            {
                if (!ExecuteChecklist(false)) return;
                //if (!ExecuteChecklist(false)) return;
                //SetBillboardShaderVariables();
                //SetupCullingGroupCam(SelectedCamera, false, false);
                ExecuteRenderVegetation(true);

            }
            
        }
#endif
        private void MoveBillboardSystem(Vector3 offset)
        {
            BillboardSystem billboardSystem = gameObject.GetComponent<BillboardSystem>();
            if (billboardSystem) billboardSystem.MoveBillboardSystem(offset);
        }

        private void MoveVegetationSystem(Vector3 offset)
        {
            UnityTerrainData.UpdatePosition();
            VegetationCellQuadTree.Move(new Vector2(offset.x, offset.z));

            for (int i = 0; i <= VegetationCellList.Count - 1; i++)
            {
                VegetationCellList[i].Move(offset);
            }

            SetupCullingGroup(false, false);
            _floatingOriginChanged = true;
            SetDirty();
        }

        public void AdjustWindSampler()
        {
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                Camera sceneviewCamera = SceneViewDetector.GetCurrentSceneViewCamera();
                if (sceneviewCamera)
                {
                    WindSampler.transform.position = sceneviewCamera.transform.position;
                    WindSampler.transform.rotation = sceneviewCamera.transform.rotation;
                }
#endif
            }
            else
            {
                WindSampler.SetActive(VisibleVegetationCellList.Count != 0);
                WindSampler.transform.position = SelectedCamera.transform.position;
                WindSampler.transform.rotation = SelectedCamera.transform.rotation;
            }
        }

        void SetShadowMapVariables()
        {

            if (SunDirectionalLight)
            {
                Vector3 sundirection = -SunDirectionalLight.transform.forward * 2.5f;
                Vector4 gVsSunDirection = new Vector4(sundirection.x, sundirection.y, sundirection.z, SunDirectionalLight.intensity);
                Shader.SetGlobalVector("gVSSunDirection", gVsSunDirection);
                Shader.SetGlobalVector("gVSSunSettings", new Vector4(SunDirectionalLight.shadowStrength, SunDirectionalLight.shadowBias, 0, 0));

            }
            else
            {
                Shader.SetGlobalVector("gVSSunDirection", Vector4.zero);
                Shader.SetGlobalVector("gVSSunSettings", new Vector4(0, 0, 0, 0));
            }
        }



        public void ExecuteRenderVegetation(bool forceRender)
        {
            
            if (!ExecuteChecklist(false)) return;

            //Camera cam = GetCurrentCamera();
            //if (!Application.isPlaying && cam == null) return;
            
#if UNITY_EDITOR
            if (_cullingGroup != null && _cullingGroup.targetCamera == null)
            {
                if (GetCurrentCamera()) SetupCullingGroup();
            }
#endif

            if (Application.isPlaying && SelectedCamera == null && AutoselectCamera)
            {
                Camera newCamera = Camera.main;
                Camera[] cameras = FindObjectsOfType<Camera>();
                for (int i = 0; i <= cameras.Length - 1; i++)
                {
                    if (cameras[i].tag == "MainCamera")
                    {
                        newCamera = cameras[i];
                        break;
                    }
                }
                if (newCamera)
                {
                    SelectedCamera = newCamera;
                    _refreshCulling = true;
                    Enable();
                }
            }

            SetShadowMapVariables();

            if (RenderVegetation && IsDirty && VisibleVegetationCellList.Count > 0)
            {
                Profiler.BeginSample("Update Vegetation Instance Lists");
                UpdateVegetationInstanceLists();

                Profiler.EndSample();
                IsDirty = false;
            }

            if (RenderVegetation && VisibleVegetationCellList.Count > 0)
            {
                //DistanceLODCulling();
                DrawVegetation();
                if (DirectFromCell)
                {
                    if (SystemInfo.supportsComputeShaders && UseComputeShaders)
                    {
                        if (Application.isPlaying) DrawCellsIndirectComputeShader();
                    }
                    else
                    {
                        Profiler.BeginSample("DrawMeshInstancedIndirect");
                        if (Application.isPlaying) DrawVegetationIndirectFromCell();
                        Profiler.EndSample();
                    }
                }
            }

            if (_floatingOriginChanged)
            {
                VisibleVegetationCellList.Clear();
                _floatingOriginChanged = false;
            }
        }

        private void DrawTouchReact()
        {
#if TOUCH_REACT
            if (TouchReactSystem.TouchReactEnabled())
                for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
                    if (currentVegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.Objects ||
                        currentVegetationPackage.VegetationInfoList[i].VegetationType == VegetationType.LargeObjects)
                        for (int j = 0; j <= VegetationModelInfoList[i].SplitVegetationInstanceList.Count - 1; j++)
                            if (currentVegetationPackage.VegetationInfoList[i].UseTouchReact)
                                for (int l = 0;
                                    l <= VegetationModelInfoList[i].VegetationItemInfo.sourceMesh.subMeshCount - 1;
                                    l++)
                                    TouchReactSystem.DrawMeshInstanced(VegetationModelInfoList[i].VegetationItemInfo.sourceMesh,
                                        VegetationModelInfoList[i].SplitVegetationInstanceList[j], l);
#endif
        }

        void DrawVegetation()
        {
            Camera targetCamera = null;
            if (RenderSingleCamera && Application.isPlaying) targetCamera = SelectedCamera;

            if (VisibleVegetationCellList.Count == 0) return;

            VegetationItemInfo[] vegItemInfo = (currentVegetationPlacingData == null) ? currentVegetationPackage.VegetationInfoList.ToArray() : currentVegetationPlacingData.VegetationInfoList;

            for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
            {
                bool instancedIndirect = VegetationModelInfoList[i].VegetationRenderType == VegetationRenderType.InstancedIndirect;

                VegetationItemInfo vegetationItemInfo = vegItemInfo[i];
                if (!Application.isPlaying) instancedIndirect = false;


                if (!instancedIndirect)
                {
                    var shadowMode = Application.isPlaying ? GetVegetationShadowMode(vegetationItemInfo.VegetationType) : GetVegetationEditorShadows(vegetationItemInfo.VegetationType);
                    if (vegetationItemInfo.DisableShadows) shadowMode = ShadowCastingMode.Off;

                    VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0.Clear();
                    VegetationModelInfoList[i].VegetationMaterialPropertyBlockShadowsLOD0.Clear();
                    /*
                    
                    //VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD1.Clear();
                    //VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD2.Clear();
                    
                   // VegetationModelInfoList[i].VegetationMaterialPropertyBlockShadowsLOD1.Clear();
                    //VegetationModelInfoList[i].VegetationMaterialPropertyBlockShadowsLOD2.Clear();

                   
                        UpdateMaterialPropertyBlockWind(VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0);
                       // UpdateMaterialPropertyBlockWind(VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD1);
                      //  UpdateMaterialPropertyBlockWind(VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD2);
                        UpdateMaterialPropertyBlockWind(VegetationModelInfoList[i].VegetationMaterialPropertyBlockShadowsLOD0);
                       // UpdateMaterialPropertyBlockWind(VegetationModelInfoList[i].VegetationMaterialPropertyBlockShadowsLOD1);
                       // UpdateMaterialPropertyBlockWind(VegetationModelInfoList[i].VegetationMaterialPropertyBlockShadowsLOD2);
                    //}
                    */

                    for (int j = 0; j <= VegetationModelInfoList[i].SplitVegetationInstanceList.Count - 1; j++)
                    {
                        if (VegetationModelInfoList[i].SplitVegetationInstanceList[j].Count <= 0) continue;

                        if (UseCPUDistanceLODCulling(currentVegetationPackage.VegetationInfoList[i].VegetationType))
                        {
                            bool drawInstanced = RenderInstanced &&
                                                 VegetationModelInfoList[i].VegetationRenderType !=
                                                 VegetationRenderType.Normal;
                            DrawLODMesh(drawInstanced,
                                VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD0InstanceList,
                                VegetationModelInfoList[i].VegetationItemInfo.sourceMesh,
                                VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials, GetVegetationLayer(
                                    vegetationItemInfo
                                        .VegetationType), targetCamera, shadowMode,
                                VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0,
                                vegetationItemInfo.VegetationType, VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD0FadeList);
                            /*
                            DrawLODMesh(drawInstanced,
                                VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD1InstanceList,
                                VegetationModelInfoList[i].VegetationMeshLod1,
                                VegetationModelInfoList[i].VegetationMaterialsLOD1, GetVegetationLayer(
                                    vegetationItemInfo
                                        .VegetationType), targetCamera, shadowMode,
                                VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD1,
                                vegetationItemInfo.VegetationType, VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD1FadeList);

                            DrawLODMesh(drawInstanced,
                                VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD2InstanceList,
                                VegetationModelInfoList[i].VegetationMeshLod2,
                                VegetationModelInfoList[i].VegetationMaterialsLOD2, GetVegetationLayer(
                                    vegetationItemInfo
                                        .VegetationType), targetCamera, shadowMode,
                                VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD2,
                                vegetationItemInfo.VegetationType, VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD2FadeList);
                                */
                            if (shadowMode == ShadowCastingMode.On)
                            {
                                DrawLODMesh(drawInstanced,
                                    VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD0ShadowInstanceList,
                                    VegetationModelInfoList[i].VegetationItemInfo.sourceMesh,
                                    VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials, GetVegetationLayer(
                                        vegetationItemInfo
                                            .VegetationType), targetCamera, ShadowCastingMode.ShadowsOnly,
                                    VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0,
                                    vegetationItemInfo.VegetationType, VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD0ShadowFadeList);
                                /*
                                DrawLODMesh(drawInstanced,
                                    VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD1ShadowInstanceList,
                                    VegetationModelInfoList[i].VegetationMeshLod1,
                                    VegetationModelInfoList[i].VegetationMaterialsLOD1, GetVegetationLayer(
                                        vegetationItemInfo
                                            .VegetationType), targetCamera, ShadowCastingMode.ShadowsOnly,
                                    VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD1,
                                    vegetationItemInfo.VegetationType, VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD1ShadowFadeList);

                                DrawLODMesh(drawInstanced,
                                    VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD2ShadowInstanceList,
                                    VegetationModelInfoList[i].VegetationMeshLod2,
                                    VegetationModelInfoList[i].VegetationMaterialsLOD2, GetVegetationLayer(
                                        vegetationItemInfo
                                            .VegetationType), targetCamera, ShadowCastingMode.ShadowsOnly,
                                    VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD2,
                                    vegetationItemInfo.VegetationType, VegetationModelInfoList[i].LODVegetationInstanceList[j].LOD2ShadowFadeList);
                                    */
                            }
                        }
                        else
                        {
#if UNITY_2018_1_OR_NEWER
                            LightProbeUsage lightProbeUsage = LightProbeUsage.Off;
                            if (UseInstancedGI(vegetationItemInfo.VegetationType))
                            {
                                SampleInstancedGI(VegetationModelInfoList[i].SplitVegetationInstanceList[j],
                                    VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0);
                                lightProbeUsage = LightProbeUsage.CustomProvided;
                            }

                            for (int l = 0;
                                    l <= VegetationModelInfoList[i].VegetationItemInfo.sourceMesh.subMeshCount - 1;
                                    l++)
                            {

                                if (RenderInstanced && VegetationModelInfoList[i].VegetationRenderType !=
                                    VegetationRenderType.Normal)
                                {
                                    Graphics.DrawMeshInstanced(VegetationModelInfoList[i].VegetationItemInfo.sourceMesh, l,
                                        VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials[l],
                                        VegetationModelInfoList[i].SplitVegetationInstanceList[j],
                                        VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0, shadowMode,
                                        RecieveShadows, GetVegetationLayer(vegetationItemInfo.VegetationType),
                                        targetCamera, lightProbeUsage);
                                }
                                else
                                {
                                    for (int k = 0;
                                        k <= VegetationModelInfoList[i].SplitVegetationInstanceList[j].Count - 1;
                                        k++)
                                    {
                                        Graphics.DrawMesh(VegetationModelInfoList[i].VegetationItemInfo.sourceMesh,
                                            VegetationModelInfoList[i].SplitVegetationInstanceList[j][k],
                                            VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials[l],
                                            GetVegetationLayer(vegetationItemInfo.VegetationType), targetCamera, l,
                                            VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0,
                                            shadowMode, RecieveShadows);
                                    }
                                }
                            }
#else
                            for (int l = 0; l <= VegetationModelInfoList[i].VegetationMeshLod0.subMeshCount - 1; l++)
                            {

                                if (RenderInstanced && VegetationModelInfoList[i].VegetationRenderType !=
                                    VegetationRenderType.Normal)
                                {
                                    Graphics.DrawMeshInstanced(VegetationModelInfoList[i].VegetationMeshLod0, l,
                                        VegetationModelInfoList[i].VegetationMaterialsLOD0[l],
                                        VegetationModelInfoList[i].SplitVegetationInstanceList[j],
                                        VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0, shadowMode,
                                        RecieveShadows, GetVegetationLayer(vegetationItemInfo.VegetationType),
                                        targetCamera);
                                }
                                else
                                {
                                    for (int k = 0;
                                        k <= VegetationModelInfoList[i].SplitVegetationInstanceList[j].Count - 1;
                                        k++)
                                    {
                                        Graphics.DrawMesh(VegetationModelInfoList[i].VegetationMeshLod0,
                                            VegetationModelInfoList[i].SplitVegetationInstanceList[j][k],
                                            VegetationModelInfoList[i].VegetationMaterialsLOD0[l],
                                            GetVegetationLayer(vegetationItemInfo.VegetationType), targetCamera, l,
                                            VegetationModelInfoList[i].VegetationMaterialPropertyBlockLOD0, shadowMode,
                                            RecieveShadows);
                                    }
                                }
                            }
#endif
                        }
                    }
                }
            }
        }


#if UNITY_2018_1_OR_NEWER
        void SampleInstancedGI(List<Matrix4x4> matrixList, MaterialPropertyBlock materialPropertyBlock )
        {
            Profiler.BeginSample("Sample Indirect GI");
            _giPositionList.Clear();
            for (int m = 0; m <= matrixList.Count - 1; m++)
            {
                Vector3 position = MatrixTools.ExtractTranslationFromMatrix(matrixList[m]);
                _giPositionList.Add(position + new Vector3(0, 1, 0));
            }

            _giProbeList.Clear();
            _giOcclusionList.Clear();

            if (vegetationSettings.InstancedOcclusionProbes)
            {
                UnityEngine.LightProbes.CalculateInterpolatedLightAndOcclusionProbes(_giPositionList, _giProbeList, _giOcclusionList);
                materialPropertyBlock.CopySHCoefficientArraysFrom(_giProbeList);
                materialPropertyBlock.CopyProbeOcclusionArrayFrom(_giOcclusionList);
            }
            else
            {
                UnityEngine.LightProbes.CalculateInterpolatedLightAndOcclusionProbes(_giPositionList, _giProbeList, null);
                materialPropertyBlock.CopySHCoefficientArraysFrom(_giProbeList);
            }
            Profiler.EndSample();
        }
#endif

        void DrawLODMesh(bool instanced, List<Matrix4x4> instanceList, Mesh mesh, Material[] materials, LayerMask vegetationLayer, Camera targetCamera, ShadowCastingMode shadowMode, MaterialPropertyBlock materialPropertyBlock, VegetationType vegetationType, List<float> fadeList)
        {
            if (instanceList.Count == 0) return;

            if (instanced)
            {
                if (fadeList.Count > 0)
                {
                    materialPropertyBlock.SetFloatArray("unity_LODFade", fadeList);
                }
#if UNITY_2018_1_OR_NEWER
                LightProbeUsage lightProbeUsage = LightProbeUsage.Off;
                if (UseInstancedGI(vegetationType))
                {
                    SampleInstancedGI(instanceList, materialPropertyBlock);
                    lightProbeUsage = LightProbeUsage.CustomProvided;
                }

                for (int l = 0; l <= mesh.subMeshCount - 1; l++)
                {
                    if (materials.Length > l && materials[l])
                    {
                        Graphics.DrawMeshInstanced(mesh, l, materials[l], instanceList, materialPropertyBlock, shadowMode, RecieveShadows, vegetationLayer, targetCamera,lightProbeUsage);
                    }
                }
#else
                for (int l = 0; l <= mesh.subMeshCount - 1; l++)
                {
                    if (materials.Length > l && materials[l])
                    {
                        Graphics.DrawMeshInstanced(mesh, l, materials[l], instanceList, materialPropertyBlock, shadowMode, RecieveShadows, vegetationLayer, targetCamera);
                    }
                }
#endif
            }
            else
            {
                for (int l = 0; l <= mesh.subMeshCount - 1; l++)
                {
                    for (int k = 0; k <= instanceList.Count - 1; k++)
                    {
                        Graphics.DrawMesh(mesh, instanceList[k], materials[l], vegetationLayer, targetCamera, l, materialPropertyBlock, shadowMode, RecieveShadows);
                    }
                }

            }
        }

        void DrawLODMeshInstancedIndirect(Mesh mesh, Material[] materials, Bounds bounds, List<ComputeBuffer> argsList, ShadowCastingMode shadowCastingMode, bool recieveShadows, LayerMask vegetationLayer, Camera targetCamera, MaterialPropertyBlock materialPropertyBlock)
        {
            for (int k = 0; k <= mesh.subMeshCount - 1; k++)
            {
                Graphics.DrawMeshInstancedIndirect(mesh, k,
                    materials[k],
                    bounds,
                    argsList[k], 0,
                    materialPropertyBlock, shadowCastingMode,
                    RecieveShadows, vegetationLayer, targetCamera);
            }
        }

        private void DrawVegetationIndirectFromCell()
        {
#if UNITY_5_6_OR_NEWER
            Camera targetCamera = null;
            if (Application.isPlaying) targetCamera = SelectedCamera;

            for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
            {
                VegetationItemInfo vegetationItemInfo = currentVegetationPackage.VegetationInfoList[i];

                LayerMask vegetationLayer = GetVegetationLayer(vegetationItemInfo.VegetationType);
                if (vegetationItemInfo.VegetationRenderType != VegetationRenderType.InstancedIndirect) continue;

                ShadowCastingMode shadowMode = ShadowCastingMode.Off;
                if (Application.isPlaying) shadowMode = GetVegetationShadowMode(vegetationItemInfo.VegetationType);
                if (vegetationItemInfo.DisableShadows) shadowMode = ShadowCastingMode.Off;

                for (int j = 0; j <= VisibleVegetationCellList.Count - 1; j++)
                {
                    if (VisibleVegetationCellList[j].DistanceBand >= 3) continue;
                    if (VisibleVegetationCellList[j].IndirectInfoList[i].InstanceCount == 0) continue;
                    if (VisibleVegetationCellList[j].IndirectInfoList[i].InstancedIndirect &&
                        VisibleVegetationCellList[j].IndirectInfoList[i].PositionBuffer != null)
                    {
                        /*
                        if (UseIndirectLoDs)
                        {
                            switch (VisibleVegetationCellList[j].DistanceBand)
                            {
                                case 0:
                                    DrawLODMeshInstancedIndirect(VegetationModelInfoList[i].VegetationMeshLod0,
                                        VegetationModelInfoList[i].VegetationMaterialsLOD0,
                                        VisibleVegetationCellList[j].CellBounds,
                                        VisibleVegetationCellList[j].IndirectInfoList[i].ArgsBufferlod0List,
                                        shadowMode, RecieveShadows, vegetationLayer, targetCamera,
                                        VisibleVegetationCellList[j].IndirectInfoList[i].MaterialPropertyBlockLOD0);
                                    break;
                                case 1:

                                        VegetationModelInfoList[i].VegetationMaterialsLOD1,
                                        VisibleVegetationCellList[j].CellBounds,
                                        VisibleVegetationCellList[j].IndirectInfoList[i].ArgsBufferlod1List,
                                        shadowMode, RecieveShadows, vegetationLayer, targetCamera,
                                        VisibleVegetationCellList[j].IndirectInfoList[i].MaterialPropertyBlockLOD0);
                                    break;
                                case 2:
                                    DrawLODMeshInstancedIndirect(VegetationModelInfoList[i].VegetationMeshLod2,
                                        VegetationModelInfoList[i].VegetationMaterialsLOD2,
                                        VisibleVegetationCellList[j].CellBounds,
                                        VisibleVegetationCellList[j].IndirectInfoList[i].ArgsBufferlod2List,
                                        shadowMode, RecieveShadows, vegetationLayer, targetCamera,
                                        VisibleVegetationCellList[j].IndirectInfoList[i].MaterialPropertyBlockLOD0);
                                    break;
                            }
                        }
                        else
                        */
                        {
                            DrawLODMeshInstancedIndirect(VegetationModelInfoList[i].VegetationItemInfo.sourceMesh,
                                VegetationModelInfoList[i].VegetationItemInfo.sourceMaterials,
                                VisibleVegetationCellList[j].CellBounds,
                                VisibleVegetationCellList[j].IndirectInfoList[i].ArgsBufferlod0List,
                                shadowMode, RecieveShadows, vegetationLayer, targetCamera,
                                VisibleVegetationCellList[j].IndirectInfoList[i].MaterialPropertyBlockLOD0);
                        }
                    }
                }
            }
#endif
        }


        bool UseCPUDistanceLODCulling(VegetationType vegetationType)
        {
            if (vegetationType == VegetationType.Tree) return true;
            if (vegetationType == VegetationType.Objects && UseObjectCPUCulling) return true;
            if (vegetationType == VegetationType.LargeObjects && UseLargeObjectCPUCulling) return true;
            return false;
        }
        private void DistanceLODCulling()
        {
            // AABBList.Clear();
            // _shadowAABBList.Clear();
            Profiler.BeginSample("DistanceLODCulling");

            bool useCPUCulling = (UseCPUCulling && Application.isPlaying);

            var cullDistance = Mathf.RoundToInt((GetVegetationDistance() + GetTreeDistance() + 0.01f) - CellSize);

            Vector3 cameraPosition = Vector3.zero;
            Camera currentCamera = GetCurrentCamera();
            if (currentCamera) cameraPosition = GetCameraPosition();
            
            bool hasDirectionalLight = false;
            Vector3 lightDirection = Vector3.forward;
            if (SunDirectionalLight && _currentTerrainCollider)
            {
                hasDirectionalLight = true;
                lightDirection = SunDirectionalLight.transform.forward;
            }
            
            VegetationItemInfo[] vegItems = (currentVegetationPlacingData == null) ? currentVegetationPackage.VegetationInfoList.ToArray() : currentVegetationPlacingData.VegetationInfoList;

            for (int i = 0; i <= VegetationModelInfoList.Count - 1; i++)
            {
                VegetationItemInfo vegetationItemInfo = VegetationModelInfoList[i].VegetationItemInfo;
                /*
                bool useLODFade = false;
                
#if UNITY_2018_1_OR_NEWER
                if (vegetationItemInfo.ShaderType == VegetationShaderType.Speedtree) useLODFade = true;
#endif
*/
                float maxScale = vegetationItemInfo.MaxScale;
                Vector3 boundsSize = vegetationItemInfo.Bounds.size * maxScale;

                if (!UseCPUDistanceLODCulling(currentVegetationPackage.VegetationInfoList[i].VegetationType)) continue;

                float lod1Distance = VegetationModelInfoList[i].LOD1Distance * QualitySettings.lodBias * vegItems[i].LODFactor * (60 / currentCamera.fieldOfView);
                float lod2Distance = VegetationModelInfoList[i].LOD2Distance * QualitySettings.lodBias * vegItems[i].LODFactor * (60 / currentCamera.fieldOfView);

                ClearVegetationLODSplitList(VegetationModelInfoList[i].LODVegetationInstanceList, VegetationModelInfoList[i].MatrixListPool, VegetationModelInfoList[i].FloatListPool);
                VegetationModelInfoList[i].LODVegetationInstanceList.Clear();

                for (int j = 0; j <= VegetationModelInfoList[i].SplitVegetationInstanceList.Count - 1; j++)
                {
                    LODVegetationInstanceInfo lodVegetationInstanceInfo = _LODVegetationInstanceInfoListPool.Get();
                    InitLODVegetationInstanceInfo(lodVegetationInstanceInfo, VegetationModelInfoList[i].MatrixListPool,
                        VegetationModelInfoList[i].FloatListPool);


                    VegetationModelInfoList[i].LODVegetationInstanceList.Add(lodVegetationInstanceInfo);
                    var maxSplitVegetationInstanceListCount =
                        VegetationModelInfoList[i].SplitVegetationInstanceList[j].Count - 1;

                    for (int k = 0; k <= maxSplitVegetationInstanceListCount; k++)
                    {

                        Vector3 position = MatrixTools.ExtractTranslationFromMatrix(VegetationModelInfoList[i]
                            .SplitVegetationInstanceList[j][k]);

                        float distance = Vector3.Distance(cameraPosition, position);

                        int visibility = 1;
                        if (useCPUCulling)
                        {
                            Profiler.BeginSample("check visibility");
                            visibility = CheckVegetationItemVisibility(position, boundsSize, vegetationItemInfo, hasDirectionalLight, lightDirection, distance);
                            Profiler.EndSample();
                        }

                        if (visibility == 2)
                        {
                            if (distance <= cullDistance)
                            {

                                    lodVegetationInstanceInfo.LOD0ShadowInstanceList.Add(VegetationModelInfoList[i]
                                        .SplitVegetationInstanceList[j][k]);
                            }
                        }
                        else if (visibility == 1)
                        {
                            if (distance <= cullDistance)
                            {
                                    lodVegetationInstanceInfo.LOD0InstanceList.Add(VegetationModelInfoList[i]
                                        .SplitVegetationInstanceList[j][k]);
                               
                            }
                        }
                    }
                }
            }
            Profiler.EndSample();
        }

        float CalculateLODFade(float cameraDistance, float nextLODDistance)
        {
            float distance = nextLODDistance - cameraDistance;
            if (distance <= vegetationSettings.LODFadeDistance)
            {
                return Mathf.Clamp01(1 - distance / vegetationSettings.LODFadeDistance);
            }
            return 0;
        }
        int CheckVegetationItemVisibility(Vector3 position, Vector3 boundSize, VegetationItemInfo vegetationItemInfo, bool hasSunDirection, Vector3 lightDirection, float distance)
        {
            Vector3 boundsCenter = position + new Vector3(0, boundSize.y / 2f, 0);
            Bounds itemBounds = new Bounds(boundsCenter, boundSize);
            //AABBList.Add(itemBounds); 

            bool visible = GeometryUtility.TestPlanesAABB(GeometryUtilityAllocFree.FrustrumPlanes, itemBounds);
            if (visible) return 1;

            if (hasSunDirection && distance < QualitySettings.shadowDistance && vegetationItemInfo.VegetationType != VegetationType.Objects)
            {
                bool visibleShadow = CheckVegetationItemShadowVisibility(itemBounds, lightDirection);
                if (visibleShadow) return 2;
            }
            else
            {
                return 2;
            }
            return 0;
        }

        bool CheckVegetationItemShadowVisibility(Bounds itemBounds, Vector3 lightDirection)
        {
            Ray p0 = new Ray(new Vector3(itemBounds.min.x, itemBounds.max.y, itemBounds.min.z), lightDirection);
            Ray p1 = new Ray(new Vector3(itemBounds.min.x, itemBounds.max.y, itemBounds.max.z), lightDirection);
            Ray p2 = new Ray(new Vector3(itemBounds.max.x, itemBounds.max.y, itemBounds.min.z), lightDirection);
            Ray p3 = new Ray(itemBounds.max, lightDirection);

            float shadowDistance = QualitySettings.shadowDistance;
            RaycastHit hit;
            bool hasHit = false;

            if (_currentTerrainCollider.Raycast(p0, out hit, shadowDistance))
            {
                itemBounds.Encapsulate(hit.point);
                hasHit = true;
            }

            if (_currentTerrainCollider.Raycast(p1, out hit, shadowDistance))
            {
                itemBounds.Encapsulate(hit.point);
                hasHit = true;
            }
            if (_currentTerrainCollider.Raycast(p2, out hit, shadowDistance))
            {
                itemBounds.Encapsulate(hit.point);
                hasHit = true;
            }
            if (_currentTerrainCollider.Raycast(p3, out hit, shadowDistance))
            {
                itemBounds.Encapsulate(hit.point);
                hasHit = true;
            }

            //_shadowAABBList.Add(itemBounds);
            return hasHit && GeometryUtility.TestPlanesAABB(GeometryUtilityAllocFree.FrustrumPlanes, itemBounds);
        }

        LayerMask GetVegetationLayer(VegetationType vegetationType)
        {
            switch (vegetationType)
            {
                case VegetationType.Grass:
                    return vegetationSettings.GrassLayer;
                case VegetationType.Plant:
                    return vegetationSettings.PlantLayer;
                case VegetationType.Tree:
                    return vegetationSettings.TreeLayer;
                case VegetationType.Objects:
                    return vegetationSettings.ObjectLayer;
                case VegetationType.LargeObjects:
                    return vegetationSettings.LargeObjectLayer;
                default:
                    return 0;
            }
        }
        private ShadowCastingMode GetVegetationShadowMode(VegetationType vegetationType)
        {
            bool castShadows;
            switch (vegetationType)
            {
                case VegetationType.Grass:
                    castShadows = vegetationSettings.GrassShadows;
                    break;
                case VegetationType.Plant:
                    castShadows = vegetationSettings.PlantShadows;
                    break;
                case VegetationType.Tree:
                    castShadows = vegetationSettings.TreeShadows;
                    break;
                case VegetationType.Objects:
                    castShadows = vegetationSettings.ObjectShadows;
                    break;
                case VegetationType.LargeObjects:
                    castShadows = vegetationSettings.LargeObjectShadows;
                    break;
                default:
                    castShadows = false;
                    break;
            }

            return castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
        }

        private bool UseInstancedGI(VegetationType vegetationType)
        {
            switch (vegetationType)
            {
                case VegetationType.Grass:
                    return vegetationSettings.InstancedGIGrass;
                case VegetationType.Plant:
                    return vegetationSettings.InstancedGIPlant;
                case VegetationType.Tree:
                    return vegetationSettings.InstancedGITree;
                case VegetationType.Objects:
                    return vegetationSettings.InstancedGIObject;
                case VegetationType.LargeObjects:
                    return vegetationSettings.InstancedGILargeObject;
                default:
                    return vegetationSettings.InstancedGIGrass;
            }
        }

        ShadowCastingMode GetVegetationEditorShadows(VegetationType vegetationType)
        {
            bool castShadows;
            switch (vegetationType)
            {
                case VegetationType.Grass:
                    castShadows = vegetationSettings.EditorGrassShadows;
                    break;
                case VegetationType.Plant:
                    castShadows = vegetationSettings.EditorPlantShadows;
                    break;
                case VegetationType.Tree:
                    castShadows = vegetationSettings.EditorTreeShadows;
                    break;
                case VegetationType.Objects:
                    castShadows = vegetationSettings.EditorObjectShadows;
                    break;
                case VegetationType.LargeObjects:
                    castShadows = vegetationSettings.EditorLargeObjectShadows;
                    break;
                default:
                    castShadows = false;
                    break;
            }

            return castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
        }

#endregion
#region Gizmos  


        // ReSharper disable once UnusedMember.Local
        void OnDrawGizmos()
        {
            //Gizmos.color = Color.white;

            //for (int i = 0; i <= AABBList.Count - 1; i++)
            //{
            //    Gizmos.DrawWireCube(AABBList[i].center, AABBList[i].size);
            //}

            //Gizmos.color = Color.red;

            //for (int i = 0; i <= _shadowAABBList.Count - 1; i++)
            //{
            //    Gizmos.DrawWireCube(_shadowAABBList[i].center, _shadowAABBList[i].size);
            //}

            // for (int i=0; i< VegetationCellQuadTree.Count; i++)
            //{
            /*
            if (UnityEditor.Selection.activeGameObject == transform.gameObject)
            {
                Gizmos.color = Color.red;
                VegetationCellQuadTree.Draw();

                for (int i=0; i< VegetationCellQuadTree.m_root.Contents.Count; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(VegetationCellQuadTree.m_root.Contents[i].CellBounds.center,
                         VegetationCellQuadTree.m_root.Contents[i].CellBounds.size);

                    Gizmos.color = Color.blue;
                    VegetationCellQuadTree.m_root._mNodes[i].Draw();
                }

            }
            */
           // }

            if (ShowCellGrid)
            {
                if (ShadowVegetationCellList != null)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.5F);
                    for (int i = 0; i <= ShadowVegetationCellList.Count - 1; i++)
                    {
                        Gizmos.DrawWireCube(ShadowVegetationCellList[i].CellBounds.center,
                            ShadowVegetationCellList[i].CellBounds.size);

                        if (!ShadowVegetationCellList[i].IsVisible)
                        {
                            Gizmos.DrawSphere(ShadowVegetationCellList[i].CellBounds.center, 2);
                        }
                    }
                }
            }

            if (PotentialVisibleVegetationCellList == null) return;
            {
                for (int i = 0; i <= PotentialVisibleVegetationCellList.Count - 1; i++)
                {
                    if (PotentialVisibleVegetationCellList[i].InitDone)
                    {
                        if (ShowCellGrid)
                        {
                            if (PotentialVisibleVegetationCellList[i].IsVisible)
                            {
                                switch (PotentialVisibleVegetationCellList[i].DistanceBand)
                                {
                                    case 0:
                                        Gizmos.color = new Color(0, 1f, 0, 1F);
                                        break;
                                    case 1:
                                        Gizmos.color = new Color(1f, 1f, 0, 1f);
                                        break;
                                    case 2:
                                        Gizmos.color = new Color(1f, 0.5f, 0, 1f);
                                        break;
                                    case 3:
                                        Gizmos.color = new Color(1f, 1f, 1f, 1f);
                                        break;
                                    case 4:
                                        Gizmos.color = new Color(0, 0, 1f, 1f);
                                        break;
                                    default:
                                        Gizmos.color = new Color(1f, 0, 0, 1f);
                                        break;
                                }

                                Gizmos.DrawWireCube(PotentialVisibleVegetationCellList[i].CellBounds.center, PotentialVisibleVegetationCellList[i].CellBounds.size);

                            }
                            else
                            {
                                // Gizmos.color = new Color(1, 0, 0, 0.5F);
                                // Gizmos.DrawWireCube(PotentialVisibleVegetationCellList[i].CellBounds.center, PotentialVisibleVegetationCellList[i].CellBounds.size);
                            }
                        }
                        if (!ShowCellLoadState) continue;
                        if (PotentialVisibleVegetationCellList[i].DataLoadLevel >= 4) continue;
                        Gizmos.color = PotentialVisibleVegetationCellList[i].DataLoadLevel == 3 ? new Color(1, 1, 1, 15F) : new Color(1, 1, 0, 1F);
                        Gizmos.DrawSphere(PotentialVisibleVegetationCellList[i].CellBounds.center, 2);
                    }
                }
            }
        }
#endregion
    }
}
