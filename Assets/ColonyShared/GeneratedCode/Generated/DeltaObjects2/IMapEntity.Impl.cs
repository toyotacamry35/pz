// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("MapEntity")]
    public partial class MapEntity : SharedCode.EntitySystem.BaseEntity, SharedCode.MapSystem.IMapEntity, IMapEntityImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public MapEntity()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                RealmId = default(System.Guid);
                RealmRules = default(SharedCode.Aspects.Sessions.RealmRulesDef);
                Map = default(GeneratedCode.Custom.Config.MapDef);
                State = default(SharedCode.MapSystem.MapEntityState);
                Dead = default(bool);
                WorldSpaces = new SharedCode.EntitySystem.Delta.DeltaList<SharedCode.MapSystem.IWorldSpaceDescription>();
                SavedScenes = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool>();
            }

            constructor();
        }

        public MapEntity(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                RealmId = default(System.Guid);
                RealmRules = default(SharedCode.Aspects.Sessions.RealmRulesDef);
                Map = default(GeneratedCode.Custom.Config.MapDef);
                State = default(SharedCode.MapSystem.MapEntityState);
                Dead = default(bool);
                WorldSpaces = new SharedCode.EntitySystem.Delta.DeltaList<SharedCode.MapSystem.IWorldSpaceDescription>();
                SavedScenes = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>, bool>();
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Server) == (long)SharedCode.EntitySystem.ReplicationLevel.Server)
                if (_WorldSpaces != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_WorldSpaces).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Server, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Always) == (long)SharedCode.EntitySystem.ReplicationLevel.Always)
                if (_SavedScenes != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_SavedScenes).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Always, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _WorldSpaces, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Server ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Server, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _SavedScenes, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Always ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Always, true, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _WorldSpaces, 15, false, SharedCode.EntitySystem.ReplicationLevel.Server);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _SavedScenes, 16, false, SharedCode.EntitySystem.ReplicationLevel.Always);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaces)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SavedScenes)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _WorldSpaces, SharedCode.EntitySystem.ReplicationLevel.Server, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _SavedScenes, SharedCode.EntitySystem.ReplicationLevel.Always, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaces)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SavedScenes)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "RealmId":
                    RealmId__Changed += callback;
                    break;
                case "RealmRules":
                    RealmRules__Changed += callback;
                    break;
                case "Map":
                    Map__Changed += callback;
                    break;
                case "State":
                    State__Changed += callback;
                    break;
                case "Dead":
                    Dead__Changed += callback;
                    break;
                case "WorldSpaces":
                    WorldSpaces__Changed += callback;
                    break;
                case "SavedScenes":
                    SavedScenes__Changed += callback;
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
                case "RealmId":
                    RealmId__Changed -= callback;
                    break;
                case "RealmRules":
                    RealmRules__Changed -= callback;
                    break;
                case "Map":
                    Map__Changed -= callback;
                    break;
                case "State":
                    State__Changed -= callback;
                    break;
                case "Dead":
                    Dead__Changed -= callback;
                    break;
                case "WorldSpaces":
                    WorldSpaces__Changed -= callback;
                    break;
                case "SavedScenes":
                    SavedScenes__Changed -= callback;
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
                case "RealmId":
                    RealmId__Changed = null;
                    break;
                case "RealmRules":
                    RealmRules__Changed = null;
                    break;
                case "Map":
                    Map__Changed = null;
                    break;
                case "State":
                    State__Changed = null;
                    break;
                case "Dead":
                    Dead__Changed = null;
                    break;
                case "WorldSpaces":
                    WorldSpaces__Changed = null;
                    break;
                case "SavedScenes":
                    SavedScenes__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            RealmId__Changed = null;
            RealmRules__Changed = null;
            Map__Changed = null;
            State__Changed = null;
            Dead__Changed = null;
            WorldSpaces__Changed = null;
            SavedScenes__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && RealmId__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_RealmId, nameof(RealmId), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, RealmId__Changed);
            }

            if (NeedFireEvent(11) && RealmRules__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_RealmRules, nameof(RealmRules), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, RealmRules__Changed);
            }

            if (NeedFireEvent(12) && Map__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Map, nameof(Map), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Map__Changed);
            }

            if (NeedFireEvent(13) && State__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_State, nameof(State), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, State__Changed);
            }

            if (NeedFireEvent(14) && Dead__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Dead, nameof(Dead), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Dead__Changed);
            }

            if (NeedFireEvent(15) && WorldSpaces__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 15;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_WorldSpaces, nameof(WorldSpaces), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, WorldSpaces__Changed);
            }

            if (NeedFireEvent(16) && SavedScenes__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 16;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_SavedScenes, nameof(SavedScenes), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, SavedScenes__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                State = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                Dead = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Server & mask) > 0)
                WorldSpaces = default;
            if (_WorldSpaces != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaces).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                SavedScenes = default;
            if (_SavedScenes != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SavedScenes).Downgrade(mask);
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("WorldSpaceDescription")]
    public partial class WorldSpaceDescription : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.MapSystem.IWorldSpaceDescription
    {
        public WorldSpaceDescription()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                ChunkDef = default(GeneratedCode.Custom.Config.MapDef);
                UnityRepositoryId = default(System.Guid);
                WorldSpaceRepositoryId = default(System.Guid);
                WorldSpaceGuid = default(System.Guid);
                State = default(SharedCode.MapSystem.MapChunkState);
            }

            constructor();
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "ChunkDef":
                    ChunkDef__Changed += callback;
                    break;
                case "UnityRepositoryId":
                    UnityRepositoryId__Changed += callback;
                    break;
                case "WorldSpaceRepositoryId":
                    WorldSpaceRepositoryId__Changed += callback;
                    break;
                case "WorldSpaceGuid":
                    WorldSpaceGuid__Changed += callback;
                    break;
                case "State":
                    State__Changed += callback;
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
                case "ChunkDef":
                    ChunkDef__Changed -= callback;
                    break;
                case "UnityRepositoryId":
                    UnityRepositoryId__Changed -= callback;
                    break;
                case "WorldSpaceRepositoryId":
                    WorldSpaceRepositoryId__Changed -= callback;
                    break;
                case "WorldSpaceGuid":
                    WorldSpaceGuid__Changed -= callback;
                    break;
                case "State":
                    State__Changed -= callback;
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
                case "ChunkDef":
                    ChunkDef__Changed = null;
                    break;
                case "UnityRepositoryId":
                    UnityRepositoryId__Changed = null;
                    break;
                case "WorldSpaceRepositoryId":
                    WorldSpaceRepositoryId__Changed = null;
                    break;
                case "WorldSpaceGuid":
                    WorldSpaceGuid__Changed = null;
                    break;
                case "State":
                    State__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            ChunkDef__Changed = null;
            UnityRepositoryId__Changed = null;
            WorldSpaceRepositoryId__Changed = null;
            WorldSpaceGuid__Changed = null;
            State__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && ChunkDef__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ChunkDef, nameof(ChunkDef), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ChunkDef__Changed);
            }

            if (NeedFireEvent(11) && UnityRepositoryId__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_UnityRepositoryId, nameof(UnityRepositoryId), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, UnityRepositoryId__Changed);
            }

            if (NeedFireEvent(12) && WorldSpaceRepositoryId__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_WorldSpaceRepositoryId, nameof(WorldSpaceRepositoryId), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, WorldSpaceRepositoryId__Changed);
            }

            if (NeedFireEvent(13) && WorldSpaceGuid__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_WorldSpaceGuid, nameof(WorldSpaceGuid), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, WorldSpaceGuid__Changed);
            }

            if (NeedFireEvent(14) && State__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_State, nameof(State), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, State__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                ChunkDef = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                UnityRepositoryId = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                WorldSpaceRepositoryId = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                WorldSpaceGuid = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                State = default;
        }
    }
}