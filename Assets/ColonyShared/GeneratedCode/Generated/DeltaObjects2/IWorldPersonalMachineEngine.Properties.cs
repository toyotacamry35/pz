// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldPersonalMachineEngine
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<ResourceSystem.Utils.OuterRef, ResourceSystem.Utils.OuterRef> _WorldPersonalMachines;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<ResourceSystem.Utils.OuterRef, ResourceSystem.Utils.OuterRef> WorldPersonalMachines
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _WorldPersonalMachines, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _WorldPersonalMachines, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? WorldPersonalMachines__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldPersonalMachines)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate WorldPersonalMachines__Changed;
        public string WorldPersonalMachines__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(WorldPersonalMachines__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool WorldPersonalMachines__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(10);
            set
            {
            }
        }
    }
}