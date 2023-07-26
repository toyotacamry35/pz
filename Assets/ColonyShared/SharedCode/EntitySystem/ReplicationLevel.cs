using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    [Flags]
    public enum ReplicationLevel: long
    {
        None = 0,
        [ReplicationLevelGenerateInterface]
        Always = 1,
        [ReplicationLevelGenerateInterface]
        ClientBroadcast = 2 | Always,
        [ReplicationLevelGenerateInterface]
        ClientFullApi = 4,
        [ReplicationLevelGenerateInterface]
        ClientFull = 8 | ClientBroadcast | ClientFullApi,
        [ReplicationLevelGenerateInterface]
        ServerApi = 16 | ClientFullApi,
        [ReplicationLevelGenerateInterface]
        Server = 32 | ClientFull | ServerApi,
        Master = long.MaxValue
    }
}
