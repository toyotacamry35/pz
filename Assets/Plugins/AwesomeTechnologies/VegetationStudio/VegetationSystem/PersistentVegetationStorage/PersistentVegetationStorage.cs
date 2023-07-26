using System;
using System.Collections.Generic;
using System.Threading;
using AwesomeTechnologies.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Assets.Instancenator;
using AwesomeTechnologies.VegetationStudio;

namespace AwesomeTechnologies.Vegetation.PersistentStorage
{
    [Serializable]
    public enum PrecisionPaintingMode
    {
        Terrain,
        TerrainAndColliders,
        TerrainAndMeshes
    }

    public enum PaintMode
    {
        None,
        Paint,
        Precision,
        Edit,
    }

    [HelpURL("http://www.awesometech.no/index.php/persistent-vegetation-storage")]
    public class PersistentVegetationStorage : MonoBehaviour
    {
#if PERSISTENT_VEGETATION
        public PersistentVegetationStoragePackage PersistentVegetationStoragePackage;
        public const int blockCount = 8;

        public VegetationSystem VegetationSystem;
        public PaintMode mode = PaintMode.None;

        [NonSerialized]
        public int CurrentTabIndex;
        public int SelectedBrushIndex;
        public float BrushSize = 5;
        public float SampleDistance = 1f;
        public bool RandomizePosition = true;
        public bool PaintOnColliders;
        public bool UseSteepnessRules;
        public bool DisablePersistentStorage = false;

        public string SelectedEditVegetationID;
        public string SelectedPaintVegetationID;
        public string SelectedBakeVegetationID;
        public string SelectedStorageVegetationID;
        public string SelectedPrecisionPaintingVegetationID;
        public PrecisionPaintingMode PrecisionPaintingMode = PrecisionPaintingMode.TerrainAndMeshes;

        public bool AutoInitPersistentVegetationStoragePackage = false;

        public List<IVegetationImporter> VegetationImporterList = new List<IVegetationImporter>();
        public int SelectedImporterIndex;

        /// <summary>
        /// Tests if the persistent storage is initialized for the current terrain. 
        /// </summary>
        /// <param name="cellCount"></param>
        /// <returns></returns>
        public bool HasValidPersistentStorage(int cellCount)
        {
            //TUDO add terrainID and size to the test
            if (PersistentVegetationStoragePackage == null) return false;
            if (PersistentVegetationStoragePackage.PersistentVegetationCellList.Count != cellCount) return false;

            return true;
        }


#if UNITY_EDITOR


