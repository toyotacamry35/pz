// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("AccumulatedStat")]
    public partial class AccumulatedStat : SharedCode.EntitySystem.BaseDeltaObject, Src.Aspects.Impl.Stats.IAccumulatedStat, IAccumulatedStatImplementRemoteMethods
    {
        public AccumulatedStat()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                InitialValue = default(float);
                ValueCache = default(float);
                LimitMinCache = default(float);
                LimitMaxCache = default(float);
                StatType = default(Assets.Src.Aspects.Impl.Stats.StatType);
            }

            constructor();
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "InitialValue":
                    InitialValue__Changed += callback;
                    break;
                case "ValueCache":
                    ValueCache__Changed += callback;
                    break;
                case "LimitMinCache":
                    LimitMinCache__Changed += callback;
                    break;
                case "LimitMaxCache":
                    LimitMaxCache__Changed += callback;
                    break;
                case "StatType":
                    StatType__Changed += callback;
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
                case "InitialValue":
                    InitialValue__Changed -= callback;
                    break;
                case "ValueCache":
                    ValueCache__Changed -= callback;
                    break;
                case "LimitMinCache":
                    LimitMinCache__Changed -= callback;
                    break;
                case "LimitMaxCache":
                    LimitMaxCache__Changed -= callback;
                    break;
                case "StatType":
                    StatType__Changed -= callback;
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
                case "InitialValue":
                    InitialValue__Changed = null;
                    break;
                case "ValueCache":
                    ValueCache__Changed = null;
                    break;
                case "LimitMinCache":
                    LimitMinCache__Changed = null;
                    break;
                case "LimitMaxCache":
                    LimitMaxCache__Changed = null;
                    break;
                case "StatType":
                    StatType__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            InitialValue__Changed = null;
            ValueCache__Changed = null;
            LimitMinCache__Changed = null;
            LimitMaxCache__Changed = null;
            StatType__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && InitialValue__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_InitialValue, nameof(InitialValue), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, InitialValue__Changed);
            }

            if (NeedFireEvent(11) && ValueCache__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_ValueCache, nameof(ValueCache), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ValueCache__Changed);
            }

            if (NeedFireEvent(12) && LimitMinCache__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_LimitMinCache, nameof(LimitMinCache), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, LimitMinCache__Changed);
            }

            if (NeedFireEvent(13) && LimitMaxCache__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_LimitMaxCache, nameof(LimitMaxCache), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, LimitMaxCache__Changed);
            }

            if (NeedFireEvent(14) && StatType__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_StatType, nameof(StatType), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, StatType__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                InitialValue = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                ValueCache = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                LimitMinCache = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                LimitMaxCache = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast & mask) > 0)
                StatType = default;
        }
    }
}