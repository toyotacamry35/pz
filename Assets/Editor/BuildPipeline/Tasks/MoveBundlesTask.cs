using System.IO;
using Assets.Src.App;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class MoveBundlesTask : IBuildTask
    {
        [InjectContext(ContextUsage.InOut)]
        private IPipelineConfigurationContext _pipelineConfigurationContext;

        [InjectContext(ContextUsage.InOut)]
        private ICollectorContext _collectorContext;

        public int Version => 1;

        public ReturnCode Run()
        {
            if (Directory.Exists(_pipelineConfigurationContext.TargetPath))
                Directory.Delete(_pipelineConfigurationContext.TargetPath, true);
            
            Directory.CreateDirectory(_pipelineConfigurationContext.TargetPath);
            
            if (!Directory.Exists(_pipelineConfigurationContext.SharedPath))
                Directory.CreateDirectory(_pipelineConfigurationContext.SharedPath);

            var files = Directory.GetFiles(_pipelineConfigurationContext.TempPath, "*.bundle", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var filePath = PathExtension.GetRelativePath(file,_pipelineConfigurationContext.TempPath);
                if (filePath == null) continue;

                var movePath = _collectorContext.SharedBundleToMapReferences.ContainsKey(filePath) || filePath.Contains(AssetBundleHelper.ProduceSharedBuiltinShaderBundleName())?
                    Path.Combine(_pipelineConfigurationContext.SharedPath, filePath) : 
                    Path.Combine(_pipelineConfigurationContext.TargetPath, filePath);

                if (filePath.Contains(Path.DirectorySeparatorChar.ToString()) || filePath.Contains(Path.AltDirectorySeparatorChar.ToString()))
                {
                    var directoryName = PathExtension.GetDirectoryPath(movePath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                }

                if(File.Exists(movePath))
                    File.Delete(movePath);
                
                File.Move(file, movePath);
            }

            return ReturnCode.Success;
        }
    }
}