        [MenuItem("Level Design/Grass/Convert Grass Materials", priority = 50)]
        public static void ConvertMaterials()
        {


            VegetationSystem system = FindObjectOfType<VegetationSystem>();
            // for (int i=0; i<VegetationSystem.currentVegetationPackage)
            for (int i = 0; i < system.currentVegetationPackage.VegetationInfoList.Count; i++)
            {
                //Debug.Log(system.currentVegetationPackage.VegetationInfoList[i].Name);
                if (system.currentVegetationPackage.VegetationInfoList[i].sourceSecondMaterial == null)
                {
                    EditorUtility.DisplayProgressBar("Processing...", system.currentVegetationPackage.VegetationInfoList[i].Name, ((float)i / (float)system.currentVegetationPackage.VegetationInfoList.Count));

                    string sourceMaterialAssetPath = AssetDatabase.GetAssetPath(system.currentVegetationPackage.VegetationInfoList[i].sourceMaterials[0]);
                    string name = sourceMaterialAssetPath.Substring(0, sourceMaterialAssetPath.LastIndexOf('.'));
                    string suffix = name.Substring(name.Length - 5);

                    Material instMat = AssetDatabase.LoadAssetAtPath<Material>(name + "_inst.mat");
                    if (instMat == null)
                    {
                        Material mat = new Material(system.currentVegetationPackage.VegetationInfoList[i].sourceMaterials[0]);
                        if (mat.shader.name == "AwesomeTechnologies/AGrassStandard")
                        {
                            mat.shader = Shader.Find("Instancenator/InstancenatorAGrass");
                            //Debug.Log(system.currentVegetationPackage.VegetationInfoList[i].sourceMesh.name + " Create Grass Material");
                        }
                        else
                            if (mat.shader.name == "AwesomeTechnologies/AFoliageStandard" || mat.shader.name == "Instancenator/GrassShader")
                        {
                            mat.shader = Shader.Find("Instancenator/InstancenatorAFoliage");
                            //Debug.Log(system.currentVegetationPackage.VegetationInfoList[i].sourceMesh.name + " Create Foliage Material");
                        }
                        else
                        {
                            Debug.LogError(system.currentVegetationPackage.VegetationInfoList[i].sourceMesh.name + " Incorrect Source Mateiral");
                        }
                        name += "_inst.mat";
                        AssetDatabase.CreateAsset(mat, name);
                        EditorUtility.SetDirty(mat);
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                        AssetDatabase.SaveAssets();

                        instMat = AssetDatabase.LoadAssetAtPath<Material>(name + "_inst.mat");
                        system.currentVegetationPackage.VegetationInfoList[i].sourceSecondMaterial = instMat;
                        EditorUtility.SetDirty(system.currentVegetationPackage);
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        //Debug.Log(system.currentVegetationPackage.VegetationInfoList[i].sourceMesh.name + " Use Inst Material");
                        system.currentVegetationPackage.VegetationInfoList[i].sourceSecondMaterial = instMat;
                        EditorUtility.SetDirty(system.currentVegetationPackage);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Level Design/Grass/Bake Grass For PlayMode", priority = 20)]
        public static void BakeGrass()
        {
            PersistentVegetationStorage[] vs = FindObjectsOfType<PersistentVegetationStorage>();
            if (vs == null)
            {
                Debug.LogError("No Storages");
                return;
            }

            int ch = 0;
            foreach (PersistentVegetationStorage storage in vs)
            {
                EditorUtility.DisplayProgressBar("Processing...", storage.gameObject.name, (float)ch / (float)vs.Length);
                storage.Convert();
                ch++;
            }
            EditorUtility.ClearProgressBar();
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(vs[0].gameObject.scene);
            AssetDatabase.SaveAssets();
        }

        public InstanceComposition.Block CreateBlock(int hor, int ver, int element, Vector3 size, PersistentVegetationInstanceInfo info, List<InstanceComposition.InstanceData> instanceData)
        {
            InstanceComposition.Block block = new InstanceComposition.Block();
            Vector3 center = (size * 0.5f) / blockCount;
            center.x += hor * size.x / blockCount;
            center.z += ver * size.z / blockCount;
            center.y = VegetationSystem.currentTerrain.SampleHeight(transform.position + center);
            Vector3 blockSize = new Vector3(size.x / blockCount, Mathf.Max(size.x, size.z) / blockCount, size.z / blockCount);

            block.bounds = new Bounds(center, blockSize);
            block.instancesOffset = instanceData.Count;

            float maxDistance = size.magnitude * 2.0f;
            
            block.transitionOffset = Vector4.zero;// new Vector4(maxDistance * 0.1f, maxDistance * 0.2f, 0, 0);
            block.transitionDistance = Vector4.zero;// = new Vector4(maxDistance * 0.1f, maxDistance * 0.1f, 0, 0);            


            VegetationItemInfo vegetationItemInfo = VegetationSystem.currentVegetationPackage.GetVegetationInfo(info.VegetationItemID);
            int lod = 0;
            if (vegetationItemInfo.lods.Count > 0 && vegetationItemInfo.lods != null)
            {

                block.lods = new InstanceComposition.LOD[vegetationItemInfo.lods.Count + 1];
                block.lods[0].maxDistance = vegetationItemInfo.lods[0].LODDistance;

                for (lod = 0; lod < vegetationItemInfo.lods.Count; lod++)
                {
                    if (vegetationItemInfo.lods[lod] == null)
                        Debug.LogError(vegetationItemInfo.Name + " No LOD");

                    if (vegetationItemInfo.lods[lod].LODMesh == null)
                        Debug.LogError(vegetationItemInfo.Name + " No LOD Mesh");
                    else
                        block.lods[lod + 1].instanceMesh = vegetationItemInfo.lods[lod].LODMesh;

                    if (vegetationItemInfo.lods[lod].LODMaterial == null)
                        Debug.LogError(vegetationItemInfo.Name + " No LOD Material");
                    else
                        block.lods[lod + 1].instanceMaterial = vegetationItemInfo.lods[lod].LODMaterial;

                    if (lod < vegetationItemInfo.lods.Count - 1)
                        block.lods[lod + 1].maxDistance = vegetationItemInfo.lods[lod + 1].LODDistance;
                    else
                        block.lods[lod + 1].maxDistance = 100;

                    block.lods[lod + 1].isCastShadows = !vegetationItemInfo.DisableShadows;
                }
            }
            else
            {
                block.lods = new InstanceComposition.LOD[1];
                block.lods[0].maxDistance = 100;
            }

            block.transitionOffset[lod] = block.lods[lod].maxDistance * 0.8f;
            block.transitionDistance[lod] = block.lods[lod].maxDistance * 0.2f;
            if (lod > 0 && lod < block.lods.Length)
                block.transitionDistance[lod] = Mathf.Min(block.transitionDistance[lod], block.lods[lod].maxDistance - block.lods[lod - 1].maxDistance);

            block.lods[0].isCastShadows = !vegetationItemInfo.DisableShadows;
            block.lods[0].instanceMesh = vegetationItemInfo.sourceMesh;

            if (vegetationItemInfo.sourceSecondMaterial == null)
            {
                Debug.LogError(vegetationItemInfo.Name + " No Material");
            }
            block.lods[0].instanceMaterial = vegetationItemInfo.sourceSecondMaterial;

            Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            Vector3 max = Vector3.zero;


            foreach (PersistentVegetationCell cell in PersistentVegetationStoragePackage.PersistentVegetationCellList)
            {
                foreach (PersistentVegetationInfo persistentInfo in cell.PersistentVegetationInfoList)
                {
                    if (string.Equals(persistentInfo.VegetationItemID, vegetationItemInfo.VegetationItemID))
                    {
                        foreach (PersistentVegetationItem item in persistentInfo.VegetationItemList)
                        {
                            min = Vector3.Min(min, item.Scale);
                            max = Vector3.Max(max, item.Scale);
                        }
                    }

                }
            }

            block.valueMin = min;
            block.valueMax = max;
            Vector4 scaleDelta = block.valueMax - block.valueMin;
            scaleDelta.x = (scaleDelta.x > 0.001f) ? 1.0f / scaleDelta.x : 0.0f;
            scaleDelta.y = (scaleDelta.y > 0.001f) ? 1.0f / scaleDelta.y : 0.0f;
            scaleDelta.z = (scaleDelta.z > 0.001f) ? 1.0f / scaleDelta.z : 0.0f;
            scaleDelta.w = 0.0f;

            foreach (PersistentVegetationCell cell in PersistentVegetationStoragePackage.PersistentVegetationCellList)
            {
                foreach (PersistentVegetationInfo persistentInfo in cell.PersistentVegetationInfoList)
                {
                    if (string.Equals(persistentInfo.VegetationItemID, vegetationItemInfo.VegetationItemID))
                    {
                        foreach (PersistentVegetationItem item in persistentInfo.VegetationItemList)
                        {
                            if (block.bounds.Contains(item.Position))
                            {
                                Vector3 posWorld = item.Position - block.bounds.min;
                                posWorld.x /= block.bounds.size.x;
                                posWorld.y /= block.bounds.size.y;
                                posWorld.z /= block.bounds.size.z;

                                Vector4 value = Vector4.zero;
                                value.x = (item.Scale.x - block.valueMin.x) * scaleDelta.x;
                                value.y = (item.Scale.y - block.valueMin.y) * scaleDelta.y;
                                value.z = (item.Scale.z - block.valueMin.z) * scaleDelta.z;

                                instanceData.Add(InstanceComposition.InstanceData.Create(posWorld, item.Rotation, value, lod, 0));
                            }

                        }
                    }
                }
            }

            Shuffle(instanceData, block.instancesOffset, instanceData.Count - 1, new System.Random(DateTime.UtcNow.Millisecond));

            
            block.lods[0].instancesCount = instanceData.Count - block.instancesOffset;
            if (vegetationItemInfo.lods.Count > 0 && vegetationItemInfo.lods != null)
            {

                    for (int i = 0; i < vegetationItemInfo.lods.Count; i++)
                        block.lods[i + 1].instancesCount = instanceData.Count - block.instancesOffset;
                
            }
                
            return block;
        }

        bool isBlockExist(int hor, int ver, Bounds bounds, PersistentVegetationInstanceInfo info)
        {
            VegetationItemInfo vegetationItemInfo = VegetationSystem.currentVegetationPackage.GetVegetationInfo(info.VegetationItemID);

            foreach (PersistentVegetationCell cell in PersistentVegetationStoragePackage.PersistentVegetationCellList)
            {
                foreach (PersistentVegetationInfo persistentInfo in cell.PersistentVegetationInfoList)
                {
                    if (string.Equals(persistentInfo.VegetationItemID, vegetationItemInfo.VegetationItemID))
                    {
                        foreach (PersistentVegetationItem item in persistentInfo.VegetationItemList)
                        {
                            if (bounds.Contains(item.Position))
                            {
                                //Debug.Log(bounds.center + " " + bounds.size + " " + item.Position + " True");
                                return true;
                            }

                            //Debug.Log(bounds.center + " " + bounds.size + " " + item.Position + " False");
                        }
                    }

                }
            }

            return false;
        }

        [ContextMenu("Convert")]
        public void Convert()
        {
            if (VegetationSystem == default)
            {
                Debug.LogError($"No {nameof(VegetationSystem)} found for {gameObject}", gameObject);
                return;
            }
            if (VegetationSystem.currentTerrain == default)
            {
                Debug.LogError($"No {nameof(VegetationSystem.currentTerrain)} found for {gameObject}", gameObject);
                return;
            }
            InstanceComposition ic = GetOrMakeAsset(VegetationSystem.currentTerrain);
            if (ic == null)
                return;

            Vector3 size = VegetationSystem.currentTerrain.terrainData.size;
            int hmWidth = VegetationSystem.currentTerrain.terrainData.heightmapResolution;
            int hmHeight = VegetationSystem.currentTerrain.terrainData.heightmapResolution;

            
            List<InstanceComposition.Block> newBlocks = new List<InstanceComposition.Block>();

            List<PersistentVegetationInstanceInfo> info = PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();
            // dirty hack copied from PersistentVegetationStorageEditor_colonyImplementation.DrawStoredVegetationInspectorStored()
            int fullCount = info.Count;
            for (int i = 0; i < fullCount; i++)
            {
                VegetationItemInfo itemInfo = VegetationSystem.currentVegetationPackage.GetVegetationInfo(info[i].VegetationItemID);
                if (itemInfo == null)
                    PersistentVegetationStoragePackage.RemoveVegetationItemInstances(info[i].VegetationItemID);
            }
            info = PersistentVegetationStoragePackage.GetPersistentVegetationInstanceInfoList();

            int blocksVariants = PersistentVegetationStoragePackage.PersistentVegetationInstanceInfoList.Count;
            List<InstanceComposition.InstanceData> instanceData = new List<InstanceComposition.InstanceData>();

            for (int hor = 0; hor < blockCount; hor++)
                for (int ver = 0; ver < blockCount; ver++)
                {
                    //int realblock = (hor * blockCount + ver) * blocksVariants + i;
                    Vector3 center = (size * 0.5f) / blockCount;
                    center.x += hor * size.x / blockCount;
                    center.z += ver * size.z / blockCount;
                    center.y = VegetationSystem.currentTerrain.SampleHeight(transform.position + center);
                    //Bounds bounds  = new Bounds(center, size / blockCount);

                    Vector3 blockSize = new Vector3(size.x / blockCount, Mathf.Max(size.x, size.z) / blockCount, size.z / blockCount);

                    Bounds bounds = new Bounds(center, blockSize);

                    for (int i = 0; i < blocksVariants; i++)
                    {
                        if (isBlockExist(hor, ver, bounds, info[i]))
                        {
                            newBlocks.Add(CreateBlock(hor, ver, i, size, info[i], instanceData));
                        }
                    }
                }

            ic.blocks = newBlocks.ToArray();
            ic.instances = instanceData.ToArray();
            if (ic.blocks.Length != 0)
            {
                ic.bounds = ic.blocks[0].bounds;
                for (int i = 1; i < ic.blocks.Length; i++)
                {
                    ic.bounds.Encapsulate(ic.blocks[i].bounds);
                }
            }

            EditorUtility.SetDirty(ic);
            AssetDatabase.SaveAssets();

            InstanceCompositionRenderer grassRenderer = gameObject.GetComponent<InstanceCompositionRenderer>();
            if (grassRenderer == null)
                grassRenderer = gameObject.AddComponent<InstanceCompositionRenderer>();

            grassRenderer.composition = ic;
            grassRenderer.isEnableDrawInEditor = false;

        }

        private static List<T> Shuffle<T>(List<T> list, int startIndex, int endIndex, System.Random rng)
        {
            if (startIndex < 0 || endIndex < 0 || startIndex >= list.Count || endIndex >= list.Count)
                throw new ArgumentOutOfRangeException();
            if (startIndex > endIndex)
                throw new ArgumentException("startIndex should be greater or equal than endIndex");
            if (startIndex == endIndex)
                return list;
            int n = endIndex - startIndex;
            for (int i = n; i > 0; --i)
            {
                var rnd = rng.Next(i);
                var temp = list[i + startIndex];
                list[i + startIndex] = list[rnd + startIndex];
                list[rnd + startIndex] = temp;
            }
            return list;
        }

        private InstanceComposition GetOrMakeAsset(Terrain terrain)
        {
            const string assetExt = ".asset";
            string prefixPath = AssetDatabase.GetAssetOrScenePath(terrain.terrainData);
            if (prefixPath.EndsWith(assetExt))
            {
                prefixPath = prefixPath.Remove(prefixPath.Length - assetExt.Length, assetExt.Length);
            }
            string prefixName = prefixPath;
            if (prefixName.LastIndexOf('/') >= 0)
            {
                prefixName = prefixName.Remove(0, prefixName.LastIndexOf('/') + 1);
            }

            const string postfix = "_filling";
            string assetPath = prefixPath + postfix + assetExt;
            InstanceComposition instanceComposition = AssetDatabase.LoadAssetAtPath<InstanceComposition>(assetPath);
            if (instanceComposition == null)
            {
                instanceComposition = ScriptableObject.CreateInstance<InstanceComposition>();
                AssetDatabase.CreateAsset(instanceComposition, assetPath);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                instanceComposition = AssetDatabase.LoadAssetAtPath<InstanceComposition>(assetPath);
            }

            return instanceComposition;
        }
#endif
        /// <summary>
        /// InitializePersistentStorage will clean the storage and set it up for the current VegetationSystem.
        /// </summary>
        public void InitializePersistentStorage()
        {
            if (PersistentVegetationStoragePackage != null)
            {
                PersistentVegetationStoragePackage.ClearPersistentVegetationCells();
                for (int i = 0; i <= VegetationSystem.VegetationCellList.Count - 1; i++)
                {
                    PersistentVegetationStoragePackage.AddVegetationCell();
                }
            }
        }

        public void InitializePersistentStorage(int cellCount)
        {
            if (PersistentVegetationStoragePackage != null)
            {
                PersistentVegetationStoragePackage.ClearPersistentVegetationCells();
                for (int i = 0; i <= cellCount - 1; i++)
                {
                    PersistentVegetationStoragePackage.AddVegetationCell();
                }
            }
        }

        /// <summary>
        /// AddVegetationItem will add a new instance of a Vegetation Item to the persistent storage. Position, scale and rotation is in worldspace. The Optional clearCellCache will refresh the area where the item is added. 
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="worldPosition"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="applyMeshRotation"></param>
        /// <param name="vegetationSourceID"></param>
        /// <param name="clearCellCache"></param>
        public void AddVegetationItemInstance(string vegetationItemID, Vector3 worldPosition, Vector3 scale, Quaternion rotation, bool applyMeshRotation, byte vegetationSourceID, bool clearCellCache = false)
        {

            if (!VegetationSystem || !PersistentVegetationStoragePackage) return;

#if UNITY_EDITOR
            if (ThreadUtility.MainThread == Thread.CurrentThread)
            {
                if (VegetationSystem.GetSleepMode() && !Application.isPlaying)
                {
                    Debug.LogWarning("You need to Start Vegetation Studio in order to use the Persistent storage API");
                    return;
                }
            }
#endif


            Rect positionRect = new Rect(new UnityEngine.Vector2(worldPosition.x, worldPosition.z), UnityEngine.Vector2.zero);

            VegetationItemInfo vegetationItemInfo =
                VegetationSystem.currentVegetationPackage.GetVegetationInfo(vegetationItemID);

            if (applyMeshRotation)
            {
                rotation *= Quaternion.Euler(vegetationItemInfo.RotationOffset);
            }

            List<VegetationCell> overlapCellList = VegetationSystem.VegetationCellQuadTree.Query(positionRect);


            Vector3 terrainPosition = VegetationSystem.UnityTerrainData.terrainPosition;


            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].CellIndex;
                if (clearCellCache)
                {
                    overlapCellList[i].ClearCache();
                    VegetationSystem.SetDirty();
                }

                PersistentVegetationStoragePackage.AddVegetationItemInstance(cellIndex, vegetationItemID, worldPosition - terrainPosition, scale, rotation, vegetationSourceID);
            }


            //VegetationStudioManager.Instance?.UpdateIcons(VegetationSystem);
        }



        public void AddVegetationItemInstanceEx(string vegetationItemID, Vector3 worldPosition, Vector3 scale, Quaternion rotation, byte vegetationSourceID, float minimumDistance, bool clearCellCache = false)
        {
            if (!VegetationSystem || !PersistentVegetationStoragePackage) return;

            Rect positionRect = new Rect(new UnityEngine.Vector2(worldPosition.x, worldPosition.z), UnityEngine.Vector2.zero);

            List<VegetationCell> overlapCellList = VegetationSystem.VegetationCellQuadTree.Query(positionRect);


            Vector3 terrainPosition = VegetationSystem.UnityTerrainData.terrainPosition;

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].CellIndex;
                if (clearCellCache)
                {
                    overlapCellList[i].ClearCache();
                    VegetationSystem.SetDirty();
                }


                PersistentVegetationStoragePackage.AddVegetationItemInstanceEx(cellIndex, vegetationItemID, worldPosition - terrainPosition, scale, rotation, vegetationSourceID, minimumDistance);
            }

        }

