using System;
using System.Collections.Generic;
using SharedCode.EntitySystem;

namespace SharedCode.Utils
{
    public static class ReplicationMaskUtils
    {
        public static IEnumerable<int> AllReplicationMasks;

        static ReplicationMaskUtils()
        {
            var allReplicationMasks = new List<int>();
            foreach (var value in Enum.GetValues(typeof(ReplicationLevel)))
            {
                var intVal = (int)(ReplicationLevel)value;
                if (intVal > 0)
                    allReplicationMasks.Add(intVal);
            }
            allReplicationMasks.Add(12);//From ClientBroadcast to Client Full
            AllReplicationMasks = allReplicationMasks;
        }

        public static bool IsReplicationLevelAdded(long oldReplicationMask, long newReplicationMask, ReplicationLevel checkLevel)
        {
            if ((oldReplicationMask & (long)checkLevel) == (long)checkLevel)
                return false;
            return (newReplicationMask & (long)checkLevel) == (long)checkLevel;
        }

        public static bool IsReplicationLevelRemoved(long oldReplicationMask, long newReplicationMask, ReplicationLevel checkLevel)
        {
            if ((oldReplicationMask & (long)checkLevel) != (long)checkLevel)
                return false;
            return (newReplicationMask & (long)checkLevel) != (long)checkLevel;
        }
        
        public static bool HasReplicationLevel(long replicationMask, ReplicationLevel checkLevel)
        {
            return (replicationMask & (long)checkLevel) == (long)checkLevel;
        }

    }
}
