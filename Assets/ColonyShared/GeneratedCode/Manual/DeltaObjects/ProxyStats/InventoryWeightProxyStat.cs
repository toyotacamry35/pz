using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using Src.Aspects.Impl.Stats;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    [GenerateDeltaObjectCode]
    public interface IInventoryWeightProxyStat : IProxyStat
    {
        [ReplicationLevel(ReplicationLevel.Server)] Task SetCacheValue(float value);
    }

    public partial class InventoryWeightProxyStat
    {
        private PropertyAddress _containerAddress;

        public ValueTask InitializeImpl(StatDef statDef, bool resetState)
        {
            LimitMaxCache = float.MaxValue;
            return new ValueTask();
        }

        public ValueTask<float> GetValueImpl()
        {
            return new ValueTask<float>(ValueCache);
        }

        public Task SetCacheValueImpl(float value)
        {
            ValueCache = value;
            return Task.CompletedTask;
        }

        public async Task ProxySubscribeImpl(PropertyAddress containerAddress)
        {
            if (containerAddress?.IsValid() ?? false)
            {
                _containerAddress = containerAddress;
                using (var wrapper = await EntitiesRepository.Get(_containerAddress.EntityTypeId, _containerAddress.EntityId))
                {
                    var entity = wrapper.Get<IEntity>(_containerAddress.EntityTypeId, _containerAddress.EntityId, ReplicationLevel.ClientFull);
                    var container = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(entity, _containerAddress, ReplicationLevel.ClientFull);

                    container.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdatedWrap;
                    container.Items.OnItemRemoved += Items_OnItemRemovedWrap;

                    await RecalculateCachesImpl(false);
                }
            }
        }

        private async Task Items_OnItemAddedOrUpdatedWrap(DeltaDictionaryChangedEventArgs<int, ISlotItemClientFull> args)
        {
            using (var wrapper = await EntitiesRepository.Get(ParentTypeId, ParentEntityId))
            {
                await Items_OnItemAddedOrUpdated(args);
            }
        }
        private async Task Items_OnItemAddedOrUpdated(DeltaDictionaryChangedEventArgs<int, ISlotItemClientFull> args)
        {
            args.Value.SubscribePropertyChanged(nameof(args.Value.Stack), Stack_PropertyChanged);
            using (var sender = await EntitiesRepository.Get(ParentTypeId, ParentEntityId))
            {
                await RecalculateCaches(false);
            }
        }

        private async Task Items_OnItemRemovedWrap(DeltaDictionaryChangedEventArgs<int, ISlotItemClientFull> args)
        {
            using (var wrapper = await EntitiesRepository.Get(ParentTypeId, ParentEntityId))
            {
                await Items_OnItemRemoved(args);
            }
        }
        private async Task Items_OnItemRemoved(DeltaDictionaryChangedEventArgs<int, ISlotItemClientFull> args)
        {
            args.OldValue.UnsubscribePropertyChanged(nameof(args.OldValue.Stack), Stack_PropertyChanged);
            using (var sender = await EntitiesRepository.Get(ParentTypeId, ParentEntityId))
            {
                await RecalculateCaches(false);
            }
        }

        private async Task Stack_PropertyChanged(EntityEventArgs args)
        {
            using (var sender = await EntitiesRepository.Get(ParentTypeId, ParentEntityId))
            {
                await RecalculateCaches(false);
            }
        }

        public async ValueTask<bool> RecalculateCachesImpl(bool calcersOnly)
        {
            using (var wrapper = await EntitiesRepository.Get(_containerAddress.EntityTypeId, _containerAddress.EntityId))
            {
                var entity = wrapper.Get<IEntity>(_containerAddress.EntityTypeId, _containerAddress.EntityId, ReplicationLevel.ClientFull);
                var container = EntityPropertyResolver.Resolve<IItemsContainerClientFull>(entity, _containerAddress, ReplicationLevel.ClientFull);
                var res = await container.GetTotalWeight();

                if (Math.Abs(Math.Abs(res) - Math.Abs(ValueCache)) > 0.000000001f)
                {
                    await SetCacheValue(res);
                    return true;
                    //await OnStatChangedInvoke(ValueCache, _recalculatedStats);
                }
            }

            return false;
        }

        public override string ToString()
        {
            return $"{ValueCache,6:F1} [{(LimitMinCache < -100000 ? "-∞" : LimitMinCache + ""),6:F1}; {(LimitMaxCache > 100000 ? "+∞" : LimitMaxCache + ""),6:F1}]";
        }
    }
}
