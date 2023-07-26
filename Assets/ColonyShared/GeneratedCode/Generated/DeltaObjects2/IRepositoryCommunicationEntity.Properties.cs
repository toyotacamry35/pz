// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class RepositoryCommunicationEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Config.EntitiesRepositoryConfig _Config;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Config.EntitiesRepositoryConfig Config
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Config, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Config, value, SharedCode.EntitySystem.ReplicationLevel.Master, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Config.EntitiesRepositoryConfig Config__Serialized
        {
            get
            {
                return _Config;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Config, value, SharedCode.EntitySystem.ReplicationLevel.Master, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Config__Changed;
        public string Config__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Config__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Config__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private string _ConfigId;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string ConfigId
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ConfigId, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ConfigId, value, SharedCode.EntitySystem.ReplicationLevel.Server, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public string ConfigId__Serialized
        {
            get
            {
                return _ConfigId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _ConfigId, value, SharedCode.EntitySystem.ReplicationLevel.Server, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ConfigId__Changed;
        public string ConfigId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ConfigId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ConfigId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Num;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Num
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Num, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Num, value, SharedCode.EntitySystem.ReplicationLevel.Server, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Num__Serialized
        {
            get
            {
                return _Num;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Num, value, SharedCode.EntitySystem.ReplicationLevel.Server, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Num__Changed;
        public string Num__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Num__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Num__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Cloud.EndpointAddress _InternalAddress;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Cloud.EndpointAddress InternalAddress
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _InternalAddress, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _InternalAddress, value, SharedCode.EntitySystem.ReplicationLevel.Server, 13, true);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Entities.Cloud.EndpointAddress InternalAddress__Serialized
        {
            get
            {
                return _InternalAddress;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _InternalAddress, value, SharedCode.EntitySystem.ReplicationLevel.Server, 13, true);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate InternalAddress__Changed;
        public string InternalAddress__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(InternalAddress__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool InternalAddress__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Cloud.EndpointAddress _ExternalAddress;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Cloud.EndpointAddress ExternalAddress
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ExternalAddress, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ExternalAddress, value, SharedCode.EntitySystem.ReplicationLevel.Server, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Entities.Cloud.EndpointAddress ExternalAddress__Serialized
        {
            get
            {
                return _ExternalAddress;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _ExternalAddress, value, SharedCode.EntitySystem.ReplicationLevel.Server, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ExternalAddress__Changed;
        public string ExternalAddress__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ExternalAddress__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ExternalAddress__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _CloudRequirementsMet;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CloudRequirementsMet
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CloudRequirementsMet, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CloudRequirementsMet, value, SharedCode.EntitySystem.ReplicationLevel.Server, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool CloudRequirementsMet__Serialized
        {
            get
            {
                return _CloudRequirementsMet;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CloudRequirementsMet, value, SharedCode.EntitySystem.ReplicationLevel.Server, 15, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CloudRequirementsMet__Changed;
        public string CloudRequirementsMet__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CloudRequirementsMet__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CloudRequirementsMet__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _InitializationTasksCompleted;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool InitializationTasksCompleted
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _InitializationTasksCompleted, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _InitializationTasksCompleted, value, SharedCode.EntitySystem.ReplicationLevel.Server, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool InitializationTasksCompleted__Serialized
        {
            get
            {
                return _InitializationTasksCompleted;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _InitializationTasksCompleted, value, SharedCode.EntitySystem.ReplicationLevel.Server, 16, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate InitializationTasksCompleted__Changed;
        public string InitializationTasksCompleted__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(InitializationTasksCompleted__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool InitializationTasksCompleted__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(16);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _ExternalCommunicationNodeOpen;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ExternalCommunicationNodeOpen
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ExternalCommunicationNodeOpen, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ExternalCommunicationNodeOpen, value, SharedCode.EntitySystem.ReplicationLevel.Server, 17, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(17, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool ExternalCommunicationNodeOpen__Serialized
        {
            get
            {
                return _ExternalCommunicationNodeOpen;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _ExternalCommunicationNodeOpen, value, SharedCode.EntitySystem.ReplicationLevel.Server, 17, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ExternalCommunicationNodeOpen__Changed;
        public string ExternalCommunicationNodeOpen__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ExternalCommunicationNodeOpen__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ExternalCommunicationNodeOpen__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(17);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Cloud.CloudNodeType _CloudNodeType;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Cloud.CloudNodeType CloudNodeType
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CloudNodeType, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CloudNodeType, value, SharedCode.EntitySystem.ReplicationLevel.Always, 18, true);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(18, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Cloud.CloudNodeType CloudNodeType__Serialized
        {
            get
            {
                return _CloudNodeType;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CloudNodeType, value, SharedCode.EntitySystem.ReplicationLevel.Always, 18, true);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CloudNodeType__Changed;
        public string CloudNodeType__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CloudNodeType__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CloudNodeType__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(18);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _ProcessId;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int ProcessId
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ProcessId, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ProcessId, value, SharedCode.EntitySystem.ReplicationLevel.Server, 19, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(19, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int ProcessId__Serialized
        {
            get
            {
                return _ProcessId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _ProcessId, value, SharedCode.EntitySystem.ReplicationLevel.Server, 19, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ProcessId__Changed;
        public string ProcessId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ProcessId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ProcessId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(19);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IPingDiagnostics _PingDiagnostics;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IPingDiagnostics PingDiagnostics
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _PingDiagnostics, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _PingDiagnostics, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 20, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(20, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? PingDiagnostics__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_PingDiagnostics)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 20, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate PingDiagnostics__Changed;
        public string PingDiagnostics__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(PingDiagnostics__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool PingDiagnostics__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(20);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Cloud.WantsToDisconnectEventProxy _WantsToDisconnect;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Cloud.WantsToDisconnectEventProxy WantsToDisconnect
        {
            get;
            set;
        }
    }
}