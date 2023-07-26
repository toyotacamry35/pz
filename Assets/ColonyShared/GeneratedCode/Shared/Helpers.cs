using Assets.ColonyShared.SharedCode.Shared;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using System.Threading;

namespace Assets.ColonyShared.GeneratedCode.Shared
{
    public static class GlobalLoggers
    {
        private static int WatchID = 0;
        public static int GetNextWatchID()
        {
            return Interlocked.Increment(ref WatchID);
        }

        public static readonly NLog.Logger SubscribeLogger = NLog.LogManager.GetLogger("Subscribe");
        public static readonly NLog.Logger SpellSystemLogger = NLog.LogManager.GetLogger("SpellSystemLogger");        
    }
}
