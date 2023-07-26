using Assets.Editor.SceneProcessors;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace Assets.TerrainBlend
{
    public class MeshBlendingSetupPostProcess : IProcessSceneWithReport
    {
        public int callbackOrder => 99;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            foreach (var meshblending in SceneHelpers.CollectComponents<MeshBlending>(scene))
            {
                meshblending.SetupRenderers();
                meshblending.SetupDefault();
            }

        }
    }
}
