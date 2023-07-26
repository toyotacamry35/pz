// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    [ProtoBuf.ProtoContract]
    [MongoDB.Bson.Serialization.Attributes.BsonDiscriminator("CraftingQueueItem")]
    public partial class CraftingQueueItem : SharedCode.EntitySystem.BaseDeltaObject, SharedCode.Entities.Engine.ICraftingQueueItem
    {
        public CraftingQueueItem()
        {
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                Index = default(int);
                CraftRecipe = default(Assets.ColonyShared.SharedCode.Aspects.Craft.CraftRecipeDef);
                MandatorySlotPermutation = new System.Collections.Generic.List<int>();
                OptionalSlotPermutation = new System.Collections.Generic.List<int>();
                SelectedVariantIndex = default(int);
                TimeAlreadyCrafted = default(long);
                CraftStartTime = default(long);
                IsActive = default(bool);
                Count = default(int);
            }

            constructor();
        }

        protected override void Subscribe(string propertyName, SharedCode.EntitySystem.PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "Index":
                    Index__Changed += callback;
                    break;
                case "CraftRecipe":
                    CraftRecipe__Changed += callback;
                    break;
                case "MandatorySlotPermutation":
                    MandatorySlotPermutation__Changed += callback;
                    break;
                case "OptionalSlotPermutation":
                    OptionalSlotPermutation__Changed += callback;
                    break;
                case "SelectedVariantIndex":
                    SelectedVariantIndex__Changed += callback;
                    break;
                case "TimeAlreadyCrafted":
                    TimeAlreadyCrafted__Changed += callback;
                    break;
                case "CraftStartTime":
                    CraftStartTime__Changed += callback;
                    break;
                case "IsActive":
                    IsActive__Changed += callback;
                    break;
                case "Count":
                    Count__Changed += callback;
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
                case "Index":
                    Index__Changed -= callback;
                    break;
                case "CraftRecipe":
                    CraftRecipe__Changed -= callback;
                    break;
                case "MandatorySlotPermutation":
                    MandatorySlotPermutation__Changed -= callback;
                    break;
                case "OptionalSlotPermutation":
                    OptionalSlotPermutation__Changed -= callback;
                    break;
                case "SelectedVariantIndex":
                    SelectedVariantIndex__Changed -= callback;
                    break;
                case "TimeAlreadyCrafted":
                    TimeAlreadyCrafted__Changed -= callback;
                    break;
                case "CraftStartTime":
                    CraftStartTime__Changed -= callback;
                    break;
                case "IsActive":
                    IsActive__Changed -= callback;
                    break;
                case "Count":
                    Count__Changed -= callback;
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
                case "Index":
                    Index__Changed = null;
                    break;
                case "CraftRecipe":
                    CraftRecipe__Changed = null;
                    break;
                case "MandatorySlotPermutation":
                    MandatorySlotPermutation__Changed = null;
                    break;
                case "OptionalSlotPermutation":
                    OptionalSlotPermutation__Changed = null;
                    break;
                case "SelectedVariantIndex":
                    SelectedVariantIndex__Changed = null;
                    break;
                case "TimeAlreadyCrafted":
                    TimeAlreadyCrafted__Changed = null;
                    break;
                case "CraftStartTime":
                    CraftStartTime__Changed = null;
                    break;
                case "IsActive":
                    IsActive__Changed = null;
                    break;
                case "Count":
                    Count__Changed = null;
                    break;
                default:
                    throw new System.ArgumentException($"Field {propertyName} does not exist in {GetType()}", nameof(propertyName));
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            Index__Changed = null;
            CraftRecipe__Changed = null;
            MandatorySlotPermutation__Changed = null;
            OptionalSlotPermutation__Changed = null;
            SelectedVariantIndex__Changed = null;
            TimeAlreadyCrafted__Changed = null;
            CraftStartTime__Changed = null;
            IsActive__Changed = null;
            Count__Changed = null;
        }

        protected override void FireEvents(System.Collections.Generic.List<System.Func<System.Threading.Tasks.Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(10) && Index__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 10;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Index, nameof(Index), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Index__Changed);
            }

            if (NeedFireEvent(11) && CraftRecipe__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 11;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CraftRecipe, nameof(CraftRecipe), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CraftRecipe__Changed);
            }

            if (NeedFireEvent(12) && MandatorySlotPermutation__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 12;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_MandatorySlotPermutation, nameof(MandatorySlotPermutation), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, MandatorySlotPermutation__Changed);
            }

            if (NeedFireEvent(13) && OptionalSlotPermutation__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 13;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_OptionalSlotPermutation, nameof(OptionalSlotPermutation), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, OptionalSlotPermutation__Changed);
            }

            if (NeedFireEvent(14) && SelectedVariantIndex__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 14;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_SelectedVariantIndex, nameof(SelectedVariantIndex), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, SelectedVariantIndex__Changed);
            }

            if (NeedFireEvent(15) && TimeAlreadyCrafted__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 15;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_TimeAlreadyCrafted, nameof(TimeAlreadyCrafted), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, TimeAlreadyCrafted__Changed);
            }

            if (NeedFireEvent(16) && CraftStartTime__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 16;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_CraftStartTime, nameof(CraftStartTime), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, CraftStartTime__Changed);
            }

            if (NeedFireEvent(17) && IsActive__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 17;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_IsActive, nameof(IsActive), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, IsActive__Changed);
            }

            if (NeedFireEvent(18) && Count__Changed != null)
            {
                SharedCode.EntitySystem.PropertyAddress __propAddress__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 18;
                var __Event__Args__ = new SharedCode.EntitySystem.EntityEventArgs(_Count, nameof(Count), __propAddress__, this);
                GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, Count__Changed);
            }
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                Index = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                CraftRecipe = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                MandatorySlotPermutation = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                OptionalSlotPermutation = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                SelectedVariantIndex = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                TimeAlreadyCrafted = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                CraftStartTime = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                IsActive = default;
            if (((long)SharedCode.EntitySystem.ReplicationLevel.ClientFull & mask) > 0)
                Count = default;
        }
    }
}