using Assets.Src.ResourcesSystem.Base;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace SharedCode.Entities.Engine
{
    [ProtoContract]
    public struct ModifierCauser : IEquatable<ModifierCauser>
    {
        [ProtoMember(1)]
        public BaseResource Causer;

        [ProtoMember(2)]
        public ulong SpellId;

        public ModifierCauser(BaseResource causer, ulong spellId)
        {
            Causer = causer;
            SpellId = spellId;
        }

        public bool Equals(ModifierCauser other)
        {
            return other.Causer == Causer && other.SpellId == SpellId;
        }

        public override int GetHashCode()
        {
            var hashCode = -4683566;
            hashCode = hashCode * -1521134295 + EqualityComparer<BaseResource>.Default.GetHashCode(Causer);
            hashCode = hashCode * -1521134295 + SpellId.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return Causer.____GetDebugShortName() + ": " + SpellId;
        }

        public override bool Equals(object obj)
        {
            return obj is ModifierCauser && Equals((ModifierCauser)obj);
        }

        public static bool operator ==(ModifierCauser left, ModifierCauser right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModifierCauser left, ModifierCauser right)
        {
            return !(left == right);
        }
    }
}