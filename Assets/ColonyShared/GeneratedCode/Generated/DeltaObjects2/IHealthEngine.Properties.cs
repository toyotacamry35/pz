// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class HealthEngine
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _AreadyInvokeZerohealth;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool AreadyInvokeZerohealth
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _AreadyInvokeZerohealth, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _AreadyInvokeZerohealth, value, SharedCode.EntitySystem.ReplicationLevel.Master, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool AreadyInvokeZerohealth__Serialized
        {
            get
            {
                return _AreadyInvokeZerohealth;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _AreadyInvokeZerohealth, value, SharedCode.EntitySystem.ReplicationLevel.Master, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate AreadyInvokeZerohealth__Changed;
        public string AreadyInvokeZerohealth__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(AreadyInvokeZerohealth__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool AreadyInvokeZerohealth__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(10);
            set
            {
            }
        }
    }
}