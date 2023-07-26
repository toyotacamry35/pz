using System.IO;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class CleanBuildCache : IBuildTask
    {
        public int Version => 1;

        public string ShaderBundleName { get; set; }

        public CleanBuildCache(string bundleName)
        {
            ShaderBundleName = bundleName;
        }

        public ReturnCode Run()
        {
            var buildCache = $"{UnityEditorUtils.LibraryPath()}/BuildCache";
            
            if(Directory.Exists(buildCache))
                Directory.Delete(buildCache);
            
            return ReturnCode.Success;
        }
    }
}