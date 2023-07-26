using System;
using System.Runtime.CompilerServices;
using Core.Environment.Logging;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using SharedCode.EntitySystem;
using ILogger = NLog.ILogger;

namespace ColonyShared.SharedCode.Utils
{
    public static class EntityIdNLogExtensions
    {
        [MessageTemplateFormatMethod("message")]
        public static MessageContext Message(this Level level, Guid entityId, string message, params object[] args)
        {
            return level.Message(
                new LogEventInfo
                {
                    LoggerName = level.Logger.Name,
                    Level = level.LogLevel,
                    Message = message,
                    Parameters = args,
                    Properties = {{EntityIdLayoutRenderer.EntityIdKey, entityId}}
                });
        }
        
        public static MessageContext Message(this Level level, Guid entityId, string message)
        {
            return level.Message(
                new LogEventInfo
                {
                    LoggerName = level.Logger.Name,
                    Level = level.LogLevel,
                    Message = message,
                    Properties = {{EntityIdLayoutRenderer.EntityIdKey, entityId}}
                });
        }

        public static void Write(this Level level, Guid entityId, string message, 
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            level.Message(entityId, message).Write(callerMemberName, callerFilePath, callerLineNumber);
        }
        
        public static MessageContext Entity(this MessageContext mc, Guid entityId)
        {
            return mc.Property(EntityIdLayoutRenderer.EntityIdKey, entityId);
        }
    }
}