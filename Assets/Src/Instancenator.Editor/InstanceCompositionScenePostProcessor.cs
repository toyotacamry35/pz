using Assets.Editor.SceneProcessors;
using Assets.Instancenator;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Src.Instancenator
{
    public class InstanceCompositionScenePostProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => 100;
        string path = "Assets/Src/Instancenator/Shaders/InstancenatorComputeShader.compute";

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            foreach (var instanceComposition in SceneHelpers.CollectComponents<InstanceCompositionRenderer>(scene))
            {
                if (instanceComposition.computeShader == default)
                    instanceComposition.computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(path);
            }
        }
    }
}
