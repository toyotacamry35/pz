using UnityEditor.Build.Pipeline.Interfaces;

public interface IBuildPipelineSettings : IContextObject
{
    string[] ReleaseMapNames { get; }
    bool CleanMapDependencyCacheBeforeBuild { get; }
    bool DependencyCache { get; }
    bool UseCacheServer { get; }
    bool RemoteDependencyCache { get; set; }
    bool UseBuildCache { get; }
    bool DebugBuildPipeline { get; set; }
}