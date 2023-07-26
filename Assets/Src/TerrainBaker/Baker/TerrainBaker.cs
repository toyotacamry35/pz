using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Assets.TerrainBaker
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Terrain))]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    [RequireComponent(typeof(TerrainBakerMaterialSupport))]
    [AddComponentMenu("Terrain/Terrain Baker")]
    public class TerrainBaker : MonoBehaviour
    {
#if UNITY_EDITOR

        public string errorString { get; private set; }

        public bool enableUpdatesFromFixedTerrainFormer;
        public bool enableUpdateEveryFrame;
        public bool enableDrawSystemTextures;
        public bool enableHolesUpdate;

        public TerrainAtlas atlas;

        public TerrainBakerLink linkMinX;
        public TerrainBakerLink linkMinZ;
        public TerrainBakerLink linkMaxX;
        public TerrainBakerLink linkMaxZ;

        public float baseHeightError;
        public int approximatelyMaxTrianglesCount;

        public Terrain terrain { get; private set; }

        public bool debugEnableDumpBuildMaterialsTextures;

        private TerrainData terrainData;
        private TerrainBakerMaterialSupport materialSupport;

        private static TerrainMaterialsBuilder materialsBuilder;
        private static TerrainMeshBuilder meshBuilder;

        private bool isRegistryInMeshBuilder;

        private int heightmapSize;
        private int alphaMapSize;
        private int matMapSize;
        private int weightsMapSize;
        private int layersCount;
        private float curBaseHeightError;
        private int curApproximatelyMaxTrianglesCount;

        private int meshMaterialsCount;

        private RectInt updateMeshRect;
        private RectInt updateMaterialsRect;

        private byte[] remapLayersToAtlasMaterials;

        private bool isCreateAssetsInProgress = false;

        #region System textures

        public const string sysLayerNamePrefix = "sys_";

        public const string sysLayerHole = "sys_hole";
        public const string sysLayerNoGrass = "sys_no_grass";

        public const string sysTexturesShaderName = "Hidden/PaintSysTextures";
        public const int maxSysTextures = 4;

        private struct SysTextureInfo
        {
            public readonly string name;
            public int index;
            public Texture2D texture;


            public SysTextureInfo(string n)
            {
                name = n;
                index = -1;
                texture = null;
            }
        }

        private SysTextureInfo[] sysTextures = new SysTextureInfo[] { new SysTextureInfo(sysLayerHole), new SysTextureInfo(sysLayerNoGrass) };
        public bool isPresentSysTextures { get; private set; }
        private Material sysTexturesMaterial;

        public int GetSysLayerIndex(string name)
        {
            for (int i = 0; i < sysTextures.Length; i++)
            {
                if (sysTextures[i].name == name)
                {
                    return sysTextures[i].index;
                }
            }
            return -1;
        }

        public Texture2D GetSysLayerTexture(string name)
        {
            for (int i = 0; i < sysTextures.Length; i++)
            {
                if (sysTextures[i].name == name)
                {
                    return sysTextures[i].texture;
                }
            }
            return null;
        }

        #endregion


        private void Awake()
        {
            terrain = GetComponent<Terrain>();
            terrainData = terrain.terrainData;
            meshBuilder = null;//force rebuild terrains map when create new TerrainBaker
            baseHeightError = AdaptiveMeshBuilder.heightMaxError;
            approximatelyMaxTrianglesCount = AdaptiveMeshBuilder.approximatelyMaxTrianglesCount;
        }

        private void Start()
        {
            if (isCreateAssetsInProgress)
            {
                return;
            }

            isRegistryInMeshBuilder = false;
            if (CheckParameters())
            {
                EditorUpdate(true);
            }
        }

        public void Update()
        {
            if (isCreateAssetsInProgress)
            {
                return;
            }

            bool isForceUpdate = false;
            if (terrain == null)
            {
                terrain = GetComponent<Terrain>();
                terrainData = terrain.terrainData;
                isForceUpdate = true;
            }
            if (terrainData.alphamapLayers == 0)
            {
                //Terrain not init now
                return;
            }

            if (enableDrawSystemTextures && isPresentSysTextures && materialSupport != null && materialSupport.terrainMesh != null)
            {
                DrawSysTextures();
            }

            isForceUpdate |= (alphaMapSize != terrainData.alphamapWidth) | (layersCount != terrainData.alphamapLayers) | (heightmapSize != terrainData.heightmapResolution);
            isForceUpdate |= (curApproximatelyMaxTrianglesCount != approximatelyMaxTrianglesCount) | (curBaseHeightError != baseHeightError);
            bool isDirtyRects = false;
            isDirtyRects |= updateMeshRect.width > 0 && updateMeshRect.height > 0;
            isDirtyRects |= updateMaterialsRect.width > 0 && updateMaterialsRect.height > 0;
            if (updateIndex >= editorSceneUpdateIndex && !(isForceUpdate | isDirtyRects | enableUpdateEveryFrame | enableHolesUpdate))
            {
                materialSupport.UpdateMaterial();
                return;
            }
            if (isForceUpdate || enableUpdateEveryFrame || enableHolesUpdate)
            {
                DirtyAllTerrain();
            }
            updateIndex = editorSceneUpdateIndex;
            if (CheckParameters())
            {
                EditorUpdate(isForceUpdate);
            }

            materialSupport.UpdateMaterial();
            if (materialSupport.terrainMesh != null && meshMaterialsCount != materialSupport.terrainMesh.subMeshCount)
            {
                SetMeshMaterials();
            }
        }

        private void SetupParameters()
        {
            heightmapSize = terrainData.heightmapResolution;
            alphaMapSize = terrainData.alphamapWidth;
            matMapSize = alphaMapSize + 1;
            weightsMapSize = matMapSize * 2;
            layersCount = terrainData.alphamapLayers;
            curApproximatelyMaxTrianglesCount = approximatelyMaxTrianglesCount;
            curBaseHeightError = baseHeightError;
        }

        private void EditorUpdate(bool isReloadAssets)
        {
            if (!CheckParameters()) return;

            SetupParameters();

            if (materialSupport == null || isReloadAssets)
            {
                materialSupport = GetComponent<TerrainBakerMaterialSupport>();
                if (materialSupport.atlas == null || isReloadAssets)
                {
                    CreateAssets();
                }
            }

            if (materialsBuilder == null)
            {
                materialsBuilder = new TerrainMaterialsBuilder();
                materialsBuilder.Init(alphaMapSize, matMapSize, layersCount);
            }

            if (meshBuilder == null)
            {
                meshBuilder = new TerrainMeshBuilder();
            }
            if (isRegistryInMeshBuilder == false)
            {
                meshBuilder.DirtyMap();
                isRegistryInMeshBuilder = true;
            }

            if (remapLayersToAtlasMaterials == null || remapLayersToAtlasMaterials.Length == 0 || isReloadAssets)
            {
                UpdateRemapLayersToAtlasMaterials();
            }

            if (enableHolesUpdate)
            {
                UpdateHoles();
            }

            if (updateMeshRect.width > 0 && updateMeshRect.height > 0)
            {
                Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
                RectInt dirtyRect = new RectInt(updateMeshRect.position + pos, updateMeshRect.size);
                meshBuilder.AddDirtyRect(dirtyRect);
                updateMeshRect.width = 0;
                updateMeshRect.height = 0;
            }
            if (updateMaterialsRect.width > 0 && updateMaterialsRect.height > 0)
            {
                TerrainMaterialProcessData data = new TerrainMaterialProcessData();
                data.terrainData = terrainData;
                data.matTexture = materialSupport.matTexture;
                data.weightsTexture = materialSupport.weightsTexture;
                data.materialsArray = materialSupport.materialsArray;
                data.remapLayersToAtlasMaterials = remapLayersToAtlasMaterials;
                data.dirtyRect = updateMaterialsRect;
                data.isEnableDump = debugEnableDumpBuildMaterialsTextures;

                materialsBuilder.AddMaterialsForProcess(data);

                updateMaterialsRect.width = 0;
                updateMaterialsRect.height = 0;
            }
        }

        public void ForceUpdate(bool isCreateAssets)
        {
            materialsBuilder = null;
            meshBuilder = null;
            remapLayersToAtlasMaterials = null;
            isRegistryInMeshBuilder = false;
            if (materialSupport != null)
            {
                materialSupport.atlas = null;
                materialSupport = null;
            }
            DirtyAllTerrain();
            editorSceneUpdateIndex = 1;
            updateIndex = 0;
            if (isCreateAssets)
            {
                if (CheckParameters())
                {
                    SetupParameters();
                    CreateAssets();
                }
            }
        }

        public void UpdateHoles()
        {
            if (sysTextures.Length > 0 && sysTextures[0].name == sysLayerHole && sysTextures[0].index >= 0)
            {
                if (meshBuilder != null && terrain != null)
                {
                    meshBuilder.UpdateMapOfHoles(terrain.terrainData, sysTextures[0].index);
                }
            }
        }

        private bool CheckParameters()
        {
            errorString = CheckTerrainData(terrainData);
            if (errorString != null)
            {
                return false;
            }
            if (atlas == null)
            {
                errorString = "TerrainAtlas is not set";
                return false;
            }
            if (atlas.albedo == null || atlas.normals == null)
            {
                errorString = "TerrainAtlas is build";
                return false;
            }
            return true;
        }

        public static string CheckTerrainData(TerrainData terrainData)
        {
            if (terrainData == null)
            {
                return "terrainData not set";
            }
            int heightmapSize = terrainData.heightmapResolution;
            if (heightmapSize < 33)
            {
                return "Heightmap resolution is too small";
            }
            if (heightmapSize > 513)
            {
                return "Heightmap resolution is too large";
            }
            if (!Mathf.IsPowerOfTwo(heightmapSize - 1))
            {
                return "Heightmap resolution must have size is N^2 + 1 (129, 257, 513, etc.)";
            }
            if (heightmapSize != terrainData.heightmapResolution)
            {
                return "Heightmap must be is quad";
            }
            if (terrainData.alphamapWidth < 16)
            {
                return "Control texture resolution is too small";
            }
            if (terrainData.alphamapWidth != terrainData.alphamapHeight)
            {
                return "Control texture be is quad";
            }
            if (!Mathf.IsPowerOfTwo(terrainData.alphamapWidth))
            {
                return "Control texture resolution must have size is heightmap resolution - 1";
            }
            if (terrainData.terrainLayers.Length != terrainData.alphamapLayers)
            {
                return "Control textures count problems";
            }
            if (terrainData.alphamapLayers > 32)
            {
                return "Control textures count maxumum is 32";
            }
            const float eps = 1e-20f;
            if (Mathf.Abs(heightmapSize - 1 - terrainData.size.x) > eps)
            {
                return "Terrain Width is not equal Heightmap Resolution - 1";
            }
            if (Mathf.Abs(heightmapSize - 1 - terrainData.size.z) > eps)
            {
                return "Terrain Length is not equal Heightmap Resolution - 1";
            }
            return null;
        }

        private void UpdateRemapLayersToAtlasMaterials()
        {
            TerrainLayer[] terrainLayers = terrainData.terrainLayers;
            if (remapLayersToAtlasMaterials == null || remapLayersToAtlasMaterials.Length == 0)
            {
                remapLayersToAtlasMaterials = new byte[32];
            }
            isPresentSysTextures = false;
            for (int j = 0; j < sysTextures.Length; j++)
            {
                sysTextures[j].index = -1;
            }
            for (int i = 0; i < layersCount; i++)
            {
                byte findIndex = 255;
                TerrainLayer proto = terrainLayers[i];
                Texture2D tex = proto.diffuseTexture as Texture2D;
                if (tex.name.StartsWith(sysLayerNamePrefix))
                {
                    for (int j = 0; j < sysTextures.Length; j++)
                    {
                        if (tex.name == sysTextures[j].name)
                        {
                            sysTextures[j].index = i;
                            sysTextures[j].texture = tex;
                            isPresentSysTextures = true;
                            break;
                        }
                    }
                    continue;
                }
                for (byte j = 0; j < atlas.layers.Length; j++)
                {
                    if (tex.name == atlas.layers[j].albedoName)
                    {
                        findIndex = j;
                        break;
                    }
                }
                if (findIndex == 255)
                {
                    if (errorString != null && errorString.Length > 0)
                    {
                        errorString += "\n";
                    }
                    errorString += "Error: no find texture " + tex.name + " in terrain atlas";
                }
                remapLayersToAtlasMaterials[i] = findIndex;
            }
            for (int i = layersCount; i < 32; i++) remapLayersToAtlasMaterials[i] = 255;
        }

        #region Create assets, init components

        private void CreateAssets()
        {
            const string assetExt = ".asset";

            if (isCreateAssetsInProgress)
            {
                return;
            }
            isCreateAssetsInProgress = true;

            string prefixPath = AssetDatabase.GetAssetOrScenePath(terrainData);
            if (prefixPath.EndsWith(assetExt))
            {
                prefixPath = prefixPath.Remove(prefixPath.Length - assetExt.Length, assetExt.Length);
            }
            string prefixName = prefixPath;
            if (prefixName.LastIndexOf('/') >= 0)
            {
                prefixName = prefixName.Remove(0, prefixName.LastIndexOf('/') + 1);
            }

            if (materialSupport == null)
            {
                materialSupport = GetComponent<TerrainBakerMaterialSupport>();
            }

            materialSupport.atlas = atlas;

            const string matPostfix = "_tb_tex_mats";
            string matTexturePath = prefixPath + matPostfix + assetExt;
            materialSupport.matTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(matTexturePath);
            if (materialSupport.matTexture == null)
            {
                DirtyAllTerrain();
                materialSupport.matTexture = new Texture2D(matMapSize, matMapSize, TextureFormat.ARGB32, false, true);
                //materialSupport.matTexture.name = prefixName + matPostfix;  //AUTOMATIC NAME
                materialSupport.matTexture.filterMode = FilterMode.Point;
                materialSupport.matTexture.wrapMode = TextureWrapMode.Clamp;
                materialSupport.matTexture.anisoLevel = 1;
                AssetDatabase.CreateAsset(materialSupport.matTexture, matTexturePath);
                AssetDatabase.ImportAsset(matTexturePath, ImportAssetOptions.ForceUpdate);
                materialSupport.matTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(matTexturePath);
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
            const string weightsPostfix = "_tb_tex_weights";
            string weightsTexturePath = prefixPath + weightsPostfix + assetExt;
            materialSupport.weightsTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(weightsTexturePath);
            if (materialSupport.weightsTexture == null)
            {
                DirtyAllTerrain();
                materialSupport.weightsTexture = new Texture2D(weightsMapSize, weightsMapSize, TextureFormat.ARGB32, false, true);
                //materialSupport.weightsTexture.name = prefixName + weightsPostfix;  //AUTOMATIC NAME
                materialSupport.weightsTexture.filterMode = FilterMode.Bilinear;
                materialSupport.weightsTexture.wrapMode = TextureWrapMode.Clamp;
                materialSupport.weightsTexture.anisoLevel = 1;
                AssetDatabase.CreateAsset(materialSupport.weightsTexture, weightsTexturePath);
                AssetDatabase.ImportAsset(weightsTexturePath, ImportAssetOptions.ForceUpdate);
                materialSupport.weightsTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(weightsTexturePath);
            }
            const string meshNamePostfix = "_tb_mesh";
            string meshPath = prefixPath + meshNamePostfix + assetExt;
            materialSupport.terrainMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (materialSupport.terrainMesh == null)
            {
                DirtyAllTerrain();
                materialSupport.terrainMesh = new Mesh();
                //materialSupport.terrainMesh.name = prefixName + meshNamePostfix;  //AUTOMATIC NAME
                Vector3[] vertices = new Vector3[3];
                Vector3[] normals = new Vector3[3];
                int[] triangles = new int[3];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = Vector3.zero;
                    normals[i] = new Vector3(0, 1, 0);
                    triangles[i] = i;
                }
                materialSupport.terrainMesh.vertices = vertices;
                materialSupport.terrainMesh.normals = normals;
                materialSupport.terrainMesh.triangles = triangles;
                AssetDatabase.CreateAsset(materialSupport.terrainMesh, meshPath);
                AssetDatabase.ImportAsset(meshPath, ImportAssetOptions.ForceUpdate);
                materialSupport.terrainMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            }
            GetComponent<MeshFilter>().mesh = materialSupport.terrainMesh;

            const string materialPostfix = "_tb_material.mat";
            string materialPath = prefixPath + materialPostfix;
            materialSupport.material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (materialSupport.material == null)
            {
                DirtyAllTerrain();
                Material mat = new Material(Shader.Find(TerrainBakerMaterialSupport.renderShaderName));
                AssetDatabase.CreateAsset(mat, materialPath);
                AssetDatabase.ImportAsset(materialPath, ImportAssetOptions.ForceUpdate);
                materialSupport.material = mat;//AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            }

            const string normalsHeightsPostfix = "_tb_nrm_hgt";
            string normalsHeightsTexturePath = prefixPath + normalsHeightsPostfix + assetExt;
            materialSupport.normalsHeightsTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalsHeightsTexturePath);
            if (materialSupport.normalsHeightsTexture == null)
            {
                DirtyAllTerrain();
                materialSupport.normalsHeightsTexture = new Texture2D(heightmapSize, heightmapSize, TextureFormat.ARGB32, true, true);
                //materialSupport.normalsHeightsTexture.name = prefixName + normalsHeightsPostfix;  //AUTOMATIC NAME
                materialSupport.normalsHeightsTexture.filterMode = FilterMode.Bilinear;
                materialSupport.normalsHeightsTexture.wrapMode = TextureWrapMode.Clamp;
                materialSupport.normalsHeightsTexture.anisoLevel = 1;
                AssetDatabase.CreateAsset(materialSupport.normalsHeightsTexture, normalsHeightsTexturePath);
                AssetDatabase.ImportAsset(normalsHeightsTexturePath, ImportAssetOptions.ForceUpdate);
                materialSupport.normalsHeightsTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalsHeightsTexturePath);
            }

            const string materailsArrayPostfix = "_tb_mat_array";
            string materailsArrayPath = prefixPath + materailsArrayPostfix + assetExt;
            materialSupport.materialsArray = AssetDatabase.LoadAssetAtPath<TerrainMaterialsArray>(materailsArrayPath);
            if (materialSupport.materialsArray == null)
            {
                DirtyAllTerrain();
                materialSupport.materialsArray = ScriptableObject.CreateInstance<TerrainMaterialsArray>();
                materialSupport.materialsArray.alphaMapSize = alphaMapSize;
                materialSupport.materialsArray.materialsMap = new byte[alphaMapSize * alphaMapSize];
                for (int i = 0; i < materialSupport.materialsArray.materialsMap.Length; i++)
                {
                    materialSupport.materialsArray.materialsMap[i] = TerrainMaterialsArray.noMaterial;
                }
                AssetDatabase.CreateAsset(materialSupport.materialsArray, materailsArrayPath);
                AssetDatabase.ImportAsset(materailsArrayPath, ImportAssetOptions.ForceUpdate);
                materialSupport.materialsArray = AssetDatabase.LoadAssetAtPath<TerrainMaterialsArray>(materailsArrayPath);
            }

            UpdateMaterialParameters();

            isCreateAssetsInProgress = false;
        }

        private void UpdateMaterialParameters()
        {
            UpdateRemapLayersToAtlasMaterials();

            bool isHasMacroTexture = false;
            bool isHasEmissionTexture = false;
            for (int i = 0; i < layersCount; i++)
            {
                int index = remapLayersToAtlasMaterials[i];
                if (index < atlas.layers.Length)
                {
                    isHasMacroTexture |= atlas.layers[i].macroIndex >= 0;
                    isHasEmissionTexture |= atlas.layers[i].emissionIndex >= 0;
                }
            }
            TerrainData terrainData = GetComponent<Terrain>().terrainData;
            materialSupport.heightsScale = terrainData.size.y;
            materialSupport.isHasEmissionTexture = isHasEmissionTexture;
            materialSupport.isHasMacroTexture = isHasMacroTexture;
            materialSupport.terrainSizes = new Vector4(1.0f / terrainData.size.x, terrainData.size.x, 1.0f / terrainData.size.z, terrainData.size.z);
            materialSupport.terrainMatTexInfo = new Vector4(matMapSize, 0.5f / matMapSize, 1.0f / matMapSize, (matMapSize - 1.0f) / matMapSize);
            materialSupport.terrainWgtTexInfo = new Vector4(weightsMapSize, 0.5f / weightsMapSize, 1.0f / weightsMapSize, 0.0f);

            materialSupport.UpdateMaterial();
            SetMeshMaterials();
        }

        private void SetMeshMaterials()
        {
            GetComponent<MeshRenderer>().material = materialSupport.material;
            int subMeshCount = (materialSupport.terrainMesh != null) ? Mathf.Max(materialSupport.terrainMesh.subMeshCount, 1) : 4;
            meshMaterialsCount = subMeshCount;
            Material[] materials = new Material[subMeshCount];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = materialSupport.material;
            }
            GetComponent<MeshRenderer>().materials = materials;
        }

        #endregion

        #region Take events for detect update

        private int editorSceneUpdateIndex = 1;
        private int updateIndex = 0;

        private void OnEnable()
        {
            SceneView.duringSceneGui += EditorSceneUpdate;
            Undo.undoRedoPerformed += ObjectUndoCallback;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= EditorSceneUpdate;
            Undo.undoRedoPerformed -= ObjectUndoCallback;
        }

        private void EditorSceneUpdate(SceneView sc)
        {
            if (enableUpdatesFromFixedTerrainFormer)
            {
                return;
            }
            if (Selection.activeObject != gameObject)
            {
                return;
            }
            if (Event.current != null)
            {
                if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout)
                {
                    UnityEngine.Object activeObject = Selection.activeObject;
                    if (activeObject != gameObject)
                    {
                        if (gameObject.transform.parent == null)
                        {
                            return;
                        }
                        if (activeObject != gameObject.transform.parent.gameObject)
                        {
                            return;
                        }
                    }
                    DirtyAllTerrain();
                    RequireUpdate();
                    EditorUtility.SetDirty(this);
                }
            }
        }

        private void ObjectUndoCallback()
        {
            DirtyAllTerrain();
            RequireUpdate();
        }

        private void RequireUpdate()
        {
            editorSceneUpdateIndex++;
            if (editorSceneUpdateIndex < 0x6fffffff)
            {
                editorSceneUpdateIndex = 1;
                updateIndex = 0;
            }
        }

        private void TerrainChangeAlphamaps(RectInt updateRect)
        {
            if (updateRect.width <= 0 || updateRect.height <= 0)
            {
                return;
            }
            if (updateMaterialsRect.width > 0 && updateMaterialsRect.height > 0)
            {
                updateMaterialsRect = UnionRects(updateMaterialsRect, updateRect);
            }
            else
            {
                updateMaterialsRect = updateRect;
            }
            RequireUpdate();
        }

        private void TerrainChangeHeights(RectInt updateRect)
        {
            if (updateRect.width <= 0 || updateRect.height <= 0)
            {
                return;
            }
            if (updateMeshRect.width > 0 && updateMeshRect.height > 0)
            {
                updateMeshRect = UnionRects(updateMeshRect, updateRect);
            }
            else
            {
                updateMeshRect = updateRect;
            }
            RequireUpdate();
        }

        private void DirtyAllMesh()
        {
            updateMeshRect = new RectInt(0, 0, heightmapSize, heightmapSize);
        }

        private void DirtyAllMaterials()
        {
            updateMaterialsRect = new RectInt(0, 0, alphaMapSize, alphaMapSize);
        }

        private void DirtyAllTerrain()
        {
            DirtyAllMesh();
            DirtyAllMaterials();
        }

        private static RectInt UnionRects(RectInt r1, RectInt r2)
        {
            Vector2Int min = Vector2Int.Min(r1.min, r2.min);
            Vector2Int max = Vector2Int.Max(r1.max, r2.max);
            return new RectInt(min, max - min);
        }

        #endregion

        public void OnRenderObject()
        {
            if (materialsBuilder != null)
            {
                materialsBuilder.Process();
            }
            if (meshBuilder != null)
            {
                meshBuilder.Process();
            }

        }

        void DrawSysTextures()
        {
            if (sysTexturesMaterial == null)
            {
                sysTexturesMaterial = new Material(Shader.Find(sysTexturesShaderName));
                sysTexturesMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                sysTexturesMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            if (sysTexturesMaterial == null)
            {
                return;
            }
            Texture2D[] alphamapTextures = terrain.terrainData.alphamapTextures;
            int sysTextureIndex = 0;
            for (int i = 0; i < sysTextures.Length; i++)
            {
                if (sysTextures[i].index >= 0)
                {
                    int alphmapIndex = sysTextures[i].index / 4;
                    if (alphmapIndex < alphamapTextures.Length)
                    {
                        sysTexturesMaterial.SetTexture("SysTexture" + sysTextureIndex, sysTextures[i].texture);
                        sysTexturesMaterial.SetTexture("Alphamap" + sysTextureIndex, alphamapTextures[alphmapIndex]);
                        Vector4 selectMask = Vector4.zero;
                        selectMask[sysTextures[i].index % 4] = 1.0f;
                        sysTexturesMaterial.SetVector("AlphamapSelectMask" + sysTextureIndex, selectMask);
                        sysTextureIndex++;
                        if (sysTextureIndex > maxSysTextures)
                        {
                            break;
                        }
                    }
                }
            }
            Vector3 size = terrain.terrainData.size;
            Vector2 invSize = new Vector2();
            invSize.x = size.x > 1.0f ? 1.0f / size.x : 0.0f;
            invSize.y = size.z > 1.0f ? 1.0f / size.z : 0.0f;
            sysTexturesMaterial.SetVector("InvTerrainSize", invSize);
            Graphics.DrawMesh(materialSupport.terrainMesh, transform.localToWorldMatrix, sysTexturesMaterial, 0);
        }

#endif
    }

}
