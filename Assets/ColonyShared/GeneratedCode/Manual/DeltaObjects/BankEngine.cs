using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Refs;
using System;
using System.Threading.Tasks;
using ResourceSystem.Utils;
using SharedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class BankEngine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<OuterRef<IEntity>> OpenBankCellImpl()
        {
            return await GetBankCell((openedBankCell, clientId) =>
            {
                return EntitiesRepository.SubscribeReplication(openedBankCell.TypeId, openedBankCell.Guid, clientId, ReplicationLevel.ClientFull);
            });
        }

        public async Task<OuterRef<IEntity>> CloseBankCellImpl()
        {
            return await GetBankCell((openedBankCell, clientId) =>
            {
                return EntitiesRepository.UnsubscribeReplication(openedBankCell.TypeId, openedBankCell.Guid, clientId, ReplicationLevel.ClientFull);
            });
        }

        private async Task<OuterRef<IEntity>> GetBankCell(Func<OuterRef, Guid, Task> action)
        {
            if (BankDef == null)
                return default;

            OuterRef<IEntity> bankerRef;
            using (var wrapper = await EntitiesRepository.GetFirstService<IBankServiceEntityServer>())
            {
                var service = wrapper.GetFirstService<IBankServiceEntityServer>();
                bankerRef = await service.GetBanker();
            }

            AccountData characterData;
            var clientId = GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId;
            using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
            {
                var service = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                characterData = await service.GetAccountDataByUserId(clientId);
            }

            var cellOwner = new OuterRef(characterData.AccountId, ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)));
            OuterRef openedBankCell;
            EntityRef<IBankerEntity> loadedBanker = await EntitiesRepository.Load<IBankerEntity>(bankerRef.Guid) ?? await EntitiesRepository.Create<IBankerEntity>(bankerRef.Guid);
            using (var wrapper = await EntitiesRepository.Get(loadedBanker.TypeId, loadedBanker.Id))
            {
                var bankerEntity = wrapper.Get<IBankerEntityServer>(loadedBanker.TypeId, loadedBanker.Id, ReplicationLevel.Server);
                openedBankCell = await bankerEntity.GetBankCell(BankDef, cellOwner);
            }

            await action(openedBankCell, clientId);

            return new OuterRef<IEntity>(openedBankCell.Guid, openedBankCell.TypeId);
        }
    }
}
