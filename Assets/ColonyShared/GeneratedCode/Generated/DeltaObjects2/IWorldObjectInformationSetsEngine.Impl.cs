// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("WorldObjectInformationSetsEngine")]
    public partial class WorldObjectInformationSetsEngine : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Entities.IWorldObjectInformationSetsEngine, IWorldObjectInformationSetsEngineImplementRemoteMethods
    {
        public WorldObjectInformationSetsEngine()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                WorldObjectInformationRefsCounter = new SharedCode.EntitySystem.Delta.DeltaDictionary<Entities.GameMapData.WorldObjectInformationClientSubSetDef, int>();
                CurrentWorldObjectInformationRefs = new SharedCode.EntitySystem.Delta.DeltaDictionary<Entities.GameMapData.WorldObjectInformationClientSubSetDef, ResourceSystem.Utils.OuterRef>();
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_WorldObjectInformationRefsCounter != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_WorldObjectInformationRefsCounter).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_CurrentWorldObjectInformationRefs != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_CurrentWorldObjectInformationRefs).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _WorldObjectInformationRefsCounter, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _CurrentWorldObjectInformationRefs, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, false, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _WorldObjectInformationRefsCounter, 10, false, SharedCode.EntitySystem.ReplicationLevel.Master);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _CurrentWorldObjectInformationRefs, 11, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldObjectInformationRefsCounter)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CurrentWorldObjectInformationRefs)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _WorldObjectInformationRefsCounter, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _CurrentWorldObjectInformationRefs, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldObjectInformationRefsCounter)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CurrentWorldObjectInformationRefs)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "WorldObjectInformationRefsCounter":
                    WorldObjectInformationRefsCounter__Changed += callback;
                    break;
                case "CurrentWorldObjectInformationRefs":
                    CurrentWorldObjectInformationRefs__Changed += callback;
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
                case "WorldObjectInformationRefsCounter":
                    WorldObjectInformationRefsCounter__Changed -= callback;
                    break;
                case "CurrentWorldObjectInformationRefs":
                    CurrentWorldObjectInformationRefs__Changed -= callback;
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
                case "WorldObjectInformationRefsCounter":
                    WorldObjectInformationRefsCounter__Changed = null;
                    break;
                case "CurrentWorldObjectInformationRefs":
                    CurrentWorldObjectInformationRefs__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            WorldObjectInformationRefsCounter__Changed = null;
            CurrentWorldObjectInformationRefs__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && WorldObjectInformationRefsCounter__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_WorldObjectInformationRefsCounter, nameof(WorldObjectInformationRefsCounter), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, WorldObjectInformationRefsCounter__Changed);
            }

            if (NeedFireEvent(11) && CurrentWorldObjectInformationRefs__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CurrentWorldObjectInformationRefs, nameof(CurrentWorldObjectInformationRefs), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CurrentWorldObjectInformationRefs__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                WorldObjectInformationRefsCounter = default;
            if (_WorldObjectInformationRefsCounter != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldObjectInformationRefsCounter).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                CurrentWorldObjectInformationRefs = default;
            if (_CurrentWorldObjectInformationRefs != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_CurrentWorldObjectInformationRefs).Downgrade(mask);
        }
    }
}