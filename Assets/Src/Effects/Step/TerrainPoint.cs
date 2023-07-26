using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPoint
{
    // Update is called once per frame
    public static Texture2D GetTexture(Terrain terrain, Vector3 WorldPos)
    {
        if (terrain == null || terrain.terrainData == null || terrain.terrainData.terrainLayers == null)
        {
            return null;
        }

        int surfaceIndex = GetMainTexture(terrain, WorldPos);
        if (surfaceIndex >= 0 && surfaceIndex < terrain.terrainData.terrainLayers.Length)
        {
            return terrain.terrainData.terrainLayers[surfaceIndex].diffuseTexture;
        }
        else
        {
            return null;
        }
    }

    /*void OnGUI()
    {
        GUI.Box(new Rect(100, 100, 200, 25), "index: " + surfaceIndex.ToString() + ", name: " + terrainData.splatPrototypes[surfaceIndex].texture.name);
    }*/

    private static float[] GetTextureMix(Terrain terrain, Vector3 WorldPos)
    {
        try
        {
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;

            // returns an array containing the relative mix of textures
            // on the main terrain at this world position.

            // The number of values in the array will equal the number
            // of textures added to the terrain.

            // calculate which splat map cell the worldPos falls within (ignoring y)
            int mapX = (int)(((WorldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int mapZ = (int)(((WorldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight); 

            // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

            // extract the 3D array data to a 1D array:
            float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

            for (int n = 0; n < cellMix.Length; n++)
            {
                cellMix[n] = splatmapData[0, 0, n];
            }
            return cellMix;
        }
        catch
        {
            Debug.LogError("TerrainPointError");
        }
        return null;
    }

    private static int GetMainTexture(Terrain terrain, Vector3 WorldPos)
    {
    // returns the zero-based index of the most dominant texture
    // on the main terrain at this world position.
        float[] mix = GetTextureMix(terrain, WorldPos);
        return GetMaxIndex(mix);
    }

    static int GetMaxIndex(float[] mix)
    {
        float maxMix = 0;
        int maxIndex = 0;

        // loop through each mix value and find the maximum
        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }
}
