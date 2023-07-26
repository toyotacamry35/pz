using System;
using System.Linq;
using NLog;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class SharedDependencyCollectTask : IBuildTask
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
                var dependentBundles = _collectorContext.SharedBundleToMapReferences.Values
                    .Select(x => x.AssetBundleBuild).ToArray();

                _profileContext.Profiler.SaveProfileInfo("bundling_dependencies", dependentBundles);

                _bundleBuildContent = new BundleBuildContent(dependentBundles);
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