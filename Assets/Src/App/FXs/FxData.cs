using System;
using Assets.ResourceSystem.Aspects.Effects;
using Assets.Src.App.FXs.Definitions;
using Assets.Src.GameObjectAssembler;

namespace UnityEngine
{
    [Serializable]
    public class FxData : MonoBehaviour, IFromDef<FxDataDef>
    {
        public GameObject ParticlePrefab;
        public FXMarkerToPrefabDef MarkerToPrefab;
        public AudioClip Sound;
        public bool LoopSound;
        public float SoundMaxDist;
        public float LifeTimeManualSec;
        // Use it for (f.e.) fire effects, which y-axis should not be rotated
        public bool PositionAlwaysVertical;

        public float LifeTimeSec
        {
            get
            {
                if (LifeTimeManualSec > 0f)
                    return LifeTimeManualSec;

                float res = 0f;
                var particle = ParticlePrefab?.GetComponent<ParticleSystem>();
                if (particle)
                    res = particle.main.duration;

                if (Sound && !LoopSound)
                    res = Mathf.Max(res, Sound.length);

                return res;
            }
        }

        private FxDataDef _def;
        public FxDataDef Def
        {
            get { return _def; }
            set
            {
                _def = value;
                ParticlePrefab = _def.ParticlePrefabPath.Target;
                Sound = _def.SoundPath.Target;
                SoundMaxDist           = _def.SoundMaxDist;
                LifeTimeManualSec      = _def.LifeTimeManualSec;
                PositionAlwaysVertical = _def.PositionAlwaysVertical;
            }
        }

        public override string ToString()
        {
            return string.Format("ParticlePrefab = {0}, MarkerToPrefab = {1}", ParticlePrefab == null ? "null" : ParticlePrefab.name,
                MarkerToPrefab == null ? "null" : MarkerToPrefab.____GetDebugShortName());
        }
    }
}
