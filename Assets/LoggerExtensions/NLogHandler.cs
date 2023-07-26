using System;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LoggerExtensions
{
    public class NLogHandler : ILogHandler
    {
        [NotNull] private static readonly NLog.Logger UnityDefaultLogger = LogManager.GetLogger("Unity");

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            UnityDefaultLogger.If(LogTypeConverter.FromUnity(logType))?.Message(format, args).UnityObj(context).Write();
        }

        public void LogException(Exception exception, Object context)
        {
            UnityDefaultLogger.IfFatal()?.Message(exception, exception.Message + "\n" + exception.StackTrace).UnityObj(context).Write();
        }
    }
}
