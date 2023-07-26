#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.TerrainBaker
{

    public class GenerateAdaptiveMesh
    {
        public struct Submesh
        {
            public int[] triangles;
            public int baseVertexIndex;
        }

        public Submesh[] subMeshes;
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector2[] uv;
        public Vector2Int terrainPosInMap;
    }


    public class AdaptiveMeshBuilder
    {
        /*
         * 
         *   Z
         *  /|\
         *   |
         *   |
         *   10------Up-----11
         *   |               |
         *   |               | 
         *   Left Center Right
         *   |               |
         *   |               |
         *   00-----Down----01 --->X
         *
         *   array_access[z, x]
         *
         *   Vector2Int v; v.x -> X; v.y -> Z
         */

        public const int levelsCount = 6;
        public const int maxGlobalHeightsMapSize = 16384;
        public const float heightMaxError = 0.1f;
        public const float heightMaxErrorMin = 0.001f;
        public const float heightMaxErrorMax = 1000.0f;
        public const int approximatelyMaxTrianglesCount = 60000;
        public const int approximatelyMaxTrianglesCountMin = 32 * 32 * 2;
        //public const bool isCalculateMeshNormals = false;


        private struct Node
        {
            public const uint splitUp = 0x01;
            public const uint splitDown = 0x02;
            public const uint splitLeft = 0x04;
            public const uint splitRight = 0x08;
            public const uint splitCenter = 0x10;
            public const uint splitPresentChild00 = 0x100;
            public const uint splitPresentChild01 = 0x200;
            public const uint splitPresentChild10 = 0x400;
            public const uint splitPresentChild11 = 0x800;

            public const uint splitMask = splitUp | splitDown | splitLeft | splitRight | splitCenter;
            public const uint splitChildrenMask = splitPresentChild00 | splitPresentChild01 | splitPresentChild10 | splitPresentChild11;
            public const uint splitDataMask = splitMask | splitChildrenMask;

            public const uint splitHole = 0x2000;

            public const uint splitBorderForceSet = 0x4000;
            public const uint splitBorderSplit = 0x8000;

            public const int splitSnapshotShift = 16;
            public const uint splitSnapshotMask = splitDataMask << splitSnapshotShift;


            public const byte splitMapNotSet = 0;
            public const byte splitMapSplitNode = 1;
            public const byte splitMapNoSplitNode = 2;


            public uint splitFlags;
            public float deltaUp;
            public float deltaDown;
            public float deltaLeft;
            public float deltaRight;
            public float deltaCenter;
        }


        private class Level
        {
            public Node[,] nodes;
            public int quadSize;
            public int nodesPerSidePerTerrain;
            public Vector2Int nodesPerSide;
        }

        public readonly Vector2Int terrainsPerSide;
        private readonly Vector2Int heightmapSize;
        public readonly int heightmapSizePerTerrain;
        public readonly int terrainSize;
        private readonly int splitMapSize;
        public readonly float heightScale;
        private Level[] levels;
        private float[,] heightMap;
        private Vector2[,] normalsMap;
        private bool[,] terrainsPresentMap;

        #region Init

        public AdaptiveMeshBuilder(Vector2Int _sizeInTerrains, int _heightmapSize, float _heightScale, bool[,] _terrainsPresentMap)
        {
            terrainsPerSide = _sizeInTerrains;
            heightmapSizePerTerrain = _heightmapSize;
            heightScale = _heightScale;
            terrainSize = heightmapSizePerTerrain - 1;
            terrainsPresentMap = _terrainsPresentMap;
            splitMapSize = CalcSplitMapSize(heightmapSizePerTerrain);
            levels = new Level[levelsCount];
            int nodesPerSidePerTerrain = terrainSize / 2;
            int quadSize = 2;
            for (int levelIndex = levelsCount - 1; levelIndex >= 0; levelIndex--)
            {
                levels[levelIndex] = new Level();
                levels[levelIndex].quadSize = quadSize;
                levels[levelIndex].nodesPerSidePerTerrain = nodesPerSidePerTerrain;
                Vector2Int nodesPerSide = terrainsPerSide * nodesPerSidePerTerrain;
                levels[levelIndex].nodesPerSide = nodesPerSide;
                levels[levelIndex].nodes = new Node[nodesPerSide.y, nodesPerSide.x];
                for (int y = 0; y < nodesPerSide.y; y++)
                {
                    for (int x = 0; x < nodesPerSide.x; x++)
                    {
                        levels[levelIndex].nodes[y, x].splitFlags = Node.splitDataMask;
                    }
                }
                nodesPerSidePerTerrain /= 2;
                quadSize *= 2;
            }
            heightmapSize = terrainsPerSide * terrainSize + Vector2Int.one;
            heightMap = new float[heightmapSize.y + 2, heightmapSize.x + 2];
            normalsMap = new Vector2[heightmapSize.y, heightmapSize.x];
            for (int i = 0; i < heightmapSize.x + 2; i++)
            {
                heightMap[0, i] = -1.0f;
                heightMap[heightmapSize.y + 1, i] = -1.0f;
            }
            for (int i = 0; i < heightmapSize.y + 2; i++)
            {
                heightMap[i, 0] = -1.0f;
                heightMap[i, heightmapSize.x + 1] = -1.0f;
            }
        }

        public void Clear()
        {
            Array.Clear(heightMap, 0, (heightmapSize.y + 2) * (heightmapSize.x + 2));
            Array.Clear(normalsMap, 0, heightmapSize.y * heightmapSize.x);
            for (int i = 0; i < levels.Length; i++)
            {
                Array.Clear(levels[i].nodes, 0, levels[i].nodesPerSide.x * levels[i].nodesPerSide.y);
            }
        }

        #endregion

        #region Link utilites

        public void ApplyLinkMinX(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            if (CheckAndFixLink(link))
            {
                CopyLinkHeightmapDataByX(0, terrainMapIndeces.y * terrainSize, link.heightMapBorder);
                CopyLinkSplitsMap(link.splitsMaps, terrainMapIndeces, 0, 1, 0);
            }
        }

        public void ApplyLinkMaxX(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            if (CheckAndFixLink(link))
            {
                CopyLinkHeightmapDataByX(heightmapSize.x - 1, terrainMapIndeces.y * terrainSize, link.heightMapBorder);
                CopyLinkSplitsMap(link.splitsMaps, terrainMapIndeces, 0, 1, 1);
            }
        }

        public void ApplyLinkMinZ(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            if (CheckAndFixLink(link))
            {
                CopyLinkHeightmapDataByZ(terrainMapIndeces.x * terrainSize, 0, link.heightMapBorder);
                CopyLinkSplitsMap(link.splitsMaps, terrainMapIndeces, 1, 0, 0);
            }
        }

        public void ApplyLinkMaxZ(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            if (CheckAndFixLink(link))
            {
                CopyLinkHeightmapDataByZ(terrainMapIndeces.x * terrainSize, heightmapSize.y - 1, link.heightMapBorder);
                CopyLinkSplitsMap(link.splitsMaps, terrainMapIndeces, 1, 0, 1);
            }
        }

        private void CopyLinkHeightmapDataByX(int offsetX, int offsetZ, float[] heightMapBorder)
        {
            for (int z = 0, index = 0; z < heightmapSizePerTerrain; z++, index += 3)
            {
                heightMap[offsetZ + z, offsetX + 0] = heightMapBorder[index + 0];
                heightMap[offsetZ + z, offsetX + 1] = heightMapBorder[index + 1];
                heightMap[offsetZ + z, offsetX + 2] = heightMapBorder[index + 2];
            }
        }

        private void CopyLinkHeightmapDataByZ(int offsetX, int offsetZ, float[] heightMapBorder)
        {
            for (int x = 0, index = 0; x < heightmapSizePerTerrain; x++, index += 3)
            {
                heightMap[offsetZ + 0, offsetX + x] = heightMapBorder[index + 0];
                heightMap[offsetZ + 1, offsetX + x] = heightMapBorder[index + 1];
                heightMap[offsetZ + 2, offsetX + x] = heightMapBorder[index + 2];
            }
        }

        private void CopyLinkSplitsMap(byte[] splitsMaps, Vector2Int terrainMapIndeces, int dx, int dz, int maxV)
        {
            for (int levelIndex = 0, splitsMapsIndex = 0; levelIndex < levelsCount; levelIndex++)
            {
                Node[,] nodes = levels[levelIndex].nodes;
                int nodesPerTerrainSide = levels[levelIndex].nodesPerSidePerTerrain;
                int x = terrainMapIndeces.x * nodesPerTerrainSide + dz * maxV * (nodesPerTerrainSide - 1);
                int z = terrainMapIndeces.y * nodesPerTerrainSide + dx * maxV * (nodesPerTerrainSide - 1);
                for (int i = 0; i < nodesPerTerrainSide; i++, x += dx, z += dz, splitsMapsIndex++)
                {
                    uint splitFlags = nodes[z, x].splitFlags;
                    splitFlags &= ~(Node.splitBorderForceSet | Node.splitBorderSplit);
                    switch (splitsMaps[splitsMapsIndex])
                    {
                        case Node.splitMapNoSplitNode:
                            splitFlags |= Node.splitBorderForceSet;
                            break;
                        case Node.splitMapSplitNode:
                            splitFlags |= Node.splitBorderForceSet | Node.splitBorderSplit;
                            break;
                    }
                    nodes[z, x].splitFlags = splitFlags;
                }
            }
        }


        public void UpdateLinkMinX(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            CheckAndFixLink(link);
            UpdateLinkHeightmapDataForX(terrainMapIndeces.x * terrainSize - 1, terrainMapIndeces.y * terrainSize, link.heightMapBorder);
            UpdateSplitsMap(link.splitsMaps, terrainMapIndeces, 0, 1, 0, Node.splitLeft);
        }

        public void UpdateLinkMaxX(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            CheckAndFixLink(link);
            UpdateLinkHeightmapDataForX((terrainMapIndeces.x + 1) * terrainSize - 1, terrainMapIndeces.y * terrainSize, link.heightMapBorder);
            UpdateSplitsMap(link.splitsMaps, terrainMapIndeces, 0, 1, 1, Node.splitRight);
        }

        public void UpdateLinkMinZ(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            CheckAndFixLink(link);
            UpdateLinkHeightmapDataForZ(terrainMapIndeces.x * terrainSize, terrainMapIndeces.y * terrainSize - 1, link.heightMapBorder);
            UpdateSplitsMap(link.splitsMaps, terrainMapIndeces, 1, 0, 0, Node.splitDown);
        }

        public void UpdateLinkMaxZ(Vector2Int terrainMapIndeces, TerrainBakerLink link)
        {
            CheckAndFixLink(link);
            UpdateLinkHeightmapDataForZ(terrainMapIndeces.x * terrainSize, (terrainMapIndeces.y + 1) * terrainSize - 1, link.heightMapBorder);
            UpdateSplitsMap(link.splitsMaps, terrainMapIndeces, 1, 0, 1, Node.splitUp);
        }

        private void UpdateLinkHeightmapDataForX(int offsetX, int offsetZ, float[] heightMapBorder)
        {
            offsetX += 1;
            offsetZ += 1;
            for (int z = 0, index = 0; z < heightmapSizePerTerrain; z++, index += 3)
            {
                heightMapBorder[index + 0] = Mathf.Max(heightMap[offsetZ + z, offsetX + 0], 0.0f);
                heightMapBorder[index + 1] = Mathf.Max(heightMap[offsetZ + z, offsetX + 1], 0.0f);
                heightMapBorder[index + 2] = Mathf.Max(heightMap[offsetZ + z, offsetX + 2], 0.0f);
            }
        }

        private void UpdateLinkHeightmapDataForZ(int offsetX, int offsetZ, float[] heightMapBorder)
        {
            for (int x = 0, index = 0; x < heightmapSizePerTerrain; x++, index += 3)
            {
                heightMapBorder[index + 0] = Mathf.Max(heightMap[offsetZ + 0, offsetX + x], 0.0f);
                heightMapBorder[index + 1] = Mathf.Max(heightMap[offsetZ + 1, offsetX + x], 0.0f);
                heightMapBorder[index + 2] = Mathf.Max(heightMap[offsetZ + 2, offsetX + x], 0.0f);
            }
        }

        private void UpdateSplitsMap(byte[] splitsMaps, Vector2Int terrainMapIndeces, int dx, int dz, int maxV, uint mask)
        {
            for (int levelIndex = 0, splitsMapsIndex = 0; levelIndex < levelsCount; levelIndex++)
            {
                Node[,] nodes = levels[levelIndex].nodes;
                int nodesPerTerrainSide = levels[levelIndex].nodesPerSidePerTerrain;
                int x = terrainMapIndeces.x * nodesPerTerrainSide + dz * maxV * (nodesPerTerrainSide - 1);
                int z = terrainMapIndeces.y * nodesPerTerrainSide + dx * maxV * (nodesPerTerrainSide - 1);
                for (int i = 0; i < nodesPerTerrainSide; i++, x += dx, z += dz, splitsMapsIndex++)
                {
                    splitsMaps[splitsMapsIndex] = (nodes[z, x].splitFlags & mask) != 0 ? Node.splitMapSplitNode : Node.splitMapNoSplitNode;
                }
            }
        }

        public static void InitLinkData(TerrainBakerLink link, int heightmapSize)
        {
            link.heightmapSize = heightmapSize;
            int splitMapSize = CalcSplitMapSize(heightmapSize);
            link.splitsMaps = new byte[splitMapSize];
            for (int i = 0; i < splitMapSize; i++)
            {
                link.splitsMaps[i] = Node.splitMapNotSet;
            }
            link.heightMapBorder = new float[3 * heightmapSize];
        }

        public bool CheckAndFixLink(TerrainBakerLink link)
        {
            if (link != null)
            {
                if (link.heightmapSize != heightmapSizePerTerrain ||
                    link.heightMapBorder.Length != heightmapSizePerTerrain * 3 ||
                    link.splitsMaps.Length != splitMapSize)
                {
                    InitLinkData(link, heightmapSizePerTerrain);
                    return false;
                }
                return true;
            }
            return false;
        }

        public static int CalcSplitMapSize(int heightmapSize)
        {
            int splitMapSize = 0;
            int nodesPerSide = (heightmapSize - 1) / 2;
            for (int levelIndex = levelsCount - 1; levelIndex >= 0; levelIndex--)
            {
                splitMapSize += nodesPerSide;
                nodesPerSide /= 2;
            }
            return splitMapSize;
        }

        #endregion

        #region 1) Update heights map, heightmap & normals utilites

        public void ApplyHeightmapFragment(Vector2Int pos, float[,] newHeightMap)
        {
            int height = newHeightMap.GetLength(0);
            int width = newHeightMap.GetLength(1);
            pos += Vector2Int.one;
            //3 pixels border
            int changeMinX = 3;
            int changeMinY = 3;
            int changeMaxX = heightmapSize.x + 2 - 3;
            int changeMaxY = heightmapSize.y + 2 - 3;
            if (pos.x > changeMinX && pos.y > changeMinY && pos.x + width < changeMaxX && pos.y + height < changeMaxY)
            {
                for (int y = 0; y < height; y++)
                {
                    int globalY = pos.y + y;
                    for (int x = 0; x < width; x++)
                    {
                        int globalX = pos.x + x;
                        heightMap[globalY, globalX] = newHeightMap[y, x];
                    }
                }
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    int globalY = pos.y + y;
                    for (int x = 0; x < width; x++)
                    {
                        int globalX = pos.x + x;
                        if (globalX < changeMinX)
                        {
                            if (heightMap[globalY, 0] >= 0.0)
                            {
                                continue;
                            }
                        }
                        if (globalX >= changeMaxX)
                        {
                            if (heightMap[globalY, heightmapSize.x + 2 - 1] >= 0.0)
                            {
                                continue;
                            }
                        }
                        if (globalY < changeMinY)
                        {
                            if (heightMap[0, globalX] >= 0.0)
                            {
                                continue;
                            }
                        }
                        if (globalY >= changeMaxY)
                        {
                            if (heightMap[heightmapSize.y + 2 - 1, globalX] >= 0.0)
                            {
                                continue;
                            }
                        }
                        heightMap[globalY, globalX] = newHeightMap[y, x];
                    }
                }
            }
        }


        public void UpdateNormals(Vector2Int pos, Vector2Int size)
        {
            for (int y = 0; y < size.y; y++)
            {
                int mapY = pos.y + y;
                int heightMapY = mapY + 1;
                for (int x = 0; x < size.x; x++)
                {
                    int mapX = pos.x + x;
                    int heightMapX = mapX + 1;

                    float ch = heightMap[heightMapY, heightMapX] * heightScale;
                    float dh = heightMap[heightMapY, heightMapX - 1];
                    dh = dh >= 0.0f ? dh * heightScale - ch : 0.0f;
                    Vector3 n0 = new Vector3(dh, 1, 0);
                    dh = heightMap[heightMapY, heightMapX + 1];
                    dh = dh >= 0.0f ? dh * heightScale - ch : 0.0f;
                    Vector3 n1 = new Vector3(-dh, 1, 0);
                    dh = heightMap[heightMapY - 1, heightMapX];
                    dh = dh >= 0.0f ? dh * heightScale - ch : 0.0f;
                    Vector3 n2 = new Vector3(0, 1, dh);
                    dh = heightMap[heightMapY + 1, heightMapX];
                    dh = dh >= 0.0f ? dh * heightScale - ch : 0.0f;
                    Vector3 n3 = new Vector3(0, 1, -dh);

                    Vector3 n = (n0.normalized + n1.normalized + n2.normalized + n3.normalized).normalized;

                    normalsMap[mapY, mapX] = new Vector2(Mathf.Clamp(n.x, -1.0f, 1.0f), Mathf.Clamp(n.z, -1.0f, 1.0f));
                }
            }
        }

        public void GetNormalsAndHeights(Vector2Int terrainMapIndeces, Color32[] terrainData)
        {
            int terrainX = terrainMapIndeces.x * terrainSize;
            int terrainY = terrainMapIndeces.y * terrainSize;
            for (int y = 0, index = 0; y < heightmapSizePerTerrain; y++)
            {
                for (int x = 0; x < heightmapSizePerTerrain; x++, index++)
                {
                    Vector2 normal = normalsMap[terrainY + y, terrainX + x];
                    byte r = (byte)(normal.x * 127 + 127);
                    byte g = (byte)(normal.y * 127 + 127);

                    float height = Mathf.Clamp01(heightMap[terrainY + y + 1, terrainX + x + 1]) * 0xffff;
                    int qSacle = (int)height;
                    byte b = (byte)(qSacle & 0xff);
                    byte a = (byte)((qSacle >> 8) & 0xff);

                    terrainData[index] = new Color32(r, g, b, a);
                }
            }
        }

        #endregion

        #region 2) Update errors in splitmap

        public void UpdateErrors(RectInt rect)
        {
            int minX = rect.xMin - 2;
            int minZ = rect.yMin - 2;
            int maxX = rect.xMax + 2;
            int maxZ = rect.yMax + 2;

            for (int levelIndex = levels.Length - 1; levelIndex >= 0; levelIndex--)
            {
                Level level = levels[levelIndex];
                int quadSize = level.quadSize;
                int halfSize = quadSize / 2;

                minX = Mathf.Max(minX / 2 - 1, 0);
                minZ = Mathf.Max(minZ / 2 - 1, 0);
                maxX = Mathf.Min((maxX + 1) / 2 + 1, level.nodesPerSide.x);
                maxZ = Mathf.Min((maxZ + 1) / 2 + 1, level.nodesPerSide.y);

                for (int z = minZ; z < maxZ; z++)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        int x0 = x * quadSize;
                        int z0 = z * quadSize;
                        int x1 = x0 + quadSize;
                        int z1 = z0 + quadSize;
                        int xc = x0 + halfSize;
                        int zc = z0 + halfSize;
                        level.nodes[z, x].deltaUp = CalcHeightDelta(z1, x0, z1, xc, z1, x1);
                        level.nodes[z, x].deltaDown = CalcHeightDelta(z0, x0, z0, xc, z0, x1);
                        level.nodes[z, x].deltaRight = CalcHeightDelta(z0, x1, zc, x1, z1, x1);
                        level.nodes[z, x].deltaLeft = CalcHeightDelta(z0, x0, zc, x0, z1, x0);
                        float hc1 = CalcHeightDelta(z0, x0, zc, xc, z1, x1);
                        float hc2 = CalcHeightDelta(z0, x1, zc, xc, z1, x0);
                        level.nodes[z, x].deltaCenter = Mathf.Max(hc1, hc2);
                    }
                }
            }
        }

        private float CalcHeightDelta(int z1, int x1, int zc, int xc, int z2, int x2)
        {
            float h1 = heightMap[z1 + 1, x1 + 1];
            float h2 = heightMap[z2 + 1, x2 + 1];
            float hc = heightMap[zc + 1, xc + 1];
            return Mathf.Abs((h1 + h2) * 0.5f - hc);
        }

        #endregion

        #region 3) Build splitmaps

        public void SplitMapsSnapshot()
        {
            for (int levelIndex = levels.Length - 1; levelIndex >= 0; levelIndex--)
            {
                Level level = levels[levelIndex];
                Node[,] nodes = level.nodes;
                Vector2Int size = level.nodesPerSide;
                for (int z = 0; z < size.y; z++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        uint splitFlags = nodes[z, x].splitFlags;
                        uint snapshotValue = (splitFlags << Node.splitSnapshotShift) & Node.splitSnapshotMask;
                        nodes[z, x].splitFlags = (splitFlags & ~Node.splitSnapshotMask) | snapshotValue;
                    }
                }
            }
        }

        public void MakeSplitMaps(Vector2Int terrainMapIndeces, float maxHeightDelta)
        {
            int minX = terrainMapIndeces.x * terrainSize;
            int minZ = terrainMapIndeces.y * terrainSize;
            int maxX = minX + terrainSize;
            int maxZ = minZ + terrainSize;

            ClearNodes(minX, minZ, maxX, maxZ);
            SplitQuads(minX, minZ, maxX, maxZ, maxHeightDelta);
            ParentsPostSplit(minX, minZ, maxX, maxZ);
        }

        private void ClearNodes(int minX, int minZ, int maxX, int maxZ)
        {
            for (int levelIndex = levels.Length - 1; levelIndex >= 0; levelIndex--)
            {
                Level level = levels[levelIndex];
                Node[,] nodes = level.nodes;

                minX = Mathf.Max(minX / 2, 0);
                minZ = Mathf.Max(minZ / 2, 0);
                maxX = Mathf.Min((maxX + 1) / 2, level.nodesPerSide.x);
                maxZ = Mathf.Min((maxZ + 1) / 2, level.nodesPerSide.y);

                for (int z = minZ; z < maxZ; z++)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        nodes[z, x].splitFlags &= ~Node.splitDataMask;
                    }
                }
            }
        }

        private void SplitQuads(int minX, int minZ, int maxX, int maxZ, float maxHeightDelta)
        {
            for (int levelIndex = levels.Length - 1; levelIndex >= 0; levelIndex--)
            {
                Level level = levels[levelIndex];
                Node[,] nodes = level.nodes;
                int lastX = level.nodesPerSide.x - 1;
                int lastZ = level.nodesPerSide.y - 1;

                minX = Mathf.Max(minX / 2, 0);
                minZ = Mathf.Max(minZ / 2, 0);
                maxX = Mathf.Min((maxX + 1) / 2, level.nodesPerSide.x);
                maxZ = Mathf.Min((maxZ + 1) / 2, level.nodesPerSide.y);

                for (int z = minZ; z < maxZ; z++)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        if ((nodes[z, x].splitFlags & Node.splitHole) != 0)
                        {
                            nodes[z, x].splitFlags |= Node.splitMask;
                            continue;
                        }
                        if (nodes[z, x].deltaCenter > maxHeightDelta)
                        {
                            nodes[z, x].splitFlags |= Node.splitCenter;
                        }
                        if (nodes[z, x].deltaDown > maxHeightDelta)
                        {
                            nodes[z, x].splitFlags |= Node.splitDown;
                            if (z > 0) nodes[z - 1, x].splitFlags |= Node.splitUp;
                        }
                        if (nodes[z, x].deltaLeft > maxHeightDelta)
                        {
                            nodes[z, x].splitFlags |= Node.splitLeft;
                            if (x > 0) nodes[z, x - 1].splitFlags |= Node.splitRight;
                        }
                        if (lastX == x && nodes[z, x].deltaRight > maxHeightDelta)
                        {
                            nodes[z, x].splitFlags |= Node.splitRight;
                        }
                        if (lastZ == z && nodes[z, x].deltaUp > maxHeightDelta)
                        {
                            nodes[z, x].splitFlags |= Node.splitUp;
                        }
                    }
                }
            }
        }

        private void ParentsPostSplit(int minX, int minZ, int maxX, int maxZ)
        {
            minX = Mathf.Max(minX / 2, 0);
            minZ = Mathf.Max(minZ / 2, 0);
            maxX = Mathf.Min((maxX + 1) / 2, levels[levels.Length - 1].nodesPerSide.x);
            maxZ = Mathf.Min((maxZ + 1) / 2, levels[levels.Length - 1].nodesPerSide.y);

            for (int levelIndex = levels.Length - 2; levelIndex >= 0; levelIndex--)
            {
                Level level = levels[levelIndex];
                Node[,] nodes = level.nodes;
                Node[,] childrenNodes = levels[levelIndex + 1].nodes;
                int lastX = level.nodesPerSide.x - 1;
                int lastZ = level.nodesPerSide.y - 1;

                minX = Mathf.Max(minX / 2, 0);
                minZ = Mathf.Max(minZ / 2, 0);
                maxX = Mathf.Min((maxX + 1) / 2, level.nodesPerSide.x);
                maxZ = Mathf.Min((maxZ + 1) / 2, level.nodesPerSide.y);

                for (int z = minZ; z < maxZ; z++)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        int cx = x * 2;
                        int cz = z * 2;
                        bool isSplit00 = (childrenNodes[cz, cx].splitFlags & Node.splitDataMask) != 0;
                        bool isSplit01 = (childrenNodes[cz, cx + 1].splitFlags & Node.splitDataMask) != 0;
                        bool isSplit10 = (childrenNodes[cz + 1, cx].splitFlags & Node.splitDataMask) != 0;
                        bool isSplit11 = (childrenNodes[cz + 1, cx + 1].splitFlags & Node.splitDataMask) != 0;
                        if ((isSplit00 | isSplit01 | isSplit10 | isSplit11) == false) continue;
                        //nodes[z, x].splitFlags |= Node.splitChildren;
                        if (isSplit00 | isSplit01)
                        {
                            nodes[z, x].splitFlags |= Node.splitDown;
                            if (z > 0) nodes[z - 1, x].splitFlags |= Node.splitUp;
                        }
                        if (isSplit10 | isSplit11)
                        {
                            nodes[z, x].splitFlags |= Node.splitUp;
                            if (z < lastZ) nodes[z + 1, x].splitFlags |= Node.splitDown;
                        }
                        if (isSplit00 | isSplit10)
                        {
                            nodes[z, x].splitFlags |= Node.splitLeft;
                            if (x > 0) nodes[z, x - 1].splitFlags |= Node.splitRight;
                        }
                        if (isSplit01 | isSplit11)
                        {
                            nodes[z, x].splitFlags |= Node.splitRight;
                            if (x < lastX) nodes[z, x + 1].splitFlags |= Node.splitLeft;
                        }
                        if (isSplit00) nodes[z, x].splitFlags |= Node.splitPresentChild00;
                        if (isSplit01) nodes[z, x].splitFlags |= Node.splitPresentChild01;
                        if (isSplit10) nodes[z, x].splitFlags |= Node.splitPresentChild10;
                        if (isSplit11) nodes[z, x].splitFlags |= Node.splitPresentChild11;
                    }
                }
            }
        }

        private void CheckSplitMap(int levelIndex)
        {
            Node[,] nodes = levels[levelIndex].nodes;
            Vector2Int nodesPerSide = levels[levelIndex].nodesPerSide;
            Vector2Int lastNodeIndex = nodesPerSide - Vector2Int.one;
            for (int z = 0; z < nodesPerSide.y; z++)
                for (int x = 0; x < nodesPerSide.x; x++)
                {
                    if ((nodes[z, x].splitFlags & Node.splitUp) != 0)
                    {
                        if (z > 0 && (nodes[z - 1, x].splitFlags & Node.splitDown) == 0) SplitError(levelIndex, z, x, z - 1, x);
                    }
                    if ((nodes[z, x].splitFlags & Node.splitRight) != 0)
                    {
                        if (x < lastNodeIndex.x && (nodes[z, x + 1].splitFlags & Node.splitLeft) == 0) SplitError(levelIndex, z, x, z, x + 1);
                    }
                    if ((nodes[z, x].splitFlags & Node.splitDown) != 0)
                    {
                        if (z < lastNodeIndex.y && (nodes[z + 1, x].splitFlags & Node.splitUp) == 0) SplitError(levelIndex, z, x, z + 1, x);
                    }
                    if ((nodes[z, x].splitFlags & Node.splitLeft) != 0)
                    {
                        if (x > 0 && (nodes[z, x - 1].splitFlags & Node.splitRight) == 0) SplitError(levelIndex, z, x, z, x - 1);
                    }
                }
        }

        void SplitError(int levelIndex, int z1, int x1, int z2, int x2)
        {
            throw new Exception(string.Format("split map erorr on level {0} between nodes (z:{1},x:{2})-(z:{3},x:{4})", levelIndex, z1, x1, z2, x2));
        }

        public RectInt FinalizeSplitMaps()
        {
            ApplyBorderSplits();
            ReconcileSplitMaps();
            ParentsPostSplit(0, 0, heightmapSize.x, heightmapSize.y);

            return CalcChangesRect();
        }

        private void ApplyBorderSplits()
        {
            Level level = levels[0];
            Vector2Int nodesPerSide = level.nodesPerSide;
            for (int i = 0; i < nodesPerSide.x; i++)
            {
                FixBorderNode(0, i, 0, Node.splitDown, false);
                FixBorderNode(0, i, nodesPerSide.y - 1, Node.splitUp, false);
            }
            for (int i = 0; i < nodesPerSide.y; i++)
            {
                FixBorderNode(0, 0, i, Node.splitLeft, false);
                FixBorderNode(0, nodesPerSide.y - 1, i, Node.splitRight, false);
            }
        }

        private void FixBorderNode(int levelIndex, int nodeX, int nodeZ, uint splitFlag, bool isClearSplits)
        {
            if (levels == null)
            {
                Debug.Log("Wrong logic: levels == null");
                return;
            }

            if (levelIndex < 0 || levelIndex >= levels.Length)
            {
                Debug.Log("Wrong logic: FixBorderNode levelIndex = " + levelIndex + " [0.." + levels.Length + "]");
                return;
            }

            Node[,] nodes = levels[levelIndex].nodes;

            if (nodes == null)
            {
                Debug.Log("Wrong logic: nodes == null");
            }

            if (nodeZ < 0 || nodeZ >= nodes.GetLength(0) || nodeX < 0 || nodeX >= nodes.GetLength(1))
            {
                Debug.Log("Wrong logic: FixBorderNode nodeX = " + nodeX + " [0.." + nodes.GetLength(1) + "], nodeZ = " + nodeZ + " [0.." + nodes.GetLength(0) + "]");
                return;
            }

            if (nodeZ < 0 || nodeZ >= nodes.GetLength(0) || nodeX < 0 || nodeX >= nodes.GetLength(1))
            {
                Debug.Log("Wrong logic: FixBorderNode nodeX = " + nodeX + " [0.." + nodes.GetLength(1) + "], nodeZ = " + nodeZ + " [0.." + nodes.GetLength(0) + "]");
                return;
            }

            //multithreading errors? some change adaptive mesh data when did process?

            uint splitFlags = nodes[nodeZ, nodeX].splitFlags;
            if (!isClearSplits)
            {
                if ((splitFlags & Node.splitBorderForceSet) != 0)
                {
                    if ((splitFlags & Node.splitBorderSplit) != 0)
                    {
                        nodes[nodeZ, nodeX].splitFlags |= splitFlag;
                    }
                    else
                    {
                        nodes[nodeZ, nodeX].splitFlags &= ~splitFlag;
                        isClearSplits = true;
                    }
                }
            }
            else
            {
                nodes[nodeZ, nodeX].splitFlags &= ~Node.splitDataMask;
            }
            if (levelIndex < levelsCount - 1)
            {
                FixBorderNode(levelIndex + 1, nodeX * 2 + 0, nodeZ * 2 + 0, splitFlag, isClearSplits);
                FixBorderNode(levelIndex + 1, nodeX * 2 + 1, nodeZ * 2 + 0, splitFlag, isClearSplits);
                FixBorderNode(levelIndex + 1, nodeX * 2 + 0, nodeZ * 2 + 1, splitFlag, isClearSplits);
                FixBorderNode(levelIndex + 1, nodeX * 2 + 1, nodeZ * 2 + 1, splitFlag, isClearSplits);
            }
        }

        private void ReconcileSplitMaps()
        {
            for (int levelIndex = levels.Length - 1; levelIndex >= 0; levelIndex--)
            {
                Level level = levels[levelIndex];
                Node[,] nodes = level.nodes;
                int nodesPerSideX = level.nodesPerSide.x;
                int nodesPerSideZ = level.nodesPerSide.y;
                int lastX = nodesPerSideX - 1;
                int lastZ = nodesPerSideZ - 1;
                for (int z = 0; z < nodesPerSideZ; z++)
                {
                    for (int x = 0; x < nodesPerSideX; x++)
                    {
                        uint splitFlags = nodes[z, x].splitFlags & ~Node.splitChildrenMask;
                        nodes[z, x].splitFlags = splitFlags;
                        if ((splitFlags & Node.splitMask) != 0)
                        {
                            if ((splitFlags & Node.splitLeft) != 0)
                            {
                                if (x > 0) nodes[z, x - 1].splitFlags |= Node.splitRight;
                            }
                            if ((splitFlags & Node.splitRight) != 0)
                            {
                                if (x < lastX) nodes[z, x + 1].splitFlags |= Node.splitLeft;
                            }
                            if ((splitFlags & Node.splitUp) != 0)
                            {
                                if (z < lastZ) nodes[z + 1, x].splitFlags |= Node.splitDown;
                            }
                            if ((splitFlags & Node.splitDown) != 0)
                            {
                                if (z > 0) nodes[z - 1, x].splitFlags |= Node.splitUp;
                            }
                        }
                    }
                }
            }
        }

        public RectInt CalcChangesRect()
        {
            Vector2Int min = Vector2Int.zero;
            Vector2Int max = Vector2Int.zero;
            for (int levelIndex = levels.Length - 1; levelIndex >= 0; levelIndex--)
            {
                Level level = levels[levelIndex];
                Node[,] nodes = level.nodes;
                int quadSize = level.quadSize;
                Vector2Int nodesPerSide = level.nodesPerSide;
                Vector2Int size = new Vector2Int(quadSize, quadSize);
                for (int z = 0; z < nodesPerSide.y; z++)
                {
                    for (int x = 0; x < nodesPerSide.x; x++)
                    {
                        uint splitFlags = nodes[z, x].splitFlags;
                        uint diff = ((splitFlags << Node.splitSnapshotShift) ^ splitFlags) & Node.splitSnapshotMask;
                        if (diff != 0)
                        {
                            Vector2Int pos = new Vector2Int(x, z) * quadSize;
                            if (max.x != 0)
                            {
                                min = Vector2Int.Min(min, pos);
                                max = Vector2Int.Max(max, pos + size);
                            }
                            else
                            {
                                min = pos;
                                max = pos + size;
                            }
                        }
                    }
                }
            }
            int quadsPerTerrain = terrainSize;
            min.x = min.x / quadsPerTerrain;
            min.y = min.y / quadsPerTerrain;
            max.x = (max.x + quadsPerTerrain - 1) / quadsPerTerrain;
            max.y = (max.y + quadsPerTerrain - 1) / quadsPerTerrain;
            return new RectInt(min, max - min);
        }

        public void ApplyMapOfHoles(Vector2Int terrainMapIndeces, bool[,] mapOfHoles)
        {
            Level level = levels[levels.Length - 1];
            int nodesPerTerrain = level.nodesPerSidePerTerrain;
            int nodeOffsetX = terrainMapIndeces.x * nodesPerTerrain;
            int nodeOffsetZ = terrainMapIndeces.y * nodesPerTerrain;
            for (int z = 0; z < nodesPerTerrain; z++)
            {
                for (int x = 0; x < nodesPerTerrain; x++)
                {
                    if (mapOfHoles[z, x])
                    {
                        level.nodes[nodeOffsetZ + z, nodeOffsetX + x].splitFlags |= Node.splitHole;
                    }
                    else
                    {
                        level.nodes[nodeOffsetZ + z, nodeOffsetX + x].splitFlags &= ~Node.splitHole;
                    }
                }
            }
        }


        #endregion

        #region 4) generate mesh

        public bool IsPresentTerrain(Vector2Int terrainMapIndeces)
        {
            if (terrainMapIndeces.x >= 0 && terrainMapIndeces.x < terrainsPerSide.x &&
                terrainMapIndeces.y >= 0 && terrainMapIndeces.y < terrainsPerSide.y)
            {
                return terrainsPresentMap[terrainMapIndeces.y, terrainMapIndeces.x];
            }
            return false;
        }

        const int writeIndexShift = 17;
        const int writeIndexMask = 0xfff << writeIndexShift;
        const int writeIndexDataMask = 0xffff;
        public class GenerateContext
        {
            public List<uint> genTriangles;
            public List<int> submeshTriangles;
            public List<Vector3> meshVertices;
            public List<Vector3> meshNormals;
            public List<GenerateAdaptiveMesh.Submesh> submeshes;
            public int[,] verticesPlacement;
            public int currentWriteIndex;
        }

        public GenerateContext CreateContext()
        {
            GenerateContext context = new GenerateContext();
            context.genTriangles = new List<uint>(heightmapSizePerTerrain * heightmapSizePerTerrain / 8);
            context.submeshTriangles = new List<int>(heightmapSizePerTerrain * heightmapSizePerTerrain / 8);
            context.meshVertices = new List<Vector3>();
            context.meshNormals = new List<Vector3>();
            context.submeshes = new List<GenerateAdaptiveMesh.Submesh>();
            context.verticesPlacement = new int[heightmapSizePerTerrain, heightmapSizePerTerrain];
            context.currentWriteIndex = 1;
            return context;
        }

        public void Clear(GenerateContext context)
        {
            context.genTriangles.Clear();
            context.submeshTriangles.Clear();
            context.meshVertices.Clear();
            context.meshNormals.Clear();
            context.submeshes.Clear();
            Array.Clear(context.verticesPlacement, 0, heightmapSizePerTerrain * heightmapSizePerTerrain);
            context.currentWriteIndex = 1;
        }

        public int DoSplitStep(Vector2Int terrainMapIndeces, float maxHeightDelta)
        {
            MakeSplitMaps(terrainMapIndeces, maxHeightDelta);
            return CalculateAproxTrianglesCount(terrainMapIndeces);
        }

        public int CalculateAproxTrianglesCount(Vector2Int terrainMapIndeces)
        {
            int nodesPerSidePerTerrain = levels[0].nodesPerSidePerTerrain;
            int terrainX = terrainMapIndeces.x * nodesPerSidePerTerrain;
            int terrainZ = terrainMapIndeces.y * nodesPerSidePerTerrain;

            int trianglesCount = 0;

            for (int z = 0; z < nodesPerSidePerTerrain; z++)
            {
                for (int x = 0; x < nodesPerSidePerTerrain; x++)
                {
                    AddTriangles(0, terrainZ + z, terrainX + x, terrainZ, terrainX, (uint v0, uint v1, uint v2) => { trianglesCount++; });
                }
            }

            return trianglesCount;
        }


        public int GenerateRawTriangles(Vector2Int terrainMapIndeces, GenerateContext context)
        {
            context.genTriangles.Clear();

            int nodesPerSidePerTerrain = levels[0].nodesPerSidePerTerrain;
            int terrainX = terrainMapIndeces.x * nodesPerSidePerTerrain;
            int terrainZ = terrainMapIndeces.y * nodesPerSidePerTerrain;

            for (int z = 0; z < nodesPerSidePerTerrain; z++)
            {
                for (int x = 0; x < nodesPerSidePerTerrain; x++)
                {
                    AddTriangles(0, terrainZ + z, terrainX + x, terrainZ, terrainX,
                        (uint v0, uint v1, uint v2) =>
                        {
                            context.genTriangles.Add(v0);
                            context.genTriangles.Add(v1);
                            context.genTriangles.Add(v2);
                        });
                }
            }

            return context.genTriangles.Count;
        }

        private delegate void AddTriangle(uint v0, uint v1, uint v2);

        private void AddTriangles(int levelIndex, int z, int x, int terrainZ, int terrainX, AddTriangle addTriangle)
        {
            Level level = levels[levelIndex];
            uint splitFlags = level.nodes[z, x].splitFlags;
            if ((splitFlags & Node.splitHole) != 0)
            {
                return;
            }
            int nextLevel = levelIndex + 1;
            int cx = x * 2;
            int cz = z * 2;
            if ((splitFlags & Node.splitPresentChild00) != 0) AddTriangles(nextLevel, cz + 0, cx + 0, terrainZ * 2, terrainX * 2, addTriangle);
            if ((splitFlags & Node.splitPresentChild01) != 0) AddTriangles(nextLevel, cz + 0, cx + 1, terrainZ * 2, terrainX * 2, addTriangle);
            if ((splitFlags & Node.splitPresentChild11) != 0) AddTriangles(nextLevel, cz + 1, cx + 1, terrainZ * 2, terrainX * 2, addTriangle);
            if ((splitFlags & Node.splitPresentChild10) != 0) AddTriangles(nextLevel, cz + 1, cx + 0, terrainZ * 2, terrainX * 2, addTriangle);
            int x0 = (x - terrainX) * level.quadSize;
            int z0 = (z - terrainZ) * level.quadSize;
            int x1 = x0 + level.quadSize;
            int z1 = z0 + level.quadSize;
            uint v00 = PackVertex(z0, x0); uint v01 = PackVertex(z0, x1);
            uint v10 = PackVertex(z1, x0); uint v11 = PackVertex(z1, x1);
            int xc = x0 + level.quadSize / 2; int zc = z0 + level.quadSize / 2;
            uint vcc = PackVertex(zc, xc);
            //Down
            if ((splitFlags & Node.splitDown) != 0)
            {
                uint v0c = PackVertex(z0, xc);
                if ((splitFlags & Node.splitPresentChild00) == 0) addTriangle(v00, vcc, v0c);
                if ((splitFlags & Node.splitPresentChild01) == 0) addTriangle(v0c, vcc, v01);
            }
            else
            {
                addTriangle(v00, vcc, v01);
            }
            //Right
            if ((splitFlags & Node.splitRight) != 0)
            {
                uint vc1 = PackVertex(zc, x1);
                if ((splitFlags & Node.splitPresentChild01) == 0) addTriangle(v01, vcc, vc1);
                if ((splitFlags & Node.splitPresentChild11) == 0) addTriangle(vc1, vcc, v11);
            }
            else
            {
                addTriangle(v01, vcc, v11);
            }
            //Up
            if ((splitFlags & Node.splitUp) != 0)
            {
                uint v1c = PackVertex(z1, xc);
                if ((splitFlags & Node.splitPresentChild11) == 0) addTriangle(v11, vcc, v1c);
                if ((splitFlags & Node.splitPresentChild10) == 0) addTriangle(v1c, vcc, v10);
            }
            else
            {
                addTriangle(v11, vcc, v10);
            }
            //Left
            if ((splitFlags & Node.splitLeft) != 0)
            {
                uint vc0 = PackVertex(zc, x0);
                if ((splitFlags & Node.splitPresentChild10) == 0) addTriangle(v10, vcc, vc0);
                if ((splitFlags & Node.splitPresentChild00) == 0) addTriangle(vc0, vcc, v00);
            }
            else
            {
                addTriangle(v10, vcc, v00);
            }
        }

        private uint PackVertex(int z, int x)
        {
            return (((uint)z & 0xffff) << 16) | ((uint)x & 0xffff);
        }

        private void UnpackVertex(uint packVertex, out int z, out int x)
        {
            x = (int)(packVertex & 0xffff);
            z = (int)((packVertex >> 16) & 0xffff);
        }

        private void AddTriangle1(uint v0, uint v1, uint v2, GenerateContext context)
        {
            context.genTriangles.Add(v0);
            context.genTriangles.Add(v1);
            context.genTriangles.Add(v2);
        }

        public GenerateAdaptiveMesh BuildMesh(Vector2Int terrainMapIndeces, GenerateContext context)
        {
            context.submeshes.Clear();
            context.meshVertices.Clear();
            context.meshNormals.Clear();
            Vector2Int heightsMapOffset = terrainMapIndeces * terrainSize;
            List<Vector3> meshVertices = context.meshVertices;
            List<Vector3> meshNormals = context.meshNormals;
            List<GenerateAdaptiveMesh.Submesh> submeshes = context.submeshes;
            if (context.currentWriteIndex > writeIndexMask - 10)
            {
                for (int z = 0; z < heightmapSizePerTerrain; z++)
                {
                    for (int x = 0; x < heightmapSizePerTerrain; x++)
                    {
                        context.verticesPlacement[z, x] = 0;
                    }
                }
                context.currentWriteIndex = 1;
            }

            int i = 0;
            while (i < context.genTriangles.Count)
            {
                context.submeshTriangles.Clear();
                context.currentWriteIndex++;
                int currentWriteIndex = context.currentWriteIndex << writeIndexShift;

                GenerateAdaptiveMesh.Submesh subMesh = new GenerateAdaptiveMesh.Submesh();
                subMesh.baseVertexIndex = meshVertices.Count;

                int vertexIndex = 0;
                for (; i < context.genTriangles.Count; i += 3)
                {
                    if (vertexIndex > 65500) break;
                    for (int j = 0; j < 3; j++)
                    {
                        int x, z;
                        UnpackVertex(context.genTriangles[i + j], out z, out x);
                        if ((context.verticesPlacement[z, x] & writeIndexMask) != currentWriteIndex)
                        {
                            context.verticesPlacement[z, x] = vertexIndex | currentWriteIndex;
                            vertexIndex++;

                            float height = heightMap[z + heightsMapOffset.y + 1, x + heightsMapOffset.x + 1];
                            height = (int)(height * 65535.0f) / 65535.0f;

                            Vector3 v = new Vector3();
                            v.x = x;
                            v.y = height * heightScale;
                            v.z = z;
                            meshVertices.Add(v);

                            /*
                            if (isCalculateMeshNormals)
                            {
                                float dh = heightMap[z + heightsMapOffset.y + 1, x + heightsMapOffset.x + 1 - 1];
                                dh = dh >= 0.0f ? dh * heightScale - v.y : 0.0f;
                                Vector3 n0 = new Vector3(dh, 1, 0);
                                dh = heightMap[z + heightsMapOffset.y + 1, x + heightsMapOffset.x + 1 + 1];
                                dh = dh >= 0.0f ? dh * heightScale - v.y : 0.0f;
                                Vector3 n1 = new Vector3(-dh, 1, 0);
                                dh = heightMap[z + heightsMapOffset.y + 1 - 1, x + heightsMapOffset.x + 1];
                                dh = dh >= 0.0f ? dh * heightScale - v.y : 0.0f;
                                Vector3 n2 = new Vector3(0, 1, dh);
                                dh = heightMap[z + heightsMapOffset.y + 1 + 1, x + heightsMapOffset.x + 1];
                                dh = dh >= 0.0f ? dh * heightScale - v.y : 0.0f;
                                Vector3 n3 = new Vector3(0, 1, -dh);
                            }
                            else*/
                            {
                                meshNormals.Add(new Vector3(0, 1, 0));
                            }
                        }
                        context.submeshTriangles.Add(context.verticesPlacement[z, x] & writeIndexDataMask);
                    }
                }

                subMesh.triangles = context.submeshTriangles.ToArray();
                submeshes.Add(subMesh);
            }
            GenerateAdaptiveMesh genMesh = new GenerateAdaptiveMesh();
            genMesh.subMeshes = submeshes.ToArray();
            genMesh.vertices = meshVertices.ToArray();
            genMesh.normals = meshNormals.ToArray();
            genMesh.terrainPosInMap = terrainMapIndeces;

            Vector2[] uv = new Vector2[meshVertices.Count];
            float terrainSizeScale = 1.0f / terrainSize;
            for (int j = 0; j < meshVertices.Count; j++)
            {
                uv[j] = new Vector2(genMesh.vertices[j].x, genMesh.vertices[j].z) * terrainSizeScale;
            }
            genMesh.uv = uv;

            return genMesh;
        }

        #endregion

    };

}

#endif
