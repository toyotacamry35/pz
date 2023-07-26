using System;
using System.Collections.Concurrent;
using Core.Environment.Logging.Extension;
using NLog;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public static class ServerRepoReadyNotifier
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static event Action<bool, Guid> ServerRepoIsReadyChangedEvent;

        public static bool IsServerRepoReady(Guid repoId) => ReadyRepos.ContainsKey(repoId);
        public static void Reset() => ReadyRepos.Clear();

        private static readonly ConcurrentDictionary<Guid, bool> ReadyRepos = new ConcurrentDictionary<Guid, bool>(); ///#PZ-9450: #Dbg: ?toVitaly: no need concurrent?

        public static void ServerRepoIsReadyChanged(bool newStatus, Guid repoId)
        {
            if (newStatus)
                if (!ReadyRepos.TryAdd(repoId, true))
                    Logger.IfError()?.Message($"{nameof(ReadyRepos)} already contains id: {repoId}").Write();
            else
                if (!ReadyRepos.TryRemove(repoId, out bool noneed))
                    Logger.IfError()?.Message($"{nameof(ReadyRepos)} contains not id: {repoId}").Write();

            ServerRepoIsReadyChangedEvent?.Invoke(newStatus, repoId);
        }
    }
}
