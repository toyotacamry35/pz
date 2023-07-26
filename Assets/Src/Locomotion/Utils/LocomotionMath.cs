using System;
using System.Runtime.CompilerServices;
using Assets.ColonyShared.SharedCode.Utils;
using Vector2 = SharedCode.Utils.Vector2;
using Vector3 = SharedCode.Utils.Vector3;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers; 

namespace Src.Locomotion
{
    public static class LocomotionMath
    {
        public const float PI = SharedHelpers.Pi;
        public const float DoublePI = PI * 2;
        public const float HalfPI = PI * 0.5f;
        public const float DefaultTolerance = 0.001f;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool ApproximatelyEqual(this float a, float b, float tolerance = DefaultTolerance)
        {
            return Math.Abs(b - a) < tolerance;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool ApproximatelyEqual(this Vector2 a, Vector2 b, float tolerance = DefaultTolerance)
        {
            return (b - a).sqrMagnitude < tolerance * tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool ApproximatelyEqual(this Vector3 a, Vector3 b, float tolerance = DefaultTolerance)
        {
            return (b - a).sqrMagnitude < tolerance * tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool ApproximatelyEqual(this in LocomotionVector a, in LocomotionVector b, float tolerance = DefaultTolerance)
        {
            return (b - a).SqrMagnitude < tolerance * tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool ApproximatelyZero(this float val, float tolerance = DefaultTolerance)
        {
            return Math.Abs(val) < tolerance;            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool ApproximatelyZero(this Vector2 val, float tolerance = DefaultTolerance)
        {
            return ApproximatelyZero(val.sqrMagnitude, tolerance * tolerance);            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool ApproximatelyZero(this in LocomotionVector val, float tolerance = DefaultTolerance)
        {
            return ApproximatelyZero(val.SqrMagnitude, tolerance * tolerance);            
        }
        
        public static bool IsNormalized(this Vector2 val, float tolerance = DefaultTolerance)
        {
            var mag = val.sqrMagnitude;
            tolerance *= tolerance;
            return ApproximatelyEqual(mag, 1, tolerance) || ApproximatelyZero(mag, tolerance);
        }

        public static bool IsNormalized(this Vector3 val, float tolerance = DefaultTolerance)
        {
            var mag = val.sqrMagnitude;
            tolerance *= tolerance;
            return ApproximatelyEqual(mag, 1, tolerance) || ApproximatelyZero(mag, tolerance);
        }

        public static bool IsNormalized(this LocomotionVector val, float tolerance = DefaultTolerance)
        {
            var mag = val.SqrMagnitude;
            tolerance *= tolerance;
            return ApproximatelyEqual(mag, 1, tolerance) || ApproximatelyZero(mag, tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool Longer(this Vector2 val, float threshold)
        {
            return val.sqrMagnitude > threshold * threshold;            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool Shorter(this Vector2 val, float threshold)
        {
            return val.sqrMagnitude < threshold * threshold;            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 Threshold(this Vector2 val, float threshold)
        {
            return val.sqrMagnitude > threshold * threshold ? val : Vector2.zero;            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 Clamp(this Vector2 val, float maxLength)
        {
            return val.sqrMagnitude < maxLength * maxLength ? val : val.normalized * maxLength;            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 Step(this Vector2 val, float threshold)
        {
            return val.sqrMagnitude > threshold * threshold ? val.normalized : Vector2.zero;            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool Longer(this in LocomotionVector val, float threshold)
        {
            return val.SqrMagnitude > threshold * threshold;            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool Shorter(this in LocomotionVector val, float threshold)
        {
            return val.SqrMagnitude < threshold * threshold;            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector Threshold(this in LocomotionVector self, float threshold)
        {
            return self.SqrMagnitude > threshold * threshold ? self : LocomotionVector.Zero;            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector Clamp(this in LocomotionVector vector, float maxLength)
        {
            var sqrMagnitude = vector.SqrMagnitude; 
            if (sqrMagnitude > maxLength * maxLength)
                return vector / Mathf.Sqrt(sqrMagnitude) * maxLength;
            return vector;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float Sqr(this float v)
        {
            return v * v;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 LeftPerpendicular(this in Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 RightPerpendicular(this in Vector2 v)
        {
            return new Vector2(v.y, -v.x);
        }

        public static Vector2 MoveTowards(this Vector2 current, Vector2 target, float speed, float deltaTime)
        {
            return Vector2.MoveTowards(current, target, speed * deltaTime);
        }

        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float speed, float deltaTime)
        {
            return Vector3.MoveTowards(current, target, speed * deltaTime);
        }

        public static LocomotionVector MoveTowards(this LocomotionVector current, LocomotionVector target, float speed, float deltaTime)
        {
            var delta = speed * deltaTime;
            var v1 = new Vector3(current.Horizontal.x, current.Horizontal.y, current.Vertical);
            var v2 = new Vector3(target.Horizontal.x, target.Horizontal.y, target.Vertical);
            var res = Vector3.MoveTowards(v1, v2, delta);
            return new LocomotionVector(res.x, res.y, res.z);
        }

        public static float CLerpRad(float a, float b, float c, float angle)
        {
            var f = Mathf.Abs(Mathf.DeltaAngleRad(0, angle)) / HalfPI; // -> [0,2]
            if (f < 1)
                return Mathf.Lerp(a, b, f);
            return Mathf.Lerp(b, c, f - 1);
        }
    }
}
