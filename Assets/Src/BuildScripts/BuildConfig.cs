using System;
using System.IO;
using UnityEngine;

namespace Assets.Src.BuildScripts
{
    [CreateAssetMenu(fileName = "BuildConfig", menuName = "BuildConfig")]
    public class BuildConfig : ScriptableObject
    {
        public string BuildLocation; // !Build/UnityBuild/Bin
        public string RuntimeGameResourcesFolderName; // GameResources
        public string ExeName = "Tonarchy.exe";
        public string BundlesFolderName; // Bundles
        [Header("BuildOptions")]

        public bool AllowDebugging;
        public bool Development;
        public bool ScriptOnly;

        public bool Headless;

        public string[] AddDefines;
        public string[] RemoveDefines;
        
        public string BuildLocationNormalized => new Uri(Path.Combine(Application.dataPath, Path.GetFullPath(BuildLocation))).LocalPath;

        public string BundlesFolderPath => new Uri(Path.Combine(BuildLocationNormalized, "..", BundlesFolderName)).LocalPath; // !Build/UnityBuild/Bundles

        public string GameResourcesFolderPath => new Uri(Path.Combine(BuildLocationNormalized, "..", "..", RuntimeGameResourcesFolderName)).LocalPath; //!Build/GameResources
        public string LocalAssetsGameResourcesFolderPath => new Uri(Path.Combine(BuildLocationNormalized, "..", "..", "Assets")).LocalPath; //colony_proto/Assets


        public string RuntimeGameResourcesFolderPath => new Uri(Application.isEditor ? Application.dataPath : Path.Combine(Application.dataPath, "..", "..", "..", RuntimeGameResourcesFolderName)).LocalPath;
        public string RuntimeBundlesFolderPath => new Uri(Path.Combine(Application.dataPath, "..", "..", BundlesFolderName)).LocalPath;
    }
}
