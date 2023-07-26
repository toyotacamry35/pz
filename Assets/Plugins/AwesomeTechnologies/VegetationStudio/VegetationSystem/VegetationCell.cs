using System.Collections.Generic;
using UnityEngine;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Utility.Quadtree;
using AwesomeTechnologies.Vegetation;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using UnityEngine.Profiling;

namespace AwesomeTechnologies
{
    public partial class VegetationCell : IHasRect
    {
        public Vector3 CellCorner;
        public Vector3 CellSize;
        public bool EdgeCell;
        public bool SeaCell;

        public VegetationCellCacheController VegetationCellCacheController;
        public int DistanceBand = 0;
        public int CellIndex = 0;
        public int RandomIndexOffset;
        public RandomNumberGenerator RandomNumberGenerator;

        public PersistentVegetationCell PersistentVegetationCell;

        public int TreeCount;
        public int GrassCount;

        public bool InitDone;
        public Bounds CellBounds;
        public bool IsVisible;

        public int DataLoadLevel = 5;

        private bool _refreshedData;
        public bool IsPlaying;

        public List<VegetationItemIndirectInfo> IndirectInfoList = new List<VegetationItemIndirectInfo>();
        public List<VegetationItemIndirectInfo> DeleteIndirectInfoList = new List<VegetationItemIndirectInfo>();

        public delegate void MultiSpawnVegetationDelegate(VegetationCell vegetationCell);
        public MultiSpawnVegetationDelegate OnSpawnVegetationDelegate;
        private readonly List<List<Matrix4x4>> _vegetationInfoList = new List<List<Matrix4x4>>();
        private readonly List<CustomList<Matrix4x4>> _vegetationIndirectInfoList = new List<CustomList<Matrix4x4>>();

        //public delegate void MultionClearCellCacheDelegate(VegetationCell vegetationCell);
        //public MultionClearCellCacheDelegate OnClearCellCacheDelegate;

        private readonly List<BaseMaskArea> _vegetationMaskList = new List<BaseMaskArea>();

        private VegetationSettings _currentvegetationSettings;
        public VegetationPackage CurrentvegetationPackage;

        public List<TextureMaskBase> RuntimeTextureMaskList;
        public UnityTerrainData UnityTerrainData;
        private int _seedCounter;

        public bool[,] SpawnLocations;
        public int SpawnLocationSize;

        //public int FrustumKernelHandle;
        //public ComputeShader FrusumMatrixShader;

        //public ManualResetEvent ManualResetEvent;
        //public WaitCallback WaitCallback;


        //public VegetationCell()
        //{
        //    WaitCallback = (obj) =>
        //    {
        //        Preprocess();
        //        ManualResetEvent.Set();
        //    };
        //}

        public void AddVegetationMask(BaseMaskArea maskArea)
        {
            if (_vegetationMaskList.Contains(maskArea)) return;

            _vegetationMaskList.Add(maskArea);
            maskArea.OnMaskDeleteDelegate += OnMaskDelete;
            ClearCache();
        }

        public void RemoveVegetationMask(BaseMaskArea maskArea)
        {
            if (_vegetationMaskList.Remove(maskArea))
            {
                if (DataLoadLevel < 5) ClearCache();
            }
        }

        private void OnMaskDelete(BaseMaskArea maskArea)
        {
            maskArea.OnMaskDeleteDelegate -= OnMaskDelete;
            if (_vegetationMaskList.Remove(maskArea))
            {
                if (DataLoadLevel < 5) ClearCache();
            }
        }

        public bool IsTreeOrLargeObject(int index)
        {
            if (CurrentvegetationPackage.VegetationInfoList[index].VegetationType == VegetationType.Tree ||
                CurrentvegetationPackage.VegetationInfoList[index].VegetationType == VegetationType.LargeObjects)
            {
                return true;
            }

            return false;
        }

        public Vector3 GetCellCenter()
        {
            return CellBounds.center;
        }

        private int GetSeedCounter()
        {
            _seedCounter++;
            if (_seedCounter > 9999) _seedCounter = 0;
            return _seedCounter;
        }

        private void ResetSeedCounter(int offset)
        {
            _seedCounter = 0 + RandomIndexOffset + offset;          

            while (_seedCounter > 9999)
            {
                _seedCounter = _seedCounter - 10000;
            }
        }
        public void SetVisible(bool value)
        {
            IsVisible = value;
        }

        private Rect _rectangle;

        public Rect Rectangle
        {
            get
            {
                return _rectangle;
            }
            set
            {
                _rectangle = value;
            }
        }

        public void LoadVegetation(int overrideDistanceBand = -1)
        {
            
            if (overrideDistanceBand != -1)
            {
                if (overrideDistanceBand < DataLoadLevel) SpawnCellVegetation(overrideDistanceBand);
            }
            else
            {
                if (DistanceBand < DataLoadLevel) SpawnCellVegetation(DistanceBand);
            }
        }

