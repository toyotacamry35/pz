using System;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

public static class BuildPipelineManager
{
    private const string BuildPipelineSettings = "BuildPipelineSettings";
    private const string SettingsPath = "Assets/Data/DataAssets/BuildSettings/BuildPipelineSettings.asset";

    public static IBuildPipelineSettings GetBuildPipelineSettings()
    {
        if (EditorBuildSettings.TryGetConfigObject(BuildPipelineSettings, out BuildPipelineSettings buildPipelineSettings) && buildPipelineSettings != default) 
            return buildPipelineSettings;
        
        AddDefaultSettings();
        if (!EditorBuildSettings.TryGetConfigObject(BuildPipelineSettings, out buildPipelineSettings))
            throw new BuildSettingsException("Cant't get default settings for bundle build");
        return buildPipelineSettings;
    }

    private static void AddDefaultSettings()
    {
        var settingsObject = AssetDatabase.LoadAssetAtPath<BuildPipelineSettings>(SettingsPath);
        if (!settingsObject)
        {
            settingsObject = ScriptableObject.CreateInstance<BuildPipelineSettings>();
            try
            {
                AssetDatabase.CreateAsset(settingsObject, SettingsPath);
            }
            catch (Exception e)
            {
                throw new BuildSettingsException("Cant't add default settings for bundle build", e);
            }
        }

        EditorBuildSettings.AddConfigObject(BuildPipelineSettings, settingsObject, true);
        AssetDatabase.SaveAssets();
    }
}

[Serializable]
public class BuildSettingsException : Exception
{
    public BuildSettingsException()
    {
    }

    public BuildSettingsException(string message) : base(message)
    {
    }

    public BuildSettingsException(string message, Exception inner) : base(message, inner)
    {
    }

    protected BuildSettingsException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}