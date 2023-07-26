using NLog;
using UnityEngine;
using NLog.Fluent;
using JetBrains.Annotations;

namespace LoggerExtensions
{
    public static class LogSystemInit
    {
        [NotNull]
        internal static readonly ILogHandler DefaultLogHandler = Debug.unityLogger.logHandler;

        public static void Init(bool ignoreUnityLogs = true)
        {
            Debug.unityLogger.logHandler = new NLogHandler();
            if(!ignoreUnityLogs)
                Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
        }
        private static readonly NLog.Logger ExLogger = LogManager.GetLogger("Unity");

        private static void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            if (stackTrace.Contains("UnityConsoleTarget"))
                return;

            ExLogger.Log(LogTypeConverter.FromUnity(type)).Property("FromUnity", true).Message("{0}: {1}", condition, stackTrace).Write();
        }
    }
}
