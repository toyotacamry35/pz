using UnityEditor;
using Assets.ColonyShared.SharedCode.Utils;

namespace Assets.Src.Editor.SpawnSystem
{
    [InitializeOnLoad]
    static class ServerRepoReadyNotifierResetter
    {
        static ServerRepoReadyNotifierResetter()
        {
            UnityEngine.Debug.Log("[InitializeOnLoad] ServerRepoReadyNotifierResetter.ctor");
            ServerRepoReadyNotifier.Reset();
        }
    }
}
