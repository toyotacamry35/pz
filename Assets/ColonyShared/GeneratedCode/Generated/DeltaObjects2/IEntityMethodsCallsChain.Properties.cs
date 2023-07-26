// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class EntityMethodsCallsChain
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Core.IChainContext _ChainContext;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Core.IChainContext ChainContext
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ChainContext, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ChainContext, value, SharedCode.EntitySystem.ReplicationLevel.Master, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? ChainContext__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_ChainContext)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ChainContext__Changed;
        public string ChainContext__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ChainContext__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ChainContext__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<SharedCode.EntitySystem.ChainCalls.ChainBlockBase> _Chain;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<SharedCode.EntitySystem.ChainCalls.ChainBlockBase> Chain
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Chain, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Chain, value, SharedCode.EntitySystem.ReplicationLevel.Master, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Chain__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Chain)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 11, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Chain__Changed;
        public string Chain__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Chain__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Chain__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _CurrentChainIndex;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int CurrentChainIndex
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CurrentChainIndex, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CurrentChainIndex, value, SharedCode.EntitySystem.ReplicationLevel.Master, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int CurrentChainIndex__Serialized
        {
            get
            {
                return _CurrentChainIndex;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CurrentChainIndex, value, SharedCode.EntitySystem.ReplicationLevel.Master, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CurrentChainIndex__Changed;
        public string CurrentChainIndex__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CurrentChainIndex__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CurrentChainIndex__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private long _NextTimeToCall;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public long NextTimeToCall
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _NextTimeToCall, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _NextTimeToCall, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        public long NextTimeToCall__Serialized
        {
            get
            {
                return _NextTimeToCall;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _NextTimeToCall, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate NextTimeToCall__Changed;
        public string NextTimeToCall__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(NextTimeToCall__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool NextTimeToCall__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<System.Guid> _ForksIds;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<System.Guid> ForksIds
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ForksIds, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ForksIds, value, SharedCode.EntitySystem.ReplicationLevel.Master, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? ForksIds__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_ForksIds)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 14, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ForksIds__Changed;
        public string ForksIds__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ForksIds__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ForksIds__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Guid _ForkCreatorId;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Guid ForkCreatorId
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ForkCreatorId, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ForkCreatorId, value, SharedCode.EntitySystem.ReplicationLevel.Master, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = false, DynamicType = false, OverwriteList = false)]
        public System.Guid ForkCreatorId__Serialized
        {
            get
            {
                return _ForkCreatorId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _ForkCreatorId, value, SharedCode.EntitySystem.ReplicationLevel.Master, 15, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ForkCreatorId__Changed;
        public string ForkCreatorId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ForkCreatorId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ForkCreatorId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Guid _Id;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Guid Id
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Id, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Id, value, SharedCode.EntitySystem.ReplicationLevel.Always, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = false, DynamicType = false, OverwriteList = false)]
        public System.Guid Id__Serialized
        {
            get
            {
                return _Id;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Id, value, SharedCode.EntitySystem.ReplicationLevel.Always, 16, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Id__Changed;
        public string Id__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Id__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Id__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(16);
            set
            {
            }
        }
    }
}