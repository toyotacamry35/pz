using System;
using System.Collections.Generic;
using AwesomeTechnologies.Utility.Quadtree;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using AwesomeTechnologies.VegetationStudio;
using AwesomeTechnologies.Vegetation.PersistentStorage;

[System.Serializable]
public class LayerDetails
{
    public List<int> details;

    public LayerDetails()
    {
        details = new List<int>();
    }
    public LayerDetails(int[] array)
    {
        details = new List<int>();
        details.AddRange(array);
    }
}

[System.Serializable]
public class CustomDragData
{
    public int originalIndex = -1;
    public int originalLayer = -1;

    public CustomDragData(int _originalIndex, int _originalLayer)
    {
        originalIndex = _originalIndex;
        originalLayer = _originalLayer;
    }
}

public enum ArrayType
{
    Free = 0,
    Used = 1,
    Blocked = 2,
}

[System.Serializable]
public class ArrayCell
{
    public ArrayType arrayType;
    public GrassAtlasCell cell;

    public ArrayCell()
    {
        arrayType = ArrayType.Free;
    }
}



[System.Serializable]
public class GrassAtlasCell
{
    public int size_x = 1;
    public int size_y = 1;
    public UnityEngine.Object sourceGrass;
    public Texture2D tex2D;
    public Texture2D tex2D_EM;
    public Vector2 center;

    


    public int x = 0;
    public int y = 0;

    public GrassAtlasCell(int _x, int _y, int _sizeX, int _sizeY, int sizeAtlas)
    {
        x = _x;
        y = _y;
        size_x = _sizeX;
        size_y = _sizeY;

        float xStep = 1f / (float)sizeAtlas;


        float center_x = (float)_x * xStep + (xStep * _sizeX) / 2;

        float yStep = 1f / (float)sizeAtlas;

        float center_y = (float)_y * yStep + (yStep * _sizeY) / 2;


        center = new Vector2(center_x, 1f - center_y);

    }

    public void GetTexture()
    {
        GameObject grass = GameObject.Instantiate(sourceGrass) as GameObject;
        MeshRenderer MR = grass.GetComponent<MeshRenderer>();
        Material mat = MR.sharedMaterial;
        tex2D = (Texture2D)mat.mainTexture;
        tex2D_EM = (Texture2D)mat.GetTexture("_SpecTex03");
        GameObject.DestroyImmediate(grass);
    }
}

namespace AwesomeTechnologies.VegetationStudio
{
    [System.Serializable]
    public enum RTSize
    {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048
    }

    [System.Serializable]
    public class ItemIcons
    {
        public string guid;
        public Texture2D ico;

        public ItemIcons(string _guid, Texture2D _ico)
        {
            guid = _guid;
            ico = _ico;// new Texture2D(_ico.width, _ico.height);
            //ico.SetPixels(_ico.GetPixels());
        }
    }

    public partial class VegetationStudioManager : MonoBehaviour
    {
        public bool isEditedGrass = false;
        public static bool isAdvancedMode = false;

        public List<ItemIcons> itemIcons;
        public Terrain[] terrains;
        public int selectedVegetationSystem = 0;

        public bool isDraw = false;
        public int mode = 1;

        
        public bool isCreated = false;
        public int size = 3;
        public bool isSelected = false;
        public List<GrassAtlasCell> cells;
        public ArrayCell[] array;
        public GrassAtlasCell currentAtlasCell;
        public bool isDebugMaterial = false;
        
        public LayerDetails[] layerDetails;
        public LayerDetails other;
        public RTSize atlasSize = RTSize._512;

        public string atlasName;
        public string directory;

        public VegetationPlacingData[] currentVegetationPlacingDatas;

        

        public Vector3 oldViewPos;
        public Quaternion oldViewRot;

        public VegetationPackage context;

        public void UpdateTerrain()
        {
            for (int z = 0; z < terrains.Length; z++)
            {
                TerrainData terData = terrains[z].terrainData;
                int count = context.VegetationInfoList.Count;

                List<DetailPrototype> detailPrototypes = new List<DetailPrototype>();
                for (int i = 0; i < count; i++)
                {
                    DetailPrototype detailPrototype = new DetailPrototype();
                    detailPrototype.usePrototypeMesh = true;
                    detailPrototype.prototype = context.VegetationInfoList[i].VegetationPrefab;
                    detailPrototype.renderMode = DetailRenderMode.Grass;
                    detailPrototype.dryColor = Color.white;
                    detailPrototype.healthyColor = Color.white;
                    detailPrototypes.Add(detailPrototype);
                }

                terData.detailPrototypes = detailPrototypes.ToArray();
                terData.RefreshPrototypes();
            }
        }

