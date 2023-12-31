// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WizardEntity
    {
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.OuterRef<SharedCode.Wizardry.IWizardEntity> _HostWizard;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.OuterRef<SharedCode.Wizardry.IWizardEntity> HostWizard
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _HostWizard, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _HostWizard, value, SharedCode.EntitySystem.ReplicationLevel.Master, 10, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(10, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.EntitySystem.OuterRef<SharedCode.Wizardry.IWizardEntity> HostWizard__Serialized
        {
            get
            {
                return _HostWizard;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _HostWizard, value, SharedCode.EntitySystem.ReplicationLevel.Master, 10, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate HostWizard__Changed;
        public string HostWizard__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(HostWizard__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool HostWizard__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(10);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _IsDead;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsDead
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _IsDead, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _IsDead, value, SharedCode.EntitySystem.ReplicationLevel.Master, 11, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(11, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool IsDead__Serialized
        {
            get
            {
                return _IsDead;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _IsDead, value, SharedCode.EntitySystem.ReplicationLevel.Master, 11, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate IsDead__Changed;
        public string IsDead__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(IsDead__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsDead__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(11);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private bool _IsInterestingEnoughToLog;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsInterestingEnoughToLog
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _IsInterestingEnoughToLog, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _IsInterestingEnoughToLog, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 12, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(12, AsReference = false, DynamicType = false, OverwriteList = false)]
        public bool IsInterestingEnoughToLog__Serialized
        {
            get
            {
                return _IsInterestingEnoughToLog;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _IsInterestingEnoughToLog, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 12, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate IsInterestingEnoughToLog__Changed;
        public string IsInterestingEnoughToLog__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(IsInterestingEnoughToLog__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool IsInterestingEnoughToLog__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(12);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Wizardry.SpellId _Counter;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Wizardry.SpellId Counter
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Counter, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Counter, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(13, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Wizardry.SpellId Counter__Serialized
        {
            get
            {
                return _Counter;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Counter, value, SharedCode.EntitySystem.ReplicationLevel.Master, 13, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Counter__Changed;
        public string Counter__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Counter__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Counter__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(13);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Wizardry.SpellId _SlaveCounter;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Wizardry.SpellId SlaveCounter
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _SlaveCounter, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _SlaveCounter, value, SharedCode.EntitySystem.ReplicationLevel.Master, 14, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(14, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.Wizardry.SpellId SlaveCounter__Serialized
        {
            get
            {
                return _SlaveCounter;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _SlaveCounter, value, SharedCode.EntitySystem.ReplicationLevel.Master, 14, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate SlaveCounter__Changed;
        public string SlaveCounter__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(SlaveCounter__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool SlaveCounter__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(14);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaList<SharedCode.Wizardry.SpellThatMustBeStoppedAtStart> _SpellsThatMustBeStoppedAtStart;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaList<SharedCode.Wizardry.SpellThatMustBeStoppedAtStart> SpellsThatMustBeStoppedAtStart
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _SpellsThatMustBeStoppedAtStart, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _SpellsThatMustBeStoppedAtStart, value, SharedCode.EntitySystem.ReplicationLevel.Master, 15, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(15, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? SpellsThatMustBeStoppedAtStart__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_SpellsThatMustBeStoppedAtStart)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 15, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate SpellsThatMustBeStoppedAtStart__Changed;
        public string SpellsThatMustBeStoppedAtStart__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(SpellsThatMustBeStoppedAtStart__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool SpellsThatMustBeStoppedAtStart__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(15);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Wizardry.SpellId, bool> _CanceledSpells;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Wizardry.SpellId, bool> CanceledSpells
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CanceledSpells, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CanceledSpells, value, SharedCode.EntitySystem.ReplicationLevel.Master, 16, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(16, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? CanceledSpells__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_CanceledSpells)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 16, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CanceledSpells__Changed;
        public string CanceledSpells__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CanceledSpells__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CanceledSpells__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) && IsDirty(16);
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
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Owner, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 17, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(17, AsReference = false, DynamicType = false, OverwriteList = false)]
        public SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> Owner__Serialized
        {
            get
            {
                return _Owner;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _Owner, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 17, false);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Owner__Changed;
        public string Owner__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Owner__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Owner__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(17);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Wizardry.SpellId, SharedCode.Wizardry.ISpell> _Spells;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Wizardry.SpellId, SharedCode.Wizardry.ISpell> Spells
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _Spells, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _Spells, value, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, 18, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(18, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? Spells__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_Spells)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 18, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate Spells__Changed;
        public string Spells__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(Spells__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Spells__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) && IsDirty(18);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Wizardry.CooldownGroupDef, long> _CooldownsUntil;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.EntitySystem.Delta.IDeltaDictionary<SharedCode.Wizardry.CooldownGroupDef, long> CooldownsUntil
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _CooldownsUntil, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _CooldownsUntil, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 19, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(19, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? CooldownsUntil__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_CooldownsUntil)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 19, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate CooldownsUntil__Changed;
        public string CooldownsUntil__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(CooldownsUntil__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool CooldownsUntil__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(19);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Entities.IPingDiagnostics _PingDiagnostics;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Entities.IPingDiagnostics PingDiagnostics
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _PingDiagnostics, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _PingDiagnostics, value, SharedCode.EntitySystem.ReplicationLevel.ClientFull, 20, false);
        }

        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoMember(20, AsReference = true, DynamicType = true, OverwriteList = false)]
        public ulong? PingDiagnostics__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_PingDiagnostics)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 20, value);
            }
        }

        event SharedCode.EntitySystem.PropertyChangedDelegate PingDiagnostics__Changed;
        public string PingDiagnostics__ChangedSubscribers => GeneratedCode.EntitySystem.EventHelper.SubscribersToString(PingDiagnostics__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool PingDiagnostics__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull) && IsDirty(20);
            set
            {
            }
        }

        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        private SharedCode.Wizardry.UnityEnvironmentMark _SlaveWizardMark;
        [ProtoBuf.ProtoIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public SharedCode.Wizardry.UnityEnvironmentMark SlaveWizardMark
        {
            get;
            set;
        }
    }
}