        public void RemoveVegetationItemInstance(string vegetationItemID, Vector3 origin, Vector3 worldPosition, float minimumDistance, bool clearCellCache = false)
        {
            if (!VegetationSystem || !PersistentVegetationStoragePackage) return;
            Rect positionRect = new Rect(new UnityEngine.Vector2(worldPosition.x, worldPosition.z), UnityEngine.Vector2.zero);

            List<VegetationCell> overlapCellList = VegetationSystem.VegetationCellQuadTree.Query(positionRect);

            Vector3 terrainPosition = VegetationSystem.UnityTerrainData.terrainPosition;

            for (int i = 0; i <= overlapCellList.Count - 1; i++)
            {
                int cellIndex = overlapCellList[i].CellIndex;
                if (clearCellCache)
                {
                    overlapCellList[i].ClearCache();
                    VegetationSystem.SetDirty();
                }

                PersistentVegetationStoragePackage.RemoveVegetationItemInstance(cellIndex, vegetationItemID, origin - terrainPosition, worldPosition - terrainPosition, minimumDistance);
            }

            //VegetationStudioManager.Instance?.UpdateIcons(VegetationSystem);
        }


        /// <summary>
        /// RepositionCellItems is used to check all instances of a VegetationItem in a cell and confirm that they are located in the correct cell. 
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <param name="id"></param>
        public void RepositionCellItems(int cellIndex, string id)
        {
            PersistentVegetationInfo persistentVegetationInfo = PersistentVegetationStoragePackage
                .PersistentVegetationCellList[cellIndex]
                .GetPersistentVegetationInfo(id);
            if (persistentVegetationInfo == null) return;

            List<PersistentVegetationItem> origialItemList = new List<PersistentVegetationItem>();
            origialItemList.AddRange(persistentVegetationInfo.VegetationItemList);
            persistentVegetationInfo.ClearCell();

            for (int i = 0; i <= origialItemList.Count - 1; i++)
            {
                AddVegetationItemInstance(id, origialItemList[i].Position + VegetationSystem.UnityTerrainData.terrainPosition, origialItemList[i].Scale,
                    origialItemList[i].Rotation, false, origialItemList[i].VegetationSourceID, true);
            }

            VegetationSystem.VegetationCellList[cellIndex].ClearCache();
            VegetationSystem.SetDirty();

            //VegetationStudioManager.Instance?.UpdateIcons(VegetationSystem);
        }

