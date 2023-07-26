using Assets.ResourceSystem.Aspects.Effects;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using UnityEngine;

namespace Assets.Src.Effects.Blood
{
    public class FXBloodMarkerOnObj : FX.FXParamsOnObj, FX.IScriptableMarker
    {
        [SerializeField] JdbMetadata id;

        private FXBloodMarkerDef _id;

        private void Awake()
        {
            _id = id.Get<FXBloodMarkerDef>();
        }

        public BaseResource Marker(Vector3 position)
        {
            return _id;
        }
    }
}
