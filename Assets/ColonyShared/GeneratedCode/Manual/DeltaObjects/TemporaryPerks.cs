using SharedCode.Entities;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class TemporaryPerks
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
            return Task.FromResult(0.0f);
        }

        public Task<int> GetMaxStackForSlotImpl(int destinationSlot)
        {
            return Task.FromResult(1);
        }

        public Task<bool> IgnoreMaxStackImpl()
        {
            return Task.FromResult(false);
        }

        public Task<bool> CanAutoselectEmptySlotsForAddStacksImpl()
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanAddImpl(IItem item, int index, int count, bool manual)
        {
            return Task.FromResult(item.ItemResource is PerkResource);
        }

        public Task<bool> CanRemoveImpl(IItem item, int index, int count, bool manual)
        {
            return Task.FromResult(true); // await CharacterPerks.CanRemove(parentEntity, item, index, count, manual, GetType());
        }

        public async Task OnItemAddedImpl(IItem item, int index, int count, bool manual)
        {
            await OnItemAddedToContainer(item?.ItemResource, index, count, manual);
        }

        public async Task<PropertyAddress> OnBeforeItemRemovedImpl(IItem item, int index, int count, bool manual)
        {
            await OnItemRemovedToContainer(item?.ItemResource, index, count, manual);
            return await CharacterPerks.OnBeforeItemRemoved(parentEntity as IWorldCharacter, item, index, count, manual);
        }

        public Task AddPerkSlotImpl(int slotId, ItemTypeResource perkSlotType)
        {
            PerkSlots[slotId] = perkSlotType;
            return Task.CompletedTask;
        }

        public Task RemovePerkSlotImpl(int slotId)
        {
            PerkSlots.Remove(slotId);
            return Task.CompletedTask;
        }
    }
}