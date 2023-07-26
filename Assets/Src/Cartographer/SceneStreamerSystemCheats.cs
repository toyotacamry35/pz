using Assets.Src.Cartographer;
using Assets.Src.Lib.ProfileTools;
using Core.Cheats;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using Core.Environment.Logging.Extension;
using TOD;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Src.BuildingSystem
{
    public class SceneStreamerSystemCheat
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // Cheats ---------------------------------------------------------------------------------
        [Cheat]
        public static void StreamerDebugReportCheat()
        {
            SceneStreamerSystem.SetDebugReportCheat();
        }

        [Cheat]
        public static void StreamerLoadSceneRangeCheat(float loadSceneRange)
        {
            SceneStreamerSystem.SetLoadSceneRangeCheat(loadSceneRange);
        }

        [Cheat]
        public static void StreamerLoadAllCheat(bool enable)
        {
            SceneStreamerSystem.SetLoadAllCheat(enable);
        }

        [Cheat]
        public static void StreamerCheckPositionCheat(bool enable)
        {
            SceneStreamerSystem.SetCheckPositionCheat(enable);
        }

        [Cheat]
        public static void StreamerDebugModeCheat(bool enable)
        {
            SceneStreamerSystem.SetDebugModeCheat(enable);
        }

        [Cheat]
        public static void StreamerDebugModeVerboseCheat(bool enable)
        {
            SceneStreamerSystem.SetDebugModeVerboseCheat(enable);
        }

        [Cheat]
        public static void StreamerShowBackgroundTerrainCheat(bool enable)
        {
            SceneStreamerSystem.SetShowBackgroundTerrainCheat(enable);
        }

        [Cheat]
        public static void StreamerShowBackgroundObjectsCheat(bool enable)
        {
            SceneStreamerSystem.SetShowBackgroundObjectsCheat(enable);
        }

        [Cheat]
        public static void StreamerShowTerrainCheat(bool enable)
        {
            SceneStreamerSystem.SetShowTerrainCheat(enable);
        }

        [Cheat]
        public static void StreamerShowFoliageCheat(bool enable)
        {
            SceneStreamerSystem.SetShowFoliageCheat(enable);
        }

        [Cheat]
        public static void StreamerShowObjectsCheat(bool enable)
        {
            SceneStreamerSystem.SetShowObjectsCheat(enable);
        }

        [Cheat]
        public static void StreamerShowEffectsCheat(bool enable)
        {
            SceneStreamerSystem.SetShowEffectsCheat(enable);
        }

        private static void VisitAllObjects(GameObject gameObject, Action<GameObject> action)
        {
            if (gameObject != null)
            {
                action?.Invoke(gameObject);
                var childCount = gameObject.transform.childCount;
                for (var childIndex = 0; childIndex < childCount; ++childIndex)
                {
                    var child = gameObject.transform.GetChild(childIndex).gameObject;
                    VisitAllObjects(child, action);
                }
            }
        }

        private static string CheckKeywords(string[] keywords, Material material)
        {
            var result = new StringBuilder();
            if ((keywords != null) && (keywords.Length > 0))
            {
                foreach(var keyword in keywords)
                {
                    if (((material != null) && material.IsKeywordEnabled(keyword)) ||
                        ((material == null) && Shader.IsKeywordEnabled(keyword)))
                    {
                        if (result.Length > 0)
                        {
                            result.Append(" ");
                        }
                        result.Append(keyword);
                    }
                }
            }
            if (result.Length == 0)
            {
                result.Append("<Empty>");
            }
            return result.ToString();
        }

        [Cheat]
        public static void MaterialTest()
        {
            var emptyKeywords = new string[] { "<empty>" };
            var keywords = new string[] {
                "NORMAL_ON",
                "NORMAL_MAP_ON",
                "GPU_FRUSTUM_ON",
                "EMISSION_GRASS_ON",
                "MULTI_PIVOTS_ON",
                "WIND_ON",
                "FAR_CULL_ON",
                "UNITY_PROCEDURAL_INSTANCING_ENABLED",
                "FAR_CULL_ON_PROCEDURAL_INSTANCING",
                "FAR_CULL_ON_SIMPLE",
                "INSTANCENATOR_FOLIAGE",
                };


            var WindCompositeShader = Shader.Find("WindComposite");
            var WindBaseTex = Profile.Load<Texture>("Default wind base texture");

            Logger.IfError()?.Message($"WindCompositeShader: {(WindCompositeShader == null ? "TRUE" : "FALSE")}").Write();
            Logger.IfError()?.Message($"WindBaseTex: {(WindBaseTex == null ? "TRUE" : "FALSE")}").Write();

            var sceneCount = SceneManager.sceneCount;
            for (var sceneIndex = 0; sceneIndex < sceneCount; ++sceneIndex)
            {
                var scene = SceneManager.GetSceneAt(sceneIndex);
                if (scene.isLoaded)
                {
                    var rootGameObjects = scene.GetRootGameObjects();
                    foreach (var gameObject in rootGameObjects)
                    {
                        VisitAllObjects(gameObject, (visitedGameObject) =>
                        {
                            var meshRenderer = visitedGameObject.GetComponent<MeshRenderer>();
                            if (meshRenderer != null)
                            {
                                var scenename = visitedGameObject.scene.name;
                                var gameObjectName = visitedGameObject.name;
                                var materialName = meshRenderer.material.name ?? emptyKeywords[0];
                                var shaderName = meshRenderer.material.shader?.name ?? emptyKeywords[0];
                                var enableInstancing = meshRenderer.material.enableInstancing ? "InstancingON" : "InstancingOFF";
                                var materialKeywords = string.Join(" ", meshRenderer.material?.shaderKeywords ?? emptyKeywords);
                                var enabledMaterialKeywords = CheckKeywords(keywords, meshRenderer.material);
                                var enabledShaderKeywords = CheckKeywords(keywords, null);
                                Logger.IfError()?.Message($"=== MaterialTest: scene: {scenename}, gameObject: {gameObjectName}, material: {materialName}, instansing: {enableInstancing}, shader: {shaderName}, material keywords: {materialKeywords}, enabled material keywords: {enabledMaterialKeywords}, enabled shader keywords: {enabledShaderKeywords}").Write();
                            }
                        }
                        );
                    }
                }
            }
        }
    }
}