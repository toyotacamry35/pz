using Assets.Src.Aspects.Impl.Stats;
using JetBrains.Annotations;
using SharedCode.Entities.Engine;
using Src.Aspects.Impl.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;

namespace GeneratedCode.DeltaObjects
{
    public partial class AccumulatedStat
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private AccumulatedStatModifiers _modifiers;
        
        public AccumulatedStatModifiers Modifiers => _modifiers ?? (_modifiers = new AccumulatedStatModifiers());

        private AccumulatedStatDef _statDef = null;
        private IStat _limitMinStatCache;
        private IStat _limitMaxStatCache;

        public ValueTask<float> GetValueImpl()
        {
            return new ValueTask<float>(ValueCache);
        }

        public ValueTask<bool> AddModifierImpl(ModifierCauser causer, StatModifierType modifierType, float value)
        {
            Logger.IfDebug()?.Write("Add Modifier", _=>_  + ("Stat", _statDef) + ("Causer", causer) + ParentEntityId);
            if (causer.Causer != null)
            {
                var modifier = Modifiers[modifierType];
                if (modifier.Modifiers.TryGetValue(causer, out float previousValue))
                    modifier.Modifiers[causer] = value;
                else
                    modifier.Modifiers.Add(causer, value);

                switch (modifierType)
                {
                    case StatModifierType.Add:
                    case StatModifierType.Mul:
                        modifier.Cache += value - previousValue;
                        break;
                    case StatModifierType.ClampMin:
                        modifier.Cache = Math.Max(modifier.Modifiers.Values.Min(), _statDef.LimitMinDefault);
                        break;
                    case StatModifierType.ClampMax:
                        modifier.Cache = Math.Min(modifier.Modifiers.Values.Max(), _statDef.LimitMaxDefault);
                        break;
                    default:
                        throw new NotImplementedException($"modifierType = {modifierType}");
                }
            }

            return RecalculateCachesImpl(false);
        }
        
        public ValueTask<bool> IsGetAffectedByImpl(ModifierCauser causer)
        {
            foreach (var tuple in Modifiers)
                if (tuple.Value.Modifiers.ContainsKey(causer))
                    return new ValueTask<bool>(true);
            return new ValueTask<bool>(false);
        }

        public ValueTask<bool> RemoveModifiersImpl([CanBeNull] ModifierCauser causer)
        {
            bool removed = false;
            foreach (var tuple in Modifiers)
                removed |= RemoveModifierInternal(causer, tuple.Key);
            return removed ? RecalculateCachesImpl(false) : new ValueTask<bool>(false);
        }

        public ValueTask<bool> RemoveModifierImpl([CanBeNull] ModifierCauser causer, StatModifierType modifierType)
        {
            if(RemoveModifierInternal(causer, modifierType))
                return RecalculateCachesImpl(false);
            Logger.IfWarn()?.Write("Modifier not found", _=>_ + ("Stat", _statDef) + ("Causer", causer) + ParentEntityId);
            return new ValueTask<bool>(false);
        }
        
        public bool RemoveModifierInternal([CanBeNull] ModifierCauser causer, StatModifierType modifierType)
        {
            Logger.IfDebug()?.Write("Remove Modifier", _=>_ + ("Stat", _statDef) + ("Causer", causer) + ParentEntityId);
            if (causer.Causer != null)
            {
                var modifier = Modifiers[modifierType];
                if (modifier.Modifiers.TryGetValue(causer, out float previousValue))
                {
                    modifier.Modifiers.Remove(causer);
                    switch (modifierType)
                    {
                        case StatModifierType.Add:
                        case StatModifierType.Mul:
                            modifier.Cache -= previousValue;
                            break;
                        case StatModifierType.ClampMin:
                        {
                            var modifiers = modifier.Modifiers;
                            modifier.Cache = Math.Max(modifiers.Any() ? modifiers.Values.Min() : float.MinValue, _statDef.LimitMinDefault);
                            break;
                        }
                        case StatModifierType.ClampMax:
                        {
                            var modifiers = modifier.Modifiers;
                            modifier.Cache = Math.Min(modifiers.Any() ? modifiers.Values.Max() : float.MaxValue, _statDef.LimitMaxDefault);
                            break;
                        }
                        default:
                            throw new NotImplementedException($"modifierType = {modifierType}");
                    }
                }
                return true;
            }
            return false;
        }

