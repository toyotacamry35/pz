using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AwesomeTechnologies.External.Terrafirma
{
    [ExecuteInEditMode]
    public class TerrafirmaExportTerrainData : MonoBehaviour
    {
        public Terrain terrain;
        public Texture2D outputTexture;

        private void OnEnable()
        {
            if (terrain)
            {
                float[,] heights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
                outputTexture = convertHeightField(heights, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
            }
        } 

        public void RefreshImage()
        {
            OnEnable();
        }

        public void Export()
        {
            if (terrain)
            {
                float[,] heights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
                outputTexture = convertHeightField(heights, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
                SaveTexture(outputTexture, "Assets\\" + terrain.gameObject.name);
            }
        }

        public static void SaveTexture(Texture2D tex, string name)
        {
#if UNITY_EDITOR
            var bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(name + ".png", bytes);
#endif
        }

        void RefreshAssets()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        Texture2D convertHeightField(float[,] heights, int heightmapWidth, int heightmapHeight)
        {
            Color[] pix = new Color[heightmapWidth * heightmapWidth];

            Texture2D heightMap = new Texture2D(heightmapWidth, heightmapHeight, TextureFormat.RFloat, false);
            heightMap.filterMode = FilterMode.Point;

            for (int y = 0; y < heightmapWidth; y++) for (int x = 0; x < heightmapWidth; x++)
                {
                    float h = heights[y, x];
                    Color c = new Color {r = h};

                    pix[x + y * heightmapWidth] = c;
                }

            heightMap.SetPixels(pix);
            heightMap.Apply();

            return heightMap;
        }
    }
}
