#if UNITY_STANDALONE_WIN && !UNITY_EDITOR && !VS_NO_HEIGHTMAP_DLL
#define VS_USE_HEIGHTMAP_DLL
#endif


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
 

#if VS_USE_HEIGHTMAP_DLL
public class HeightMapUtilDLL
{
    [DllImport("HeightmapUtil", EntryPoint = "HSCreate")]
	public static extern System.IntPtr HSCreate(float[] heights, int width, int height, float[] scale);

	[DllImport("HeightmapUtil", EntryPoint = "HSDestroy")]
	public static extern int HSDestroy(System.IntPtr h);

	[DllImport("HeightmapUtil", EntryPoint = "HSSampleHeight")]
	public static extern float HSSampleHeight(System.IntPtr h, float x, float y);

	[DllImport("HeightmapUtil", EntryPoint = "HSSampleNormal")]
	public static extern void HSSampleNormal(System.IntPtr h, float x, float y, float[] result);

    [DllImport("HeightmapUtil", EntryPoint = "HSSampleNormalNonAlloc")]
	public static extern void HSSampleNormalNonAlloc(System.IntPtr h, float x, float y, ref float outx, ref float outy, ref float outz);

    //Keep as private variables. Shared memory with Native Plugin. 
	private float[] _heights;
    private float[] _scale;

	System.IntPtr heightMap;

    public HeightMapUtilDLL(TerrainData terrainData)
	{
		int w = terrainData.heightmapResolution;
		int h = terrainData.heightmapResolution;

		_heights = new float[w * h];

		float[,] hs = terrainData.GetHeights(0, 0, w, h);

		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				_heights[y * w + x] = hs[y, x]; // this is correct
			}
		}

		var s = terrainData.heightmapScale;
		_scale = new float[] { s.x, s.y, s.z };

		heightMap = HSCreate(_heights, w, h, _scale); // create CPP heightMap, dont destroy it and "heights" array till thread complete
	}

    public void SetHeights(float[,] heights)
    {
        int width = heights.GetLength(0);
        int height = heights.GetLength(1);

        if (width * height == _heights.Length)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _heights[y * width + x] = heights[y, x]; // this is correct
                }
            }
        }
    }
        

	public float GetInterpolatedHeight(float x, float y)
	{
		return HSSampleHeight(heightMap, x, y);
	}

	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		//float[] n = new float[3]; 

        float outx = 0;
        float outy = 0;
        float outz = 0;

		//HSSampleNormal(heightMap, x, y, heightMapSampleArray); 
        HSSampleNormalNonAlloc(heightMap, x, y, ref outx,ref outy,ref outz); 

        //return new Vector3(heightMapSampleArray[0], heightMapSampleArray[1], heightMapSampleArray[2]); 
		return new Vector3(outx, outy, outz); 
	}

	public void Destroy()
	{
		HSDestroy(heightMap);
	}
}
#endif
