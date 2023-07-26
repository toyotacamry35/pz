using Assets.Src.BuildScripts;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public interface IPipelineConfigurationContext : IContextObject
    {
        DependencyCollectType DependencyCollectType { get; }
        string MapName { get; }
        string TargetPath { get; }
        string SharedPath { get; }
        string TempPath { get; }
        BuildConfig Config { get; }
    }
}