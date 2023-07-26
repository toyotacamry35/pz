using System.IO;
using Assets.Src.App;
using NLog;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class TempSharedBundleCheckTask : IBuildTask
    {
        private Logger Logger = LogManager.GetCurrentClassLogger();
        
        [InjectContext(ContextUsage.InOut)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;
        
        public int Version => 1;

        public ReturnCode Run()
        {
            var bundlePath = $"{_pipelineConfigurationContext.TempPath}/{AssetBundleHelper.ProduceCustomShaderBundleName(AssetBundleHelper.SharedName)}";
            if (File.Exists(bundlePath))
                return ReturnCode.Success;
            
            Logger.Error($"Missed Custom Shared Shader Bundle.");
            return ReturnCode.MissingRequiredObjects;
        }
    }
}