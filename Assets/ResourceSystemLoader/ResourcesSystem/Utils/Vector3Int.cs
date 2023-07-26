
using System;
using ResourcesSystem.Loader;
using Newtonsoft.Json;
using ProtoBuf;
using ResourcesSystem.Base;

namespace SharedCode.Utils
{
    [KnownToGameResources]
    [ProtoContract]
    public partial struct Vector3Int : IHasRandomFill
    {
        [ProtoMember(1)]
        public int x;
        [ProtoMember(2)]
        public int y;
        [ProtoMember(3)]
        public int z;

        [ProtoIgnore]
        public static readonly Vector3Int zero = new Vector3Int(0, 0, 0);
        [ProtoIgnore]
        public static readonly Vector3Int one = new Vector3Int(1, 1, 1);
        [ProtoIgnore]
        public static readonly Vector3Int up = new Vector3Int(0, 1, 0);
        [ProtoIgnore]
        public static readonly Vector3Int forward = new Vector3Int(0, 0, 1);

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Int(Vector3Int v) : this()
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            x = random.Next(100);
            y = random.Next(100);
            z = random.Next(100);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3Int))
            {
                return false;
            }
            return this == (Vector3Int)obj;
        }

        public override int GetHashCode()
        {
            var hashCode = 1462929421;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
            }
        }

        public static Vector3Int operator *(Vector3Int v, int scale)
        {
            return new Vector3Int(v.x * scale, v.y * scale, v.z * scale);
        }

        public static Vector3Int operator /(Vector3Int v, int scale)
        {
            return new Vector3Int(v.x / scale, v.y / scale, v.z / scale);
        }

        public static Vector3Int operator +(Vector3Int v1, Vector3Int v2)
        {
            return new Vector3Int(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3Int operator -(Vector3Int v1, Vector3Int v2)
        {
            return new Vector3Int(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static bool operator !=(Vector3Int v1, Vector3Int v2)
        {
            return ((v1.x != v2.x) || (v1.y != v2.y) || (v1.z != v2.z));
        }

        public static bool operator ==(Vector3Int v1, Vector3Int v2)
        {
            return ((v1.x == v2.x) && (v1.y == v2.y) && (v1.z == v2.z));
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float SqrLength => x * x + y * y + z * z;

        [JsonIgnore]
        [ProtoIgnore]
        public float Length => (float)Math.Sqrt(SqrLength);

        public static double GetDistance(Vector3Int v1, Vector3Int v2)
        {
            return (v1 - v2).Length;
        }

        public static double GetSqrDistance(Vector3Int v1, Vector3Int v2)
        {
            return (v1 - v2).SqrLength;
        }
#if UNITY_5_3_OR_NEWER
        public Vector3Int(UnityEngine.Vector3Int v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static explicit operator UnityEngine.Vector3Int(Vector3Int v)
        {
            return new UnityEngine.Vector3Int(v.x, v.y, v.z);
        }
#endif

        public bool Any(Predicate<int> pred) => pred(x) || pred(y) || pred(z);
        public bool All(Predicate<int> pred) => pred(x) && pred(y) && pred(z);

        public override string ToString()
        {
            return $"[ 'x':{x}, 'y':{y}, 'z':{z} ]";
        }
    }

    public class Vector3IntJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = (Vector3Int)value;
            writer.WriteValue($"{v.x}:{v.y}:{v.z}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var strVal = (string)reader.Value;
            var arr = strVal.Split(':');
            return new Vector3Int(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3Int);
        }
    }
}
