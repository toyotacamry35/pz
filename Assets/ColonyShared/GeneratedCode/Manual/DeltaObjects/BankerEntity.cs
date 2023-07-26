using NLog;
using SharedCode.Entities;
using SharedCode.Refs;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Threading.Tasks;
using Assets.ResourceSystem.Aspects.Banks;
using SharedCode.EntitySystem.Delta;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using GeneratedCode.Transactions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class BankerEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<OuterRef> GetBankCellImpl(BankDef bankDef, OuterRef cellOwner)
        {
            //if (!cellOwner.IsValid)
            //    return default;

            if (!BankCells.TryGetValue(bankDef, out IBankHolder cells))
            {
                cells = new BankHolder();
                BankCells.Add(bankDef, cells);
            }

            if (cells.Cells.TryGetValue(cellOwner, out OuterRef cell))
            {
                EntityRef<IBankCell> loadedCell = await EntitiesRepository.Load<IBankCell>(cell.Guid);
                if (loadedCell == null)
                {
                    Logger.IfError()?.Message($"Could not load BankCell (BankDef = {bankDef.____GetDebugAddress()}, CellOwner = '{cellOwner}') from Database, but it was in BankerEntity's Lists.").Write();
                    await CreateBankAccount(bankDef, cell.Guid);
                }

                return cell;
            }
            else
            {
                var cellRef = await CreateBankAccount(bankDef);
                cells.Cells.Add(cellOwner, cellRef);

                return cellRef;
            }
        }

        public async Task DestroyBankCellsImpl(BankDef bankDef, PropertyAddress corpseInventoryAddress)
        {
            try
            {
                if (BankCells.TryGetValue(bankDef, out IBankHolder cells))
                {
                    var moveTransactions = new List<ItemMoveAllManagementTransaction>();
                    foreach (KeyValuePair<OuterRef, OuterRef> cell in cells.Cells)
                    {
                        var cellRef = cell.Value;

                        using (var wrapper = await EntitiesRepository.Get(cellRef.TypeId, cellRef.Guid))
                        {
                            var cellEntity = wrapper.Get<IHasInventoryServer>(cellRef.TypeId, cellRef.Guid, ReplicationLevel.Server);
                            var cellInventoryAddress = EntityPropertyResolver.GetPropertyAddress(cellEntity.Inventory);

                            var moveInventoryItemTransaction = new ItemMoveAllManagementTransaction(cellInventoryAddress, corpseInventoryAddress, false, false, EntitiesRepository);
                            moveTransactions.Add(moveInventoryItemTransaction);
                        }
                    }

                    foreach (var transaction in moveTransactions)
                    {
                        var result = await transaction.ExecuteTransaction();
                        if (!result.IsSuccess)
                        {
                            Logger.IfError()?.Message($"Can't move item on transaction = {transaction.ToString()}").Write();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.IfError()?.Exception(ex).Write();
            }
        }

        private async Task<OuterRef> CreateBankAccount(BankDef bankDef, Guid? cellId = null)
        {
            var id = cellId.HasValue ? cellId.Value : Guid.NewGuid();
            var cellRef = await EntitiesRepository.Create<IBankCell>(id, async createdBankCell =>
            {
                createdBankCell.Inventory.Size = bankDef.InitialSize;
            });

            var outerRef = cellRef.OuterRef;
            if (!outerRef.IsValid)
            {
                Logger.IfError()?.Message($"Could not create bank Cell (BankDef = {bankDef.____GetDebugAddress()}).").Write();
            }

            return new OuterRef(outerRef.Guid, outerRef.TypeId);
        }
    }
}
