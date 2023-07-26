using System;
using ProtoBuf;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    [ProtoContract]
    public struct SpellGlobalId : IEquatable<SpellGlobalId>
    {
        [ProtoMember(1)] public readonly Guid _wizardGuid;
        [ProtoMember(2)] public readonly SpellId _spellId;

        public SpellGlobalId(Guid wizardGuid, SpellId spellId)
        {
            _wizardGuid = wizardGuid;
            _spellId = spellId;
        }
        
        
        public static bool operator ==(SpellGlobalId a, SpellGlobalId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SpellGlobalId a, SpellGlobalId b)
        {
            return !a.Equals(b);
        }
        
        public bool Equals(SpellGlobalId other)
        {
            return _wizardGuid == other._wizardGuid && _spellId == other._spellId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SpellGlobalId && Equals((SpellGlobalId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _wizardGuid.GetHashCode();
                hashCode = (hashCode * 397) ^ _spellId.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"[Id:{_spellId} Wizard:{_wizardGuid}]";
        }
    }
}