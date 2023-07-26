using Assets.Test.Src.Editor;
using System.IO;
using UnityEditor;
using UnityEditor.Build;

namespace Assets.Src.BuildScripts.Editor
{
    public class CopyResourcesBuildPostProcess : IPostprocessBuild
    {
        public int callbackOrder => 2;

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            GameResourcesUpdate.CopyGameResources(Path.Combine(path, "..", "..", "..", BuildResourcesInitializer.CommonConfig.RuntimeGameResourcesFolderName));
        }
    }
}
