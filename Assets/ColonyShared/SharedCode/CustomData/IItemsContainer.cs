using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.CustomData
{
    public interface IItemsContainer
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        IDeltaDictionary<int, ISlotItem> Items { get; set; }

        [RuntimeData] 
        ConcurrentDictionary<int, Guid> TransactionReservedSlots { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        int Size { get; set; }

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.LockfreeLocal)]
        Task<float> GetMaxWeigth();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        Task<float> GetTotalWeight();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.LockfreeLocal)]
        Task<int> GetMaxStackForSlot(int destinationSlot);

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.LockfreeLocal)]
        Task<bool> IgnoreMaxStack();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)]
        [EntityMethodCallType(EntityMethodCallType.LockfreeLocal)]
        Task<bool> CanAutoselectEmptySlotsForAddStacks();

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]//TODO не ставить Local - внутри разные проверяются разные стейты фракций и т.д.
        Task<bool> CanAdd(IItem item, int index, int count, bool manual);

        [ReplicationLevel(ReplicationLevel.Server)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        Task<bool> CanRemove(IItem item, int index, int count, bool manual);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task OnItemAdded(IItem item, int index, int count, bool manual);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<PropertyAddress> OnBeforeItemRemoved(IItem item, int index, int count, bool manual);

        [ReplicationLevel(ReplicationLevel.ClientFull)]
        event Func<BaseItemResource /*item*/, int /*index*/, int /*count*/, bool /*manual*/, Task> ItemAddedToContainer;
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        event Func<BaseItemResource /*item*/, int /*slotId*/, int /*count*/, bool /*manual*/, Task> ItemRemovedToContainer;
    }
}
