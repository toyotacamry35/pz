// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeSyncerEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private long _LastServerTime;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public long LastServerTime
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _LastServerTime, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _LastServerTime, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public long LastServerTime__Serialized
        {
            get
            {
                return _LastServerTime;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _LastServerTime, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate LastServerTime__Changed;
        public string LastServerTime__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(LastServerTime__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool LastServerTime__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(10);
            set
            {
            }
        }
    }
}