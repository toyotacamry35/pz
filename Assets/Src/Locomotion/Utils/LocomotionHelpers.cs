using System.Runtime.CompilerServices;
using SharedCode.Utils;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers; 

namespace Src.Locomotion
{
    public static class LocomotionHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector WorldToLocomotionVector(in Vector3 v)
        {
            return new LocomotionVector(v.z, -v.x, v.y);
        }
        
#if UNITY_5_3_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector WorldToLocomotionVector(in UnityEngine.Vector3 v)
        {
            return new LocomotionVector(v.z, -v.x, v.y);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 LocomotionToWorldVector(in LocomotionVector v)
        {
            return new Vector3(-v.Horizontal.y, v.Vertical, v.Horizontal.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector3 LocomotionToWorldVector(in Vector2 v, float vertical = 0)
        {
            return new Vector3(-v.y, vertical, v.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float WorldToLocomotionVertical(in Vector3 v)
        {
            return v.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float WorldToLocomotionOrientationAngle(float angle)
        {
            return -angle * Mathf.Deg2Rad;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float LocomotionToWorldOrientationAngle(float angle)
        {
            return -angle * Mathf.Rad2Deg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float WorldToLocomotionOrientation(in Quaternion ornt)
        {
            
            return -GetEulerY(ornt);
        }
        
#if UNITY_5_3_OR_NEWER
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float WorldToLocomotionOrientation(in UnityEngine.Quaternion ornt)
        {
            return -GetEulerY((Quaternion)ornt);
        }
#endif
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Quaternion LocomotionToWorldOrientation(float angle)
        {
            angle *= -0.5f;
            return new Quaternion(0, Mathf.Sin(angle), 0, Mathf.Cos(angle));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 TransformMovementInputAxes(in Vector2 axes, in Vector2 guide)
        {
            return TransformMovementInputAxes(axes.x, axes.y, guide);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 LocomotionOrientationToForwardVector(float orientation)
        {
            return new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        }

        /// Перевод осей ввода в мировой вектор 
        public static Vector2 TransformMovementInputAxes(float lng, float lat, Vector2 guide)
        {
            //       Assert.AreApproximatelyEqual(forward.magnitude, 1);
           // Assert.IsTrue(lng >= -1 && lng <= 1);
           // Assert.IsTrue(lat >= -1 && lat <= 1);

            var left = guide.LeftPerpendicular();
            var direction = (guide * lng + left * lat);
            if (direction.sqrMagnitude > 1)
                direction = direction.normalized;
            return direction;

            /*
            if(Mathf.Approximately(lng, 0) && Mathf.Approximately(lat, 0))
                return Vector2.zero;

            if(Mathf.Approximately(lat, 0))
                return forward * lng;
            
            if(Mathf.Approximately(lng, 0))
                return right * lat;
            
            var len = LocomotionMath.EllipseRadiusAlongVector(Mathf.Abs(lng), Mathf.Abs(lat), new Vector2(lng, lat));
            var direction = (forward * lng + right * lat).normalized * len;
            return direction;
            */
        }

        /// Перевод осей ввода в локальный вектор в СК персонажа 
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 TransformMovementInputAxesToLocal(Vector2 axes, Vector2 guide, Vector2 forward)
        {
            var inputWorld = TransformMovementInputAxes(axes.x, axes.y, guide);
            var inputLocal = LocomotionHelpers.InverseTransformVector(inputWorld, forward);
            return inputLocal;
        }

        //Translates world vec. `v` into local coordinate system set by `forward` vec.
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 InverseTransformVector(in Vector2 v, Vector2 forward)
        {
            var side = forward.LeftPerpendicular();
            var lng = Vector2.Dot(v, forward);
            var lat = Vector2.Dot(v, side);
            return new Vector2(lng, lat);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static Vector2 InverseTransformVector(Vector2 v, float orientationAngle)
        {
            var direction = new Vector2(Mathf.Cos(orientationAngle), Mathf.Sin(orientationAngle));
            return InverseTransformVector(v, direction);
        }

        public static float MovingTime(float distance, float velocity, float accel, float tolerance = LocomotionMath.DefaultTolerance)
        {
            if (distance.ApproximatelyZero(tolerance))
                return 0;

            tolerance *= tolerance;

            if (accel.ApproximatelyZero(tolerance))
            {
                if (velocity.ApproximatelyZero(tolerance))
                    return float.PositiveInfinity;
                var t = distance / velocity;
                if (t < 0)
                    return float.PositiveInfinity;
                return t;
            }

            var d = velocity * velocity + 2 * accel * distance;
            if (d < -tolerance)
                return float.PositiveInfinity;
            if (d < 0)
                d = 0;
            d = Mathf.Sqrt(d);
            var t1 = (-velocity + d) / accel;
            var t2 = (-velocity - d) / accel;
            if (t1 < 0 && t2 < 0)
                return float.PositiveInfinity;
            if (t1 < 0)
                return t2;
            if (t2 < 0)
                return t1;
            return Mathf.Min(t1, t2);
        }

        // Вектор касательной к поверхности в заданном направлении.
        internal static LocomotionVector SurfaceTangent(in LocomotionVector normal, Vector2 dir)
        {
            return LocomotionVector.Cross(normal, dir.LeftPerpendicular()).Normalized;
        }

        // Вектор касательной к поверхности в заданном направлении.
        internal static LocomotionVector SurfaceTangent(float slopeSin, Vector2 dir)
        {
            var slopeCos = Mathf.Sqrt(1 - slopeSin * slopeSin); 
            return new LocomotionVector(dir * slopeCos, -slopeSin);
        }

        private static float GetEulerY(in Quaternion rotation)
        {
            float sqw = rotation.w * rotation.w;
            float sqx = rotation.x * rotation.x;
            float sqy = rotation.y * rotation.y;
            float sqz = rotation.z * rotation.z;
            float unit = sqx + sqy + sqz + sqw; 
            float test = rotation.x * rotation.w - rotation.y * rotation.z;
            float y;

            if (test > 0.4995f * unit)
                y = 2f * Mathf.Atan2(rotation.y, rotation.x);
            else
            if (test < -0.4995f * unit)
                y = -2f * Mathf.Atan2(rotation.y, rotation.x);
            else
            {
                Quaternion q = new Quaternion(rotation.w, rotation.z, rotation.x, rotation.y);
                y = Mathf.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));
            }
            return Mathf.Repeat(y, Mathf.DoublePi);
        }
    }
}