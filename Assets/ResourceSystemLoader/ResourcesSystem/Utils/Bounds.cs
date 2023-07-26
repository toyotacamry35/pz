using Assets.ColonyShared.SharedCode.Utils;
using ResourcesSystem.Loader;
using Newtonsoft.Json;
using NLog.Layouts;
using ProtoBuf;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.Utils
{
    [ProtoContract]
    [KnownToGameResources]
    public struct Bounds
    {
        [ProtoMember(1)]
        public Vector3 center;

        [ProtoMember(2)]
        public Vector3 extents;

        [JsonIgnore]
        [ProtoIgnore]
        public Vector3 Size => extents * 2;

        [JsonIgnore]
        [ProtoIgnore]
        public Vector3 Min => center - extents;

        [JsonIgnore]
        [ProtoIgnore]
        public Vector3 Max => center + extents;
        
        public Bounds(Vector3 center, Vector3 size)
        {
            this.center = center;
            extents = size * 0.5f;
        }

        public bool Contains(Vector3 point)
        {
            var diff = point - center;
            return diff.x >= -extents.x && diff.x <= extents.x &&
                   diff.y >= -extents.y && diff.y <= extents.y &&
                   diff.z >= -extents.z && diff.z <= extents.z;
        }
        
#if UNITY_5_3_OR_NEWER
        public Bounds(UnityEngine.Bounds v)
        {
            center = new Vector3(v.center);
            extents = new Vector3(v.extents);
        }

        public static explicit operator UnityEngine.Bounds(Bounds v)
        {
            return new UnityEngine.Bounds((UnityEngine.Vector3)v.center, (UnityEngine.Vector3)v.extents * 2);
        }
#endif        
    }
}