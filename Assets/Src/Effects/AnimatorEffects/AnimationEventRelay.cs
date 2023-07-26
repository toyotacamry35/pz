using Assets.Src.Effects.Blood;
using Assets.Src.App.FXs;
using UnityEngine;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using NLog;
using Assets.ResourceSystem.Aspects.Effects;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Effects.AnimatorEffects
{
    public class AnimationEventRelay : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public void Spawn(Object markerMetadata)
        {
            if (!(markerMetadata is JdbMetadata))
                return;

            var marker = (markerMetadata as JdbMetadata).Get<BaseResource>();

            if (marker == default)
                Logger.IfWarn()?.Message($"Incorrect type of marker (should be {nameof(JdbMetadata)}: {markerMetadata}").Write();

            if (markerOnObj == null)
            {
                markerOnObj = gameObject.GetComponentsInChildren<FXBloodMarkerOnObj>();
            }

            foreach (var obj in markerOnObj)
            {
                if (obj.Marker(Vector3.zero) == marker)
                {
                    GameObject prefab = _markerToPrefab.Get(obj.Marker(Vector3.zero));
                    if (prefab != null)
                    {
                        FxPlayer.StartPlay(prefab, new FXInfo(_targetIsParent ? obj.gameObject : null, obj.gameObject.transform.position,
                            _rotationFromMarker ? obj.gameObject.transform.rotation : prefab.transform.rotation, obj.Marker(Vector3.zero), obj.pair));
                    }
                }
            }
        }


        [SerializeField] FXMarkerToPrefabDef _markerToPrefab;
        [SerializeField] bool _targetIsParent = true;
        [SerializeField] bool _rotationFromMarker = true;

        FXBloodMarkerOnObj[] markerOnObj;
    }
}
