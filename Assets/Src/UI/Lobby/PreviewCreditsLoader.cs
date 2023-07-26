using System.IO;
using Assets.Src.BuildScripts;
using Core.Environment.Logging;
using JetBrains.Annotations;
using LoggerExtensions;
using NLog;
using ResourcesSystem.Loader;
using UnityEngine;

namespace Uins
{
    public class PreviewCreditsLoader : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private BuildConfig BuildConfig;

        [SerializeField, UsedImplicitly]
        private CreditsContr _creditsContr;

        public string LogConfigPathPlayMode = "System/NLog.config-play.xml";

        public GameResources GameResources { get; private set; }

        private void Awake()
        {
            InitLogger();
            if (Directory.Exists(BuildConfig.LocalAssetsGameResourcesFolderPath))
                GameResources = new GameResources(new FolderLoader(BuildConfig.LocalAssetsGameResourcesFolderPath));
            else
                GameResources = new GameResources(new FolderLoader(BuildConfig.RuntimeGameResourcesFolderPath));

            _creditsContr.OnShowCredits();
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