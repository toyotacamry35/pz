using System.IO;
using Core.Environment.Logging;
using Core.Environment.Logging.Extension;
using NLog;

namespace LocalizationShared.Logger
{
    public static class LoggerInitialization
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Default");

        public static string InitLogger(string resourceRoot, string logConfigPath)
        {
            var rr = new DirectoryInfo(resourceRoot);
            if (!rr.Exists)
            {
                return $"Log config not found at '{logConfigPath}', logging isn't initialized";
            }
            var logCfgFile = new FileInfo(Path.Combine(rr.FullName, logConfigPath));
            if (!logCfgFile.Exists)
            {
                return $"Log config not found at '{logConfigPath}', logging isn't initialized";
            }
            LogInitializer.InitLogger(logCfgFile);

            Logger.IfDebug()?.Message($"Log system is initialized with config '{logConfigPath}'").Write();
            return null;
        }
    }
}