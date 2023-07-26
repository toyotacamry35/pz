using System;
using System.Diagnostics;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;

namespace SharedCode.Logging
{
    public class StopwatchLog : IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Stopwatch");

        private readonly NLog.Logger _logger;
        private readonly Stopwatch _stopwatch;
        private readonly string _name;
        private long _lastMilestone;

        private static readonly StopwatchLog Empty = new StopwatchLog(Logger, "", null);

        private StopwatchLog(NLog.Logger logger, string name, Stopwatch stopwatch)
        {
            _logger = logger;
            _stopwatch = stopwatch;
            _name = name;
        }

        public static StopwatchLog StartNew(string name) => StartNew(name, Logger);

        public static StopwatchLog StartNew(string name, NLog.Logger logger) => logger.IsDebugEnabled ? new StopwatchLog(logger, name, Stopwatch.StartNew()) : Empty;

        public static StopwatchLog CreateNew(string name) => CreateNew(name, Logger);

        public static StopwatchLog CreateNew(string name, NLog.Logger logger) => logger.IsDebugEnabled ? new StopwatchLog(logger, name, new Stopwatch()) : Empty;

        [Pure] public void Start()
        {
            _lastMilestone = 0;
            _stopwatch?.Start();
            Log("Start");
        }

        [Pure] public void Restart()
        {
            _lastMilestone = 0;
            _stopwatch?.Restart();
            Log("Start");
        }

        [Pure] public void Stop()
        {
            _stopwatch?.Stop();
        }

        [System.Diagnostics.Contracts.Pure]
        public void Log()
        {
            Log(null);
        }

        private void Log(string name)
        {
            if (_stopwatch != null)
            {
                var milestone = _stopwatch.ElapsedMilliseconds;
                _logger.IfDebug()?.Message($"{_name}{(name != null ? $" | {name}" : string.Empty)}: {milestone} (+{milestone - _lastMilestone})").Write();
                _lastMilestone = milestone;
            }
        }

        public void Milestone(string name) => Log(name);

        public void Dispose()
        {
            if (_stopwatch != null)
            {
                _stopwatch.Stop();
                Log();
            }
        }
    }
}