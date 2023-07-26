using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ContainerApis;
using Assets.Src.SpawnSystem;
using Assets.Src.WorldSpace;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    /// <summary>
    /// Выставление источников индикаторов по серверным событиям
    /// </summary>
    public class PointMarkers : HasDisposablesMonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private UserMarkerNotifier _userMarkerNotifierPrefab;

        private Dictionary<Guid, UserMarkerNotifier> _notifiers = new Dictionary<Guid, UserMarkerNotifier>();


        //=== Unity ===========================================================

        private void Awake()
        {
            _userMarkerNotifierPrefab.AssertIfNull(nameof(_userMarkerNotifierPrefab));
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        }


        //=== Private =========================================================

        private EntityApiWrapper<PointMarkersApi> _pointMarkersApiWrapper;

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                if (_notifiers.Count > 0)
                {
                    var notifiers = _notifiers.ToArray();
                    for (int i = 0; i < notifiers.Length; i++)
                        OnPointMarkerAddedOrRemoved(notifiers[i].Key, null, true, false);
                }

                _pointMarkersApiWrapper.EntityApi.UnsubscribeFromPointMarkers(OnPointMarkerAddedOrRemoved);
                _pointMarkersApiWrapper.Dispose();
                _pointMarkersApiWrapper = null;
            }

            if (newEgo != null)
            {
                _pointMarkersApiWrapper = EntityApi.GetWrapper<PointMarkersApi>(newEgo.OuterRef);
                _pointMarkersApiWrapper.EntityApi.SubscribeToPointMarkers(OnPointMarkerAddedOrRemoved);
            }
        }

        private void OnPointMarkerAddedOrRemoved(Guid markerGuid, PointMarker pointMarker, bool isRemoved, bool firstTime)
        {
            if (isRemoved)
            {
                UserMarkerNotifier userMarkerNotifier;
                if (!_notifiers.TryGetValue(markerGuid, out userMarkerNotifier))
                {
                    UI.Logger.IfError()?.Message($"Unable to remove userMarkerNotifier by guid {markerGuid}").Write();
                    return;
                }

                _notifiers.Remove(markerGuid);
                Destroy(userMarkerNotifier.gameObject);
            }
            else
            {
                if (pointMarker.NavIndicatorDef.AssertIfNull($"{nameof(pointMarker)}.{pointMarker.NavIndicatorDef}"))
                    return;

                if (_notifiers.ContainsKey(markerGuid))
                {
                    UI.Logger.IfError()?.Message($"{nameof(_notifiers)} already contains one with guid={markerGuid}: {_notifiers[markerGuid]}").Write();
                    return;
                }

                var newNotifier = Instantiate(
                    _userMarkerNotifierPrefab, UnityWorldSpace.ToVector3(pointMarker.Position), Quaternion.identity);
                if (newNotifier.AssertIfNull(nameof(newNotifier)))
                    return;

                newNotifier.NavIndicatorDef = pointMarker.NavIndicatorDef;
                newNotifier.MarkerGuid = markerGuid;
                newNotifier.name = $"{_userMarkerNotifierPrefab.name}_{newNotifier.NavIndicator?.Icon?.name}";
                _notifiers.Add(markerGuid, newNotifier);
            }
        }
    }
}