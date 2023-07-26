using System;
using System.Collections.Generic;
using System.Diagnostics;
using NLog;

namespace Core.Environment.Logging.Extension
{
    public static class LoggerExtensions
    {
        public static Level? IfTrace(this ILogger logger, Action<IDictionary<object, object>> propertiesApplier= null)
        {
            if (!logger.IsTraceEnabled)
                return null;

            return new Level(logger, LogLevel.Trace, propertiesApplier);
        }
        
        public static Level? IfDebug(this ILogger logger, Action<IDictionary<object, object>> propertiesApplier= null)
        {
            if (!logger.IsDebugEnabled)
                return null;

            return new Level(logger, LogLevel.Debug, propertiesApplier);
        }

        public static Level? IfInfo(this ILogger logger, Action<IDictionary<object, object>> propertiesApplier= null)
        {
            if (!logger.IsInfoEnabled)
                return null;

            return new Level(logger, LogLevel.Info, propertiesApplier);
        }
        
        public static Level? IfWarn(this ILogger logger, Action<IDictionary<object, object>> propertiesApplier= null)
        {
            if (!logger.IsWarnEnabled)
                return null;

            return new Level(logger, LogLevel.Warn, propertiesApplier);
        }
        
        public static Level? IfError(this ILogger logger, Action<IDictionary<object, object>> propertiesApplier= null)
        {
            if (!logger.IsErrorEnabled)
                return null;

            return new Level(logger, LogLevel.Error, propertiesApplier);
        }

        public static Level? If(this ILogger logger, LogLevel level, Action<IDictionary<object, object>> propertiesApplier= null)
        {
            if (!logger.IsEnabled(level))
                return null;

            return new Level(logger, level, propertiesApplier);
        }

        public static Level? IfFatal(this ILogger logger, Action<IDictionary<object, object>> propertiesApplier= null)
        {
            if (!logger.IsFatalEnabled)
                return null;

            return new Level(logger, LogLevel.Fatal, propertiesApplier);
        }
    }
}