        public void GetDependedTerrain()
        {
            terrains = transform.GetComponentsInChildren<Terrain>(false);
        }

#if UNITY_EDITOR
        public void LoadFromContextPreset(VegetationPackage ctx)
        {
            context = ctx;
            context.InitPackage();
            currentVegetationPlacingDatas = context.GetCurrentPackagePlacingData();

            for (int z = 0; z < VegetationSystemList.Count; z++)
            {
                VegetationSystemList[z].RefreshVegetationPackage();
            }

        }
#endif
        public static void SetVegetationCamera(Camera camera)
        {
            VegetationSystem[] vs = FindObjectsOfType<VegetationSystem>();
            for (int i = 0; i < vs.Length; i++)
            {
                //if (vs[i].InitDone)
                    vs[i].SetCamera(camera);
                //else
                //    vs[i].SelectedCamera = camera;
            }
        }

        /*
        public void AddIcon(VegetationItemInfo item)
        {
            for (int i = 0; i < itemIcons.Count; i++)
            {
                if (itemIcons[i].guid.Equals(item.VegetationGuid))
                {
                    if (itemIcons[i].ico == null)
                    {
                        itemIcons.Remove(itemIcons[i]);
                        ItemIcons itemIconNew = new ItemIcons(item.VegetationGuid, GetPrevievTexture(item));
                        itemIcons.Add(itemIconNew);
                    }
                    else
                        return;
                }  
            }

            ItemIcons itemIcon = new ItemIcons(item.VegetationGuid, GetPrevievTexture(item));
            itemIcons.Add(itemIcon);
        }

        /*
        public Texture2D GetIcon(VegetationItemInfo info)
        {
            for (int i=0; i<itemIcons.Count; i++)
            {
                if (itemIcons[i].guid.Equals(info.VegetationGuid))
                    return itemIcons[i].ico;
            }
            return null;
        }
        */

#if UNITY_EDITOR
        private static List<string> CreateVegetationInfoIdList(VegetationPackage vegetationPackage)
        {
            List<string> resultList = new List<string>();

            for (int i = 0; i <= vegetationPackage.VegetationInfoList.Count - 1; i++)
            {
                resultList.Add(vegetationPackage.VegetationInfoList[i].VegetationItemID);
            }
            return resultList;
        }
#endif

#if UNITY_EDITOR
        [MenuItem("Level Design/Grass/Bake All Grass", priority = 100)]
        public static void BakeAllStaticGrass()
        {
            VegetationStudioManager[] vsm = FindObjectsOfType<VegetationStudioManager>();
            //Debug.Log(vsm.Length);
            for (int i = 0; i< vsm.Length; i++)
            {
                vsm[i].BakeAllGrass();
            }
        }
#endif
        public void BakeAllGrass()
        {
#if UNITY_EDITOR
            for (int i = 0; i < VegetationSystemList.Count; i++)
            {
                VegetationSystemList[i].vegetationSettings.isRenderRuleInstances = true;
                VegetationSystemList[i].SetupVegetationCells();
                VegetationSystemList[i].SetupCullingGroup();

                VegetationSystemList[i].SetDirty();

                if (VegetationSystemList[i].InitDone)
                {
                    if (VegetationSystemList[i].OnSetVegetationPackageDelegate != null) VegetationSystemList[i].OnSetVegetationPackageDelegate(VegetationSystemList[i].currentVegetationPackage);
                }
#if PERSISTENT_VEGETATION
                PersistentVegetationStorage persistentVegetationStorage = VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                if (persistentVegetationStorage != null)
                {
                    if (persistentVegetationStorage.PersistentVegetationStoragePackage.PersistentVegetationCellList.Count == 0)
                        persistentVegetationStorage.InitializePersistentStorage();
                    List<string> vegetationItemIdList = CreateVegetationInfoIdList(persistentVegetationStorage.VegetationSystem.currentVegetationPackage);

                    for (int j = 0; j < vegetationItemIdList.Count; j++)
                    {
                        if (VegetationSystemList[i].GetVegetationInfo(vegetationItemIdList[j]).IncludeDetailLayer > -2)
                        {
                            persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[j], 0);
                            persistentVegetationStorage.BakeVegetationItem(vegetationItemIdList[j]);
                        }
                    }
                    persistentVegetationStorage.VegetationSystem.DelayedClearVegetationCellCache();
                    EditorUtility.SetDirty(persistentVegetationStorage.PersistentVegetationStoragePackage);
                    EditorUtility.SetDirty(persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    if (persistentVegetationStorage.VegetationSystem.currentVegetationPlacingData != null)
                        EditorUtility.SetDirty(persistentVegetationStorage.VegetationSystem.currentVegetationPlacingData);

                    VegetationSystemList[i].vegetationSettings.isRenderRuleInstances = false;
                    persistentVegetationStorage.DisablePersistentStorage = false;
                    persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                    EditorUtility.SetDirty(persistentVegetationStorage);
                    VegetationSystemList[i].SetDirty();
                }
#endif
            }
#endif
        }

