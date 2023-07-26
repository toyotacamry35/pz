using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.EntitySystem;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using Src.Aspects.Impl.Stats;
using System;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class ProceduralStat
    {
        private ProceduralStatDef _statDef = null;
        private IStat _limitMaxStatCache = null;
        private IStat _limitMinStatCache = null;

        public ValueTask<float> GetValueImpl()
        {
            return new ValueTask<float>(ValueCache);
        }

        public async ValueTask InitializeImpl(StatDef statDef, bool resetState)
        {
            _statDef = statDef as ProceduralStatDef;
            var statsHolder = (IStatsEngine)this.parentDeltaObject.GetParentObject();

            StatType = _statDef.StatType;

            if (_statDef.LimitMaxStat != null)
                _limitMaxStatCache = await statsHolder.GetStat(_statDef.LimitMaxStat);
            if (_statDef.LimitMinStat != null)
                _limitMinStatCache = await statsHolder.GetStat(_statDef.LimitMinStat);

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

            if (_statDef.ValueCalcer != null)
            {
                var parentRef = new OuterRef<IEntity>(parentEntity.Id, parentEntity.TypeId);
                if (parentRef.IsValid)
                {
                    float newValue = 0;
                    using (var parentCnt = await parentEntity.GetThis())
                    {
                        var ctx = new CalcerContext(parentCnt, parentRef, parentEntity.EntitiesRepository);
                        newValue = await _statDef.ValueCalcer.Target.CalcAsync(ctx);
                    }

                    newValue = Math.Min(Math.Max(newValue, LimitMinCache), LimitMaxCache);
                    if (newValue != ValueCache)
                    {
                        statChanged = true;
                        ValueCache = newValue;
                    }
                }
            }

            return statChanged;
        }

        public override string ToString()
        {
            return $"{ValueCache,6:F1} [{(LimitMinCache < -100000 ? "-∞" : LimitMinCache + ""),6:F1}; {(LimitMaxCache > 100000 ? "+∞" : LimitMaxCache + ""),6:F1}]";
        }
    }
}
