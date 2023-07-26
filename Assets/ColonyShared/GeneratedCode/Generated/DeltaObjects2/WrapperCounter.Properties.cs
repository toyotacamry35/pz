// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WrapperCounter
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Engine.IQuestCounter _SubCounter;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Engine.IQuestCounter SubCounter
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _SubCounter, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _SubCounter, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = true, OverwriteList = false)]
        public ulong? SubCounter__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_SubCounter)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate SubCounter__Changed;
        public string SubCounter__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(SubCounter__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool SubCounter__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.Src.Aspects.Impl.Factions.Template.QuestDef _QuestDef;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.Src.Aspects.Impl.Factions.Template.QuestDef QuestDef
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _QuestDef, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _QuestDef, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public Assets.Src.Aspects.Impl.Factions.Template.QuestDef QuestDef__Serialized
        {
            get
            {
                return _QuestDef;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _QuestDef, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate QuestDef__Changed;
        public string QuestDef__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(QuestDef__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool QuestDef__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef _CounterDef;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef CounterDef
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CounterDef, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CounterDef, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, true);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        public Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef CounterDef__Serialized
        {
            get
            {
                return _CounterDef;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CounterDef, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, true);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CounterDef__Changed;
        public string CounterDef__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CounterDef__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CounterDef__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Count;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Count
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Count, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Count, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Count__Serialized
        {
            get
            {
                return _Count;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Count, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Count__Changed;
        public string Count__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Count__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Count__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _CountForClient;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int CountForClient
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CountForClient, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CountForClient, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int CountForClient__Serialized
        {
            get
            {
                return _CountForClient;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CountForClient, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CountForClient__Changed;
        public string CountForClient__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CountForClient__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CountForClient__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _Completed;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Completed
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Completed, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Completed, value, SharedCode.EntitySystem.ReplicationLevel.Master, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool Completed__Serialized
        {
            get
            {
                return _Completed;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Completed, value, SharedCode.EntitySystem.ReplicationLevel.Master, 15, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Completed__Changed;
        public string Completed__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Completed__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Completed__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _PreventOnComplete;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool PreventOnComplete
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _PreventOnComplete, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _PreventOnComplete, value, SharedCode.EntitySystem.ReplicationLevel.Master, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool PreventOnComplete__Serialized
        {
            get
            {
                return _PreventOnComplete;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _PreventOnComplete, value, SharedCode.EntitySystem.ReplicationLevel.Master, 16, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate PreventOnComplete__Changed;
        public string PreventOnComplete__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(PreventOnComplete__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool PreventOnComplete__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(16);
            set
            {
            }
        }
    }
}