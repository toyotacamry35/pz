// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class AccountTypeServiceEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, long> _AccountTypes;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<System.Guid, long> AccountTypes
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _AccountTypes, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _AccountTypes, value, SharedCode.EntitySystem.ReplicationLevel.Server, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? AccountTypes__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_AccountTypes)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate AccountTypes__Changed;
        public string AccountTypes__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(AccountTypes__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool AccountTypes__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server) && IsDirty(10);
            set
            {
            }
        }
    }
}