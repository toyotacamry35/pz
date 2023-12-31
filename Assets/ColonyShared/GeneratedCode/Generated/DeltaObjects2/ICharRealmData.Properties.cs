// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class CharRealmData
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> _CurrentRealm;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> CurrentRealm
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CurrentRealm, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CurrentRealm, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> CurrentRealm__Serialized
        {
            get
            {
                return _CurrentRealm;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CurrentRealm, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CurrentRealm__Changed;
        public string CurrentRealm__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CurrentRealm__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CurrentRealm__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.RealmCharStateEnum _CurrentRealmCharState;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.RealmCharStateEnum CurrentRealmCharState
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CurrentRealmCharState, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CurrentRealmCharState, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Entities.RealmCharStateEnum CurrentRealmCharState__Serialized
        {
            get
            {
                return _CurrentRealmCharState;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CurrentRealmCharState, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CurrentRealmCharState__Changed;
        public string CurrentRealmCharState__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CurrentRealmCharState__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CurrentRealmCharState__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Aspects.Sessions.RealmRulesDef _CurrentRealmRulesCached;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Aspects.Sessions.RealmRulesDef CurrentRealmRulesCached
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CurrentRealmRulesCached, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CurrentRealmRulesCached, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Aspects.Sessions.RealmRulesDef CurrentRealmRulesCached__Serialized
        {
            get
            {
                return _CurrentRealmRulesCached;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CurrentRealmRulesCached, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CurrentRealmRulesCached__Changed;
        public string CurrentRealmRulesCached__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CurrentRealmRulesCached__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CurrentRealmRulesCached__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(12);
            set
            {
            }
        }
    }
}