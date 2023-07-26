using Assets.Src.Aspects.RegionsScenicObjects;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Entities.Service;
using Assets.Src.Aspects;
using Assets.Src.Lib.Extensions;
using UnityEditor.Build;
using UnityEngine.SceneManagement;
using NLog;
using NLog.Fluent;
using GeneratedCode.DeltaObjects;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;
using UnityEditor.Build.Reporting;
using Assets.Src.Tools;
using SharedCode.Utils;
using UnityEditor;
using static ShapeEditor;
using UnityEditor.SceneManagement;
using Assets.Test.Src.Editor;
using SharedCode.MapSystem;

namespace Assets.Src.SpawnSystem
{
    class SceneToSpawnersPostprocessor : IProcessSceneWithReport
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public int callbackOrder => 1;

        [MenuItem("Tools/DEBUG____SceneToSpawnerPostprocessor")]
        public static void DebugSceneToSpawner()
        {
            new PostProcessProduceShapeDefs().OnProcessScene(SceneManager.GetActiveScene(), null);
            new SceneToSpawnersPostprocessor().OnProcessScene(SceneManager.GetActiveScene(), null);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            //var debugTimer = new System.Diagnostics.Stopwatch();
            //debugTimer.Start();

            SpawnTemplate[] templates = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SpawnTemplate>()).Distinct().ToArray();
            SpawnRegion[] regions = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<SpawnRegion>()).Distinct().ToArray();

            if (VisualKiller.Enabled)
            {
                foreach (var spawnRegion in regions)
                    UnityEngine.Object.DestroyImmediate(spawnRegion);
                foreach (var spawnTemplate in templates)
                    UnityEngine.Object.DestroyImmediate(spawnTemplate.gameObject);
            }
            else
            {

                foreach (var spawnRegion in regions)
                    spawnRegion.gameObject.SetActive(false);
                foreach (var spawnTemplate in templates)
                    spawnTemplate.gameObject.SetActive(false);
            }

            //var seconds = (debugTimer?.ElapsedMilliseconds ?? 0) / 1000.0f;
            //Debug.LogError($"SceneToSpawnersPostprocessor.ScenePostProcess, time: {seconds} sec, scene: {scene.name}");
        }

   
    }

}

