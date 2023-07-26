using System;
using ProtoBuf;
using ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Newtonsoft.Json;

namespace SharedCode.Utils
{
    [ProtoContract]
    [KnownToGameResources]
    public partial struct Vector2Int : IHasRandomFill
    {
        [ProtoMember(1)]
        public int x;
        [ProtoMember(2)]
        public int y;

        [ProtoIgnore]
        public static readonly Vector2Int up = new Vector2Int(0, 1);

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Int(Vector2Int v) : this()
        {
            x = v.x;
            y = v.y;
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            x = random.Next(100);
            y = random.Next(100);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2Int))
            {
                return false;
            }
            return this == (Vector2Int)obj;
        }

        public override int GetHashCode()
        {
            var hashCode = 1462929421;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
        [JsonIgnore]
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            }
        }

        public static Vector2Int operator *(Vector2Int v, int scale)
        {
            return new Vector2Int(v.x * scale, v.y * scale);
        }

        public static Vector2Int operator /(Vector2Int v, int scale)
        {
            return new Vector2Int(v.x / scale, v.y / scale);
        }

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2)
        {
            return new Vector2Int(v1.x - v2.x, v1.y - v2.y);
        }

        public static bool operator !=(Vector2Int v1, Vector2Int v2)
        {
            return ((v1.x != v2.x) || (v1.y != v2.y));
        }

        public static bool operator ==(Vector2Int v1, Vector2Int v2)
        {
            return ((v1.x == v2.x) && (v1.y == v2.y));
        }

        [JsonIgnore]
        public float SqrLength => x * x + y * y;

        [JsonIgnore]
        public float Length => (float)Math.Sqrt(SqrLength);

        public static double GetDistance(Vector2Int v1, Vector2Int v2)
        {
            return (v1 - v2).Length;
        }

        public static double GetSqrDistance(Vector2Int v1, Vector2Int v2)
        {
            return (v1 - v2).SqrLength;
        }

#if UNITY_5_3_OR_NEWER
        public Vector2Int(UnityEngine.Vector2Int v)
        {
            x = v.x;
            y = v.y;
        }
        public static explicit operator UnityEngine.Vector2Int(Vector2Int v)
        {
            return new UnityEngine.Vector2Int(v.x, v.y);
        }
        public static explicit operator UnityEngine.Vector3(Vector2Int v)
        {
            return new UnityEngine.Vector3(v.x, v.y, 0);
        }
#endif
    }

    public class Vector2IntJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var v = (Vector2Int) value;
            writer.WriteValue($"{v.x}:{v.y}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var strVal = (string) reader.Value;
            var arr = strVal.Split(':');
            return new Vector2Int(int.Parse(arr[0]), int.Parse(arr[1]));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2Int);
        }
    }
}
