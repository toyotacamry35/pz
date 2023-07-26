using Assets.Src.Aspects.Impl.Stats;
using SharedCode.Entities.Engine;
using Src.Aspects.Impl.Stats;
using System;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class ValueStat
    {
        private ValueStatDef _stat;
        private IStat _limitMinStatCache;
        private IStat _limitMaxStatCache;

        public ValueTask<float> GetValueImpl()
        {
            return new ValueTask<float>(Value);
        }

        public async ValueTask<bool> ChangeValueImpl(float delta)
        {
            Value = Math.Min(Math.Max(LimitMinCache, Value + delta), LimitMaxCache);
            return await RecalculateCachesImpl(false) || delta != 0;
        }

        public async ValueTask InitializeImpl(StatDef statDef, bool resetState)
        {
            _stat = statDef as ValueStatDef;
            if (resetState)
                Value = statDef.InitialValue;
            StatType = statDef.StatType;

            var statsHolder = (IStatsEngine)this.parentDeltaObject.GetParentObject();
            if (statDef.LimitMaxStat != null)
                _limitMaxStatCache = await statsHolder.GetStat(statDef.LimitMaxStat);
            if (statDef.LimitMinStat != null)
                _limitMinStatCache = await statsHolder.GetStat(statDef.LimitMinStat);

            await RecalculateCachesImpl(false);
        }

        public ValueTask CopyImpl(IValueStat valueStat)
        {
            LimitMaxCache = float.MaxValue;
            LimitMinCache = float.MinValue;
            StatType = valueStat.StatType;
            Value = valueStat.Value;

            return new ValueTask();
        }

        public async ValueTask<bool> RecalculateCachesImpl(bool calcersOnly)
        {
            bool statChanged = false;

            if (!calcersOnly)
            {
                var statLimitMin = _limitMinStatCache != null ? await _limitMinStatCache.GetValue() : float.MinValue;
                var limitMin = Math.Max(statLimitMin, _stat?.LimitMinDefault ?? float.MinValue);
                if (limitMin != LimitMinCache)
                {
                    statChanged = true;
                    LimitMinCache = limitMin;
                }

                var statLimitMax = _limitMaxStatCache != null ? await _limitMaxStatCache.GetValue() : float.MaxValue;
                var limitMax = Math.Min(statLimitMax, _stat?.LimitMaxDefault ?? float.MaxValue);
                if (limitMax != LimitMaxCache)
                {
                    statChanged = true;
                    LimitMaxCache = limitMax;
                }
            }

            var newValue = Math.Min(Math.Max(LimitMinCache, Value), LimitMaxCache);
            if (newValue != Value)
            {
                statChanged = true;
                Value = newValue;
            }

            return statChanged;
        }

        public override string ToString()
        {
            return $"{GetValueImpl().Result,6:F1} [{(LimitMinCache < -100000 ? "-∞" : LimitMinCache + ""),6:F1}; {(LimitMaxCache > 100000 ? "+∞" : LimitMaxCache + ""),6:F1}]";
        }
    }
}
