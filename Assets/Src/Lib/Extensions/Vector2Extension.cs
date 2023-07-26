using UnityEngine;

namespace Assets.Src.Lib.Extensions
{
    public static class Vector2Extension
    {
        public static Vector2 RotateDeg(this Vector2 v, float deg)
        {
            return RotateRad(v, deg * Mathf.Deg2Rad);
        }

        // source: (https://answers.unity.com/answers/734946/view.html)
        public static Vector2 RotateRad(this Vector2 v, float radians)
        {
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
        }

        // ReSharper disable once InconsistentNaming
        public static Vector3 Vector2ZXToVector3(this Vector2 v) => new Vector3(v.y, 0, v.x);

        public static Vector3 Vector2ZXToVector3(this Vector2 v, float y) => new Vector3(v.y, y, v.x);
        
        // ReSharper disable once InconsistentNaming
        public static Vector3 Vector2XZToVector3(this Vector2 v) => new Vector3(v.x, 0, v.y);

        public static Vector3 Vector2XZToVector3(this Vector2 v, float y) => new Vector3(v.x, y, v.y);
        
        public static Vector2 Normalized(this Vector2 v, out float magnitude)
        {
            magnitude = v.magnitude;
            if (magnitude > 9.99999974737875E-06)
                return v / magnitude;
            magnitude = 0;
            return Vector2.zero;
        }
        
        public static float Normalize(ref Vector2 v)
        {
            var magnitude = v.magnitude;
            if (magnitude > 9.99999974737875E-06)
                v = v / magnitude;
            return 0;
        }

        public static Vector2 SetX(this Vector2 v2, float x)
        {
            return new Vector2(x, v2.y);
        }
    
        public static Vector2 SetY(this Vector2 v2, float y)
        {
            return new Vector2(v2.x, y);
        }
    }
}
