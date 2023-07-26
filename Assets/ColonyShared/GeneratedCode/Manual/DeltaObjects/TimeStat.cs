using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.EntitySystem;
using ResourceSystem.Utils;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using Src.Aspects.Impl.Stats;
using Src.Aspects.Impl.Stats.Proxy;
using System;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeStat
    {
        private TimeStatDef _statDef = null;
        private IStat _limitMinStatCache;
        private IStat _limitMaxStatCache;
        private IStat _changeRateStatCache;

        public ValueTask<float> GetValueImpl()
        {
            return new ValueTask<float>(GetValueinternal());
        }

        private float GetValueinternal()
        {
            var statsHolder = (IStatsEngine)this.parentDeltaObject.GetParentObject();

            float passedSeconds = (SyncTime.Now - State.LastBreakPointTime) / 1000.0f;
            var changeRate = statsHolder.TimeWhenIdleStarted == 0 ? State.ChangeRateCache : 0;
            var val = State.LastBreakPointValue + changeRate * passedSeconds;
            val = Math.Min(Math.Max(val, LimitMinCache), LimitMaxCache);
            return val;
        }

        public ValueTask<bool> ChangeValueImpl(float delta)
        {
            var currentValue = GetValueinternal();
            var newValue = currentValue + delta;
            State = new TimeStatState()
            {
                LastBreakPointValue = Math.Min(Math.Max(LimitMinCache, newValue), LimitMaxCache),
                LastBreakPointTime = SyncTime.Now,
                ChangeRateCache = State.ChangeRateCache
            };

            return RecalculateCachesImpl(false);
        }

        public async ValueTask InitializeImpl(StatDef statDef, bool resetState)
        {
            _statDef = statDef as TimeStatDef;
            if (resetState)
            {
                State = new TimeStatState()
                {
                    LastBreakPointValue = _statDef.InitialValue,
                    LastBreakPointTime = SyncTime.Now
                };
            }
            else
            {
                State = new TimeStatState()
                {
                    LastBreakPointValue = State.LastBreakPointValue,
                    ChangeRateCache = State.ChangeRateCache,
                    LastBreakPointTime = SyncTime.Now
                };
            }
            StatType = _statDef.StatType;

            var statsHolder = (IStatsEngine)this.parentDeltaObject.GetParentObject();
            if (_statDef.LimitMaxStat != null)
                _limitMaxStatCache = await statsHolder.GetStat(_statDef.LimitMaxStat);
            if (_statDef.LimitMinStat != null)
                _limitMinStatCache = await statsHolder.GetStat(_statDef.LimitMinStat);
            if (_statDef.ChangeRateStat != null)
                _changeRateStatCache = await statsHolder.GetStat(_statDef.ChangeRateStat);

            await RecalculateCachesImpl(false);
        }

        public async ValueTask<bool> RecalculateCachesImpl(bool calcersOnly)
        {
            bool statChanged = false;

            if (!calcersOnly)
            {
                var statLimitMin = _limitMinStatCache != null ? await _limitMinStatCache.GetValue() : float.MinValue;
                var limitMin = Math.Max(statLimitMin, _statDef.LimitMinDefault);
                if (limitMin != LimitMinCache)
                {
                    statChanged = true;
                    LimitMinCache = limitMin;
                }

                var statLimitMax = _limitMaxStatCache != null ? await _limitMaxStatCache.GetValue() : float.MaxValue;
                var limitMax = Math.Min(statLimitMax, _statDef.LimitMaxDefault);
                if (limitMax != LimitMaxCache)
                {
                    statChanged = true;
                    LimitMaxCache = limitMax;
                }
            }

            float? newChangeRateCache = null;
            if (_statDef.ChangeRateCalcer == null)
            {
                if (!calcersOnly)
                    newChangeRateCache = _changeRateStatCache != null ? await _changeRateStatCache.GetValue() : _statDef.ChangeRateDefault;
            }
            else
            {
                var parentRef = new OuterRef<IEntity>(parentEntity.Id, parentEntity.TypeId);
                if (parentRef.IsValid)
                {
                    using (var parentCnt = await parentEntity.GetThis())
                    {
                        var ctx = new CalcerContext(parentCnt, parentRef, parentEntity.EntitiesRepository);
                        newChangeRateCache = await _statDef.ChangeRateCalcer.Target.CalcAsync(ctx);
                    }
                }
            }

            if (newChangeRateCache.HasValue && newChangeRateCache != State.ChangeRateCache)
            {
                statChanged = true;
            }

            if (statChanged)
            {
                State = new TimeStatState()
                {
                    LastBreakPointValue = await GetValue(),
                    LastBreakPointTime = SyncTime.Now,
                    ChangeRateCache = newChangeRateCache.Value
                };
            }

            return statChanged;
        }

        public override string ToString()
        {
            return $"{GetValueImpl().Result,6:F1} = {State.LastBreakPointValue,6:F1} {(State.ChangeRateCache >= 0 ? '+' : '-')} {Math.Abs(State.ChangeRateCache),6:F1} x {0.001f * (SyncTime.Now - State.LastBreakPointTime),6:F1} [{(LimitMinCache < -100000 ? "-∞" : LimitMinCache + ""),6:F1}; {(LimitMaxCache > 100000 ? "+∞" : LimitMaxCache + ""),6:F1}]";
        }
    }
}
