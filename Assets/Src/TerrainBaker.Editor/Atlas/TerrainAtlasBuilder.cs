using Assets.Src.ResourceSystem.JdbRefs;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.TerrainBaker.Editor
{

    public class TerrainAtlasBuilder : ScriptableObject
    {
        [Serializable]
        public class TerrainLayer
        {
            //base textures
            public Texture2D albedo;
            public Texture2D normals;
            public Texture2D parameters;
            public Texture2D emission;
            public Texture2D macro;
            //layer uv params
            public float sizeInMeters = 10.0f;
            public float offsetX = 0.0f;
            public float offsetZ = 0.0f;
            public float macroTilling = 0.123f;
            public float macroScale = 1.0f;
            //additional parameters
            public float albedoHeightMapOffset = 0.0f;
            public float albedoHeightMapScale = 1.0f;
            public float reliefFactor = 0.7f;
            public float mipBias = 0.0f;
            public Color emissionTint = Color.white;
            public TerrainLayerFilling layerFilling;
            public FXStepMarkerDefRef layerFX;
        }

        public TerrainAtlas terrainAtlas;

        public string namePrefixForTextureArrays = "TerrainTexturesAtlas";
        public int baseTextureSize = 2048;        
        public int parametersSize = 1024;
        public int macroSize = 1024;

        public List<TerrainLayer> layersList;
        public string terrainLayersFolder = string.Empty;

        public const string albedoPostfix = "_dm";
        public const string normalsPostfix = "_nm";
        public const string parametersPostfix = "_am";
        public const string macroPostfix = "_mm";
        public const string emissionPostfix = "_em";
    }
}
