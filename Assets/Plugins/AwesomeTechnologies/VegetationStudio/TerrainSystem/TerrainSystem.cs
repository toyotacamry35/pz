
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.Common;
using AwesomeTechnologies.Terrains.Splatmap;
using AwesomeTechnologies.VegetationStudio;
using UnityEngine.SceneManagement;

namespace AwesomeTechnologies
{
    [HelpURL("http://www.awesometech.no/index.php/home/vegetation-studio/components/terrain-system")]
    [AwesomeTechnologiesScriptOrder(101)]
    [ExecuteInEditMode]
    public class TerrainSystem : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnValidate()
        {
            TerrainHeatMapMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/VegetationSystem/_Resources/TerrainHeatMap.mat");
        }
#endif

        public bool AutomaticApply = false;

        public float TerrainSystemWaterSourceType;
        public Transform WaterTransform;
      
        public VegetationSystem VegetationSystem;
        public Material TerrainHeatMapMaterial;

        public delegate void MultiOnTerrainRefreshProgressDelegate(float progress);
        public MultiOnTerrainRefreshProgressDelegate OnTerrainRefreshProgress;

        // ReSharper disable once UnusedMember.Local
        private void OnEnable()
        {
#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
            EditorSceneManager.sceneSaving += OnSceneSaving;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
#endif
#endif


            VegetationSystem = gameObject.GetComponent<VegetationSystem>();
            if (VegetationSystem)
            {
                VegetationSystem.OnTerrainSettingsChangeDelegate += OnTerrainSettingsChanged;
            }
            RegisterTerrainSystem();
        }

#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            RestoreTerrainMaterial();
        }
#else
        private void OnPlayModeStateChanged()
        {
            RestoreTerrainMaterial();
        }
