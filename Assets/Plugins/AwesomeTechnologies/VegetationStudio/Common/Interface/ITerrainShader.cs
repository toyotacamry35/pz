using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AwesomeTechnologies.Common.Interfaces
{

    [System.Serializable]
    public class TerrainTextureInfo
    {
        public Texture2D Texture;
        public string TextureGUID = "";
        public Texture2D TextureNormals;
        public string TextureNormalsGUID = "";
        public Texture2D TextureOcclusion;
        public string TextureOcclusionGUID = "";
        public Texture2D TextureHeightMap;
        public string TextureHeightMapGUID = "";
        public UnityEngine.Vector2 TileSize = new UnityEngine.Vector2(15, 15);
        public UnityEngine.Vector2 offset;
    }

    public interface ITerrainShader
    {
        void onTerrainTextureChange(TerrainTextureInfo[] terrainTextureInfo);
    }
}

