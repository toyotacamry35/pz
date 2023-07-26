#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace Assets.TerrainBaker
{

    public class TerrainMaterialProcessData
    {
        public TerrainData terrainData;
        public Texture2D matTexture;
        public Texture2D weightsTexture;
        public TerrainMaterialsArray materialsArray;
        public byte[] remapLayersToAtlasMaterials;
        public RectInt dirtyRect;
        public bool isEnableDump;
    }

    public class TerrainMaterialsBuilder
    {
        private const string dumpPath = "D:/TerrainBakerDebug/";

        //private int lastFrameProcess = -1;
        private List<TerrainMaterialProcessData> processData;

        private Material mtlSortLayers = null;
        //private Material mtlMakePreferenceMap = null;
        //private Material mtlSortPreferenceMap = null;
        private Material mtlPaintLayers = null;
        private Material mtlPaintWeights = null;
        private Material mtlReindexLayerToMaterials = null;

        private Texture2D sortLayersTexture = null;
        private Texture2D matTextureTmp = null;

        private int alphaMapSize = 0;
        private int matMapSize = 0;
        private int layersCount = 0;

        private float[] remapLayersToAtlasMaterials;

        #region Init

        public void Init(int _alphaMapSize, int _matMapSize, int _layersCount)
        {
            processData = new List<TerrainMaterialProcessData>();

            alphaMapSize = _alphaMapSize;
            matMapSize = _matMapSize;
            layersCount = _layersCount;

            mtlSortLayers = CreateMaterial("Hidden/SortTerrainLayers");
            mtlPaintLayers = CreateMaterial("Hidden/PaintTerrainLayers");
            mtlPaintWeights = CreateMaterial("Hidden/PaintTerrainWeights");
            mtlReindexLayerToMaterials = CreateMaterial("Hidden/ReindexMaterialMap");

            sortLayersTexture = CreateTexture(alphaMapSize);
            matTextureTmp = CreateTexture(matMapSize);
        }

        private Material CreateMaterial(string shaderName)
        {
            Material mtl = new Material(Shader.Find(shaderName));
            mtl.shader.hideFlags = HideFlags.HideAndDontSave;
            mtl.hideFlags = HideFlags.HideAndDontSave;
            return mtl;
        }

        private Texture2D CreateTexture(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false, true);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.anisoLevel = 1;
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }

        private void SetMaterialsParams()
        {
            mtlSortLayers.SetFloat("_TexelSize", 1.0f / (alphaMapSize - 1));

            mtlPaintLayers.SetFloat("_HalfLayersTexelSize", 0.5f / alphaMapSize);
            mtlPaintLayers.SetFloat("_TextureScale", matMapSize / (float)alphaMapSize);
            mtlPaintLayers.SetFloat("_HalfBufferTexelSize", 0.5f / matMapSize);
            mtlPaintLayers.SetFloat("_BufferSize", matMapSize);
            mtlPaintLayers.SetVector("_CurrentQuad", Vector4.one);
            mtlPaintLayers.SetInt("_ChannelIndex", 0);

            mtlPaintWeights.SetFloat("_MaterialMapSize", (float)matMapSize);
        }

        #endregion

        #region Add terrain for process

        public void AddMaterialsForProcess(TerrainMaterialProcessData data)
        {
            int index = processData.FindIndex(x => x.terrainData == data.terrainData);
            if (index < 0)
            {
                processData.Add(data);
            }
            else
            {
                processData[index] = data;
            }
        }

        #endregion

        #region Build process

        public void Process()
        {
            /*
            if (Time.frameCount == lastFrameProcess || processData.Count == 0)
            {
                return;
            }
            lastFrameProcess = Time.frameCount;
            */

            if (processData.Count == 0)
            {
                return;
            }

            TerrainMaterialProcessData data = processData[0];
            processData.RemoveAt(0);

            Texture2D[] alphamapTextures = data.terrainData.alphamapTextures;
            if (alphamapTextures.Length >= layersCount * 4)
            {
                Debug.Log("Terrain logic error: alphamapTextures.Length >= layersCount*4");
                return;
            }

            if (data.isEnableDump)
            {
                for (int i = 0; i < alphamapTextures.Length; i++)
                {
                    DumpTexture(alphamapTextures[i], "alphamapTexture" + i + ".png");
                }
            }

            SetMaterialsParams();

            //Sort materials map
            SetAlphamapsToShader(mtlSortLayers, alphamapTextures);
            ProcessTexture(mtlSortLayers, sortLayersTexture, 0);
            if (data.materialsArray != null)
            {
                Color32[] sortPixels = sortLayersTexture.GetPixels32(0);
                if (data.materialsArray.materialsMap.Length == sortPixels.Length)
                {
                    for (int i = 0; i < sortPixels.Length; i++)
                    {
                        byte index = sortPixels[i].r;
                        byte materialIndex = index < data.remapLayersToAtlasMaterials.Length ? data.remapLayersToAtlasMaterials[index] : TerrainMaterialsArray.noMaterial;
                        data.materialsArray.materialsMap[i] = materialIndex;
                    }
                    EditorUtility.SetDirty(data.materialsArray);
                }
                else
                {
                    Debug.Log("Terrain logic error: data.materialsArray.materialsMap.Length != sortPixels.Length");
                }
            }
            if (data.isEnableDump)
            {
                DumpTexture(sortLayersTexture, "sortLayersTexture.png");
            }
            //Materials
            mtlPaintLayers.SetTexture("_SortedLayers", sortLayersTexture);
            ProcessTexture(mtlPaintLayers, matTextureTmp, 0);
            if (data.isEnableDump)
            {
                DumpTexture(matTextureTmp, "matTextureTmp.png");
            }

            //Weights
            mtlPaintWeights.SetTexture("_Materials", matTextureTmp);
            SetAlphamapsToShader(mtlPaintWeights, alphamapTextures);
            ProcessTexture(mtlPaintWeights, data.weightsTexture, 0);
            if (data.isEnableDump)
            {
                DumpTexture(data.weightsTexture, "weightsTexture.png");
            }

            //Reindex layers to atlas materials
            if (remapLayersToAtlasMaterials == null || remapLayersToAtlasMaterials.Length != data.remapLayersToAtlasMaterials.Length)
            {
                remapLayersToAtlasMaterials = new float[data.remapLayersToAtlasMaterials.Length];
            }
            for (int i = 0; i < data.remapLayersToAtlasMaterials.Length; i++)
            {
                remapLayersToAtlasMaterials[i] = data.remapLayersToAtlasMaterials[i] * (1.0f / 255.0f);
            }
            mtlReindexLayerToMaterials.SetFloatArray("_RemapIndeces", remapLayersToAtlasMaterials);
            mtlReindexLayerToMaterials.SetTexture("_Materials", matTextureTmp);
            ProcessTexture(mtlReindexLayerToMaterials, data.matTexture, 0);
            if (data.isEnableDump)
            {
                DumpTexture(data.matTexture, "matTexture.png");
            }
            SceneView.RepaintAll();
        }

        private void SetAlphamapsToShader(Material mtl, Texture2D[] alphmapTextures)
        {
            if (mtl == null) return;
            mtl.SetTexture("_Layers0", (alphmapTextures.Length > 0) ? alphmapTextures[0] : Texture2D.blackTexture);
            mtl.SetTexture("_Layers1", (alphmapTextures.Length > 1) ? alphmapTextures[1] : Texture2D.blackTexture);
            mtl.SetTexture("_Layers2", (alphmapTextures.Length > 2) ? alphmapTextures[2] : Texture2D.blackTexture);
            mtl.SetTexture("_Layers3", (alphmapTextures.Length > 3) ? alphmapTextures[3] : Texture2D.blackTexture);
            mtl.SetTexture("_Layers4", (alphmapTextures.Length > 4) ? alphmapTextures[4] : Texture2D.blackTexture);
            mtl.SetTexture("_Layers5", (alphmapTextures.Length > 5) ? alphmapTextures[5] : Texture2D.blackTexture);
            mtl.SetTexture("_Layers6", (alphmapTextures.Length > 6) ? alphmapTextures[6] : Texture2D.blackTexture);
            mtl.SetTexture("_Layers7", (alphmapTextures.Length > 7) ? alphmapTextures[7] : Texture2D.blackTexture);
        }

        void ProcessTexture(Material mtl, Texture2D tex, int size)
        {
            if (size == 0) size = tex.width;
            RenderTexture activeRenderTarget = RenderTexture.active;
            RenderTexture rt = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            RenderTexture.active = rt;
            mtl.SetPass(0);
            bool curMode = GL.sRGBWrite;
            GL.sRGBWrite = false;
            GL.Begin(GL.TRIANGLES);
            GL.Vertex3(-1, -1, 0); GL.Vertex3(1, -1, 0); GL.Vertex3(-1, 1, 0);
            GL.Vertex3(1, -1, 0); GL.Vertex3(1, 1, 0); GL.Vertex3(-1, 1, 0);
            GL.End();
            GL.Flush();
            tex.ReadPixels(new Rect(0, 0, size, size), 0, 0, false);
            tex.Apply(false);
            RenderTexture.active = activeRenderTarget;
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
            GL.sRGBWrite = curMode;
            GL.Flush();

        }

        private void DumpTexture(Texture2D tex, string path)
        {
            System.IO.File.WriteAllBytes(dumpPath + path, tex.EncodeToPNG());
        }

        #endregion

    }
}

#endif
