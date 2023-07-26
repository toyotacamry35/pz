// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SimpleWorldEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private string _Name;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string Name
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Name, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Name, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public string Name__Serialized
        {
            get
            {
                return _Name;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Name, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Name__Changed;
        public string Name__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Name__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Name__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private string _Prefab;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string Prefab
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Prefab, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Prefab, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public string Prefab__Serialized
        {
            get
            {
                return _Prefab;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Prefab, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Prefab__Changed;
        public string Prefab__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Prefab__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Prefab__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.Src.ResourcesSystem.Base.ISaveableResource _SomeUnknownResourceThatMayBeUseful;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _SomeUnknownResourceThatMayBeUseful, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _SomeUnknownResourceThatMayBeUseful, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful__Serialized
        {
            get
            {
                return _SomeUnknownResourceThatMayBeUseful;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _SomeUnknownResourceThatMayBeUseful, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate SomeUnknownResourceThatMayBeUseful__Changed;
        public string SomeUnknownResourceThatMayBeUseful__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(SomeUnknownResourceThatMayBeUseful__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool SomeUnknownResourceThatMayBeUseful__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.OnSceneObjectNetId _OnSceneObjectNetId;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _OnSceneObjectNetId, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _OnSceneObjectNetId, value, SharedCode.EntitySystem.ReplicationLevel.Always, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId__Serialized
        {
            get
            {
                return _OnSceneObjectNetId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _OnSceneObjectNetId, value, SharedCode.EntitySystem.ReplicationLevel.Always, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate OnSceneObjectNetId__Changed;
        public string OnSceneObjectNetId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(OnSceneObjectNetId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool OnSceneObjectNetId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IAutoAddToWorldSpace _AutoAddToWorldSpace;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IAutoAddToWorldSpace AutoAddToWorldSpace
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _AutoAddToWorldSpace, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _AutoAddToWorldSpace, value, SharedCode.EntitySystem.ReplicationLevel.Master, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? AutoAddToWorldSpace__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_AutoAddToWorldSpace)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 14, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate AutoAddToWorldSpace__Changed;
        public string AutoAddToWorldSpace__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(AutoAddToWorldSpace__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool AutoAddToWorldSpace__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Service.IWorldSpaced _WorldSpaced;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Service.IWorldSpaced WorldSpaced
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _WorldSpaced, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _WorldSpaced, value, SharedCode.EntitySystem.ReplicationLevel.Always, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? WorldSpaced__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaced)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 15, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate WorldSpaced__Changed;
        public string WorldSpaced__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(WorldSpaced__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool WorldSpaced__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.MovementSync.ISimpleMovementSync _MovementSync;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.MovementSync.ISimpleMovementSync MovementSync
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _MovementSync, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _MovementSync, value, SharedCode.EntitySystem.ReplicationLevel.Always, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? MovementSync__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_MovementSync)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 16, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate MovementSync__Changed;
        public string MovementSync__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(MovementSync__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool MovementSync__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(16);
            set
            {
            }
        }
    }
}