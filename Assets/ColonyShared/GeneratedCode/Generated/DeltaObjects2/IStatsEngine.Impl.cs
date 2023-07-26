// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("StatsEngine")]
    public partial class StatsEngine : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Entities.Engine.IStatsEngine, IStatsEngineImplementRemoteMethods
    {
        public StatsEngine()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                StatsDef = default(Assets.Src.GameObjectAssembler.Res.StatsDef);
                TimeStats = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.ITimeStat>();
                TimeStatsBroadcast = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.ITimeStat>();
                ValueStats = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.IValueStat>();
                ValueStatsBroadcast = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.IValueStat>();
                ProxyStats = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.IProxyStat>();
                ProceduralStats = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.IProceduralStat>();
                AccumulatedStats = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.IAccumulatedStat>();
                ProceduralStatsBroadcast = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.IProceduralStat>();
                AccumulatedStatsBroadcast = new SharedCode.EntitySystem.Delta.DeltaDictionary<Assets.Src.Aspects.Impl.Stats.StatResource, Src.Aspects.Impl.Stats.IAccumulatedStat>();
                TimeWhenIdleStarted = default(long);
            }

            constructor();
        }

        public override void GetAllLinkedEntities(long replicationMask, System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_TimeStats != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_TimeStats).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast)
                if (_TimeStatsBroadcast != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_TimeStatsBroadcast).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_ValueStats != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_ValueStats).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast)
                if (_ValueStatsBroadcast != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_ValueStatsBroadcast).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_ProxyStats != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_ProxyStats).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_ProceduralStats != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_ProceduralStats).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull)
                if (_AccumulatedStats != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_AccumulatedStats).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientFull, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast)
                if (_ProceduralStatsBroadcast != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_ProceduralStatsBroadcast).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, onlyDbEntities);
            if ((replicationMask & (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast) == (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast)
                if (_AccumulatedStatsBroadcast != null)
                    ((SharedCode.EntitySystem.IDeltaObject)_AccumulatedStatsBroadcast).GetAllLinkedEntities(replicationMask, entities, currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, onlyDbEntities);
            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }

        public override void FillReplicationSetRecursive(System.Collections.Generic.Dictionary<SharedCode.EntitySystem.ReplicationLevel, System.Collections.Generic.Dictionary<SharedCode.EntitySystem.IDeltaObject, SharedCode.EntitySystem.DeltaObjectReplicationInfo>> replicationSets, System.Collections.Generic.HashSet<SharedCode.EntitySystem.ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _TimeStats, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _TimeStatsBroadcast, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _ValueStats, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _ValueStatsBroadcast, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, true, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _ProxyStats, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _ProceduralStats, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _AccumulatedStats, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientFull ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientFull, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _ProceduralStatsBroadcast, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, false, withBsonIgnore);
            GeneratedCode.EntitySystem.DeltaObjectHelper.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, _AccumulatedStatsBroadcast, currentLevel > SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast ? currentLevel : SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, false, withBsonIgnore);
        }

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _TimeStats, 11, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _TimeStatsBroadcast, 12, false, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ValueStats, 13, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ValueStatsBroadcast, 14, false, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ProxyStats, 15, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ProceduralStats, 16, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _AccumulatedStats, 17, false, SharedCode.EntitySystem.ReplicationLevel.ClientFull);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ProceduralStatsBroadcast, 18, false, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _AccumulatedStatsBroadcast, 19, false, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_TimeStats)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_TimeStatsBroadcast)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ValueStats)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ValueStatsBroadcast)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProxyStats)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProceduralStats)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_AccumulatedStats)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProceduralStatsBroadcast)?.IncrementParentRefs(parentEntity, trackChanged);
                ((SharedCode.EntitySystem.IDeltaObjectExt)_AccumulatedStatsBroadcast)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void ReplicationLevelActualize(SharedCode.EntitySystem.ReplicationLevel? actualParentLevel, SharedCode.EntitySystem.ReplicationLevel? oldParentLevel)
        {
            base.ReplicationLevelActualize(actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _TimeStats, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _TimeStatsBroadcast, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _ValueStats, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _ValueStatsBroadcast, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _ProxyStats, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _ProceduralStats, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _AccumulatedStats, SharedCode.EntitySystem.ReplicationLevel.ClientFull, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _ProceduralStatsBroadcast, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, actualParentLevel, oldParentLevel);
            GeneratedCode.EntitySystem.DeltaObjectHelper.ReplicationLevelActualize(parentEntity, _AccumulatedStatsBroadcast, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, actualParentLevel, oldParentLevel);
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_TimeStats)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_TimeStatsBroadcast)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ValueStats)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ValueStatsBroadcast)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProxyStats)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProceduralStats)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_AccumulatedStats)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProceduralStatsBroadcast)?.DecrementParentRefs();
                ((SharedCode.EntitySystem.IDeltaObjectExt)_AccumulatedStatsBroadcast)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "StatsDef":
                    StatsDef__Changed += callback;
                    break;
                case "TimeStats":
                    TimeStats__Changed += callback;
                    break;
                case "TimeStatsBroadcast":
                    TimeStatsBroadcast__Changed += callback;
                    break;
                case "ValueStats":
                    ValueStats__Changed += callback;
                    break;
                case "ValueStatsBroadcast":
                    ValueStatsBroadcast__Changed += callback;
                    break;
                case "ProxyStats":
                    ProxyStats__Changed += callback;
                    break;
                case "ProceduralStats":
                    ProceduralStats__Changed += callback;
                    break;
                case "AccumulatedStats":
                    AccumulatedStats__Changed += callback;
                    break;
                case "ProceduralStatsBroadcast":
                    ProceduralStatsBroadcast__Changed += callback;
                    break;
                case "AccumulatedStatsBroadcast":
                    AccumulatedStatsBroadcast__Changed += callback;
                    break;
                case "TimeWhenIdleStarted":
                    TimeWhenIdleStarted__Changed += callback;
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
                case "StatsDef":
                    StatsDef__Changed -= callback;
                    break;
                case "TimeStats":
                    TimeStats__Changed -= callback;
                    break;
                case "TimeStatsBroadcast":
                    TimeStatsBroadcast__Changed -= callback;
                    break;
                case "ValueStats":
                    ValueStats__Changed -= callback;
                    break;
                case "ValueStatsBroadcast":
                    ValueStatsBroadcast__Changed -= callback;
                    break;
                case "ProxyStats":
                    ProxyStats__Changed -= callback;
                    break;
                case "ProceduralStats":
                    ProceduralStats__Changed -= callback;
                    break;
                case "AccumulatedStats":
                    AccumulatedStats__Changed -= callback;
                    break;
                case "ProceduralStatsBroadcast":
                    ProceduralStatsBroadcast__Changed -= callback;
                    break;
                case "AccumulatedStatsBroadcast":
                    AccumulatedStatsBroadcast__Changed -= callback;
                    break;
                case "TimeWhenIdleStarted":
                    TimeWhenIdleStarted__Changed -= callback;
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
                case "StatsDef":
                    StatsDef__Changed = null;
                    break;
                case "TimeStats":
                    TimeStats__Changed = null;
                    break;
                case "TimeStatsBroadcast":
                    TimeStatsBroadcast__Changed = null;
                    break;
                case "ValueStats":
                    ValueStats__Changed = null;
                    break;
                case "ValueStatsBroadcast":
                    ValueStatsBroadcast__Changed = null;
                    break;
                case "ProxyStats":
                    ProxyStats__Changed = null;
                    break;
                case "ProceduralStats":
                    ProceduralStats__Changed = null;
                    break;
                case "AccumulatedStats":
                    AccumulatedStats__Changed = null;
                    break;
                case "ProceduralStatsBroadcast":
                    ProceduralStatsBroadcast__Changed = null;
                    break;
                case "AccumulatedStatsBroadcast":
                    AccumulatedStatsBroadcast__Changed = null;
                    break;
                case "TimeWhenIdleStarted":
                    TimeWhenIdleStarted__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            StatsDef__Changed = null;
            TimeStats__Changed = null;
            TimeStatsBroadcast__Changed = null;
            ValueStats__Changed = null;
            ValueStatsBroadcast__Changed = null;
            ProxyStats__Changed = null;
            ProceduralStats__Changed = null;
            AccumulatedStats__Changed = null;
            ProceduralStatsBroadcast__Changed = null;
            AccumulatedStatsBroadcast__Changed = null;
            TimeWhenIdleStarted__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && StatsDef__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_StatsDef, nameof(StatsDef), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, StatsDef__Changed);
            }

            if (NeedFireEvent(11) && TimeStats__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_TimeStats, nameof(TimeStats), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, TimeStats__Changed);
            }

            if (NeedFireEvent(12) && TimeStatsBroadcast__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_TimeStatsBroadcast, nameof(TimeStatsBroadcast), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, TimeStatsBroadcast__Changed);
            }

            if (NeedFireEvent(13) && ValueStats__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ValueStats, nameof(ValueStats), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ValueStats__Changed);
            }

            if (NeedFireEvent(14) && ValueStatsBroadcast__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ValueStatsBroadcast, nameof(ValueStatsBroadcast), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ValueStatsBroadcast__Changed);
            }

            if (NeedFireEvent(15) && ProxyStats__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 15;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ProxyStats, nameof(ProxyStats), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ProxyStats__Changed);
            }

            if (NeedFireEvent(16) && ProceduralStats__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 16;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ProceduralStats, nameof(ProceduralStats), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ProceduralStats__Changed);
            }

            if (NeedFireEvent(17) && AccumulatedStats__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 17;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_AccumulatedStats, nameof(AccumulatedStats), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, AccumulatedStats__Changed);
            }

            if (NeedFireEvent(18) && ProceduralStatsBroadcast__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 18;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ProceduralStatsBroadcast, nameof(ProceduralStatsBroadcast), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ProceduralStatsBroadcast__Changed);
            }

            if (NeedFireEvent(19) && AccumulatedStatsBroadcast__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 19;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_AccumulatedStatsBroadcast, nameof(AccumulatedStatsBroadcast), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, AccumulatedStatsBroadcast__Changed);
            }

            if (NeedFireEvent(20) && TimeWhenIdleStarted__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 20;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_TimeWhenIdleStarted, nameof(TimeWhenIdleStarted), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, TimeWhenIdleStarted__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                StatsDef = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                TimeStats = default;
            if (_TimeStats != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_TimeStats).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                TimeStatsBroadcast = default;
            if (_TimeStatsBroadcast != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_TimeStatsBroadcast).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                ValueStats = default;
            if (_ValueStats != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ValueStats).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                ValueStatsBroadcast = default;
            if (_ValueStatsBroadcast != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ValueStatsBroadcast).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                ProxyStats = default;
            if (_ProxyStats != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProxyStats).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                ProceduralStats = default;
            if (_ProceduralStats != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProceduralStats).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                AccumulatedStats = default;
            if (_AccumulatedStats != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_AccumulatedStats).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                ProceduralStatsBroadcast = default;
            if (_ProceduralStatsBroadcast != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ProceduralStatsBroadcast).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                AccumulatedStatsBroadcast = default;
            if (_AccumulatedStatsBroadcast != null)
                ((SharedCode.EntitySystem.IDeltaObjectExt)_AccumulatedStatsBroadcast).Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                TimeWhenIdleStarted = default;
        }
    }
}