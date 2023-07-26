using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Newtonsoft.Json;
using SharedCode.Utils;
using System.Collections.Generic;

namespace SharedCode.Aspects.Regions
{
    public class ConcaveShapeDef : BaseResource, ISpatialHashed
    {
        public float Height = 2000;
        public float StartHeight = -2000;
        public PrecalculatedAABB BoundingBox { get; set; }
        public Triangle[] Triangles { get; set; }

        public void GetHash(ISpatialHash spHash, ICollection<Vector3Int> resultHash)
        {
            var rh = new RectHash(BoundingBox.Center + new Vector3(0, (Height + StartHeight) / 2, 0), BoundingBox.Size + new Vector3(0, (Height - (Height + StartHeight) / 2) * 2, 0));
            rh.GetHash(spHash, resultHash);
        }
    }
    [KnownToGameResources]
    public struct PrecalculatedAABB
    {
        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;

        [JsonIgnore]
        public Vector3 Center => new Vector3((MinX + MaxX) / 2, 0, (MinY + MaxY) / 2);
        [JsonIgnore]
        public Vector3 Size => new Vector3((MaxX - (MinX + MaxX) / 2) * 2, 0, (MaxY - (MinY + MaxY) / 2) * 2);
    }
    [KnownToGameResources]
    [JsonConverter(typeof(TriangleConverter))]
    public struct Triangle
    {
        public Vector2 PointA;
        public Vector2 PointB;
        public Vector2 PointC;
    }
}
