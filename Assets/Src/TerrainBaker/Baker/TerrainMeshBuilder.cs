#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using Assets.Src.Lib.ProfileTools;

namespace Assets.TerrainBaker
{

    public class TerrainMeshBuilder
    {
        private int heightmapSize;
        private int terrainSize;
        private float heightScale;
        private float heightOffset;

        private class TerrainInfo
        {
            public Vector2Int pos;
            public TerrainData terrainData;
            public TerrainBakerLink linkMinX;
            public TerrainBakerLink linkMinZ;
            public TerrainBakerLink linkMaxX;
            public TerrainBakerLink linkMaxZ;
            public Mesh mesh;
            public Texture2D normalsHeightsTexture;
            public float baseHeightError;
            public int approxMaxTrianglesCount;
            public SplitInfo splitInfo;
            public bool needUpdateNormalsHeightsTexture;
            public bool needUpdateMapOfHoles;
            public int holesLayerIndex;
        }

        public class SplitInfo
        {
            public int heightErrorOffset;
        }

        private RectInt dirtyRect;

        private List<TerrainInfo> registryTerrainsList = new List<TerrainInfo>();
        private TerrainInfo[,] terrainsMap;
        private Vector2Int terrainsMapOffset;
        private Vector2Int terrainsMapSize;
        private bool isTerrainsMapReady;
        private bool needUpdateNormalsHeightsTexture;
        private bool needUpdateMapOfHoles;
        private AdaptiveMeshBuilder adaptiveMeshBuilder;
        private AdaptiveMeshBuilder.GenerateContext adaptiveMeshBuilderContext;//for paralell generate tri's and mesh later

        private Color32[] normalsHeightsBuffer;

        public TerrainMeshBuilder()
        {
        }

        public void DirtyMap()
        {
            isTerrainsMapReady = false;
        }

        public void AddDirtyRect(RectInt rect)
        {
            if (dirtyRect.width != 0 && dirtyRect.height != 0)
            {
                Vector2Int min = Vector2Int.Min(dirtyRect.min, rect.min);
                Vector2Int max = Vector2Int.Max(dirtyRect.max, rect.max);
                dirtyRect.min = min;
                dirtyRect.max = max;
            }
            else
            {
                dirtyRect = rect;
            }
        }

        public void UpdateMapOfHoles(TerrainData terrainData, int holesIndex)
        {
            if (registryTerrainsList == null || holesIndex >= terrainData.alphamapLayers)
            {
                return;
            }
            TerrainInfo info = registryTerrainsList.Find(obj => obj.terrainData == terrainData);
            if (info == null)
            {
                return;
            }

            info.needUpdateMapOfHoles = true;
            info.holesLayerIndex = holesIndex;
            needUpdateMapOfHoles = true;
        }

        struct TerrainUpdateData
        {
            public Vector2Int terrainMapIndeces;
            public RectInt terrainHeightsRect;
            public RectInt updateHeightsRect;
            public float baseHeightError;
            public int approxMaxTrianglesCount;
            public SplitInfo splitInfo;
            public float[,] updateHeights;
            public bool isInit;
        }

        struct BuildData
        {
            public TerrainUpdateData[] updates;
            public RectInt updateRect;
            public RectInt terrainsRect;
            public int terrainSize;
            public AdaptiveMeshBuilder adaptiveMeshBuilder;
            public AdaptiveMeshBuilder.GenerateContext adaptiveMeshBuilderContext;
        }

        private Task<GenerateAdaptiveMesh[]> buildTask = null;

