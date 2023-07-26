using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Navigation;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.WorldSpace;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Repositories;
using SharedCode.Serializers;
using Uins;
using UnityEngine;

namespace Assets.Src.ContainerApis
{
    public delegate void PointMarkerAddedRemovedDelegate(Guid markerGuid, PointMarker pointMarker, bool isRemoved, bool firstTime);

    public class PointMarkersApi : EntityApi
    {
        private bool _isSubscribedOnMarkers;
        private event PointMarkerAddedRemovedDelegate PointMarkerAddedRemoved;
        private Dictionary<Guid, PointMarker> _pointMarkers = new Dictionary<Guid, PointMarker>();


        //=== Props ===========================================================

        protected override ReplicationLevel ReplicationLevel => ReplicationLevel.ClientFull;


        //=== Public ==========================================================

        public void SubscribeToPointMarkers(PointMarkerAddedRemovedDelegate onPointMarkerAddedRemoved)
        {
            if (onPointMarkerAddedRemoved.AssertIfNull(nameof(onPointMarkerAddedRemoved)))
                return;

            PointMarkerAddedRemoved += onPointMarkerAddedRemoved;

            if (_pointMarkers.Count == 0)
                return;

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                lock (_pointMarkers)
                {
                    foreach (var kvp in _pointMarkers)
                        onPointMarkerAddedRemoved.Invoke(kvp.Key, kvp.Value, false, false);
                }
            });
        }

        public void UnsubscribeFromPointMarkers(PointMarkerAddedRemovedDelegate onPointMarkerAddedRemoved)
        {
            if (onPointMarkerAddedRemoved.AssertIfNull(nameof(onPointMarkerAddedRemoved)))
                return;

            PointMarkerAddedRemoved -= onPointMarkerAddedRemoved;
        }

        //TODOM Объединить функционал этого статического и одноименного экземплярного методов, т.к. 2 точки входа IWorldCharacter.AddPointMarker
        //изрядно путают. Аналогично - объединить статический и экземплярный RemovePointMarker()
        public static void AddPointMarker(Guid pointMarkerGuid, NavIndicatorDef navIndicatorDef, Vector3 point, Guid playerGuid)
        {
            if (navIndicatorDef.AssertIfNull(nameof(navIndicatorDef)))
                return;

            var replicationTypeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacterClientFull));
            AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await NodeAccessor.Repository.Get(replicationTypeId, playerGuid))
                    {
                        if (wrapper != null && wrapper.Get<IEntity>(replicationTypeId, playerGuid) != null)
                        {
                            var worldCharacterClientFull = (IWorldCharacterClientFull)wrapper.Get<IEntity>(replicationTypeId, playerGuid);
                            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                                return;

                            await worldCharacterClientFull.AddPointMarker(
                                pointMarkerGuid,
                                new PointMarker()
                                {
                                    NavIndicatorDef = navIndicatorDef,
                                    Position = UnityWorldSpace.ToVector3(point)
                                });
                        }
                        else
                        {
                            UI.Logger.IfError()?.Message($"<{typeof(PointMarkersApi)}> {nameof(AddPointMarker)}() wrapper or entity is null").Write();
                        }
                    }
                });
        }

        public static void RemovePointMarker(Guid pointMarkerGuid, Guid playerGuid)
        {
            var replicationTypeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacterClientFull));
            AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await NodeAccessor.Repository.Get(replicationTypeId, playerGuid))
                    {
                        if (wrapper != null && wrapper.Get<IEntity>(replicationTypeId, playerGuid) != null)
                        {
                            var worldCharacterClientFull = (IWorldCharacterClientFull)wrapper.Get<IEntity>(replicationTypeId, playerGuid);
                            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                                return;

                            await worldCharacterClientFull.RemovePointMarker(pointMarkerGuid);
                        }
                        else
                        {
                            UI.Logger.IfError()?.Message($"<{typeof(PointMarkersApi)}> {nameof(RemovePointMarker)}() wrapper or entity is null").Write();
                        }
                    }
                });
        }

        public void AddPointMarker(Guid pointMarkerGuid, NavIndicatorDef navIndicatorDef, Vector3 point)
        {
            if (navIndicatorDef.AssertIfNull(nameof(navIndicatorDef)))
                return;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await NodeAccessor.Repository.Get(ReplicationTypeId, EntityGuid))
                {
                    if (wrapper != null && wrapper.Get<IEntity>(ReplicationTypeId, EntityGuid) != null)
                    {
                        var worldCharacterClientFull = (IWorldCharacterClientFull)wrapper.Get<IEntity>(ReplicationTypeId, EntityGuid);
                        if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                            return;

                        await worldCharacterClientFull.AddPointMarker(
                            pointMarkerGuid,
                            new PointMarker()
                            {
                                NavIndicatorDef = navIndicatorDef,
                                Position = UnityWorldSpace.ToVector3(point)
                            });
                    }
                    else
                    {
                        LogError(NullEntityErrMsg);
                    }
                }
            });
        }

        public void RemovePointMarker(Guid pointMarkerGuid)
        {
            if (Guid.Empty == pointMarkerGuid)
                return;

            AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await NodeAccessor.Repository.Get(ReplicationTypeId, EntityGuid))
                    {
                        if (wrapper != null && wrapper.Get<IEntity>(ReplicationTypeId, EntityGuid) != null)
                        {
                            var worldCharacterClientFull = (IWorldCharacterClientFull)wrapper.Get<IEntity>(ReplicationTypeId, EntityGuid);
                            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                                return;

                            await worldCharacterClientFull.RemovePointMarker(pointMarkerGuid);
                        }
                        else
                        {
                            LogError(NullEntityErrMsg);
                        }
                    }
                });
        }


        //=== Protected =======================================================

        protected override async Task OnWrapperReceivedAtStart(IEntity wrapper)
        {
            if (!await TryGetWorldCharacterClientFull(wrapper, OnGetWorldCharacterClientFullAtStart))
                UI. Logger.IfError()?.Message("Unable to subscribe to KnowledgeEngine").Write();;
        }

        protected override async Task OnWrapperReceivedAtEnd(IEntity wrapper)
        {
            if (!_isSubscribedOnMarkers)
                return;

            if (!await TryGetWorldCharacterClientFull(wrapper, OnGetWorldCharacterClientFullAtEnd))
                UI. Logger.IfError()?.Message("Unable to unsubscribe from KnowledgeEngine").Write();;
        }


        //=== Private =========================================================

        private async Task OnGetWorldCharacterClientFullAtStart(IWorldCharacterClientFull worldCharacterClientFull)
        {
            if (!worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)) &&
                !_isSubscribedOnMarkers)
            {
                var pointMarkers = worldCharacterClientFull.PointMarkers;
                pointMarkers.OnItemAddedOrUpdated += OnPointMarkersAddedOrUpdated;
                pointMarkers.OnItemRemoved += OnPointMarkersAddedOrRemoved;
                _isSubscribedOnMarkers = true;

                if (pointMarkers.Count == 0)
                    return;

                lock (_pointMarkers)
                {
                    foreach (var kvp in pointMarkers)
                        _pointMarkers.Add(kvp.Key, kvp.Value);
                }

                if (PointMarkerAddedRemoved == null)
                    return;

                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    lock (_pointMarkers)
                    {
                        foreach (var kvp in _pointMarkers)
                        {
                            PointMarkerAddedRemoved?.Invoke(kvp.Key, kvp.Value, false, true);
                        }
                    }
                });
            }
        }

        private Task OnGetWorldCharacterClientFullAtEnd(IWorldCharacterClientFull worldCharacterClientFull)
        {
            if (!worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)) &&
                _isSubscribedOnMarkers)
            {
                worldCharacterClientFull.PointMarkers.OnItemAddedOrUpdated -= OnPointMarkersAddedOrUpdated;
                worldCharacterClientFull.PointMarkers.OnItemRemoved -= OnPointMarkersAddedOrRemoved;
                _isSubscribedOnMarkers = false;
            }

            return Task.CompletedTask;
        }

        private async Task<bool> TryGetWorldCharacterClientFull(IEntity chracterWrapper, Func<IWorldCharacterClientFull, Task> onSuccess)
        {
            if (chracterWrapper.AssertIfNull(nameof(chracterWrapper)) ||
                onSuccess.AssertIfNull(nameof(onSuccess)))
                return false;

            var worldCharacterClientFull = chracterWrapper as IWorldCharacterClientFull;
            if (worldCharacterClientFull.AssertIfNull(nameof(worldCharacterClientFull)))
                return false;

            await onSuccess.Invoke(worldCharacterClientFull);
            return true;
        }

        private async Task OnPointMarkersAddedOrUpdated(DeltaDictionaryChangedEventArgs<Guid, PointMarker> kvp)
        {
            var key = kvp.Key;
            var val = kvp.Value;
            lock (_pointMarkers)
            {
                _pointMarkers[key] = val;
            }

            UnityQueueHelper.RunInUnityThreadNoWait(() => PointMarkerAddedRemoved?.Invoke(key, val, false, false));
        }

        private async Task OnPointMarkersAddedOrRemoved(DeltaDictionaryChangedEventArgs<Guid, PointMarker> kvp)
        {
            var key = kvp.Key;
            var val = kvp.OldValue;
            lock (_pointMarkers)
            {
                PointMarker pointMarker;
                if (!_pointMarkers.TryGetValue(key, out pointMarker))
                {
                    UI.Logger.IfError()?.Message($"Unable remove point marker guid={key}, marker={val}").Write();
                    return;
                }

                _pointMarkers.Remove(key);
            }

            UnityQueueHelper.RunInUnityThreadNoWait(() => PointMarkerAddedRemoved?.Invoke(key, val, true, false));
        }
    }
}