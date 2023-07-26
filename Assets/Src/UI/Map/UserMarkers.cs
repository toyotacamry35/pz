using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Navigation;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ContainerApis;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Src.Shared;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;

namespace Uins
{
    public class UserMarkers : HasDisposablesMonoBehaviour
    {
        private const float MaxLevelHight = 1000;

        private const int MaxUserMarkerCount = 500; //TODOM Перевести в константы

        public event Action<Vector2> ShowContextMenu;
        public event Action HideContextMenu;

        [SerializeField, UsedImplicitly]
        private UserMarkerNotifier _defaultUserMarkerNotifierPrefab;

        [SerializeField, UsedImplicitly]
        private MapIndicatorsPositions _mapIndicatorsPositions;

        [SerializeField, UsedImplicitly]
        private NavIndicatorDefRef[] _userNavIndicatorDefRefs;

        private UserMarkerNotifier _defaultUserMarkerNotifier;
        private Dictionary<Guid, NavIndicatorDef> _userNavIndicatorsOnServer = new Dictionary<Guid, NavIndicatorDef>();

        private Vector2 _customLevelPoint;
        private Vector2 _customMapAnchorPoint;
        private EntityApiWrapper<PointMarkersApi> _pointMarkersApiWrapper;


        //=== Props ===========================================================

        public NavIndicatorDef[] UserIndicatorDefs { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _defaultUserMarkerNotifierPrefab.AssertIfNull(nameof(_defaultUserMarkerNotifierPrefab));
            _mapIndicatorsPositions.AssertIfNull(nameof(_mapIndicatorsPositions));
            if (!_userNavIndicatorDefRefs.IsNullOrEmptyOrHasNullElements(nameof(_userNavIndicatorDefRefs)))
                UserIndicatorDefs = _userNavIndicatorDefRefs.Select(r => r.Target).ToArray();

            _mapIndicatorsPositions.IndicatorClickedRmb += OnMapIndicatorClickedRmb;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_mapIndicatorsPositions != null)
                _mapIndicatorsPositions.IndicatorClickedRmb -= OnMapIndicatorClickedRmb;
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        }

        public void OnMapClick(Vector2 mapAnchorPosition, bool isLmb)
        {
            if (isLmb)
            {
                _customMapAnchorPoint = mapAnchorPosition;
                _customLevelPoint = _mapIndicatorsPositions.GetLevelPositionXzByAnchorRatio(mapAnchorPosition);
                SetUserMarkerNotifier(mapAnchorPosition, true);
            }
            else
            {
                //UI.CallerLog($"RMB anchorPos={mapAnchorPosition}");
                ShowContextMenu?.Invoke(mapAnchorPosition);
            }
        }

        public void SetUserMarkerNotifier(Vector2 mapAnchorPosition, bool isDefaultMarker, NavIndicatorDef navIndicatorDef = null)
        {
            var levelPoint = _mapIndicatorsPositions.GetLevelPositionXzByAnchorRatio(mapAnchorPosition);

            if (isDefaultMarker)
            {
                if (_defaultUserMarkerNotifier == null)
                {
                    _defaultUserMarkerNotifier = Instantiate(_defaultUserMarkerNotifierPrefab);
                    _defaultUserMarkerNotifier.name = "UserMarker_Default";
                }

                _defaultUserMarkerNotifier.transform.position = GetTerrainPoint(levelPoint);
            }
            else
            {
                if (_userNavIndicatorsOnServer.Count > MaxUserMarkerCount)
                {
                    UI.Logger.IfError()?.Message($"Max marker limit ({MaxUserMarkerCount}) reached").Write();
                    return;
                }

                if (navIndicatorDef.AssertIfNull(nameof(navIndicatorDef)))
                    return;

                var markerGuid = Guid.NewGuid();
                _pointMarkersApiWrapper.EntityApi.AddPointMarker(markerGuid, navIndicatorDef, GetTerrainPoint(levelPoint));
            }

            HideContextMenu?.Invoke();
        }

        public void SetCustomMapAnchorPoint(int index)
        {
            _mapIndicatorsPositions.SetCustomMapAnchorPoint(index, _customMapAnchorPoint);
        }

        public void SetCustomLevelPoint(int index)
        {
            _mapIndicatorsPositions.SetCustomLevelPoint(index, _customLevelPoint);
        }

        public void SetCorners(bool isReset)
        {
            _mapIndicatorsPositions.SetMapCorners(isReset);
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _pointMarkersApiWrapper.EntityApi.UnsubscribeFromPointMarkers(OnPointMarkerAddedOrRemoved);
                _userNavIndicatorsOnServer.Clear();

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
                if (!_userNavIndicatorsOnServer.TryGetValue(markerGuid, out _))
                    return;

                _userNavIndicatorsOnServer.Remove(markerGuid);
            }
            else
            {
                // if (!UserIndicatorDefs.Contains(pointMarker.NavIndicatorDef)) //лишало возможности удалять что-либо кроме этих
                //     return;

                if (_userNavIndicatorsOnServer.ContainsKey(markerGuid))
                {
                    UI.Logger.Error(
                        $"{nameof(_userNavIndicatorsOnServer)} already contains one with guid={markerGuid}: {_userNavIndicatorsOnServer[markerGuid]}");
                    return;
                }

                _userNavIndicatorsOnServer.Add(markerGuid, pointMarker.NavIndicatorDef);
            }
        }

        private void OnMapIndicatorClickedRmb(MapIndicator mapIndicator)
        {
            if (mapIndicator.AssertIfNull(nameof(mapIndicator)))
                return;

            if (_defaultUserMarkerNotifier != null && mapIndicator.Target == _defaultUserMarkerNotifier.transform)
            {
                Destroy(_defaultUserMarkerNotifier.gameObject);
                return;
            }

            if (!_userNavIndicatorsOnServer.ContainsKey(mapIndicator.MarkerGuid))
                return;

            _pointMarkersApiWrapper.EntityApi.RemovePointMarker(mapIndicator.MarkerGuid);
        }

        private Vector3 GetTerrainPoint(Vector2 levelPositionXz)
        {
            var highPoint = new Vector3(levelPositionXz.x, MaxLevelHight, levelPositionXz.y);
            if (!Physics.Raycast(
                new Ray(highPoint, Vector3.down),
                out var raycastHit,
                PhysicsChecker.CheckClientDistance(MaxLevelHight * 2, "UserMarkers"),
                PhysicsLayers.TerrainMask))
                return new Vector3(
                    levelPositionXz.x,
                    _mapIndicatorsPositions.PawnGameObject != null ? _mapIndicatorsPositions.PawnGameObject.transform.position.y : 0,
                    levelPositionXz.y);

            return raycastHit.point;
        }
    }
}