#endif

        private void OnSceneSaving(Scene scene, string path)
        {
            RestoreTerrainMaterial();
        }

        public Terrain GetTerrain()
        {
            return VegetationSystem.currentTerrain;
        }

        public VegetationPackage GetVegetationPackage()
        {
            return VegetationSystem.currentVegetationPackage;
        }

        private void OnTerrainSettingsChanged()
        {
        }

        private void RegisterTerrainSystem()
        {
                VegetationStudioManager.RegisterTerrainSystem(this);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDisable()
        {
#if UNITY_EDITOR && UNITY_5_6_OR_NEWER
            EditorSceneManager.sceneSaving -= OnSceneSaving;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged -= OnPlayModeStateChanged;
#endif
#endif

            if (VegetationSystem)
                VegetationSystem.OnTerrainSettingsChangeDelegate -= OnTerrainSettingsChanged;

            VegetationStudioManager.UnregisterTerrainSystem(this);
        }


        private void New_GenerateTerrainSplatMap(Bounds bounds, bool useBounds, bool clearAllLayers = false)
        {
            BaseSplatmapGenerator splatmapGenerator = new CPUSplatmapGenerator(VegetationSystem.UnityTerrainData);
            splatmapGenerator.WaterLevel = VegetationSystem.vegetationSettings.WaterLevel;
            splatmapGenerator.MaxCurveHeight = GetMaxCurveHeight();
            VegetationPackage vegetationPackage = GetVegetationPackage();
            List<TextureSplatmapRule> textureSplatmapRuleList = new List<TextureSplatmapRule>();
            for (int i = 0; i < VegetationSystem.UnityTerrainData.alphamapLayers; i++)
            {                
                TextureSplatmapRule layerRule = new TextureSplatmapRule
                {
                    AutomaticGeneration = vegetationPackage.TerrainTextureSettingsList[i].Enabled,
                    UseNoise = vegetationPackage.TerrainTextureSettingsList[i].TextureUseNoise,
                    NoiseScale = vegetationPackage.TerrainTextureSettingsList[i].TextureNoiseScale,
                    HeightRuleAnimationCurve = vegetationPackage.TerrainTextureSettingsList[i].TextureHeightCurve,
                    SteepnessRuleAnimationCurve = vegetationPackage.TerrainTextureSettingsList[i].TextureAngleCurve,
                    Weight = vegetationPackage.TerrainTextureSettingsList[i].TextureWeight,
                    MinHeight = 0,
                    MaxHeight = splatmapGenerator.MaxCurveHeight,
                    MinSteepness = 0,
                    MaxSteepness = 90
                };
                textureSplatmapRuleList.Add(layerRule);
            }

            splatmapGenerator.AddSplatmapRules(textureSplatmapRuleList);
            splatmapGenerator.GenerateSplatmap(bounds, useBounds, clearAllLayers);
        }

        public void GenerateTerrainSplatMap(Bounds bounds, bool useBounds, bool clearAllLayers = false)
        {
            if (VegetationSystem.GetSleepMode()) return;
            if (!VegetationSystem.InitDone) return;
            if (VegetationSystem.UnityTerrainData == null) return;

            VegetationPackage vegetationPackage = GetVegetationPackage();
            if (vegetationPackage.TerrainTextureCount == 0) return;
           
                       
            float waterLevel = VegetationSystem.vegetationSettings.WaterLevel;
            int calculatedAlphamapWidth = VegetationSystem.UnityTerrainData.alphamapWidth;
            int calculatedAlphamapHeight = VegetationSystem.UnityTerrainData.alphamapHeight;
            int startX = 0;
            int startY = 0;
            float maxCurveHeight = GetMaxCurveHeight();

            if (useBounds)
            {
                int newWidth = Mathf.CeilToInt(bounds.size.x / VegetationSystem.UnityTerrainData.alphamapCellWidth);
                int newHeight = Mathf.CeilToInt(bounds.size.z / VegetationSystem.UnityTerrainData.alphamapCellHeight);
                float boundsStartX = bounds.center.x - bounds.extents.x;
                float boundsStartY = bounds.center.z - bounds.extents.z;
                float positionStartX = boundsStartX - VegetationSystem.UnityTerrainData.terrainPosition.x;
                float positionStartY = boundsStartY - VegetationSystem.UnityTerrainData.terrainPosition.z;
                int newStartX = Mathf.RoundToInt(positionStartX / VegetationSystem.UnityTerrainData.alphamapCellWidth);
                int newStartY = Mathf.RoundToInt(positionStartY / VegetationSystem.UnityTerrainData.alphamapCellHeight);

                if (newStartX < 0) newStartX = 0;
                if (newStartY < 0) newStartY = 0;
                if (newStartX + newWidth > VegetationSystem.UnityTerrainData.alphamapWidth) newWidth = VegetationSystem.UnityTerrainData.alphamapWidth - newStartX;
                if (newStartY + newHeight > VegetationSystem.UnityTerrainData.alphamapHeight) newHeight = VegetationSystem.UnityTerrainData.alphamapHeight - newStartY;

                calculatedAlphamapWidth = newHeight;
                calculatedAlphamapHeight = newWidth;
                startX = newStartX;
                startY = newStartY;
            }

            int firstEnabledLayer = 0;
            for (int i = 0; i < VegetationSystem.UnityTerrainData.alphamapLayers; i++)
            {
                if (vegetationPackage.TerrainTextureSettingsList[i].Enabled)
                {
                    firstEnabledLayer = i;
                    break;
                }
            }

            float[,,] splatmapData;

            if (clearAllLayers)
            {
                splatmapData = new float[calculatedAlphamapWidth, calculatedAlphamapHeight, VegetationSystem.UnityTerrainData.alphamapLayers];

            }
            else
            {
                splatmapData = VegetationSystem.currentTerrain.terrainData.GetAlphamaps(startX, startY,
                    calculatedAlphamapHeight, calculatedAlphamapWidth);
            }


            float[] perlinNoise = new float[VegetationSystem.UnityTerrainData.alphamapLayers];
            float[] splatWeights = new float[VegetationSystem.UnityTerrainData.alphamapLayers];

            int alphamapLayers = VegetationSystem.UnityTerrainData.alphamapLayers;
            int alphamapHeight = VegetationSystem.UnityTerrainData.alphamapHeight;
            int alphamapWidth = VegetationSystem.UnityTerrainData.alphamapWidth;

            for (int y = 0; y < calculatedAlphamapHeight; y++)
            {

#if UNITY_EDITOR
                if (useBounds == false)
                {
                    if (y % 100 == 0)
                    {
                        float progress = (float)y /calculatedAlphamapHeight;
                        EditorUtility.DisplayProgressBar("Texture terrain", "Re-texturing entire terrain based on current rules", progress);
                    }
                }
#endif
                if (useBounds == false)
                {
                    if (y % 100 == 0)
                    {
                        if (OnTerrainRefreshProgress != null)
                        {
                            float progress = (float)y /calculatedAlphamapHeight;
                            OnTerrainRefreshProgress(progress);
                        }
                    }
                }

                for (int x = 0; x < calculatedAlphamapWidth; x++)
                {
                    // Normalise x/y coordinates to range 0-1 
                    float y01 = (float)(y + startX) / alphamapHeight;
                    float x01 = (float)(x + startY) / alphamapWidth;


                    //Calculate total of disabled layers
                    float disabledTotal = 0;
                    for (int i = 0; i < alphamapLayers; i++)
                    {
                        if (!vegetationPackage.TerrainTextureSettingsList[i].Enabled)
                        {
                            disabledTotal += splatmapData[x, y, i];
                        }
                    }


                    float height = VegetationSystem.UnityTerrainData.new_GetHeight(Mathf.RoundToInt(y01 * VegetationSystem.UnityTerrainData.heightmapHeight), Mathf.RoundToInt(x01 * VegetationSystem.UnityTerrainData.heightmapWidth));
                     
                    float steepness = VegetationSystem.currentTerrain.terrainData.GetSteepness(y01, x01);
                    
                    float normalizedSteepness = steepness / 90;
                    float normalizedHeight = (height - waterLevel) / maxCurveHeight;
                    
                    float z = 0;
                    for (int i = 0; i < alphamapLayers; i++)
                    {                      
                        perlinNoise[i] = 1;
                        if (vegetationPackage.TerrainTextureSettingsList[i].TextureUseNoise) perlinNoise[i] = Mathf.PerlinNoise(y01 / vegetationPackage.TerrainTextureSettingsList[i].TextureNoiseScale, x01 / vegetationPackage.TerrainTextureSettingsList[i].TextureNoiseScale);
                        if (vegetationPackage.TerrainTextureSettingsList[i].Enabled)
                        {
                            splatWeights[i] = Mathf.Clamp01(vegetationPackage.TerrainTextureSettingsList[i].TextureAngleCurve.Evaluate(normalizedSteepness)) * Mathf.Clamp01(vegetationPackage.TerrainTextureSettingsList[i].TextureHeightCurve.Evaluate(normalizedHeight)) * perlinNoise[i] * vegetationPackage.TerrainTextureSettingsList[i].TextureWeight;
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


                        if (vegetationPackage.TerrainTextureSettingsList[i].Enabled)
                        {
                            // Assign this point to the splatmap array
                            splatmapData[x, y, i] = splatWeights[i];
                        }
                         
                    }
                }
            }

            VegetationSystem.currentTerrain.terrainData.SetAlphamaps(startX, startY, splatmapData);
           
#if UNITY_EDITOR
            if (useBounds == false)
            {
                EditorUtility.ClearProgressBar();
            }
#endif

            if (OnTerrainRefreshProgress != null)
            {
                OnTerrainRefreshProgress(1);
            }

            if (useBounds == false)
            {
                if (VegetationSystem)
                {
                    VegetationSystem.RefreshHeightMap(VegetationSystem.UnityTerrainData.TerrainBounds, false, false);
                }
            }
        }

        public Texture2D GetTerrainTexture(int index)
        {
            
            if (VegetationSystem.currentTerrain)
            {
                if (index < VegetationSystem.currentTerrain.terrainData.splatPrototypes.Length)
                {
                        return VegetationSystem.currentTerrain.terrainData.splatPrototypes[index].texture;                
                }
            }           
            return new Texture2D(80,80);
        }

        private float GetMaxCurveHeight()
        {
            var maxCurveHeight = GetVegetationPackage().AutomaticMaxCurveHeight ? VegetationSystem.UnityTerrainData.MaxTerrainHeight : GetVegetationPackage().MaxCurveHeight;

            if (maxCurveHeight < 1) maxCurveHeight = 1;
            return maxCurveHeight;
        }

        public void UpdateHeatmapMaterial(int textureIndex)
        {
            if (!VegetationSystem) return;

            TerrainHeatMapMaterial.SetFloat("_TerrainMinHeight", VegetationSystem.GetWaterLevel());
            TerrainHeatMapMaterial.SetFloat("_TerrainMaxHeight", GetMaxCurveHeight());
            TerrainHeatMapMaterial.SetFloat("_MinHeight", 0);
            TerrainHeatMapMaterial.SetFloat("_MaxHeight", 0);
            TerrainHeatMapMaterial.SetFloat("_MinSteepness", 0);
            TerrainHeatMapMaterial.SetFloat("_MaxSteepness", 90);
            TerrainHeatMapMaterial.SetFloat("_TerrainYPosition", VegetationSystem.currentTerrain.transform.position.y);
            TerrainHeatMapMaterial.SetTexture("_CurveTexture", new Texture2D(1, 1));

            if (GetVegetationPackage().TerrainTextureSettingsList.Count <= textureIndex) return;

            TerrainHeatMapMaterial.SetFloatArray("_HeightCurve", GetVegetationPackage().TerrainTextureSettingsList[textureIndex].TextureHeightCurve.GenerateCurveArray());
            TerrainHeatMapMaterial.SetFloatArray("_SteepnessCurve", GetVegetationPackage().TerrainTextureSettingsList[textureIndex].TextureAngleCurve.GenerateCurveArray());
        }

        public void ShowHeatmap(int textureIndex)
        {
            UpdateHeatmapMaterial(textureIndex);

            if (VegetationSystem)
            {
                VegetationSystem.OverrideTerrainMaterial(TerrainHeatMapMaterial);
            }
        }

        public void RestoreTerrainMaterial()
        {
            if (VegetationSystem)
            {
                VegetationSystem.RestoreTerrainMaterial();
            }
        }
    }
}

