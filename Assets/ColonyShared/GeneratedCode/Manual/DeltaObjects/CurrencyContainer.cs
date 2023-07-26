using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public partial class CurrencyContainer
    {
        protected override void constructor()
        {
            base.constructor();
            TransactionReservedSlots = new ConcurrentDictionary<int, Guid>();
        }

        public Task<float> GetMaxWeigthImpl()
        {
            return Task.FromResult(float.MaxValue);
        }

        public Task<float> GetTotalWeightImpl()
        {
            return Task.FromResult(0f);
        }

        public Task<int> GetMaxStackForSlotImpl(int destinationSlot)
        {
            return Task.FromResult(int.MaxValue);
        }

        public Task<bool> IgnoreMaxStackImpl()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanAutoselectEmptySlotsForAddStacksImpl()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanAddImpl(IItem item, int index, int count, bool manual)
        {
            return Task.FromResult(item.ItemResource is CurrencyResource);
        }

        public Task<bool> CanRemoveImpl(IItem item, int index, int count, bool manual)
        {
            return Task.FromResult(item.ItemResource is CurrencyResource);
        }

        public async Task OnItemAddedImpl(IItem item, int index, int count, bool manual)
        {
            await OnItemAddedToContainer(item?.ItemResource, index, count, manual);
        }

        public async Task<PropertyAddress> OnBeforeItemRemovedImpl(IItem item, int index, int count, bool manual)
        {
            await OnItemRemovedToContainer(item?.ItemResource, index, count, manual);
            return null;
        }
    }
}
