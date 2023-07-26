using Assets.Test.Src.Editor;
using UnityEditor;

namespace Assets.Src.BuildScripts.Editor
{
    public static class BuildWindow
    {
        [MenuItem("Build/Copy Game Resources")]
        public static void CopyRes()
        {
            GameResourcesUpdate.CopyGameResources(BuildResourcesInitializer.CommonConfig.GameResourcesFolderPath);
        }
    }
}
