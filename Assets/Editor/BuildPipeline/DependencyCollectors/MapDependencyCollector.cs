using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assets.Src.App;
using Assets.Src.BuildScripts;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using Infrastructure.Config;
using NLog;
using ResourcesSystem.Loader;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public class MapDependencyCollector
    {
        private static readonly Logger Logger = LogManager.GetLogger("BuildProcess");

        private string _mapName;
        private BuildConfig _config;

        public MapDependencyCollector(string mapName, BuildConfig config)
        {
            _mapName = mapName;
            _config = config;
        }

        public AssetBundleBuild[] CollectMapDependency()
        {
            //var watch = new Stopwatch();
            //watch.Start();
            
            //Logger.IfInfo()?.Message($"Current time step: {watch.ElapsedMilliseconds}").Write();
            
            var mapDef = GameResourcesHolder.Instance.LoadResource<MapRootDef>("/Scenes/MapRoot");

            var map = mapDef.Maps.Single(v => String.Compare(v.Target.DebugName, _mapName, StringComparison.OrdinalIgnoreCase) == 0).Target;

            // Logger.IfInfo()?.Message("Collecting dependencies").Write();;
            if (EditorUtility.DisplayCancelableProgressBar("Building", "Collecting dependencies", 0))
            {
                EditorUtility.ClearProgressBar();
                 Logger.IfError()?.Message("Build process was cancelled").Write();;
                return null;
            }

            var serverScenes = map.GlobalScenesServer;
            var clientScenes = map.AllScenesToBeLoadedOnClientViaJdb;

            var commonScenes = serverScenes.Intersect(clientScenes).ToArray();
            var serverOnlyScenes = serverScenes.Except(clientScenes).ToArray();
            var clientOnlyScenes = clientScenes.Except(serverScenes).ToList();
            
            //Logger.IfInfo()?.Message($"Current time step prepare scenes: {watch.ElapsedMilliseconds}").Write();
            
            //var specialwatch = new Stopwatch();
            var clientDependencies = clientOnlyScenes.Select(
                    scene =>
                    {
                        //specialwatch.Start();
                        var deps = AssetDatabase.GetDependencies(scene, true);
                        //specialwatch.Stop();
                        /*Logger.IfInfo()?.Message($"====================================================== SceneName {scene} and count: {deps.Length}").Write();
                        foreach (var dep in deps)
                        {
                            Logger.IfInfo()?.Message($"Scene {scene}: {dep}").Write();
                        }*/

                        return new {scene, deps};
                    }).ToList();
            
            //ogger.Info($"Special watch: {specialwatch.ElapsedMilliseconds}");
            //Logger.IfInfo()?.Message($"Current time step collect scene deps: {watch.ElapsedMilliseconds}").Write();

            var clientDependenciesToScenes = new Dictionary<string, List<string>>();
            clientDependencies
                .SelectMany(t => t.deps.Select(dep => new {dep, t.scene}))
                .ForEach(
                    t =>
                    {
                        if (clientDependenciesToScenes.ContainsKey(t.dep))
                            clientDependenciesToScenes[t.dep].Add(t.scene);
                        else
                            clientDependenciesToScenes.Add(t.dep, new List<string> {t.scene});
                    });
            
            //Logger.IfInfo()?.Message($"Current time step prepare dictionary: {watch.ElapsedMilliseconds}").Write();

            var clientOnlyScenesDepsRaw = clientDependencies.SelectMany(deps => deps.deps).Distinct();
            var serverOnlyScenesDepsRaw = AssetDatabase.GetDependencies(serverOnlyScenes, true).Distinct();
            var commonScenesDepsRaw = AssetDatabase.GetDependencies(commonScenes, true).Distinct();
            
            //Logger.IfInfo()?.Message($"Current time step get server and common scene deps: {watch.ElapsedMilliseconds}").Write();

             Logger.IfInfo()?.Message("Raw dependencies collected, filtering").Write();;
            if (EditorUtility.DisplayCancelableProgressBar("Building", "Filtering dependencies", 0.25f))
            {
                EditorUtility.ClearProgressBar();
                 Logger.IfError()?.Message("Build process was cancelled").Write();;
                return null;
            }

            //Logger.IfInfo()?.Message($"Start filter: {watch.ElapsedMilliseconds}").Write();
            var clientOnlyScenesDeps = clientOnlyScenesDepsRaw.FilterByAssetType();
            //Logger.IfInfo()?.Message($"Start filter 1: {watch.ElapsedMilliseconds}").Write();
            var serverOnlyScenesDeps = serverOnlyScenesDepsRaw.FilterByAssetType();
            //Logger.IfInfo()?.Message($"Start filter 2: {watch.ElapsedMilliseconds}").Write();
            var commonScenesDeps = commonScenesDepsRaw.FilterByAssetType().ToList();
            
            //Logger.IfInfo()?.Message($"Current time step filter 3: {watch.ElapsedMilliseconds}").Write();


            var clientDeps = commonScenesDeps.Union(clientOnlyScenesDeps).ToList();
            var serverDeps = commonScenesDeps.Union(serverOnlyScenesDeps).ToList();
            //Logger.IfInfo()?.Message($"Current time union: {watch.ElapsedMilliseconds}").Write();

             Logger.IfInfo()?.Message("Raw dependencies filtered, finding netIds").Write();;
            if (EditorUtility.DisplayCancelableProgressBar("Building", "Finding NetIds", 0.5f))
            {
                EditorUtility.ClearProgressBar();
                 Logger.IfError()?.Message("Build process was cancelled").Write();;
                return null;
            }

            var allSharedDeps = clientDeps.Intersect(serverDeps).ToArray();
            var allServerOnlyDeps = serverDeps.Except(allSharedDeps).ToArray();
            var allClientOnlyDeps = clientDeps.Except(allSharedDeps).ToList();
            //Logger.IfInfo()?.Message($"Current time intersect all deps: {watch.ElapsedMilliseconds}").Write();

             Logger.IfInfo()?.Message("Bundle contents assigned, generating bundle info").Write();;
            if (EditorUtility.DisplayCancelableProgressBar("Building", "Generating bundle info", 0.75f))
            {
                EditorUtility.ClearProgressBar();
                 Logger.IfError()?.Message("Build process was cancelled").Write();;
                return null;
            }

            var serverNonSceneBundle = new[] {new AssetBundleBuild {assetBundleName = $"{_mapName}-Server.bundle", assetNames = allServerOnlyDeps}};
            var sharedNonSceneBundle = new[] {new AssetBundleBuild {assetBundleName = $"{_mapName}-Shared.bundle", assetNames = allSharedDeps}};

            var filterClientSceneSharedBundle = allClientOnlyDeps
                .Where(
                    deps =>
                    {
                        var c = clientDependenciesToScenes.ContainsKey(deps) && clientDependenciesToScenes[deps].Count >= 2;
                        return c;
                    })
                .Aggregate(
                    ValueTuple.Create($"{_mapName}-Client-Scene-Shared.bundle", new List<string>()),
                    (total, next) =>
                    {
                        total.Item2.Add(next);
                        return total;
                    });
            
            //Logger.IfInfo()?.Message($"Current time prepare Client-Scene-Shared.bundle: {watch.ElapsedMilliseconds}").Write();
            var clientSceneSharedBundle = new List<(string, List<string>)> {filterClientSceneSharedBundle}
                .Select(deps => new AssetBundleBuild {assetBundleName = deps.Item1, assetNames = deps.Item2.ToArray()});

            var uniqueSceneResources = allClientOnlyDeps
                .Where(deps => clientDependenciesToScenes.ContainsKey(deps) && clientDependenciesToScenes[deps].Count == 1)
                .Select(deps => new {scene = clientDependenciesToScenes[deps][0], deps = deps})
                .GroupBy(deps => deps.scene, deps => deps.deps, (scene, deps) => new AssetBundleBuild {assetBundleName = PathExtension.GetPathWithoutExtension(scene) + ".bundle", assetNames = deps.ToArray()});

            
            //Logger.IfInfo()?.Message($"Current time prepare uniqueSceneResources: {watch.ElapsedMilliseconds}").Write();
            var clientOtherShared = new[]
            {
                new AssetBundleBuild
                {
                    assetBundleName = $"{_mapName}-Client-Left-Shared.bundle",
                    assetNames = allClientOnlyDeps
                        .Where(deps => !clientDependenciesToScenes.ContainsKey(deps) || clientDependenciesToScenes[deps].Count == 0)
                        .ToArray()
                },
            };
            
            //Logger.IfInfo()?.Message($"Current time prepare Client-Left-Shared.bundle: {watch.ElapsedMilliseconds}").Write();

            var sceneBundles = map.AllScenesToBeLoadedViaJdb
                .Select(scenePath => new {scenePath, bundleContents = Enumerable.Repeat(scenePath, 1).ToArray()})
                .Select(t => new {t, bundleName = _mapName + "-" + Path.GetFileNameWithoutExtension(t.scenePath) + ".bundle"})
                .Select(t => new AssetBundleBuild {assetBundleName = t.bundleName, assetNames = t.t.bundleContents})
                .ToArray();

            //Logger.IfInfo()?.Message($"Current time prepare sceneBundles: {watch.ElapsedMilliseconds}").Write();
            //watch.Stop();
            
            return sceneBundles.Concat(sharedNonSceneBundle).Concat(clientSceneSharedBundle).Concat(uniqueSceneResources).Concat(clientOtherShared).Concat(serverNonSceneBundle).ToArray(); //.Concat(excludedNonSceneBundle);
        }
    }
}