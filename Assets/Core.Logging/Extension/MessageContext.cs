using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NLog;

namespace Core.Environment.Logging.Extension
{
    public readonly struct MessageContext
    {
        private readonly ILogger _logger;
        private readonly LogEventInfo _eventInfo;

        public MessageContext(
            ILogger logger,
            LogLevel level,
            Exception exception,
            string format,
            Action<IDictionary<object, object>> propertiesAdder,
            params object[] args)
            : this(logger, level, exception, format, args, propertiesAdder)
        {
        }

        public MessageContext(
            ILogger logger,
            LogLevel level,
            Exception exception,
            string format,
            object[] args,
            Action<IDictionary<object, object>> propertiesAdder)
        {
            _logger = logger;
            _eventInfo = new LogEventInfo
            {
                LoggerName = logger.Name,
                Level = level,
                Exception = exception,
                Message = format,
                Parameters = args
            };
            
            propertiesAdder?.Invoke(_eventInfo.Properties);
        }

        public MessageContext(ILogger logger, LogEventInfo eventInfo)
        {
            _logger = logger;
            _eventInfo = eventInfo;
        }

        public MessageContext Property(object key, object value)
        {
            _eventInfo.Properties[key] = value;
            return this;
        }

        public MessageContext Exception(Exception e)
        {
            _eventInfo.Exception = e;
            return this;
        }

        public void Write([CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            _eventInfo.SetCallerInfo( null, callerMemberName, callerFilePath, callerLineNumber);
            _logger.Log(_eventInfo);
        }
    }
}