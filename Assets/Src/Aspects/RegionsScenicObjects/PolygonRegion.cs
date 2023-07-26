using Assets.Src.Shared;
using NLog;
using UnityEngine;
using Assets.Src.ResourceSystem;
using SharedCode.Aspects.Regions;

namespace Assets.Src.Aspects.RegionsScenicObjects
{
    public class PolygonRegion : ColonyBehaviour
    {
        public float MaxHeight => transform.position.y + _height;
        public float MinHeight => transform.position.y + _heightBottom;
        [SerializeField]
        private float _height = 100;
        [SerializeField]
        private float _heightBottom = -100;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public JdbMetadata PathToShapeResource;
        private ConcaveShapeDef _shape;
        public ConcaveShapeDef Shape { get { if (PathToShapeResource != null) _shape = PathToShapeResource.Get<ConcaveShapeDef>(); return _shape; } set { _shape = value; } }
        public string PathToShape { get; set; }
        public long GenerationVersion = 0;
    }
    public static class Vec2ConversionToVec3Unity
    {
        public static Vector2 ToXY(this SharedCode.Utils.Vector2 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        public static Vector2 ToXY(this SharedCode.Utils.Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        public static Vector3 ToXYZ(this SharedCode.Utils.Vector2 vec)
        {
            return new Vector3(vec.x, 0, vec.y);
        }
        public static Vector3 ToXYZ(this SharedCode.Utils.Vector3 vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }
    }
}
