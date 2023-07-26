using Assets.Src.App;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class CollectAllDependencyTask : IBuildTask
    {
        [InjectContext(ContextUsage.In)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;

        [InjectContext(ContextUsage.InOut)]
        private ICollectorContext _collectorContext;

        [InjectContext(ContextUsage.In)]
        private IBuildPipelineSettings _buildPipelineSettings;

        public int Version => 1;

        public ReturnCode Run()
        {
            //Collect deps for system
            var systemCollector = new SystemCollector();
            var systemBundles = systemCollector.Collect(AssetBundleHelper.SystemName, _pipelineConfigurationContext.Config);

            _collectorContext.ProjectDependencies.Add(AssetBundleHelper.SystemName, systemBundles);

            //Collect deps for maps
            var mapCollector = new MapCollector();
            foreach (var name in _buildPipelineSettings.ReleaseMapNames)
            {
                var mapBundles = mapCollector.Collect(name, _pipelineConfigurationContext.Config);

                _collectorContext.ProjectDependencies.Add(name, mapBundles);
            }

            return ReturnCode.Success;
        }
    }
}