        public void Process()
        {
            if (buildTask != null)
            {
                SceneView.RepaintAll();
                if (buildTask.IsCompleted)
                {
                    if (isTerrainsMapReady)
                    {
                        GenerateAdaptiveMesh[] resultMeshes = buildTask.Result;
                        if (resultMeshes != null)
                        {
                            for (int i = 0; i < resultMeshes.Length; i++)
                            {
                                GenerateAdaptiveMesh resultMesh = resultMeshes[i];
                                if (resultMesh != null)
                                {
                                    Vector2Int pos = resultMesh.terrainPosInMap;
                                    TerrainInfo ti = terrainsMap[pos.y, pos.x];
                                    Mesh mesh = ti.mesh;
                                    mesh.Clear();
                                    mesh.vertices = resultMesh.vertices;
                                    mesh.subMeshCount = resultMesh.subMeshes.Length;
                                    for (int j = 0; j < resultMesh.subMeshes.Length; j++)
                                    {
                                        mesh.SetTriangles(resultMesh.subMeshes[j].triangles, j, true, resultMesh.subMeshes[j].baseVertexIndex);
                                    }

                                    if (pos.x > 0 && ti.linkMinX != null)
                                    {
                                        adaptiveMeshBuilder.UpdateLinkMinX(pos, ti.linkMinX);
                                        EditorUtility.SetDirty(ti.linkMinX);
                                    }
                                    if (pos.x < terrainsMapSize.x - 1 && ti.linkMaxX != null)
                                    {
                                        adaptiveMeshBuilder.UpdateLinkMaxX(pos, ti.linkMaxX);
                                        EditorUtility.SetDirty(ti.linkMaxX);
                                    }
                                    if (pos.y > 0 && ti.linkMinZ != null)
                                    {
                                        adaptiveMeshBuilder.UpdateLinkMinZ(pos, ti.linkMinZ);
                                        EditorUtility.SetDirty(ti.linkMinZ);
                                    }
                                    if (pos.y < terrainsMapSize.y - 1 && ti.linkMaxZ != null)
                                    {
                                        adaptiveMeshBuilder.UpdateLinkMaxZ(pos, ti.linkMaxZ);
                                        EditorUtility.SetDirty(ti.linkMaxZ);
                                    }
                                    ti.needUpdateNormalsHeightsTexture = true;
                                    needUpdateNormalsHeightsTexture = true;

                                    mesh.uv = resultMesh.uv;

                                    mesh.RecalculateNormals();
                                    mesh.RecalculateTangents();

                                    MeshUtility.SetMeshCompression(mesh, ModelImporterMeshCompression.Medium);
                                    mesh.UploadMeshData(false);
                                }
                            }
                        }
                        resultMeshes = null;
                    }
                    buildTask = null;
                }
                return;
            }


            if (!isTerrainsMapReady)
            {
                MakeTerrainsMap();
                SceneView.RepaintAll();
                return;
            }

            if (needUpdateMapOfHoles)
            {
                needUpdateMapOfHoles = false;
                SceneView.RepaintAll();
                UpdateMapOfHoles();
                return;
            }

            if (dirtyRect.width == 0 && dirtyRect.height == 0)
            {
                if (needUpdateNormalsHeightsTexture)
                {
                    needUpdateNormalsHeightsTexture = false;
                    SceneView.RepaintAll();
                    for (int y = 0; y < terrainsMapSize.y; y++)
                    {
                        for (int x = 0; x < terrainsMapSize.x; x++)
                        {
                            TerrainInfo ti = terrainsMap[y, x];
                            if (ti != null && ti.needUpdateNormalsHeightsTexture)
                            {
                                ti.needUpdateNormalsHeightsTexture = false;
                                adaptiveMeshBuilder.GetNormalsAndHeights(new Vector2Int(x, y), normalsHeightsBuffer);
                                ti.normalsHeightsTexture.SetPixels32(normalsHeightsBuffer);
                                ti.normalsHeightsTexture.Apply(true);
                            }
                        }
                    }
                }
                return;
            }

            RectInt processRect = dirtyRect;
            processRect.position = processRect.position - (terrainsMapOffset * terrainSize);
            dirtyRect.width = 0;
            dirtyRect.height = 0;

            BuildData data = new BuildData();
            Vector2Int minTerrainPos = processRect.min;
            Vector2Int maxTerrainPos = processRect.max;
            minTerrainPos.x = Mathf.Max(minTerrainPos.x / terrainSize, 0);
            minTerrainPos.y = Mathf.Max(minTerrainPos.y / terrainSize, 0);
            maxTerrainPos.x = Mathf.Min((maxTerrainPos.x + terrainSize - 1) / terrainSize, terrainsMapSize.x);
            maxTerrainPos.y = Mathf.Min((maxTerrainPos.y + terrainSize - 1) / terrainSize, terrainsMapSize.y);
            RectInt terrainsRect = new RectInt(minTerrainPos, maxTerrainPos - minTerrainPos);

            if (terrainsRect.width == 0 || terrainsRect.height == 0)
            {
                return;
            }

            data.updateRect = new RectInt(terrainsRect.position * terrainSize, terrainsRect.size * terrainSize + Vector2Int.one);
            data.terrainsRect = terrainsRect;
            data.terrainSize = terrainSize;
            data.updates = new TerrainUpdateData[terrainsRect.width * terrainsRect.height];
            data.adaptiveMeshBuilder = adaptiveMeshBuilder;
            data.adaptiveMeshBuilderContext = adaptiveMeshBuilderContext;

            Vector2Int heightsSize = new Vector2Int(heightmapSize, heightmapSize);
            for (int y = 0, index = 0; y < terrainsRect.height; y++)
            {
                for (int x = 0; x < terrainsRect.width; x++, index++)
                {
                    Vector2Int terrainMapIndeces = new Vector2Int(x, y) + terrainsRect.position;

                    TerrainInfo ti = terrainsMap[terrainMapIndeces.y, terrainMapIndeces.x];
                    if (ti == null)
                    {
                        continue;
                    }

                    TerrainUpdateData udata = new TerrainUpdateData();

                    udata.terrainMapIndeces = terrainMapIndeces;

                    RectInt terrainHeightsRect = new RectInt(terrainMapIndeces * terrainSize, heightsSize);
                    udata.terrainHeightsRect = terrainHeightsRect;

                    TerrainData terrainData = ti.terrainData;
                    udata.baseHeightError = ti.baseHeightError;
                    udata.approxMaxTrianglesCount = ti.approxMaxTrianglesCount;
                    udata.splitInfo = ti.splitInfo;

                    RectInt updateRect = processRect;
                    updateRect.ClampToBounds(terrainHeightsRect);

                    if (updateRect.width > 0 && updateRect.height > 0)
                    {
                        udata.updateHeightsRect = updateRect;
                        Vector2Int inTerrainPos = updateRect.position - terrainHeightsRect.position;
                        udata.updateHeights = terrainData.GetHeights(inTerrainPos.x, inTerrainPos.y, updateRect.width, updateRect.height);
                    }
                    else
                    {
                        continue;
                    }

                    udata.isInit = true;

                    data.updates[index] = udata;
                }
            }
            buildTask = Task.Run(() => Build(data));//async
            //buildTask = Build(data);//sync
            SceneView.RepaintAll();
        }

