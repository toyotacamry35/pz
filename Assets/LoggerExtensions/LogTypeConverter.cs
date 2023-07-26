using JetBrains.Annotations;
using NLog;
using UnityEngine;

namespace LoggerExtensions
{
    internal static class LogTypeConverter
    {
        public static LogType ToUnity([NotNull] LogLevel level)
        {
            if (level == LogLevel.Fatal)
                return LogType.Exception;

            if (level == LogLevel.Error)
                return LogType.Error;

            if (level == LogLevel.Warn)
                return LogType.Warning;
            return LogType.Log;
        }

        [NotNull]
        public static LogLevel FromUnity(LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                    return LogLevel.Fatal;
                case LogType.Assert:
                    return LogLevel.Fatal;
                case LogType.Error:
                    return LogLevel.Error;
                case LogType.Warning:
                    return LogLevel.Warn;
                default:
                    return LogLevel.Info;
            }
        }
    }
}
