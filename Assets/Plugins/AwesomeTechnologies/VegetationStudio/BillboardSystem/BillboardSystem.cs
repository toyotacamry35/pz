using System.Collections.Generic;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Utility.Quadtree;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace AwesomeTechnologies.Billboards
{
    public enum BillboardSystemType
    {
        Realtime,
        Baked
    }

    [HelpURL("http://www.awesometech.no/index.php/home/vegetation-studio/components/billboard-system")]
    [AwesomeTechnologiesScriptOrder(98)]
    [ExecuteInEditMode]
    public class BillboardSystem : MonoBehaviour
    {
        public VegetationSystem VegetationSystem;
        public BillboardSystemType BillboardSystemType = BillboardSystemType.Realtime;

        public GameObject BakedBillboardsParent;

        public bool AutoBakeOnSetVegetationPackage;

        public bool DisableInEditorMode;
        public bool RenderBillboards = true;
        public bool BillboardShadows = true;

        public bool TerrainBakeParrent = true;
        public bool BakeToProject;
        public bool ClearBakedBillboardsOnBake = true;

        public int AdditonalCacheDistance;

        public float CellSize = 1000;
        public bool ShowCellGrid;
        public bool PreloadAroundCamera = true;
        public bool ClearInvisibleCacheAtFloatingOrigin;
        public bool OverrideRenderQueue = false;
        public int RenderQueue = 2450;
        public bool OverrideLayer = false;
        public LayerMask BillboardLayer;

        private bool _floatingOriginChanged;

        [System.NonSerialized]
        public QuadTree<BillboardCell> BillboardCellQuadTree = new QuadTree<BillboardCell>(new Rect());
        [System.NonSerialized]
        public List<BillboardCell> BillboardCellList = new List<BillboardCell>();
        [System.NonSerialized]
        public List<BillboardCell> PotentialVisibleBillboardCellList = new List<BillboardCell>();
        [System.NonSerialized]
        public List<BillboardCell> VisibleBillboardCellList = new List<BillboardCell>();

        private Vector3 _level1CullingCameraPosition;
        private Camera _currentCamera;
        private MaterialPropertyBlock _vegetationPropertyBlock;
        private readonly List<Material> _vegetationItemBillboardMaterialList = new List<Material>();

        //private readonly Matrix4x4 _zeroMatrix4X4 = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity,
        //    new Vector3(1, 1, 1));

        private CullingGroup _cullingGroup;
        //private readonly List<Vector3> _moveVertexTempList = new List<Vector3>(65535);

        //private readonly List<VegetationCell> _progressiveSpawningList = new List<VegetationCell>();

        // ReSharper disable once UnusedMember.Local
        private void OnEnable()
        {
            Init();
            EnableEditorApi();
        }

        private void EnableEditorApi()
        {
//#if UNITY_EDITOR
//            EditorApplication.update += UpdateEditorCallback;
//#endif
        }

        void DisableEditorApi()
        {
//#if UNITY_EDITOR
//            EditorApplication.update -= UpdateEditorCallback;
//#endif
        }

//        void UpdateEditorCallback()
//        {
//            UpdateProgressiveBillboardSpawning();
//        }

//        void UpdateProgressiveBillboardSpawning()
//        {
//            if (_progressiveSpawningList.Count > 100)
//            {
//                int maxCount = _progressiveSpawningList.Count - 1;
//                int minCount = maxCount - 100;

//                //int counter = 0;

//                for (var i = maxCount; i > minCount; i--)
//                {
//                    _progressiveSpawningList[i].LoadVegetation(3);
//                    _progressiveSpawningList.RemoveAt(i);

//                    //counter++;
//                }

//                if (_progressiveSpawningList.Count == 0)
//                {
//                    //Debug.Log("Create Meshes");
//                    SetupBillboards();
//#if UNITY_EDITOR
//                    SceneView.RepaintAll();
//#endif
//                }
//                //Debug.Log(counter.ToString() + "  " + _progressiveSpawningList.Count.ToString());
//            }
//            else if (_progressiveSpawningList.Count > 0)
//            {
//                int maxCount = _progressiveSpawningList.Count - 1;
//                //int counter = 0;

//                for (var i = maxCount; i >= 0; i--)
//                {
//                    _progressiveSpawningList[i].LoadVegetation(3);
//                    _progressiveSpawningList.RemoveAt(i);

//                    //counter++;
//                }

//                //Debug.Log(counter.ToString() + "  " + _progressiveSpawningList.Count.ToString());

//                if (_progressiveSpawningList.Count == 0)
//                {
//                    //Debug.Log("Create Meshes");
//                    SetupBillboards();
//#if UNITY_EDITOR
//                    SceneView.RepaintAll();
//#endif
//                }
//            }
//        }

//        void StartProgressiveBillboardSpawning()
//        {
//            if (!VegetationSystem) return;

//            SetupBillboards();
//#if UNITY_EDITOR
//            SceneView.RepaintAll();
//#endif

//            //_progressiveSpawningList.Clear();

//            //for (var i = 0; i <= VegetationSystem.VegetationCellList.Count - 1; i++)
//            //{
//            //    if (!VegetationSystem.VegetationCellList[i].SeaCell)
//            //    {
//            //        _progressiveSpawningList.Add((VegetationSystem.VegetationCellList[i]));
//            //    }              
//            //}
//        }

        private void Init()
        {
            _vegetationPropertyBlock = new MaterialPropertyBlock();

            GetVegetationSystem();
            if (VegetationSystem)
            {
                VegetationSystem.OnSetVegetationPackageDelegate += OnSetVegetationPackage;
                //VegetationSystem.OnVegetationCellClearCacheDelegate += OnVegetationCellClearCache;
                VegetationSystem.OnRefreshVegetationBillboardsDelegate += OnRefreshVegetationBillboards;

                if (Application.isPlaying)
                {
                    VegetationSystem.OnClearCacheDelegate += OnVegetationClearCache;
                }
            }

            if (!VegetationSystem) return;
            if (VegetationSystem.InitDone) CreateBillboards();
        }

        void OnVegetationClearCache()
        {
            for (var i = 0; i <= BillboardCellList.Count - 1; i++)
            {
                if (BillboardCellList[i].HasBillboards)
                {
                    BillboardCellList[i].ClearMeshes();

                }
            }
        }

        void OnRefreshVegetationBillboards(Bounds bounds)
        {
            Rect changedRect = RectExtension.CreateRectFromBounds(bounds);
            List<BillboardCell> overlappingBillboards = BillboardCellQuadTree.Query(changedRect);
            for (var i = 0; i <= overlappingBillboards.Count - 1; i++)
            {
                if (overlappingBillboards[i].HasBillboards)
                {
                    overlappingBillboards[i].ClearMeshes();

                }
            }
        }

        public void RefreshBillboards()
        {
            for (var i = 0; i <= BillboardCellList.Count - 1; i++)
            {
                if (BillboardCellList[i].HasBillboards)
                {
                    BillboardCellList[i].ClearMeshes();

                }
            }
        }



        //void OnVegetationCellClearCache(VegetationCell vegetationCell)
        //{
        //    List<BillboardCell> overlappingBillboards = BillboardCellQuadTree.Query(vegetationCell.Rectangle);
        //    for (var i = 0; i <= overlappingBillboards.Count - 1; i++)
        //    {
        //        if (overlappingBillboards[i].HasBillboards)
        //        {
        //            overlappingBillboards[i].ClearMeshes();

        //        }
        //    }
        //}

        private void GetVegetationSystem()
        {
            if (VegetationSystem == null)
                VegetationSystem = gameObject.GetComponent<VegetationSystem>();
        }

        private void UpdateBillboardClipping()
        {
            if (!VegetationSystem.InitDone) return;
            if (_vegetationItemBillboardMaterialList.Count !=
                VegetationSystem.currentVegetationPackage.VegetationInfoList.Count) return;

            var cullDistance = Mathf.RoundToInt(VegetationSystem.GetVegetationDistance() +
                                                VegetationSystem.GetTreeDistance() - VegetationSystem.CellSize);
            var farCullDistance = Mathf.RoundToInt(VegetationSystem.GetTotalDistance());

            for (var i = 0; i <= _vegetationItemBillboardMaterialList.Count - 1; i++)
            {
                if (_vegetationItemBillboardMaterialList[i] == null) continue;

                if (VegetationSystem)
                {
                    VegetationItemInfo vegetationItemInfo =
                        VegetationSystem.currentVegetationPackage.VegetationInfoList[i];

                    _vegetationItemBillboardMaterialList[i].SetFloat("_Cutoff", vegetationItemInfo.BillboardCutoff);
                    _vegetationItemBillboardMaterialList[i].SetInt("_CullDistance", cullDistance);
                    _vegetationItemBillboardMaterialList[i].SetInt("_FarCullDistance", farCullDistance);

                          
                    _vegetationItemBillboardMaterialList[i]
                        .SetVector("_CameraPosition", VegetationSystem.GetCameraPosition());

                    if (vegetationItemInfo.ShaderType ==
                        VegetationShaderType.Speedtree)
                    {
                        Color tempColor = vegetationItemInfo.BillboardTintColor * vegetationItemInfo.ColorTint1;
                        tempColor.r = Mathf.Clamp01(tempColor.r * 1.3f);
                        tempColor.g = Mathf.Clamp01(tempColor.g * 1.3f);
                        tempColor.b = Mathf.Clamp01(tempColor.b * 1.3f);
                        _vegetationItemBillboardMaterialList[i].SetColor("_Color", tempColor);
                        _vegetationItemBillboardMaterialList[i].SetColor("_HueVariation", vegetationItemInfo.Hue);
                    }
                    else
                    {
                        _vegetationItemBillboardMaterialList[i].SetColor("_Color", vegetationItemInfo.BillboardTintColor);
                    }
                    _vegetationItemBillboardMaterialList[i].SetFloat("_Brightness", vegetationItemInfo.BillboardBrightness);
                    _vegetationItemBillboardMaterialList[i].SetFloat("_MipmapBias", vegetationItemInfo.BillboardMipmapBias);
                }
            }
        }

        public void PreloadPotentialBillboardCells()
        {
            for (int i = 0; i <= PotentialVisibleBillboardCellList.Count - 1; i++)
                if (!PotentialVisibleBillboardCellList[i].HasBillboards)
                    GenerateBillboardCell(PotentialVisibleBillboardCellList[i]);
        }

        public void OnSetVegetationPackage(VegetationPackage vegetationPackage)
        {
            CreateBillboards();
        }

        public void CreateBillboards()
        {
            if (!enabled) return;

#if UNITY_EDITOR
            if (!Application.isPlaying && DisableInEditorMode) { return; }

            //if (!Application.isPlaying)
            //{
            //    StartProgressiveBillboardSpawning();
            //    return;
            //}
#endif         
            SetupBillboards();
        }

        void SetupBillboards()
        {
            CreateBillboardMaterials();
            SetupBillboardCells();
            SetupCullingGroup();
            if (PreloadAroundCamera) 
            {
                PreloadPotentialBillboardCells();
            }
        }


        void ClearBillboardMaterials()
        {
            for (int i = 0; i <= _vegetationItemBillboardMaterialList.Count - 1; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(_vegetationItemBillboardMaterialList[i]);
                }
                else
                {
                    DestroyImmediate(_vegetationItemBillboardMaterialList[i]);
                }
            }

            _vegetationItemBillboardMaterialList.Clear();
        }
        private void CreateBillboardMaterials()
        {

            ClearBillboardMaterials();

            for (int i = 0; i <= VegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
                if (VegetationSystem.currentVegetationPackage.VegetationInfoList[i].VegetationType ==
                    VegetationType.Tree && VegetationSystem .currentVegetationPackage.VegetationInfoList[i] .UseBillboards) //&& VegetationSystem.CurrentVegetationPackage.VegetationInfoList[i].IncludeInTerrain
                {
                    Material billboardMaterial =
                        new Material(Shader.Find("AwesomeTechnologies/Billboards/GroupBillboards"))
                        {
                            enableInstancing = true
                        };

                    if (OverrideRenderQueue)
                    {
                        billboardMaterial.renderQueue = RenderQueue;
                    }
                    VegetationItemInfo vegetationItemInfo =
                        VegetationSystem.currentVegetationPackage.VegetationInfoList[i];
                    billboardMaterial.hideFlags = HideFlags.DontSave;
                    billboardMaterial.SetTexture("_MainTex",
                        vegetationItemInfo.BillboardTexture);
                    billboardMaterial.SetTexture("_Bump",
                        vegetationItemInfo.BillboardNormalTexture);
                    billboardMaterial.SetFloat("_Cutoff", vegetationItemInfo.BillboardCutoff);
                    billboardMaterial.SetFloat("_Brightness", vegetationItemInfo.BillboardBrightness);
                    billboardMaterial.SetInt("_InRow",
                        BillboardAtlasRenderer.GetBillboardQualityColumnCount(vegetationItemInfo
                            .BillboardQuality));
                    billboardMaterial.SetInt("_InCol",
                        BillboardAtlasRenderer.GetBillboardQualityRowCount(vegetationItemInfo
                            .BillboardQuality));
                    billboardMaterial.SetInt("_CullDistance", 340);
                    billboardMaterial.SetColor("_HueVariation", new Color(1f, 0.5f, 0f, 25f / 256f));
                    billboardMaterial.SetColor("_Color", vegetationItemInfo.BillboardTintColor);
                    billboardMaterial.DisableKeyword("AT_CAMERA_SHADER");
                    billboardMaterial.EnableKeyword("AT_CAMERA_MATERIAL");

                    //billboardMaterial.EnableKeyword("LOD_FADE_CROSSFADE");

                    if (vegetationItemInfo.ShaderType ==
                        VegetationShaderType.Speedtree)
                    {
                        billboardMaterial.EnableKeyword("AT_HUE_VARIATION_ON");
                        billboardMaterial.DisableKeyword("AT_HUE_VARIATION_OFF");
                    }
                    else
                    {
                        billboardMaterial.DisableKeyword("AT_HUE_VARIATION_ON");
                        billboardMaterial.EnableKeyword("AT_HUE_VARIATION_OFF");
                    }
                    _vegetationItemBillboardMaterialList.Add(billboardMaterial);
                }
                else
                {
                    _vegetationItemBillboardMaterialList.Add(null);
                }
        }

        public void MoveBillboardSystem(Vector3 offset)
        {
            BillboardCellQuadTree.Move(new Vector2(offset.x, offset.z));
           // Debug.Log(offset);

            for (int i = 0; i <= BillboardCellList.Count - 1; i++)
            {
                //if (ClearInvisibleCacheAtFloatingOrigin && !BillboardCellList[i].IsVisible)
                //{
                //    BillboardCellList[i].ClearMeshes();
                //}
                //_moveVertexTempList.Clear();
                BillboardCellList[i].Move(offset);//, _moveVertexTempList);
            }
            SetupCullingGroup(false);
             _floatingOriginChanged = true;
    }

    public void ClearBillboards()
        {
            ClearBillboardCells();
        }

        // ReSharper disable once UnusedMember.Local
        void OnDestroy()
        {
            if (_cullingGroup != null)
            {
                _cullingGroup.Dispose();
                _cullingGroup = null;
            }
        }

        public void OnDisable()
        {
            if (VegetationSystem)
            {
                VegetationSystem.OnSetVegetationPackageDelegate -= OnSetVegetationPackage;
                //VegetationSystem.OnVegetationCellClearCacheDelegate -= OnVegetationCellClearCache;
                VegetationSystem.OnRefreshVegetationBillboardsDelegate -= OnRefreshVegetationBillboards;

                if (Application.isPlaying)
                {
                    VegetationSystem.OnClearCacheDelegate -= OnVegetationClearCache;
                }
            }

            if (_cullingGroup != null)
            {
                _cullingGroup.Dispose();
                _cullingGroup = null;
            }

            ClearBillboardMaterials();
            DisableEditorApi();
        }

        public void OnVegetationSystemInitComplete()
        {
            if (gameObject.activeSelf) CreateBillboards();
        }

        public void OnVegetationDistanceChange()
        {
            if (gameObject.activeSelf) SetupCullingGroup();
        }

        private void ClearBillboardCells()
        {
            for (int i = 0; i <= BillboardCellList.Count - 1; i++)
            {
                BillboardCellList[i].ClearMeshGenetators();
            }

            BillboardCellList.Clear();
            PotentialVisibleBillboardCellList.Clear();
            VisibleBillboardCellList.Clear();
        }

        public int GetBillboardCount()
        {
            int billboardCount = 0;
            for (int i = 0; i <= BillboardCellList.Count - 1; i++)
                for (int j = 0; j <= BillboardCellList[i].BillboardMeshGeneratorList.Count - 1; j++)
                    billboardCount += BillboardCellList[i].BillboardMeshGeneratorList[j].QuadCount;


            return billboardCount;
        }

        private float GetWorldSpaceMinTreeHeight()
        {
            float minTreeHeight = 0;
            if (VegetationSystem)
                for (int i = 0; i <= VegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
                {
                    VegetationItemInfo vegetationItemInfo = VegetationSystem.currentVegetationPackage.VegetationInfoList[i];
                    if (vegetationItemInfo.VegetationType == VegetationType.Tree && vegetationItemInfo.UseHeightLevel)// && vegetationInfo.IncludeInTerrain)
                        if (vegetationItemInfo.MinimumHeight < minTreeHeight) minTreeHeight = vegetationItemInfo.MinimumHeight;
                }

            return minTreeHeight +// VegetationSystem.unityTerrainData.terrainPosition.y +
                   VegetationSystem.GetWaterLevel();
        }

        private float GetWorldSpaceMaxTreeHeight()
        {
            float maxTreeHeight = 0;
            if (VegetationSystem)
                for (int i = 0; i <= VegetationSystem.currentVegetationPackage.VegetationInfoList.Count - 1; i++)
                {
                    VegetationItemInfo vegetationItemInfo = VegetationSystem.currentVegetationPackage.VegetationInfoList[i];
                    if (vegetationItemInfo.VegetationType == VegetationType.Tree && vegetationItemInfo.UseHeightLevel) //&& vegetationInfo.IncludeInTerrain)
                        if (vegetationItemInfo.MaximumHeight > maxTreeHeight) maxTreeHeight = vegetationItemInfo.MaximumHeight;

                    if (vegetationItemInfo.VegetationType == VegetationType.Tree && !vegetationItemInfo.UseHeightLevel)// && vegetationInfo.IncludeInTerrain)
                    {
                        maxTreeHeight = VegetationSystem.UnityTerrainData.MaxTerrainHeight;
                        break;
                    }
                }

            if (maxTreeHeight > VegetationSystem.UnityTerrainData.MaxTerrainHeight)
                maxTreeHeight = VegetationSystem.UnityTerrainData.MaxTerrainHeight;

            return maxTreeHeight + //+ VegetationSystem.unityTerrainData.terrainPosition.y 
                   VegetationSystem.GetWaterLevel();
        }

        public void SetupBillboardCells()
        {
            if (!VegetationSystem) return;
            if (VegetationSystem.UnityTerrainData == null) return;

            Random.InitState(VegetationSystem.vegetationSettings.RandomSeed);
            ClearBillboardCells();

            BillboardCellQuadTree =
                new QuadTree<BillboardCell>(VegetationSystem.UnityTerrainData.GetTerrainRect(CellSize));

            int cellXCount = Mathf.CeilToInt(VegetationSystem.UnityTerrainData.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(VegetationSystem.UnityTerrainData.size.z / CellSize);

            float worldSpaceMaxTreeHeight = GetWorldSpaceMaxTreeHeight();
            float worldSpaceMinTreeHeight = GetWorldSpaceMinTreeHeight();
            float yExtent = (worldSpaceMaxTreeHeight + worldSpaceMinTreeHeight) / 2f;

            for (int x = 0; x <= cellXCount - 1; x++)
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    BillboardCell billboardCell = new BillboardCell
                    {
                        CellCorner = new Vector3(
                            VegetationSystem.UnityTerrainData.terrainPosition.x + CellSize * x,
                            VegetationSystem.UnityTerrainData.terrainPosition.y,
                            VegetationSystem.UnityTerrainData.terrainPosition.z + CellSize * z),
                        CellSize = new Vector3(CellSize, yExtent * 2, CellSize),
                        VegetationCellSize = VegetationSystem.CellSize,
                        CurrentvegetationPackage = VegetationSystem.currentVegetationPackage,
                    };

                    billboardCell.Init();
                    BillboardCellList.Add(billboardCell);
                    BillboardCellQuadTree.Insert(billboardCell);
                }
        }

        public void SetupCullingGroup(bool clearVisibleList = true)
        {
            if (_cullingGroup != null)
            {
                _cullingGroup.Dispose();
                _cullingGroup = null;

                if (clearVisibleList)
                {
                    for (int i = 0; i <= BillboardCellList.Count - 1; i++)
                    {
                        BillboardCellList[i].SetVisible(false);
                        VisibleBillboardCellList.Clear();
                    }
                }
            }

            Camera targetCamera = VegetationSystem.GetCurrentCamera();
            _cullingGroup = new CullingGroup { targetCamera = targetCamera };
            if (targetCamera)
            {
                _cullingGroup.SetDistanceReferencePoint(VegetationSystem.GetCurrentCamera().transform);
            }

            Rect cullingAreaRect =
                VegetationSystem.GetCullingAreaRect(VegetationSystem.GetTotalDistance() + CellSize +
                                                    AdditonalCacheDistance);
            _level1CullingCameraPosition = targetCamera.transform.position;// VegetationSystem.GetCameraPosition();
            PotentialVisibleBillboardCellList = BillboardCellQuadTree.Query(cullingAreaRect);

            BoundingSphere[] spheres = new BoundingSphere[PotentialVisibleBillboardCellList.Count];
            for (int i = 0; i <= PotentialVisibleBillboardCellList.Count - 1; i++)
            {
                PotentialVisibleBillboardCellList[i].Init();
                spheres[i] = PotentialVisibleBillboardCellList[i].GetBoundingSphere();
            }
            _cullingGroup.SetBoundingSpheres(spheres);
            _cullingGroup.SetBoundingSphereCount(PotentialVisibleBillboardCellList.Count);
            _cullingGroup.onStateChanged = VisibilityStateChangedMethod;

            float[] distanceBands = new float[1];
            distanceBands[0] = VegetationSystem.GetVegetationDistance() + VegetationSystem.GetTreeDistance() +
                               VegetationSystem.GetBillboardDistance() + Mathf.Sqrt(CellSize * CellSize * 2f) / 2f;

            _cullingGroup.SetBoundingDistances(distanceBands);
        }

        // ReSharper disable once UnusedMember.Local
        private void LateUpdate()
        {
            if (!VegetationSystem) return;
            if (!VegetationSystem.InitDone) return;
            //ExecuteRenderBillboards();

            if (Vector3.Distance(_level1CullingCameraPosition, VegetationSystem.GetCameraPosition()) > CellSize)
                SetupCullingGroup();          
        }
      
        // ReSharper disable once UnusedMember.Local
        private void OnRenderObject()
        {
            if (!VegetationSystem.InitDone && VegetationSystem.UnityTerrainData == null) return;

            if (!Application.isPlaying)
                ExecuteRenderBillboards();
        }

        private void ExecuteRenderBillboards()
        {
            if (_cullingGroup == null) return;

#if UNITY_EDITOR
            if (_cullingGroup.targetCamera == null)
            {
                if (VegetationSystem.GetCurrentCamera()) SetupCullingGroup();
            }

            if (!Application.isPlaying && DisableInEditorMode) return;
#endif

            UpdateBillboardClipping();

            Profiler.BeginSample("ProcessVisibleCells");
            ProcessVisibleCells();
            Profiler.EndSample();

            if (RenderBillboards && VegetationSystem.RenderVegetation && VegetationSystem.enabled &&
                !VegetationSystem.GetSleepMode())
            {
                DrawVisibleCells();
            }

            if (_floatingOriginChanged)
            {
                VisibleBillboardCellList.Clear();
                _floatingOriginChanged = false;
            }
        }
        
        private void DrawVisibleCells()
        {
            Matrix4x4 translationMatrix = Matrix4x4.TRS(VegetationSystem.UnityTerrainData.terrainPosition,
                Quaternion.identity, Vector3.one);

            Profiler.BeginSample("Draw billboards");
            _currentCamera = null;

            LayerMask currentLayerMask = VegetationSystem.vegetationSettings.TreeLayer;
            if (OverrideLayer) currentLayerMask = BillboardLayer;

            for (int i = 0; i <= _vegetationItemBillboardMaterialList.Count - 1; i++)
            {
                if (_vegetationItemBillboardMaterialList[i] == null) continue;

                Material currentMaterial = _vegetationItemBillboardMaterialList[i];

                for (int j = 0; j <= VisibleBillboardCellList.Count - 1; j++)
                {
                    for (int l = 0; l <= VisibleBillboardCellList[j].BillboardMeshGeneratorList[i].MeshList.Count - 1; l++)
                    {
                        //Graphics.DrawMesh(VisibleBillboardCellList[j].BillboardMeshGeneratorList[i].MeshList[l],
                        //   _zeroMatrix4X4, currentMaterial, VegetationSystem.vegetationSettings.TreeLayer, _currentCamera,
                        //    0, _vegetationPropertyBlock, BillboardShadows, true);

                        Graphics.DrawMesh(VisibleBillboardCellList[j].BillboardMeshGeneratorList[i].MeshList[l],
                            translationMatrix, currentMaterial, currentLayerMask, _currentCamera,
                            0, _vegetationPropertyBlock, BillboardShadows, true);
                    }
                }
            }
            Profiler.EndSample();
        }

        private void ProcessVisibleCells()
        {
            for (int i = 0; i <= VisibleBillboardCellList.Count - 1; i++)
                if (!VisibleBillboardCellList[i].HasBillboards)
                {
                    Profiler.BeginSample("GenerateBillboardCell");
                    GenerateBillboardCell(VisibleBillboardCellList[i]);
                    Profiler.EndSample();
                }
        }

        private void VisibilityStateChangedMethod(CullingGroupEvent evt)
        {
            if (evt.currentDistance != evt.previousDistance)
                PotentialVisibleBillboardCellList[evt.index].DistanceBand = evt.currentDistance;

            if (evt.hasBecomeVisible)
            {
                PotentialVisibleBillboardCellList[evt.index].SetVisible(true);
                PotentialVisibleBillboardCellList[evt.index].DistanceBand = evt.currentDistance;
                VisibleBillboardCellList.Add(PotentialVisibleBillboardCellList[evt.index]);
            }

            if (evt.hasBecomeInvisible)
            {
                PotentialVisibleBillboardCellList[evt.index].SetVisible(false);
                PotentialVisibleBillboardCellList[evt.index].DistanceBand = evt.currentDistance;
                VisibleBillboardCellList.Remove(PotentialVisibleBillboardCellList[evt.index]);
            }

            PotentialVisibleBillboardCellList[evt.index].SetVisible(evt.isVisible);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDrawGizmos()
        {
            if (PotentialVisibleBillboardCellList != null)
                for (int i = 0; i <= PotentialVisibleBillboardCellList.Count - 1; i++)
                    if (ShowCellGrid)
                    {
                        Gizmos.color = PotentialVisibleBillboardCellList[i].IsVisible ? Color.white : Color.red;

                        Gizmos.DrawWireCube(PotentialVisibleBillboardCellList[i].CellBounds.center,
                            PotentialVisibleBillboardCellList[i].CellBounds.size);

                        BoundingSphere boundingSphere = PotentialVisibleBillboardCellList[i].GetBoundingSphere();
                        Gizmos.DrawWireSphere(boundingSphere.position, boundingSphere.radius);
                    }

            if (VegetationSystem)
                Gizmos.color = Color.green;
        }

        private void GenerateBillboardCell(BillboardCell billboardCell)
        {
            Vector3 terrainPosition = VegetationSystem.UnityTerrainData.terrainPosition;
            List<VegetationCell> intercectingVegetationCellList =
                VegetationSystem.VegetationCellQuadTree.Query(billboardCell.Rectangle);
            //VegetationSystem.SpawnCells(3, intercectingVegetationCellList);
            billboardCell.MaterialList = _vegetationItemBillboardMaterialList;
            billboardCell.AddVegetationCells(intercectingVegetationCellList, terrainPosition);
            billboardCell.GenerateBillboardMeshes(terrainPosition);
        }

        public void ClearBakedBillboards()
        {
            if (BakedBillboardsParent) DestroyImmediate(BakedBillboardsParent);

            enabled = true;
        }

        public void BakeBillboards(bool bakeToProject,bool clearBakedBillboardsOnBake,  Transform parent = null)
        {
            enabled = true;

#if UNITY_EDITOR
            if (clearBakedBillboardsOnBake)
            {
                if (BakedBillboardsParent) DestroyImmediate(BakedBillboardsParent);
            }

            string billboardID = System.Guid.NewGuid().ToString();
            string assetPath = "";

            if (bakeToProject)
            {
                if (!AssetDatabase.IsValidFolder("Assets/BakedBillboards"))
                {
                    AssetDatabase.CreateFolder("Assets", "BakedBillboards");
                }
                AssetDatabase.CreateFolder("Assets/BakedBillboards", "Billboard-" + billboardID);
                assetPath = "Assets/BakedBillboards/Billboard-" + billboardID + "/";
            }        

            BakedBillboardsParent = new GameObject { name = "BakedBillboards:" + billboardID };
            BakedBillboardsParent.transform.SetParent(parent, false);
            BakedBillboardsParent.transform.position = VegetationSystem.UnityTerrainData.terrainPosition;

            List<Material> bakedMaterialList = new List<Material>();
            for (int i = 0; i <= _vegetationItemBillboardMaterialList.Count - 1; i++)
            {
                if (bakeToProject && _vegetationItemBillboardMaterialList[i])
                {
                    string materialPath = assetPath + "Material_" + i + ".mat";
                    Material clonedMaterial = new Material(_vegetationItemBillboardMaterialList[i]);
                    AssetDatabase.CreateAsset(clonedMaterial, materialPath);
                    Material bakedMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    bakedMaterial.shader = Shader.Find("AwesomeTechnologies/Billboards/BakedGroupBillboards");
                    bakedMaterialList.Add(bakedMaterial);
                }
                else
                {
                    if (_vegetationItemBillboardMaterialList[i])
                    {
                        Material clonedMaterial = new Material(_vegetationItemBillboardMaterialList[i]);
                        clonedMaterial.shader = Shader.Find("AwesomeTechnologies/Billboards/BakedGroupBillboards");
                        bakedMaterialList.Add(clonedMaterial);
                    }
                    else
                    {
                        bakedMaterialList.Add(null);
                    }                 
                }
            }

            for (int i = 0; i <= BillboardCellList.Count - 1; i++)
            {
                GenerateBillboardCell(BillboardCellList[i]);
                for (int j = 0; j <= BillboardCellList[i].BillboardMeshGeneratorList.Count - 1; j++)
                {                    
                    for (int k = 0; k <= BillboardCellList[i].BillboardMeshGeneratorList[j].MeshList.Count - 1; k++)
                    {
                        GameObject billboardMeshObject =
                            new GameObject { name = i + "_" + j + "_" + k };
                        billboardMeshObject.transform.SetParent(BakedBillboardsParent.transform, false);

                        billboardMeshObject.layer = VegetationSystem.vegetationSettings.TreeLayer;
                        MeshFilter meshFilter = billboardMeshObject.AddComponent<MeshFilter>();

                        if (bakeToProject)
                        {
                            string meshPath = assetPath + "Mesh_" + i + "_" + j + "_" + k + ".asset";
                            Mesh clonedMesh = Instantiate(BillboardCellList[i].BillboardMeshGeneratorList[j].MeshList[k]);
                            AssetDatabase.CreateAsset(clonedMesh, meshPath);                            
                            meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
                        }
                        else
                        {
                            meshFilter.sharedMesh = Instantiate(BillboardCellList[i].BillboardMeshGeneratorList[j].MeshList[k]);
                        }
                     
                        MeshRenderer meshRenderer = billboardMeshObject.AddComponent<MeshRenderer>();
                        meshRenderer.shadowCastingMode = BillboardShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
                        meshRenderer.sharedMaterial = bakedMaterialList[j]; //bakeToProject ? savedMaterialList[j] : _vegetationItemBillboardMaterialList[j];
                    }
                }
            }

            enabled = false;

            //BakedBillboardController bakedBillboardController =
            //    BakedBillboardsParent.gameObject.GetComponent<BakedBillboardController>();

            //if (!bakedBillboardController)
            //{
            //    bakedBillboardController = BakedBillboardsParent.gameObject.AddComponent<BakedBillboardController>();
            //}

            //bakedBillboardController.VegetationSystem = VegetationSystem;
            //bakedBillboardController.BillboardSystem = this;

            //bakedBillboardController.AddMaterialList(_vegetationItemBillboardMaterialList);

#endif
        }
    }
}
