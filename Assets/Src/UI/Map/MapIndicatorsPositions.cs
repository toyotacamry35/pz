using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Shared.Impl;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Entities.Engine;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    /// <summary>
    /// Инстанциирование/удаление, показ и обновление позиций индикаторов на карте от любых источников 
    /// </summary>
    [Binding]
    public class MapIndicatorsPositions : BindingViewModel
    {
        public event Action<MapIndicator> IndicatorClickedRmb;

        [SerializeField, UsedImplicitly]
        private MapIndicator _mapIndicatorPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _indicatorsTransform;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _updateInterval;

        [SerializeField, UsedImplicitly]
        private MapGuiWindow _mapGuiWindow;

        [SerializeField, UsedImplicitly]
        private bool _forceGetCornersFromTerrains;

        [SerializeField, UsedImplicitly]
        private Vector2 _mapTextureSize = new Vector2(8192, 8192);

        public int QuestZoneRotationFactor = -1;

        private List<MapIndicator> _indicators = new List<MapIndicator>();

        private bool _hasPawn;
        private LobbyGuiNode _lobbyGuiNode;

        public Vector2 _mapPoint1, _mapPoint2;
        public Vector2 _levelPoint1, _levelPoint2;

        private LinearRelation _mapXLinearRelation, _mapYLinearRelation;

        private IMapNotificationProvider _mapNotificationProvider;


        //=== Props ===========================================================

        public MapIndicator PlayerIndicator { get; private set; }

        public ReactiveProperty<MapIndicator> SelectedIndicator { get; } = new ReactiveProperty<MapIndicator>() {Value = null};

        /// <summary>
        /// Базовый масштаб карты, px/м
        /// </summary>
        private ReactiveProperty<float> _baseMapScaleRp = new ReactiveProperty<float>();

        /// <summary>
        /// Текущий масштаб карты с учетом зума, px/м
        /// </summary>
        public ReactiveProperty<float> CurrentMapScaleRp { get; } = new ReactiveProperty<float>();

        private Vector2 _cornerBl;

        [Binding]
        public Vector2 CornerBl
        {
            get => _cornerBl;
            set
            {
                if (_cornerBl != value)
                {
                    _cornerBl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Vector2 _cornerTr;

        [Binding]
        public Vector2 CornerTr
        {
            get => _cornerTr;
            set
            {
                if (_cornerTr != value)
                {
                    _cornerTr = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public GameObject PawnGameObject { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _mapGuiWindow.AssertIfNull(nameof(_mapGuiWindow));
            _mapIndicatorPrefab.AssertIfNull(nameof(_mapIndicatorPrefab));
            _indicatorsTransform.AssertIfNull(nameof(_indicatorsTransform));
            _updateInterval.AssertIfNull(nameof(_updateInterval));
        }

        private void Update()
        {
            if (!_hasPawn || _mapGuiWindow.State.Value == GuiWindowState.Closed || !_updateInterval.IsItTime())
                return;

            foreach (var mapIndicator in _indicators)
                mapIndicator.UpdatePosition();
        }


        //=== Public ==========================================================

        public void Init(
            IPawnSource pawnSource,
            IMapNotificationProvider mapNotificationProvider,
            LobbyGuiNode lobbyGuiNode,
            IStream<float> currentZoomRatioStream)
        {
            if (pawnSource.AssertIfNull(nameof(pawnSource)) ||
                mapNotificationProvider.AssertIfNull(nameof(mapNotificationProvider)) ||
                lobbyGuiNode.AssertIfNull(nameof(lobbyGuiNode)) ||
                currentZoomRatioStream.AssertIfNull(nameof(currentZoomRatioStream)))
                return;

            _lobbyGuiNode = lobbyGuiNode;
            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
            _mapNotificationProvider = mapNotificationProvider;
            _mapNotificationProvider.AddMapIndicatorTarget += OnAddMapIndicatorTarget;
            _mapNotificationProvider.RemoveMapIndicatorTarget += OnRemoveMapIndicatorTarget;

            GameObjectCreator.ClusterSpawnInstance.CurrentMap
                .Action(
                    D,
                    mapDef =>
                    {
                        if (mapDef != null)
                            CornersInit(mapDef);
                    }
                );
            
            var currentMapScaleStream = _baseMapScaleRp
                .Zip(D, currentZoomRatioStream)
                //.Log(D, "(baseMapScale, zoomRatio)", (baseMapScale, zoomRatio) => $"({baseMapScale}, {zoomRatio})") //2del
                .Func(D, (baseMapScale, zoomRatio) => baseMapScale * zoomRatio);
            currentMapScaleStream.Action(D, scale => CurrentMapScaleRp.Value = scale);
        }

        public Vector2 GetAnchorRatioByLevelPosition(Vector3 position)
        {
            if (_mapXLinearRelation == null)
                return Vector2.one * 0.5f;

            return new Vector2(_mapXLinearRelation.GetY(position.x), _mapYLinearRelation.GetY(position.z));
        }

        public Vector2 GetLevelPositionXzByAnchorRatio(Vector2 anchorRatio)
        {
            if (_mapXLinearRelation == null)
                return Vector2.zero;

            return new Vector2(_mapXLinearRelation.GetX(anchorRatio.x), _mapYLinearRelation.GetX(anchorRatio.y));
        }

        public void SetCustomMapAnchorPoint(int index, Vector2 v2)
        {
            if (index == 1)
                _mapPoint1 = v2;
            else
                _mapPoint2 = v2;
        }

        public void SetCustomLevelPoint(int index, Vector2 v2)
        {
            if (index == 1)
                _levelPoint1 = v2;
            else
                _levelPoint2 = v2;
        }

        public void SetMapCorners(bool isReset)
        {
            var mapDef = GameObjectCreator.ClusterSpawnInstance.CurrentMap.Value;
            if (!isReset || mapDef != null)
                CornersInit(mapDef, !isReset);
        }

        public void TakeSelectedIndicator(MapIndicator newSelectedIndicator)
        {
            SelectedIndicator.Value = newSelectedIndicator;
            _mapNotificationProvider.SetSelectedTarget(SelectedIndicator.Value?.Target);
        }

        public void OnIndicatorClickRmb(MapIndicator mapIndicator)
        {
            IndicatorClickedRmb?.Invoke(mapIndicator);
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            _hasPawn = newEgo != null;
            PawnGameObject = newEgo != null ? newEgo.gameObject : null;
        }

        private void OnAddMapIndicatorTarget(
            Transform target,
            Guid markerGuid,
            IMapIndicatorSettings mapIndicatorSettings,
            bool isInteractable,
            IGuideProvider cameraGuideProvider)
        {
            if (mapIndicatorSettings.AssertIfNull(nameof(mapIndicatorSettings)) ||
                target.AssertIfNull(nameof(target)))
                return;

            if (GetIndicator(target) != null)
                return;

            var newIndicator = Instantiate(_mapIndicatorPrefab, _indicatorsTransform);
            newIndicator.name = $"{_mapIndicatorPrefab.name}{_indicators.Count + 1}";
            newIndicator.SetVmodel(
                new MapIndicatorVmodel()
                {
                    Target = target,
                    MapIndicatorSettings = mapIndicatorSettings,
                    MapIndicatorsPositions = this,
                    MarkerGuid = markerGuid
                });
            if (!isInteractable)
                newIndicator.SetOffImageColliders();
            if (cameraGuideProvider != null)
            {
                newIndicator.CameraGuideProvider = cameraGuideProvider;
                PlayerIndicator = newIndicator;
            }

            _indicators.Add(newIndicator);
        }

        private void OnRemoveMapIndicatorTarget(Guid markerGuid, Transform target)
        {
            var indicatorToRemove = GetIndicator(target);
            if (indicatorToRemove == null)
                return;

            if (PlayerIndicator == indicatorToRemove)
                PlayerIndicator = null;

            _indicators.Remove(indicatorToRemove);
            Destroy(indicatorToRemove.gameObject);
        }

        private MapIndicator GetIndicator(Transform target)
        {
            return _indicators.FirstOrDefault(indicator => indicator.Target == target);
        }

        private void CornersInit(MapDef mapDef, bool useCustomPoints = false)
        {
            Vector2 terrainBLCorner, terrainTRCorner;
            if (useCustomPoints)
            {
                GetCornersByCustomPoints(_levelPoint1, _mapPoint1, _levelPoint2, _mapPoint2, out terrainBLCorner, out terrainTRCorner);
                foreach (var mapIndicator in _indicators)
                    mapIndicator.UpdatePosition(true);
            }
            else
            {
                if (_forceGetCornersFromTerrains ||
                    mapDef.AssertIfNull(nameof(mapDef)) ||
                    (mapDef.MapBottomLeftCorner.IsZero && mapDef.MapTopRightCorner.IsZero))
                {
                    MapUtils.GetCornersFromTerrainBounds(out terrainBLCorner, out terrainTRCorner);
                }
                else
                {
                    terrainBLCorner = new Vector2(mapDef.MapBottomLeftCorner.X, mapDef.MapBottomLeftCorner.Y);
                    terrainTRCorner = new Vector2(mapDef.MapTopRightCorner.X, mapDef.MapTopRightCorner.Y);
                    UI.Logger.IfInfo()?.Message($"Corners from MapDef: BL={terrainBLCorner}, TR={terrainTRCorner}").Write(); //DEBUG
                }
            }

            if (HasEqualCoords(terrainBLCorner, terrainTRCorner))
            {
                UI.Logger.Error(
                    $"The same corner angles: x={terrainBLCorner.x}/{terrainTRCorner.x} " +
                    $"or y={terrainBLCorner.y}/{terrainTRCorner.y}");
                terrainBLCorner = Vector2.zero;
                terrainTRCorner = Vector2.one * 100;
            }

            _mapXLinearRelation = new LinearRelation(terrainBLCorner.x, 0, terrainTRCorner.x, 1);
            _mapYLinearRelation = new LinearRelation(terrainBLCorner.y, 0, terrainTRCorner.y, 1);
            CornerBl = GetCornerOnTerrain(true);
            CornerTr = GetCornerOnTerrain(false);
            _baseMapScaleRp.Value = _mapTextureSize.x / (CornerTr.x - CornerBl.x);
        }

        private bool HasEqualCoords(Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) || Mathf.Approximately(v1.y, v2.y);
        }

        private Vector2 GetCornerOnTerrain(bool isBottomLeftNorTopRight)
        {
            var y = isBottomLeftNorTopRight ? 0 : 1;
            return new Vector2(_mapXLinearRelation.GetX(y), _mapYLinearRelation.GetX(y));
        }

        private void GetCornersByCustomPoints(
            Vector2 terrainCustomPoint1,
            Vector2 mapCustomPoint1,
            Vector2 terrainCustomPoint2,
            Vector2 mapCustomPoint2,
            out Vector2 terrainBlCorner,
            out Vector2 terrainTrCorner)
        {
            terrainBlCorner = terrainTrCorner = Vector2.zero;
            if (HasEqualCoords(terrainCustomPoint1, terrainCustomPoint2) ||
                HasEqualCoords(mapCustomPoint1, mapCustomPoint2))
            {
                UI.Logger.Error(
                    $"{nameof(GetCornersByCustomPoints)}() Points has equal coords pairs. Unable to continue: " +
                    $"{terrainCustomPoint1} / {terrainCustomPoint2} OR {mapCustomPoint1} / {mapCustomPoint2}");
                return;
            }

            var xlr = new LinearRelation(mapCustomPoint1.x, terrainCustomPoint1.x, mapCustomPoint2.x, terrainCustomPoint2.x);
            var ylr = new LinearRelation(mapCustomPoint1.y, terrainCustomPoint1.y, mapCustomPoint2.y, terrainCustomPoint2.y);

            terrainBlCorner = new Vector2(xlr.GetY(0), ylr.GetY(0));
            terrainTrCorner = new Vector2(xlr.GetY(1), ylr.GetY(1));
            UI.CallerLog($"Calculated corners: BL={terrainBlCorner},  TR={terrainTrCorner}");
        }
    }
}