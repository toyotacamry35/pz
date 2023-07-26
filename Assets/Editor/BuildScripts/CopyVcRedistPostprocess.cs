using System;
using System.IO;
using Core.Environment.Logging.Extension;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Editor.BuildScripts
{
    class CopyVcRedistPostprocess : IPostprocessBuildWithReport
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public int callbackOrder => 3;

        public void OnPostprocessBuild(BuildReport report)
        {
            if ((report.summary.options & UnityEditor.BuildOptions.Development) == 0)
                return;

            var path = Path.Combine(Application.dataPath, @"..\RedistDlls");
            var pathNormalized = new Uri(path).LocalPath;
            var targetPath = Path.GetDirectoryName(report.summary.outputPath);
             Logger.IfInfo()?.Message("Debug build detected, copying vc redist dlls from {0} to {1}",  pathNormalized, targetPath).Write();

            foreach (var file in Directory.EnumerateFiles(pathNormalized, "*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var name = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(targetPath, name), true);
                }
                catch(Exception e)
                {
                    Logger.IfError()?.Message(e, "Failed to copy {0} to {1}", file, targetPath).Write();
                }
            }
             Logger.IfInfo()?.Message("Files copied").Write();;

        }
    }
}