        /// <summary>
        /// Returns the numbers of cells in the persistent vegetation storage.
        /// </summary>
        /// <returns></returns>
        public int GetPersistentVegetationCellCount()
        {
            if (PersistentVegetationStoragePackage && PersistentVegetationStoragePackage.PersistentVegetationCellList != null)
            {
                return PersistentVegetationStoragePackage.PersistentVegetationCellList.Count;
            }

            return 0;
        }


        // ReSharper disable once UnusedMember.Local
        void Reset()
        {
            VegetationSystem = gameObject.GetComponent<VegetationSystem>();
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
                return;
            }

            VegetationItemInfo[] vegItemInfo = (VegetationSystem.currentVegetationPlacingData == null) ? VegetationSystem.currentVegetationPackage.VegetationInfoList.ToArray() : VegetationSystem.currentVegetationPlacingData.VegetationInfoList;
            for (int i = 0; i < VegetationSystem.currentVegetationPackage.VegetationInfoList.Count; i++)
            {
                VegetationSystem.currentVegetationPackage.VegetationInfoList[i].EnableRuntimeSpawn = false;
                vegItemInfo[i].EnableRuntimeSpawn = false;
            }
            VegetationSystem.SetDirty();
            VegetationSystem.DelayedClearVegetationCellCache();
        }
        /// <summary>
        /// ClearVegetationItem will remove any instanced of vegetation in the storage with the provided VegetationItemID and VegetationSourceID
        /// </summary>
        /// <param name="vegetationItemID"></param>
        /// <param name="vegetationSourceID"></param>
        public void RemoveVegetationItemInstances(string vegetationItemID, byte vegetationSourceID)
        {
            if (PersistentVegetationStoragePackage == null) return;
            PersistentVegetationStoragePackage.RemoveVegetationItemInstances(vegetationItemID, vegetationSourceID);

            //VegetationStudioManager.Instance?.UpdateIcons(VegetationSystem);
        }

