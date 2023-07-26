using NLog.Fluent;
using System.Linq;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using UnityEditor;

namespace Assets.Src.Debugging.Log.Editor
{
    public static class LogFiller
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //[MenuItem("Build/Add Logs")]
        public static void FillLogs()
        {
            foreach(var i in Enumerable.Range(0, 1000))
            {
                Logger.IfInfo()?.Message("Log Message {0}", i).Write();
            }
        }
    }
}
