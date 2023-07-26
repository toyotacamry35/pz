using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.SpatialSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using UnityEngine;
using System.Threading.Tasks;
using SharedCode.Aspects.Regions;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace Uins
{
    public class RegionGroupVisitVM : BindingVmodel
    {
        // private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly DictionaryStream<IndexedRegionGroupDef, (int percent, bool visited)> _currentGroupsStatus;

        private readonly Queue<(IndexedRegionGroup indexedRegionGroup, int percent, bool visited)> _enteredGroups;

        public event Action OnEntered;

        public int EnteredCount => _enteredGroups.Count;

        public RegionGroupVisitVM(
            IReactiveProperty<ImmutableDictionary<ARegionDef, IRegion>> defToRegionMap,
            IHashSetStream<IndexedRegionDef> currentRegionDefs,
            IHashSetStream<IndexedRegionGroupDef> currentRegionGroupDefs)
        {
            _enteredGroups = new Queue<(IndexedRegionGroup indexedRegionGroup, int percent, bool visited)>();
            _currentGroupsStatus = new DictionaryStream<IndexedRegionGroupDef, (int percent, bool visited)>();

            var currentD = D.CreateInnerD();
            defToRegionMap.Action(
                D,
                defToRegion =>
                {
                    D.DisposeInnerD(currentD);
                    currentD = D.CreateInnerD();
                    _currentGroupsStatus.Clear();

                    if (defToRegion != null)
                    {
                        currentRegionGroupDefs.AddStream.Action(
                            currentD,
                            indexedRegionGroupDef =>
                            {
                                _currentGroupsStatus.Add(indexedRegionGroupDef, default);

                                if (defToRegion.TryGetValue(indexedRegionGroupDef, out var region) &&
                                    region is IndexedRegionGroup indexedRegionGroup
                                )
                                    AsyncUtils.RunAsyncTask(() => AssembleEntered(indexedRegionGroup));
                            }
                        );
                        currentRegionGroupDefs.RemoveStream.Action(
                            currentD,
                            indexedRegionGroupDef => { _currentGroupsStatus.Remove(indexedRegionGroupDef); }
                        );

                        currentRegionDefs.AddStream.Action(
                            currentD,
                            indexedRegionDef =>
                            {
                                if (defToRegion.TryGetValue(indexedRegionDef, out var region) &&
                                    region is IndexedRegion indexedRegion
                                )
                                    AsyncUtils.RunAsyncTask(() => AssembleProgress(indexedRegion));
                            }
                        );
                        currentRegionDefs.RemoveStream.Action(
                            currentD,
                            indexedRegionDef =>
                            {
                                if (defToRegion.TryGetValue(indexedRegionDef, out var region) &&
                                    region is IndexedRegion indexedRegion
                                )
                                    AsyncUtils.RunAsyncTask(() => AssembleProgress(indexedRegion));
                            }
                        );
                    }
                }
            );
        }

        public (IndexedRegionGroup indexedRegionGroup, int percent, bool visited) TakeEntered()
        {
            return _enteredGroups.Count > 0 ? _enteredGroups.Dequeue() : default;
        }

        private void AddEntered(IndexedRegionGroup indexedRegionGroup, int percent, bool visited)
        {
            if (_currentGroupsStatus.TryGetValue(indexedRegionGroup.IndexedRegionGroupDef, out (int percent, bool visited) old) &&
                old.percent == percent
            ) return;
            _currentGroupsStatus[indexedRegionGroup.IndexedRegionGroupDef] = (percent, visited);
            _enteredGroups.Enqueue((indexedRegionGroup, percent, visited));
            OnEntered?.Invoke();
        }

        private async Task AssembleEntered(IndexedRegionGroup indexedRegionGroup)
        {
            if (indexedRegionGroup != null)
            {
                var (percent, visited) = await GetIndexedGroupStatus(indexedRegionGroup);

                await UnityQueueHelper.RunInUnityThread(
                    () => { AddEntered(indexedRegionGroup, percent, visited); }
                );
            }
        }

        private async Task AssembleProgress(IndexedRegion indexedRegion)
        {
            var indexedRegionGroup = indexedRegion?.ParentRegionGroup;
            await AssembleEntered(indexedRegionGroup);
        }

        private static async Task<(int percent, bool visited)> GetIndexedGroupStatus(IndexedRegionGroup regionGroup)
        {
            if (regionGroup != null)
            {
                var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IWorldCharacterClientFull>(characterId))
                {
                    var entity = wrapper.Get<IWorldCharacterClientFull>(characterId);
                    if (entity != null)
                    {
                        var fogOfWar = entity.FogOfWar;
                        var discoveredRegions = fogOfWar.Regions;
                        var opened = 0;
                        var count = 0;
                        foreach (var indexedRegion in regionGroup.IndexedChildRegions)
                        {
                            count++;
                            if (discoveredRegions.ContainsKey(indexedRegion.IndexRegionDef))
                                opened++;
                        }

                        var percent = Mathf.RoundToInt(count > 0 ? 100f * opened / count : 100f);

                        if (fogOfWar.RegionGroups.TryGetValue(regionGroup.IndexedRegionGroupDef, out var visited))
                            return (percent, visited);
                    }
                }
            }

            return default;
        }
    }
}