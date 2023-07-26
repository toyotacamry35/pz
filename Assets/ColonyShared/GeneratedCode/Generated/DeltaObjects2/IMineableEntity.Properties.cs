// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class MineableEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private float _CurrProgressActualTime;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public float CurrProgressActualTime
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CurrProgressActualTime, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CurrProgressActualTime, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public float CurrProgressActualTime__Serialized
        {
            get
            {
                return _CurrProgressActualTime;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CurrProgressActualTime, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CurrProgressActualTime__Changed;
        public string CurrProgressActualTime__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CurrProgressActualTime__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CurrProgressActualTime__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Engine.IHealthEngine _Health;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Engine.IHealthEngine Health
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Health, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Health, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Health__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Health)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 11, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Health__Changed;
        public string Health__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Health__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Health__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ColonyShared.SharedCode.Entities.IDestroyable _Destroyable;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ColonyShared.SharedCode.Entities.IDestroyable Destroyable
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Destroyable, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Destroyable, value, SharedCode.EntitySystem.ReplicationLevel.Server, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Destroyable__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Destroyable)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 12, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Destroyable__Changed;
        public string Destroyable__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Destroyable__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Destroyable__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ColonyShared.SharedCode.Entities.IMortal _Mortal;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ColonyShared.SharedCode.Entities.IMortal Mortal
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Mortal, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Mortal, value, SharedCode.EntitySystem.ReplicationLevel.Always, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Mortal__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Mortal)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 13, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Mortal__Changed;
        public string Mortal__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Mortal__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Mortal__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ColonyShared.SharedCode.Entities.IBrute _Brute;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ColonyShared.SharedCode.Entities.IBrute Brute
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Brute, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Brute, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Brute__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Brute)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 14, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Brute__Changed;
        public string Brute__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Brute__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Brute__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.Engine.IStatsEngine _Stats;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.Engine.IStatsEngine Stats
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Stats, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Stats, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Stats__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Stats)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 15, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Stats__Changed;
        public string Stats__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Stats__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Stats__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ColonyShared.SharedCode.Entities.ILifespan _Lifespan;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ColonyShared.SharedCode.Entities.ILifespan Lifespan
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Lifespan, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Lifespan, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Lifespan__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Lifespan)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 16, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Lifespan__Changed;
        public string Lifespan__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Lifespan__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Lifespan__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(16);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine _ComputableStateMachine;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ColonyShared.SharedCode.Entities.IComputableStateMachine ComputableStateMachine
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ComputableStateMachine, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ComputableStateMachine, value, SharedCode.EntitySystem.ReplicationLevel.Always, 17, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(17, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? ComputableStateMachine__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_ComputableStateMachine)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 17, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate ComputableStateMachine__Changed;
        public string ComputableStateMachine__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ComputableStateMachine__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ComputableStateMachine__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(17);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.GameObjectEntities.ISpawnedObject _SpawnedObject;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.GameObjectEntities.ISpawnedObject SpawnedObject
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _SpawnedObject, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _SpawnedObject, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 18, true);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(18, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? SpawnedObject__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_SpawnedObject)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 18, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate SpawnedObject__Changed;
        public string SpawnedObject__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(SpawnedObject__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool SpawnedObject__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(18);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.GameObjectEntities.IEntityObjectDef _Def;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Def, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Def, value, SharedCode.EntitySystem.ReplicationLevel.Always, 19, true);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(19, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Entities.GameObjectEntities.IEntityObjectDef Def__Serialized
        {
            get
            {
                return _Def;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Def, value, SharedCode.EntitySystem.ReplicationLevel.Always, 19, true);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Def__Changed;
        public string Def__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Def__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Def__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(19);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private GeneratedCode.MapSystem.MapOwner _MapOwner;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public GeneratedCode.MapSystem.MapOwner MapOwner
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _MapOwner, true, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _MapOwner, value, SharedCode.EntitySystem.ReplicationLevel.Always, 20, false, true);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(20, AsReference = false, DynamicType = false, OverwriteList = false)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public GeneratedCode.MapSystem.MapOwner MapOwner__Serialized
        {
            get
            {
                return _MapOwner;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _MapOwner, value, SharedCode.EntitySystem.ReplicationLevel.Always, 20, false, true);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate MapOwner__Changed;
        public string MapOwner__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(MapOwner__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool MapOwner__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(20);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Guid _StaticIdFromExport;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Guid StaticIdFromExport
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _StaticIdFromExport, true);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _StaticIdFromExport, value, SharedCode.EntitySystem.ReplicationLevel.Always, 21, true);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(21, AsReference = false, DynamicType = false, OverwriteList = false)]
        public System.Guid StaticIdFromExport__Serialized
        {
            get
            {
                return _StaticIdFromExport;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _StaticIdFromExport, value, SharedCode.EntitySystem.ReplicationLevel.Always, 21, true);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate StaticIdFromExport__Changed;
        public string StaticIdFromExport__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(StaticIdFromExport__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool StaticIdFromExport__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(21);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private string _Name;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string Name
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Name, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Name, value, SharedCode.EntitySystem.ReplicationLevel.Always, 22, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(22, AsReference = false, DynamicType = false, OverwriteList = false)]
        public string Name__Serialized
        {
            get
            {
                return _Name;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Name, value, SharedCode.EntitySystem.ReplicationLevel.Always, 22, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Name__Changed;
        public string Name__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Name__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Name__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(22);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Prefab, value, SharedCode.EntitySystem.ReplicationLevel.Always, 23, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(23, AsReference = false, DynamicType = false, OverwriteList = false)]
        public string Prefab__Serialized
        {
            get
            {
                return _Prefab;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Prefab, value, SharedCode.EntitySystem.ReplicationLevel.Always, 23, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Prefab__Changed;
        public string Prefab__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Prefab__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Prefab__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(23);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _SomeUnknownResourceThatMayBeUseful, value, SharedCode.EntitySystem.ReplicationLevel.Always, 24, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(24, AsReference = false, DynamicType = false, OverwriteList = false)]
        public Assets.Src.ResourcesSystem.Base.ISaveableResource SomeUnknownResourceThatMayBeUseful__Serialized
        {
            get
            {
                return _SomeUnknownResourceThatMayBeUseful;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _SomeUnknownResourceThatMayBeUseful, value, SharedCode.EntitySystem.ReplicationLevel.Always, 24, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate SomeUnknownResourceThatMayBeUseful__Changed;
        public string SomeUnknownResourceThatMayBeUseful__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(SomeUnknownResourceThatMayBeUseful__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool SomeUnknownResourceThatMayBeUseful__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(24);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _OnSceneObjectNetId, value, SharedCode.EntitySystem.ReplicationLevel.Always, 25, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(25, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Entities.OnSceneObjectNetId OnSceneObjectNetId__Serialized
        {
            get
            {
                return _OnSceneObjectNetId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _OnSceneObjectNetId, value, SharedCode.EntitySystem.ReplicationLevel.Always, 25, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate OnSceneObjectNetId__Changed;
        public string OnSceneObjectNetId__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(OnSceneObjectNetId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool OnSceneObjectNetId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(25);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _AutoAddToWorldSpace, value, SharedCode.EntitySystem.ReplicationLevel.Master, 26, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(26, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? AutoAddToWorldSpace__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_AutoAddToWorldSpace)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 26, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate AutoAddToWorldSpace__Changed;
        public string AutoAddToWorldSpace__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(AutoAddToWorldSpace__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool AutoAddToWorldSpace__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(26);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _WorldSpaced, value, SharedCode.EntitySystem.ReplicationLevel.Always, 27, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(27, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? WorldSpaced__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaced)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 27, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate WorldSpaced__Changed;
        public string WorldSpaced__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(WorldSpaced__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool WorldSpaced__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(27);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _MovementSync, value, SharedCode.EntitySystem.ReplicationLevel.Always, 28, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(28, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? MovementSync__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_MovementSync)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 28, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate MovementSync__Changed;
        public string MovementSync__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(MovementSync__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool MovementSync__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(28);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IOwnerInformation _OwnerInformation;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IOwnerInformation OwnerInformation
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _OwnerInformation, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _OwnerInformation, value, SharedCode.EntitySystem.ReplicationLevel.Always, 29, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(29, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? OwnerInformation__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_OwnerInformation)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 29, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate OwnerInformation__Changed;
        public string OwnerInformation__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(OwnerInformation__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool OwnerInformation__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(29);
            set
            {
            }
        }
    }
}