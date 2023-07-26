// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("WizardEntity")]
    public partial class WizardEntity : SharedCode.EntitySystem.BaseEntity, SharedCode.Wizardry.IWizardEntity, IWizardEntityImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public WizardEntity()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                HostWizard = default(SharedCode.EntitySystem.OuterRef<SharedCode.Wizardry.IWizardEntity>);
                IsDead = default(bool);
                IsInterestingEnoughToLog = default(bool);
                Counter = default(SharedCode.Wizardry.SpellId);
                SlaveCounter = default(SharedCode.Wizardry.SpellId);
                SpellsThatMustBeStoppedAtStart = new SharedCode.EntitySystem.Delta.DeltaList<SharedCode.Wizardry.SpellThatMustBeStoppedAtStart>();
                CanceledSpells = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Wizardry.SpellId, bool>();
                Owner = default(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>);
                Spells = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Wizardry.SpellId, SharedCode.Wizardry.ISpell>();
                CooldownsUntil = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Wizardry.CooldownGroupDef, long>();
                PingDiagnostics = new GeneratedCode.DeltaObjects.PingDiagnostics();
            }

            constructor();
        }

        public WizardEntity(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                HostWizard = default(SharedCode.EntitySystem.OuterRef<SharedCode.Wizardry.IWizardEntity>);
                IsDead = default(bool);
                IsInterestingEnoughToLog = default(bool);
                Counter = default(SharedCode.Wizardry.SpellId);
                SlaveCounter = default(SharedCode.Wizardry.SpellId);
                SpellsThatMustBeStoppedAtStart = new SharedCode.EntitySystem.Delta.DeltaList<SharedCode.Wizardry.SpellThatMustBeStoppedAtStart>();
                CanceledSpells = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Wizardry.SpellId, bool>();
                Owner = default(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>);
                Spells = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Wizardry.SpellId, SharedCode.Wizardry.ISpell>();
                CooldownsUntil = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Wizardry.CooldownGroupDef, long>();
                PingDiagnostics = new GeneratedCode.DeltaObjects.PingDiagnostics();
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_SpellsThatMustBeStoppedAtStart != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_SpellsThatMustBeStoppedAtStart).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_CanceledSpells != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_CanceledSpells).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast)
                if (_Spells != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_Spells).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_CooldownsUntil != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_CooldownsUntil).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_PingDiagnostics != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_PingDiagnostics).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _SpellsThatMustBeStoppedAtStart, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _CanceledSpells, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _Spells, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _CooldownsUntil, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _PingDiagnostics, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, true, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _SpellsThatMustBeStoppedAtStart, 15, false, SharedCode.EntitySystem.ReplicationLevel.Master);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _CanceledSpells, 16, false, SharedCode.EntitySystem.ReplicationLevel.Master);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _Spells, 18, false, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _CooldownsUntil, 19, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _PingDiagnostics, 20, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SpellsThatMustBeStoppedAtStart)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CanceledSpells)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Spells)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CooldownsUntil)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_PingDiagnostics)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _SpellsThatMustBeStoppedAtStart, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _CanceledSpells, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _Spells, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _CooldownsUntil, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _PingDiagnostics, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SpellsThatMustBeStoppedAtStart)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CanceledSpells)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Spells)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CooldownsUntil)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_PingDiagnostics)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "HostWizard":
                    HostWizard__Changed += callback;
                    break;
                case "IsDead":
                    IsDead__Changed += callback;
                    break;
                case "IsInterestingEnoughToLog":
                    IsInterestingEnoughToLog__Changed += callback;
                    break;
                case "Counter":
                    Counter__Changed += callback;
                    break;
                case "SlaveCounter":
                    SlaveCounter__Changed += callback;
                    break;
                case "SpellsThatMustBeStoppedAtStart":
                    SpellsThatMustBeStoppedAtStart__Changed += callback;
                    break;
                case "CanceledSpells":
                    CanceledSpells__Changed += callback;
                    break;
                case "Owner":
                    Owner__Changed += callback;
                    break;
                case "Spells":
                    Spells__Changed += callback;
                    break;
                case "CooldownsUntil":
                    CooldownsUntil__Changed += callback;
                    break;
                case "PingDiagnostics":
                    PingDiagnostics__Changed += callback;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Unsubscribe(propertyName, callback);
            switch (propertyName)
            {
                case "HostWizard":
                    HostWizard__Changed -= callback;
                    break;
                case "IsDead":
                    IsDead__Changed -= callback;
                    break;
                case "IsInterestingEnoughToLog":
                    IsInterestingEnoughToLog__Changed -= callback;
                    break;
                case "Counter":
                    Counter__Changed -= callback;
                    break;
                case "SlaveCounter":
                    SlaveCounter__Changed -= callback;
                    break;
                case "SpellsThatMustBeStoppedAtStart":
                    SpellsThatMustBeStoppedAtStart__Changed -= callback;
                    break;
                case "CanceledSpells":
                    CanceledSpells__Changed -= callback;
                    break;
                case "Owner":
                    Owner__Changed -= callback;
                    break;
                case "Spells":
                    Spells__Changed -= callback;
                    break;
                case "CooldownsUntil":
                    CooldownsUntil__Changed -= callback;
                    break;
                case "PingDiagnostics":
                    PingDiagnostics__Changed -= callback;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe(string propertyName)
        {
            base.Unsubscribe(propertyName);
            switch (propertyName)
            {
                case "HostWizard":
                    HostWizard__Changed = null;
                    break;
                case "IsDead":
                    IsDead__Changed = null;
                    break;
                case "IsInterestingEnoughToLog":
                    IsInterestingEnoughToLog__Changed = null;
                    break;
                case "Counter":
                    Counter__Changed = null;
                    break;
                case "SlaveCounter":
                    SlaveCounter__Changed = null;
                    break;
                case "SpellsThatMustBeStoppedAtStart":
                    SpellsThatMustBeStoppedAtStart__Changed = null;
                    break;
                case "CanceledSpells":
                    CanceledSpells__Changed = null;
                    break;
                case "Owner":
                    Owner__Changed = null;
                    break;
                case "Spells":
                    Spells__Changed = null;
                    break;
                case "CooldownsUntil":
                    CooldownsUntil__Changed = null;
                    break;
                case "PingDiagnostics":
                    PingDiagnostics__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            HostWizard__Changed = null;
            IsDead__Changed = null;
            IsInterestingEnoughToLog__Changed = null;
            Counter__Changed = null;
            SlaveCounter__Changed = null;
            SpellsThatMustBeStoppedAtStart__Changed = null;
            CanceledSpells__Changed = null;
            Owner__Changed = null;
            Spells__Changed = null;
            CooldownsUntil__Changed = null;
            PingDiagnostics__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && HostWizard__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_HostWizard, nameof(HostWizard), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, HostWizard__Changed);
            }

            if (NeedFireEvent(11) && IsDead__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_IsDead, nameof(IsDead), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, IsDead__Changed);
            }

            if (NeedFireEvent(12) && IsInterestingEnoughToLog__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_IsInterestingEnoughToLog, nameof(IsInterestingEnoughToLog), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, IsInterestingEnoughToLog__Changed);
            }

            if (NeedFireEvent(13) && Counter__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Counter, nameof(Counter), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Counter__Changed);
            }

            if (NeedFireEvent(14) && SlaveCounter__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_SlaveCounter, nameof(SlaveCounter), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, SlaveCounter__Changed);
            }

            if (NeedFireEvent(15) && SpellsThatMustBeStoppedAtStart__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 15;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_SpellsThatMustBeStoppedAtStart, nameof(SpellsThatMustBeStoppedAtStart), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, SpellsThatMustBeStoppedAtStart__Changed);
            }

            if (NeedFireEvent(16) && CanceledSpells__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 16;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CanceledSpells, nameof(CanceledSpells), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CanceledSpells__Changed);
            }

            if (NeedFireEvent(17) && Owner__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 17;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Owner, nameof(Owner), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Owner__Changed);
            }

            if (NeedFireEvent(18) && Spells__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 18;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Spells, nameof(Spells), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Spells__Changed);
            }

            if (NeedFireEvent(19) && CooldownsUntil__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 19;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CooldownsUntil, nameof(CooldownsUntil), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CooldownsUntil__Changed);
            }

            if (NeedFireEvent(20) && PingDiagnostics__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 20;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_PingDiagnostics, nameof(PingDiagnostics), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, PingDiagnostics__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                HostWizard = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                IsDead = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                IsInterestingEnoughToLog = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                Counter = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                SlaveCounter = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                SpellsThatMustBeStoppedAtStart = default;
            if (_SpellsThatMustBeStoppedAtStart != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SpellsThatMustBeStoppedAtStart).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                CanceledSpells = default;
            if (_CanceledSpells != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CanceledSpells).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                Owner = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                Spells = default;
            if (_Spells != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Spells).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                CooldownsUntil = default;
            if (_CooldownsUntil != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CooldownsUntil).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                PingDiagnostics = default;
            if (_PingDiagnostics != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_PingDiagnostics).Downgrade(mask);
        }
    }
}