using Assets.ResourceSystem.Aspects.Effects;
using Assets.Src.ResourceSystem;
using UnityEngine;

namespace Assets.Src.Effects.Step
{
    public class FXMarkerOnObj : MonoBehaviour
    {
        public JdbMetadata _surfaceType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_surfaceType != null && _surfaceType.Get<FXStepMarkerDef>() == null)
                Debug.LogError($"Wrong type of resource in {nameof(_surfaceType)} field of {gameObject.name}");
        }
#endif
    }
}
