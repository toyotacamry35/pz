using Assets.Src.BuildScripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    [InitializeOnLoad]
    public static class BuildResourcesInitializer
    {
        const string commonConfigPath = "Assets/BuildConfig.asset";
        const string commonReleasePath = "Assets/BuildConfigRelease.asset";
        const string headlessConfigPath = "Assets/BuildConfigHeadless.asset";

        const string commonConfigName = "com.enplexgames.colony.commonconfig";
        const string releaseConfigName = "com.enplexgames.colony.releaseconfig";
        const string headlessConfigName = "com.enplexgames.colony.headlessconfig";

        public static BuildConfig CommonConfig => Get(commonConfigName);
        public static BuildConfig HeadlessConfig => Get(headlessConfigName);
        public static BuildConfig ReleaseConfig => Get(releaseConfigName);

        private static BuildConfig Get(string name)
        {
            BuildConfig tgt;
            EditorBuildSettings.TryGetConfigObject(name, out tgt);
            return tgt;
        }

        static BuildResourcesInitializer()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            if (GetOrCreateConfig(commonConfigName, commonConfigPath) == null)
            {
                Debug.LogError(string.Format("CommonConfig == null. name = {0}, path = {1}", commonConfigName, commonConfigPath));
            }
            if (GetOrCreateConfig(releaseConfigName, commonReleasePath) == null)
            {
                Debug.LogError(string.Format("ReleaseConfig == null. name = {0}, path = {1}", releaseConfigName, commonReleasePath));
            }
            if (GetOrCreateConfig(headlessConfigName, headlessConfigPath) == null)
            {
                Debug.LogError(string.Format("HeadlessConfig == null. name = {0}, path = {1}", headlessConfigName, headlessConfigPath));
            }
        }

        static BuildConfig GetOrCreateConfig(string name, string path = null)
        {
            BuildConfig data = null;

            if (! EditorBuildSettings.TryGetConfigObject<BuildConfig>(name, out data) && !string.IsNullOrEmpty(path))
            {
                data = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildConfig>(path);
                EditorBuildSettings.AddConfigObject(name, data, false);
                Debug.LogError(string.Format("Create BuildConfig. name = {0}, path = {1}, data = {2}", name, path, data != null ? data.name : "null"));
            }

            return data;
        }
    }
}