        public void ReturnRealtimeGrass()
        {
#if UNITY_EDITOR
#if PERSISTENT_VEGETATION
            for (int i = 0; i < VegetationSystemList.Count; i++)
            {
                PersistentVegetationStorage persistentVegetationStorage = VegetationSystemList[i].GetComponent<PersistentVegetationStorage>();
                if (persistentVegetationStorage != null)
                {
                    List<string> vegetationItemIdList = CreateVegetationInfoIdList(persistentVegetationStorage.VegetationSystem.currentVegetationPackage);
                    for (int j = 0; j < vegetationItemIdList.Count; j++)
                        persistentVegetationStorage.RemoveVegetationItemInstances(vegetationItemIdList[j], 0);
                    persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                    EditorUtility.SetDirty(persistentVegetationStorage);
                }

                VegetationSystemList[i].vegetationSettings.isRenderRuleInstances = true;
                VegetationSystemList[i].SetupVegetationCells();
                VegetationSystemList[i].SetupCullingGroup();

                VegetationSystemList[i].SetDirty();

                if (VegetationSystemList[i].InitDone)
                {
                    if (VegetationSystemList[i].OnSetVegetationPackageDelegate != null) VegetationSystemList[i].OnSetVegetationPackageDelegate(VegetationSystemList[i].currentVegetationPackage);
                }

                persistentVegetationStorage.DisablePersistentStorage = false;
                persistentVegetationStorage.VegetationSystem.RefreshVegetationPackage();
                EditorUtility.SetDirty(persistentVegetationStorage);
                VegetationSystemList[i].SetDirty();
            }
#endif
#endif
        }

        public void AddToTerrains()
        {
#if UNITY_EDITOR
            for (int z = 0; z < terrains.Length; z++)
            {
                VegetationSystem vg = terrains[z].GetComponent<VegetationSystem>();

                if (vg == null)
                    vg = terrains[z].gameObject.AddComponent<VegetationSystem>();

                vg.AutoselectTerrain = false;
                vg.SetTerrain(terrains[z]);
                vg.VegetationPackageList = new List<VegetationPackage>();
                vg.AddVegetationPackage(context, true);
                vg.VegetationPackageList[vg.VegetationPackageIndex].LoadTexturesFromTerrain(terrains[z].terrainData);

                vg.LoadUnityTerrainDetails = true;
                vg.UseComputeShaders = true;
                vg.UseGPUCulling = true;

#if PERSISTENT_VEGETATION
                PersistentVegetationStorage ps = terrains[z].GetComponent<PersistentVegetationStorage>();

                if (ps == null)
                    ps = terrains[z].gameObject.AddComponent<PersistentVegetationStorage>();

                PersistentVegetationStoragePackage asset = ScriptableObject.CreateInstance<PersistentVegetationStoragePackage>();
                string terPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(terrains[z].terrainData));
                string terName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(terrains[z].terrainData));


                AssetDatabase.CreateAsset(asset, terPath + "/" + terName + "_GrassTerrainData.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                ps.PersistentVegetationStoragePackage = asset;

                ps.InitializePersistentStorage();

                ps.VegetationSystem.RefreshVegetationPackage();
#endif
                if (!VegetationSystemList.Contains(vg))
                {
                    VegetationSystemList.Add(vg);
                }

                vg.Enable();
            }
#endif
        }

#if UNITY_EDITOR
        public void Reset()
        {
            if (context != null)
                LoadFromContextPreset(context);

        }
#endif

