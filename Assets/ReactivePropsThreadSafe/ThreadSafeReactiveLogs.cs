using NLog;

namespace ReactivePropsNs.ThreadSafe
{
    public class ReactiveLogs
    {
        public static readonly NLog.Logger Logger = LogManager.GetLogger("Reactive");
    }
}