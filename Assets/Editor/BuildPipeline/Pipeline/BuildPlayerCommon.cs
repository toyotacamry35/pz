using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.App;
using Assets.Src.BuildScripts;
using Assets.Src.ResourceSystem.Editor;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Test.Src.Editor
{
    public static class BuildPlayerCommon
    {
        private static readonly Logger Logger = LogManager.GetLogger("BuildProcess");

        private static string buildingTempFile = "build.lock";

        public static bool IsLockExist() => File.Exists($"{UnityEditorUtils.LibraryPath()}/{buildingTempFile}");

        public static int BuildPlayer(BuildConfig cfg)
        {
            return Build(new PlayerBuilder(buildingTempFile, cfg), BuildPipelineManager.GetBuildPipelineSettings());
        }

        public static int BuildMap(string mapName, bool invalidate = false, bool ignoreSettings = false)
        {
            if (invalidate || (!ignoreSettings && BuildPipelineManager.GetBuildPipelineSettings().CleanMapDependencyCacheBeforeBuild))
                DependencyCacheController<MapCacheEntry>.InvalidateCache(mapName);

            var pipeline = new AssetBundleBuildPipeline(mapName, DependencyCollectType.Map, BuildResourcesInitializer.CommonConfig);
            pipeline.Setup();
            return pipeline.BuildBundle();
        }

        public static int BuildSystem(bool invalidate = false, bool ignoreSettings = false)
        {
            if (invalidate || (!ignoreSettings && BuildPipelineManager.GetBuildPipelineSettings().CleanMapDependencyCacheBeforeBuild))
                DependencyCacheController<SystemCacheEntry>.InvalidateCache(AssetBundleHelper.SystemName);

            var pipeline = new AssetBundleBuildPipeline(AssetBundleHelper.SystemName, DependencyCollectType.System, BuildResourcesInitializer.CommonConfig);
            pipeline.Setup();
            return pipeline.BuildBundle();
        }

        public static int BuildShared(bool invalidate = false, bool ignoreSettings = false)
        {
            if (invalidate || (!ignoreSettings && BuildPipelineManager.GetBuildPipelineSettings().CleanMapDependencyCacheBeforeBuild))
                InvalidateCacheAll();
            
            var pipeline = new AssetBundleBuildPipeline(AssetBundleHelper.SharedName, DependencyCollectType.System, BuildResourcesInitializer.CommonConfig);
            pipeline.Setup();
            return pipeline.BuildBundle();
        }

        public static int BuildMaps(IEnumerable<string> mapNames, bool invalidate = false, bool ignoreSettings = false)
        {
            return mapNames
                .Select(
                    x =>
                    {
                        DependencyCacheController<MapCacheEntry>.InvalidateCache(x);
                        return x;
                    })
                .Select(x => BuildMap(x, invalidate, ignoreSettings))
                .Aggregate(Mathf.Max);
        }

        public static void InvalidateCacheAll()
        {
            DependencyCacheController<MapCacheEntry>.InvalidateCacheAll();
            DependencyCacheController<SystemCacheEntry>.InvalidateCacheAll();
        }

        public static void CleanBuild(bool cleanFullFolder = true)
        {
            JsonCustomImporter.ReimportAll();
            InvalidateCacheAll();
            if(cleanFullFolder)
                CleanBundleFolder();
            else
                CleanSharedFolder();
        }

        internal static void CleanBundleFolder()
        {
            var bundleFolder = $"{BuildResourcesInitializer.CommonConfig.BundlesFolderPath}";
            Logger.IfInfo()?.Message($"Shared path: {bundleFolder}").Write();
            if (Directory.Exists(bundleFolder))
                Directory.Delete(bundleFolder,true);
        }
        
        internal static void CleanSharedFolder()
        {
            var sharedPath = $"{BuildResourcesInitializer.CommonConfig.BundlesFolderPath}/{AssetBundleHelper.SharedName}";
            Logger.IfInfo()?.Message($"Shared path: {sharedPath}").Write();
            if (Directory.Exists(sharedPath))
                Directory.Delete(sharedPath,true);
        }

        private static int Build(
            IBuilder builder,
            IBuildPipelineSettings settings,
            Action beforeCallback = null,
            Action afterCallback = null)
        {
            if (beforeCallback != null)
                builder.BeforeBuild += beforeCallback;

            if (afterCallback != null)
                builder.AfterBuild += afterCallback;

            return builder.Build();
        }
    }
}