        private void MakeTerrainsMap()
        {
            TerrainBaker[] terrainBakers = Profile.FindObjectsOfTypeAll<TerrainBaker>();
            for (int i = 0; i < terrainBakers.Length; i++)
            {
                if (terrainBakers[i].terrain == null)
                {
                    //terrainBakers[i].ForceInit();
                    continue;
                }
                string check = TerrainBaker.CheckTerrainData(terrainBakers[i].terrain.terrainData);
                if (!string.IsNullOrEmpty(check))
                {
                    Debug.Log("Terrain " + terrainBakers[i].terrain.name + " error: " + check);
                    continue;
                }
                Registry(terrainBakers[i]);
            }
            if (registryTerrainsList.Count == 0)
            {
                return;
            }
            Vector2Int minPos = registryTerrainsList[0].pos;
            Vector2Int maxPos = registryTerrainsList[0].pos;
            for (int i = 1; i < registryTerrainsList.Count; i++)
            {
                minPos = Vector2Int.Min(minPos, registryTerrainsList[i].pos);
                maxPos = Vector2Int.Max(maxPos, registryTerrainsList[i].pos);
            }
            Vector2Int sizes = maxPos - minPos + Vector2Int.one;
            if (sizes.x * heightmapSize > AdaptiveMeshBuilder.maxGlobalHeightsMapSize || sizes.y * heightmapSize > AdaptiveMeshBuilder.maxGlobalHeightsMapSize)
            {
                Debug.Log("TerrainMeshBuilder terrains global height map too large. Please load less terrains at once.");
                return;
            }
            if (terrainsMap != null)
            {
                if (terrainsMapSize.x == sizes.x && terrainsMapSize.y == sizes.y)
                {
                    for (int y = 0; y < sizes.y; y++)
                    {
                        for (int x = 0; x < sizes.x; x++)
                        {
                            terrainsMap[y, x] = null;
                        }
                    }
                }
                else
                {
                    terrainsMap = null;
                    adaptiveMeshBuilder = null;
                    adaptiveMeshBuilderContext = null;
                }
            }
            terrainsMapOffset = minPos;
            terrainsMapSize = sizes;
            if (terrainsMap == null)
            {
                terrainsMap = new TerrainInfo[sizes.y, sizes.x];
            }
            for (int i = 0; i < registryTerrainsList.Count; i++)
            {
                Vector2Int pos = registryTerrainsList[i].pos - terrainsMapOffset;
                terrainsMap[pos.y, pos.x] = registryTerrainsList[i];
            }
            if (adaptiveMeshBuilder == null ||
                adaptiveMeshBuilder.heightmapSizePerTerrain != heightmapSize ||
                sizes != adaptiveMeshBuilder.terrainsPerSide ||
                Mathf.Abs(heightScale - adaptiveMeshBuilder.heightScale) > 0)
            {
                bool[,] terrainPresentMap = new bool[sizes.y, sizes.x];
                for (int y = 0; y < sizes.y; y++)
                {
                    for (int x = 0; x < sizes.x; x++)
                    {
                        terrainPresentMap[y, x] = terrainsMap[y, x] != null;
                    }
                }
                adaptiveMeshBuilder = new AdaptiveMeshBuilder(sizes, heightmapSize, heightScale, terrainPresentMap);
                adaptiveMeshBuilderContext = adaptiveMeshBuilder.CreateContext();
            }
            else
            {
                adaptiveMeshBuilder.Clear();
                adaptiveMeshBuilder.Clear(adaptiveMeshBuilderContext);
            }

            if (normalsHeightsBuffer == null)
            {
                normalsHeightsBuffer = new Color32[heightmapSize * heightmapSize];
            }

            for (int x = 0; x < terrainsMapSize.x; x++)
            {
                int lastY = terrainsMapSize.y - 1;
                adaptiveMeshBuilder.ApplyLinkMinZ(new Vector2Int(x, 0), terrainsMap[0, x] != null ? terrainsMap[0, x].linkMinZ : null);
                adaptiveMeshBuilder.ApplyLinkMaxZ(new Vector2Int(x, lastY), terrainsMap[lastY, x] != null ? terrainsMap[lastY, x].linkMaxZ : null);
            }
            for (int y = 0; y < terrainsMapSize.y; y++)
            {
                int lastX = terrainsMapSize.x - 1;
                adaptiveMeshBuilder.ApplyLinkMinX(new Vector2Int(0, y), terrainsMap[y, 0] != null ? terrainsMap[y, 0].linkMinX : null);
                adaptiveMeshBuilder.ApplyLinkMaxX(new Vector2Int(lastX, y), terrainsMap[y, lastX] != null ? terrainsMap[y, lastX].linkMaxX : null);
            }

            dirtyRect.position = terrainsMapOffset * terrainSize;
            dirtyRect.size = terrainsMapSize * terrainSize + Vector2Int.one;

            isTerrainsMapReady = true;
        }

