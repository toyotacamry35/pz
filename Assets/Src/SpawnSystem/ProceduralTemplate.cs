using Assets.Src.ResourceSystem;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    [CreateAssetMenu(fileName = "NewProceduralTemplate", menuName = "Spawn/ProceduralTemplate", order = 1)]
    public class ProceduralTemplate : ScriptableObject
    {
        public float Radius = 1;
        [System.Serializable] // for Unity to show this in inspector
        public struct SpawnPointSettings
        {
            public Texture2D TextureFilter;
            public JdbMetadata SpawnPointType;
            public int Count;
            public float MaxRadius;
            public float MinRadius;
        }
        public SpawnPointSettings[] Settings;
        public JdbMetadata ExclusionGroup;
        public float ExclusionRadius = 10;
    }
}
