using System;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.SpatialSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using L10n;
using ReactivePropsNs;
using DisposableComposite = ReactivePropsNs.DisposableComposite;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using UnityAsyncAwaitUtil;
using UnityEngine;
using Logger = NLog.Logger;

namespace Uins
{
    public class RegionVisitNotifier
    {
        // private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly RegionGroupVisitVM _regionGroupVM;

        private DisposableComposite D { get; } = new DisposableComposite();


        public RegionVisitNotifier(RegionGroupVisitVM regionGroupVM)
        {
            _regionGroupVM = regionGroupVM;

            NotifyAll();
            _regionGroupVM.OnEntered += NotifyAll;
        }

        private void NotifyAll()
        {
            while (_regionGroupVM.EnteredCount > 0)
                NotifyEnter(_regionGroupVM.TakeEntered());
        }

        private static async void NotifyEnter((IndexedRegionGroup group, int percent, bool visited) status)
        {
            var (group, percent, visited) = status;
            var groupDef = group.IndexedRegionGroupDef;

            if (!visited)
                await AsyncUtils.RunAsyncTask(() => SetVisited(groupDef));
            SendNotification(groupDef, visited, percent);
        }

        private static async Task SetVisited(IndexedRegionGroupDef indexedRegionGroupDef)
        {
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            var repo = GameState.Instance.ClientClusterNode;

            using (var wrapper = await repo.Get<IWorldCharacterClientFull>(characterId))
            {
                var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                await entity.FogOfWar.SetGroupVisited(indexedRegionGroupDef);
            }
        }

        private static void SendNotification(IndexedRegionGroupDef indexedRegionGroupDef, bool currentVisited, int percent)
        {
            var regionName = indexedRegionGroupDef.TitleJdbRef.Target.LocalizedStrings[indexedRegionGroupDef.TitleJdbKey];
            var notificationInfo = new NewRegionNotificationInfo(regionName, !currentVisited, percent);
            CenterNotificationQueue.Instance.SendNotification(notificationInfo);
        }
    }
}