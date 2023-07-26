using NLog;

namespace ReactivePropsNs
{
    public static class ReactiveLogs
    {
        public static readonly NLog.Logger Logger = LogManager.GetLogger("Default");
    }
}