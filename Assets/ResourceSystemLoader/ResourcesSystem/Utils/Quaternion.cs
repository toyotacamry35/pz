using System;
using Assets.ColonyShared.SharedCode.Utils;
using ResourcesSystem.Loader;
using ProtoBuf;
using ResourcesSystem.Base;

namespace SharedCode.Utils
{
    [KnownToGameResources]
    [ProtoContract]
    public struct Quaternion : IHasRandomFill
    {
        [ProtoMember(1)]
        public float x;
        [ProtoMember(2)]
        public float y;
        [ProtoMember(3)]
        public float z;
        [ProtoMember(4)]
        public float w;

        [ProtoIgnore]
        public static Quaternion identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        [ProtoIgnore]
        public static readonly Quaternion Default = default(Quaternion);

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Vector3 vec, float scalar) : this()
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
            w = scalar;
        }

        public Quaternion(Quaternion q) : this()
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;

        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            x = random.Next(100);
            y = random.Next(100);
            z = random.Next(100);
            w = random.Next(100);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Quaternion))
            {
                return false;
            }

            return this == (Quaternion)obj;
        }

        public override int GetHashCode()
        {
            var hashCode = -1743314642;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            hashCode = hashCode * -1521134295 + w.GetHashCode();
            return hashCode;
        }

        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            return Math.Sqrt(Math.Pow(q1.x - q2.x, 2) + Math.Pow(q1.y - q2.y, 2) + Math.Pow(q1.z - q2.z, 2) + Math.Pow(q1.w - q2.w, 2)) > 1E-06F;
        }

        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            return !(q1 != q2);
        }

        //https://gamedev.stackexchange.com/questions/28395/rotating-vector3-by-a-quaternion
        public static Vector3 operator *(Quaternion q, Vector3 v)
        {
            // Extract the vector part of the quaternion
            var u = new Vector3(q.x, q.y, q.z);

            // Extract the scalar part of the quaternion
            float s = q.w;

            // Do the math
            return 2.0f * Vector3.Dot(u, v) * u
                 + (s * s - Vector3.Dot(u, u)) * v
                 + 2.0f * s * Vector3.Cross(u, v);
        }

        //https://referencesource.microsoft.com/#System.Numerics/System/Numerics/Quaternion.cs,475
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            Quaternion ans;

            float q1x = q1.x;
            float q1y = q1.y;
            float q1z = q1.z;
            float q1w = q1.w;

            float q2x = q2.x;
            float q2y = q2.y;
            float q2z = q2.z;
            float q2w = q2.w;

            // cross(av, bv)
            float cx = q1y * q2z - q1z * q2y;
            float cy = q1z * q2x - q1x * q2z;
            float cz = q1x * q2y - q1y * q2x;

            float dot = q1x * q2x + q1y * q2y + q1z * q2z;

            ans.x = q1x * q2w + q2x * q1w + cx;
            ans.y = q1y * q2w + q2y * q1w + cy;
            ans.z = q1z * q2w + q2z * q1w + cz;
            ans.w = q1w * q2w - dot;

            return ans;
        }

        public Vector3 eulerAngles => ToEuler(this);

        public Vector3 eulerAnglesRad => ToEulerRad(this);

        public static Quaternion Euler(float x, float y, float z)
        {
            return EulerRad(x * SharedHelpers.Deg2Rad, y * SharedHelpers.Deg2Rad, z * SharedHelpers.Deg2Rad);
        }

        public static Quaternion Euler(Vector3 v)
        {
            return EulerRad(v * SharedHelpers.Deg2Rad);
        }

        public static Quaternion EulerRad(Vector3 euler)
        {
            return EulerRad(euler.x, euler.y, euler.z);
        }

        public static float SquaredNorm(Quaternion q)
        {
            return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
        }

        public static Quaternion Inverse(Quaternion q)
        {
            var qSqrNorm = Quaternion.SquaredNorm(q);
            return new Quaternion(-q.x / qSqrNorm, -q.y / qSqrNorm, -q.z / qSqrNorm, q.w / qSqrNorm);
        }

        public static Quaternion EulerRad(float x, float y, float z)
        {
            double yawOver2 = x * 0.5f;
            float cosYawOver2 = (float)Math.Cos(yawOver2);
            float sinYawOver2 = (float)Math.Sin(yawOver2);
            double pitchOver2 = y * 0.5f;
            float cosPitchOver2 = (float)Math.Cos(pitchOver2);
            float sinPitchOver2 = (float)Math.Sin(pitchOver2);
            double rollOver2 = z * 0.5f;
            float cosRollOver2 = (float)Math.Cos(rollOver2);
            float sinRollOver2 = (float)Math.Sin(rollOver2);            
            Quaternion result;
            result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.x = sinYawOver2 * cosPitchOver2 * cosRollOver2 + cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.y = cosYawOver2 * sinPitchOver2 * cosRollOver2 - sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
            return result;
        }

        public static Vector3 ToEuler(Quaternion rotation)
        {
            return ToEulerRad(rotation) * SharedHelpers.Rad2Deg;
        }

        public static Vector3 ToEulerRad(Quaternion rotation)
        {
            float sqw = rotation.w * rotation.w;
            float sqx = rotation.x * rotation.x;
            float sqy = rotation.y * rotation.y;
            float sqz = rotation.z * rotation.z;
            float unit = sqx + sqy + sqz + sqw; 
            float test = rotation.x * rotation.w - rotation.y * rotation.z;
            Vector3 v;

            if (test > 0.4995f * unit)
            {
                v.y = 2f * (float)Math.Atan2(rotation.y, rotation.x);
                v.x = SharedHelpers.HalfPi;
                v.z = 0;
            }
            else
            if (test < -0.4995f * unit)
            {
                v.y = -2f * (float)Math.Atan2(rotation.y, rotation.x);
                v.x = -SharedHelpers.HalfPi;
                v.z = 0;
            }
            else
            {
                Quaternion q = new Quaternion(rotation.w, rotation.z, rotation.x, rotation.y);
                v.y = (float) Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));
                v.x = (float) Math.Asin(2f * (q.x * q.z - q.w * q.y));
                v.z = (float) Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));
            }
            return NormalizeAnglesRad(v);
        }
      
        private static Vector3 NormalizeAnglesRad(Vector3 angles)
        {
            angles.x = SharedHelpers.NormalizeAngleRad(angles.x);
            angles.y = SharedHelpers.NormalizeAngleRad(angles.y);
            angles.z = SharedHelpers.NormalizeAngleRad(angles.z);
            return angles;
        }

#if UNITY_5_3_OR_NEWER
        public Quaternion(UnityEngine.Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public static explicit operator UnityEngine.Quaternion(Quaternion q)
        {
            return new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
        }
        public static explicit operator Quaternion(UnityEngine.Quaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }
#endif

        public override string ToString()
        {
            return $"({x:F3}, {y:F3}, {z:F3}, {w:F3})";
        }
    }
}
