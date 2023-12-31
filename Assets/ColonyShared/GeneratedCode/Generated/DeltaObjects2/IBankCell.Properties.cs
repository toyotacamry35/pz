// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class BankCell
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IOwnerInformation _OwnerInformation;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IOwnerInformation OwnerInformation
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _OwnerInformation, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _OwnerInformation, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? OwnerInformation__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_OwnerInformation)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate OwnerInformation__Changed;
        public string OwnerInformation__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(OwnerInformation__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool OwnerInformation__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.DeltaObjects.IContainer _Inventory;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.DeltaObjects.IContainer Inventory
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Inventory, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Inventory, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Inventory__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Inventory)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 11, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Inventory__Changed;
        public string Inventory__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Inventory__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Inventory__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IOpenMechanics _OpenMechanics;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IOpenMechanics OpenMechanics
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _OpenMechanics, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _OpenMechanics, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? OpenMechanics__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_OpenMechanics)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 12, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate OpenMechanics__Changed;
        public string OpenMechanics__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(OpenMechanics__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool OpenMechanics__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IContainerApi _ContainerApi;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IContainerApi ContainerApi
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ContainerApi, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ContainerApi, value, SharedCode.EntitySystem.ReplicationLevel.Always, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? ContainerApi__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_ContainerApi)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 13, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ContainerApi__Changed;
        public string ContainerApi__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ContainerApi__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ContainerApi__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(13);
            set
            {
            }
        }
    }
}