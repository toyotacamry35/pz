using System;
using System.Runtime.CompilerServices;
using ColonyShared.SharedCode.Utils;

namespace Src.Locomotion
{
    public readonly struct LocomotionTimestamp
    {
        private readonly Int64 _timestamp;

        public static readonly LocomotionTimestamp None = long.MinValue;

        private LocomotionTimestamp(long timestamp)
        {
            _timestamp = timestamp;
        }

        public bool Valid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _timestamp != None._timestamp;
        }

        public float Seconds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => SyncTime.ToSeconds(_timestamp);
        }

        public override string ToString() => _timestamp.ToString();

        public void CheckValidity(string name = "this")
        {
            if (!Valid) throw new ArgumentException("Timestamp is not valid", name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static LocomotionTimestamp FromSeconds(float sec) => SyncTime.FromSeconds(sec);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator long(in LocomotionTimestamp self) => self._timestamp;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator LocomotionTimestamp(long ts) => new LocomotionTimestamp(ts);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static LocomotionTimestamp operator -(in LocomotionTimestamp a, in LocomotionTimestamp b) =>
            new LocomotionTimestamp(a._timestamp - b._timestamp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static LocomotionTimestamp operator +(in LocomotionTimestamp a, in LocomotionTimestamp b) =>
            new LocomotionTimestamp(a._timestamp + b._timestamp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static LocomotionTimestamp Lerp(in LocomotionTimestamp a, in LocomotionTimestamp b, float t) =>
            (long) ((b._timestamp - a._timestamp) * t) + a._timestamp;

        public static float InverseLerp(LocomotionTimestamp a, LocomotionTimestamp b, LocomotionTimestamp v)
        {
            if (a > b) throw new ArgumentException($"{a} > {b}");
            return v >= b ? 1 : v <= a ? 0 : (float) (v._timestamp - a._timestamp) / (float) (b._timestamp - a._timestamp);
        }
    }
}