using System.IO;
using Assets.Src.BuildScripts;
using ResourcesSystem.Loader;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using Core.Environment.Logging;
using LoggerExtensions;

namespace Uins
{
    public class PreviewAtlasResourcesLoader : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private BuildConfig BuildConfig;

        [SerializeField, UsedImplicitly]
        private TechnoAtlasContr _technoAtlasContr;

        public string LogConfigPathPlayMode = "System/NLog.config-play.xml";

        public GameResources GameResources { get; private set; }

        private void Awake()
        {
            InitLogger();
            if (Directory.Exists(BuildConfig.LocalAssetsGameResourcesFolderPath))
                GameResources = new GameResources(new FolderLoader(BuildConfig.LocalAssetsGameResourcesFolderPath));
            else
                GameResources = new GameResources(new FolderLoader(BuildConfig.RuntimeGameResourcesFolderPath));

            _technoAtlasContr.SetCharacterStreams(null, null, null, null);
        }

        private void InitLogger()
        {
            LogSystemInit.Init();
            var file = new FileInfo(Path.Combine(new DirectoryInfo(BuildConfig.RuntimeGameResourcesFolderPath).FullName, LogConfigPathPlayMode));
            LogInitializer.InitLogger(file);
        }

        private void OnDestroy()
        {
            LogManager.Flush();
            LogManager.Shutdown();
        }
    }
}