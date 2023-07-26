using System;
using System.Linq;
using Assets.Src.BuildScripts;
using Core.Environment.Logging.Extension;
using NLog;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public class MapCollector : IBuildDependencyCollector
    {
        private readonly Logger _logger = LogManager.GetLogger("BuildProcess");

        public AssetBundleBuild[] Collect(string mapName, BuildConfig config)
        {
            _logger.Info("Collect dependencies {0}", mapName);

            AssetBundleBuild[] dependencies;
            if (DependencyCacheController<MapCacheEntry>.UseCache && DependencyCacheController<MapCacheEntry>.Validate(mapName))
            {
                var cacheEntry = DependencyCacheController<MapCacheEntry>.ReadCache(mapName);
                var name = cacheEntry.Name;
                var deps = cacheEntry.Dependencies;
                dependencies = name.Select((t, i) => new AssetBundleBuild {assetBundleName = t, assetNames = deps[i]}).ToArray();
            }
            else
            {
                var collector = new MapDependencyCollector(mapName, config);
                dependencies = collector.CollectMapDependency();
                if (dependencies == null)
                    throw new InvalidOperationException("Could not gather dependency");

                var cacheEntry = new MapCacheEntry {
                    Name = dependencies.Select(t => t.assetBundleName).ToList(), 
                    Dependencies = dependencies.Select(t => t.assetNames).ToList(), 
                    IsValid = true};
                DependencyCacheController<MapCacheEntry>.WriteCache(mapName, cacheEntry);
            }

            _logger.IfInfo()?.Message($"Collected dependencies count: {dependencies.Length}").Write();
            return dependencies;
        }
    }
}