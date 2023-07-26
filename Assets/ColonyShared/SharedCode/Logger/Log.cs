using NLog;

namespace SharedCode.Logging
{
    public static class Log
    {
        public static readonly NLog.Logger Logger = LogManager.GetLogger("Common");
        public static StopwatchLog StartupStopwatch => _StartupStopwatch;
        
        private static readonly StopwatchLog _StartupStopwatch = StopwatchLog.CreateNew("Startup", LogManager.GetLogger("StartupStopwatch"));

        public static StopwatchLog AttackStopwatch => _AttackStopwatch;
        
        private static readonly StopwatchLog _AttackStopwatch = StopwatchLog.CreateNew("Attack", LogManager.GetLogger("AttackStopwatch"));
    }
}