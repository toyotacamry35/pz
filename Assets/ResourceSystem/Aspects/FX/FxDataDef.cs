using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using UnityEngine;

namespace Assets.Src.App.FXs.Definitions
{
    public class FxDataDef : ComponentDef
    {
        public UnityRef<GameObject> ParticlePrefabPath;
        public UnityRef<AudioClip> SoundPath;
        public float SoundMaxDist;
        public float LifeTimeManualSec;
        // Use it for (f.e.) fire effects, which y-axis should not be rotated
        public bool PositionAlwaysVertical;
    }
}
