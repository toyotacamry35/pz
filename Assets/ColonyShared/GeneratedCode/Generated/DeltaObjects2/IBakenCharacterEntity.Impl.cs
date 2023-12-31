// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("BakenCharacterEntity")]
    public partial class BakenCharacterEntity : SharedCode.EntitySystem.BaseEntity, SharedCode.Entities.IBakenCharacterEntity, IBakenCharacterEntityImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public BakenCharacterEntity()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                CharacterId = default(System.Guid);
                CharacterLoaded = default(bool);
                Logined = default(bool);
                RegisteredBakens = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool>();
                ActiveBaken = default(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>);
            }

            constructor();
        }

        public BakenCharacterEntity(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                CharacterId = default(System.Guid);
                CharacterLoaded = default(bool);
                Logined = default(bool);
                RegisteredBakens = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool>();
                ActiveBaken = default(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>);
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_RegisteredBakens != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_RegisteredBakens).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _RegisteredBakens, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, true, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _RegisteredBakens, 13, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_RegisteredBakens)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _RegisteredBakens, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_RegisteredBakens)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "CharacterId":
                    CharacterId__Changed += callback;
                    break;
                case "CharacterLoaded":
                    CharacterLoaded__Changed += callback;
                    break;
                case "Logined":
                    Logined__Changed += callback;
                    break;
                case "RegisteredBakens":
                    RegisteredBakens__Changed += callback;
                    break;
                case "ActiveBaken":
                    ActiveBaken__Changed += callback;
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
                case "CharacterId":
                    CharacterId__Changed -= callback;
                    break;
                case "CharacterLoaded":
                    CharacterLoaded__Changed -= callback;
                    break;
                case "Logined":
                    Logined__Changed -= callback;
                    break;
                case "RegisteredBakens":
                    RegisteredBakens__Changed -= callback;
                    break;
                case "ActiveBaken":
                    ActiveBaken__Changed -= callback;
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
                case "CharacterId":
                    CharacterId__Changed = null;
                    break;
                case "CharacterLoaded":
                    CharacterLoaded__Changed = null;
                    break;
                case "Logined":
                    Logined__Changed = null;
                    break;
                case "RegisteredBakens":
                    RegisteredBakens__Changed = null;
                    break;
                case "ActiveBaken":
                    ActiveBaken__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            CharacterId__Changed = null;
            CharacterLoaded__Changed = null;
            Logined__Changed = null;
            RegisteredBakens__Changed = null;
            ActiveBaken__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && CharacterId__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CharacterId, nameof(CharacterId), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CharacterId__Changed);
            }

            if (NeedFireEvent(11) && CharacterLoaded__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CharacterLoaded, nameof(CharacterLoaded), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CharacterLoaded__Changed);
            }

            if (NeedFireEvent(12) && Logined__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Logined, nameof(Logined), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Logined__Changed);
            }

            if (NeedFireEvent(13) && RegisteredBakens__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_RegisteredBakens, nameof(RegisteredBakens), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, RegisteredBakens__Changed);
            }

            if (NeedFireEvent(14) && ActiveBaken__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ActiveBaken, nameof(ActiveBaken), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ActiveBaken__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Server & mask) > 0)
                CharacterId = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Server & mask) > 0)
                CharacterLoaded = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                Logined = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                RegisteredBakens = default;
            if (_RegisteredBakens != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_RegisteredBakens).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                ActiveBaken = default;
        }
    }
}