        /*
        public static Texture2D GetPrevievTexture(VegetationItemInfo source)
        {
#if UNITY_EDITOR

            if (source.VegetationType != VegetationType.Grass && source.VegetationType != VegetationType.Plant)
            {
                Texture2D tex2d = AssetPreview.GetAssetPreview(source.VegetationPrefab);
                tex2d.alphaIsTransparency = true;
                Debug.Log(source.VegetationPrefab.name + " " + tex2d.GetPixels().Length);
                return tex2d;
            }

            GameObject grass = Instantiate(source.VegetationPrefab) as GameObject;
            MeshRenderer MR = grass.GetComponentInChildren<MeshRenderer>();
            Material mat = MR.sharedMaterial;
            Texture2D tex2D = (Texture2D)mat.mainTexture;

            int curWidth = tex2D.width;
            int curHeight = tex2D.height;

            MeshFilter MF = grass.GetComponentInChildren<MeshFilter>();
            Vector4 max = new Vector4(5, -5, 5, -5);
            Vector2[] uv = MF.sharedMesh.uv;
            int count = MF.sharedMesh.vertexCount;
            for (int i = 0; i < MF.sharedMesh.vertexCount; i++)
            {
                if (uv[i].x < max.x)
                    max.x = uv[i].x;
                if (uv[i].x > max.y)
                    max.y = uv[i].x;
                if (uv[i].y < max.z)
                    max.z = uv[i].y;
                if (uv[i].y > max.w)
                    max.w = uv[i].y;
            }

            int x = Mathf.Abs((int)(max.x * curWidth));
            int blockX = Mathf.Abs((int)(max.y * curWidth)) - x;
            blockX = Mathf.Min(blockX, curWidth - x);

            int y = Mathf.Abs((int)(max.z * curHeight));
            int blockY = Mathf.Abs((int)(max.w * curHeight)) - y;
            blockY = Mathf.Min(blockY, curHeight - y);

            Texture2D tex = new Texture2D(blockX, blockY);

            Color[] cols = tex2D.GetPixels(x, y, blockX, blockY);
            tex.SetPixels(0, 0, blockX, blockY, cols);
            tex.Apply();



            DestroyImmediate(grass);
            return tex;
#else
            return null;
#endif

        }

    */
        public void Create()
        {
            GetDependedTerrain();

            isCreated = true;
            array = new ArrayCell[size * size];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new ArrayCell();
            }
            cells = new List<GrassAtlasCell>();
            Names();

            isSelected = false;
            currentAtlasCell = null;
        }

        public Texture2D GetTextureFromObject(UnityEngine.Object obj)
        {
            GameObject grass = GameObject.Instantiate(obj) as GameObject;
            MeshRenderer MR = grass.GetComponent<MeshRenderer>();
            Material mat = MR.sharedMaterial;
            Texture2D tex2D = (Texture2D)mat.mainTexture;
            GameObject.DestroyImmediate(grass);
            return tex2D;
        }

        public void Names()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            int index = sceneName.IndexOf("_");
            if (index >= 0)
            {
                atlasName = sceneName.Substring(0, sceneName.IndexOf("_"));
            }
            else
            {
                atlasName = sceneName;
            }
            directory = Application.dataPath + "/Content/Environment/Savannah/Terrain/GrassLayer/Presets/PresetsVegetation/";// EditorUtility.SaveFolderPanel("Select Atlas Directory", Application.dataPath, sceneName);
        }

        public GrassAtlasCell GetCell(int _x, int _y)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].x == _x && cells[i].y == _y)
                    return cells[i];
            }
            return null;
        }

        public void DeleteCell(int _x, int _y)
        {

            GrassAtlasCell currentCellDelete = GetCell(_x, _y);

            if (currentAtlasCell == currentCellDelete)
                currentAtlasCell = null;

            for (int i = currentCellDelete.x; i < (currentCellDelete.size_x + currentCellDelete.x); i++)
                for (int j = currentCellDelete.y; j < (currentCellDelete.size_y + currentCellDelete.y); j++)
                {
                    array[i * size + j].arrayType = ArrayType.Free;
                }
            cells.Remove(currentCellDelete);
            array[_x * size + _y].arrayType = ArrayType.Free;
        }

        public GrassAtlasCell AddCell(int _x, int _y, int _sizeX, int _sizeY)
        {
            cells.Add(new GrassAtlasCell(_x, _y, _sizeX, _sizeY, size));
            GrassAtlasCell current = cells[cells.Count - 1];


            for (int i = 0; i < _sizeX; i++)
                for (int j = 0; j < _sizeY; j++)
                {
                    //Debug.Log(i + " " + j + " " + ((_x + i) * size + _y + j).ToString());
                    array[(_x + i) * size + _y + j].arrayType = ArrayType.Blocked;
                }

            array[_x * size + _y].cell = current;
            array[_x * size + _y].arrayType = ArrayType.Used;
            return current;
        }

      
    }
}
