using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.CustomData;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Refs;

namespace GeneratedCode.DeltaObjects
{
    public partial class MachineOutputContainer
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

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
            return Task.FromResult(Items.Values.Where(v => v?.Item?.ItemResource != null).Select(v => v.Stack * v.Item.ItemResource.Weight).Sum());
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
            return Task.FromResult(item.ItemResource is ItemResource && !(item.ItemResource is DollItemResource) && !manual);
        }

        public Task<bool> CanRemoveImpl(IItem item, int index, int count, bool manual)
        {
            return Task.FromResult(true);
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