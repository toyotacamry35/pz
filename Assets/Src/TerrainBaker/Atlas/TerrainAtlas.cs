using Assets.Src.ResourceSystem.JdbRefs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.TerrainBaker
{
    public class TerrainAtlas : ScriptableObject
    {
        #region Serialized part
        [Serializable]
        public class Layer
        {
            public string albedoName;
            public float sizeInMeters;
            public float offsetX;
            public float offsetZ;
            public float albedoIndex;
            public float normalsIndex;
            public float parametersIndex;
            public float emissionIndex;
            public float macroIndex;
            public float macroTilling;
            public float macroScale;
            public float albedoHeightMapOffset;
            public float albedoHeightMapScale;
            public float reliefFactor;
            public float mipBias;
            public Color emissionTint;
            public FXStepMarkerDefRef layerFX;
#if UNITY_EDITOR
            public TerrainLayerFilling layerFilling;//It is not good, but no more ideas
#endif
        }

        public Layer[] layers;
        public Texture2DArray albedo;
        public Texture2DArray normals;
        public Texture2DArray parameters;
        public Texture2DArray emission;
        public Texture2DArray macro;

        #endregion



        #region Runtime part


        private struct ShaderLayer
        {
            public const int structSize = 5 * (4 * sizeof(float));//must be align by 16 byte (4 float)

            public Vector2 textureOffset;
            public float textureTilling;
            public float textureMipBias;
            public float albedoHeightMapOffset;
            public float albedoHeightMapScale;
            public float reliefFactor;
            public float albedoIndex;
            public float normalsIndex;
            public float parametersIndex;
            public float emissionIndex;
            public float macroIndex;
            public float macroTilling;
            public float macroMipBias;
            public float macroScale;
            public Vector3 emissionTint;
            public float reserved0;
            public float reserved1;
        }

        [NonSerialized]
        private ShaderLayer[] shaderLayers;
        [NonSerialized]
        private ComputeBuffer layersParams;
        [NonSerialized]
        private Vector4 texturesMipInfo;
        [NonSerialized]
        private Texture2DArray emptyArray;

        private static int shaderLayersBuffer;
        private static int shaderSplatsAlbedo;
        private static int shaderSplatsNormal;
        private static int shaderSplatsParameters;
        private static int shaderSplatsEmission;
        private static int shaderSplatsMacro;
        private static int shaderTexturesMipInfo;

        private static bool isInitShaderIDs = false;

        public void Set(Material material)
        {
            if (material == null)
            {
                return;
            }
            UpdateData(false);

            InitShaderIDs();
            material.SetBuffer(shaderLayersBuffer, layersParams);
            material.SetTexture(shaderSplatsAlbedo, albedo);
            material.SetTexture(shaderSplatsNormal, normals);
            material.SetTexture(shaderSplatsParameters, parameters);
            //material.SetTexture(shaderSplatsEmission, emission != null ? emission : emptyArray);
            //material.SetTexture(shaderSplatsMacro, macro != null ? macro : emptyArray);
            material.SetVector(shaderTexturesMipInfo, texturesMipInfo);
        }

        public void SetPropertyBlock(MaterialPropertyBlock propertyBlock)
        {
            if (propertyBlock == null)
            {
                return;
            }

            UpdateData(false);

            InitShaderIDs();
            propertyBlock.SetBuffer(shaderLayersBuffer, layersParams);
            propertyBlock.SetTexture(shaderSplatsAlbedo, albedo);
            propertyBlock.SetTexture(shaderSplatsNormal, normals);
            propertyBlock.SetTexture(shaderSplatsParameters, parameters);
            //propertyBlock.SetTexture(shaderSplatsEmission, emission != null ? emission : emptyArray);
            //propertyBlock.SetTexture(shaderSplatsMacro, macro != null ? macro : emptyArray);
            propertyBlock.SetVector(shaderTexturesMipInfo, texturesMipInfo);
        }

        public void Update()
        {
            UpdateData(true);
        }

        private static void InitShaderIDs()
        {
            if (!isInitShaderIDs)
            {
                shaderLayersBuffer = Shader.PropertyToID("TerrainLayersBuffer");
                shaderSplatsAlbedo = Shader.PropertyToID("TerrainSplatsAlbedo");
                shaderSplatsNormal = Shader.PropertyToID("TerrainSplatsNormal");
                shaderSplatsParameters = Shader.PropertyToID("TerrainSplatsParameters");
                shaderSplatsEmission = Shader.PropertyToID("TerrainSplatsEmission");
                shaderSplatsMacro = Shader.PropertyToID("TerrainSplatsMacro");
                shaderTexturesMipInfo = Shader.PropertyToID("TerrainTexturesMipInfo");

                isInitShaderIDs = true;
            }
        }

        private void UpdateData(bool forceUpdateData)
        {
            bool needUpdateData = forceUpdateData;
            if (shaderLayers == null || shaderLayers.Length != layers.Length)
            {
                shaderLayers = new ShaderLayer[layers.Length];
                needUpdateData = true;
            }

            if (layersParams == null)
            {
                layersParams = new ComputeBuffer(layers.Length, ShaderLayer.structSize);
                needUpdateData = true;
            }

            if (needUpdateData)
            {
                const float sizeEps = 0.001f;
                for (int i = 0; i < shaderLayers.Length; i++)
                {
                    ShaderLayer shaderLayer = new ShaderLayer();
                    TerrainAtlas.Layer layer = layers[i];
                    if (layer.sizeInMeters > sizeEps)
                    {
                        shaderLayer.textureTilling = 1.0f / layer.sizeInMeters;
                        shaderLayer.textureOffset = new Vector2(layer.offsetX / layer.sizeInMeters, layer.offsetZ / layer.sizeInMeters);
                        shaderLayer.textureMipBias = Mathf.Log(shaderLayer.textureTilling, 2.0f) + layer.mipBias;
                        shaderLayer.macroTilling = Mathf.Max(layer.macroTilling, sizeEps);
                        shaderLayer.macroMipBias = shaderLayer.textureMipBias + Mathf.Log(Mathf.Max(shaderLayer.macroTilling, sizeEps), 2.0f);
                    }
                    else
                    {
                        shaderLayer.textureTilling = 0;
                        shaderLayer.textureOffset = Vector2.zero;
                        shaderLayer.textureMipBias = 0;
                        shaderLayer.macroTilling = 0;
                        shaderLayer.macroMipBias = 0;
                    }


                    shaderLayer.albedoIndex = layer.albedoIndex;
                    shaderLayer.normalsIndex = layer.normalsIndex;
                    shaderLayer.parametersIndex = layer.parametersIndex;
                    shaderLayer.emissionIndex = layer.emissionIndex;
                    shaderLayer.macroIndex = layer.macroIndex;
                    shaderLayer.macroScale = layer.macroScale;
                    shaderLayer.albedoHeightMapOffset = layer.albedoHeightMapOffset;
                    shaderLayer.albedoHeightMapScale = layer.albedoHeightMapScale;
                    shaderLayer.reliefFactor = layer.reliefFactor;
                    shaderLayer.emissionTint = new Vector3(layer.emissionTint.r, layer.emissionTint.g, layer.emissionTint.b);
                    shaderLayers[i] = shaderLayer;
                }

                texturesMipInfo = new Vector4(Mathf.Log(albedo.width, 2.0f),
                                                Mathf.Log(parameters.width, 2.0f),
                                                (macro != null) ? Mathf.Log(macro.width, 2.0f) : 0.0f,
                                                0.0f);

                layersParams.SetData(shaderLayers);

                if ((emission == null || macro == null) && emptyArray == null)
                {
                    emptyArray = new Texture2DArray(4, 4, 2, TextureFormat.ARGB32, false);
                    Color[] empty = new Color[16];
                    emptyArray.SetPixels(empty, 0);
                    emptyArray.SetPixels(empty, 1);
                    emptyArray.Apply();
                }
            }
        }

        public void OnDisable()
        {
            if (layersParams != null)
            {
                layersParams.Release();
                layersParams = null;
            }
            emptyArray = null;
        }


        #endregion

    }
}
