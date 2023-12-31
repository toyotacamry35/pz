// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class CraftingQueueItem
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Index;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Index
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Index, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Index, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Index__Serialized
        {
            get
            {
                return _Index;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Index, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Index__Changed;
        public string Index__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Index__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Index__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef _CraftRecipe;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef CraftRecipe
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CraftRecipe, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CraftRecipe, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef CraftRecipe__Serialized
        {
            get
            {
                return _CraftRecipe;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CraftRecipe, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CraftRecipe__Changed;
        public string CraftRecipe__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CraftRecipe__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CraftRecipe__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Collections.Generic.List<int> _MandatorySlotPermutation;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Collections.Generic.List<int> MandatorySlotPermutation
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _MandatorySlotPermutation, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _MandatorySlotPermutation, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = true, DynamicType = false, OverwriteList = false)]
        public System.Collections.Generic.List<int> MandatorySlotPermutation__Serialized
        {
            get
            {
                return _MandatorySlotPermutation;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _MandatorySlotPermutation, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate MandatorySlotPermutation__Changed;
        public string MandatorySlotPermutation__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(MandatorySlotPermutation__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool MandatorySlotPermutation__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private System.Collections.Generic.List<int> _OptionalSlotPermutation;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public System.Collections.Generic.List<int> OptionalSlotPermutation
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _OptionalSlotPermutation, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _OptionalSlotPermutation, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = true, DynamicType = false, OverwriteList = false)]
        public System.Collections.Generic.List<int> OptionalSlotPermutation__Serialized
        {
            get
            {
                return _OptionalSlotPermutation;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _OptionalSlotPermutation, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate OptionalSlotPermutation__Changed;
        public string OptionalSlotPermutation__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(OptionalSlotPermutation__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool OptionalSlotPermutation__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _SelectedVariantIndex;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int SelectedVariantIndex
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _SelectedVariantIndex, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _SelectedVariantIndex, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int SelectedVariantIndex__Serialized
        {
            get
            {
                return _SelectedVariantIndex;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _SelectedVariantIndex, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate SelectedVariantIndex__Changed;
        public string SelectedVariantIndex__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(SelectedVariantIndex__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool SelectedVariantIndex__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private long _TimeAlreadyCrafted;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public long TimeAlreadyCrafted
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _TimeAlreadyCrafted, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _TimeAlreadyCrafted, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = false, DynamicType = false, OverwriteList = false)]
        public long TimeAlreadyCrafted__Serialized
        {
            get
            {
                return _TimeAlreadyCrafted;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _TimeAlreadyCrafted, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 15, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate TimeAlreadyCrafted__Changed;
        public string TimeAlreadyCrafted__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(TimeAlreadyCrafted__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool TimeAlreadyCrafted__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private long _CraftStartTime;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public long CraftStartTime
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CraftStartTime, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CraftStartTime, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = false, DynamicType = false, OverwriteList = false)]
        public long CraftStartTime__Serialized
        {
            get
            {
                return _CraftStartTime;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _CraftStartTime, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 16, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CraftStartTime__Changed;
        public string CraftStartTime__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CraftStartTime__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CraftStartTime__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(16);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _IsActive;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsActive
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _IsActive, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _IsActive, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 17, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(17, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool IsActive__Serialized
        {
            get
            {
                return _IsActive;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _IsActive, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 17, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate IsActive__Changed;
        public string IsActive__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(IsActive__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsActive__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(17);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private int _Count;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public int Count
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Count, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Count, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 18, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(18, AsReference = false, DynamicType = false, OverwriteList = false)]
        public int Count__Serialized
        {
            get
            {
                return _Count;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Count, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 18, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Count__Changed;
        public string Count__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Count__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Count__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(18);
            set
            {
            }
        }
    }
}