using UnityEngine;
using Assets.Instancenator;

namespace Assets.WorldFiller
{

    [AddComponentMenu("Terrain/Terrain Filler")]
    [RequireComponent(typeof(InstanceCompositionRenderer))]
    public class TerrainFiller : MonoBehaviour
    {
#if UNITY_EDITOR

        public TerrainBaker.TerrainAtlas atlas;
        public InstanceComposition terrainFilling;

#endif
    }
}