        public async ValueTask InitializeImpl(StatDef statDef, bool resetState)
        {
            _statDef = statDef as AccumulatedStatDef ?? throw new ArgumentException($"typeof(statDef) is {statDef?.GetType()}");

            if (resetState)
            {
                Modifiers.Clear();
                InitialValue = _statDef.InitialValue;
            }
            StatType = _statDef.StatType;

            if (Modifiers[StatModifierType.Add] == null)
                Modifiers[StatModifierType.Add] = new AccumulatedStatModifier() {Cache = 0f};
            if (Modifiers[StatModifierType.Mul] == null)
                Modifiers[StatModifierType.Mul] = new AccumulatedStatModifier() { Cache = 1f };
            if (Modifiers[StatModifierType.ClampMin] == null)
                Modifiers[StatModifierType.ClampMin] = new AccumulatedStatModifier() { Cache = _statDef.LimitMinDefault };
            if (Modifiers[StatModifierType.ClampMax] == null)
                Modifiers[StatModifierType.ClampMax] = new AccumulatedStatModifier() { Cache = _statDef.LimitMaxDefault };

            var statsHolder = (IStatsEngine)parentDeltaObject.GetParentObject();
            if (_statDef.LimitMinStat != null)
                _limitMinStatCache = await statsHolder.GetStat(_statDef.LimitMinStat);
            if (_statDef.LimitMaxStat != null)
                _limitMaxStatCache = await statsHolder.GetStat(_statDef.LimitMaxStat);

            await RecalculateCachesImpl(false);
        }

        public async ValueTask<bool> RecalculateCachesImpl(bool calcersOnly)
        {
            bool statChanged = false;

            if (!calcersOnly)
            {
                var statLimitMin = _limitMinStatCache != null ? await _limitMinStatCache.GetValue() : float.MinValue;
                var limitMin = Math.Max(statLimitMin, Modifiers[StatModifierType.ClampMin].Cache);
                if (LimitMinCache != limitMin)
                {
                    statChanged = true;
                    LimitMinCache = limitMin;
                }

                var statLimitMax = _limitMaxStatCache != null ? await _limitMaxStatCache.GetValue() : float.MaxValue;
                var limitMax = Math.Min(statLimitMax, Modifiers[StatModifierType.ClampMax].Cache);
                if (LimitMaxCache != limitMax)
                {
                    statChanged = true;
                    LimitMaxCache = limitMax;
                }
            }

            var newValue = Math.Min(Math.Max((InitialValue + Modifiers[StatModifierType.Add].Cache) * Modifiers[StatModifierType.Mul].Cache, LimitMinCache), LimitMaxCache);
            if (newValue != ValueCache)
            {
                statChanged = true;
                ValueCache = newValue;
            }
            
            return statChanged;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var tuple in Modifiers)
            {
                if (tuple.Value.Modifiers.Any())
                    sb.Append($"{tuple.Key} = {tuple.Value.Cache}; ({string.Join("; ", tuple.Value.Modifiers.Select(modifier => $"{modifier.Key.Causer?.____GetDebugShortName() ?? "null"}: {modifier.Key.SpellId} -> {modifier.Value}"))})");
            }

            return $"{ValueCache,6:F1} [{(LimitMinCache < -100000 ? "-∞" : LimitMinCache + ""),6:F1}; {(LimitMaxCache > 100000 ? "+∞" : LimitMaxCache + ""),6:F1}]; {sb.ToString()}";
        }
    }
}
