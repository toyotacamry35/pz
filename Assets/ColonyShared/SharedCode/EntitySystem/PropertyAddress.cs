using System;
using ProtoBuf;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public class PropertyAddress
    {
        [ProtoMember(1)]
        public Guid EntityId { get; set; }

        [ProtoMember(2)]
        public int EntityTypeId { get; set; }

        [ProtoMember(3)]
        public ulong DeltaObjectLocalId { get; set; }

        [ProtoMember(4)]
        public int DeltaObjectFieldId { get; set; } = -1; //Optional, -1 = DeltaObject itself

        public bool IsValid()
        {
            return EntityTypeId != 0 && EntityId != Guid.Empty;
        }

        public override string ToString()
        {
            return $"{nameof(PropertyAddress)}:[{EntityId}:{EntityTypeId}/{DeltaObjectLocalId}/{DeltaObjectFieldId}]";
        }
    }

    public static class PropertyAddressExtensions
    {
        public static PropertyAddress Clone(this PropertyAddress @this) => @this != null ? new PropertyAddress {EntityId = @this.EntityId, EntityTypeId = @this.EntityTypeId, DeltaObjectLocalId = @this.DeltaObjectLocalId, DeltaObjectFieldId = @this.DeltaObjectFieldId} : null;
    }
}