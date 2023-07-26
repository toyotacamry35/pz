using System;
using Assets.Src.ResourcesSystem.Base;
using ProtoBuf;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    /// <summary>
    /// Идентификатор каста word'а (или любой другой части) спелла уникальный в рамках кастера спелла.
    /// Может использоваться как "causer" в различных методах кастера спелла.
    /// </summary>
    [ProtoContract]
    public struct SpellPartCastId : IEquatable<SpellPartCastId>
    {
        [ProtoMember(1)] public readonly SpellId _spellId;
        [ProtoMember(2)] public readonly int _subSpellId;
        [ProtoMember(3)] public readonly BaseResource _partDef;

        public SpellPartCastId(SpellId spellId, int subSpellId, BaseResource partDef)
        {
            _spellId = spellId;
            _partDef = partDef;
            _subSpellId = subSpellId;
        }

        public static bool operator ==(SpellPartCastId a, SpellPartCastId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SpellPartCastId a, SpellPartCastId b)
        {
            return !a.Equals(b);
        }
        
        public bool Equals(SpellPartCastId other)
        {
            return _spellId == other._spellId && _subSpellId == other._subSpellId && Equals(_partDef, other._partDef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SpellPartCastId && Equals((SpellPartCastId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_spellId.GetHashCode() * 397) ^ (_subSpellId.GetHashCode() * 397) ^ (_partDef != null ? _partDef.GetHashCode() : 0);
            }
        }
        
        public override string ToString()
        {
            return $"[Res:{_partDef?.____GetDebugAddress()} Id:{_spellId}:{_subSpellId}]";
        }

    }
}