using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AwesomeTechnologies.Terrains.Splatmap
{
    [System.Serializable]
    public struct TextureSplatmapRule
    {
        public bool AutomaticGeneration;
        public AnimationCurve HeightRuleAnimationCurve;
        public AnimationCurve SteepnessRuleAnimationCurve;
        public bool UseNoise;
        public float NoiseScale;
        public float MinHeight;
        public float MaxHeight;
        public float MinSteepness;
        public float MaxSteepness;
        public float Weight;
    }

    public struct HeightmapSampleInfo
    {
        public int CalculatedAlphamapWidth;
        public int CalculatedAlphamapHeight;
        public int StartX;
        public int StartY;
    }


    public class BaseSplatmapGenerator
    {
        protected UnityTerrainData UnityTerrainData;
        protected List<TextureSplatmapRule> SplatmapRuleList = new List<TextureSplatmapRule>();
        public float WaterLevel = 0;
        public float MaxCurveHeight = 500;

        public BaseSplatmapGenerator(UnityTerrainData unityTerrainData)
        {
            this.UnityTerrainData = unityTerrainData;
        }

        public BaseSplatmapGenerator()
        {
            
        }

        public void AddSplatmapRules(List<TextureSplatmapRule> splatmapRuleList)
        {
            SplatmapRuleList.Clear();
            splatmapRuleList.AddRange(splatmapRuleList);
        }

        public virtual void GenerateSplatmap(Bounds bounds, bool useBounds, bool clearAllLayers = false)
        {
            
        }

        public HeightmapSampleInfo CalculateHeightmapSampleInfo(Bounds bounds, bool useBounds)
        {
            HeightmapSampleInfo sampleInfo = new HeightmapSampleInfo();

            int calculatedAlphamapWidth = UnityTerrainData.alphamapWidth;
            int calculatedAlphamapHeight = UnityTerrainData.alphamapHeight;
            int startX = 0;
            int startY = 0;           

            if (useBounds)
            {
                int newWidth = Mathf.CeilToInt(bounds.size.x / UnityTerrainData.alphamapCellWidth);
                int newHeight = Mathf.CeilToInt(bounds.size.z / UnityTerrainData.alphamapCellHeight);
                float boundsStartX = bounds.center.x - bounds.extents.x;
                float boundsStartY = bounds.center.z - bounds.extents.z;
                float positionStartX = boundsStartX - UnityTerrainData.terrainPosition.x;
                float positionStartY = boundsStartY - UnityTerrainData.terrainPosition.z;
                int newStartX = Mathf.RoundToInt(positionStartX / UnityTerrainData.alphamapCellWidth);
                int newStartY = Mathf.RoundToInt(positionStartY / UnityTerrainData.alphamapCellHeight);

                if (newStartX < 0) newStartX = 0;
                if (newStartY < 0) newStartY = 0;
                if (newStartX + newWidth > UnityTerrainData.alphamapWidth) newWidth = UnityTerrainData.alphamapWidth - newStartX;
                if (newStartY + newHeight > UnityTerrainData.alphamapHeight) newHeight = UnityTerrainData.alphamapHeight - newStartY;

                calculatedAlphamapWidth = newHeight;
                calculatedAlphamapHeight = newWidth;
                startX = newStartX;
                startY = newStartY;
            }

            sampleInfo.CalculatedAlphamapWidth = calculatedAlphamapWidth;
            sampleInfo.CalculatedAlphamapHeight = calculatedAlphamapHeight;
            sampleInfo.StartX = startX;
            sampleInfo.StartY = startY;
            return sampleInfo;
        }

        public int GetFirstEnabledLayer()
        {            
            for (int i = 0; i <= SplatmapRuleList.Count - 1; i++)
            {
                if (SplatmapRuleList[i].AutomaticGeneration)
                {
                    return i;
                }
            }

            return 0;
        }

        protected static float SampleCurveArray(float[] curveArray, float value)
        {
            if (curveArray.Length == 0) return 0f;

            int index = Mathf.RoundToInt(value * curveArray.Length);
            index = Mathf.Clamp(index, 0, curveArray.Length - 1);
            return curveArray[index];
        }
    }
}
