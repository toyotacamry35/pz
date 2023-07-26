// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterDoll
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<int> _BlockedSlotsId;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<int> BlockedSlotsId
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _BlockedSlotsId, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _BlockedSlotsId, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public ulong? BlockedSlotsId__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_BlockedSlotsId)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate BlockedSlotsId__Changed;
        public string BlockedSlotsId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(BlockedSlotsId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool BlockedSlotsId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<Assets.Src.ResourcesSystem.Base.ResourceIDFull> _UsedSlots;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<Assets.Src.ResourcesSystem.Base.ResourceIDFull> UsedSlots
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _UsedSlots, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _UsedSlots, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? UsedSlots__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_UsedSlots)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 11, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate UsedSlots__Changed;
        public string UsedSlots__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(UsedSlots__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool UsedSlots__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<int, SharedCode.DeltaObjects.ISlotItem> _Items;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<int, SharedCode.DeltaObjects.ISlotItem> Items
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Items, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Items, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Items__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Items)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 12, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Items__Changed;
        public string Items__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Items__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Items__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Size;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Size
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Size, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Size, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Size__Serialized
        {
            get
            {
                return _Size;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Size, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Size__Changed;
        public string Size__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Size__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Size__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Collections.Concurrent.ConcurrentDictionary<int, System.Guid> _TransactionReservedSlots;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Collections.Concurrent.ConcurrentDictionary<int, System.Guid> TransactionReservedSlots
        {
            get;
            set;
        }
    }
}