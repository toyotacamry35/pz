
using ProtoBuf;
using System;
using System.Threading;
using ColonyShared.SharedCode;

namespace SharedCode.Wizardry
{

    [ProtoContract]
    public struct SpellId : IEquatable<SpellId>, IComparable, IComparable<SpellId>
    {
        private const long SlaveGeneratedSpellId = 1L << 63;

        public static SpellId Invalid => default(SpellId);

        [ProtoMember(1)]
        public long _counter;

        public ulong Counter => (ulong) _counter;        
        public bool IsValid => _counter != 0;
        public bool IsGeneratedBySlave => (_counter & SlaveGeneratedSpellId) != 0;
        
        public static SpellId FirstMasterValid => new SpellId(1);

        public static SpellId FirstSlaveValid => new SpellId(1 | SlaveGeneratedSpellId);
        
        public SpellId Next()
        {
            return new SpellId(_counter + 1);
        }
        
        public SpellId(ulong counter)
        {
            _counter = (long)counter;
        }

        private SpellId(long counter)
        {
            _counter = counter;
        }
 
        public SpellId InterlockedGet()
        {
            return new SpellId(Interlocked.Read(ref _counter));
        }

        public void InterlockedSet(SpellId v)
        {
            Interlocked.Exchange(ref _counter, v._counter);
        }
 
        public static bool operator ==(SpellId a, SpellId b) => a._counter == b._counter;

        public static bool operator !=(SpellId a, SpellId b) => a._counter != b._counter;

        public long PureCounter => _counter & ~SlaveGeneratedSpellId;

        public override string ToString()
        {
            if (!IsValid)
                return "Invalid";
            return $"{(IsGeneratedBySlave?"S":"H")}{(PureCounter)}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SpellId))
            {
                return false;
            }

            var id = (SpellId)obj;
            return this == id;
        }

        public override int GetHashCode()
        {
            var hashCode = -873303510;
            hashCode = hashCode * -1521134295 + _counter.GetHashCode();
//            hashCode = hashCode * -1521134295 + IsValid.GetHashCode();
//            hashCode = hashCode * -1521134295 + IsDefault.GetHashCode();
            return hashCode;
        }

        public bool Equals(SpellId other)
        {
            return other._counter == this._counter;
        }

        public int CompareTo(object obj)
        {
            return _counter.CompareTo(((SpellId)obj)._counter);
        }

        public int CompareTo(SpellId other)
        {
            return _counter.CompareTo(other._counter);
        }
    }
    
    
    public static class ValueExtensionSpellId
    {
        public static SpellId SpellId(this in Value value) => new SpellId(value.ULong);
        
        public static Value ToValue(this SpellId spellId) => new Value(spellId.Counter);
    }
}
