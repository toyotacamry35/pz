using SharedCode.Aspects.Building;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class FenceLinkData
    {
        public GameObject GameObject { get; set; } = null;
        public FenceLinkDef Def { get; set; } = null;
        public float FracturedChunkScale { get; set; } = 1.0f;
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;
        public bool Existed { get; set; } = false;
    }
}