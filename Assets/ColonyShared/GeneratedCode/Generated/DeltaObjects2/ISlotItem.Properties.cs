// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SlotItem
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Stack;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Stack
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Stack, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Stack, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Stack__Serialized
        {
            get
            {
                return _Stack;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Stack, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Stack__Changed;
        public string Stack__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Stack__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Stack__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IItem _Item;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IItem Item
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Item, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Item, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Item__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Item)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 11, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Item__Changed;
        public string Item__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Item__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Item__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(11);
            set
            {
            }
        }
    }
}