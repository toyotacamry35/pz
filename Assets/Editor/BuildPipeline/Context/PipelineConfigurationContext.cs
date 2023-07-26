using System.IO;
using Assets.Src.BuildScripts;

namespace Assets.Test.Src.Editor
{
    public class PipelineConfigurationContext : IPipelineConfigurationContext
    {
        public DependencyCollectType DependencyCollectType { get; }
        public string MapName { get; }
        public string TargetPath { get; }
        public string SharedPath { get; }
        public string TempPath { get; }
        public BuildConfig Config { get; }

        public PipelineConfigurationContext(string mapName, string targetPath, string sharedPath, DependencyCollectType dependencyCollectType, BuildConfig config)
        {
            DependencyCollectType = dependencyCollectType;
            MapName = mapName;
            Config = config;
            TargetPath = targetPath;
            SharedPath = sharedPath;
            TempPath = UnityEditorUtils.LocalCachePath($"TempBundleStorage");
        }
    }
}