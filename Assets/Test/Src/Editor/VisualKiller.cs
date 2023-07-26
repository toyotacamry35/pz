using AdvancedTerrainGrass;
using NLog;
using System.Collections.Generic;
using System.Linq;
using EnumerableExtensions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Test.Src.Editor
{
    public class VisualKiller : IProcessSceneWithReport
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public static bool Enabled { get; set; } = false;

        public int callbackOrder => 2;

        [UnityEditor.MenuItem("Build/Kill visuals on open scenes")]
        public static void TransformOpenScenes()
        {
            var vk = new VisualKiller();
            Enumerable.Range(0, SceneManager.sceneCount).Select(SceneManager.GetSceneAt).ForEach(v => vk.OnProcessScene(v, null));
        }

        public static IEnumerable<System.Type> TypesToKill { get; } = new[]
        {
            //typeof(pb_Entity),
            //typeof(pb_Object),
            //typeof(Assets.Src.Aspects.ClientViewed),
            //typeof(Assets.Src.Effects.AnimatorEffects.AnimationEventRelay),
            //typeof(Assets.Src.Effects.AnimatorEffects.AnimationEventRelayWithSurface),
            //typeof(Assets.Src.Effects.AnimatorEffects.AnimationEventRelayTrail),
            //typeof(FracturedObject),
            //typeof(DetailObject),
            //typeof(MeshBlending),
            //typeof(GrassManager),
            //typeof(ReliefTerrain),
            typeof(Terrain),
            //typeof(CoatingObject),
            typeof(TextMesh),
            //typeof(RamSpline),
            //typeof(ThreeEyedGames.Decal),
            typeof(TMPro.TextMeshPro),
            typeof(Cloth),
            //typeof(Outline),
            typeof(MeshFilter),
            typeof(Renderer),
            typeof(ParticleSystem),
            typeof(LODGroup),
            //typeof(AwesomeTechnologies.VegetationSystem),
            //typeof(AwesomeTechnologies.VegetationStudio.VegetationStudioManager),
            //typeof(AwesomeTechnologies.Vegetation.PersistentStorage.PersistentVegetationStorage),
            typeof(Wind)
        };
        //public static IEnumerable<System.Type> PossiblyDangerousTypesToKillInAssets { get; } = new[]
        //{
        //    typeof(pb_Entity),
        //    typeof(pb_Object)
        //};
        private static IEnumerable<Object> FindObjectsOfTypeInScene(Scene scene, System.Type type)
        {
            foreach (var obj in scene.GetRootGameObjects())
                foreach (var comp in obj.GetComponentsInChildren(type, true))
                    yield return comp;
        }

        private void Process(Scene scene)
        {
            TypesToKill.SelectMany(v => FindObjectsOfTypeInScene(scene, v)).ToList().ForEach(Object.DestroyImmediate);
            //VisualKiller.PossiblyDangerousTypesToKillInAssets.SelectMany(v => FindObjectsOfTypeInScene(scene, v)).ToList().ForEach(v => { Logger.IfWarn()?.Message($"{v} has a component {v.GetType().Name} that we have to destroy, but that might be dangerous"); Object.DestroyImmediate(v, true); }).Write();
        }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (!Enabled)
                return;

            TypesToKill.SelectMany(v => FindObjectsOfTypeInScene(scene, v)).ToList().ForEach(Object.DestroyImmediate);
            //VisualKiller.PossiblyDangerousTypesToKillInAssets.SelectMany(v => FindObjectsOfTypeInScene(scene, v)).ToList().ForEach(v => { Logger.IfWarn()?.Message($"{v} has a component {v.GetType().Name} that we have to destroy, but that might be dangerous"); Object.DestroyImmediate(v, true); }).Write();
        }
    }
}
