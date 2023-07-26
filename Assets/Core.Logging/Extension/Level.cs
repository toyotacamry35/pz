using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NLog;

namespace Core.Environment.Logging.Extension
{
    public readonly struct Level
    {
        private readonly Action<IDictionary<object, object>> _propertiesAdder;

        public Level(ILogger logger, LogLevel level, Action<IDictionary<object, object>> propertiesAdder)
        {
            _propertiesAdder = propertiesAdder;
            Logger = logger;
            LogLevel = level;
        }

        public LogLevel LogLevel { get; }

        public ILogger Logger { get; }

        public MessageContext Message(LogEventInfo eventInfo)
        {
            return new MessageContext(Logger, eventInfo);
        }

        public MessageContext Message(StringBuilder sb)
        {
            return Message(sb.ToString());
        }
        
        public MessageContext Message(string format)
        {
            return new MessageContext(Logger, LogLevel, null, format, _propertiesAdder);
        }

        public MessageContext Exception(Exception e)
        {
            return new MessageContext(Logger, LogLevel, e, null, _propertiesAdder);
        }
        
        [MessageTemplateFormatMethod("format")]
        public MessageContext Message(string format, params object[] args)
        {
            return new MessageContext(Logger, LogLevel, null, format, _propertiesAdder, args);
        }

        public MessageContext Message(Exception exception, string format)
        {
            return new MessageContext(Logger, LogLevel, exception, format, _propertiesAdder);
        }

        [MessageTemplateFormatMethod("format")]
        public MessageContext Message(Exception exception, string format, params object[] args)
        {
            return new MessageContext(Logger, LogLevel, exception, format, _propertiesAdder, args);
        }
        
        public void Write(string message, [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Message(message).Write(callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Write(string message, Func<MessageBuilder,MessageBuilder> mb, [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var mc = new MessageContext(Logger, LogLevel, null, message, _propertiesAdder);
            mb(new MessageBuilder(mc));
            mc.Write(callerMemberName, callerFilePath, callerLineNumber);
        }

        public void Write(Func<MessageBuilder, MessageBuilder> mb, [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Write(String.Empty, mb, callerMemberName, callerFilePath, callerLineNumber);
        }
    }
}