using UnityEngine;

namespace Src.Locomotion.Unity
{
    public static class VectorUtils
    {
        public static (Vector3,float) Normalized(this Vector3 v)
        {
            var magnitude = v.magnitude;
            return magnitude > 9.99999974737875E-06 ? (v / magnitude, magnitude) : (Vector3.zero, 0);
        }
        
        public static float Normalize(ref Vector3 v)
        {
            var magnitude = v.magnitude;
            if (magnitude > 9.99999974737875E-06)
                v = v / magnitude;
            return 0;
        }
        
        public static (Vector2,float) Normalized(this Vector2 v)
        {
            var magnitude = v.magnitude;
            return magnitude > 9.99999974737875E-06 ? (v / magnitude, magnitude) : (Vector2.zero, 0);
        }

        public static Vector3 SetX(this Vector3 v3, float x)
        {
            return new Vector3(x, v3.y, v3.z);
        }
    
        public static Vector3 SetY(this Vector3 v3, float y)
        {
            return new Vector3(v3.x, y, v3.z);
        }

        public static Vector3 SetZ(this Vector3 v3, float z)
        {
            return new Vector3(v3.x, v3.y, z);
        }

        public static Vector3 AddX(this Vector3 v3, float x)
        {
            return new Vector3(v3.x + x, v3.y, v3.z);
        }
    
        public static Vector3 AddY(this Vector3 v3, float y)
        {
            return new Vector3(v3.x, v3.y + y, v3.z);
        }

        public static Vector3 AddZ(this Vector3 v3, float z)
        {
            return new Vector3(v3.x, v3.y, v3.z + z);
        }
        
        public static bool ApproximatelyEqual(this Vector2 a, Vector2 b, float tolerance = LocomotionMath.DefaultTolerance)
        {
            return (b - a).sqrMagnitude < tolerance * tolerance;
        }

        public static bool ApproximatelyEqual(this Vector3 a, Vector3 b, float tolerance = LocomotionMath.DefaultTolerance)
        {
            return (b - a).sqrMagnitude < tolerance * tolerance;
        }
        
        public static bool ApproximatelyEqual(this Quaternion a, Quaternion b, float tolerance = LocomotionMath.DefaultTolerance)
        {
            return Quaternion.Dot(b, a) > (1 - tolerance);
        }
    }
}