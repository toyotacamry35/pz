// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class ComputableStateMachine
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _IsPristineInternal;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsPristineInternal
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _IsPristineInternal, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _IsPristineInternal, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool IsPristineInternal__Serialized
        {
            get
            {
                return _IsPristineInternal;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _IsPristineInternal, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate IsPristineInternal__Changed;
        public string IsPristineInternal__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(IsPristineInternal__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsPristineInternal__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef _FixedStates;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef FixedStates
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _FixedStates, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _FixedStates, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public Assets.ColonyShared.SharedCode.Aspects.WorldObjects.ComputableStatesDef FixedStates__Serialized
        {
            get
            {
                return _FixedStates;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _FixedStates, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate FixedStates__Changed;
        public string FixedStates__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(FixedStates__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool FixedStates__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(11);
            set
            {
            }
        }
    }
}