using System.Collections.Generic;
using Assets.ResourceSystem.Aspects.Effects;
using Assets.Src.Cartographer;
using Assets.Src.Character;
using Assets.Src.Effects.Blood;
using Assets.Src.Effects.FX;
using Assets.Src.Effects.Step;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using Assets.Src.Shared;
using Assets.Src.Tools;
using Core.Environment.Logging.Extension;
using NLog;
using Plugins.DebugDrawingExtension;
using UnityEngine;
using Logger = NLog.Logger;

namespace Assets.Src.Effects.AnimatorEffects
{
    public class AnimationEventRelayWithSurface : MonoBehaviour
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly DebugDraw Drawer = DebugDraw.Manager.GetDrawer("AnimationEventRelayWithSurface");

        [SerializeField]
        JdbMetadata _markerToSurfaceRelation;

        [SerializeField]
        float _weightThreshold = 0.5f;

        [SerializeField]
        Vector3 MarkerRaycastOriginOffset = Vector3.up * 0.5f;

        [SerializeField]
        float MarkerRaycastDistance = 1.5f;

        private FXMarkerToDictionaryDef _markerToSurface;
        private readonly Dictionary<BaseResource, Data> _markerDictionary = new Dictionary<BaseResource, Data>();

        private GameObjectPoolSettings _settings;
        private ThirdPersonCharacterView _view;
        private FXElementParams _fxElementParams;


        public void SpawnSurface(AnimationEvent @event)
        {
            var info = @event.objectReferenceParameter;

            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"{info?.name} from {@event.animatorClipInfo.clip?.name} with weight:{@event.animatorClipInfo.weight}").Write();
            if (@event.animatorClipInfo.weight <= _weightThreshold)
                return;