        private void Registry(TerrainBaker baker)
        {
            Terrain terrain = baker.terrain;
            if (terrain == null)
            {
                Debug.Log("TerrainBaker " + terrain.name + " not registered in TerrainMeshBuilder because have not Terrain");
                return;
            }
            if (terrain.terrainData == null)
            {
                Debug.Log("Terrain " + terrain.name + " not registered in TerrainMeshBuilder because have not terrainData");
                return;
            }
            if (heightmapSize == 0)
            {
                heightmapSize = terrain.terrainData.heightmapResolution;
                terrainSize = heightmapSize - 1;
                heightScale = terrain.terrainData.size.y;
                heightOffset = terrain.transform.position.y;
            }

            Mesh mesh = baker.GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null)
            {
                Debug.Log("Terrain " + terrain.name + " not registered in TerrainMeshBuilder because they gameObject have not mesh");
            }
            if (terrain.terrainData.heightmapResolution != heightmapSize || terrain.terrainData.heightmapResolution != heightmapSize)
            {
                Debug.Log("Terrain " + terrain.name + " not registered in TerrainMeshBuilder because have diference heightmap size");
                return;
            }
            if (Mathf.Abs(terrain.terrainData.size.y - heightScale) > 1e-10f)
            {
                Debug.Log("Terrain " + terrain.name + " not registered in TerrainMeshBuilder because have diference height scale");
                return;
            }
            if (Mathf.Abs(terrain.transform.position.y - heightOffset) > 1e-10f)
            {
                Debug.Log("Terrain " + terrain.name + " not registered in TerrainMeshBuilder because diference height position");
                return;
            }

