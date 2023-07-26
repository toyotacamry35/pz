using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Utils
{
    /// <summary>
    /// Helper Functions
    /// </summary>
    public static class SharedHelpers
    {
        [NotNull] internal static readonly NLog.Logger Logger = LogManager.GetLogger("SharedHelpers");

        public const int InvalidIndex = -1;
        public const float HalfPi = 1.57079632679489661923f; //pi/2
        public const float Pi = 3.14159265358979f;
        public const float DoublePi = Pi * 2;
        // Degrees-to-radians conversion constant (Read Only).
        public const float Deg2Rad = 0.0174532924F;
        // Radians-to-degrees conversion constant (Read Only).
        public const float Rad2Deg = 57.29578F;
        public static volatile float FloatMinNormal = 1.175494E-38f;
        public static volatile float FloatMinDenormal = float.Epsilon;
        public static bool IsFlushToZeroEnabled = FloatMinDenormal == 0.0;
        public static readonly float Epsilon = !IsFlushToZeroEnabled ? FloatMinDenormal : FloatMinNormal;
        public static string FloatNumberRegex = @"([+-]?((\d+[\.,]?\d*)|([\.,]\d+)))";

    /// Math ///
    #region Math

        public static bool IsOdd(int  i) => i % 2f != 0f;
        public static bool IsOdd(uint u) => u % 2f != 0f;
        public static bool IsEven(int  i) => !IsOdd(i);
        public static bool IsEven(uint u) => !IsOdd(u);

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static long Min(long a, long b) => a < b ? a : b;

        public static long Max(long a, long b) => a > b ? a : b;

        public static T Max<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        public static T Min<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) < 0 ? a : b;
        }

        public static T Max<T>(T a, T b, T c) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0
                ? Max(a, c)
                : Max(b, c);
        }

        public static T Max<T>(T a, T b, T c, T d) where T : IComparable<T>
        {
            return Max(Max(a, b), Max(c, d));
        }

        public static T Min<T>(T a, T b, T c, T d) where T : IComparable<T>
        {
            return Min(Min(a, b), Min(c, d));
        }

        public static void MinMax<T>(T a, T b, out T min, out T max) where T : IComparable<T>
        {
            if (a.CompareTo(b) > 0)
            {
                min = b;
                max = a;
            }
            else
            {
                min = a;
                max = b;
            }
        }

        // Including bounds:
        public static bool InRange<T>(T val, T min, T max) where T : IComparable<T>
        {
            return !OutOfRange(val, min, max);
        }
        public static bool InRangeFast<T>(T val, T min, T max) where T : IComparable<T>
        {
            return !OutOfRangeFast(val, min, max);
        }
        public static bool OutOfRange<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0)
            {
                Logger.IfError()?.Message($"Passed min({min}) > max({max})!").Write();
                return false;
            }

            return OutOfRangeFast(val, min, max);
        }
        public static bool OutOfRangeFast<T>(T val, T min, T max) where T : IComparable<T>
        {
            return (val.CompareTo(min) < 0)
                || (val.CompareTo(max) > 0);
        }

        // `min` > `max` is not exception:
        // F.e. time between 20:00 & 04:00 handles properly --> min 20, max 4.
        public static bool InRangeCycled<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) <= 0)
                return (val.CompareTo(min) >= 0)
                    && (val.CompareTo(max) <= 0);
            else
                return (val.CompareTo(min) >= 0)
                    || (val.CompareTo(max) <= 0);
        }

        private const float CompareDfltTol = 0.01f; //0.001f;
        public static bool CompareWithTol(float a, float b, EqualityType type, float tolerance = CompareDfltTol)
        {
            float diff = a - b;
            switch (type)
            {
                case EqualityType.Equal:          return diff <= tolerance && diff >= -tolerance;
                case EqualityType.Greater:        return diff > tolerance;
                case EqualityType.Less:           return diff < -tolerance;
                case EqualityType.GreaterOrEqual: return diff >= -tolerance;
                case EqualityType.LessOrEqual:    return diff <= tolerance;
                case EqualityType.Inequal:        return diff > tolerance || diff < -tolerance;
                default:
                    Logger.IfError()?.Message("Unexpected default!").Write();
                    return false;
            }
        }

        public static bool CompareWithTol(Vector3 a, Vector3 b, EqualityType type, float tolerance = 0.001f)
        {
            switch (type)
            {
                case EqualityType.Equal:
                    return CompareWithTol(a.x, b.x, type, tolerance)
                        && CompareWithTol(a.y, b.y, type, tolerance)
                        && CompareWithTol(a.z, b.z, type, tolerance);
                case EqualityType.Inequal:
                    return CompareWithTol(a.x, b.x, type, tolerance)
                        || CompareWithTol(a.y, b.y, type, tolerance)
                        || CompareWithTol(a.z, b.z, type, tolerance);
                default:
                    Logger.IfWarn()?.Message($"Operation is undefined for Vector3 args & passed equalityType: {type}").Write();
                    return false;
            }

        }

        public static bool CheckByEqulityType<T>(T a, T b, EqualityType type) where T : IComparable
        {
            int comparision = a.CompareTo(b);
            switch (type)
            {
                case EqualityType.Equal:          return comparision == 0;
                case EqualityType.Greater:        return comparision > 0;
                case EqualityType.Less:           return comparision < 0;
                case EqualityType.GreaterOrEqual: return comparision >= 0;
                case EqualityType.LessOrEqual:    return comparision <= 0;
                case EqualityType.Inequal:        return comparision != 0;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public enum EqualityType : ushort
        {
            Equal,
            Greater,
            Less,
            GreaterOrEqual,
            LessOrEqual,
            Inequal
        }

        private static Random _random;
        public static Random Random => _random ?? (_random = new Random());

        //@param `chances` should be list of chances [0f .. 1f]. Sum should be == 1f
        //@Returns randomly chosen element index (regarding passed chances). Or `HConsts.InvalidIndex` (-1) if got error
        public static int ChoseRandomly1From(List<float> chances)
        {
            //#old: if (!Mathf.Approximately(1f, chances.Sum()))
            if (CompareWithTol(1f, chances.Sum(), EqualityType.Inequal))
            {
                Logger.IfWarn()?.Message("input chances sum != 1f (=={0})", chances.Sum()).Write();
                return -1;
            }

            float rnd = (float)Random.NextDouble();
            float sum = 0;
            for (int i=0; i < chances.Count; ++i)
            {
                float nextSumVal = sum + chances[i];
                if (rnd > sum && rnd <= nextSumVal)
                    return i;

                sum = nextSumVal;
            }

            Logger.IfWarn()?.Message("Possibly smthng went wrong (rnd=={0}, chances sum=={1})", rnd, chances.Sum()).Write();
            //should return last not-zero chance index
            return -1;
        }

        // returns 1 || 0 || -1 equiprobably(равновероятно)
        public static int Rnd3()
        {
            float rnd = (float)Random.NextDouble();
            return rnd > 0.33f ? 1 : (rnd > 0.66f ? 0 : -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Abs(float arg) => arg >= 0 ? arg : -arg;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static int Sign(float arg) => arg > 0 ? 1 : arg < 0 ? -1 : 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static int CeilToInt(float f) => (int) Math.Ceiling(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static int FloorToInt(float f) => (int) Math.Floor(f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Sqrt(float arg) => (float)Math.Sqrt(arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Sqr(float arg) => arg * arg;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Sin(float arg) => (float)Math.Sin(arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Cos(float arg) => (float)Math.Cos(arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Asin(float arg) => (float)Math.Asin(arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Acos(float arg) => (float)Math.Acos(arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Tan(float arg) => (float)Math.Tan(arg);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Atan(float a) => (float)Math.Atan(a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value); 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static float Clamp01(float value) => value < 0.0 ? 0 : value > 1.0 ? 1f : value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  public static bool Approximately(float a, float b) => Abs(b - a) < Max(1E-06f * Max(Abs(a), Abs(b)), Epsilon * 8f);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);
        
        public static float LerpUnclamped(float a, float b, float t) => a + (b - a) * t;

        public static float InverseLerp(float a, float b, float value) =>  a != b ? Clamp01(( value -  a) / (b - a)) : 0.0f;

        public static float InverseLerp(long a, long b, long value) =>  a != b ? Clamp01((float)( value -  a) / (float)(b - a)) : 0.0f;

        public static float InverseLerp(int a, int b, int value) =>  a != b ? Clamp01((float)( value -  a) / (float)(b - a)) : 0.0f;
        
        public static float Repeat(float t, float length) => Clamp(t - (float)Math.Floor(t / length) * length, 0.0f, length);
        
        public static float LerpAngle(float a, float b, float t) => a + DeltaAngle(a, b) * Clamp01(t);

        public static float DeltaAngle(float current, float target)
        {
            var num = Repeat(target - current, 360f);
            return num > 180.0 ? num - 360f : num;
        }

        public static float LerpAngleRad(float a, float b, float t) => a + DeltaAngleRad(a, b) * Clamp01(t);

        public static float DeltaAngleRad(float current, float target)
        {
            var num = Repeat(target - current, DoublePi);
            return num > Pi ? num - DoublePi : num;
        }

        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
                return target;
            return current + Math.Sign(target - current) * maxDelta;
        }
        
        public static float NormalizeAngleRad(float angle) => Repeat(angle, DoublePi);

        public static bool CheckInsideCapsule(Vector3 point, Vector3 center, float radius, float height, CapsuleDirection dir)
        {
            var cylHalfHgh = height * 0.5f - radius;
            var radiusSqr = radius * radius; 
            switch (dir)
            {
                case CapsuleDirection.X when Math.Abs(point.x - center.x) <= cylHalfHgh:
                    return Vector2.GetSqrDistance(center.YZ, point.YZ) <= radiusSqr;
                case CapsuleDirection.X when point.x > center.x:
                    return Vector3.GetSqrDistance(new Vector3(center.x + cylHalfHgh, center.y, center.z), point) <= radiusSqr;
                case CapsuleDirection.X:
                    return Vector3.GetSqrDistance(new Vector3(center.x - cylHalfHgh, center.y, center.z), point) <= radiusSqr;
                case CapsuleDirection.Y when Math.Abs(point.y - center.y) <= cylHalfHgh:
                    return Vector2.GetSqrDistance(center.XZ, point.XZ) <= radiusSqr;
                case CapsuleDirection.Y when point.y > center.y:
                    return Vector3.GetSqrDistance(new Vector3(center.x, center.y + cylHalfHgh, center.z), point) <= radiusSqr;
                case CapsuleDirection.Y:
                    return Vector3.GetSqrDistance(new Vector3(center.x, center.y - cylHalfHgh, center.z), point) <= radiusSqr;
                case CapsuleDirection.Z when Math.Abs(point.z - center.z) <= cylHalfHgh:
                    return Vector2.GetSqrDistance(center.XY, point.XY) <= radiusSqr;
                case CapsuleDirection.Z when point.z > center.z:
                    return Vector3.GetSqrDistance(new Vector3(center.x, center.y, center.z + cylHalfHgh), point) <= radiusSqr;
                case CapsuleDirection.Z:
                    return Vector3.GetSqrDistance(new Vector3(center.x, center.y, center.z - cylHalfHgh), point) <= radiusSqr;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
        }

        public static float DistanceToCapsule(Vector3 point, Vector3 center, float radius, float height, CapsuleDirection dir)
        {
            var cylHalfHgh = height * 0.5f - radius;
            switch (dir)
            {
                case CapsuleDirection.X when Math.Abs(point.x - center.x) <= cylHalfHgh:
                    return Vector2.GetDistance(center.YZ, point.YZ) - radius;
                case CapsuleDirection.X when point.x > center.x:
                    return Vector3.GetDistance(new Vector3(center.x + cylHalfHgh, center.y, center.z), point) - radius;
                case CapsuleDirection.X:
                    return Vector3.GetDistance(new Vector3(center.x - cylHalfHgh, center.y, center.z), point) - radius;
                case CapsuleDirection.Y when Math.Abs(point.y - center.y) <= cylHalfHgh:
                    return Vector2.GetDistance(center.XZ, point.XZ) - radius;
                case CapsuleDirection.Y when point.y > center.y:
                    return Vector3.GetDistance(new Vector3(center.x, center.y + cylHalfHgh, center.z), point) - radius;
                case CapsuleDirection.Y:
                    return Vector3.GetDistance(new Vector3(center.x, center.y - cylHalfHgh, center.z), point) - radius;
                case CapsuleDirection.Z when Math.Abs(point.z - center.z) <= cylHalfHgh:
                    return Vector2.GetDistance(center.XY, point.XY) - radius;
                case CapsuleDirection.Z when point.z > center.z:
                    return Vector3.GetDistance(new Vector3(center.x, center.y, center.z + cylHalfHgh), point) - radius;
                case CapsuleDirection.Z:
                    return Vector3.GetDistance(new Vector3(center.x, center.y, center.z - cylHalfHgh), point) - radius;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }

            
            
            if (Math.Abs(point.y - center.y) <= cylHalfHgh)
                return Vector2.GetDistance(center.XZ, point.XZ) - radius;
            if (point.y > center.y)
                return Vector3.GetDistance(new Vector3(center.x, center.y + cylHalfHgh, center.z), point) - radius;
            return Vector3.GetDistance(new Vector3(center.x, center.y - cylHalfHgh, center.z), point) - radius;
        }

        public static float SimpleDamp(float current, float delta, out float correctionSpeed, float dampTime, float maxSpeed, float dt)
        {
            correctionSpeed = delta / dampTime;
            correctionSpeed = Sign(correctionSpeed) * Min(Abs(correctionSpeed), maxSpeed);
                return current + correctionSpeed * dt;
        }

        public static float SmoothDamp(
            float current,
            float target,
            ref float currentVelocity,
            float smoothTime,
            float maxSpeed,
            float deltaTime)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            float change = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = Clamp(change, -maxChange, maxChange);
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0F == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }
        
        public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + DeltaAngle(current, target);
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        public static float SmoothDampAngleRad(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + DeltaAngleRad(current, target);
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }
        
    #endregion //Math

        public static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }

        public static string NowStamp => DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        public static string TimeStamp(DateTime time) => time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
    }


    public static class SharedHelpersExtensions
    {
        public static T Clamped<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(max) > 0) return max;
            if (val.CompareTo(min) < 0) return min;
            return val;
        }

        public static float MoveTowardsAngleRad(this float current, float target, float maxDelta)
        {
            float num = SharedHelpers.DeltaAngleRad(current, target);
            if (-maxDelta < num && num < maxDelta)
                return target;
            target = current + num;
            return SharedHelpers.MoveTowards(current, target, maxDelta);
        }
    }


    public static class IEnumerableExtension
    {
        public static bool HelperAny<T>([NotNull] this IEnumerable<T> enumerable, [NotNull] Predicate<T> pred)
        {
            foreach (var entry in enumerable)
                if (pred(entry))
                    return true;

            return false;
        }
    }
    
    public enum CapsuleDirection
    {
        X, Y, Z
    }
}