using Assets.ColonyShared.SharedCode.Aspects.Item;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.Transactions
{
    public class ItemMoveManagementTransaction: BaseTransaction<ContainerItemOperation>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private PropertyAddress _source;
        private int _sourceSlotId;
        private PropertyAddress _destination;
        private int _destinationSlotId;
        private int _count;
        private Guid _clientSrcEntityId;
        private bool _manual;

        public ItemMoveManagementTransaction(PropertyAddress source, int sourceSlotId, PropertyAddress destination, int destinationSlotId, 
            int count, Guid clientSrcEntityId, bool manual, IEntitiesRepository repository)
            :base(repository)
        {
            _source = source;
            _sourceSlotId = sourceSlotId;
            _destination = destination;
            _destinationSlotId = destinationSlotId;
            _count = count;
            _clientSrcEntityId = clientSrcEntityId;
            _manual = manual;
        }

        public static implicit operator RemoveItemBatchElement (ItemMoveManagementTransaction transaction)
        {
            return new RemoveItemBatchElement(transaction._source, transaction._sourceSlotId, transaction._count, transaction._clientSrcEntityId);
        }

        public override async Task<ContainerItemOperation> ExecuteTransaction()
        {
            var batch = EntityBatch.Create()
                .Add(_source.EntityTypeId, _source.EntityId)
                .Add(_destination.EntityTypeId, _destination.EntityId);

            using (var container = await Repository.Get(batch))
            {
                var sourceEntity = container.Get<IHasContainerApiServer>(_source.EntityTypeId, _source.EntityId, ReplicationLevel.Server);
                if (sourceEntity == null)
                {
                    Logger.IfError()?.Message("Source entity not found {0} or cant cast to IHasContainerApi", _source).Write();
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcNotFound };
                }

                var destEntity = container.Get<IHasContainerApiServer>(_destination.EntityTypeId, _destination.EntityId, ReplicationLevel.Server);
                if (destEntity == null)
                {
                    Logger.IfError()?.Message("Destination entity not found {0} or cant cast to IHasContainerApi", _destination).Write();
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstNotFound };
                }

                var removePrepareResult = await sourceEntity.ContainerApi.ContainerOperationMoveRemovePrepare(Id, _source, _sourceSlotId, _count, _clientSrcEntityId, _manual);
                if (removePrepareResult == null || removePrepareResult.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                {
                    Logger.IfError()?.Message("ContainerOperationRemovePrepare {0} fail. Result {1}", _source, removePrepareResult?.ContainerItemOperationResult.ToString() ?? "null").Write();
                    return new ContainerItemOperation() { Result = removePrepareResult?.ContainerItemOperationResult ?? ContainerItemOperationResult.ErrorUnknown };
                }

                var addPrepareResult = await destEntity.ContainerApi.ContainerOperationMoveAddPrepare(Id, _destination, _destinationSlotId, new IItemWrapper { Item = removePrepareResult.Item, Count = _count }, _manual);
                if (addPrepareResult.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                {
                    var rollbackResult = await sourceEntity.ContainerApi.ContainerOperationMoveRemoveRollback(Id);
                    if (!rollbackResult)
                        Logger.IfError()?.Message("rollback error {0}", Id).Write();

                    return new ContainerItemOperation() { Result = addPrepareResult.ContainerItemOperationResult };
                }

                if (addPrepareResult.Count < _count)
                {
                    var removeRollbackResult = await sourceEntity.ContainerApi.ContainerOperationMoveRemoveRollback(Id);
                    if (!removeRollbackResult)
                    {
                        Logger.IfError()?.Message("Transaction {0} remove rollback error", Id).Write();

                        var addRollbackResult = await destEntity.ContainerApi.ContainerOperationMoveAddRollback(Id);
                        if (!addRollbackResult)
                            Logger.IfError()?.Message("Transaction {0} add rollback error", Id).Write();

                        return new ContainerItemOperation() { Result = removePrepareResult?.ContainerItemOperationResult ?? ContainerItemOperationResult.ErrorUnknown };
                    }
                    else
                    {
                        removePrepareResult = await sourceEntity.ContainerApi.ContainerOperationMoveRemovePrepare(Id, _source, _sourceSlotId, addPrepareResult.Count, _clientSrcEntityId, _manual);
                        if (removePrepareResult == null || removePrepareResult.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                        {
                            Logger.IfError()?.Message("ContainerOperationRemovePrepare {0} fail. Result {1}", _source, removePrepareResult?.ContainerItemOperationResult.ToString() ?? "null").Write();
                            return new ContainerItemOperation() { Result = removePrepareResult?.ContainerItemOperationResult ?? ContainerItemOperationResult.ErrorUnknown };
                        }
                    }
                }

                if (addPrepareResult.Item != null)
                {
                    var changeResult = await sourceEntity.ContainerApi.ContainerOperationMoveChangePrepare(Id, new IItemWrapper { Item = addPrepareResult.Item, Count = addPrepareResult.Count });
                    if (!changeResult.IsSuccess)
                    {
                        var removeRollbackResult = await sourceEntity.ContainerApi.ContainerOperationMoveRemoveRollback(Id);
                        if (!removeRollbackResult)
                            Logger.IfError()?.Message("Transaction {0} remove rollback error", Id).Write();

                        var addRollbackResult = await destEntity.ContainerApi.ContainerOperationMoveAddRollback(Id);
                        if (!addRollbackResult)
                            Logger.IfError()?.Message("Transaction {0} remove rollback error", Id).Write();

                        return changeResult;
                    }
                }

                var removeCommitResult = await sourceEntity.ContainerApi.ContainerOperationMoveRemoveCommit(Id);
                if (removeCommitResult == null || removeCommitResult.IsSuccess)
                {
                    await destEntity.ContainerApi.ContainerOperationMoveAddCommit(Id);
                }
            }

            return new ContainerItemOperation() { ItemsCount = _count, Result = ContainerItemOperationResult.Success };
        }
    }
}
