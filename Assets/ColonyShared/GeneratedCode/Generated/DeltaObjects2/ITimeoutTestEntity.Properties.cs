// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutTestEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<int> _TestDeltaListInt;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<int> TestDeltaListInt
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaListInt, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaListInt, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaListInt__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaListInt)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaListInt__Changed;
        public string TestDeltaListInt__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaListInt__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaListInt__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<GeneratedCode.EntityModel.Test.ITestDeltaObject> _TestDeltaListDeltaObject;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<GeneratedCode.EntityModel.Test.ITestDeltaObject> TestDeltaListDeltaObject
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaListDeltaObject, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaListDeltaObject, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaListDeltaObject__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaListDeltaObject)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 11, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaListDeltaObject__Changed;
        public string TestDeltaListDeltaObject__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaListDeltaObject__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaListDeltaObject__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<int, int> _TestDeltaDictionaryInt;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<int, int> TestDeltaDictionaryInt
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaDictionaryInt, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaDictionaryInt, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaDictionaryInt__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaDictionaryInt)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 12, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaDictionaryInt__Changed;
        public string TestDeltaDictionaryInt__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaDictionaryInt__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaDictionaryInt__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<int, GeneratedCode.EntityModel.Test.ITestDeltaObject> _TestDeltaDictionaryIntDeltaObject;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<int, GeneratedCode.EntityModel.Test.ITestDeltaObject> TestDeltaDictionaryIntDeltaObject
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaDictionaryIntDeltaObject, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaDictionaryIntDeltaObject, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaDictionaryIntDeltaObject__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaDictionaryIntDeltaObject)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 13, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaDictionaryIntDeltaObject__Changed;
        public string TestDeltaDictionaryIntDeltaObject__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaDictionaryIntDeltaObject__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaDictionaryIntDeltaObject__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _TestProperty;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int TestProperty
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestProperty, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestProperty, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int TestProperty__Serialized
        {
            get
            {
                return _TestProperty;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _TestProperty, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestProperty__Changed;
        public string TestProperty__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestProperty__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestProperty__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(14);
            set
            {
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class TestDeltaObject
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Value;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Value
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Value, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Value, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Value__Serialized
        {
            get
            {
                return _Value;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Value, value, SharedCode.EntitySystem.ReplicationLevel.Always, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Value__Changed;
        public string Value__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Value__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Value__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(10);
            set
            {
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeoutSubTestEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<int> _TestDeltaListInt;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<int> TestDeltaListInt
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaListInt, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaListInt, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaListInt__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaListInt)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 10, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaListInt__Changed;
        public string TestDeltaListInt__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaListInt__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaListInt__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<GeneratedCode.EntityModel.Test.ITestDeltaObject> _TestDeltaListDeltaObject;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<GeneratedCode.EntityModel.Test.ITestDeltaObject> TestDeltaListDeltaObject
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaListDeltaObject, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaListDeltaObject, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaListDeltaObject__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaListDeltaObject)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 11, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaListDeltaObject__Changed;
        public string TestDeltaListDeltaObject__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaListDeltaObject__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaListDeltaObject__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<int, int> _TestDeltaDictionaryInt;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<int, int> TestDeltaDictionaryInt
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaDictionaryInt, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaDictionaryInt, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaDictionaryInt__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaDictionaryInt)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 12, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaDictionaryInt__Changed;
        public string TestDeltaDictionaryInt__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaDictionaryInt__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaDictionaryInt__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<int, GeneratedCode.EntityModel.Test.ITestDeltaObject> _TestDeltaDictionaryIntDeltaObject;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<int, GeneratedCode.EntityModel.Test.ITestDeltaObject> TestDeltaDictionaryIntDeltaObject
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestDeltaDictionaryIntDeltaObject, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestDeltaDictionaryIntDeltaObject, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? TestDeltaDictionaryIntDeltaObject__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_TestDeltaDictionaryIntDeltaObject)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 13, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestDeltaDictionaryIntDeltaObject__Changed;
        public string TestDeltaDictionaryIntDeltaObject__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestDeltaDictionaryIntDeltaObject__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestDeltaDictionaryIntDeltaObject__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _TestProperty;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int TestProperty
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TestProperty, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TestProperty, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int TestProperty__Serialized
        {
            get
            {
                return _TestProperty;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _TestProperty, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TestProperty__Changed;
        public string TestProperty__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TestProperty__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TestProperty__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(14);
            set
            {
            }
        }
    }
}