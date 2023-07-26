using ColonyShared.SharedCode.Utils;
using ProtoBuf;
using System;
using Assets.ColonyShared.SharedCode.Utils;

namespace Src.Aspects.Impl.Stats.Proxy
{
    [ProtoContract]
    public struct TimeStatState : IEquatable<TimeStatState>
    {
        private const float Epsilon = 0.0001f;
        [ProtoMember(1)]
        public float ChangeRateCache;

        [ProtoMember(2)]
        public long LastBreakPointTime;

        [ProtoMember(3)]
        public float LastBreakPointValue;

        public TimeStatState(float changeRateCache, long lastBreakPointTime, float lastBreakPointValue)
        {
            ChangeRateCache = changeRateCache;
            LastBreakPointTime = lastBreakPointTime;
            LastBreakPointValue = lastBreakPointValue;
        }

        public bool Equals(TimeStatState other)
        {
            return SharedHelpers.Approximately(other.ChangeRateCache, ChangeRateCache) &&
                   other.LastBreakPointTime == LastBreakPointTime &&
                   SharedHelpers.Approximately(other.LastBreakPointValue, LastBreakPointValue);
        }

        public override int GetHashCode()
        {
            var hashCode = 1250757225;
            hashCode = hashCode * -1521134295 + ChangeRateCache.GetHashCode();
            hashCode = hashCode * -1521134295 + LastBreakPointTime.GetHashCode();
            hashCode = hashCode * -1521134295 + LastBreakPointValue.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{LastBreakPointValue:f1} {ChangeRateCache.AsSignedString()} x{0.001f * (SyncTime.Now - LastBreakPointTime):f1}";
        }

        public string ToDebug()
        {
            return $"({nameof(TimeStatState)}: LastBPValue={LastBreakPointValue:f2}, Rate={ChangeRateCache:f2}, " +
                   $"LastBPTime={LastBreakPointTime:f0})";
        }
    }
}