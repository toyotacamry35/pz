using Assets.ColonyShared.SharedCode.Aspects.Item;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorAnnotations;

namespace SharedCode.Entities
{
    public interface IHasContainerApi
    {
        IContainerApi ContainerApi { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface IContainerApi : IDeltaObject
    {
        ///////////////////////////////////// MoveAdd
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerOperationAddPrepareResult> ContainerOperationMoveAddPrepare(Guid transactionId, PropertyAddress address, int slotId, IItemWrapper itemWrapper, bool manual);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> ContainerOperationMoveAddCommit(Guid transactionId);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> ContainerOperationMoveAddRollback(Guid transactionId);

        ///////////////////////////////////// MoveRemove
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerOperationRemovePrepareResult> ContainerOperationMoveRemovePrepare(Guid transactionId, PropertyAddress address, int slotId, int count, Guid clientSrcEntityId, bool manual);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperation> ContainerOperationMoveRemoveCommit(Guid transactionId);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> ContainerOperationMoveRemoveRollback(Guid transactionId);

        ///////////////////////////////////// MoveAll
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerOperationMoveAllAddPrepareResult> ContainerOperationMoveAllAddPrepare(Guid transactionId, PropertyAddress address, Dictionary<int, IItemWrapper> itemWrappers, bool manual, bool sameSlots);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> ContainerOperationMoveAllAddCommit(Guid transactionId);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> ContainerOperationMoveAllAddRollback(Guid transactionId);

        ///////////////////////////////////// MoveAllRemove
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerOperationMoveAllRemovePrepareResult> ContainerOperationMoveAllRemovePrepare(Guid transactionId, PropertyAddress address, bool manual);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> ContainerOperationMoveAllRemoveCommit(Guid transactionId);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<bool> ContainerOperationMoveAllRemoveRollback(Guid transactionId);

        ///////////////////////////////////// MoveAllRemove
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperation> ContainerOperationMoveChangePrepare(Guid transactionId, IItemWrapper itemWrapper);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperation> ContainerOperationAddNewItem(IItemWrapper itemWrapper, PropertyAddress destination, int destinationSlotId, bool manual);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperation> ContainerOperationAddItems(List<ItemResourcePack> itemResourcesToAdd, PropertyAddress destination, bool manual);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperation> ContainerOperationRemoveBatchItem(List<RemoveItemBatchElement> items, bool manual);
        [ReplicationLevel(ReplicationLevel.ServerApi)] Task<ContainerItemOperation> ContainerOperationMoveAllChangePrepare(Guid transactionId, Dictionary<int, IItemWrapper> itemWrappers);

        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<PropertyAddress> ContainerOperationSetSize(PropertyAddress address, int size);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<ContainerItemOperation> Drop(PropertyAddress address, int slotId, int stackCount);
    }
}