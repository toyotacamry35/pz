using Core.Environment.Logging;
using LoggerExtensions;
using NLog;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Debugging.Log.Editor
{
    public class EditorLogInitializer : AssetPostprocessor
    {
        private const string ConfigPath = "System/NLog.config-editor.xml";
        private const string HeadlessLogPath = "System/NLog.config-tc.xml";

        private const string EditorConfig = "Assets/" + ConfigPath; 
        
        private static bool IsHeadless => Environment.CommandLine.Contains("-batchmode");

        private static RestServer _restServer { get; set; }

        [InitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                InitEditorMode();
        }

        private static void InitEditorMode()
        {
            var cfg = IsHeadless ? HeadlessLogPath : ConfigPath;
            LogSystemInit.Init();
            var file = new FileInfo(Path.Combine(Application.dataPath, cfg));
            LogInitializer.InitLogger(file, new DirectoryInfo(Path.Combine(Application.dataPath, "..", "EditorLogs")));
            if(_restServer != null)
            {
                Debug.LogError("Rest server is not null, reiniting");
                _restServer.Dispose();
            }
            _restServer = RestServer.Create(true);
        }

        private static void ShutdownEditorMode()
        {
            if(_restServer == null)
            {
                Debug.LogError("Rest server is null");
            }
            else
            {
                _restServer.Dispose();
                _restServer = null;
            }

            LogManager.Flush();
            LogManager.Shutdown();
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                InitEditorMode();

            if (state == PlayModeStateChange.ExitingEditMode)
                ShutdownEditorMode();
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var importedAsset in importedAssets)
            {
                if(importedAsset.Equals(EditorConfig, StringComparison.OrdinalIgnoreCase))
                    InitEditorMode();
            }
        }
    }
}
