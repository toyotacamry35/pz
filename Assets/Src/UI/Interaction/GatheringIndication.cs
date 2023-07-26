using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using ProcessSourceNamespace;
using SharedCode.Entities;
using Uins.Sound;
using UnityEngine;

namespace Uins
{
    public class GatheringIndication
    {
        private IProcessSourceOps _processSourceOps;
        private GameObject _soundControlSenderGameObject;


        //=== Ctor ============================================================

        public GatheringIndication(IProcessSourceOps processSourceOps, GameObject soundControlSenderGameObject)
        {
            if (processSourceOps.AssertIfNull(nameof(processSourceOps)))
                return;

            _processSourceOps = processSourceOps;
            _soundControlSenderGameObject = soundControlSenderGameObject;
        }


        //=== Public ==========================================================

        public void SetPawn(GameObject ourPawnGameObject)
        {
            if (ourPawnGameObject == null)
                return;

            var resourceGatherer = ourPawnGameObject.Kind<ResourceGatherer>();
            if (resourceGatherer.AssertIfNull(nameof(resourceGatherer)))
                return;

            resourceGatherer.GatherAchievedEvent += OnGatherAchievedEvent;
            resourceGatherer.GatherFailEvent += OnGatherFailEvent;
        }


        //=== Private =========================================================

        private void OnGatherAchievedEvent(GameObject mineableGameObject, int barIndex, IList<ItemResourcePack> achievedItems)
        {
            UI.CallerLog(achievedItems.ItemsToString());
            var processSource =
                _processSourceOps.ProcessSources.Values.FirstOrDefault(source => source.Id.Type == ProcessSourceId.ProcessType.CommonInteraction);
            if (processSource != null)
            {
                processSource.SetItemsAchieved(achievedItems, _processSourceOps.GetInventoryCounts(achievedItems));
                _processSourceOps.RemoveProcessSource(processSource);
            }
            else
            {
                UI.Logger.Warn($"{nameof(OnGatherAchievedEvent)}({mineableGameObject}, " +
                               $"{nameof(achievedItems)}={achievedItems.ItemsToString()}) " +
                               "Not found processSource with type Gathering. It's JustItemsAchieved");

                _processSourceOps.SetJustItemsAchieved(achievedItems);
            }

            SoundControl.Instance.ItemPickupEvent?.Post(_soundControlSenderGameObject);
        }

        private void OnGatherFailEvent(GameObject interactionGameObject)
        {
            UI.CallerLog($"go={interactionGameObject?.name}");
            var processSource =
                _processSourceOps.ProcessSources.Values.FirstOrDefault(source => source.Id.Type == ProcessSourceId.ProcessType.CommonInteraction);
            if (processSource != null)
            {
                processSource.SetEnding(true);
                _processSourceOps.RemoveProcessSource(processSource);
            }
            else
            {
                UI.Logger.IfWarn()?.Message($"{nameof(OnGatherFailEvent)}({interactionGameObject?.name}) Not found processSource").Write();
            }
        }
    }
}