using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.App;
using Assets.Src.Lib.Unity;
using EnumerableExtensions;
using Harmony;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

namespace Assets.Test.Src.Editor
{
    public class FilterAllDependencies : IBuildTask
    {
        [InjectContext(ContextUsage.InOut)]
        private ICollectorContext _collectorContext;
        
        [InjectContext(ContextUsage.In)]
        IProfileContext _profileContext;

        public int Version => 1;

        private List<(string, string)> Zip(string[] names, string[] address)
        {
            if (address == null || address.Length < names.Length)
                return names.Select(n => ValueTuple.Create(n, "")).ToList();

            var result = new List<(string, string)>();
            for (int i = 0; i < names.Length; i++)
            {
                result.Add(ValueTuple.Create(names[i], address[i]));
            }

            return result;
        }

        public ReturnCode Run()
        {
            var groups = _collectorContext.ProjectDependencies
                .SelectMany(x => x.Value, (list, dependencies) => ValueTuple.Create(list.Key, dependencies))
                .SelectMany(
                    x => Zip(x.Item2.assetNames, x.Item2.addressableNames),
                    (tuple, s) => new DependencyHolder {MapName = tuple.Item1, BundleName = tuple.Item2.assetBundleName, Asset = s.Item1, LoadName = s.Item2})
                .GroupBy(x => x.Asset)
                .ToArray();

            _collectorContext.ProjectDependencies.Clear();

            var allShaders = groups
                .Where(a => a.Key.EndsWith(".shader", StringComparison.OrdinalIgnoreCase) || a.Key.EndsWith(".compute", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            _collectorContext.SharedBundleToMapReferences.Add(
                AssetBundleHelper.ProduceCustomShaderBundleName(AssetBundleHelper.SharedName),
                new AssetBundleReference {Bundles = new List<MapBundle>()});
            var sharedShaders = allShaders
                .SelectMany(x => x)
                .Aggregate(
                    new AssetBundleBuildAggregator
                    {
                        assetBundleName = AssetBundleHelper.ProduceCustomShaderBundleName(AssetBundleHelper.SharedName)
                    },
                    (assetBundleBuild, holders) =>
                    {
                        if (assetBundleBuild.assetNames == null)
                            assetBundleBuild.assetNames = new List<string> {holders.Asset};
                        else if (!assetBundleBuild.assetNames.Contains(holders.Asset))
                            assetBundleBuild.assetNames.Add(holders.Asset);

                        if (assetBundleBuild.addressableNames == null)
                            assetBundleBuild.addressableNames = new List<string> {holders.LoadName};
                        else if (!assetBundleBuild.assetNames.Contains(holders.Asset))
                            assetBundleBuild.addressableNames.Add(holders.LoadName);

                        if (_collectorContext.SharedBundleToMapReferences[assetBundleBuild.assetBundleName]
                            .Bundles.All(m => m.Map != holders.MapName))
                            _collectorContext.SharedBundleToMapReferences[assetBundleBuild.assetBundleName]
                                .Bundles.Add(
                                    new MapBundle
                                    {
                                        Map = holders.MapName,
                                        Bundle = holders.BundleName
                                    });

                        return assetBundleBuild;
                    });

            var files = new DirectoryInfo("Assets/Data/DataAssets/GraphicsSettings").GetFiles("*.shadervariants");
            foreach (var file in files)
            {
                var relative = UnityEditorUtils.GetLocalPathFromLocalPath(file.FullName);
                sharedShaders.assetNames.Add(relative);
            }
            
          
            var shaderBundle = sharedShaders.GetBundle();
            _collectorContext.SharedBundleToMapReferences[AssetBundleHelper.ProduceCustomShaderBundleName(AssetBundleHelper.SharedName)].AssetBundleBuild = shaderBundle;

            var nonShaders = groups.Except(allShaders).ToArray();
            var shared = nonShaders.Where(x => x.Count() > 1).ToArray();
            var unique = nonShaders.Except(shared);


            var jdbPack = shared
                .Where(g => g.Any(d => d.BundleName.Contains("System-Jdb")))
                .ToArray();
            shared = shared.Except(jdbPack).ToArray();

            unique
                .SelectMany(x => x)
                .GroupBy(x => x.MapName)
                .ForEach(
                    map =>
                    {
                        var uniqueShaderBundle = new List<(string, string)>();
                        var bundles = map
                            .GroupBy(b => b.BundleName)
                            .Select(
                                b =>
                                    b.Aggregate(
                                        new AssetBundleBuildAggregator {assetBundleName = b.Key},
                                        (assetBundleBuild, holders) =>
                                        {
                                            if (holders.Asset.EndsWith(".shader", StringComparison.OrdinalIgnoreCase))
                                            {
                                                uniqueShaderBundle.Add(ValueTuple.Create(holders.Asset, holders.LoadName));
                                                return assetBundleBuild;
                                            }

                                            if (assetBundleBuild.assetNames == null)
                                                assetBundleBuild.assetNames = new List<string> {holders.Asset};
                                            else
                                                assetBundleBuild.assetNames.Add(holders.Asset);

                                            if (assetBundleBuild.addressableNames == null)
                                                assetBundleBuild.addressableNames = new List<string> {holders.LoadName};
                                            else
                                                assetBundleBuild.addressableNames.Add(holders.LoadName);
                                            return assetBundleBuild;
                                        }))
                            .Select(b => b.GetBundle())
                            .Where(b => b.assetNames.Length != 0)
                            .ToArray();

                        if (uniqueShaderBundle.Count > 0)
                        {
                            bundles = bundles.Concat(
                                    new[]
                                    {
                                        new AssetBundleBuild
                                        {
                                            assetBundleName = AssetBundleHelper.ProduceCustomShaderBundleName(map.Key), assetNames = uniqueShaderBundle.Select(b => b.Item1).ToArray(),
                                            addressableNames = uniqueShaderBundle.Select(b => b.Item2).ToArray()
                                        }
                                    })
                                .ToArray();
                        }

                        _collectorContext.ProjectDependencies.Add(map.Key, bundles);
                    });



            var jdbBundleBuild = _collectorContext.ProjectDependencies[AssetBundleHelper.SystemName].Single(b => b.assetBundleName.Contains("System-Jdb"));
            _collectorContext.ProjectDependencies[AssetBundleHelper.SystemName] = _collectorContext.ProjectDependencies[AssetBundleHelper.SystemName].Except(new[] {jdbBundleBuild}).ToArray();

            var aggregator = new AssetBundleBuildAggregator {assetBundleName = jdbBundleBuild.assetBundleName, assetNames = new List<string>(), addressableNames = new List<string>()};
            foreach (var dep in jdbBundleBuild.assetNames)
            {
                aggregator.assetNames.Add(dep);
            }

            foreach (var dep in jdbBundleBuild.addressableNames)
            {
                aggregator.addressableNames.Add(dep);
            }

            foreach (var dep in jdbPack.SelectMany(x => x))
            {
                if (aggregator.assetNames.Contains(dep.Asset)) continue;
                aggregator.assetNames.Add(dep.Asset);
                aggregator.addressableNames.Add(dep.LoadName);
            }

            _collectorContext.ProjectDependencies[AssetBundleHelper.SystemName] = _collectorContext.ProjectDependencies[AssetBundleHelper.SystemName].Concat(new[] {aggregator.GetBundle()}).ToArray();

            //Пока закпаковка одного ассета в отдельный бандл, то достаточно просто добавить лист ассетнеймов 
            var allShared = shared
                .SelectMany(x => x)
                .GroupBy(x => x.Asset)
                .Select(
                    b =>
                    {
                        var bundleName = AssetBundleHelper.ProduceBundleNameByPath(b.Key, AssetDatabase.AssetPathToGUID(b.Key));
                        _collectorContext.SharedBundleToMapReferences.Add(bundleName, new AssetBundleReference {Bundles = new List<MapBundle>()});
                        var bundleBuild = b.Aggregate(
                            new AssetBundleBuildAggregator
                            {
                                assetBundleName = bundleName
                            },
                            (assetBundleBuild, holders) =>
                            {
                                _collectorContext.SharedBundleToMapReferences[bundleName]
                                    .Bundles.Add(
                                        new MapBundle
                                        {
                                            Map = holders.MapName,
                                            Bundle = holders.BundleName
                                        });

                                if (assetBundleBuild.assetNames == null)
                                    assetBundleBuild.assetNames = new List<string> {holders.Asset};

                                if (assetBundleBuild.addressableNames == null && !string.IsNullOrWhiteSpace(holders.LoadName))
                                    assetBundleBuild.addressableNames = new List<string> {holders.LoadName};

                                return assetBundleBuild;
                            });
                        _collectorContext.SharedBundleToMapReferences[bundleName].AssetBundleBuild = bundleBuild.GetBundle();
                        return bundleBuild;
                    })
                .Select(b => b.GetBundle())
                .ToArray();

            //Перераспределим бандлы так чтобы найти расшаренные для конкретной карты
            _collectorContext.SharedBundleToMapReferences.Values.ForEach(
                assetBundleReference =>
                {
                    assetBundleReference.Bundles.ForEach(
                        bundle =>
                        {
                            if (!_collectorContext.SharedMapToBundleReferences.ContainsKey(bundle.Map))
                                _collectorContext.SharedMapToBundleReferences.Add(bundle.Map, new MapBundleReference {References = new Dictionary<string, List<AssetBundleReference>>()});

                            var mapBundleReference = _collectorContext.SharedMapToBundleReferences[bundle.Map];
                            if (!mapBundleReference.References.ContainsKey(bundle.Bundle))
                                mapBundleReference.References.Add(bundle.Bundle, new List<AssetBundleReference> {assetBundleReference});
                            else
                                mapBundleReference.References[bundle.Bundle].Add(assetBundleReference);
                        });
                });


            _collectorContext.ProjectDependencies.Add(AssetBundleHelper.SharedName, allShared.Concat(new[] {shaderBundle}).ToArray());

            _profileContext.Profiler.SaveProfileInfo("all_dependencies", _collectorContext.SharedMapToBundleReferences);

            return ReturnCode.Success;
        }
    }
}