            Vector3 fPos = terrain.transform.position / terrainSize;
            Vector2Int pos = new Vector2Int((int)fPos.x, (int)fPos.z);
            TerrainInfo regTi = null;
            int regIndex = registryTerrainsList.FindIndex(obj => obj.pos == pos);
            if (regIndex >= 0)
            {
                regTi = registryTerrainsList[regIndex];
                if (regTi.terrainData != terrain.terrainData)
                {
                    Debug.Log("Terrain " + terrain.name + " have some position with other terrain with different TerrainData");
                    return;
                }
                if (baker.linkMinX != regTi.linkMinX || baker.linkMinZ != regTi.linkMinZ ||
                    baker.linkMaxX != regTi.linkMaxX || baker.linkMaxZ != regTi.linkMaxZ)
                {
                    Debug.Log("Terrain " + terrain.name + " have some position with other terrain with different TerrainBakerLink");
                    return;
                }
                if (regTi.mesh != mesh)
                {
                    Debug.Log("Terrain " + terrain.name + " have some position with other terrain with different Mesh");
                    return;
                }
                //same terrain in other scene
            }
            TerrainBakerMaterialSupport materialSupport = baker.GetComponent<TerrainBakerMaterialSupport>();
            if (materialSupport == null || materialSupport.normalsHeightsTexture == null)
            {
                Debug.Log("Terrain " + terrain.name + " have not ready TerrainBakerMaterialSupport. Wrong logic?");
                return;
            }


            TerrainInfo ti = (regTi == null) ? new TerrainInfo() : regTi;

            ti.pos = pos;
            ti.terrainData = terrain.terrainData;
            ti.linkMinX = baker.linkMinX;
            ti.linkMinZ = baker.linkMinZ;
            ti.linkMaxX = baker.linkMaxX;
            ti.linkMaxZ = baker.linkMaxZ;
            ti.mesh = mesh;
            ti.approxMaxTrianglesCount = Mathf.Max(baker.approximatelyMaxTrianglesCount, AdaptiveMeshBuilder.approximatelyMaxTrianglesCountMin);
            ti.baseHeightError = Mathf.Clamp(baker.baseHeightError, AdaptiveMeshBuilder.heightMaxErrorMin, AdaptiveMeshBuilder.heightMaxErrorMax) / heightScale;
            ti.splitInfo = new SplitInfo();
            ti.normalsHeightsTexture = materialSupport.normalsHeightsTexture;
            ti.holesLayerIndex = baker.GetSysLayerIndex(TerrainBaker.sysLayerHole);
            ti.needUpdateMapOfHoles = (ti.holesLayerIndex >= 0);
            needUpdateMapOfHoles |= ti.needUpdateMapOfHoles;
            if (regTi == null)
            {
                registryTerrainsList.Add(ti);
            }
            isTerrainsMapReady = false;
        }

        private void UpdateMapOfHoles()
        {
            float eps = 0.001f;
            int mapOfHolesSize = terrainSize / 2;
            bool[,] mapOfHoles = new bool[mapOfHolesSize, mapOfHolesSize];
            for (int y = 0; y < terrainsMapSize.y; y++)
            {
                for (int x = 0; x < terrainsMapSize.x; x++)
                {
                    TerrainInfo ti = terrainsMap[y, x];
                    if (ti == null || !ti.needUpdateMapOfHoles || ti.holesLayerIndex < 0 || ti.holesLayerIndex >= ti.terrainData.alphamapLayers)
                    {
                        continue;
                    }
                    float[,,] alphaMaps = ti.terrainData.GetAlphamaps(0, 0, terrainSize, terrainSize);
                    int holesIndex = ti.holesLayerIndex;
                    for (int mhY = 0; mhY < mapOfHolesSize; mhY++)
                    {
                        for (int mhX = 0; mhX < mapOfHolesSize; mhX++)
                        {
                            int amX = mhX * 2;
                            int amY = mhY * 2;
                            bool newVal = alphaMaps[amY + 0, amX + 0, holesIndex] > eps;
                            newVal |= alphaMaps[amY + 1, amX + 0, holesIndex] > eps;
                            newVal |= alphaMaps[amY + 0, amX + 1, holesIndex] > eps;
                            newVal |= alphaMaps[amY + 1, amX + 1, holesIndex] > eps;
                            mapOfHoles[mhY, mhX] = newVal;
                        }
                    }
                    adaptiveMeshBuilder.ApplyMapOfHoles(new Vector2Int(x, y), mapOfHoles);
                }
            }
        }


