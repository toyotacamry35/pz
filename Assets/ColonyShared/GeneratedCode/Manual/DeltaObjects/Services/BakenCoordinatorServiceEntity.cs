using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class BakenCoordinatorServiceEntity : IHookOnStart
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task OnStart()
        {
            EntitiesRepository.UserDisconnected += EntitiesRepository_UserDisconnected;
            return Task.CompletedTask;
        }

        private async Task EntitiesRepository_UserDisconnected(Guid repoId)
        {
            using (var wrapper = await EntitiesRepository.Get<IBakenCoordinatorServiceEntity>(Id))
            {
                var coordinatorServer = wrapper.Get<IBakenCoordinatorServiceEntity>(Id);
                if (coordinatorServer == null)
                {
                    Logger.IfWarn()?.Message($"Missing {nameof(BakenCoordinatorServiceEntity)} on user disconnect userId:{repoId} bakenCoordId:{Id}").Write();
                    return;
                }
                await coordinatorServer.OnUserDisconnected(repoId);
            }
        }

        public async ValueTask CheckForLoadImpl(Guid charId)
        {
            if (charId == Guid.Empty)
                throw new ArgumentException("CharId is empty");

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(charId))
            {
                if (wrapper.TryGet<IBakenCharacterEntityServer>(charId, out var _))
                    return;
            }

            var eref = await EntitiesRepository.Load<IBakenCharacterEntity>(charId);
            if (eref == null)
            {
                eref = await EntitiesRepository.Create<IBakenCharacterEntity>(charId, (e) => Task.CompletedTask);
                Logger.IfInfo()?.Message("Baken character created {0}", charId).Write();
            }
            else
                Logger.IfInfo()?.Message("Baken character loaded {0}", charId).Write();
        }

        public async ValueTask CheckForUnloadImpl(Guid charId)
        {
            if (charId == Guid.Empty)
                throw new ArgumentException("CharId is empty");

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(charId))
            {                
                if (!wrapper.TryGet<IBakenCharacterEntityServer>(charId, out var bakenEntity))
                {
                    Logger.IfError()?.Message("Baken character is not loaded {0}", charId).Write();
                    return; // Already Unloaded
                }

                if (!await bakenEntity.CanBeUnloaded())
                    return;
            }
            Logger.IfInfo()?.Message("Unloading IBakenCharacterEntity {0}", charId).Write();
            await EntitiesRepository.Destroy<IBakenCharacterEntity>(charId);
        }

        public async ValueTask OnUserConnectedImpl(Guid charGuid, Guid clientRepoId)
        {
            if (charGuid == Guid.Empty)
                throw new ArgumentException("CharId is empty");
            if (clientRepoId == Guid.Empty)
                throw new ArgumentException("clientRepoId is empty");

            await CheckForLoad(charGuid);
            await EntitiesRepository.SubscribeReplication(ReplicaTypeRegistry.GetIdByType(typeof(IBakenCharacterEntity)), charGuid, clientRepoId, ReplicationLevel.ClientFull);

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(charGuid))
            {
                var bakenEntity = wrapper.Get<IBakenCharacterEntityServer>(charGuid);
                await bakenEntity.SetLogin(true);
            }
        }
        public async ValueTask OnUserDisconnectedImpl(Guid repoId)
        {
            if (!CharacterByRepoId.TryGetValue(repoId, out var charId))
                return;

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(charId))
            {
                var coordinatorServer = wrapper.Get<IBakenCharacterEntityServer>(charId);
                await coordinatorServer.SetLogin(false);
            }

            await CheckForUnload(charId);
        }

        public async ValueTask RegisterBakenImpl(Guid characterId, Guid masterRepoId, OuterRef<IEntity> bakenRef)
        {
            if (CharacterByBakenId.ContainsKey(bakenRef.Guid))
            {
                Logger.IfError()?.Message("Baken is already registered {0}", bakenRef.Guid).Write();
                return;
            }

            await CheckForLoad(characterId);

            CharacterByBakenId.Add(bakenRef.Guid, characterId);

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(characterId))
            {
                var bakenChar = wrapper?.Get<IBakenCharacterEntityServer>(characterId);
                if (bakenChar == null)
                    Logger.IfError()?.Message("Baken character is not loaded {0}", characterId).Write();
                else
                    await bakenChar.RegisterBaken(bakenRef, true);
            }

            await EntitiesRepository.SubscribeReplication(ReplicaTypeRegistry.GetIdByType(typeof(IBakenCharacterEntity)), characterId, masterRepoId, ReplicationLevel.ClientFull);
        }

        public async ValueTask BakenIsDestroyedImpl(Guid characterId, OuterRef<IEntity> bakenRef)
        {
            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(characterId))
            {
                var bakenChar = wrapper?.Get<IBakenCharacterEntityServer>(characterId);
                if (bakenChar == null)
                    Logger.IfError()?.Message("Baken character is not loaded {0}", characterId).Write();
                else
                    await bakenChar.BakenIsDestroyed(bakenRef);
            }

            await CheckForUnload(characterId);
        }

        public async ValueTask UnregisterBakenImpl(OuterRef<IEntity> bakenRef)
        {
            if (!CharacterByBakenId.TryGetValue(bakenRef.Guid, out var characterId))
            {
                Logger.IfError()?.Message("Baken is not registered {0}", bakenRef.Guid).Write();
                return;
            }

            CharacterByBakenId.Remove(bakenRef.Guid);

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(characterId))
            {
                var bakenChar = wrapper?.Get<IBakenCharacterEntityServer>(characterId);
                if (bakenChar == null)
                    Logger.IfError()?.Message("Baken character is not loaded {0}", characterId).Write();
                else
                    await bakenChar.RegisterBaken(bakenRef, false);
            }

            await CheckForUnload(characterId);
        }

        public async Task SetCharacterLoadedImpl(Guid characterId, bool loaded)
        {
            if (loaded)
            {
                using (await EntitiesRepository.Get<IBakenCoordinatorServiceEntity>(Id))
                {
                    await CheckForLoad(characterId);
                }
            }

            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(characterId))
            {
                var bakenChar = wrapper.Get<IBakenCharacterEntityServer>(characterId);
                if (bakenChar == null)
                    Logger.IfError()?.Message("Baken character is not loaded {0}", characterId).Write();
                else
                    await bakenChar.SetCharacterLoaded(loaded);
            }
            if (loaded)
                await EntitiesRepository.SubscribeReplication(ReplicaTypeRegistry.GetIdByType(typeof(IBakenCharacterEntity)), characterId, GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId, ReplicationLevel.ClientFull);
            else
            {
                using (await EntitiesRepository.Get<IBakenCoordinatorServiceEntity>(Id))
                {
                    await CheckForUnload(characterId);
                }
            }
        }

        public async Task<bool> ActivateBakenImpl(Guid characterId, OuterRef<IEntity> bakenRef)
        {
            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(characterId))
            {
                var coordinatorServer = wrapper.Get<IBakenCharacterEntityServer>(characterId);
                return await coordinatorServer.ActivateBaken(bakenRef);
            }
        }

        public async Task<OuterRef<IEntity>> GetActiveBakenImpl(Guid characterId)
        {
            using (var wrapper = await EntitiesRepository.Get<IBakenCharacterEntityServer>(characterId))
            {
                var coordinatorServer = wrapper.Get<IBakenCharacterEntityServer>(characterId);
                if (coordinatorServer != null)
                    return coordinatorServer.ActiveBaken;
            }
            return OuterRef<IEntity>.Invalid;
        }
    }
}
