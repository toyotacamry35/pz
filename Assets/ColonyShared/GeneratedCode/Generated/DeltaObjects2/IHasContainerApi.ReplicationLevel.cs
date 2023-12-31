// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public class ContainerApiAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiAlways
    {
        public ContainerApiAlways(SharedCode.Entities.IContainerApi deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IContainerApi __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IContainerApi)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1047081823;
    }

    public class ContainerApiClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiClientBroadcast
    {
        public ContainerApiClientBroadcast(SharedCode.Entities.IContainerApi deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IContainerApi __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IContainerApi)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1208426381;
    }

    public class ContainerApiClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiClientFullApi
    {
        public ContainerApiClientFullApi(SharedCode.Entities.IContainerApi deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IContainerApi __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IContainerApi)__deltaObjectBase__;
            }
        }

        public override int TypeId => -750579503;
    }

    public class ContainerApiClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiClientFull
    {
        public ContainerApiClientFull(SharedCode.Entities.IContainerApi deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IContainerApi __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IContainerApi)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.EntitySystem.PropertyAddress> ContainerOperationSetSize(SharedCode.EntitySystem.PropertyAddress address, int size)
        {
            return __deltaObject__.ContainerOperationSetSize(address, size);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> Drop(SharedCode.EntitySystem.PropertyAddress address, int slotId, int stackCount)
        {
            return __deltaObject__.Drop(address, slotId, stackCount);
        }

        public override int TypeId => -1893414868;
    }

    public class ContainerApiServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiServerApi
    {
        public ContainerApiServerApi(SharedCode.Entities.IContainerApi deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IContainerApi __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IContainerApi)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationAddPrepareResult> ContainerOperationMoveAddPrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, int slotId, SharedCode.Entities.IItemWrapper itemWrapper, bool manual)
        {
            return __deltaObject__.ContainerOperationMoveAddPrepare(transactionId, address, slotId, itemWrapper, manual);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAddCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAddCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAddRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAddRollback(transactionId);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationRemovePrepareResult> ContainerOperationMoveRemovePrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, int slotId, int count, System.Guid clientSrcEntityId, bool manual)
        {
            return __deltaObject__.ContainerOperationMoveRemovePrepare(transactionId, address, slotId, count, clientSrcEntityId, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationMoveRemoveCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveRemoveCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveRemoveRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveRemoveRollback(transactionId);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationMoveAllAddPrepareResult> ContainerOperationMoveAllAddPrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, System.Collections.Generic.Dictionary<int, SharedCode.Entities.IItemWrapper> itemWrappers, bool manual, bool sameSlots)
        {
            return __deltaObject__.ContainerOperationMoveAllAddPrepare(transactionId, address, itemWrappers, manual, sameSlots);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllAddCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllAddCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllAddRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllAddRollback(transactionId);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationMoveAllRemovePrepareResult> ContainerOperationMoveAllRemovePrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, bool manual)
        {
            return __deltaObject__.ContainerOperationMoveAllRemovePrepare(transactionId, address, manual);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllRemoveCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllRemoveCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllRemoveRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllRemoveRollback(transactionId);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationMoveChangePrepare(System.Guid transactionId, SharedCode.Entities.IItemWrapper itemWrapper)
        {
            return __deltaObject__.ContainerOperationMoveChangePrepare(transactionId, itemWrapper);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationAddNewItem(SharedCode.Entities.IItemWrapper itemWrapper, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, bool manual)
        {
            return __deltaObject__.ContainerOperationAddNewItem(itemWrapper, destination, destinationSlotId, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationAddItems(System.Collections.Generic.List<SharedCode.Entities.ItemResourcePack> itemResourcesToAdd, SharedCode.EntitySystem.PropertyAddress destination, bool manual)
        {
            return __deltaObject__.ContainerOperationAddItems(itemResourcesToAdd, destination, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationRemoveBatchItem(System.Collections.Generic.List<SharedCode.Entities.RemoveItemBatchElement> items, bool manual)
        {
            return __deltaObject__.ContainerOperationRemoveBatchItem(items, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationMoveAllChangePrepare(System.Guid transactionId, System.Collections.Generic.Dictionary<int, SharedCode.Entities.IItemWrapper> itemWrappers)
        {
            return __deltaObject__.ContainerOperationMoveAllChangePrepare(transactionId, itemWrappers);
        }

        public override int TypeId => 412178022;
    }

    public class ContainerApiServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IContainerApiServer
    {
        public ContainerApiServer(SharedCode.Entities.IContainerApi deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.IContainerApi __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.IContainerApi)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationAddPrepareResult> ContainerOperationMoveAddPrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, int slotId, SharedCode.Entities.IItemWrapper itemWrapper, bool manual)
        {
            return __deltaObject__.ContainerOperationMoveAddPrepare(transactionId, address, slotId, itemWrapper, manual);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAddCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAddCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAddRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAddRollback(transactionId);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationRemovePrepareResult> ContainerOperationMoveRemovePrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, int slotId, int count, System.Guid clientSrcEntityId, bool manual)
        {
            return __deltaObject__.ContainerOperationMoveRemovePrepare(transactionId, address, slotId, count, clientSrcEntityId, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationMoveRemoveCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveRemoveCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveRemoveRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveRemoveRollback(transactionId);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationMoveAllAddPrepareResult> ContainerOperationMoveAllAddPrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, System.Collections.Generic.Dictionary<int, SharedCode.Entities.IItemWrapper> itemWrappers, bool manual, bool sameSlots)
        {
            return __deltaObject__.ContainerOperationMoveAllAddPrepare(transactionId, address, itemWrappers, manual, sameSlots);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllAddCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllAddCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllAddRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllAddRollback(transactionId);
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.ContainerOperationMoveAllRemovePrepareResult> ContainerOperationMoveAllRemovePrepare(System.Guid transactionId, SharedCode.EntitySystem.PropertyAddress address, bool manual)
        {
            return __deltaObject__.ContainerOperationMoveAllRemovePrepare(transactionId, address, manual);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllRemoveCommit(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllRemoveCommit(transactionId);
        }

        public System.Threading.Tasks.Task<bool> ContainerOperationMoveAllRemoveRollback(System.Guid transactionId)
        {
            return __deltaObject__.ContainerOperationMoveAllRemoveRollback(transactionId);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationMoveChangePrepare(System.Guid transactionId, SharedCode.Entities.IItemWrapper itemWrapper)
        {
            return __deltaObject__.ContainerOperationMoveChangePrepare(transactionId, itemWrapper);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationAddNewItem(SharedCode.Entities.IItemWrapper itemWrapper, SharedCode.EntitySystem.PropertyAddress destination, int destinationSlotId, bool manual)
        {
            return __deltaObject__.ContainerOperationAddNewItem(itemWrapper, destination, destinationSlotId, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationAddItems(System.Collections.Generic.List<SharedCode.Entities.ItemResourcePack> itemResourcesToAdd, SharedCode.EntitySystem.PropertyAddress destination, bool manual)
        {
            return __deltaObject__.ContainerOperationAddItems(itemResourcesToAdd, destination, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationRemoveBatchItem(System.Collections.Generic.List<SharedCode.Entities.RemoveItemBatchElement> items, bool manual)
        {
            return __deltaObject__.ContainerOperationRemoveBatchItem(items, manual);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> ContainerOperationMoveAllChangePrepare(System.Guid transactionId, System.Collections.Generic.Dictionary<int, SharedCode.Entities.IItemWrapper> itemWrappers)
        {
            return __deltaObject__.ContainerOperationMoveAllChangePrepare(transactionId, itemWrappers);
        }

        public System.Threading.Tasks.Task<SharedCode.EntitySystem.PropertyAddress> ContainerOperationSetSize(SharedCode.EntitySystem.PropertyAddress address, int size)
        {
            return __deltaObject__.ContainerOperationSetSize(address, size);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Item.ContainerItemOperation> Drop(SharedCode.EntitySystem.PropertyAddress address, int slotId, int stackCount)
        {
            return __deltaObject__.Drop(address, slotId, stackCount);
        }

        public override int TypeId => -895479309;
    }
}