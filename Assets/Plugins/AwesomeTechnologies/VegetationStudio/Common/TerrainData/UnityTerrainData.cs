
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR && !VS_NO_HEIGHTMAP_DLL
#define VS_USE_HEIGHTMAP_DLL
#endif

using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies
{
    [System.Serializable]
    public class UnityTerrainData : ITerrainData
    {
        private int _heightmapHeight;
        private int _heightmapWidth;
        private int _alphamapHeight;
        private int _alphamapWidth;
        private float _alphamapCellWidth;
        private float _alphamapCellHeight;
        private int _alphamapLayers;
        public float[,,] splatmapData;

        private int _detailMapHeight;
        private int _detailMapWidth;
        private int _detailLayers;
        private int[][,] _detailLayersData;

        public Vector3 _size;
        public Vector3 _scale;
        public Vector3 _heightmapScale;
        public Vector3 _terrainPosition;

        public Bounds TerrainBounds;
        public Bounds VegetationBounds;
        public float MaxTerrainHeight;

        private bool _hasDetailMap;
        



#if VS_USE_HEIGHTMAP_DLL
        public HeightMapUtilDLL HeightmapMapUtilDll;
#endif
        public int heightmapHeight
        {
            get
            {
                return _heightmapHeight;
            }
        }

        public Vector3 heightmapScale
        {
            get
            {
                return _heightmapScale;
            }
        }

        public int heightmapWidth
        {
            get
            {
                return _heightmapWidth;
            }

        }

        public Vector3 size
        {
            get
            {
                return _size;
            }
        }

        public Vector3 terrainPosition
        {
            get
            {
                return _terrainPosition;
            }
        }

        public int alphamapWidth
        {
            get
            {
                return _alphamapWidth;
            }
        }

        public int alphamapHeight
        {
            get
            {
                return _alphamapHeight;
            }
        }

        public int detailMapWidth
        {
            get
            {
                return _detailMapWidth;
            }
        }

        public int detailMapHeight
        {
            get
            {
                return _detailMapHeight;
            }
        }

        public bool HasDetailMap
        {
            get
            {
                return _hasDetailMap;
            }
        }

        public float alphamapCellWidth
        {
            get
            {
                return _alphamapCellWidth;
            }
        }

        public float alphamapCellHeight
        {
            get
            {
                return _alphamapCellHeight;
            }
        }

        public int alphamapLayers
        {
            get
            {
                return _alphamapLayers;
            }
        }

        public Terrain terrain
        {
            get
            {
                return _terrain;
            }
        }

        private Terrain _terrain;

        [SerializeField]
        public float[,] _heights;

        public void CalculateMaxHeight()
        {
            float maxHeight = 0;
            for (int x = 0; x <= _heightmapWidth - 1; x++)
            {
                for (int z = 0; z <= _heightmapHeight - 1; z++)
                {
                    if (maxHeight < _heights[z, x]) maxHeight = _heights[z, x];
                }
            }
            MaxTerrainHeight =  maxHeight * _heightmapScale.y;
        }
        public UnityTerrainData(Terrain terrain, bool readTerrainBuffers = true, bool readDetailData = false)
        {
            _terrain = terrain;

            _size = _terrain.terrainData.size;
            _heightmapScale = _terrain.terrainData.heightmapScale;
            _terrainPosition = _terrain.transform.position;
            _heightmapHeight = _terrain.terrainData.heightmapResolution;
            _heightmapWidth = _terrain.terrainData.heightmapResolution;

            _alphamapHeight = _terrain.terrainData.alphamapHeight;
            _alphamapWidth = _terrain.terrainData.alphamapWidth;
            _alphamapCellWidth = _size.x / _alphamapWidth;
            _alphamapCellHeight = _size.z / _alphamapHeight;
            _alphamapLayers = _terrain.terrainData.alphamapLayers;


            _detailMapHeight = _terrain.terrainData.detailHeight;
            _detailMapWidth = _terrain.terrainData.detailWidth;
            _detailLayers = _terrain.terrainData.detailPrototypes.Length;

            _heights = new float[_heightmapWidth, _heightmapHeight];

            _scale.x = size.x / (_heightmapWidth - 1);
            _scale.y = size.y;
            _scale.z = size.z / (_heightmapHeight - 1);

            splatmapData = new float[_alphamapWidth, _alphamapHeight, _alphamapLayers];

            CalculateBounds();

            if (!Application.isPlaying)
            {
                //if (readTerrainBuffers)
                {
                    RefreshHeightData(new Bounds(), false);
                    RefreshSplatMap(new Bounds(), false);
                }

                //if (readDetailData)
                {
                    RefreshDetailData();

                }
            }

#if VS_USE_HEIGHTMAP_DLL
            HeightmapMapUtilDll = new HeightMapUtilDLL(terrain.terrainData);
            //Debug.Log("Creating HeightmapDLL");
#endif
        }

        public void OnDestroy()
        {
            _heights = new float[0, 0];
            splatmapData = new float[0, 0, 0];

#if VS_USE_HEIGHTMAP_DLL
            if (HeightmapMapUtilDll != null)
            {
                HeightmapMapUtilDll.Destroy();
                HeightmapMapUtilDll = null;
            }
#endif
        }

        public void UpdatePosition()
        {
            _terrainPosition = _terrain.transform.position;
            CalculateBounds();
        }

        void CalculateBounds()
        {
            TerrainBounds = new Bounds(terrainPosition, new Vector3(0, 0, 0));
            foreach (var r in _terrain.GetComponentsInChildren<TerrainCollider>())
            {
                TerrainBounds.Encapsulate(r.bounds);
            }

            Bounds terrainDataBounds = _terrain.terrainData.bounds;
            terrainDataBounds.center = terrainDataBounds.center +  new Vector3(terrainPosition.x, 0, terrainPosition.z);

            TerrainBounds.Encapsulate(terrainDataBounds);

            VegetationBounds = new Bounds(TerrainBounds.center, TerrainBounds.size);
        }

        public float GetInterpolatedHeight(float x, float y)
        {
            return new_GetTriangleInterpolatedHeight(x, y);
        }

        public Vector3 GetInterpolatedNormal(float x, float y)
        {
            return new_GetInterpolatedNormal(x, y);
        }

        public float GetSteepness(float x, float y)
        {
            return new_GetSteepness(x, y);
        }
        public Rect GetTerrainRect(float cellSize)
        {
            Rect terrainRect = new Rect(terrainPosition.x, terrainPosition.z, size.x, size.z);
            terrainRect.xMin = terrainRect.xMin - cellSize;
            terrainRect.yMin = terrainRect.yMin - cellSize;
            terrainRect.width = terrainRect.width + cellSize * 2;
            terrainRect.height = terrainRect.height + cellSize * 2;

            return terrainRect;
        }

        public void RefreshDetailData()
        {
            _detailLayersData = new int[_detailLayers][,];
            if (_detailLayers > 0) _hasDetailMap = true;


            //int maxValue = int.MinValue;

            for (int i = 0; i <= _detailLayers - 1; i++)
            {
                _detailLayersData[i] = _terrain.terrainData.GetDetailLayer(0, 0, _detailMapWidth, _detailMapHeight, i);

                //for (int x = 0; x <= _detailMapWidth - 1; x++)
                //{
                //    for (int y = 0; y <= _detailMapHeight - 1; y++)
                //    {
                //        if (_detailLayersData[i][x,y] > maxValue)
                //        {
                //            maxValue = _detailLayersData[i][x, y];
                //        }
                //    }
                //}
            }

            //Debug.Log(maxValue);
        }

        public int SampleDetailMap(int x, int y, int layer)
        {           
            if (layer >= _detailLayers || layer < 0) return 0;
            if (x < 0 || x > _detailMapWidth - 1) return 0;
            if (y < 0 || y > _detailMapHeight  - 1) return 0;
            return _detailLayersData[layer][y, x];
        }

        public void RefreshHeightData()
        {
            RefreshHeightData(new Bounds(), false);
        }
        public void RefreshHeightData(Bounds bounds, bool useBounds)
        {
            if (useBounds)
            {
                int newWidth = Mathf.CeilToInt(bounds.size.x / _heightmapScale.x);
                int newHeight = Mathf.CeilToInt(bounds.size.x / _heightmapScale.z);
                int newStartX = Mathf.RoundToInt(((bounds.center.x - bounds.extents.x) - _terrainPosition.x) / _heightmapScale.x);
                int newStartZ = Mathf.RoundToInt(((bounds.center.z - bounds.extents.z) - _terrainPosition.z) / _heightmapScale.z);
                if (newStartX < 0) newStartX = 0;
                if (newStartZ < 0) newStartZ = 0;
                if (newStartX + newWidth > _heightmapWidth) newWidth = _heightmapWidth - newStartX - 1;
                if (newStartZ + newHeight > _heightmapHeight) newHeight = _heightmapHeight - newStartZ - 1;


                if (newWidth <= 0 || newHeight <= 0) return;
                float[,] newHeights = _terrain.terrainData.GetHeights(newStartX, newStartZ, newWidth, newHeight);

                for (int x = 0; x <= newWidth - 1; x++)
                {
                    for (int z = 0; z <= newHeight - 1; z++)
                    {
                        _heights[z + newStartZ, x + newStartX] = newHeights[z, x];
                    }
                }
            }
            else
            {
                _heights = _terrain.terrainData.GetHeights(0, 0, _heightmapWidth, _heightmapHeight);
            }

            CalculateMaxHeight();

#if VS_USE_HEIGHTMAP_DLL
            if (HeightmapMapUtilDll != null)
            {
                HeightmapMapUtilDll.SetHeights(_heights);
            }           
#endif
        }

        public void RefreshSplatMap()
        {
            RefreshSplatMap(new Bounds(), false);
        }

        public void RefreshSplatMap(Bounds bounds, bool useBounds)
        {            
            if (useBounds)
            {
                int newWidth = Mathf.CeilToInt(bounds.size.x / alphamapCellWidth);
                int newHeight = Mathf.CeilToInt(bounds.size.z / alphamapCellHeight);
                float boundsStartX = bounds.center.x - bounds.extents.x;
                float boundsStartY = bounds.center.z - bounds.extents.z;
                float positionStartX = boundsStartX - terrainPosition.x;
                float positionStartY = boundsStartY - terrainPosition.z;
                int newStartX = Mathf.RoundToInt(positionStartX / alphamapCellWidth);
                int newStartY = Mathf.RoundToInt(positionStartY / alphamapCellHeight);

                if (newStartX < 0) newStartX = 0;
                if (newStartY < 0) newStartY = 0;
                if (newStartX + newWidth > alphamapWidth) newWidth = alphamapWidth - newStartX;
                if (newStartY + newHeight >alphamapHeight) newHeight = alphamapHeight - newStartY;

                if (newWidth <= 0 || newHeight <= 0) return;
                int calculatedAlphamapWidth = newWidth;
                int calculatedAlphamapHeight = newHeight;
                int startX = newStartX;
                int startY = newStartY;

               
                float[,,] tempSplatmapData = _terrain.terrainData.GetAlphamaps(startX, startY, calculatedAlphamapWidth, calculatedAlphamapHeight);

                for (int y = 0; y < calculatedAlphamapHeight - 1; y++)
                {
                    for (int x = 0; x < calculatedAlphamapWidth - 1; x++)
                    {
                        for (int i = 0; i < _alphamapLayers; i++)
                        {
                            splatmapData[y + startY, x + startX, i] = tempSplatmapData[y, x, i];
                        }
                    }
                }
            }
            else
            {
                splatmapData = _terrain.terrainData.GetAlphamaps(0, 0, _alphamapWidth, _alphamapHeight);
            }           
        }

        public void SampleTerrain(float xNorm, float yNorm, out Vector3 newPos, out Vector3 normal, out Vector3 barycentric)
        {
            float x = xNorm * (_heightmapWidth - 1f);
            float z = yNorm * (_heightmapHeight - 1f);

            Vector3 worldPos = new Vector3(xNorm * _size.x, 0, yNorm * _size.z);

            int activeTriangle;
            //Vector3 barycentric;

            int x1 = Mathf.FloorToInt(x);  // Fllor x
            int x2 = x1 + 1;                // Ceil  x
            int z1 = Mathf.FloorToInt(z);  // Floor y
            int z2 = z1 + 1;                // Ceil  y

            if (z2 == _heightmapHeight) z2 = z1;
            if (x2 == _heightmapWidth) x2 = x1;

            float h11 = _heights[z1, x1] * _size.y;
            float h12 = _heights[z1, x2] * _size.y;
            float h21 = _heights[z2, x1] * _size.y;
            float h22 = _heights[z2, x2] * _size.y;

            // ======================================================================================================================================================
            // calculate the two values which are forming the lower left point of the quad patch
            // later by knowing and using the quad patch size we can calculate all other quad patch points
            float pX1 = Mathf.Lerp(0f, _size.x, ((float)x1 / (_heightmapWidth - 1f)));
            //float pX2 = Mathf.Lerp(0f, 30f, ((float)x2 / 128));   // Not needed
            float pZ1 = Mathf.Lerp(0f, _size.z, ((float)z1 / (_heightmapHeight - 1f)));
            //float pZ2 = Mathf.Lerp(0f, 30f, ((float)z2 / 128));   // Not needed

            // ======================================================================================================================================================
            // Calculate the quad patch four points
            Vector3 p1 = new Vector3(pX1, h11, pZ1);
            Vector3 p2 = new Vector3(pX1 + _heightmapScale.x, h12, pZ1);
            Vector3 p3 = new Vector3(pX1, h21, pZ1 + _heightmapScale.z);
            Vector3 p4 = new Vector3(pX1 + _heightmapScale.x, h22, pZ1 + _heightmapScale.z);

            // ======================================================================================================================================================
            // Calculatin in which of the two triangles the given point is lying
            // Find where is the point located - in which of both triangles
            Vector3 edge1 = Vector3.forward;
            Vector3 aP0 = p1; aP0.y = 0f;
            Vector3 edge2 = (new Vector3(worldPos.x, 0f, worldPos.z) - aP0).normalized;
            float angle = Vector3.Angle(edge1, edge2);
            if (angle > 45f)
            {
                activeTriangle = 1;     // down/right triangle
            }
            else activeTriangle = 0;    // up/leftright triangle

            Vector3 pN1 = p1; pN1.y = 0f;
            Vector3 pN2 = p2; pN2.y = 0f;
            Vector3 pN3 = p3; pN3.y = 0f;
            Vector3 pN4 = p4; pN4.y = 0f;
            if (activeTriangle == 0)
            {
                barycentric = Barycentric(pN1, pN3, pN4, worldPos);
                newPos = p1 * barycentric.x + p3 * barycentric.y + p4 * barycentric.z;
                normal = Vector3.Cross(p1 - p3, p1 - p4).normalized;
            }
            else
            {
                barycentric = Barycentric(pN1, pN4, pN2, worldPos);
                newPos = p1 * barycentric.x + p4 * barycentric.y + p2 * barycentric.z;
                normal = Vector3.Cross(p1 - p4, p1 - p2).normalized;
            }
        }

        public Vector3 Barycentric(Vector3 a, Vector3 b, Vector3 c, Vector3 I)
        {
            Vector3 barycentric;

            Vector3 v0 = b - a;
            Vector3 v1 = c - a;
            Vector3 v2 = I - a;

            float d00 = v0.x * v0.x + v0.y * v0.y + v0.z * v0.z;//float d00 = Vector3.Dot(v0, v0);
            float d01 = v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;//float d01 = Vector3.Dot(v0, v1);

            float d11 = v1.x * v1.x + v1.y * v1.y + v1.z * v1.z;//float d11 = Vector3.Dot(v1, v1);
            float d20 = v2.x * v0.x + v2.y * v0.y + v2.z * v0.z;//float d20 = Vector3.Dot(v2, v0);
            float d21 = v2.x * v1.x + v2.y * v1.y + v2.z * v1.z;//float d21 = Vector3.Dot(v2, v1); 

            float denom = d00 * d11 - d01 * d01;

            barycentric.y = (d11 * d20 - d01 * d21) / denom;
            barycentric.z = (d00 * d21 - d01 * d20) / denom;
            barycentric.x = 1.0f - barycentric.z - barycentric.y;

            return barycentric;
        }

        public float new_GetTriangleInterpolatedHeight(float x, float y)
        {
#if VS_USE_HEIGHTMAP_DLL
            return HeightmapMapUtilDll.GetInterpolatedHeight(x, y);
#else
            float fx = x * (_heightmapWidth - 1);
            float fy = y * (_heightmapHeight - 1);
            int lx = (int)fx;
            int ly = (int)fy;

            float u = fx - lx;
            float v = fy - ly;
            if (u > v)
            {
                float z00 = new_GetHeight(lx + 0, ly + 0);
                float z01 = new_GetHeight(lx + 1, ly + 0);
                float z11 = new_GetHeight(lx + 1, ly + 1);
                return z00 + (z01 - z00) * u + (z11 - z01) * v;
            }
            else
            {
                float z00 = new_GetHeight(lx + 0, ly + 0);
                float z10 = new_GetHeight(lx + 0, ly + 1);
                float z11 = new_GetHeight(lx + 1, ly + 1);
                return z00 + (z11 - z10) * u + (z10 - z00) * v;
            }
#endif
        }

        public float GetQuadHeightInterpolatedNormalized(float x, float z)
        {
            float _x = x * (_heightmapWidth - 1f);
            float _z = z * (_heightmapHeight - 1f);

            int x1 = Mathf.FloorToInt(_x);
            int x2 = x1 + 1;
            int z1 = Mathf.FloorToInt(_z);
            int z2 = z1 + 1;

            float q11 = GetHeight(x1, z1);
            float q12 = GetHeight(x1, z2);
            float q21 = GetHeight(x2, z1);
            float q22 = GetHeight(x2, z2);

            float r1 = ((x2 - _x) / (x2 - x1)) * q11 + ((_x - x1) / (x2 - x1)) * q21;
            float r2 = ((x2 - _x) / (x2 - x1)) * q12 + ((_x - x1) / (x2 - x1)) * q22;

            return ((z2 - _z) / (z2 - z1)) * r1 + ((_z - z1) / (z2 - z1)) * r2;
        }


        //public Vector3 GetNormalFast(float normalizedX, float normalizedZ)
        //{
        //    float x = normalizedX * (_heightmapWidth - 1f);
        //    float z = normalizedZ * (_heightmapHeight - 1f);

        //    int x1 = Mathf.FloorToInt(x);
        //    int x2 = x1 + 1;
        //    int z1 = Mathf.FloorToInt(z);
        //    int z2 = z1 + 1;

        //    float q11 = GetHeight(x1, z1);
        //    float q12 = GetHeight(x1, z2);
        //    float q21 = GetHeight(x2, z1);
        //    float q22 = GetHeight(x2, z2);

        //    Vector3 vertical = new Vector3(0.0f, q11 - q12, 2.0f );
        //    Vector3 horizontal = new Vector3(2.0f, q22 - q21, 0.0f );
        //    Vector3 normal = Vector3.Cross(vertical, horizontal);
        //    normal.Normalize();
        //    return normal;
        //}

        float GetHeight(int x, int z)
        {
            if (x < 0 || x > _heightmapWidth - 1) return 0;
            if (z < 0 || z > _heightmapHeight - 1) return 0;
            return _heights[z, x] * _heightmapScale.y;
        }

        public float new_GetHeight(int x, int y)
        {
            x = Mathf.Clamp(x, 0, _heightmapWidth - 1);
            y = Mathf.Clamp(y, 0, _heightmapHeight - 1);
            return _heights[y, x] * _heightmapScale.y;
        }


        public float new_GetSteepness(float x, float y)
        {
            float steepness = Vector3.Dot(new_GetInterpolatedNormal(x, y), new Vector3(0.0F, 1.0F, 0.0F));
            steepness = Mathf.Rad2Deg * Mathf.Acos(steepness);
            return steepness;
        }

        public Vector3 new_GetInterpolatedNormal(float x, float y)
        {
#if VS_USE_HEIGHTMAP_DLL
            return HeightmapMapUtilDll.GetInterpolatedNormal(x, y);
#else
            float fx = x * (_heightmapWidth - 1);
            float fy = y * (_heightmapHeight - 1);
            int lx = (int)fx;
            int ly = (int)fy;

            Vector3 n00 = new_CalculateNormalSobel(lx + 0, ly + 0);
            Vector3 n10 = new_CalculateNormalSobel(lx + 1, ly + 0);
            Vector3 n01 = new_CalculateNormalSobel(lx + 0, ly + 1);
            Vector3 n11 = new_CalculateNormalSobel(lx + 1, ly + 1);

            float u = fx - lx;
            float v = fy - ly;

            Vector3 s = Vector3.Lerp(n00, n10, u);
            Vector3 t = Vector3.Lerp(n01, n11, u);
            Vector3 value = Vector3.Lerp(s, t, v);

            value.Normalize();
            return value;
#endif
        }

        Vector3 new_CalculateNormalSobel(int x, int y)
        {
            Vector3 normal;
            float dY, dX;
            dX = new_GetHeight(x - 1, y - 1) * -1.0F;
            dX += new_GetHeight(x - 1, y) * -2.0F;
            dX += new_GetHeight(x - 1, y + 1) * -1.0F;
            dX += new_GetHeight(x + 1, y - 1) * 1.0F;
            dX += new_GetHeight(x + 1, y) * 2.0F;
            dX += new_GetHeight(x + 1, y + 1) * 1.0F;

            dX /= _scale.x;

            dY = new_GetHeight(x - 1, y - 1) * -1.0F;
            dY += new_GetHeight(x, y - 1) * -2.0F;
            dY += new_GetHeight(x + 1, y - 1) * -1.0F;
            dY += new_GetHeight(x - 1, y + 1) * 1.0F;
            dY += new_GetHeight(x, y + 1) * 2.0F;
            dY += new_GetHeight(x + 1, y + 1) * 1.0F;
            dY /= _scale.z;

            normal.x = -dX;
            normal.y = 8;
            normal.z = -dY;

            normal.Normalize();
            return normal;
        }      
    }
}