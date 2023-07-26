using NLog;
using NLog.Config;
using NLog.Internal;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core.Environment.Logging
{
    public struct LogEntry
    {
        public int Index { get; set; }

        public DateTime DateTime { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public StackTrace StackTrace { get; set; }

    }

    [Target("SyncedMemory")]
    public sealed class SyncedMemoryTarget : Target, IUsesStackTrace
    {
        public SyncedMemoryTarget()
        {
        }

        public SyncedMemoryTarget(string name)
        {
            Name = name;
        }

        public Queue<LogEntry> Logs { get; } = new Queue<LogEntry>();

        public int MessageLimit { get; set; } = 10000;

        public StackTraceUsage StackTraceUsage => StackTraceUsage.Max;

        protected override void Write(LogEventInfo logEvent)
        {
            var newEntry = new LogEntry()
            {
                Index = logEvent.SequenceID,
                DateTime = logEvent.TimeStamp,
                Level = logEvent.Level.Name,
                Message = logEvent.FormattedMessage,
                StackTrace = logEvent.StackTrace
            };

            lock (Logs)
            {
                while (Logs.Count >= MessageLimit)
                    Logs.Dequeue();

                Logs.Enqueue(newEntry);
            }
        }
    }
}
