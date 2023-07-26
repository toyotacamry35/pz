using System;
using System.Linq;
using UnityEditor;

[InitializeOnLoad]
sealed class BuildPipelineDebugMode
{
    const string k_Define = "SBP_PROFILER_ENABLE";
        
    static BuildPipelineDebugMode()
    {
        var settings = BuildPipelineManager.GetBuildPipelineSettings();
        ApplyDef(settings);
    }

    private static void ApplyDef(IBuildPipelineSettings settings)
    {
        var targets = Enum.GetValues(typeof(BuildTargetGroup))
            .Cast<BuildTargetGroup>()
            .Where(x => x != BuildTargetGroup.Unknown)
            .Where(x => !IsObsolete(x));

        foreach (var target in targets)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

            var list = defines.Split(';', ' ')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (settings.DebugBuildPipeline && !list.Contains(k_Define))
            {
                list.Add(k_Define);
                defines = list.Aggregate((a, b) => a + ";" + b);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }
            if (!settings.DebugBuildPipeline && list.Contains(k_Define))
            {
                list.Remove(k_Define);
                defines = list.Aggregate((a, b) => a + ";" + b);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }
        }
    }
    
    public static void SetMode(bool debugMode)
    {
        var settings = BuildPipelineManager.GetBuildPipelineSettings();
        settings.DebugBuildPipeline = debugMode;
        ApplyDef(settings);
    }

    static bool IsObsolete(BuildTargetGroup group)
    {
        var attrs = typeof(BuildTargetGroup)
            .GetField(group.ToString())
            .GetCustomAttributes(typeof(ObsoleteAttribute), false);

        return attrs != null && attrs.Length > 0;
    }
}