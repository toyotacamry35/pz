// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("EntityMethodsCallsChain")]
    public partial class EntityMethodsCallsChain : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Entities.Core.IEntityMethodsCallsChain, IEntityMethodsCallsChainImplementRemoteMethods
    {
        public EntityMethodsCallsChain()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                ChainContext = new GeneratedCode.DeltaObjects.ChainContext();
                Chain = new SharedCode.EntitySystem.Delta.DeltaList<SharedCode.EntitySystem.ChainCalls.ChainBlockBase>();
                CurrentChainIndex = default(int);
                NextTimeToCall = default(long);
                ForksIds = new SharedCode.EntitySystem.Delta.DeltaList<System.Guid>();
                ForkCreatorId = default(System.Guid);
                Id = default(System.Guid);
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_ChainContext != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_ChainContext).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_Chain != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_Chain).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.Master) == (long)SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_ForksIds != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_ForksIds).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _ChainContext, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _Chain, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _ForksIds, currentLevel > SharedCode.EntitySystem.ReplicationLevel.Master ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.Master, true, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ChainContext, 10, false, SharedCode.EntitySystem.ReplicationLevel.Master);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _Chain, 11, false, SharedCode.EntitySystem.ReplicationLevel.Master);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ForksIds, 14, false, SharedCode.EntitySystem.ReplicationLevel.Master);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ChainContext)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Chain)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ForksIds)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _ChainContext, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _Chain, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _ForksIds, SharedCode.EntitySystem.ReplicationLevel.Master, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ChainContext)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Chain)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ForksIds)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "ChainContext":
                    ChainContext__Changed += callback;
                    break;
                case "Chain":
                    Chain__Changed += callback;
                    break;
                case "CurrentChainIndex":
                    CurrentChainIndex__Changed += callback;
                    break;
                case "NextTimeToCall":
                    NextTimeToCall__Changed += callback;
                    break;
                case "ForksIds":
                    ForksIds__Changed += callback;
                    break;
                case "ForkCreatorId":
                    ForkCreatorId__Changed += callback;
                    break;
                case "Id":
                    Id__Changed += callback;
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
                case "ChainContext":
                    ChainContext__Changed -= callback;
                    break;
                case "Chain":
                    Chain__Changed -= callback;
                    break;
                case "CurrentChainIndex":
                    CurrentChainIndex__Changed -= callback;
                    break;
                case "NextTimeToCall":
                    NextTimeToCall__Changed -= callback;
                    break;
                case "ForksIds":
                    ForksIds__Changed -= callback;
                    break;
                case "ForkCreatorId":
                    ForkCreatorId__Changed -= callback;
                    break;
                case "Id":
                    Id__Changed -= callback;
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
                case "ChainContext":
                    ChainContext__Changed = null;
                    break;
                case "Chain":
                    Chain__Changed = null;
                    break;
                case "CurrentChainIndex":
                    CurrentChainIndex__Changed = null;
                    break;
                case "NextTimeToCall":
                    NextTimeToCall__Changed = null;
                    break;
                case "ForksIds":
                    ForksIds__Changed = null;
                    break;
                case "ForkCreatorId":
                    ForkCreatorId__Changed = null;
                    break;
                case "Id":
                    Id__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            ChainContext__Changed = null;
            Chain__Changed = null;
            CurrentChainIndex__Changed = null;
            NextTimeToCall__Changed = null;
            ForksIds__Changed = null;
            ForkCreatorId__Changed = null;
            Id__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && ChainContext__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ChainContext, nameof(ChainContext), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ChainContext__Changed);
            }

            if (NeedFireEvent(11) && Chain__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Chain, nameof(Chain), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Chain__Changed);
            }

            if (NeedFireEvent(12) && CurrentChainIndex__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CurrentChainIndex, nameof(CurrentChainIndex), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CurrentChainIndex__Changed);
            }

            if (NeedFireEvent(13) && NextTimeToCall__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_NextTimeToCall, nameof(NextTimeToCall), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, NextTimeToCall__Changed);
            }

            if (NeedFireEvent(14) && ForksIds__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ForksIds, nameof(ForksIds), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ForksIds__Changed);
            }

            if (NeedFireEvent(15) && ForkCreatorId__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 15;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ForkCreatorId, nameof(ForkCreatorId), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ForkCreatorId__Changed);
            }

            if (NeedFireEvent(16) && Id__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 16;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Id, nameof(Id), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Id__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                ChainContext = default;
            if (_ChainContext != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ChainContext).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                Chain = default;
            if (_Chain != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_Chain).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                CurrentChainIndex = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                NextTimeToCall = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                ForksIds = default;
            if (_ForksIds != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ForksIds).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
                ForkCreatorId = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Always & mask) > 0)
                Id = default;
        }
    }
}