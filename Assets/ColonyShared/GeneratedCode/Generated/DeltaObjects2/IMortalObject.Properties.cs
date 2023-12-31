// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class Mortal
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _IsAlive;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsAlive
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _IsAlive, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _IsAlive, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool IsAlive__Serialized
        {
            get
            {
                return _IsAlive;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _IsAlive, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate IsAlive__Changed;
        public string IsAlive__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(IsAlive__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsAlive__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _PermaDead;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool PermaDead
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _PermaDead, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _PermaDead, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool PermaDead__Serialized
        {
            get
            {
                return _PermaDead;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _PermaDead, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate PermaDead__Changed;
        public string PermaDead__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(PermaDead__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool PermaDead__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _IsKnockedDown;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsKnockedDown
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _IsKnockedDown, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _IsKnockedDown, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsKnockedDown__Serialized
        {
            get
            {
                return _IsKnockedDown;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _IsKnockedDown, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate IsKnockedDown__Changed;
        public string IsKnockedDown__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(IsKnockedDown__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsKnockedDown__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Wizardry.SpellId _KnockDownSpellId;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Wizardry.SpellId KnockDownSpellId
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _KnockDownSpellId, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _KnockDownSpellId, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Wizardry.SpellId KnockDownSpellId__Serialized
        {
            get
            {
                return _KnockDownSpellId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _KnockDownSpellId, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate KnockDownSpellId__Changed;
        public string KnockDownSpellId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(KnockDownSpellId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool KnockDownSpellId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private long _LastResurrectTime;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public long LastResurrectTime
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _LastResurrectTime, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _LastResurrectTime, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public long LastResurrectTime__Serialized
        {
            get
            {
                return _LastResurrectTime;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _LastResurrectTime, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate LastResurrectTime__Changed;
        public string LastResurrectTime__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(LastResurrectTime__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool LastResurrectTime__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private long _LastDeathTime;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public long LastDeathTime
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _LastDeathTime, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _LastDeathTime, value, SharedCode.EntitySystem.ReplicationLevel.Master, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = false, DynamicType = false, OverwriteList = false)]
        public long LastDeathTime__Serialized
        {
            get
            {
                return _LastDeathTime;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _LastDeathTime, value, SharedCode.EntitySystem.ReplicationLevel.Master, 15, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate LastDeathTime__Changed;
        public string LastDeathTime__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(LastDeathTime__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool LastDeathTime__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, long> _LastStrike;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, long> LastStrike
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _LastStrike, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _LastStrike, value, SharedCode.EntitySystem.ReplicationLevel.Master, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? LastStrike__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_LastStrike)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 16, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate LastStrike__Changed;
        public string LastStrike__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(LastStrike__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool LastStrike__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(16);
            set
            {
            }
        }
    }
}