using System.Linq;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchTerrainComposerBuildProcessor// : IProcessScene
{
    public int callbackOrder => 2;

    public void OnProcessScene(Scene scene)
    {
        scene.GetRootGameObjects().Where(v => v.name == "TerrainComposer2").ToList().ForEach(Object.DestroyImmediate);
    }
}
