using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SharedCode.Utils;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public readonly struct LocomotionVector
    {
        public readonly Vector2 Horizontal;
        public readonly float Vertical;

        [JsonIgnore]
        public float Magnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Sqrt(Horizontal.x * Horizontal.x + Horizontal.y * Horizontal.y + Vertical * Vertical);
        }

        [JsonIgnore]
        public float SqrMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Horizontal.x * Horizontal.x + Horizontal.y * Horizontal.y + Vertical * Vertical;
        }

        [JsonIgnore]
        public LocomotionVector Normalized
        {
            get
            {
                float num = Magnitude;
                if (num <= 9.99999974737875E-06)
                    return Zero;
                float mul = 1 / num;
                return new LocomotionVector(Horizontal * mul, Vertical * mul);
            }
        }

        [JsonIgnore]
        public bool IsNan => float.IsNaN(Horizontal.x) || float.IsNaN(Horizontal.y) || float.IsNaN(Vertical);

        public static readonly LocomotionVector Invalid = new LocomotionVector(float.MinValue, float.MinValue, float.MinValue);
        
        public static readonly LocomotionVector Zero = new LocomotionVector(0,0,0);

        public static readonly LocomotionVector Up = new LocomotionVector(0,0,1);

        public static readonly LocomotionVector Forward = new LocomotionVector(1,0,0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public LocomotionVector(in Vector2 horizontal)
        {
            Horizontal = horizontal;
            Vertical = 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public LocomotionVector(in Vector2 horizontal, float vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public LocomotionVector(float x, float y, float vertical)
        {
            Horizontal = new Vector2(x,y);
            Vertical = vertical;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static implicit operator LocomotionVector(in Vector2 horizontal)
        {
            return new LocomotionVector(horizontal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static float Dot(in LocomotionVector lhs, in LocomotionVector rhs)
        {
            return Vector2.Dot(lhs.Horizontal, rhs.Horizontal) + lhs.Vertical * rhs.Vertical;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector Cross(in LocomotionVector lhs, in LocomotionVector rhs)
        {
            return new LocomotionVector(
                lhs.Horizontal.y * rhs.Vertical - lhs.Vertical * rhs.Horizontal.y,
                lhs.Vertical * rhs.Horizontal.x - lhs.Horizontal.x * rhs.Vertical,
                lhs.Horizontal.x * rhs.Horizontal.y - lhs.Horizontal.y * rhs.Horizontal.x);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public LocomotionVector SetVertical(float v)
        {
            return new LocomotionVector(Horizontal, v);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector operator +(in LocomotionVector a, in LocomotionVector b)
        {
            return new LocomotionVector(a.Horizontal + b.Horizontal, a.Vertical + b.Vertical);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector operator -(in LocomotionVector a, in LocomotionVector b)
        {
            return new LocomotionVector(a.Horizontal - b.Horizontal, a.Vertical - b.Vertical);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector operator -(in LocomotionVector a)
        {
            return new LocomotionVector(-a.Horizontal, -a.Vertical);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector operator *(in LocomotionVector a, float d)
        {
            return new LocomotionVector(a.Horizontal * d, a.Vertical * d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector operator *(float d, in LocomotionVector a)
        {
            return new LocomotionVector(a.Horizontal * d, a.Vertical * d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector operator /(in LocomotionVector a, float d)
        {
            return new LocomotionVector(a.Horizontal / d, a.Vertical / d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public bool ExactlyEqualsTo(in LocomotionVector v)
        {
            return v.Horizontal.x == Horizontal.x && v.Horizontal.y == Horizontal.y && v.Vertical == Vertical;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator ==(in LocomotionVector lhs, in LocomotionVector rhs)
        {
            return (lhs - rhs).SqrMagnitude < 9.99999943962493E-11;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static bool operator !=(in LocomotionVector lhs, in LocomotionVector rhs)
        {
            return !(lhs == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector Lerp(in LocomotionVector a, in LocomotionVector b, float t)
        {
            t = Clamp01(t);
            return new LocomotionVector(
                a.Horizontal.x + (b.Horizontal.x - a.Horizontal.x) * t, 
                a.Horizontal.y + (b.Horizontal.y - a.Horizontal.y) * t, 
                a.Vertical + (b.Vertical - a.Vertical) * t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static LocomotionVector LerpUnclamped(in LocomotionVector a, in LocomotionVector b, float t)
        {
            return new LocomotionVector(
                a.Horizontal.x + (b.Horizontal.x - a.Horizontal.x) * t, 
                a.Horizontal.y + (b.Horizontal.y - a.Horizontal.y) * t, 
                a.Vertical + (b.Vertical - a.Vertical) * t);
        }
        
        public static LocomotionVector SmoothDamp(LocomotionVector current, LocomotionVector target, ref LocomotionVector currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Max(0.0001f, smoothTime);
            float num1 = 2f / smoothTime;
            float num2 = num1 * deltaTime;
            float num3 = 1.0f / (1.0f + num2 + 0.479999989271164f * num2 * num2 + 0.234999999403954f * num2 * num2 * num2);
            var vector = current - target;
            var vector3_1 = target;
            float maxLength = maxSpeed * smoothTime;
            var vector3_2 = vector.Clamp(maxLength);
            target = current - vector3_2;
            var vector3_3 = (currentVelocity + num1 * vector3_2) * deltaTime;
            currentVelocity = (currentVelocity - num1 * vector3_3) * num3;
            var vector3_4 = target + (vector3_2 + vector3_3) * num3;
            if (Dot(vector3_1 - current, vector3_4 - vector3_1) > 0.0)
            {
                vector3_4 = vector3_1;
                currentVelocity = (vector3_4 - vector3_1) / deltaTime;
            }
            return vector3_4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Vector3 ToWorld() => LocomotionHelpers.LocomotionToWorldVector(this);

        public override string ToString()
        {
            return $"({Horizontal.x:F2} , {Horizontal.y:F2} ; {Vertical:F2})";
        }
        public string ToStringVerbose()
        {
            var w = LocomotionHelpers.LocomotionToWorldVector(this);
            return $"(Hx:{Horizontal.x:F2} , Hy:{Horizontal.y:F2} ; V:{Vertical:F2} | Wx:{w.x}, Wy:{w.y}, Wz:{w.z})";
        }
    }
}