        public List<Matrix4x4> DirectSpawnVegetation(string vegetationItemID, bool includePersistentStorage)
        {
            List<Matrix4x4> matrixList = new List<Matrix4x4>();
            CustomList<Matrix4x4> indirectMatrixList = new CustomList<Matrix4x4>();
            VegetationItemInfo vegetationItemInfo = GetVegetationInfo(vegetationItemID);

            if (vegetationItemInfo.VegetationType == VegetationType.Tree && vegetationItemInfo.UseCollisionDetection)
            {
                ClearSpawnLocations();

                int vegetationIndex = CurrentvegetationPackage.GetVegetationItemIndexFromID(vegetationItemID);
                for (int i = 0; i <= vegetationIndex - 1; i++)
                {                  
                    VegetationItemInfo treeVegetationItemInfo = SetItem(i);
                    if (treeVegetationItemInfo.VegetationType != VegetationType.Tree && treeVegetationItemInfo.VegetationType != VegetationType.LargeObjects) continue;

                    matrixList.Clear();
                    indirectMatrixList.Clear();
                    SpawnVegetation(treeVegetationItemInfo, _currentvegetationSettings, matrixList, indirectMatrixList, null, true, includePersistentStorage);
                }
                matrixList.Clear();
                indirectMatrixList.Clear();
            }

            SpawnVegetation(vegetationItemInfo, _currentvegetationSettings, matrixList, indirectMatrixList,null,true,includePersistentStorage);
            return matrixList;
        }
        public CustomList<Matrix4x4> DirectSpawnVegetationIndirect(string vegetationItemID, bool includePersistentStorage)
        {
            List<Matrix4x4> matrixList = new List<Matrix4x4>();
            CustomList<Matrix4x4> indirectMatrixList = new CustomList<Matrix4x4>();
            VegetationItemInfo vegetationItemInfo = GetVegetationInfo(vegetationItemID);

            if (vegetationItemInfo.VegetationType == VegetationType.Tree && vegetationItemInfo.UseCollisionDetection)
            {
                ClearSpawnLocations();

                int vegetationIndex = CurrentvegetationPackage.GetVegetationItemIndexFromID(vegetationItemID);
                for (int i = 0; i <= vegetationIndex - 1; i++)
                {
                    VegetationItemInfo treeVegetationItemInfo = SetItem(i);
                    if (treeVegetationItemInfo.VegetationType != VegetationType.Tree && treeVegetationItemInfo.VegetationType != VegetationType.LargeObjects) continue;

                    matrixList.Clear();
                    indirectMatrixList.Clear();
                    SpawnVegetation(treeVegetationItemInfo, _currentvegetationSettings, matrixList, indirectMatrixList, null, true, includePersistentStorage);
                }
                matrixList.Clear();
                indirectMatrixList.Clear();
            }

            SpawnVegetation(vegetationItemInfo, _currentvegetationSettings, matrixList, indirectMatrixList, null, true, includePersistentStorage);
            return indirectMatrixList;
        }

        public List<Matrix4x4> DirectSpawnVegetationLocalspace(string vegetationItemID, bool includePersistentStorage)
        {
            List<Matrix4x4> matrixList = DirectSpawnVegetation(vegetationItemID, includePersistentStorage);
            for (int i = 0; i <= matrixList.Count - 1; i++)
            {
                Vector3 position = MatrixTools.ExtractTranslationFromMatrix(matrixList[i]);
                Vector3 scale = MatrixTools.ExtractScaleFromMatrix(matrixList[i]);
                Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(matrixList[i]);
               matrixList[i] = Matrix4x4.TRS(position - UnityTerrainData.terrainPosition,rotation,scale);
            }
            return matrixList;
        }

        public bool NeedsLoadVegetation()
        {
            return (DistanceBand < DataLoadLevel);
        }

        public void PostProcess(bool useComputeShaders)
        {
            return;
            Profiler.BeginSample("Update Compute Buffer");
            int listcounter = IndirectInfoList.Count - 1;
            for (int i = 0; i <= listcounter; i++)
            {
                IndirectInfoList[i].UpdateComputeBuffer(useComputeShaders);
            }
            Profiler.EndSample();

            ClearDeleteIndirectInfoList();

            if (!_refreshedData) return;
            _refreshedData = false;

            if (OnSpawnVegetationDelegate != null) OnSpawnVegetationDelegate(this);
        }


        //public void ResetFrustrumArgBuffers()
        //{
        //    int listcounter = IndirectInfoList.Count - 1;
        //    for (int i = 0; i <= listcounter; i++)
        //    {
        //        IndirectInfoList[i].SetArgBuffers();
        //    }
        //}

        //public void DispatchGPUFrustumCulling()
        //{
        //    if (DistanceBand > 2) return;
        //    Profiler.BeginSample("DispatchGPUFrustumCulling");
        //    int listcounter = IndirectInfoList.Count - 1;
        //    for (int i = 0; i <= listcounter; i++)
        //    {
        //        IndirectInfoList[i].DispatchGPUFrustumCulling(DistanceBand);
        //    }
        //    Profiler.EndSample();
        //}

        public List<Matrix4x4> GetCurrentVegetationList(int index)
        {
            if (index > _vegetationInfoList.Count - 1) return null;

            if (DistanceBand < 3) return _vegetationInfoList[index];
            if (DistanceBand >= 3 && IsTreeOrLargeObject(index)) return _vegetationInfoList[index];
            return null;
        }

        public CustomList<Matrix4x4> GetCurrentIndirectVegetationList(int index)
        {
            if (index > _vegetationIndirectInfoList.Count - 1) return null;

            if (DistanceBand < 3) return _vegetationIndirectInfoList[index];
            if (DistanceBand >= 3 && IsTreeOrLargeObject(index)) return _vegetationIndirectInfoList[index];
            return null;
        }

        public void Setup(VegetationSettings vegetationSettings, VegetationPackage vegetationPackage, UnityTerrainData unityTerrainData, VegetationPlacingData _currentIndexPlacingData)
        {
            Vector3 cellExtent = CellSize / 2;
            cellExtent.y = 10f;
            Vector3 cellCenter = CellCorner + cellExtent;
            CellBounds = new Bounds(cellCenter, CellSize);
            _rectangle = RectExtension.CreateRectFromBounds(CellBounds);
            CurrentvegetationPackage = vegetationPackage;
            _currentvegetationSettings = vegetationSettings;
            UnityTerrainData = unityTerrainData;
            currentVegetationPlacingData = _currentIndexPlacingData;

            SpawnLocationSize = Mathf.RoundToInt(CellSize.x);
            SpawnLocations = new bool[SpawnLocationSize, SpawnLocationSize];
        }

