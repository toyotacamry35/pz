using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using GeneratedCode.Transactions;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.MapSystem;
using Assets.ColonyShared.SharedCode.Entities;
using SharedCode.Refs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using Core.Environment.Logging.Extension;
using GeneratedCode.MapSystem;
using SharedCode.MovementSync;
using SharedCode.Repositories;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class CorpseSpawner : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("CorpseSpawner");

        // --- IHookOnInit implementation: ---------------------------------------------------

        public Task OnInit()
        {
            return Initialization();
        }

        public Task OnDatabaseLoad()
        {
            return Initialization();
        }

        private Task Initialization()
        {
            var mortal = parentEntity as IHasMortal;
            if (mortal == null)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(ICorpseSpawner)}. Can't get entity as {nameof(IHasMortal)}. [ May be Ok (PointsOfInterest e.g.) ]").Write();
                return Task.CompletedTask;
            }

            mortal.Mortal.DieEvent += SpawnCorpse;
            return Task.CompletedTask;
        }

        // --- ICorpseSpawnerWithImplementation: ---------------------------------------------------

        public async Task SpawnCorpseImpl(Guid entityId, int entityTypeId, PositionRotation corpsePlace)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"SpawnCorpse: worldObj.Position: {corpsePlace}").Write();

            IEntityObjectDef corpseEntityDef = ((ICorpseSpawnerDef)((IEntityObject)parentEntity).Def).CorpseEntityDef.Target;
            Guid guid = Guid.NewGuid();
            var repo = EntitiesRepository;

            var spawnedType = (corpseEntityDef is InteractiveEntityDef)
                ? DefToType.GetEntityType(typeof(CorpseInteractiveEntityDef))  ///#PZ-14283 tmp hack, while GD don't change all corpses to mineables (т.к. иначе нужно во всех jdb трупов изменить тип на `CorpseInteractiveEntityDef`, но д/этого нужно понимать, какие из jdb такого типа трупы, а какие иные интерактивные сущности)
                : DefToType.GetEntityType(corpseEntityDef.GetType());
            var spawnedTypeId = ReplicaTypeRegistry.GetIdByType(spawnedType);

            //var corpseRef = await repo.Create<ICorpseInteractiveEntity>(guid,
            var corpseRef = await repo.Create(spawnedTypeId, guid, 
            (entity) =>
            {
                var a = entity as IEntityObject;           if (a != null)  a.Def = corpseEntityDef;
                var b = entity as IHasSimpleMovementSync;  if (b != null)  b.MovementSync.SetTransform = new Transform(corpsePlace.Position);
                var c = entity as IHasWorldSpaced;         if (c != null)  c.WorldSpaced.OwnWorldSpace = ((IHasWorldSpaced)parentEntity).WorldSpaced.OwnWorldSpace;
                var d = entity as IScenicEntity;           if (d != null)  d.MapOwner = ((IHasMapped)parentEntity).MapOwner;
                var e = entity as IHasOwner;               if (e != null)  e.OwnerInformation.Owner = new OuterRef<IEntity>(entityId, entityTypeId);

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Corpse IInteractiveEntity cre-ed at {(entity as IHasSimpleMovementSync)?.MovementSync.Transform.Position}, entityDef: {corpseEntityDef}.").Write();
                return Task.CompletedTask;
            });

            return;
            // All farther is no need for mobs any more: -----------------------------------

            PropertyAddress corpseInventoryAddress = null;
            var hasDollEntity = parentEntity as IHasDoll;
            if (hasDollEntity != null)
            {
                PropertyAddress corpseDollAddress = null;
                using (var wrapper = await repo.Get(corpseRef.TypeId, corpseRef.Id))
                {
                    var entity = wrapper.Get<ICorpseInteractiveEntityServer>(corpseRef.Id);
                    if (entity == null)
                    {
                        Logger.IfError()?.Message("Failed to create corpse").Write();
                        return;
                    }

                    corpseDollAddress = EntityPropertyResolver.GetPropertyAddress(entity.Doll);
                    corpseInventoryAddress = EntityPropertyResolver.GetPropertyAddress(entity.Inventory);
                }

                var moveTransactions = new List<ItemMoveManagementTransaction>();
                List<RemoveItemBatchElement> removeItems = new List<RemoveItemBatchElement>();

                foreach (var item in hasDollEntity.Doll.Items)
                {
                    var calcerOnDeath = (item.Value.Item.ItemResource as ItemResource)?.ActionOnDeathCalcer.Target;
                    ActionOnDeathDef actionOnDeath = Constants.ItemConstants.MoveToCorpse.Target;
                    if (calcerOnDeath != null)
                    {
                        actionOnDeath = await calcerOnDeath.CalcAsync(new OuterRef<IEntity>(ParentEntityId, ParentTypeId), repo) as ActionOnDeathDef;
                    }

                    if (actionOnDeath == Constants.ItemConstants.LeaveAtCharacter)
                        continue;

                    var sourceAddress = EntityPropertyResolver.GetPropertyAddress(hasDollEntity.Doll);
                    if (actionOnDeath == Constants.ItemConstants.Destroy)
                    {
                        removeItems.Add(new RemoveItemBatchElement(sourceAddress, item.Key, item.Value.Stack, item.Value.Item.Id));
                    }
                    else if (actionOnDeath == Constants.ItemConstants.MoveToCorpse)
                    {
                        var moveInventoryItemTransaction = new ItemMoveManagementTransaction(sourceAddress, item.Key, corpseDollAddress, item.Key, item.Value.Stack, item.Value.Item.Id, false, repo);
                        moveTransactions.Add(moveInventoryItemTransaction);
                    }
                }

                var removeTransaction = new ItemRemoveBatchManagementTransaction(removeItems, false, repo);
                var removeResult = await removeTransaction.ExecuteTransaction();
                if (!removeResult.IsSuccess)
                    Logger.IfError()?.Message($"Can't remove item on transaction = {removeTransaction.ToString()}").Write();

                foreach (var transaction in moveTransactions)
                {
                    var result = await transaction.ExecuteTransaction();
                    if (!result.IsSuccess)
                    {
                        Logger.IfError()?.Message($"Can't move item on transaction = {transaction.ToString()}").Write();
                    }
                }
            }

            if (corpseInventoryAddress != null)
            {
                var bankEntity = parentEntity as IBank;
                if (bankEntity != null && bankEntity.Bank?.BankDef != null && corpseInventoryAddress != null)
                {
                    OuterRef<IEntity> bankerRef;
                    using (var wrapper = await repo.GetFirstService<IBankServiceEntityServer>())
                    {
                        var service = wrapper.GetFirstService<IBankServiceEntityServer>();
                        bankerRef = await service.GetBanker();
                    }

                    EntityRef<IBankerEntity> loadedBanker = await repo.Load<IBankerEntity>(bankerRef.Guid) ?? await repo.Create<IBankerEntity>(bankerRef.Guid);
                    using (var wrapper = await repo.Get(loadedBanker.TypeId, loadedBanker.Id))
                    {
                        var bankerEntity = wrapper.Get<IBankerEntityServer>(loadedBanker.TypeId, loadedBanker.Id, ReplicationLevel.Server);
                        await bankerEntity.DestroyBankCells(bankEntity.Bank?.BankDef, corpseInventoryAddress);
                    }
                }
            }
        }

    }
}
