using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class ReleaseDependencyCollectTask : IBuildTask
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
                var assets = _collectorContext.ProjectDependencies[_pipelineConfigurationContext.MapName];

                var dependentBundles = _collectorContext.SharedMapToBundleReferences[_pipelineConfigurationContext.MapName]
                    .References.Values
                    .SelectMany(x => x)
                    .Select(x => x.AssetBundleBuild);

                assets = assets.Concat(dependentBundles).ToArray();
                
                _profileContext.Profiler.SaveProfileInfo("bundling_dependencies",assets);
                
                _bundleBuildContent = new BundleBuildContent(assets);
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