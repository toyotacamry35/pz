// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SimpleMovementSync
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Transform ___SyncTransform;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Transform __SyncTransform
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref ___SyncTransform, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref ___SyncTransform, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Entities.Transform __SyncTransform__Serialized
        {
            get
            {
                return ___SyncTransform;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref ___SyncTransform, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate __SyncTransform__Changed;
        public string __SyncTransform__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(__SyncTransform__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool __SyncTransform__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Type _GridSyncType;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Type GridSyncType
        {
            get;
            set;
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.MovementSync.SimpleMovementStateEvent _OnMovementStateChanged;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.MovementSync.SimpleMovementStateEvent OnMovementStateChanged
        {
            get;
            set;
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _VisibilityOff;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool VisibilityOff
        {
            get;
            set;
        }
    }
}