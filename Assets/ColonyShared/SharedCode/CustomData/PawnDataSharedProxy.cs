using System;
using System.Collections.Concurrent;
using NLog;
using SharedCode.Utils;
 
namespace Assets.ColonyShared.SharedCode.CustomData
{
    public static class PawnDataSharedProxy
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly ConcurrentStack<DbgBrokenLocomotionData> Dbg_BrokenLocomotions = new ConcurrentStack<DbgBrokenLocomotionData>();

        public static void Print()
        {
            Logger.Warn($"#Dbg (not Warn): PrintBrokenLocomotions(S) [{ Dbg_BrokenLocomotions.Count}]:\n" 
                        + string.Join(".\n", Dbg_BrokenLocomotions));
        }

        public static void Clean()
        {
            Dbg_BrokenLocomotions.Clear();
        }
    }

    public struct DbgBrokenLocomotionData
    {
        public Guid EntityId;
        public string GoName;
        public DateTime Timestamp;
        public long LocoDbgUpdateCounter;
        public Vector3 Pos;
        public string ExceptionMsg;

        public DbgBrokenLocomotionData(Guid entityId, string goName, DateTime timestamp, long locoDbgUpdateCounter, Vector3 pos, string exceptionMsg)
        {
            EntityId = entityId;
            GoName = goName;
            Timestamp = timestamp;
            LocoDbgUpdateCounter = locoDbgUpdateCounter;
            Pos = pos;
            ExceptionMsg = exceptionMsg;
        }

        public override string ToString()
            => $"Guid: {EntityId}, goName: {GoName}, timestamp: {Timestamp}, locoDbgUpdateCounter: {LocoDbgUpdateCounter}, pos: {Pos}, exceptionMsg: \"{ExceptionMsg}\"";
    }

}
