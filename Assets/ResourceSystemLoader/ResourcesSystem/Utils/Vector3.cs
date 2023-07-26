
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using ResourcesSystem.Loader;
using Newtonsoft.Json;
using NLog;
using ProtoBuf;
using ResourcesSystem.Base;

namespace SharedCode.Utils
{
    [KnownToGameResources]
    [ProtoContract]
    public partial struct Vector3 : IHasRandomFill
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [ProtoMember(1)]
        public float x;
        [ProtoMember(2)]
        public float y;
        [ProtoMember(3)]
        public float z;

        [ProtoIgnore]
        public static readonly Vector3 zero = new Vector3(0.0f, 0.0f, 0.0f);
        [ProtoIgnore]
        public static readonly Vector3 one = new Vector3(1.0f, 1.0f, 1.0f);
        [ProtoIgnore]
        public static readonly Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        [ProtoIgnore]
        public static readonly Vector3 forward = new Vector3(0.0f, 0.0f, 1.0f);
        [ProtoIgnore]
        public static readonly Vector3 right = new Vector3(1f, 0.0f, 0.0f);
        [ProtoIgnore]
        public static readonly Vector3 Default;


        // static ctor
        static Vector3()
        {
            Default = default(Vector3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Vector3(in Vector3 v) : this()
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Vector3(in Vector2 xy, float z = 0)
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = z;
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            x = random.Next(100);
            y = random.Next(100);
            z = random.Next(100);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector3))
            {
                return false;
            }
            return this == (Vector3)obj;
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

