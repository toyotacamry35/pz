// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("TraumaGiver")]
    public partial class TraumaGiver : SharedCode.EntitySystem.BaseDeltaObject, Src.Aspects.Impl.Stats.ITraumaGiver, ITraumaGiverImplementRemoteMethods
    {
        public TraumaGiver()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                SpellId = default(ulong);
                CurrentTraumaPoints = default(int);
                Def = default(Assets.Src.Aspects.Impl.Traumas.Template.TraumaDef);
            }

            constructor();
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "SpellId":
                    SpellId__Changed += callback;
                    break;
                case "CurrentTraumaPoints":
                    CurrentTraumaPoints__Changed += callback;
                    break;
                case "Def":
                    Def__Changed += callback;
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
                case "SpellId":
                    SpellId__Changed -= callback;
                    break;
                case "CurrentTraumaPoints":
                    CurrentTraumaPoints__Changed -= callback;
                    break;
                case "Def":
                    Def__Changed -= callback;
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
                case "SpellId":
                    SpellId__Changed = null;
                    break;
                case "CurrentTraumaPoints":
                    CurrentTraumaPoints__Changed = null;
                    break;
                case "Def":
                    Def__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            SpellId__Changed = null;
            CurrentTraumaPoints__Changed = null;
            Def__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && SpellId__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_SpellId, nameof(SpellId), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, SpellId__Changed);
            }

            if (NeedFireEvent(11) && CurrentTraumaPoints__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CurrentTraumaPoints, nameof(CurrentTraumaPoints), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CurrentTraumaPoints__Changed);
            }

            if (NeedFireEvent(12) && Def__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Def, nameof(Def), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Def__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                SpellId = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                CurrentTraumaPoints = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                Def = default;
        }
    }
}