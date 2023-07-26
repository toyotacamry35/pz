// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class AccountEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Experience;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Experience
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Experience, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Experience, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Experience__Serialized
        {
            get
            {
                return _Experience;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Experience, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Experience__Changed;
        public string Experience__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Experience__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Experience__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _UnconsumedExperience;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int UnconsumedExperience
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _UnconsumedExperience, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _UnconsumedExperience, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int UnconsumedExperience__Serialized
        {
            get
            {
                return _UnconsumedExperience;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _UnconsumedExperience, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate UnconsumedExperience__Changed;
        public string UnconsumedExperience__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(UnconsumedExperience__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool UnconsumedExperience__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.ISessionResult _LastSessionResult;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.ISessionResult LastSessionResult
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _LastSessionResult, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _LastSessionResult, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? LastSessionResult__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_LastSessionResult)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 12, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate LastSessionResult__Changed;
        public string LastSessionResult__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(LastSessionResult__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool LastSessionResult__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Aspects.Sessions.RealmRulesQueryDef, SharedCode.Entities.RealmRulesQueryState> _AvailableRealmQueries;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Aspects.Sessions.RealmRulesQueryDef, SharedCode.Entities.RealmRulesQueryState> AvailableRealmQueries
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _AvailableRealmQueries, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _AvailableRealmQueries, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = true, DynamicType = true, OverwriteList = false)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public ulong? AvailableRealmQueries__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_AvailableRealmQueries)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 13, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate AvailableRealmQueries__Changed;
        public string AvailableRealmQueries__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(AvailableRealmQueries__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool AvailableRealmQueries__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<SharedCode.Entities.IAccountCharacter> _Characters;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<SharedCode.Entities.IAccountCharacter> Characters
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Characters, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Characters, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Characters__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Characters)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 14, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Characters__Changed;
        public string Characters__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Characters__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Characters__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.ICharRealmData _CharRealmData;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.ICharRealmData CharRealmData
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CharRealmData, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CharRealmData, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? CharRealmData__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_CharRealmData)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 15, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CharRealmData__Changed;
        public string CharRealmData__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CharRealmData__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CharRealmData__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private string _AccountId;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string AccountId
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _AccountId, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _AccountId, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = false, DynamicType = false, OverwriteList = false)]
        public string AccountId__Serialized
        {
            get
            {
                return _AccountId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _AccountId, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 16, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate AccountId__Changed;
        public string AccountId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(AccountId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool AccountId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(16);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private ResourceSystem.Aspects.Misc.GenderDef _Gender;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public ResourceSystem.Aspects.Misc.GenderDef Gender
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Gender, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Gender, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 17, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(17, AsReference = false, DynamicType = false, OverwriteList = false)]
        public ResourceSystem.Aspects.Misc.GenderDef Gender__Serialized
        {
            get
            {
                return _Gender;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Gender, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 17, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Gender__Changed;
        public string Gender__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Gender__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Gender__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(17);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Guid _CurrentUserId;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Guid CurrentUserId
        {
            get;
            set;
        }
    }
}