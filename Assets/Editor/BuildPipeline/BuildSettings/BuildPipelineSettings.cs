using System;
using GeneratedCode.Custom.Config;
using Infrastructure.Config;
using ResourcesSystem.Loader;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BuildPipelineSettings : ScriptableObject, IBuildPipelineSettings
{
    [SerializeField]
    private string[] releaseMaps;

    public string[] ReleaseMapNames => releaseMaps;

    [SerializeField]
    private bool cleanMapDependencyCacheBeforeBuild = true;
    public bool CleanMapDependencyCacheBeforeBuild => cleanMapDependencyCacheBeforeBuild;

    [SerializeField]
    private bool useCacheServer = true;
    public bool UseCacheServer => useCacheServer;

    [SerializeField]
    private bool dependencyCache = false;
    public bool DependencyCache => dependencyCache;
    
    [SerializeField]
    private bool remoteDependencyCache = false;
    public bool RemoteDependencyCache
    {
        get => remoteDependencyCache;
        set => remoteDependencyCache = value;
    }

    [SerializeField]
    private bool useBuildCache = true;
    public bool UseBuildCache => useBuildCache;
    
    
    [SerializeField]
    private bool debugBuildPipeline = true;
    public bool DebugBuildPipeline
    {
        get => debugBuildPipeline;
        set => debugBuildPipeline = value;
    }

}