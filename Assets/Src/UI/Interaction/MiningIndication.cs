using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Aspects.Impl.Interaction;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using GeneratedCode.Repositories;
using NLog;
using ProcessSourceNamespace;
using SharedCode.Entities;
using SharedCode.Entities.Mineable;
using SharedCode.Repositories;
using Uins.Sound;
using UnityEngine;

namespace Uins
{
    public class MiningIndication
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private IProcessSourceOps _processSourceOps;
        private GameObject _soundControlSenderGameObject;

        //=== Ctor ============================================================

        public MiningIndication(IProcessSourceOps processSourceOps, GameObject soundControlSenderGameObject)
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

            //#Clean: `ResourceMinerComponent` doesn't exist any more:
            // var resourceMiner = ourPawnGameObject.Kind<ResourceMinerComponent>();
            // if (resourceMiner.AssertIfNull(nameof(resourceMiner)))
            //     return;
            //          // resourceMiner.MineableChangedEvent   += OnMineableChanged;
            // resourceMiner.MineableAchievedEvent  += OnMineableAchieved;
            // resourceMiner.MineableCancelledEvent += OnMineableCancelled;
            // resourceMiner.MineableExhaustedEvent += OnMineableExhausted;
            // 
            // if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466 UI +++ {nameof(SetPawn)}. Subscribed to {nameof(ResourceMinerComponent)}(Id: {resourceMiner.EntityId}) + + + UI").Write();
        }


        //=== Private =========================================================

        private void OnMineableChanged(Guid mineableEntityId, int barIndex, float currentProgress, List<ProbabilisticItemPack> expectedItems)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   ##_3.4.2(V-Z)  UI.`{nameof(OnMineableChanged)}`({nameof(mineableEntityId)}: {mineableEntityId};   {nameof(barIndex)}: {barIndex};   {nameof(currentProgress)}: {currentProgress};   {nameof(expectedItems)}[N:{expectedItems?.Count}]: {expectedItems}).").Write();

            UI.CallerLog($"{mineableEntityId} {nameof(barIndex)}={barIndex}, {nameof(currentProgress)}={currentProgress}, " +
                         $"{nameof(expectedItems)}={expectedItems.ItemsToString()}");

            if (expectedItems == null || expectedItems.Count == 0)
                return;

            if (_processSourceOps.SelectedTarget == null)
                return;

            var processSourceId = new ProcessSourceId(mineableEntityId, ReplicaTypeRegistry.GetIdByType(typeof(IMineableEntity)), ProcessSourceId.ProcessType.Mining, (ulong) barIndex);
            ProcessSource processSource;
            if (_processSourceOps.ProcessSources.TryGetValue(processSourceId, out processSource))
                processSource.SetStateChanged(processSource.EndProgress, currentProgress, -1, expectedItems);
            else
                _processSourceOps.AddProcessSource(new ProcessSource(processSourceId, 0, currentProgress, -1, expectedItems.ToList()));
        }

        private void OnMineableAchieved(Guid mineableEntityId, int barIndex, IList<ItemResourcePack> achievedItems)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"TC-3466   ##_4.4(V-Z)  UI.`{nameof(OnMineableAchieved)}`({nameof(mineableEntityId)}: {mineableEntityId};  {nameof(barIndex)}: {barIndex};  {nameof(achievedItems)} [N:{achievedItems?.Count}]: {achievedItems}.").Write();

            UI.CallerLog($"{mineableEntityId} {nameof(barIndex)}={barIndex}, " +
                         $"{nameof(achievedItems)}={achievedItems.ItemsToString()}");

            if (achievedItems == null || achievedItems.Count == 0)
                return;

            var processSourceId = new ProcessSourceId(mineableEntityId, ReplicaTypeRegistry.GetIdByType(typeof(IMineableEntity)), ProcessSourceId.ProcessType.Mining, (ulong) barIndex);
            ProcessSource processSource;
            if (_processSourceOps.ProcessSources.TryGetValue(processSourceId, out processSource))
            {
                SoundControl.Instance.ItemPickupEvent?.Post(_soundControlSenderGameObject);

                processSource.SetItemsAchieved(achievedItems, _processSourceOps.GetInventoryCounts(achievedItems));

                _processSourceOps.RemoveProcessSource(processSource);
            }
            else
            {
                UI.Logger.Warn($"{nameof(OnMineableAchieved)}({mineableEntityId}, {nameof(barIndex)}={barIndex}, " +
                               $"{nameof(achievedItems)}={achievedItems.ItemsToString()}) Not found processSource");
            }
        }

        private void OnMineableCancelled(Guid mineableEntityId)
        {
            UI.CallerLog(mineableEntityId.ToString());
            var sourcesToCancel = _processSourceOps.ProcessSources.Values.Where(source => source.Id.EntityId == mineableEntityId && !source.IsEnded).ToList();
            foreach (var processSource in sourcesToCancel)
            {
                processSource.SetEnding(false);
                _processSourceOps.RemoveProcessSource(processSource);
            }
        }

        private void OnMineableExhausted(Guid mineableEntityId)
        {
            UI.CallerLog(mineableEntityId.ToString());
            var sourcesToCancel =
                _processSourceOps.ProcessSources.Values.Where(source => source.Id.EntityId == mineableEntityId && !source.IsEnded).ToList();
            foreach (var processSource in sourcesToCancel)
            {
                processSource.SetEnding(true);
                _processSourceOps.RemoveProcessSource(processSource);
            }
        }
    }
}