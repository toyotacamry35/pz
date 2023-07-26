using UnityEditor;

namespace Assets.Tools.Editor
{
    [InitializeOnLoad]
    public static class BuildCacheLimitSetter
    {
        private const int BuildCacheSizeGb = 50;
        static BuildCacheLimitSetter()
        {
            EditorPrefs.SetInt("BuildCache.maximumSize", BuildCacheSizeGb);
        }

        public static void Prune()
        {
            UnityEditor.Build.Pipeline.Utilities.BuildCache.PruneCache_Background(BuildCacheSizeGb);
        }
    }
}
