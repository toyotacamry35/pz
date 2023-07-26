using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;

namespace GeneratedCode.Transactions
{
    public class ItemMoveAllManagementTransaction : BaseTransaction<ContainerItemOperation>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private PropertyAddress _source;
        private PropertyAddress _destination;
        private bool _manual;
        private bool _sameSlots;

        public ItemMoveAllManagementTransaction(PropertyAddress source, PropertyAddress destination, bool manual, bool sameSlots, IEntitiesRepository repository) : base(repository)
        {
            _source = source;
            _destination = destination;
            _manual = manual;
            _sameSlots = sameSlots;
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
                    Logger.IfError()?.Message("Source entity not found {0}", _source).Write();
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorSrcNotFound };
                }

                var destEntity = container.Get<IHasContainerApiServer>(_destination.EntityTypeId, _destination.EntityId, ReplicationLevel.Server);
                if (destEntity == null)
                {
                    Logger.IfError()?.Message("Destination entity not found {0}", _destination).Write();
                    return new ContainerItemOperation() { Result = ContainerItemOperationResult.ErrorDstNotFound };
                }

                var removePrepareResult = await sourceEntity.ContainerApi.ContainerOperationMoveAllRemovePrepare(Id, _source, _manual);
                if (removePrepareResult == null || removePrepareResult.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                {
                    Logger.IfError()?.Message("ContainerOperationRemovePrepare {0} fail", _source).Write();
                    return new ContainerItemOperation() { Result = removePrepareResult?.ContainerItemOperationResult ?? ContainerItemOperationResult.ErrorUnknown };
                }

                var addPrepareResult = await destEntity.ContainerApi.ContainerOperationMoveAllAddPrepare(Id, _destination, removePrepareResult.Items, _manual, _sameSlots);
                if (addPrepareResult.ContainerItemOperationResult != ContainerItemOperationResult.Success)
                {
                    var rollbackResult = await sourceEntity.ContainerApi.ContainerOperationMoveRemoveRollback(Id);
                    if (!rollbackResult)
                        Logger.IfError()?.Message("rollback error {0}", Id).Write();

                    return new ContainerItemOperation() { Result = addPrepareResult.ContainerItemOperationResult };
                }

                var changeResult = await sourceEntity.ContainerApi.ContainerOperationMoveAllChangePrepare(Id, addPrepareResult.Items);
                if (!changeResult.IsSuccess)
                {
                    var removeRollbackResult = await sourceEntity.ContainerApi.ContainerOperationMoveAllRemoveRollback(Id);
                    if (!removeRollbackResult)
                        Logger.IfError()?.Message("Transaction {0} remove rollback error", Id).Write();

                    var addRollbackResult = await destEntity.ContainerApi.ContainerOperationMoveAllAddRollback(Id);
                    if (!addRollbackResult)
                        Logger.IfError()?.Message("Transaction {0} add rollback error", Id).Write();

                    return changeResult;
                }

                var removeCommitResult = await sourceEntity.ContainerApi.ContainerOperationMoveAllRemoveCommit(Id); 
                if (!removeCommitResult)
                    Logger.IfError()?.Message("Transaction {0} remove commit error", Id).Write();

                var addCommitResult = await destEntity.ContainerApi.ContainerOperationMoveAllAddCommit(Id);
                if (!addCommitResult)
                    Logger.IfError()?.Message("Transaction {0} add commit error", Id).Write();

            }

            return new ContainerItemOperation() { Result = ContainerItemOperationResult.Success };
        }
    }
}
