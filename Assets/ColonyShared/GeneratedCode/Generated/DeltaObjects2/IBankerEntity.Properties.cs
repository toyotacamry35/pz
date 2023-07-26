// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class BankerEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.ResourceSystem.Aspects.Banks.BankDef, SharedCode.Entities.IBankHolder> _BankCells;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<Assets.ResourceSystem.Aspects.Banks.BankDef, SharedCode.Entities.IBankHolder> BankCells
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _BankCells, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _BankCells, value, SharedCode.EntitySystem.ReplicationLevel.Server, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? BankCells__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_BankCells)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate BankCells__Changed;
        public string BankCells__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(BankCells__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool BankCells__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(10);
            set
            {
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class BankHolder
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<ResourceSystem.Utils.OuterRef, ResourceSystem.Utils.OuterRef> _Cells;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<ResourceSystem.Utils.OuterRef, ResourceSystem.Utils.OuterRef> Cells
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Cells, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Cells, value, SharedCode.EntitySystem.ReplicationLevel.Server, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Cells__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Cells)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Cells__Changed;
        public string Cells__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Cells__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Cells__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(10);
            set
            {
            }
        }
    }
}