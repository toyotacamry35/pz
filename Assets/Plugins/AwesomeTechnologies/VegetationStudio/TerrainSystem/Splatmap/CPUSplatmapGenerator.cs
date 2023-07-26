using System;
using System.Collections.Generic;
using AwesomeTechnologies.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AwesomeTechnologies.Terrains.Splatmap
{
    public class CPUSplatmapGenerator : BaseSplatmapGenerator
    {
        public CPUSplatmapGenerator(UnityTerrainData unityTerrainData)
        {
            UnityTerrainData = unityTerrainData;
        }

        public override void GenerateSplatmap(Bounds bounds, bool useBounds, bool clearAllLayers = false)
        {
            List<float[]> sampledHeightCurveList =  new List<float[]>();
            List<float[]> sampledSteepnessCurveList = new List<float[]>();
            for (int i = 0; i <= SplatmapRuleList.Count - 1; i++)
            {
                sampledHeightCurveList.Add(SplatmapRuleList[i].HeightRuleAnimationCurve.GenerateCurveArray());
                sampledSteepnessCurveList.Add(SplatmapRuleList[i].SteepnessRuleAnimationCurve.GenerateCurveArray());
            }

            HeightmapSampleInfo heightmapSampleInfo = CalculateHeightmapSampleInfo(bounds, useBounds);
            int firstEnabledLayer = GetFirstEnabledLayer();

            float[,,] splatmapData;
            if (clearAllLayers)
            {
                splatmapData = new float[heightmapSampleInfo.CalculatedAlphamapWidth, heightmapSampleInfo.CalculatedAlphamapHeight, UnityTerrainData.alphamapLayers];
            }
            else
            {
                splatmapData = UnityTerrainData.terrain.terrainData.GetAlphamaps(heightmapSampleInfo.StartX, heightmapSampleInfo.StartY,
                    heightmapSampleInfo.CalculatedAlphamapHeight, heightmapSampleInfo.CalculatedAlphamapWidth);
            }

            float[] perlinNoise = new float[UnityTerrainData.alphamapLayers];
            float[] splatWeights = new float[UnityTerrainData.alphamapLayers];

            int alphamapLayers = UnityTerrainData.alphamapLayers;
            int alphamapHeight = UnityTerrainData.alphamapHeight;
            int alphamapWidth = UnityTerrainData.alphamapWidth;

            for (int y = 0; y < heightmapSampleInfo.CalculatedAlphamapHeight; y++)
            {

#if UNITY_EDITOR
                if (useBounds == false)
                {
                    if (y % 100 == 0)
                    {
                        float progress = (float)y / heightmapSampleInfo.CalculatedAlphamapHeight;
                        EditorUtility.DisplayProgressBar("Texture terrain", "Re-texturing entire terrain based on current rules", progress);
                    }
                }
#endif             
                for (int x = 0; x < heightmapSampleInfo.CalculatedAlphamapWidth; x++)
                {
                    // Normalise x/y coordinates to range 0-1 
                    float y01 = (float)(y + heightmapSampleInfo.StartX) / alphamapHeight;
                    float x01 = (float)(x + heightmapSampleInfo.StartY) / alphamapWidth;

                    //Calculate total of disabled layers
                    float disabledTotal = 0;
                    for (int i = 0; i < alphamapLayers; i++)
                    {
                        if (!SplatmapRuleList[i].AutomaticGeneration)
                        {
                            disabledTotal += splatmapData[x, y, i];
                        }
                    }

                    float height = UnityTerrainData.new_GetHeight(Mathf.RoundToInt(y01 * UnityTerrainData.heightmapHeight), Mathf.RoundToInt(x01 * UnityTerrainData.heightmapWidth));

                    float steepness = UnityTerrainData.terrain.terrainData.GetSteepness(y01, x01);

                    float normalizedSteepness = steepness / 90;
                    float normalizedHeight = (height - WaterLevel) / MaxCurveHeight;

                    float z = 0;
                    for (int i = 0; i < alphamapLayers; i++)
                    {
                        perlinNoise[i] = 1;
                        if (SplatmapRuleList[i].UseNoise) perlinNoise[i] = Mathf.PerlinNoise(y01 / SplatmapRuleList[i].NoiseScale, x01 / SplatmapRuleList[i].NoiseScale);
                        if (SplatmapRuleList[i].AutomaticGeneration)
                        {
                            float steepnessSample =
                                SampleCurveArray(sampledSteepnessCurveList[i], normalizedSteepness);
                            float heightSample =
                                SampleCurveArray(sampledHeightCurveList[i], normalizedHeight);

                            splatWeights[i] = Mathf.Clamp01(steepnessSample) * Mathf.Clamp01(heightSample) * perlinNoise[i] * SplatmapRuleList[i].Weight;
                        }
                        else
                        {
                            splatWeights[i] = 0;
                        }

                        z += splatWeights[i];
                    }

                    if (Math.Abs(z) < 0.01f && Math.Abs(disabledTotal) < 0.01f) splatWeights[firstEnabledLayer] = 1;

                    z = 0;
                    for (int i = 0; i < alphamapLayers; i++)
                    {
                        z += splatWeights[i];
                    }

                    for (int i = 0; i < alphamapLayers; i++)
                    {
                        // Normalize so that sum of all texture weights = 1
                        splatWeights[i] /= (z);
                        splatWeights[i] *= (1 - disabledTotal);

                        if(SplatmapRuleList[i].AutomaticGeneration)
                        {
                            // Assign this point to the splatmap array
                            splatmapData[x, y, i] = splatWeights[i];
                        }

                    }
                }
            }
            UnityTerrainData.terrain.terrainData.SetAlphamaps(heightmapSampleInfo.StartX, heightmapSampleInfo.StartY, splatmapData);

#if UNITY_EDITOR
            if (useBounds == false)
            {
                EditorUtility.ClearProgressBar();
            }
#endif
        }
    }
}
