// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("WorldObjectsInformationDataSetEntity")]
    public partial class WorldObjectsInformationDataSetEntity : SharedCode.EntitySystem.BaseEntity, SharedCode.Entities.IWorldObjectsInformationDataSetEntity, IWorldObjectsInformationDataSetEntityImplementRemoteMethods
    {
        public override string CodeVersion => ThisAssembly.AssemblyInformationalVersion;
        public WorldObjectsInformationDataSetEntity()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Positions = new SharedCode.EntitySystem.Delta.DeltaDictionary<ResourceSystem.Utils.OuterRef, SharedCode.Entities.IWorldObjectPositionInformation>();
                WorldObjectsInformationDataSetEngine = new GeneratedCode.DeltaObjects.WorldObjectsInformationDataSetEngine();
            }

            constructor();
        }

        public WorldObjectsInformationDataSetEntity(System.Guid id): base(id)
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Positions = new SharedCode.EntitySystem.Delta.DeltaDictionary<ResourceSystem.Utils.OuterRef, SharedCode.Entities.IWorldObjectPositionInformation>();
                WorldObjectsInformationDataSetEngine = new GeneratedCode.DeltaObjects.WorldObjectsInformationDataSetEngine();
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast)
                if (_Positions != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_Positions).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_WorldObjectsInformationDataSetEngine != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_WorldObjectsInformationDataSetEngine).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _Positions, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _WorldObjectsInformationDataSetEngine, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, false, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _Positions, 10, false, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _WorldObjectsInformationDataSetEngine, 11, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Positions)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldObjectsInformationDataSetEngine)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _Positions, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _WorldObjectsInformationDataSetEngine, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Positions)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldObjectsInformationDataSetEngine)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "Positions":
                    Positions__Changed += callback;
                    break;
                case "WorldObjectsInformationDataSetEngine":
                    WorldObjectsInformationDataSetEngine__Changed += callback;
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
                case "Positions":
                    Positions__Changed -= callback;
                    break;
                case "WorldObjectsInformationDataSetEngine":
                    WorldObjectsInformationDataSetEngine__Changed -= callback;
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
                case "Positions":
                    Positions__Changed = null;
                    break;
                case "WorldObjectsInformationDataSetEngine":
                    WorldObjectsInformationDataSetEngine__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Positions__Changed = null;
            WorldObjectsInformationDataSetEngine__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && Positions__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Positions, nameof(Positions), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Positions__Changed);
            }

            if (NeedFireEvent(11) && WorldObjectsInformationDataSetEngine__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_WorldObjectsInformationDataSetEngine, nameof(WorldObjectsInformationDataSetEngine), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, WorldObjectsInformationDataSetEngine__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                Positions = default;
            if (_Positions != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Positions).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                WorldObjectsInformationDataSetEngine = default;
            if (_WorldObjectsInformationDataSetEngine != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_WorldObjectsInformationDataSetEngine).Downgrade(mask);
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("WorldObjectPositionInformation")]
    public partial class WorldObjectPositionInformation : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Entities.IWorldObjectPositionInformation, IWorldObjectPositionInformationImplementRemoteMethods
    {
        public WorldObjectPositionInformation()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Position = default(SharedCode.Utils.Vector3);
            }

            constructor();
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "Position":
                    Position__Changed += callback;
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
                case "Position":
                    Position__Changed -= callback;
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
                case "Position":
                    Position__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Position__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && Position__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Position, nameof(Position), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Position__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                Position = default;
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("CharacterPositionInformation")]
    public partial class CharacterPositionInformation : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Entities.ICharacterPositionInformation, ICharacterPositionInformationImplementRemoteMethods
    {
        public CharacterPositionInformation()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Mutation = default(Assets.Src.Aspects.Impl.Factions.Template.MutationStageDef);
                Position = default(SharedCode.Utils.Vector3);
            }

            constructor();
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "Mutation":
                    Mutation__Changed += callback;
                    break;
                case "Position":
                    Position__Changed += callback;
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
                case "Mutation":
                    Mutation__Changed -= callback;
                    break;
                case "Position":
                    Position__Changed -= callback;
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
                case "Mutation":
                    Mutation__Changed = null;
                    break;
                case "Position":
                    Position__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Mutation__Changed = null;
            Position__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && Mutation__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Mutation, nameof(Mutation), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Mutation__Changed);
            }

            if (NeedFireEvent(11) && Position__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Position, nameof(Position), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Position__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                Mutation = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                Position = default;
        }
    }
}