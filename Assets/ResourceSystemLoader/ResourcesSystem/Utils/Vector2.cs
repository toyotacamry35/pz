using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Assets.ColonyShared.SharedCode.Utils;
using ProtoBuf;
using ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Newtonsoft.Json;

namespace SharedCode.Utils
{
    [ProtoContract]
    [KnownToGameResources]
    public partial struct Vector2 : IHasRandomFill
    {
        [ProtoMember(1)]
        public float x;
        [ProtoMember(2)]
        public float y;
        
        [ProtoIgnore]
        public static readonly Vector2 zero = new Vector2(0.0f, 0.0f);
        [ProtoIgnore]
        public static readonly Vector2 up = new Vector2(0.0f, 1.0f);
        [JsonIgnore]
        public Vector3 AsXZ => new Vector3(x, 0, y);
        [JsonIgnore]
        public bool IsNan => float.IsNaN(x) || float.IsNaN(y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public Vector2(in Vector2 v) : this()
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
            if (!(obj is Vector2))
            {
                return false;
            }
            return this == (Vector2)obj;
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (float) Math.Sqrt(x * x + y * y);
        }

        [JsonIgnore]
        public float magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (float) Math.Sqrt(x * x + y * y);
        }

        [JsonIgnore]
        public float SqrMagnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)]  get => x * x + y * y; }

        [JsonIgnore]
        public float sqrMagnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => x * x + y * y; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 operator *(in Vector2 v, float scale)
        {
            return new Vector2(v.x * scale, v.y * scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 operator /(in Vector2 v, float scale)
        {
            return new Vector2(v.x / scale, v.y / scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 operator + (in Vector2 v1, in Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 operator - (in Vector2 v1, in Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 operator - (in Vector2 v1)
        {
            return new Vector2(-v1.x , -v1.y);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(in Vector2 v1, in Vector2 v2)
        {
            return (v1 - v2).Magnitude > 1E-05F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(in Vector2 v1, in Vector2 v2)
        {
            return !(v1 != v2);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float SqrLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => x * x + y * y;
        }

        [JsonIgnore]
        [ProtoIgnore]
        public float Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => (float) Math.Sqrt(SqrLength);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public Vector2 Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Normalize(this);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public (Vector2,float) Decomposed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Decompose(this);
        }

        [JsonIgnore]
        [ProtoIgnore]
        public Vector2 normalized { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Normalize(this); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float GetDistance(in Vector2 v1, in Vector2 v2)
        {
            return (v1 - v2).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float GetSqrDistance(in Vector2 v1, in Vector2 v2)
        {
            return (v1 - v2).SqrLength;
        }
             
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float Dot(in Vector2 lhs, in Vector2 rhs)
        {
            return (lhs.x * rhs.x + lhs.y * rhs.y);
        }
        
        public static Vector2 Normalize(Vector2 v)
        {
            float num = v.Magnitude;
            if (num > 9.99999974737875E-06)
                return v / num;
            return zero;
        }

        public static (Vector2, float) Decompose(Vector2 v)
        {
            float num = v.Magnitude;
            if (num > 9.99999974737875E-06)
                return (v / num, num);
            return (zero, 0);
        }
        
        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            Vector2 vector2 = target - current;
            float magnitude = vector2.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0.0)
                return target;
            return current + vector2 / magnitude * maxDistanceDelta;
        }
        
        public static Vector2 AlignToSector(Vector2 vector, int numberOfSectors)
        {
            if (vector.magnitude < 0.01f)
                return zero;
            if (Math.Abs(vector.sqrMagnitude - 1) > 0.001f)
                vector = vector.Normalized;
            float sectorSize = SharedHelpers.DoublePi / numberOfSectors;
            float angle = SharedHelpers.Repeat(SharedHelpers.Atan2(vector.y, vector.x), SharedHelpers.DoublePi);
            angle = (float)Math.Round(angle / sectorSize) * sectorSize;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public override string ToString()
        {
            return $"({x:F2}, {y:F2})";
        }

        public string ToString(string fmt)
        {
            // Not equal to $"({x:F2}, {y:F2})" because string.Format is boxing arguments
            return $"({x.ToString("F2")}, {y.ToString("F2")})";
        }
        public Vector2 RotateDeg(float deg)
        {
            return RotateRad(deg * Assets.ColonyShared.SharedCode.Utils.MathF.Deg2Rad);
        }

        public Vector2 RotateRad(float radians)
        {
            float sin = (float)Math.Sin(radians);
            float cos = (float)Math.Cos(radians);

            return new Vector2(cos * x - sin * y, sin * x + cos * y);
        }
#if UNITY_5_3_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public Vector2(in UnityEngine.Vector2 v)
        {
            x = v.x;
            y = v.y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator UnityEngine.Vector2(in Vector2 v)
        {
            return new UnityEngine.Vector2(v.x, v.y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator UnityEngine.Vector3(in Vector2 v)
        {
            return new UnityEngine.Vector3(v.x, v.y, 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static explicit operator Vector2(in UnityEngine.Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }
       

#endif
        public const float kEpsilon = 0.00001F;
        public const float kEpsilonNormalSqrt = 1e-15f;
        public static float Angle(Vector2 from, Vector2 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = Assets.ColonyShared.SharedCode.Utils.MathF.Clamp(Dot(from, to) / denominator, -1F, 1F);
            return (float)Math.Acos(dot) * Assets.ColonyShared.SharedCode.Utils.MathF.Rad2Deg;
        }
        
          
        private static readonly Regex Vector2Regex = new Regex($@"([\s\(\[\{{]|\s|^){SharedHelpers.FloatNumberRegex}\s*,?\s*{SharedHelpers.FloatNumberRegex}\s*([\)\\}}]]?|\s|$)");

        public static bool TryParse(string str, out Vector2 rv)
        {
            var match = Vector2Regex.Match(str);
            if (match.Success)
            {
                //var culture = new CultureInfo("en-US");
                float x = (float) Convert.ChangeType(match.Groups[2].Value.Replace(",", "."), typeof(float), CultureInfo.InvariantCulture);
                float y = (float) Convert.ChangeType(match.Groups[6].Value.Replace(",", "."), typeof(float), CultureInfo.InvariantCulture);
                rv = new Vector2(x, y);
                return true;
            }
            rv = zero;
            return false;
        }
        
        public static Vector2 Parse(string str)
        {
            if (TryParse(str, out var rv))
                return rv;
            throw new FormatException($"Is not a Vector2: {str}");
        }
    }
}