            if (Logger.IsDebugEnabled && !Logger.IsTraceEnabled)
                Logger.IfDebug()?.Message($"{info?.name} from {@event.animatorClipInfo.clip?.name} with weight:{@event.animatorClipInfo.weight}").Write();
            if (info is JdbMetadata animationEventInfoMetadata)
            {
                AnimationEventInfoDef animationEventInfo = animationEventInfoMetadata.Get<AnimationEventInfoDef>();
                if (_markerDictionary.ContainsKey(animationEventInfo.Marker.Target))
                {
                    Data data = _markerDictionary[animationEventInfo.Marker.Target];
                    if (data.info == null && animationEventInfo.DeltaTime < data.time /*пришло время*/)
                    {
                        data.info = animationEventInfo;
                    }
                }
            }
            else
                Logger.IfWarn()?.Message($"It seems animation event passes incorrect object to script '{nameof(AnimationEventRelayWithSurface)}'").Write();
        }

        private void Start()
        {
            _markerToSurfaceRelation.AssertIfNull(nameof(_markerToSurfaceRelation));
            _markerToSurface = _markerToSurfaceRelation.Get<FXMarkerToDictionaryDef>();
            if (_markerToSurface == default)
                Logger.IfWarn()?.Message($"Incorrect type of data in 'MarkerToSurfaceRelation' field of '{nameof(AnimationEventRelayWithSurface)}' script").Write();

            FXBloodMarkerOnObj[] markerOnObj = gameObject.GetComponentsInChildren<FXBloodMarkerOnObj>();
            foreach (var obj in markerOnObj)
            {
                BaseResource marker = obj.Marker(Vector3.zero);

                if (!_markerDictionary.ContainsKey(marker))
                {
                    _markerDictionary.Add(marker, new Data(obj.gameObject));
                }
                else
                {
                    _markerDictionary[marker].AddParent(obj.gameObject);
                }
            }

            _view = GetComponentInParent<ThirdPersonCharacterView>();
            var gameObj = GetComponentInChildren<AkGameObj>();
            _fxElementParams = new FXElementParams {AkGameObject = gameObj};
            _settings = GameObjectPoolSettingsLevels.NonMandatory_0_1_100;
        }

        private void Update()
        {
            foreach (var element in _markerDictionary)
            {
                element.Value.AddTime(Time.deltaTime);

                //спаун
                if (element.Value.info != null)
                {
                    //ищем таблицу соответствия маркера поверхности к префабу
                    FXMarkerToPrefabDef surfaceRelation = _markerToSurface.GetObject(element.Key) as FXMarkerToPrefabDef;
                    if (surfaceRelation == null)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfWarn()?.Message($"{element.Value.info}: surface relation not found for '{element.Key}'").Write();
                        element.Value.info = null;
                        return;
                    }

                    foreach (var parent in element.Value.parent)
                    {
                        Vector3 position;
                        BaseResource surfaceMarker = GetMarker(parent.transform, parent.transform.position, out position);
                        if (surfaceMarker != null)
                        {
                            GameObject prefab = surfaceRelation.Get(surfaceMarker);

                            if (element.Value.info.PositionFromMarker)
                            {
                                position = parent.transform.position;
                            }

                            Quaternion rotation;
                            if (element.Value.info.RotationAllFromMarker)
                            {
                                rotation = parent.transform.rotation;
                            }
                            else if (element.Value.info.RotationForwardFromMarker)
                            {
                                rotation = Quaternion.Euler(0, parent.transform.eulerAngles.y, 0);
                            }
                            else
                            {
                                rotation = prefab.transform.rotation;
                            }

                            if (prefab != null)
                            {
                                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{element.Value.info}: spawn '{prefab}'").Write();
                                CheckPriority();
                                GameObjectPool.Instance.DelayedSpawn<FXPoolElement>(_settings, prefab, position, rotation, _fxElementParams);
                                /*if (go != null && element.Value.info.MarkerIsParent)
                                {
                                    go.transform.SetParent(parent.transform);
                                }*/
                            }
                            else if (Logger.IsDebugEnabled) Logger.IfWarn()?.Message($"{element.Value.info}: no prefab").Write();

                            element.Value.time = 0;
                        }
                        else if (Logger.IsDebugEnabled) Logger.IfWarn()?.Message($"{element.Value.info}: surface marker not found for '{parent.transform}'").Write();
                    }

                    element.Value.info = null;
                }
            }
        }

        private void CheckPriority()
        {
            if (_view.HasAuthority && _settings.priority != GameObjectPoolSettingsLevels.NonMandatory_0_1_1000.priority)
                _settings = GameObjectPoolSettingsLevels.NonMandatory_0_1_1000;
            else if (!_view.HasAuthority && _settings.priority != GameObjectPoolSettingsLevels.NonMandatory_0_1_100.priority)
                _settings = GameObjectPoolSettingsLevels.NonMandatory_0_1_100;
        }

        RaycastHit[] hits = new RaycastHit[5];

        public FXStepMarkerDef GetMarker(Transform foot, Vector3 defaultPosition, out Vector3 position)
        {
            position = defaultPosition;
            //получаем поверхность на которую наступил игрок
            var hitsCount = Physics.RaycastNonAlloc(
                foot.position + MarkerRaycastOriginOffset,
                Vector3.down,
                hits,
                PhysicsChecker.CheckDistance(MarkerRaycastDistance, "AnimationEventRelayWithSurface"),
                PhysicsLayers.CheckIsGroundedAndWaterMask,
                QueryTriggerInteraction.Ignore);

            if (hitsCount == 0)
            {
                if (Logger.IsDebugEnabled) Logger.IfWarn()?.Message($"{foot}: no hits").Write();
                Drawer.Debug?.Line(foot.position + MarkerRaycastOriginOffset, foot.position + MarkerRaycastOriginOffset + Vector3.down * MarkerRaycastDistance, Color.magenta);
                return default(FXStepMarkerDef);
            }

            float minDistance = -1;
            RaycastHit nearestHit = default(RaycastHit);
            for (int i = 0; i < hitsCount; i++)
            {
                var hit = hits[i];
                if (minDistance < 0 || hit.distance < minDistance)
                {
                    position = hit.point;
                    minDistance = hit.distance;
                    nearestHit = hit;
                }
            }

            var collider = nearestHit.collider;
            var terrainHolder = collider.GetComponent<TerrainHolderBehaviour>();
            var tBakerMatSupport = terrainHolder?.Terrain ?? null;
            if (tBakerMatSupport != null)
            {
                var tLayer = tBakerMatSupport.GetDominantLayer(nearestHit.point.ToXZ() - nearestHit.transform.position.ToXZ());
                if (tLayer == null || tLayer.layerFX == null)
                {
                    if (Logger.IsDebugEnabled) Logger.IfWarn()?.Message($"{foot}: no layer").Write();
                    return null;
                }

                var lFX = tLayer.layerFX?.Target;
                if (lFX != default(FXStepMarkerDef))
                    return lFX;
                else if (Logger.IsDebugEnabled) Logger.IfWarn()?.Message($"{foot}: no FXStepMarkerDef").Write();
            }

            var fxMarkerOnObj = collider.GetComponentInParent<FXMarkerOnObj>();
            if (fxMarkerOnObj != null)
                return fxMarkerOnObj._surfaceType.Get<FXStepMarkerDef>();
            else if (Logger.IsDebugEnabled) Logger.IfWarn()?.Message($"{foot}: no fxMarkerOnObj").Write();
            return default(FXStepMarkerDef);
        }
    }

    public class Data
    {
        public float time; //время прошедшее с предыдущего запуска такого типа эффекта        
        public List<GameObject> parent = new List<GameObject>();
        public AnimationEventInfoDef info; //информация об объекте для спауна

        public Data(GameObject obj)
        {
            time = 0;
            info = null;
            AddParent(obj);
        }

        public void AddParent(GameObject obj)
        {
            parent.Add(obj);
        }

        public void AddTime(float deltaTime)
        {
            time += deltaTime;
        }
    }
}