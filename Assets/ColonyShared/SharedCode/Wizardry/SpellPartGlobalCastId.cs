using System;
using Assets.Src.ResourcesSystem.Base;
using ProtoBuf;
using SharedCode.Wizardry;

namespace GeneratedCode.DeltaObjects
{
    /// <summary>
    /// Идентификатор каста word'а (или любой другой части) спелла уникальный глобально.
    /// Может использоваться как "causer" в различных глобальных методах.
    /// </summary>
    [ProtoContract]
    public struct SpellPartGlobalCastId : IEquatable<SpellPartGlobalCastId>
    {
        [ProtoMember(1)] public readonly Guid _wizardGuid;
        [ProtoMember(2)] public readonly SpellId _spellId;
        [ProtoMember(3)] public readonly int _subSpellId;
        [ProtoMember(4)] public readonly BaseResource _partDef;

        public SpellPartGlobalCastId(Guid wizardGuid, SpellId spellId, int subSpellId, BaseResource partDef)
        {
            _wizardGuid = wizardGuid;
            _spellId = spellId;
            _partDef = partDef;
            _subSpellId = subSpellId;
        }

        public static bool operator ==(SpellPartGlobalCastId a, SpellPartGlobalCastId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SpellPartGlobalCastId a, SpellPartGlobalCastId b)
        {
            return !a.Equals(b);
        }
        
        public bool Equals(SpellPartGlobalCastId other)
        {
            return _wizardGuid == other._wizardGuid && _spellId == other._spellId && _subSpellId == other._subSpellId && Equals(_partDef, other._partDef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SpellPartGlobalCastId && Equals((SpellPartGlobalCastId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _wizardGuid.GetHashCode();
                hashCode = (hashCode * 397) ^ _spellId.GetHashCode();
                hashCode = (hashCode * 397) ^ (_partDef != null ? _partDef.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"[Res:{_partDef?.____GetDebugAddress()} Id:{_spellId} Wizard:{_wizardGuid}]";
        }
    }
}