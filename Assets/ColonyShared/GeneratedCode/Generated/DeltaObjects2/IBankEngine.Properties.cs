// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class BankEngine
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ResourceSystem.Aspects.Banks.BankDef _BankDef;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ResourceSystem.Aspects.Banks.BankDef BankDef
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _BankDef, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _BankDef, value, SharedCode.EntitySystem.ReplicationLevel.Server, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public Assets.ResourceSystem.Aspects.Banks.BankDef BankDef__Serialized
        {
            get
            {
                return _BankDef;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _BankDef, value, SharedCode.EntitySystem.ReplicationLevel.Server, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate BankDef__Changed;
        public string BankDef__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(BankDef__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool BankDef__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(10);
            set
            {
            }
        }
    }
}