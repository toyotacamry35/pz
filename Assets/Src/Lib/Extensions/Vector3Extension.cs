using GeneratedDefsForSpells;
using UnityEngine;

namespace Assets.Src.Lib.Extensions
{
    public static class Vector3Extension
    {
        public static float GetVectorComponent(this Vector3 v, XyzEnum component)
        {
            return component == XyzEnum.X
                ? v.x
                : (component == XyzEnum.Y
                    ? v.y
                    : v.z);
        }

        // ReSharper disable once InconsistentNaming
        public static Vector2 ToVector2ZX(this Vector3 v)
        {
            return new Vector2(v.z, v.x);
        }

        // ReSharper disable once InconsistentNaming
        public static Vector2 ToVector2XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

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

        public static bool IsNan(this Vector3 v) => float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);

        public static float MaxComponent(this Vector3 v) => Mathf.Max(Mathf.Max(v.x, v.y), v.z);
        
        public static float MinComponent(this Vector3 v) => Mathf.Min(Mathf.Min(v.x, v.y), v.z);
        
        public static bool Approx3(Vector3 v, Vector3 b, float delta = 0.0001f)
        {
            return Mathf.Abs(v.x - b.x) < delta && Mathf.Abs(v.y - b.y) < delta && Mathf.Abs(v.z - b.z) < delta;
        }
    }

    // --- Internal types: --------------------------------------------------------------

    public enum XyzEnum : byte
    {
        X,
        Y,
        Z
    }

    public static class Vector3SharedExtension
    {
        public static Vector3 ToUnityVector3(this SharedCode.Utils.Vector3 v3Shared) => new Vector3(v3Shared.x, v3Shared.y, v3Shared.z);
        public static Vector3Int ToUnityVector3Int(this SharedCode.Utils.Vector3Int v3Shared) => new Vector3Int(v3Shared.x, v3Shared.y, v3Shared.z);
    }
}