        public static float Distance(Vector3 value1, Vector3 value2)
        {
            return (value1 - value2).magnitude;
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (float) Math.Sqrt(x * x + y * y + z * z); }
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (float) Math.Sqrt(x * x + y * y + z * z); }
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float SqrMagnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return x * x + y * y + z * z; } }

        [JsonIgnore]
        [ProtoIgnore]
        public float sqrMagnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return x * x + y * y + z * z; } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator *(float scale, in Vector3 v)
        {
            return v * scale;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator *(in Vector3 v, float scale)
        {
            return new Vector3(v.x * scale, v.y * scale, v.z * scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator /(in Vector3 v, float scale)
        {
            return new Vector3(v.x / scale, v.y / scale, v.z / scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator +(in Vector3 v1, in Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator -(in Vector3 v1, in Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator *(in Vector3 v1, in Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(in Vector3 v1, in Vector3 v2)
        {
            return (v1 - v2).Magnitude > 1E-05F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(in Vector3 v1, in Vector3 v2)
        {
            return !(v1 != v2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator -(in Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }

        // float operators:
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator +(in Vector3 v, float f)
        {
            return new Vector3(v.x + f, v.y + f, v.z + f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 operator -(in Vector3 v, float f)
        {
            return v + (-f);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float SqrLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => x * x + y * y + z * z;
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (float) Math.Sqrt(SqrLength);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public Vector3 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Normalize(this);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public Vector3 normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Normalize(this);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public bool IsDefault => this == Default;
        
        public static (Vector3, float) Decompose(Vector3 v)
        {
            float num = v.Magnitude;
            if (num > 9.99999974737875E-06)
                return (v / num, num);
            return (zero, 0);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float GetDistance(in Vector3 v1, in Vector3 v2)
        {
            return (v1 - v2).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float GetSqrDistance(in Vector3 v1, in Vector3 v2)
        {
            return (v1 - v2).SqrLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float Dot(in Vector3 lhs, in Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 Scale(in Vector3 lhs, in Vector3 rhs)
        {
            return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static void Scale(in Vector3 scaler, ref Vector3 x, ref Vector3 y, ref Vector3 z)
        {
            x *= scaler.x;
            y *= scaler.y;
            z *= scaler.z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 Cross(in Vector3 lhs, in Vector3 rhs)
        {
            return new Vector3(
                (lhs.y * rhs.z - lhs.z * rhs.y),
                (lhs.z * rhs.x - lhs.x * rhs.z),
                (lhs.x * rhs.y - lhs.y * rhs.x));
        }

        public static Vector3 Normalize(Vector3 v)
        {
            float num = v.Magnitude;
            if (num > 9.99999974737875E-06)
                return v / num;
            return zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 Lerp(in Vector3 a, in Vector3 b, float t)
        {
            t = SharedHelpers.Clamp01(t);
            return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        }
        
        public static float SignedAngle(Vector3 fromNormalized, Vector3 toNormalized, Vector3 axis)
        {
            if ( SharedHelpers.CompareWithTol(fromNormalized.SqrMagnitude, 1f, SharedHelpers.EqualityType.Inequal, 0.001f)
              || SharedHelpers.CompareWithTol(toNormalized.SqrMagnitude,   1f, SharedHelpers.EqualityType.Inequal, 0.001f) )
            {
                Logger.IfError()?.Message($"{nameof(SignedAngle)} is called with not normalized vectors: from:{fromNormalized}, to:{toNormalized}").Write();
                return 0f;
            }

            return SignedAngleDangerousBuFast(fromNormalized, toNormalized, axis);
        }

        public static float SignedAngleDangerousBuFast(Vector3 fromNormalized, Vector3 toNormalized, Vector3 axis)
        {
            var angle = (float)Math.Acos(Dot(fromNormalized, toNormalized));
            return Dot(axis, Cross(fromNormalized, toNormalized)) >= 0 ? angle : -angle;
        }

        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 vector3 = target - current;
            float magnitude = vector3.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude < 1.40129846432482E-45)
                return target;
            return current + vector3 / magnitude * maxDistanceDelta;
        }
        
        public static Vector3 AlignToSector(Vector3 vector, int numberOfSectors, Vector3 up)
        {
            if (vector.magnitude < 0.01f)
                return zero;
            if (Math.Abs(vector.sqrMagnitude - 1) > 0.001f)
                vector = vector.Normalized;
            float sectorSize = SharedHelpers.DoublePi / numberOfSectors;
            float angle = SharedHelpers.Repeat(SignedAngle(forward, vector, up), SharedHelpers.DoublePi);
            angle = (float)Math.Round(angle / sectorSize) * sectorSize;
            return new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
        }
        
        [JsonIgnore] public Vector2 XZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Vector2(x, z);
        }

        [JsonIgnore] public Vector2 YZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Vector2(y, z);
        }

        [JsonIgnore] public Vector2 XY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Vector2(x, y);
        }

        [JsonIgnore] public Vector2 ZX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new Vector2(z, x);
        }

#if UNITY_5_3_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public Vector3(in UnityEngine.Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator UnityEngine.Vector3(in Vector3 v)
        {
            return new UnityEngine.Vector3(v.x, v.y, v.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator Vector3(in UnityEngine.Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator Vector3(in UnityEngine.Vector2 v)
        {
            return new Vector3(v.x, v.y, 0);
        }
#endif
        public bool Any(Predicate<float> pred) => pred(x) || pred(y) || pred(z);
        public bool All(Predicate<float> pred) => pred(x) && pred(y) && pred(z);

        // Not equal to $"({x:F3}, {y:F3}, {z:F3})" because string.Format is boxing arguments
        public override string ToString()
        {
            return $"({x.ToString("F3")}, {y.ToString("F3")}, {z.ToString("F3")})";
        }

        // Not equal to $"({x:F3}, {y:F3}, {z:F3})" because string.Format is boxing arguments
        public string ToShortString()
        {
            return $"({x.ToString("F0")},{y.ToString("F0")},{z.ToString("F0")})";
        }
        
        private static readonly Regex VectorRegex = new Regex($@"([\s\(\[\{{]|\s|^){SharedHelpers.FloatNumberRegex}\s*,?\s*{SharedHelpers.FloatNumberRegex}\s*,?\s*{SharedHelpers.FloatNumberRegex}([\)\\}}]]?|\s|$)");

        public static bool TryParse(string str, out Vector3 rv)
        {
            var match = VectorRegex.Match(str);
            if (match.Success)
            {
                //var culture = new CultureInfo("en-US");
                float x = (float) Convert.ChangeType(match.Groups[2].Value.Replace(",", "."), typeof(float), CultureInfo.InvariantCulture);
                float y = (float) Convert.ChangeType(match.Groups[6].Value.Replace(",", "."), typeof(float), CultureInfo.InvariantCulture);
                float z = (float) Convert.ChangeType(match.Groups[10].Value.Replace(",", "."), typeof(float), CultureInfo.InvariantCulture);
                rv = new Vector3(x, y, z);
                return true;
            }
            rv = zero;
            return false;
        }
        
        public static Vector3 Parse(string str)
        {
            if (TryParse(str, out var rv))
                return rv;
            throw new FormatException($"Is not a Vector3: {str}");
        }

    }
}
