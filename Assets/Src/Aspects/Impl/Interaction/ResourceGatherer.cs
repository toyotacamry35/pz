using System.Collections.Generic;
using System.Linq;
using Assets.Src.Inventory;
using Core.Environment.Logging.Extension;
using SharedCode.Entities;
using ResourcesSystem.Loader;
using NLog;
using UnityEngine;

namespace Assets.Src.Aspects.Impl
{
    public delegate void GatherAchievedDelegate(GameObject mineableGameObject, int barIndex, IList<ItemResourcePack> achievedResources);
    public delegate void GatherEndingDelegate(GameObject mineableGameObject);

    //The only user - PlayerPawn.prefab (& no more)
    class ResourceGatherer : /*ColonyNetworkBehaviour*/MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Visibility");

        /// <summary>
        /// Successful gather ending
        /// </summary>
        public event GatherAchievedDelegate GatherAchievedEvent;
        public event GatherEndingDelegate GatherFailEvent;

///        No more [Server] - see task #PZ-6145
        public void BeforeGrantItems(IList<ItemResourcePack> grantedItems)
        {
            if (grantedItems == null || grantedItems.Count == 0)
                return;

            Logger.IfWarn()?.Message("Не работает сейчас. У Паши есть task - пересадить эту логику с UNet'а.").Write();

            NOTRpcAnyMore_OnGathering(grantedItems);
        }

///        No more [ClientRpc] - see task #PZ-6145
        private void NOTRpcAnyMore_OnGathering(IList<ItemResourcePack> resultItems)
        {
///        No more `HasAuthority` - see task #PZ-6145
///            if (!HasAuthority)
///                return;

            if (resultItems.Count == 0)
                GatherFailEvent?.Invoke(gameObject);
            else
                GatherAchievedEvent?.Invoke(gameObject, 0, resultItems);
        }
    }
}