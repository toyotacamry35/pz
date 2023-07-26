using System;
using NLog;
using NLog.Targets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LoggerExtensions
{
    [Target("UnityConsole")]
    public sealed class UnityConsoleTarget : TargetWithLayout
    {
        public UnityConsoleTarget()
        {
            Layout = "${message}";
        }
        
        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Properties.ContainsKey("FromUnity"))
                return;
            LogType type = LogTypeConverter.ToUnity(logEvent.Level);
            object obj;
            logEvent.Properties.TryGetValue("UnityObject", out obj);
            Object uObj = obj as Object;
            string msg = Layout.Render(logEvent);
            msg = msg.Replace("{", "{{");
            msg = msg.Replace("}", "}}");
            if (logEvent.Exception != null)
            {
                LogException(new MESSAGE( msg, logEvent.Exception), uObj);
            }
            else
            {
                LogSystemInit.DefaultLogHandler.LogFormat(type, uObj, msg);
            }
        }

        private void LogException(Exception exception, Object uObj)
        {
            if (exception is AggregateException aggregate)
            {
                foreach (var inner in aggregate.InnerExceptions)
                    LogException(inner, uObj);
            }
            else
                LogSystemInit.DefaultLogHandler.LogException(exception, uObj);
        }
        
        private class MESSAGE : Exception {
            public MESSAGE(string msg, Exception exception) : base(msg, exception) {}
        }
    }
}