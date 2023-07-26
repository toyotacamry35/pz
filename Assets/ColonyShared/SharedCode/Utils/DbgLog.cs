using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;

namespace Assets.ColonyShared.SharedCode.Utils
{
    // Use only for debug logs, to be able change verbosity level in 1 place. When task is done, change all calls to `Logger.Debug(...)`
    public static class DbgLog
    {
        [NotNull] public static readonly Logger Logger = LogManager.GetLogger("DbgLog");

        public static bool Enabled => GlobalConstsDef.DebugFlagsGetter.IsDbgLogEnabled(GlobalConstsHolder.GlobalConstsDef);

        public static void Log(string s)
        {
            Log(-1, s);
        }

        public static void LogErr(string s)
        {
            Logger.IfError()?.Message(SharedHelpers.NowStamp + s).Write();
        }
        public static void Log(string s, params object[] args)
        {
            Log(-1, s, args);
        }

        public static void Log(int i, string s)
        {
            if (!Enabled)
                return;
            if (i != -10)
                /**/Logger.IfDebug()?.Message($"{SharedHelpers.NowStamp} " + s).Write();
                ///Logger.IfWarn()?.Message($"{SharedHelpers.NowStamp} " + s).Write();
        }

        public static void Log(int i, string s, params object[] args)
        {
            if (!Enabled)
                return;
            if (i != -10)
                /**/Logger.IfDebug()?.Message(SharedHelpers.NowStamp + s, args).Write();
                ///Logger.IfWarn()?.Message(SharedHelpers.NowStamp + s, args).Write();
        }
    }
}