        public void SetupBaked(VegetationSettings vegetationSettings, VegetationPackage vegetationPackage, UnityTerrainData unityTerrainData, VegetationPlacingData _currentIndexPlacingData)
        {
            CurrentvegetationPackage = vegetationPackage;
            _currentvegetationSettings = vegetationSettings;
            UnityTerrainData = unityTerrainData;
            currentVegetationPlacingData = _currentIndexPlacingData;

            SpawnLocationSize = Mathf.RoundToInt(CellSize.x);
            SpawnLocations = new bool[SpawnLocationSize, SpawnLocationSize];
        }

        public void ClearSpawnLocations()
        {
            int spawnLocationSize = Mathf.RoundToInt(CellSize.x);
            for (int x = 0; x <= spawnLocationSize - 1; x++)
            {
                for (int z = 0; z <= spawnLocationSize - 1; z++)
                {
                    SpawnLocations[x, z] = false;
                }
            }
        }
        public void SetSpawnLocation(Vector3 worldPosition)
        {
            int xIndex = Mathf.FloorToInt(worldPosition.x - CellCorner.x);
            int zIndex = Mathf.FloorToInt(worldPosition.z - CellCorner.z);
            xIndex = Mathf.Clamp(xIndex, 0, SpawnLocationSize - 1);
            zIndex = Mathf.Clamp(zIndex, 0, SpawnLocationSize - 1);

            SpawnLocations[xIndex, zIndex] = true;
        }

        public bool AvailableSpawnLocation(Vector3 worldPosition)
        {
            int xIndex = Mathf.FloorToInt(worldPosition.x - CellCorner.x);
            int zIndex = Mathf.FloorToInt(worldPosition.z - CellCorner.z);

            if (SampleSpawnLocation(xIndex, zIndex)) return false;

            if (SampleSpawnLocation(xIndex + 1, zIndex)) return false;
            if (SampleSpawnLocation(xIndex - 1, zIndex)) return false;
            if (SampleSpawnLocation(xIndex, zIndex + 1)) return false;
            if (SampleSpawnLocation(xIndex, zIndex - 1)) return false;
            if (SampleSpawnLocation(xIndex +1 , zIndex + 1)) return false;
            if (SampleSpawnLocation(xIndex -1, zIndex  -1)) return false;
            if (SampleSpawnLocation(xIndex + 1, zIndex - 1)) return false;
            if (SampleSpawnLocation(xIndex - 1, zIndex + 1)) return false;

            return true;
        }

        private bool SampleSpawnLocation(int xIndex, int zIndex)
        {
            xIndex = Mathf.Clamp(xIndex, 0, SpawnLocationSize - 1);
            zIndex = Mathf.Clamp(zIndex, 0, SpawnLocationSize - 1);

            return SpawnLocations[xIndex, zIndex];
        }
        public void Move(Vector3 offset)
        {
            CellCorner += offset;
            CellBounds.center += offset;
            _rectangle = RectExtension.CreateRectFromBounds(CellBounds);

            if (IsVisible)
            {
                MoveVegetationItems(_vegetationInfoList, offset);
                for (int i = 0; i <= IndirectInfoList.Count - 1; i++)
                {
                    if (IndirectInfoList[i] != null && IndirectInfoList[i].MatrixList != null)
                    {
                        MoveVegetationItems(IndirectInfoList[i].MatrixList, offset);
                        IndirectInfoList[i].SetDirty();
                    }                    
                }
            }
            else
            {
                ClearCache(false);
            }
        }

        private void MoveVegetationItems(List<List<Matrix4x4>> vegetationItemList, Vector3 offset)
        {
            for (int i = 0; i <= vegetationItemList.Count - 1; i++)
            {
                for (int j = 0; j <= vegetationItemList[i].Count - 1; j++)
                {
                    Vector3 position = MatrixTools.ExtractTranslationFromMatrix(vegetationItemList[i][j]);
                    Vector3 scale = MatrixTools.ExtractScaleFromMatrix(vegetationItemList[i][j]);
                    Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(vegetationItemList[i][j]);
                    vegetationItemList[i][j] = Matrix4x4.TRS(position + offset, rotation, scale);
                }
            }
        }

        private void MoveVegetationItems(CustomList<Matrix4x4> vegetationItemList, Vector3 offset)
        {
            for (int i = 0; i <= vegetationItemList.Count - 1; i++)
            {
                Vector3 position = MatrixTools.ExtractTranslationFromMatrix(vegetationItemList[i]);
                Vector3 scale = MatrixTools.ExtractScaleFromMatrix(vegetationItemList[i]);
                Quaternion rotation = MatrixTools.ExtractRotationFromMatrix(vegetationItemList[i]);
                vegetationItemList[i] = Matrix4x4.TRS(position + offset, rotation, scale);
            }
        }
        public void Init()
        {
            if (InitDone) return;
            CalculateMinMaxHeight();
            InitDone = true;
        }

        private void CreateIndirectInfoList()
        {
            ClearIndirectInfoList();

            for (int i = 0; i <= CurrentvegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                if (CurrentvegetationPackage.VegetationInfoList[i].IncludeDetailLayer < -1) continue;
                VegetationItemIndirectInfo vegetationItemIndirectInfo = new VegetationItemIndirectInfo(
                    CurrentvegetationPackage.VegetationInfoList[i].VegetationRenderType ==
                    VegetationRenderType.InstancedIndirect && Application.isPlaying)
                {
                    CurrentVegetationItemModelInfo = CurrentvegetationPackage.VegetationInfoList[i].VegetationItemModelInfo,
                    //FrustumKernelHandle = FrustumKernelHandle,
                    //FrusumMatrixShader = FrusumMatrixShader
                };
                IndirectInfoList.Add(vegetationItemIndirectInfo);
            }
        }

