using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using UnityEditor;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Test.Src.Editor
{
    public static class TCIntegration
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool _jenkinsCache;

        private static void ApplyJenkinsSettings()
        {
            var settings = BuildPipelineManager.GetBuildPipelineSettings();
            _jenkinsCache = settings.RemoteDependencyCache;
            settings.RemoteDependencyCache = true;
        }

        private static void RevertJenkinsSettings()
        {
            var settings = BuildPipelineManager.GetBuildPipelineSettings();
            settings.RemoteDependencyCache = _jenkinsCache;
        }

        public static void BuildPlayer()
        {
            var result = BuildPlayerCommon.BuildPlayer(BuildResourcesInitializer.CommonConfig);
            EditorApplication.Exit(result);
        }

        public static void BuildReleasePlayer()
        {
            var result = BuildPlayerCommon.BuildPlayer(BuildResourcesInitializer.ReleaseConfig);
            EditorApplication.Exit(result);
        }

        public static void CleanBuild()
        {
            BuildPlayerCommon.CleanBuild();
            IEnumerable<string> args = Environment.GetCommandLineArgs();
            SetBuildPipeline(args);
            EditorApplication.Exit(0);
        }

        public static void BuildSystemBundles()
        {
            // ApplyJenkinsSettings();
            var result1 = BuildPlayerCommon.BuildShared(ignoreSettings: true);
            var result2 = BuildPlayerCommon.BuildSystem(ignoreSettings: true);
            //RevertJenkinsSettings();
            var combined = new[] {result1, result2};
            EditorApplication.Exit(combined.Max());
        }

        public static void BuildMaps()
        {
            IEnumerable<string> args = Environment.GetCommandLineArgs();
            var maps = GetMapsToBuild(args);

            // ApplyJenkinsSettings();
            var result = BuildPlayerCommon.BuildMaps(maps, ignoreSettings: true);
            // RevertJenkinsSettings();
            EditorApplication.Exit(result);
        }

        private static void SetBuildPipeline(IEnumerable<string> args)
        {
            var mode = args.Select(x => x.ToUpperInvariant()).Any(x => x == "-DebugPipeline");
            BuildPipelineDebugMode.SetMode(mode);
        }

        private static IEnumerable<string> GetMapsToBuild(IEnumerable<string> args)
        {
            using (var enumerator = args.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var arg = enumerator.Current;
                    if (arg.ToUpperInvariant() != "-BUILDCOMPONENTS")
                        continue;

                    if (enumerator.MoveNext())
                    {
                        var mapArg = enumerator.Current;
                        var maps = mapArg.Split(new[] {";", "\""}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var map in maps)
                        {
                            yield return map;
                        }
                    }
                    else
                        yield break;
                }
            }
        }

        //[UnityEditor.MenuItem("Build/Test/Multi-Map")]
        public static void TestMultiMap()
        {
            string[] args = {"-BUILDCOMPONENTS", "\"Savannah;Dropzone\"", "-BUILDCOMPONENTS", "Test_Obj", "-BUILDCOMPONENTS"};

            var maps = GetMapsToBuild(args);
            foreach (var map in maps)
                Debug.Log($"MAP: {map}");
        }
    }
}