using System;
using System.Linq;
using Boo.Lang;
using NLog;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class TestDependencyCollectTask : IBuildTask
    {
        private readonly Logger _logger = LogManager.GetLogger("BuildProcess");

        public int Version => 1;

        [InjectContext(ContextUsage.In)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;

        [InjectContext(ContextUsage.In)]
        private ICollectorContext _collectorContext;

        [InjectContext(ContextUsage.InOut)]
        private IBuildContent _buildContent;

        [InjectContext(ContextUsage.InOut)]
        private IBundleBuildContent _bundleBuildContent;
        
        [InjectContext(ContextUsage.In)]
        IProfileContext _profileContext;

        public ReturnCode Run()
        {
            try
            {
                var mapCollector = new MapCollector();
                var mapBundles = mapCollector.Collect(_pipelineConfigurationContext.MapName, _pipelineConfigurationContext.Config);

                var sharedToBuild = new List<AssetBundleBuild>();
                var mapBundlesBuild = new List<AssetBundleBuild>();
                foreach (var bundleBuild in mapBundles)
                {
                    var tempBundleNames = new List<(string, string)>();
                    for (var i = 0; i < bundleBuild.assetNames.Length; i++)
                    {
                        var name = bundleBuild.assetNames[i];
                        var added = false;
                        foreach (var reference in _collectorContext.SharedBundleToMapReferences.Values.Where(reference => reference.AssetBundleBuild.assetNames.Contains(name)))
                        {
                            if (!sharedToBuild.Contains(reference.AssetBundleBuild))
                                sharedToBuild.Add(reference.AssetBundleBuild);
                            added = true;
                        }

                        if (!added)
                            tempBundleNames.Add(ValueTuple.Create(name, bundleBuild.addressableNames?[i]));
                    }

                    if (tempBundleNames.Count > 0)
                        mapBundlesBuild.Add(
                            new AssetBundleBuild
                            {
                                assetBundleName = bundleBuild.assetBundleName,
                                assetNames = tempBundleNames.Select(b => b.Item1).ToArray(),
                                addressableNames = tempBundleNames.Select(b => b.Item2).ToArray()
                            });
                }

                _collectorContext.ProjectDependencies.Add(_pipelineConfigurationContext.MapName, mapBundlesBuild.ToArray());

                var bundlesToBuild = mapBundlesBuild.Concat(sharedToBuild).ToArray();
                _profileContext.Profiler.SaveProfileInfo("bundling_dependencies", bundlesToBuild);
                
                _bundleBuildContent = new BundleBuildContent(bundlesToBuild);
                _buildContent = _bundleBuildContent;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return ReturnCode.Exception;
            }

            return ReturnCode.Success;
        }
    }
}