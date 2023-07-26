// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("SpawnDaemon")]
    public partial class SpawnDaemon : SharedCode.EntitySystem.BaseEntity, SharedCode.Entities.GameObjectEntities.ISpawnDaemon, ISpawnDaemonImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public SpawnDaemon()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Name = default(string);
                SpawnedObjectsAmounts = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Entities.GameObjectEntities.IEntityObjectDef, int>();
                Maps = new SharedCode.EntitySystem.Delta.DeltaList<SharedCode.Entities.GameObjectEntities.SpawnTemplatesMapDef>();
                SceneDef = default(SharedCode.Entities.GameObjectEntities.SpawnDaemonSceneDef);
                Def = default(SharedCode.Entities.GameObjectEntities.IEntityObjectDef);
                MapOwner = default(GeneratedCode.MapSystem.MapOwner);
                StaticIdFromExport = default(System.Guid);
                WorldSpaced = new GeneratedCode.DeltaObjects.WorldSpaced();
                LinksEngine = new GeneratedCode.DeltaObjects.LinksEngine();
                MovementSync = new GeneratedCode.DeltaObjects.SimpleMovementSync();
            }

            constructor();
        }

        public SpawnDaemon(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Name = default(string);
                SpawnedObjectsAmounts = new SharedCode.EntitySystem.Delta.DeltaDictionary<SharedCode.Entities.GameObjectEntities.IEntityObjectDef, int>();
                Maps = new SharedCode.EntitySystem.Delta.DeltaList<SharedCode.Entities.GameObjectEntities.SpawnTemplatesMapDef>();
                SceneDef = default(SharedCode.Entities.GameObjectEntities.SpawnDaemonSceneDef);
                Def = default(SharedCode.Entities.GameObjectEntities.IEntityObjectDef);
                MapOwner = default(GeneratedCode.MapSystem.MapOwner);
                StaticIdFromExport = default(System.Guid);
                WorldSpaced = new GeneratedCode.DeltaObjects.WorldSpaced();
                LinksEngine = new GeneratedCode.DeltaObjects.LinksEngine();
                MovementSync = new GeneratedCode.DeltaObjects.SimpleMovementSync();
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_SpawnedObjectsAmounts != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_SpawnedObjectsAmounts).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_Maps != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_Maps).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Always) == (long)SharedCode.EntitySystem.ReplicationLevel.Always)
                if (_WorldSpaced != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_WorldSpaced).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Always, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast)
                if (_LinksEngine != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_LinksEngine).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Always) == (long)SharedCode.EntitySystem.ReplicationLevel.Always)
                if (_MovementSync != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_MovementSync).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Always, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _SpawnedObjectsAmounts, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _Maps, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _WorldSpaced, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Always ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Always, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _LinksEngine, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _MovementSync, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Always ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Always, true, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _SpawnedObjectsAmounts, 11, false, SharedCode.EntitySystem.ReplicationLevel.Master);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _Maps, 12, false, SharedCode.EntitySystem.ReplicationLevel.Master);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _WorldSpaced, 17, false, SharedCode.EntitySystem.ReplicationLevel.Always);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _LinksEngine, 18, false, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _MovementSync, 19, false, SharedCode.EntitySystem.ReplicationLevel.Always);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SpawnedObjectsAmounts)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Maps)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaced)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_LinksEngine)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_MovementSync)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _SpawnedObjectsAmounts, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _Maps, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _WorldSpaced, SharedCode.EntitySystem.ReplicationLevel.Always, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _LinksEngine, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _MovementSync, SharedCode.EntitySystem.ReplicationLevel.Always, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SpawnedObjectsAmounts)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Maps)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaced)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_LinksEngine)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_MovementSync)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "Name":
                    Name__Changed += callback;
                    break;
                case "SpawnedObjectsAmounts":
                    SpawnedObjectsAmounts__Changed += callback;
                    break;
                case "Maps":
                    Maps__Changed += callback;
                    break;
                case "SceneDef":
                    SceneDef__Changed += callback;
                    break;
                case "Def":
                    Def__Changed += callback;
                    break;
                case "MapOwner":
                    MapOwner__Changed += callback;
                    break;
                case "StaticIdFromExport":
                    StaticIdFromExport__Changed += callback;
                    break;
                case "WorldSpaced":
                    WorldSpaced__Changed += callback;
                    break;
                case "LinksEngine":
                    LinksEngine__Changed += callback;
                    break;
                case "MovementSync":
                    MovementSync__Changed += callback;
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
                case "Name":
                    Name__Changed -= callback;
                    break;
                case "SpawnedObjectsAmounts":
                    SpawnedObjectsAmounts__Changed -= callback;
                    break;
                case "Maps":
                    Maps__Changed -= callback;
                    break;
                case "SceneDef":
                    SceneDef__Changed -= callback;
                    break;
                case "Def":
                    Def__Changed -= callback;
                    break;
                case "MapOwner":
                    MapOwner__Changed -= callback;
                    break;
                case "StaticIdFromExport":
                    StaticIdFromExport__Changed -= callback;
                    break;
                case "WorldSpaced":
                    WorldSpaced__Changed -= callback;
                    break;
                case "LinksEngine":
                    LinksEngine__Changed -= callback;
                    break;
                case "MovementSync":
                    MovementSync__Changed -= callback;
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
                case "Name":
                    Name__Changed = null;
                    break;
                case "SpawnedObjectsAmounts":
                    SpawnedObjectsAmounts__Changed = null;
                    break;
                case "Maps":
                    Maps__Changed = null;
                    break;
                case "SceneDef":
                    SceneDef__Changed = null;
                    break;
                case "Def":
                    Def__Changed = null;
                    break;
                case "MapOwner":
                    MapOwner__Changed = null;
                    break;
                case "StaticIdFromExport":
                    StaticIdFromExport__Changed = null;
                    break;
                case "WorldSpaced":
                    WorldSpaced__Changed = null;
                    break;
                case "LinksEngine":
                    LinksEngine__Changed = null;
                    break;
                case "MovementSync":
                    MovementSync__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Name__Changed = null;
            SpawnedObjectsAmounts__Changed = null;
            Maps__Changed = null;
            SceneDef__Changed = null;
            Def__Changed = null;
            MapOwner__Changed = null;
            StaticIdFromExport__Changed = null;
            WorldSpaced__Changed = null;
            LinksEngine__Changed = null;
            MovementSync__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && Name__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Name, nameof(Name), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Name__Changed);
            }

            if (NeedFireEvent(11) && SpawnedObjectsAmounts__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_SpawnedObjectsAmounts, nameof(SpawnedObjectsAmounts), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, SpawnedObjectsAmounts__Changed);
            }

            if (NeedFireEvent(12) && Maps__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Maps, nameof(Maps), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Maps__Changed);
            }

            if (NeedFireEvent(13) && SceneDef__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_SceneDef, nameof(SceneDef), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, SceneDef__Changed);
            }

            if (NeedFireEvent(14) && Def__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Def, nameof(Def), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Def__Changed);
            }

            if (NeedFireEvent(15) && MapOwner__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 15;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_MapOwner, nameof(MapOwner), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, MapOwner__Changed);
            }

            if (NeedFireEvent(16) && StaticIdFromExport__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 16;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_StaticIdFromExport, nameof(StaticIdFromExport), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, StaticIdFromExport__Changed);
            }

            if (NeedFireEvent(17) && WorldSpaced__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 17;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_WorldSpaced, nameof(WorldSpaced), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, WorldSpaced__Changed);
            }

            if (NeedFireEvent(18) && LinksEngine__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 18;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_LinksEngine, nameof(LinksEngine), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, LinksEngine__Changed);
            }

            if (NeedFireEvent(19) && MovementSync__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 19;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_MovementSync, nameof(MovementSync), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, MovementSync__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                Name = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                SpawnedObjectsAmounts = default;
            if (_SpawnedObjectsAmounts != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_SpawnedObjectsAmounts).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                Maps = default;
            if (_Maps != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Maps).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                SceneDef = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                MapOwner = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                WorldSpaced = default;
            if (_WorldSpaced != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldSpaced).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                LinksEngine = default;
            if (_LinksEngine != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_LinksEngine).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                MovementSync = default;
            if (_MovementSync != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_MovementSync).Downgrade(mask);
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("SpawnedObject")]
    public partial class SpawnedObject : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Entities.GameObjectEntities.ISpawnedObject
    {
        public SpawnedObject()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Spawner = default(SharedCode.EntitySystem.OuterRef<SharedCode.Entities.GameObjectEntities.ISpawnDaemon>);
                PointType = default(SharedCode.Entities.GameObjectEntities.SpawnPointTypeDef);
            }

            constructor();
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "Spawner":
                    Spawner__Changed += callback;
                    break;
                case "PointType":
                    PointType__Changed += callback;
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
                case "Spawner":
                    Spawner__Changed -= callback;
                    break;
                case "PointType":
                    PointType__Changed -= callback;
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
                case "Spawner":
                    Spawner__Changed = null;
                    break;
                case "PointType":
                    PointType__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Spawner__Changed = null;
            PointType__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && Spawner__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Spawner, nameof(Spawner), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Spawner__Changed);
            }

            if (NeedFireEvent(11) && PointType__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_PointType, nameof(PointType), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, PointType__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Server & mask) > 0)
                PointType = default;
        }
    }
}