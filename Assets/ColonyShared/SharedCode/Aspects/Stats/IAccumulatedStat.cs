using System;
using System.Collections;
using Assets.Src.ResourcesSystem.Base;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using Src.Aspects.Impl.Stats.Proxy;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Src.Aspects.Impl.Stats
{
    [GenerateDeltaObjectCode]
    public interface IAccumulatedStat : IStat, IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)] float InitialValue { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] float ValueCache { get; set; }

        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> AddModifier(ModifierCauser causer, StatModifierType modifierType, float value);
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> IsGetAffectedBy(ModifierCauser causer);
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> RemoveModifier(ModifierCauser causer, StatModifierType modifierType);
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> RemoveModifiers(ModifierCauser causer);
        [RuntimeData(SkipField = true)] AccumulatedStatModifiers Modifiers { get; }
    }

    public class AccumulatedStatModifiers
    {
        private static readonly StatModifierType[] ModifierTypes = Enum.GetValues(typeof(StatModifierType)).Cast<StatModifierType>().ToArray();
        private static readonly int ModifierTypesCount = (int)ModifierTypes.Max() + 1;
        
        private readonly AccumulatedStatModifier[] _modifiers = new AccumulatedStatModifier[ModifierTypesCount];
        
        public AccumulatedStatModifier this[StatModifierType type] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _modifiers[(int) type]; set => _modifiers[(int) type] = value; }

        public void Clear()
        {
            if (_modifiers != null)
                Array.Clear(_modifiers, 0, _modifiers.Length);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator
        {
            private int _idx;
            private readonly AccumulatedStatModifiers _modifiers;

            [MethodImpl(MethodImplOptions.AggressiveInlining)] public Enumerator(AccumulatedStatModifiers modifiers)
            {
                _idx = -1;
                _modifiers = modifiers;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool MoveNext()
            {
                while (++_idx < ModifierTypes.Length && _modifiers[ModifierTypes[_idx]] == null) {} 
                return _idx < ModifierTypes.Length;
            }

            public KeyValuePair<StatModifierType, AccumulatedStatModifier> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)] get
                {
                    var modifierType = ModifierTypes[_idx];
                    return new KeyValuePair<StatModifierType, AccumulatedStatModifier>(modifierType, _modifiers[modifierType]);
                }
            }
        }
    }
    
    public class AccumulatedStatModifier
    {
        public float Cache;
        Dictionary<ModifierCauser, float> _modifiers;
        public Dictionary<ModifierCauser, float> Modifiers
        {
            get => _modifiers == null ? _modifiers = new Dictionary<ModifierCauser, float>() : _modifiers;
        }
    }

    public enum StatModifierType
    {
        Add = 0,
        Mul,
        ClampMin,
        ClampMax
    }
}
