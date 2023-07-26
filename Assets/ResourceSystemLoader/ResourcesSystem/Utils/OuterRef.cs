using System;
using ProtoBuf;

namespace ResourceSystem.Utils
{
    [ProtoContract]
    public struct OuterRef : IEquatable<OuterRef>
    {
        [ProtoMember(1)] public Guid Guid;
        [ProtoMember(2)] public int TypeId;
        
        public bool IsValid => TypeId != 0 && Guid != Guid.Empty;
        
        public static readonly OuterRef Invalid = default(OuterRef);
        
        public OuterRef(Guid id, int typeId)
        {
            Guid = id;
            TypeId = typeId;
        }
        
        public override string ToString() => IsValid ? $"{Guid} {TypeId}" : "<not valid>";
        
        public override bool Equals(object obj) => obj is OuterRef @ref && Equals(@ref);

        public bool Equals(OuterRef other) => TypeId == other.TypeId && Guid.Equals(other.Guid);

        public override int GetHashCode()
        {
            var hashCode = -1531579387;
            hashCode = hashCode * -1521134295 + Guid.GetHashCode();
            hashCode = hashCode * -1521134295 + TypeId.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(OuterRef ref1, OuterRef ref2) => ref1.Equals(ref2);

        public static bool operator !=(OuterRef ref1, OuterRef ref2) => !ref1.Equals(ref2);
        
        public string ShortDebugGuid => Guid.ToString().Substring(28);
    }
}