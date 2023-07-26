using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.BuildScripts;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using Infrastructure.Config;
using NLog;
using NLog.Fluent;
using ResourcesSystem.Loader;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public class SystemCollector : IBuildDependencyCollector
    {
        private readonly Logger _logger = LogManager.GetLogger("BuildProcess");

        private ReferenceMapping[] AllUnityRefsFromJdb { get; } = AssetDatabase.FindAssets("t: JdbMetadata")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<JdbMetadata>)
            .SelectMany(v => v.UnityRefs)
            .Distinct()
            .ToArray();

        public AssetBundleBuild[] Collect(string mapName, BuildConfig config)
        {
            List<(string, string)> addresses;
            string[] scenes;
            string[] sceneDeps;

            if (DependencyCacheController<SystemCacheEntry>.UseCache && DependencyCacheController<SystemCacheEntry>.Validate(mapName))
            {
                var cacheEntry = DependencyCacheController<SystemCacheEntry>.ReadCache(mapName);

                addresses = cacheEntry.Addresses;
                scenes = cacheEntry.Scenes;
                sceneDeps = cacheEntry.SceneDeps;
            }
            else
            {
                var mapDef = GameResourcesHolder.Instance.LoadResource<MapRootDef>("/Scenes/MapRoot");

                var jdbDirectRefs = AllUnityRefsFromJdb;
                var allDepsOfJdb = AssetDatabase.GetDependencies(
                        jdbDirectRefs
                            .Select(v => v.Resolved)
                            .ToArray(),
                        true)
                    .Distinct()
                    .Union(
                        jdbDirectRefs
                            .Select(v => v.Resolved))
                    .FilterByAssetType()
                    .ToList();

                var rawRefs = allDepsOfJdb
                    .GroupJoin(jdbDirectRefs, dep => dep, address => address.Resolved, (dep, grp) => new {dep, grp})
                    .Select(
                        t => new
                        {
                            t, distinct = t.grp
                                .Select(v => v.Original)
                                .Distinct()
                        })
                    .Select(t => ValueTuple.Create(t.t.dep, t.distinct.ToArray()))
                    .ToList();

                var validRefs = rawRefs
                    .Where(
                        @ref => !@ref.Item2
                            .Skip(1)
                            .Any())
                    .Select(@ref => ValueTuple.Create(@ref.Item1, @ref.Item2.SingleOrDefault()))
                    .ToList();

                var noExtRefs = rawRefs
                    .Where(
                        @ref => @ref.Item2
                            .Skip(1)
                            .Any())
                    .Select(
                        @ref => new
                        {
                            @ref, noExt = ValueTuple.Create(
                                @ref.Item1,
                                @ref.Item2.Select(v => Path.ChangeExtension(v, null))
                                    .Distinct()
                                    .ToArray())
                        })
                    .Select(@t => @t.noExt)
                    .ToList();

                var semiValidRefs = noExtRefs
                    .Where(
                        @ref => !@ref.Item2
                            .Skip(1)
                            .Any())
                    .Select(@ref => ValueTuple.Create(@ref.Item1, @ref.Item2.SingleOrDefault()))
                    .ToList();

                var invalidRefs = noExtRefs
                    .Where(
                        @ref => @ref.Item2
                            .Skip(1)
                            .Any())
                    .ToList();

                if (invalidRefs.Any())
                {
                    foreach (var @ref in invalidRefs)
                        _logger.IfError()
                            ?.Message(
                                "Multiple different references to same object. Object: {0}, Refs: ({1})",
                                @ref.Item1,
                                string.Join(", ", @ref.Item2))
                            .UnityObj(AssetDatabase.LoadMainAssetAtPath(@ref.Item1))
                            .Write();

                    EditorUtility.ClearProgressBar();
                }

                addresses = validRefs.Union(semiValidRefs).ToList();
                scenes = mapDef.SystemScenes.Concat(mapDef.KeyDependencyScenes.Select(v => v.Path)).Distinct().ToArray();
                sceneDeps = AssetDatabase.GetDependencies(scenes, true).Distinct().FilterByAssetType().Except(allDepsOfJdb).ToArray();

                var cacheEntry = new SystemCacheEntry {Addresses = addresses, IsValid = true, Scenes = scenes, SceneDeps = sceneDeps};
                DependencyCacheController<SystemCacheEntry>.WriteCache(mapName, cacheEntry);
            }

            var globalBundleWithDependencies = new AssetBundleBuild {assetBundleName = "System-Global.bundle", assetNames = sceneDeps};
            var jdbBundleWithDependencies = new AssetBundleBuild {assetBundleName = "System-Jdb.bundle", assetNames = addresses.Select(v => v.Item1).ToArray(), addressableNames = addresses.Select(v => v.Item2).ToArray()};

            var sceneBundles = scenes
                .Select(scenePath => new {scenePath, bundleContents = new[] {scenePath}})
                .Select(t => new {t, bundleName = "System-" + Path.GetFileNameWithoutExtension(t.scenePath)})
                .Select(t => new AssetBundleBuild {assetBundleName = t.bundleName + ".bundle", assetNames = t.t.bundleContents})
                .ToArray();

            AssetBundleBuild[] nonSceneBundles = {globalBundleWithDependencies, jdbBundleWithDependencies};
            return sceneBundles.Concat(nonSceneBundles).ToArray();
        }
    }
}