        static Task<GenerateAdaptiveMesh[]> Build(BuildData data)
        {
            for (int i = 0; i < data.updates.Length; i++)
            {
                if (data.updates[i].isInit)
                {
                    data.adaptiveMeshBuilder.ApplyHeightmapFragment(data.updates[i].updateHeightsRect.position, data.updates[i].updateHeights);
                    int height = data.updates[i].updateHeights.GetLength(0);
                    int width = data.updates[i].updateHeights.GetLength(1);
                    data.adaptiveMeshBuilder.UpdateNormals(data.updates[i].updateHeightsRect.position, new Vector2Int(width, height));
                }
            }

            for (int i = 0; i < data.updates.Length; i++)
            {
                if (data.updates[i].isInit)
                {
                    data.adaptiveMeshBuilder.UpdateErrors(data.updates[i].updateHeightsRect);
                }
            }

            data.adaptiveMeshBuilder.SplitMapsSnapshot();

            for (int i = 0; i < data.updates.Length; i++)
            {
                if (!data.updates[i].isInit)
                {
                    continue;
                }

                Vector2Int terrainMapIndeces = data.updates[i].terrainMapIndeces;

                SplitInfo splitInfo = data.updates[i].splitInfo;
                float baseHeightError = data.updates[i].baseHeightError;
                int approxMaxTrianglesCount = data.updates[i].approxMaxTrianglesCount;
                int approxStopTrianglesCount = (int)(approxMaxTrianglesCount * 0.9f);
                if (splitInfo.heightErrorOffset == 0)
                {
                    if (data.adaptiveMeshBuilder.DoSplitStep(terrainMapIndeces, baseHeightError) <= approxMaxTrianglesCount)
                    {
                        continue;
                    }
                    splitInfo.heightErrorOffset = 1;
                }
                bool isStopSearch = true;
                int approxTrianglesCount1 = data.adaptiveMeshBuilder.DoSplitStep(terrainMapIndeces, splitInfo.heightErrorOffset * baseHeightError);
                int approxTrianglesCount2 = data.adaptiveMeshBuilder.DoSplitStep(terrainMapIndeces, splitInfo.heightErrorOffset * baseHeightError * 2.0f);
                for (int j = 0; j < 16; j++)
                {
                    if (approxTrianglesCount2 <= approxMaxTrianglesCount)
                    {
                        if (approxTrianglesCount1 > approxMaxTrianglesCount)
                        {
                            isStopSearch = false;
                            break;
                        }
                        splitInfo.heightErrorOffset /= 2;
                        if (splitInfo.heightErrorOffset == 0)
                        {
                            break;
                        }
                        approxTrianglesCount2 = approxTrianglesCount1;
                        approxTrianglesCount1 = data.adaptiveMeshBuilder.DoSplitStep(terrainMapIndeces, splitInfo.heightErrorOffset * baseHeightError);
                    }
                    else
                    {
                        if (splitInfo.heightErrorOffset > 0x1000000)
                        {
                            break;
                        }
                        splitInfo.heightErrorOffset *= 2;
                        approxTrianglesCount1 = approxTrianglesCount2;
                        approxTrianglesCount2 = data.adaptiveMeshBuilder.DoSplitStep(terrainMapIndeces, splitInfo.heightErrorOffset * baseHeightError);
                    }
                }
                if (isStopSearch)
                {
                    continue;
                }
                float minError = splitInfo.heightErrorOffset * baseHeightError;
                float maxError = splitInfo.heightErrorOffset * baseHeightError * 2.0f;
                for (int j = 0; j < 8; j++)
                {
                    float heightError = (minError + maxError) * 0.5f;
                    int approxTrianglesCount = data.adaptiveMeshBuilder.DoSplitStep(terrainMapIndeces, heightError);
                    if (approxTrianglesCount <= approxMaxTrianglesCount)
                    {
                        if (approxTrianglesCount > approxStopTrianglesCount)
                        {
                            break;
                        }
                        maxError = heightError;
                    }
                    else
                    {
                        minError = heightError;
                    }
                }
            }

            RectInt changeIndeces = data.adaptiveMeshBuilder.FinalizeSplitMaps();

            GenerateAdaptiveMesh[] meshes = new GenerateAdaptiveMesh[Mathf.Max(changeIndeces.width * changeIndeces.height, 1)];
            for (int y = 0, index = 0; y < changeIndeces.height; y++)
            {
                for (int x = 0; x < changeIndeces.width; x++, index++)
                {
                    Vector2Int terrainMapIndeces = new Vector2Int(x, y) + changeIndeces.position;
                    if (data.adaptiveMeshBuilder.IsPresentTerrain(terrainMapIndeces))
                    {
                        data.adaptiveMeshBuilder.GenerateRawTriangles(terrainMapIndeces, data.adaptiveMeshBuilderContext);
                        meshes[index] = data.adaptiveMeshBuilder.BuildMesh(terrainMapIndeces, data.adaptiveMeshBuilderContext);
                    }
                }
            }
            return Task.FromResult(meshes);
        }


    }
}

#endif