        private void SpawnCellVegetation(int distanceBand)
        {
            ClearCache(false);
            ClearSpawnLocations();
            CreateIndirectInfoList();

            VegetationItemInfo[] items = GetItemsInfo();

            int ch = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].IncludeDetailLayer < -1) continue;

                
                //if (GetItemsInfo()[i]
                //Profiler.BeginSample("Load " + GetItemsInfo()[i].Name);

                CustomList<Matrix4x4> indirectMatrixList = new CustomList<Matrix4x4>();
                List<Matrix4x4> matrixList = new List<Matrix4x4>();

                bool spawnVegetationType = true;
                if (distanceBand >= 3)
                {
                    if (items[i].VegetationType != VegetationType.LargeObjects && items[i].VegetationType != VegetationType.Tree)
                    {
                        spawnVegetationType = false;
                    }
                }

                if (spawnVegetationType)
                {
                    SpawnVegetation(items[i], _currentvegetationSettings, matrixList, indirectMatrixList, IndirectInfoList[ch]);
                }

                ch++;
                _vegetationInfoList.Add(matrixList);
                _vegetationIndirectInfoList.Add(indirectMatrixList);

                //Profiler.EndSample();
            }

            DataLoadLevel = distanceBand;
            _refreshedData = true;
        }

        public void ClearCache(bool callDelegate = true)
        {
            ClearIndirectInfoList();
            _vegetationInfoList.Clear();
            _vegetationIndirectInfoList.Clear();

            DataLoadLevel = 5;
            TreeCount = 0;
            GrassCount = 0;
        }

        public BoundingSphere GetBoundingSphere(float additionalDistance)
        {
            return new BoundingSphere(CellBounds.center, CellBounds.extents.magnitude + additionalDistance);
        }
        public void CalculateMinMaxHeight(float sampleDistanceFactor = 2f)
        {
            float sampleDistance = UnityTerrainData.heightmapScale.x * sampleDistanceFactor;
            sampleDistance = Mathf.Clamp(sampleDistance, 1, CellSize.x / 2f);

            int xSamples = Mathf.FloorToInt(CellSize.x / sampleDistance);
            int zSamples = Mathf.FloorToInt(CellSize.z / sampleDistance);

            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;

            for (int x = 0; x <= xSamples - 1; x++)
            {
                for (int z = 0; z <= zSamples - 1; z++)
                {
                    Vector3 samplePosition = new Vector3(CellCorner.x + x * sampleDistance, 0, CellCorner.z + z * sampleDistance);
                    float height = UnityTerrainData.terrain.SampleHeight(samplePosition);
                    if (height > maxHeight) maxHeight = height;
                    if (height < minHeight) minHeight = height;
                }
            }

            float middleHeight = (minHeight + maxHeight) / 2f;
            CellBounds.center = new Vector3(CellBounds.center.x, middleHeight + UnityTerrainData.terrainPosition.y, CellBounds.center.z);
            CellBounds.size = new Vector3(CellBounds.size.x, maxHeight - minHeight + 2f, CellBounds.size.z);
        }

        private void LoadPersistentVegetation(VegetationItemInfo vegetationItemInfo, List<Matrix4x4> matrixList, CustomList<Matrix4x4> indirectMatrixList, bool instancedIndirect)
        {
            if (PersistentVegetationCell == null) return;            

            PersistentVegetationInfo persistentVegetationInfo =
                PersistentVegetationCell.GetPersistentVegetationInfo(vegetationItemInfo.VegetationItemID);

            if (persistentVegetationInfo == null) return;

            for (int i = 0; i <= persistentVegetationInfo.VegetationItemList.Count - 1; i++)
            {

                Vector3 targetPosition = persistentVegetationInfo.VegetationItemList[i].Position +
                                         UnityTerrainData.terrainPosition;

                if (vegetationItemInfo.UseVegetationMasksOnStorage && ! vegetationItemInfo.UseVegetationMask)
                {
                    if (ExcludedByVegetationMask(vegetationItemInfo, targetPosition)) continue;
                }      

                if (instancedIndirect)
                {
                    indirectMatrixList.Add(Matrix4x4.TRS(targetPosition,
                        persistentVegetationInfo.VegetationItemList[i].Rotation,
                        persistentVegetationInfo.VegetationItemList[i].Scale));
                }
                else
                {
                    matrixList.Add(Matrix4x4.TRS(targetPosition,
                        persistentVegetationInfo.VegetationItemList[i].Rotation,
                        persistentVegetationInfo.VegetationItemList[i].Scale));
                }

                if ((vegetationItemInfo.VegetationType == VegetationType.Tree || vegetationItemInfo.VegetationType == VegetationType.LargeObjects) && vegetationItemInfo.UseCollisionDetection)
                {
                    SetSpawnLocation(persistentVegetationInfo.VegetationItemList[i].Position +
                                     UnityTerrainData.terrainPosition);
                }               
            }
        }

        bool ExcludedByVegetationMask(VegetationItemInfo vegetationItemInfo, Vector3 targetPosition)
        {
            for (int i = 0; i <= _vegetationMaskList.Count - 1; i++)
            {
                if (_vegetationMaskList[i].Contains(targetPosition, vegetationItemInfo.VegetationType, true, true))
                {
                    return true;
                }
            }
            return false;
        }

        private void SpawnVegetation(VegetationItemInfo vegetationItemInfo, VegetationSettings vegetationSettings, List<Matrix4x4> matrixList, CustomList<Matrix4x4> indirectMatrixList, VegetationItemIndirectInfo vegetationItemIndirectInfo, bool directSpawn = false, bool includePersistentStorage = true)
        {

            if (includePersistentStorage)
            {
                LoadPersistentVegetation(vegetationItemInfo, matrixList, indirectMatrixList, vegetationItemInfo.VegetationRenderType == VegetationRenderType.InstancedIndirect && Application.isPlaying);
            }

            if (!vegetationItemInfo.EnableRuntimeSpawn)
            {
                if (vegetationItemIndirectInfo != null && vegetationItemIndirectInfo.InstancedIndirect && Application.isPlaying && includePersistentStorage)
                {
                    vegetationItemIndirectInfo.AddMatrixList(indirectMatrixList);
                }

                return;
            }
            if (vegetationItemInfo.UseVegetationMask && _vegetationMaskList.Count == 0) return;

            if (!vegetationSettings.isRenderRuleInstances) return;
            //TUDO fix for cells under height level. Skip sea cells etc
            //if (!vegetationInfo.UseHeightLevel)
            //{
                // if (vegetationInfo.MaximumHeight < (this.cellBounds.center.y - this.cellBounds.extents.y)) return;
            //}

            float density = VegetationSettings.GetVegetationItemDensity(vegetationItemInfo.VegetationType, vegetationSettings);
            if (density < 0.01) return;

            float sampleDistance = Mathf.Clamp(vegetationItemInfo.SampleDistance / density, 0.1f, CellSize.x / 2f);
            float sampleIdealDistance = vegetationItemInfo.SampleDistance / density;
            float spawnChance = sampleDistance / sampleIdealDistance;

            float perlinNoiceScale = vegetationItemInfo.PerlinScale;

            int xSamples = Mathf.CeilToInt(CellSize.x / sampleDistance);
            int zSamples = Mathf.CeilToInt(CellSize.z / sampleDistance);           

            for (int x = 0; x <= xSamples; x++)
            {
                for (int z = 0; z <= zSamples; z++)
                {
                    ResetSeedCounter((x * xSamples + z) * 10 + vegetationItemInfo.Seed);
                    if (RandomCutoff(vegetationItemInfo.Density)) continue;
                    if (RandomCutoff(spawnChance)) continue;

                    Vector3 samplePosition = new Vector3(CellCorner.x + x * sampleDistance, 0,
                        CellCorner.z + z * sampleDistance);
                    if (vegetationItemInfo.RandomizePosition)
                    {
                        samplePosition +=
                            GetRandomOffset(sampleDistance / (2f / vegetationItemInfo.RandomPositionRelativeDistance));
                        if (!Rectangle.Contains(new Vector2(samplePosition.x, samplePosition.z))) continue;
                    }

                    if (vegetationItemInfo.VegetationType == VegetationType.Tree ||
                        vegetationItemInfo.VegetationType == VegetationType.LargeObjects)
                    {
                        if (vegetationItemInfo.UseCollisionDetection)
                        {
                            if (!AvailableSpawnLocation(samplePosition)) continue;
                        }
                    }

                    if (vegetationItemInfo.UsePerlinMask)
                    {
                        float perlinNoise = Mathf.PerlinNoise(
                            (samplePosition.x - UnityTerrainData.terrainPosition.x) / perlinNoiceScale +
                            vegetationItemInfo.PerlinOffset.x,
                            (samplePosition.z - UnityTerrainData.terrainPosition.z) / perlinNoiceScale +
                            vegetationItemInfo.PerlinOffset.y);
                        if (vegetationItemInfo.InversePerlinMask)
                        {
                            if ((1 - perlinNoise) < vegetationItemInfo.PerlinCutoff) continue;
                        }
                        else
                        {
                            if (perlinNoise < vegetationItemInfo.PerlinCutoff) continue;
                        }
                    }




                    var terrainLocalPos = samplePosition - UnityTerrainData.terrainPosition;

                    //var normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, UnityTerrainData.size.x, terrainLocalPos.x), Mathf.InverseLerp(0.0f, UnityTerrainData.size.z, terrainLocalPos.z));
                    Vector2 normalizedPos = new Vector2(terrainLocalPos.x * 1 / UnityTerrainData.size.x,
                        terrainLocalPos.z * 1 / UnityTerrainData.size.z);

                    if (vegetationItemInfo.UseIncludeDetailMaskRules)
                    {
                        if (!UnityTerrainData.HasDetailMap) continue;

                        int detailX = Mathf.RoundToInt(normalizedPos.x * (UnityTerrainData.detailMapWidth - 1));
                        int detailY = Mathf.RoundToInt(normalizedPos.y * (UnityTerrainData.detailMapHeight - 1));

                        int detailDensity =
                            UnityTerrainData.SampleDetailMap(detailX, detailY, vegetationItemInfo.IncludeDetailLayer);
                        float detailSpawnChance = detailDensity / 16f;
                        if (RandomCutoff(detailSpawnChance)) continue;
                    }

                    //Vector3 terrainNormal = UnityTerrainData.new_GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
                    //float height = UnityTerrainData.new_GetTriangleInterpolatedHeight(normalizedPos.x, normalizedPos.y) + UnityTerrainData.terrainPosition.y;
                    //Vector3 targetPosition = new Vector3(samplePosition.x, height, samplePosition.z);
                    Vector3 targetPosition = new Vector3(samplePosition.x, 0, samplePosition.z);

                    if (EdgeCell)
                    {
                        if (!UnityTerrainData.VegetationBounds.Contains(targetPosition)) continue;
                    }

                    float sizeScale = 1;
                    float densityScale = 1;


                    bool testVegetationMask = true;

                    if (vegetationItemInfo.UseVegetationMask)
                    {
                        testVegetationMask = false;
                        bool isMasked = false;
                        for (int i = 0; i <= _vegetationMaskList.Count - 1; i++)
                        {
                            if (_vegetationMaskList[i].ContainsMask(targetPosition, vegetationItemInfo.VegetationType, vegetationItemInfo.VegetationTypeIndex, ref sizeScale, ref densityScale))
                            {
                                if (_vegetationMaskList[i] is BeaconMaskArea) testVegetationMask = true;
                                isMasked = true;
                                break;
                            }
                        }
                        if (!isMasked) continue;
                    }

                    if (testVegetationMask)
                    {
                        bool isMasked = false;
                        for (int i = 0; i <= _vegetationMaskList.Count - 1; i++)
                        {
                            if (_vegetationMaskList[i].Contains(targetPosition, vegetationItemInfo.VegetationType, true, true))
                            {
                                isMasked = true;
                                break;
                            }
                        }
                        if (isMasked) continue;
                    }

                    if (densityScale < 0.99f)
                    {
                        float densityChance = RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 1);
                        if (densityChance > densityScale) continue;
                    }

                    float height = UnityTerrainData.new_GetTriangleInterpolatedHeight(normalizedPos.x, normalizedPos.y) + UnityTerrainData.terrainPosition.y;
                    targetPosition = new Vector3(samplePosition.x, height, samplePosition.z);


                    if (vegetationItemInfo.UseHeightLevel)
                    {
                        if (vegetationItemInfo.VegetationHeightType == VegetationHeightType.Simple)
                        {
                            float relativeHeight = height - vegetationSettings.WaterLevel;
                            if (relativeHeight < (vegetationItemInfo.MinimumHeight + UnityTerrainData.terrainPosition.y) || relativeHeight > (vegetationItemInfo.MaximumHeight + UnityTerrainData.terrainPosition.y)) continue;

                        }
                        else
                        {
                            float relativeHeight = height - vegetationSettings.WaterLevel;
                            float maximumCurveHeight = vegetationItemInfo.MaxCurveHeight;
                            if (vegetationItemInfo.AutomaticCurveMaxHeight)
                                maximumCurveHeight = UnityTerrainData.MaxTerrainHeight;

                            if (relativeHeight < (vegetationItemInfo.MinCurveHeight + UnityTerrainData.terrainPosition.y) || relativeHeight > (maximumCurveHeight + UnityTerrainData.terrainPosition.y)) continue;

                            float heightSpawnChance = SampleCurveArray(vegetationItemInfo.HeightCurveArray, relativeHeight,
                                UnityTerrainData.MaxTerrainHeight);
                            if (RandomCutoff(heightSpawnChance)) continue;
                        }
                    }

                    float textureMaskRuleScale = 1;

                    if (vegetationItemInfo.UseScaleTextueMaskRules)
                    {
                        float maxScale = 0;
                        bool hasValue = false;
                        for (int i = 0; i <= vegetationItemInfo.ScaleTextureMaskRuleList.Count - 1; i++)
                        {
                            TextureMaskBase runtimeTextureMask =
                                GetRuntimeTextureMask(vegetationItemInfo.ScaleTextureMaskRuleList[i].MaskId);
                            if (runtimeTextureMask == null) continue;

                            float maskScale = runtimeTextureMask.SampleTextureMaskScaled(normalizedPos, vegetationItemInfo.ScaleTextureMaskRuleList[i].TextureMaskPropertiesList);
                            maskScale *= vegetationItemInfo.ScaleTextureMaskRuleList[i].ScaleMultiplier;
                            if (maskScale > maxScale) maxScale = maskScale;
                            hasValue = true;
                        }

                        textureMaskRuleScale = hasValue ? maxScale : 1;
                       
                    }

                    if (vegetationItemInfo.UseDensityTextueMaskRules)
                    {
                        float densitySpawnChance = 0;
                        bool hasValue = false;
                        for (int i = 0; i <= vegetationItemInfo.DensityTextureMaskRuleList.Count - 1; i++)
                        {
                            TextureMaskBase runtimeTextureMask =
                                GetRuntimeTextureMask(vegetationItemInfo.DensityTextureMaskRuleList[i].MaskId);
                            if (runtimeTextureMask == null) continue;

                            float maskSpawnChance = runtimeTextureMask.SampleTextureMaskScaled(normalizedPos, vegetationItemInfo.DensityTextureMaskRuleList[i].TextureMaskPropertiesList);
                            maskSpawnChance *= vegetationItemInfo.DensityTextureMaskRuleList[i].DensityMultiplier;
                            if (maskSpawnChance > densitySpawnChance) densitySpawnChance = maskSpawnChance;
                            hasValue = true;
                        }
                        if (hasValue && RandomCutoff(densitySpawnChance)) continue;
                    }

                    Vector3 terrainNormal = UnityTerrainData.new_GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
                    var slopeCos = Vector3.Dot(terrainNormal, Vector3.up);
                    float slopeAngle = Mathf.Acos(slopeCos) * Mathf.Rad2Deg;

                    if (vegetationItemInfo.UseAngle)
                    {
                        if (vegetationItemInfo.VegetationSteepnessType == VegetationSteepnessType.Simple)
                        {
                            if (slopeAngle >= vegetationItemInfo.MaximumSteepness || slopeAngle < vegetationItemInfo.MinimumSteepness) continue;
                        }
                        else
                        {
                            float steepnessSpawnChance = SampleCurveArray(vegetationItemInfo.SteepnessCurveArray, slopeAngle, 90);
                            if (RandomCutoff(steepnessSpawnChance)) continue;
                        }
                    }

                    switch (vegetationItemInfo.Rotation)
                    {
                        case VegetationRotationType.RotateY:
                            targetPosition += new Vector3(0, -((slopeAngle / 90f) * 0.4f), 0);
                            break;
                        case VegetationRotationType.RotateXYZ:
                            targetPosition += new Vector3(0, -((slopeAngle / 90f) * 0.4f), 0);
                            break;
                    }

                    if (vegetationItemInfo.UseIncludeTextueMaskRules)
                    {
                        bool include = false;
                        for (int i = 0; i <= vegetationItemInfo.IncludeTextureMaskRuleList.Count - 1; i++)
                        {
                            TextureMaskBase runtimeTextureMask =GetRuntimeTextureMask(vegetationItemInfo.IncludeTextureMaskRuleList[i].MaskId);
                            if (runtimeTextureMask == null) continue;
                            
                                float densityValue = runtimeTextureMask.SampleTextureMaskScaled(normalizedPos, vegetationItemInfo.IncludeTextureMaskRuleList[i].TextureMaskPropertiesList);
                                if (densityValue >= vegetationItemInfo.IncludeTextureMaskRuleList[i].MinDensity &&
                                    densityValue <= vegetationItemInfo.IncludeTextureMaskRuleList[i].MaxDensity)
                                {
                                    include = true;
                                    break;
                                }
                                                        
                        }
                        if (!include) continue;
                    }
                    
                    if (vegetationItemInfo.UseExcludeTextueMaskRules)
                    {
                        bool exclude = false;
                        for (int i = 0; i <= vegetationItemInfo.ExcludeTextureMaskRuleList.Count - 1; i++)
                        {
                            TextureMaskBase runtimeTextureMask = GetRuntimeTextureMask(vegetationItemInfo.ExcludeTextureMaskRuleList[i].MaskId);
                            if (runtimeTextureMask == null) continue;
                            
                                float densityValue = runtimeTextureMask.SampleTextureMaskScaled(normalizedPos, vegetationItemInfo.ExcludeTextureMaskRuleList[i].TextureMaskPropertiesList);

                                if (densityValue >= vegetationItemInfo.ExcludeTextureMaskRuleList[i].MinDensity &&
                                    densityValue <= vegetationItemInfo.ExcludeTextureMaskRuleList[i].MaxDensity)
                                {
                                    exclude = true;
                                    break;
                                }                                                       
                        }
                        if (exclude) continue;
                    }
                    
                    if (vegetationItemInfo.UseIncludeTextueMask)
                    {
                        int alphaX = Mathf.RoundToInt(normalizedPos.x * (UnityTerrainData.alphamapWidth -1));
                        int alphaY = Mathf.RoundToInt(normalizedPos.y * (UnityTerrainData.alphamapHeight -1));
                        alphaX = Mathf.Clamp(alphaX, 0, UnityTerrainData.alphamapWidth -1);
                        alphaY = Mathf.Clamp(alphaY, 0, UnityTerrainData.alphamapHeight -1);
                        bool include = false;

                        for (int i = 0; i <= vegetationItemInfo.IncludeTextureMaskList.Count - 1; i++)
                        {
                            if (UnityTerrainData.alphamapLayers > vegetationItemInfo.IncludeTextureMaskList[i].TextureLayer)
                            {
                                float splatValue = UnityTerrainData.splatmapData[alphaY, alphaX, vegetationItemInfo.IncludeTextureMaskList[i].TextureLayer];
                                if (splatValue <= vegetationItemInfo.IncludeTextureMaskList[i].MaximumValue && splatValue >= vegetationItemInfo.IncludeTextureMaskList[i].MinimumValue)
                                {
                                    include = true;
                                    break;
                                }
                            }
                        }
                        if (!include) continue;
                    }
                    
                    if (vegetationItemInfo.UseExcludeTextueMask)
                    {
                        int alphaX = Mathf.RoundToInt(normalizedPos.x * (UnityTerrainData.alphamapWidth -1));
                        int alphaY = Mathf.RoundToInt(normalizedPos.y * (UnityTerrainData.alphamapHeight -1));
                        alphaX = Mathf.Clamp(alphaX, 0, UnityTerrainData.alphamapWidth - 1);
                        alphaY = Mathf.Clamp(alphaY, 0, UnityTerrainData.alphamapHeight - 1);
                        bool exclude = false;


                        for (int i = 0; i <= vegetationItemInfo.ExcludeTextureMaskList.Count - 1; i++)
                        {
                            if (UnityTerrainData.alphamapLayers > vegetationItemInfo.ExcludeTextureMaskList[i].TextureLayer)
                            {
                                float splatValue = UnityTerrainData.splatmapData[alphaY, alphaX, vegetationItemInfo.ExcludeTextureMaskList[i].TextureLayer];

                                if (splatValue <= vegetationItemInfo.ExcludeTextureMaskList[i].MaximumValue && splatValue > vegetationItemInfo.ExcludeTextureMaskList[i].MinimumValue)
                                {
                                    exclude = true;
                                    break;
                                }
                            }

                        }
                        if (exclude) continue;
                    }

                    

                    Vector3 angleScale = Vector3.zero;
                    Vector3 randomScaleVector;

                    if (vegetationItemInfo.VegetationScaleType == VegetationScaleType.Simple)
                    {
                        float randomScale = RandomNumberGenerator.RandomRange(GetSeedCounter(), vegetationItemInfo.MinScale, vegetationItemInfo.MaxScale);
                        randomScale *= sizeScale * vegetationSettings.VegetationScale;
                        randomScaleVector = new Vector3(randomScale, randomScale, randomScale);
                    }
                    else
                    {
                        randomScaleVector = Vector3.Lerp(vegetationItemInfo.MinVectorScale, vegetationItemInfo.MaxVectorScale, RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 2)) * sizeScale;
                    }

                    Vector3 lookAt;

                    Quaternion rotation = Quaternion.identity;
                    switch (vegetationItemInfo.Rotation)
                    {
                        case VegetationRotationType.RotateY:
                            rotation = Quaternion.Euler(new Vector3(0, RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 365f), 0));
                            break;
                        case VegetationRotationType.RotateXYZ:
                            rotation = Quaternion.Euler(new Vector3(RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 365f), RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 365f), RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 365f)));
                            break;
                        case VegetationRotationType.FollowTerrain:
                            lookAt = Vector3.Cross(-terrainNormal, Vector3.right);
                            // reverse it if it is down.
                            lookAt = lookAt.y < 0 ? -lookAt : lookAt;
                            // look at the hit's relative up, using the normal as the up vector
                            rotation = Quaternion.LookRotation(lookAt, terrainNormal);
                            //targetUp = Rotation * Vector3.up;
                            rotation *= Quaternion.AngleAxis(RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 365f), new Vector3(0, 1, 0));
                            break;
                        case VegetationRotationType.FollowTerrainScale:
                            {
                                lookAt = Vector3.Cross(-terrainNormal, Vector3.right);
                                // reverse it if it is down.
                                lookAt = lookAt.y < 0 ? -lookAt : lookAt;
                                // look at the hit's relative up, using the normal as the up vector
                                rotation = Quaternion.LookRotation(lookAt, terrainNormal);
                                //targetUp = Rotation * Vector3.up;
                                rotation *= Quaternion.AngleAxis(RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 365f), new Vector3(0, 1, 0));

                                float newScale = Mathf.Clamp01(slopeAngle / 45f);
                                angleScale = new Vector3(newScale, 0, newScale);
                            }
                            break;
                        case VegetationRotationType.FollowTerrainScaleWithBlock:
                            {
                                lookAt = Vector3.Cross(-terrainNormal, Vector3.right);
                                lookAt = lookAt.y < 0 ? -lookAt : lookAt;
                                rotation = Quaternion.LookRotation(lookAt, terrainNormal);
                                rotation *= Quaternion.AngleAxis(RandomNumberGenerator.RandomRange(GetSeedCounter(), -vegetationItemInfo.RotationBlock, vegetationItemInfo.RotationBlock), new Vector3(0, 1, 0));

                                float newScale = Mathf.Clamp01(slopeAngle / 45f);
                                angleScale = new Vector3(newScale, 0, newScale);
                            }
                            break;
                            //case VegetationRotationType.TiltTrees:
                            //rotation = Quaternion.Euler(new Vector3(0, NoiseGenerator.RandomRange(GetSeedCounter(), 0, 365f), 0));
                            //    rotation = Quaternion.Euler(45, NoiseGenerator.RandomRange(GetSeedCounter(), 0, 365f), 0);

                            //    break;
                    }

                    Quaternion initalRotation = Quaternion.Euler(vegetationItemInfo.RotationOffset);

                    float yScale = 1;

                    if (vegetationItemInfo.ShaderType == VegetationShaderType.Grass)
                    {
                        yScale = vegetationItemInfo.YScale;
                    }

                    Quaternion finalRotation =  rotation * initalRotation;
                    Vector3 finalScale = new Vector3((randomScaleVector.x + angleScale.x) * textureMaskRuleScale,
                        (randomScaleVector.y + angleScale.y) * yScale * textureMaskRuleScale,
                        (randomScaleVector.z + angleScale.z) * textureMaskRuleScale);
                    Vector3 finalOffset = new Vector3(vegetationItemInfo.Offset.x * finalScale.x, vegetationItemInfo.Offset.y * finalScale.y, vegetationItemInfo.Offset.z * finalScale.z);
                    Vector3 finalPosition = targetPosition + (finalRotation * finalOffset);

                    Matrix4x4 newMatrix = Matrix4x4.TRS(finalPosition, finalRotation, finalScale);

                    if (vegetationItemInfo.VegetationType == VegetationType.Tree || vegetationItemInfo.VegetationType == VegetationType.LargeObjects)
                    {
                        TreeCount++;
                    }
                    else
                    {
                        GrassCount++;
                    }

                    if (vegetationItemInfo.VegetationType == VegetationType.Tree ||
                        vegetationItemInfo.VegetationType == VegetationType.LargeObjects)
                    {
                        if (vegetationItemInfo.UseCollisionDetection)
                        {
                            SetSpawnLocation(samplePosition);
                        }
                    }

                    if (directSpawn)
                    {
                        matrixList.Add(newMatrix);
                    }
                    else
                    {
                        if (vegetationItemIndirectInfo != null && vegetationItemIndirectInfo.InstancedIndirect && Application.isPlaying)
                        {
                            indirectMatrixList.Add(newMatrix);
                        }
                        else
                        {
                            matrixList.Add(newMatrix);
                        }
                    }    
                }
            }

            if (vegetationItemIndirectInfo != null && vegetationItemIndirectInfo.InstancedIndirect && Application.isPlaying && !directSpawn)
            {
                vegetationItemIndirectInfo.AddMatrixList(indirectMatrixList);
            }
        }
       
        TextureMaskBase GetRuntimeTextureMask(string maskID)
        {
            for (int i = 0; i <= RuntimeTextureMaskList.Count - 1; i++)
            {
                if (RuntimeTextureMaskList[i].MaskID == maskID) return RuntimeTextureMaskList[i];
            }
            return null;
        }

        private float SampleCurveArray(float[] curveArray, float value, float maxValue)
        {
            if (curveArray.Length == 0) return 0f;

            int index = Mathf.RoundToInt((value / maxValue) * curveArray.Length);
            index = Mathf.Clamp(index, 0, curveArray.Length - 1);
            return curveArray[index];
        }

        private Vector3 GetRandomOffset(float distance)
        {
            return new Vector3(RandomNumberGenerator.RandomRange(GetSeedCounter(), -distance, distance), 0, RandomNumberGenerator.RandomRange(GetSeedCounter(), -distance, distance));
        }

        private bool RandomCutoff(float value)
        {
            float randomNumber = RandomNumberGenerator.RandomRange(GetSeedCounter(), 0, 1);
            return !(value > randomNumber);
        }

        private void ClearIndirectInfoList()
        {
            for (int i = 0; i <= IndirectInfoList.Count - 1; i++)
            {
                DeleteIndirectInfoList.Add(IndirectInfoList[i]);
            }

            IndirectInfoList.Clear();
        }

        private void ClearDeleteIndirectInfoList()
        {
            for (int i = 0; i <= DeleteIndirectInfoList.Count - 1; i++)
            {
                DeleteIndirectInfoList[i].OnDisable();
            }
            DeleteIndirectInfoList.Clear();
        }

        public void OnDisable()
        {
            ClearIndirectInfoList();
            ClearDeleteIndirectInfoList();
        }
    }
}
