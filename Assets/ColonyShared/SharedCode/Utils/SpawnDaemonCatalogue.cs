using System;
using System.Collections.Concurrent;

namespace Assets.ColonyShared.SharedCode.Utils
{
    // Is used for cheat `ResetSpawnDaemon`
    public static class SpawnDaemonCatalogue
    {
        public static ConcurrentDictionary<string, Guid> DaemonNameToGuidDic = new ConcurrentDictionary<string, Guid>();
    }
}
