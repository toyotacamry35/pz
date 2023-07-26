// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class PositionedBuildingElement
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Utils.Vector3Int _Block;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Utils.Vector3Int Block
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Block, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Block, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Utils.Vector3Int Block__Serialized
        {
            get
            {
                return _Block;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Block, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Block__Changed;
        public string Block__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Block__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Block__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.DeltaObjects.Building.BuildingElementType _Type;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.DeltaObjects.Building.BuildingElementType Type
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Type, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Type, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.DeltaObjects.Building.BuildingElementType Type__Serialized
        {
            get
            {
                return _Type;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Type, value, SharedCode.EntitySystem.ReplicationLevel.Always, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Type__Changed;
        public string Type__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Type__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Type__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.DeltaObjects.Building.BuildingElementFace _Face;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.DeltaObjects.Building.BuildingElementFace Face
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Face, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Face, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.DeltaObjects.Building.BuildingElementFace Face__Serialized
        {
            get
            {
                return _Face;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Face, value, SharedCode.EntitySystem.ReplicationLevel.Always, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Face__Changed;
        public string Face__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Face__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Face__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.DeltaObjects.Building.BuildingElementSide _Side;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.DeltaObjects.Building.BuildingElementSide Side
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Side, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Side, value, SharedCode.EntitySystem.ReplicationLevel.Always, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.DeltaObjects.Building.BuildingElementSide Side__Serialized
        {
            get
            {
                return _Side;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Side, value, SharedCode.EntitySystem.ReplicationLevel.Always, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Side__Changed;
        public string Side__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Side__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Side__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.DeltaObjects.Building.BuildState _State;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.DeltaObjects.Building.BuildState State
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _State, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _State, value, SharedCode.EntitySystem.ReplicationLevel.Always, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.DeltaObjects.Building.BuildState State__Serialized
        {
            get
            {
                return _State;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _State, value, SharedCode.EntitySystem.ReplicationLevel.Always, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate State__Changed;
        public string State__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(State__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool State__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> _Owner;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> Owner
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Owner, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Owner, value, SharedCode.EntitySystem.ReplicationLevel.Always, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> Owner__Serialized
        {
            get
            {
                return _Owner;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Owner, value, SharedCode.EntitySystem.ReplicationLevel.Always, 15, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Owner__Changed;
        public string Owner__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Owner__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Owner__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Aspects.Building.BuildRecipeDef _RecipeDef;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Aspects.Building.BuildRecipeDef RecipeDef
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _RecipeDef, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _RecipeDef, value, SharedCode.EntitySystem.ReplicationLevel.Always, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Aspects.Building.BuildRecipeDef RecipeDef__Serialized
        {
            get
            {
                return _RecipeDef;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _RecipeDef, value, SharedCode.EntitySystem.ReplicationLevel.Always, 16, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate RecipeDef__Changed;
        public string RecipeDef__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(RecipeDef__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool RecipeDef__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(16);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Utils.Vector3 _Position;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Utils.Vector3 Position
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Position, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Position, value, SharedCode.EntitySystem.ReplicationLevel.Always, 17, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(17, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Utils.Vector3 Position__Serialized
        {
            get
            {
                return _Position;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Position, value, SharedCode.EntitySystem.ReplicationLevel.Always, 17, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Position__Changed;
        public string Position__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Position__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Position__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(17);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Utils.Quaternion _Rotation;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Utils.Quaternion Rotation
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Rotation, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Rotation, value, SharedCode.EntitySystem.ReplicationLevel.Always, 18, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(18, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Utils.Quaternion Rotation__Serialized
        {
            get
            {
                return _Rotation;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Rotation, value, SharedCode.EntitySystem.ReplicationLevel.Always, 18, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Rotation__Changed;
        public string Rotation__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Rotation__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Rotation__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(18);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Depth;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Depth
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Depth, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Depth, value, SharedCode.EntitySystem.ReplicationLevel.Always, 19, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(19, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Depth__Serialized
        {
            get
            {
                return _Depth;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Depth, value, SharedCode.EntitySystem.ReplicationLevel.Always, 19, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Depth__Changed;
        public string Depth__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Depth__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Depth__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(19);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.ChainCalls.ChainCancellationToken _BuildToken;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.ChainCalls.ChainCancellationToken BuildToken
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _BuildToken, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _BuildToken, value, SharedCode.EntitySystem.ReplicationLevel.Server, 20, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(20, AsReference = true, DynamicType = false, OverwriteList = false)]
        public SharedCode.EntitySystem.ChainCalls.ChainCancellationToken BuildToken__Serialized
        {
            get
            {
                return _BuildToken;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _BuildToken, value, SharedCode.EntitySystem.ReplicationLevel.Server, 20, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate BuildToken__Changed;
        public string BuildToken__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(BuildToken__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool BuildToken__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(20);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private long _BuildTimestamp;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public long BuildTimestamp
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _BuildTimestamp, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _BuildTimestamp, value, SharedCode.EntitySystem.ReplicationLevel.Always, 21, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(21, AsReference = false, DynamicType = false, OverwriteList = false)]
        public long BuildTimestamp__Serialized
        {
            get
            {
                return _BuildTimestamp;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _BuildTimestamp, value, SharedCode.EntitySystem.ReplicationLevel.Always, 21, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate BuildTimestamp__Changed;
        public string BuildTimestamp__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(BuildTimestamp__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool BuildTimestamp__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(21);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private float _BuildTime;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public float BuildTime
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _BuildTime, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _BuildTime, value, SharedCode.EntitySystem.ReplicationLevel.Always, 22, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(22, AsReference = false, DynamicType = false, OverwriteList = false)]
        public float BuildTime__Serialized
        {
            get
            {
                return _BuildTime;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _BuildTime, value, SharedCode.EntitySystem.ReplicationLevel.Always, 22, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate BuildTime__Changed;
        public string BuildTime__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(BuildTime__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool BuildTime__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(22);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private float _Health;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public float Health
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Health, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Health, value, SharedCode.EntitySystem.ReplicationLevel.Always, 23, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(23, AsReference = false, DynamicType = false, OverwriteList = false)]
        public float Health__Serialized
        {
            get
            {
                return _Health;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Health, value, SharedCode.EntitySystem.ReplicationLevel.Always, 23, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Health__Changed;
        public string Health__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Health__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Health__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(23);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Interaction;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Interaction
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Interaction, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Interaction, value, SharedCode.EntitySystem.ReplicationLevel.Always, 24, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(24, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Interaction__Serialized
        {
            get
            {
                return _Interaction;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Interaction, value, SharedCode.EntitySystem.ReplicationLevel.Always, 24, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Interaction__Changed;
        public string Interaction__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Interaction__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Interaction__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(24);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Visual;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Visual
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Visual, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Visual, value, SharedCode.EntitySystem.ReplicationLevel.Always, 25, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(25, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Visual__Serialized
        {
            get
            {
                return _Visual;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Visual, value, SharedCode.EntitySystem.ReplicationLevel.Always, 25, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Visual__Changed;
        public string Visual__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Visual__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Visual__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(25);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Id, value, SharedCode.EntitySystem.ReplicationLevel.Always, 26, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(26, AsReference = false, DynamicType = false, OverwriteList = false)]
        public System.Guid Id__Serialized
        {
            get
            {
                return _Id;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Id, value, SharedCode.EntitySystem.ReplicationLevel.Always, 26, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Id__Changed;
        public string Id__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Id__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Id__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(26);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IPositionHistory _PositionHistory;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IPositionHistory PositionHistory
        {
            get;
            set;
        }
    }
}