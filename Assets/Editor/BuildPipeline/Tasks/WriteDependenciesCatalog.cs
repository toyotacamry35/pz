using System.Collections.Generic;
using System.Linq;
using Assets.Src.App;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class WriteDependenciesCatalog : IBuildTask
    { 
        [InjectContext(ContextUsage.In)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;
        
        [InjectContext(ContextUsage.In)]
        private ICollectorContext _collectorContext;
        
        public int Version => 1;

        public ReturnCode Run()
        {
            var preparingCatalog = new Dictionary<string, string[]>();
            var catalog = new DependenciesCatalog();
            foreach (var dependency in _collectorContext.ProjectDependencies)
            {
                var map = dependency.Key;
                var mapPath = AssetBundleHelper.ProduceBundleFolderName(map);
                foreach (var bundleBuild in dependency.Value)
                {
                    var relativeBundlePath = $"{mapPath}/{bundleBuild.assetBundleName}";
                    
                    var list = new List<string>();
                    for (var index = 0; index < bundleBuild.assetNames.Length; index++)
                    {
                        if (bundleBuild.addressableNames != null && bundleBuild.addressableNames.Length == bundleBuild.assetNames.Length &&
                            !string.IsNullOrWhiteSpace(bundleBuild.addressableNames[index]))
                        {
                            list.Add(bundleBuild.addressableNames[index]);
                        }
                        else
                        {
                            list.Add(bundleBuild.assetNames[index]);
                        }
                    }
                    preparingCatalog.Add(relativeBundlePath, list.ToArray());
                }
            }

            catalog.FillCatalog(new DependenciesCatalog.DependencyCatalog{Catalog = preparingCatalog, MapNames = _collectorContext.ProjectDependencies.Keys.ToArray()});
            catalog.SaveCatalog(_pipelineConfigurationContext.Config.BundlesFolderPath);
            
            return ReturnCode.Success;
        }
    }
}