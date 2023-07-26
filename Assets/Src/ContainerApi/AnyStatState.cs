using System;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Aspects.Impl.Stats.Proxy;
using UnityEngine;

namespace Assets.Src.ContainerApis
{
    public struct AnyStatState : IEquatable<AnyStatState>
    {
        public const float Epsilon = 0.0000001f;

        public StatKind Kind;
        public float Min;
        public float Max;
        public float Val;
        public TimeStatState TimeState;

        static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(EntityApi));


        //=== Props ===========================================================

        public float Value => Kind == StatKind.Time ? GetTimeStatValue(TimeState, Min, Max) : Mathf.Clamp(Val, Min, Max);

        public float Ratio => LinearRelation.GetClampedY(new Vector4(Min, 0, Max, 1), Value);

        public float ZeroBasedRatio => Value / (Mathf.Approximately(Max, 0) ? Epsilon : Max);

        public bool NeeedToBeUpdated
        {
            get
            {
                if (Kind != StatKind.Time)
                    return false;

                var ratio = TimeState.ChangeRateCache;
                if (Mathf.Approximately(ratio, 0))
                    return false;

                var val = Value;
                return !(ratio > 0 && val == Max || ratio < 0 && val == Min);
            }
        }


        //=== Ctor ============================================================

        public AnyStatState(StatKind kind, float min, float max, float val)
        {
            Kind = kind;
            Min = min;
            Max = max;
            Val = val;
            TimeState = new TimeStatState();

            if (kind == StatKind.Time)
                Logger.IfError()?.Message($"Unexpected kind in {nameof(AnyStatState)} ctor: {kind}").Write();
        }

        public AnyStatState(StatKind kind, float min, float max, TimeStatState timeState)
        {
            Kind = kind;
            Min = min;
            Max = max;
            TimeState = timeState;
            Val = 0;

            if (kind != StatKind.Time)
                Logger.IfError()?.Message($"Unexpected kind in {nameof(AnyStatState)} ctor: {kind}").Write();
        }


        //=== Public ==========================================================

        public AnyStatState SetKind(StatKind kind)
        {
            Kind = kind;
            return this;
        }

        public AnyStatState SetMin(float min)
        {
            Min = min;
            return this;
        }

        public AnyStatState SetMax(float max)
        {
            Max = max;
            return this;
        }

        public AnyStatState SetVal(float val)
        {
            Val = val;
            return this;
        }

        public AnyStatState SetTimeStatState(TimeStatState timeState)
        {
            TimeState = timeState;
            return this;
        }

        public static float GetTimeStatValue(TimeStatState state, float limitMinCache, float limitMaxCache)
        {
            return Mathf.Clamp(
                state.LastBreakPointValue + state.ChangeRateCache * (SyncTime.Now - state.LastBreakPointTime) / 1000f,
                limitMinCache,
                limitMaxCache);
        }

        public bool Equals(AnyStatState other)
        {
            return Kind == other.Kind &&
                   Mathf.Approximately(Min, other.Min) &&
                   Mathf.Approximately(Max, other.Max) &&
                   Mathf.Approximately(Val, other.Val) &&
                   TimeState.Equals(other.TimeState);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is AnyStatState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Kind;
                hashCode = (hashCode * 397) ^ Min.GetHashCode();
                hashCode = (hashCode * 397) ^ Max.GetHashCode();
                hashCode = (hashCode * 397) ^ Val.GetHashCode();
                hashCode = (hashCode * 397) ^ TimeState.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            var valInfo = Kind == StatKind.Time ? TimeState.ToDebug() : $"{nameof(Val)}={Val}";
            return $"[{nameof(AnyStatState)}: {nameof(Kind)}={Kind}, {nameof(Value)}={Value}, {Min}...{Max}, {valInfo}]";
        }
    }
}