        /// <summary>
        /// ClearVegetationItem will remove any instances of a VegetationItem from the storage. Items from all sourceIDs will be removed.
        /// </summary>
        /// <param name="vegetationItemID"></param>
        public void RemoveVegetationItemInstances(string vegetationItemID)
        {
            if (PersistentVegetationStoragePackage == null) return;
            PersistentVegetationStoragePackage.RemoveVegetationItemInstances(vegetationItemID);

            //VegetationStudioManager.Instance?.UpdateIcons(VegetationSystem);
        }

        /// <summary>
        /// BakeVegetationItem will bake all instances of a VegetationItem from the rules to the Persisitent Vegetation Storage. The original rule will set "Include in Terrain" to false.
        /// </summary>
        /// <param name="vegetationItemID"></param>
        public void BakeVegetationItem(string vegetationItemID)
        {


            if (!VegetationSystem) return;
            if (!VegetationSystem.currentVegetationPackage) return;

            int vegetationIndex = VegetationSystem.currentVegetationPackage.GetVegetationItemIndexFromID(vegetationItemID);


            if (vegetationItemID == "")
            {
                //Debug.Log("vegetationItemID empty");
                return;
            }

            VegetationSystem.SetItem(vegetationIndex).EnableRuntimeSpawn = true;

#if UNITY_EDITOR
            VegetationItemInfo vegetationItemInfo = VegetationSystem.GetVegetationInfo(vegetationItemID);
            if (!Application.isPlaying) EditorUtility.DisplayProgressBar("Bake vegetation item: " + vegetationItemInfo.Name, "Spawn all cells", 0);
#endif
            for (int i = 0; i <= VegetationSystem.VegetationCellList.Count - 1; i++)
            {
#if UNITY_EDITOR
                if (i % 100 == 0)
                {

                    if (!Application.isPlaying) EditorUtility.DisplayProgressBar("Bake vegetation item: " + vegetationItemInfo.Name, "Spawn cell " + i + "/" + (VegetationSystem.VegetationCellList.Count - 1), i / ((float)VegetationSystem.VegetationCellList.Count - 1));
                }
#endif
                List<Matrix4x4> vegetationItemList = VegetationSystem.VegetationCellList[i].DirectSpawnVegetation(vegetationItemID, false);

                for (int j = 0; j <= vegetationItemList.Count - 1; j++)
                {
                    Matrix4x4 vegetationItemMatrix = vegetationItemList[j];
                    AddVegetationItemInstance(vegetationItemID, MatrixTools.ExtractTranslationFromMatrix(vegetationItemMatrix),
                        MatrixTools.ExtractScaleFromMatrix(vegetationItemMatrix),
                        MatrixTools.ExtractRotationFromMatrix(vegetationItemMatrix), false, 0);
                }
            }
            VegetationSystem.SetItem(vegetationIndex).EnableRuntimeSpawn = false;
            VegetationSystem.ClearVegetationCellCache();
#if UNITY_EDITOR
            if (!Application.isPlaying) EditorUtility.ClearProgressBar();
#endif

        }

#endif
    }
}
