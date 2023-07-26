using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Src.BuildScripts;
using Core.Environment.Logging.Extension;
using NLog;
using NLog.Fluent;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Test.Src.Editor
{
    public class PlayerBuilder : IBuilder
    {
        public event Action BeforeBuild;
        public event Action AfterBuild;
        
        private readonly Logger _logger = LogManager.GetLogger("BuildProcess");

        private readonly string _buildingLock;
        private readonly BuildConfig _config;


        public PlayerBuilder(string buildingLock, BuildConfig config)
        {
            _buildingLock = buildingLock;
            _config = config;
        }

        protected void AddBuildingFile()
        {
            File.WriteAllText($"{UnityEditorUtils.LibraryPath()}/{_buildingLock}", "");
        }

        protected void DeleteBuildingFile()
        {
            File.Delete($"{UnityEditorUtils.LibraryPath()}/{_buildingLock}");
        }

        protected string ApplyDefines(string defines, BuildConfig cfg)
        {
            if (cfg.RemoveDefines != null)
                foreach (var define in cfg.RemoveDefines)
                    defines = defines.Replace(define, string.Empty).Replace(";;", ";");
            if (cfg.AddDefines != null)
                foreach (var define in cfg.AddDefines)
                    defines = defines + ";" + define;
            defines = defines.Trim(';');
            return defines;
        }

        public int Build()
        {
            AddBuildingFile();

            _logger.Info("Started building player with config {0}", _config.name);

            var opts = new BuildPlayerOptions();
            opts.scenes = new[] {"Assets/Scenes/Bootstrap.unity"};
            //opts.assetBundleManifestPath = Path.Combine(bundleDir, cfg.BundlesFolderName + ".manifest");
            opts.options = BuildOptions.UncompressedAssetBundle;
            if (_config.Development)
                opts.options |= BuildOptions.Development;
            if (_config.AllowDebugging)
                opts.options |= BuildOptions.AllowDebugging;
            if (_config.ScriptOnly)
                opts.options |= BuildOptions.BuildScriptsOnly;

            var targetExe = Path.Combine(_config.BuildLocationNormalized, _config.ExeName);

            opts.locationPathName = targetExe;
            opts.target = BuildTarget.StandaloneWindows64;
            opts.targetGroup = BuildTargetGroup.Standalone;

            if (EditorUtility.DisplayCancelableProgressBar("Building", "Building", 0.5f))
            {
                EditorUtility.ClearProgressBar();
                _logger.Error("Build process was cancelled");
                return 1;
            }

            string oldDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(opts.targetGroup);
            try
            {
                var defines = ApplyDefines(oldDefines, _config);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(opts.targetGroup, defines);
                _logger.Info("{@BuildOpts}", opts);
                _logger.Info("{Defines}", defines);

                var msg = BuildPipeline.BuildPlayer(opts);

                if (msg.summary.result != BuildResult.Succeeded)
                {
                    _logger.Error().Message("Build Failed with status {0}", msg.summary.result).Write();
                    DeleteBuildingFile();
                    return 1;
                }
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                _logger.Error().Exception(e).Message("Build process failed with exception").Write();
                DeleteBuildingFile();
                return 1;
            }
            finally
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(opts.targetGroup, oldDefines);
            }

            _logger.IfInfo()?.Message("Build process finished successfully").Write();

            EditorUtility.ClearProgressBar();
            DeleteBuildingFile();
            return 0;
        }
    }
}